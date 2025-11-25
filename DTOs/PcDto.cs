using AnydeskTracker.Extensions;
using AnydeskTracker.Models;

namespace AnydeskTracker.DTOs;

public class PcDto
{
	public int Id { get; set; }

	public string AnyDeskId { get; set; }

	public string Password { get; set; }

	public PcStatus Status { get; set; }

	public string StatusText => Status.GetDisplayName();
	
	public DateTime LastStatusChange { get; set; }
	
	public PcDto(PcModel model)
	{
		Id = model.Id;
		AnyDeskId = model.PcId;
		Password = model.Password;
		Status = model.Status;
		LastStatusChange = model.LastStatusChange.ToUtc();
	}
}