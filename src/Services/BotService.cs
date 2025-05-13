using Telegram.Bot;
using Telegram.Bot.Types;
using System.Text.Json;

namespace Services;

public class BotService
{
    private readonly TelegramBotClient _telegramBotClient;
    private readonly GeminiService _geminiService;
    private readonly DatabaseService _databaseService;

    public BotService(string telegramToken, string geminiApiKey)
    {
        _telegramBotClient = new TelegramBotClient(telegramToken);
        _geminiService = new GeminiService(geminiApiKey);
        _databaseService = new DatabaseService();
    }

    [Obsolete]
    public void Start()
    {
        _databaseService.Initialize();
        _telegramBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync);

        Console.WriteLine("Bot is running...");
        Console.ReadLine();
    }

    [Obsolete]
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        var message = update.Message;
        if (message?.Text == null) return;
        if (message.From == null) return;
        var userId = message.From.Id.ToString();
        var text = message.Text.Trim().ToUpper();

        if (text.StartsWith("РОЗКЛАД"))
        {
            var parts = text.Split(' ', 2);
            var schoolClass = parts.Length > 1 ? parts[1] : "";
            var lessons = _databaseService.GetScheduleLessons(schoolClass);
            var response = ScheduleFormatter.Format(lessons);
            await _telegramBotClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: response,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
            );
            return;
        }
;
        var fullPrompt = MessageFormatter.FormatPrompt(message.Text);

        var answer = await _geminiService.Call(fullPrompt);
        
        var json = answer.Replace("```json", "").Replace("```", "").Trim();

        var result = JsonSerializer.Deserialize<Result>(json);
            if (result is null)
            {
                await _telegramBotClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "The schedule could not be processed. Please try again.");
                return;
            }
            if (!string.IsNullOrEmpty(result.Error))
            {
                await _telegramBotClient.SendTextMessageAsync(
                    message.Chat.Id,
                    result.Error);
                return;
            }
            _databaseService.Save(userId, result.Schedules);

        var confirmation = MessageFormatter.SendAnswer(result.Schedules);
        await _telegramBotClient.SendTextMessageAsync(message.Chat.Id, confirmation);
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
    {
        Console.WriteLine($"Bot error: {exception.Message}");
        return Task.CompletedTask;
    }
}