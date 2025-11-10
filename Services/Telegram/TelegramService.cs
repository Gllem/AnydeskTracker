using AnydeskTracker.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AnydeskTracker.Services;

public class TelegramService(ApplicationDbContext context, ILogger<TelegramService> logger)
{
	public readonly TelegramBotClient Client = new(Environment.GetEnvironmentVariable("TG_BOT_KEY") ?? "");

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
					await context.SaveChangesAsync();
					await SendMessageAsync(chatId, "✅ Telegram успешно привязан!");
					logger.Log(LogLevel.Information, "Added chat id {chatId} to user {userId}", update.Message.Text, userId);
				}
			}
		}
	}
	
	public async Task SendMessageAsync(string userId, string message)
	{
		var user = await context.Users.FindAsync(userId);
	    
		if(user == null)
			return;

		await SendMessageAsync(user.TelegramChatId, message);
	}

	public async Task SendMessageAsync(long chatId, string message)
	{
		try
		{
			await Client.SendMessage(chatId, message, ParseMode.Html);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ошибка при отправке сообщения Telegram пользователю {chatId}", chatId);
		}
	}
}