using AngleSharp.Dom;
using AngleSharp;
using DotNetUzBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using static System.Net.WebRequestMethods;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace DotNetUzBot.Services
{
    public class BotService
    {
        private readonly TelegramBotClient botClient;

        private IDocument document;
        private string source;
        private HttpClient http;
        private IDocsService docsService;
        private ReplyKeyboardMarkup markup;
        private IBrowsingContext context;
        public BotService(TelegramBotClient botClient)
        {
            this.botClient = botClient;
            docsService = new DocsService();
            context = BrowsingContext.New(Configuration.Default);
            http = new HttpClient();
            this.markup = new ReplyKeyboardMarkup();
            markup.ResizeKeyboard = true;
        }
        public async void OnMessageAsync(object sender, MessageEventArgs e)
        {

            long userId = e.Message.Chat.Id;
            int msgId = e.Message.MessageId;
            if (e.Message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
            {
                await botClient.SendTextMessageAsync(userId,
                    "Kechirasiz bu bot hozircha faqat matnli xabarlar bilan ishlaydi.",
                    replyToMessageId: msgId);
                return;
            }
            string msg = e.Message.Text;
            if (msg == "/start")
            {
                await botClient.SendTextMessageAsync(userId, "Assalomu alaykum", replyToMessageId: msgId);
                markup.Keyboard = new KeyboardButton[][]
                {
                    new KeyboardButton[]
                    {
                        new KeyboardButton("C# va .NET")
                    },
                    new KeyboardButton[]
                    {
                        new KeyboardButton("JavaScript"),
                        new KeyboardButton("C++")
                    }
                };
                await botClient.SendTextMessageAsync(
                     chatId: e.Message.Chat.Id,
                     text: "menu",
                     replyMarkup: markup
                 );
                return;
            }
            if (msg == "C# va .NET")
            {
                var markup = new InlineKeyboardMarkup(
                    new InlineKeyboardButton[][]
                    {
                        new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton
                                .WithCallbackData(text: "Starter (boshlang'ich)",
                                callbackData: "Starter")
                        },
                        new InlineKeyboardButton[]
                        {
                            InlineKeyboardButton
                                .WithCallbackData(text: "Essential (muhim)",
                                callbackData: "Essential"),
                            InlineKeyboardButton
                                .WithCallbackData(text: "Professional",
                                callbackData: "Professional")
                        }
                    });

                await botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat.Id,
                    text: "Quyidagi bo'limlardan birini tanlang!",
                    replyMarkup: markup,
                    replyToMessageId: msgId);
                return;
            }

            await botClient.SendTextMessageAsync(userId, "Kechirasiz botda bunday xizmat mavjud emas!", replyToMessageId: msgId);
        }
        public async void InlineCallBack(object sender, CallbackQueryEventArgs e)
        {
            string queryData = e.CallbackQuery.Data;
            long queryFormId = e.CallbackQuery.From.Id;
            if (queryData == "Starter" || queryData == "Essential" || queryData == "Professional")
            {
                try
                {
                    source = await http.GetStringAsync(Const.Domen + queryData switch
                    {
                        "Starter" => Const.Starter,
                        "Essential" => Const.Essential,
                        _ => Const.Professional
                    });
                    document = await context.OpenAsync(req => req.Content(source));
                    var docs = docsService.GetBasicDocsList(document, source);
                    IList<InlineKeyboardButton[]> inlineKeyboardButtons = new List<InlineKeyboardButton[]>();
                    foreach (var doc in docs)
                        inlineKeyboardButtons.Add(new InlineKeyboardButton[]{
                        InlineKeyboardButton
                                .WithCallbackData(text: doc.Name,
                                callbackData: doc.Url) });

                    var markup = new InlineKeyboardMarkup(
                        inlineKeyboardButtons);

                    await botClient.SendTextMessageAsync(
                        chatId: queryFormId,
                        text: "Quyidagi bo'limlardan birini tanlang!",
                        replyMarkup: markup);
                    return;
                }
                catch
                {
                    await botClient.SendTextMessageAsync(queryFormId, "Kechirasiz nomalum xatolik sodir bo'ldi!");
                }
            }
            if (queryData.Contains("c-.net/"))
            {
                try { 
                    var url = Const.Domen.Substring(0, Const.Domen.Length - 1) + queryData;
                    var html = await http.GetStringAsync(url);
                    document= await context.OpenAsync(req => req.Content(html));
                    document.GetElementsByClassName("css-1dbjc4n r-11c0sde")[1].InnerHtml = "";
                    document.GetElementsByClassName("css-1dbjc4n r-1ro0kt6 r-16y2uox r-1wbh5a2 r-puj83k")[1].InnerHtml = "";
                    document.GetElementsByClassName("css-1dbjc4n r-1p0dtai r-u8s1d r-13qz1uu r-184en5c")[0].InnerHtml = "";
                    document.GetElementsByClassName("css-1dbjc4n r-k200y r-pw2am6 r-icyqz7 r-1mkpi1y r-1rnoaur r-1h4fu65 r-gtdqiz r-eqo98v")[0].InnerHtml = "";
                    html=document.ToHtml();
                    //var path = docsService.HtmlToFileStream(html);
                    //using (var stream = System.IO.File.OpenRead(path))
                        await botClient.SendDocumentAsync(queryFormId,
                            document: new InputOnlineFile(content: GenerateStreamFromString(html), fileName: $"{e.CallbackQuery.Data.Split('/','\\').Last()}.html"),
                            caption: "@dotnet_uz_bot"
                            );
                    //System.IO.File.Delete(path);
                    await botClient.SendTextMessageAsync(queryFormId, e.CallbackQuery.Message.Text
                        );
                }
                catch
                {
                    await botClient.SendTextMessageAsync(queryFormId, "Kechirasiz nomalum xatolik sodir bo'ldi!");
                }
                return;
            }
            await botClient.SendTextMessageAsync(queryFormId, "Kechirasiz botda hali bunday xizmat mavjud emas!");
        }
        public Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
