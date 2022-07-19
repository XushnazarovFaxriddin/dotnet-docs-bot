using AngleSharp;
using AngleSharp.Dom;
using DotNetUzBot.Models;
using DotNetUzBot.Services;
using Telegram.Bot;

static class Program
{
    static TelegramBotClient botClient;

    [Obsolete]
    static async Task Main()
    {
        botClient = new TelegramBotClient("BOT-TOKENI");
        BotService botService = new BotService(botClient);
        botClient.OnMessage += botService.OnMessageAsync!;
        botClient.OnCallbackQuery += botService.InlineCallBack!;
        botClient.StartReceiving();
        Console.WriteLine("Bot ishga tushdi!");

        Console.ReadKey();
    }
}
