using System.ComponentModel.DataAnnotations;
using AnydeskTracker.Extensions;
using AnydeskTracker.Models;

namespace AnydeskTracker.DTOs;

public class PcDto
{
	public int Id { get; set; }

	[Display(Name = "AnyDesk ID")]
	public string AnyDeskId { get; set; }

	[Display(Name = "Пароль")]
	public string Password { get; set; }

	[Display(Name = "Статус")]
	public PcStatus Status { get; set; }

	public string StatusText => Status.GetDisplayName();

	[Display(Name = "Последнее изменение статуса")]
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