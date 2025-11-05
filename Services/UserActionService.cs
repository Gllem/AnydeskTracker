using System;
using System.Threading.Tasks;
using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AnydeskTracker.Services
{
	public class UserActionService(
		ApplicationDbContext context)
	{
		public async Task LogAsync(WorkSessionModel workSession, ActionType actionType, string? description = null)
		{
			var action = new UserAction
			{
				UserId = workSession.UserId,
				WorkSessionId = workSession.Id,
				ActionType = actionType,
				Description = description,
				Timestamp = DateTime.UtcNow
			};

			context.UserActions.Add(action);
			await context.SaveChangesAsync();
		}
	}
}