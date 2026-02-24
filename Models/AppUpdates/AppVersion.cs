using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class AppVersion
{
	[Key]
	public int Id { get; set; }
	public string Version { get; set; } = null!;
	public string FilePath { get; set; } = null!;
	public DateTime UploadedAt { get; set; }
}