using System.Text;
using System.Text.RegularExpressions;
using AnydeskTracker.Data;
using AnydeskTracker.Models;
using HtmlAgilityPack;

namespace AnydeskTracker.Services;

public class ParserService(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
{
	private readonly HttpClient httpClient = httpClientFactory.CreateClient("Beget");

	private const int FetchPageCount = 15;
	
	public async Task FetchBlockedCredentials(CancellationToken stoppingToken = default)
	{
		context.BlockedPhoneNumbers.RemoveRange(context.BlockedPhoneNumbers);
		context.BlockedEmails.RemoveRange(context.BlockedEmails);

		await context.SaveChangesAsync(stoppingToken);

		var phoneNumbers = new List<string>();
		var emails = new List<string>();

		for (int i = 0; i < FetchPageCount; i++)
		{
			string[] nodes = await FetchBlockedCredentialsPage(i);

			phoneNumbers.AddRange(nodes
				.Where(x => x.All(char.IsDigit))); 
				
			emails.AddRange(nodes
				.Where(x => Regex.IsMatch(x, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,}$")));
		}
			
		context.BlockedPhoneNumbers.AddRange(
			phoneNumbers
				.Distinct()
				.Select(x => new BlockedAgentPhone
				{
					Phone = x
				}));
			
		context.BlockedEmails.AddRange(emails
			.Distinct()
			.Select(x => new BlockedAgentEmail()
			{
				Email = x
			}));
			
		await context.SaveChangesAsync(stoppingToken);
	}
	
	private async Task<string[]> FetchBlockedCredentialsPage(int pageNumber)
	{
		using var content = new StringContent(
			"action=blacklist_n" +
			"&query=bl_cat%255B%255D%3Dall%26bl_new_ts%255B%255D%3Dtel%26bl_new_ts%255B%255D%3Dsite" +
			$"&page={pageNumber}",
			Encoding.UTF8,
			"application/x-www-form-urlencoded"
		);
			
		var response = await httpClient.PostAsync("https://moshelovka.onf.ru/wp-admin/admin-ajax.php", content);
			
		var html = await response.Content.ReadAsStringAsync();
			
		var doc = new HtmlDocument();
		doc.LoadHtml(html);

		return doc.DocumentNode
			.SelectNodes("//div[@class='bl_one_name']")
			.Select(x => x.InnerHtml.Trim())
			.ToArray();
	}
}