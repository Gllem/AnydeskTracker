using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using AnydeskTracker.Data;
using AnydeskTracker.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Controllers.Bots;

[Authorize(Roles = "Admin")]
[Route("api/admin/bots/updates")]
[ApiController]
public class ApiAdminBotUpdatesController(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment) : ControllerBase
{
	[HttpGet("catalog")]
	public async Task<IActionResult> GetAllVersions()
	{
		var appVersions = await dbContext.AppVersions.OrderByDescending(x => x.UploadedAt).ToListAsync();

		return Ok(appVersions.Select(x => new
		{
			x.Version,
			UploadedAt = x.UploadedAt.ToUtc(),
			Url = Url.Action(nameof(BotWatchdogApiController.Download), "BotWatchdogApi", new { version = x.Version }, Request.Scheme)
		}));
	}

	[RequestSizeLimit(250_000_000)]
	[RequestFormLimits(MultipartBodyLengthLimit = 250_000_000)]
	[HttpPost("upload")]
	public async Task<IActionResult> Upload(IFormFile file)
	{
		if (!file.FileName.EndsWith(".exe"))
			return BadRequest("Only .exe files are allowed.");

		var updatesEnvVar = Environment.GetEnvironmentVariable("APP_UPDATES_FOLDER");

		if (updatesEnvVar == null)
			return StatusCode(500, "Invalid Updates Path");
		
		var updatesDirPath = Path.Combine(webHostEnvironment.ContentRootPath, updatesEnvVar);
		var tempDirPath = Path.Combine(updatesDirPath, "temp");
		
		Directory.CreateDirectory(tempDirPath);

		var tempFilePath = Path.Combine(tempDirPath, file.FileName);
		using var sha256 = SHA256.Create();

		await using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
		await using (var cryptoStream = new CryptoStream(fileStream, sha256, CryptoStreamMode.Write))
		{
			await file.CopyToAsync(cryptoStream);
		}

		string hash = BitConverter
			.ToString(sha256.Hash!)
			.Replace("-", "")
			.ToLowerInvariant();
		
		var finalPath = Path.Combine(updatesDirPath, $"{hash}.exe");
		if (!System.IO.File.Exists(finalPath))
			System.IO.File.Move(tempFilePath, finalPath);
		else
		{
			System.IO.File.Delete(tempFilePath);
			
			return BadRequest("File already exists.");
		}

		var version = GetExeVersion(finalPath);

		var appVersion = new AppVersion
		{
			Version = version,
			FilePath = finalPath,
			UploadedAt = DateTime.UtcNow
		};

		dbContext.AppVersions.Add(appVersion);
		await dbContext.SaveChangesAsync();

		await ClearOldVersions();

		return Ok(new { version });
	}

	private async Task ClearOldVersions()
	{
		var versions = await dbContext.AppVersions.OrderBy(x => x.UploadedAt).ToListAsync();

		var versionCount = versions.Count;
		
		var updatesEnvVar = Environment.GetEnvironmentVariable("APP_UPDATES_FOLDER");

		if (updatesEnvVar == null)
			return;

		var updatesPath = Path.Combine(webHostEnvironment.ContentRootPath, updatesEnvVar);

		int index = 0;
		for (int i = versionCount; i > 3; i--, index++)
		{
			var path = Path.Combine(updatesPath, versions[index].FilePath);
			
			if(System.IO.File.Exists(path))
				System.IO.File.Delete(path);

			dbContext.AppVersions.Remove(versions[index]);
		}

		await dbContext.SaveChangesAsync();
	}
	
	private static string GetExeVersion(string filePath)
	{
		var info = FileVersionInfo.GetVersionInfo(filePath);
		return info.ProductVersion!;
	}
}