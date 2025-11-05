using System.ComponentModel.DataAnnotations;

namespace AnydeskTracker.DTOs;

public class AdminUserUpdateDto
{
	[Required]
	public string UserName { get; set; } = "";
}