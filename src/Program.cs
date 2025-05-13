using Services;

var bot = new BotService(
    telegramToken: "8005973788:AAGfwiLSH5h9CWezQzIVv9JyEagp4SqaYc0",
    geminiApiKey: "AIzaSyBPgGywY3mExz566O1vNaOFjTQ9LflHk9Y"
);

bot.Start();
