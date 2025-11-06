using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AnydeskTracker.DTOs;

public class AdminGoogleSheetDto
{
	[Required]
	public string SheetUrl { get; set; }

	[Required]
	public string SheetName { get; set; }
	
	public string SheetId {
		get
		{
			var match = Regex.Match(SheetUrl, @"\/d\/([a-zA-Z0-9-_]+)");
			
			if (!match.Success)
				return string.Empty;

			return match.Groups[1].Value;
		}
	}
}