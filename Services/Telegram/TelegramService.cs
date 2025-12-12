using AnydeskTracker.Data;
using AnydeskTracker.Models;
using Microsoft.AspNetCore.Identity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AnydeskTracker.Services;

public class TelegramService(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<TelegramService> logger)
{
	public readonly TelegramBotClient Client = new(Environment.GetEnvironmentVariable("TG_BOT_KEY") ?? "");
	private readonly bool SendTelegramNotifications = Environment.GetEnvironmentVariable("SEND_TG_NOTIFICATIONS") != "false"; 

	public async Task HandleUpdate(Update update)
	{
		if (update.Type != UpdateType.Message || update.Message!.Text == null)
			return;
		
		logger.Log(LogLevel.Information, "Handling update {update}", update.Message.Text);

		var chatId = update.Message.Chat.Id;
		var userMessage = update.Message.Text;

		if (userMessage.StartsWith("/start"))
		{
			var parts = userMessage.Split(' ');
			if (parts.Length > 1)
			{
				var userId = parts[1];
				var user = await context.Users.FindAsync(userId);
				if (user != null)
				{
					user.TelegramChatId = chatId;
					user.TelegramUserName = update.Message.Chat.Username ?? "";
					await context.SaveChangesAsync();
					await SendMessageAsync(chatId, $"✅ Telegram успешно привязан к пользователю {user.Email}!");
					logger.Log(LogLevel.Information, "Added chat id {chatId} to user {userId}", update.Message.Text, userId);
				}
			}
		}
	}

	public async Task<bool> SendMessageToAdmin(string message)
	{
		var adminUsers = await userManager.GetUsersInRoleAsync("Admin");
		var admin = adminUsers.FirstOrDefault();
		
		if(admin == null)
			return false;

		if (admin.TelegramChatId == 0)
			return false;

		return await SendMessageAsync(admin.TelegramChatId, message);
	}
	
	public async Task<bool> SendMessageAsync(string userId, string message)
	{
		var user = await context.Users.FindAsync(userId);
	    
		if(user == null)
			return false;

		await SendMessageAsync(user.TelegramChatId, message);
		return true;
	}

	public async Task<bool> SendMessageAsync(long chatId, string message)
	{
		if (!SendTelegramNotifications)
			return false;

		try
		{
			await Client.SendMessage(chatId, message, ParseMode.Html);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ошибка при отправке сообщения Telegram пользователю {chatId}", chatId);
			return false;
		}

		return true;
	}
}