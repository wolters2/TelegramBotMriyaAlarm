using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotMriyaAlarm
{
    class Program
    {
        private static TelegramBotClient Bot;
        public static async Task Main()
        {
            Bot = new TelegramBotClient(Configuration.BotToken);

            var me = await Bot.GetMeAsync();
            Console.Title = me.Username;
            //string file = "1.wav";
            //Process.Start(@"powershell", $@"-c (New-Object Media.SoundPlayer '{file}').PlaySync();");
            //using (var waveOut = new WaveOutEvent())
            //using (var wavReader = new WaveFileReader("1.wav"))
            //{
            //    waveOut.Init(wavReader);
            //    waveOut.Play();
            //}
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");
            System.IO.File.AppendAllText("log.txt", $"Start listening for @{me.Username}"+Environment.NewLine);
            while (true)
            {

            }
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            switch (message.Text.Split(' ').First())
            {
                // Send inline keyboard
                case "/start":
                    await SendInlineKeyboard(message);
                    break;
                case "/about":
                    await About(message);
                    break;
                //// send custom keyboard
                //case "/keyboard":
                //    await SendReplyKeyboard(message);
                //    break;

                //// send a photo
                //case "/photo":
                //    await SendDocument(message);
                //    break;

                //// request location or contact
                //case "/request":
                //    await RequestContactAndLocation(message);
                //    break;
                //case "1": Console.WriteLine("1");
                //    break;

                default:
                    await Usage(message);
                    break;
            }

            // Send inline keyboard
            // You can process responses in BotOnCallbackQueryReceived handler
            //await SendInlineKeyboard(message);
            //await Usage(message);
            //await About(message);
            //  async Task Usage(Message message)
            //{
            //    const string usage = "Usage:\n" +
            //                            "/start     - Надіслати саовіщення в бот\n" +
            //                            "/about  - Про бота та контакти";
            //    await Bot.SendTextMessageAsync(
            //        chatId: message.Chat.Id,
            //        text: usage,
            //        replyMarkup: new ReplyKeyboardRemove()
            //    );
            //}
            // async Task About(Message message)
            //{
            //    const string about = "Розроблено HotFix.in.ua (CrySoft)";
            //    await Bot.SendTextMessageAsync(
            //        chatId: message.Chat.Id,
            //        text: about,
            //        replyMarkup: new ReplyKeyboardRemove()
            //    );
            //}
        }
        public static async Task SendInlineKeyboard(Message message)
        {
            await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            // Simulate longer running task
            await Task.Delay(500);

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                    // first row
                    new []
                    {
                        //InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        //InlineKeyboardButton.WithCallbackData("1.2", "12"),
                        InlineKeyboardButton.WithCallbackData("Тривога", "Тривога о "+ DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")),
                        InlineKeyboardButton.WithCallbackData("Відбій", "Відбій о "+ DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")),
                    //},
                    //// second row
                    //new []
                    //{
                    //    InlineKeyboardButton.WithCallbackData("2.1", "21"),
                    //    InlineKeyboardButton.WithCallbackData("2.2", "22"),
                    }
                });
            await Bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Виберіть:",
                replyMarkup: inlineKeyboard
            );
        }
        public static async Task Usage(Message message)
        {
            const string usage = "Usage:\n" +
                                    "/start     - Надіслати сповіщення в бот\n" +
                                    "/about  - Про бота та контакти";
            await Bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: usage,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }
        public static async Task About(Message message)
        {
            const string about = "Розроблено HotFix.in.ua (CrySoft)";
            await Bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: about,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        // Process Inline Keyboard callback data
        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await Bot.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}"
            );

            await Bot.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: $"Отримано {callbackQuery.Data} від {callbackQuery.From}"
            );
            Console.WriteLine($"Отримано {callbackQuery.Data} від {callbackQuery.From}");
            System.IO.File.AppendAllText("log.txt", $"Отримано {callbackQuery.Data} від {callbackQuery.From}" + Environment.NewLine);
            if (callbackQuery.Data.Contains("Тривога"))
            {
                Console.WriteLine("Play Alarm");
                System.IO.File.AppendAllText("log.txt", "Play Alarm" + Environment.NewLine);
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Files\1.wav");
                player.Play();
                //using (var waveOut = new WaveOutEvent())
                //using (var wavReader = new WaveFileReader("Files/1.wav"))
                //{
                //    waveOut.Init(wavReader);
                //    waveOut.Play();
                //}
                //string file = "Files/1.wav";
                //Process.Start(@"powershell", $@"-c (New-Object Media.SoundPlayer '{file}').PlaySync();");
            }
            else if (callbackQuery.Data.Contains("Відбій"))
            {
                Console.WriteLine("Play End Of Alarm");
                System.IO.File.AppendAllText("log.txt", "Play End Of Alarm" + Environment.NewLine);
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Files\2.wav");
                player.Play();
                //using (var waveOut = new WaveOutEvent())
                //using (var wavReader = new WaveFileReader("Files/2.wav"))
                //{
                //    waveOut.Init(wavReader);
                //    waveOut.Play();
                //}
                //string file = "Files/2.wav";
                //Process.Start(@"powershell", $@"-c (New-Object Media.SoundPlayer '{file}').PlaySync();");
            }
        }

        #region Inline Mode

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");
            System.IO.File.AppendAllText("log.txt", $"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}" + Environment.NewLine);
            InlineQueryResultBase[] results = {
                // displayed result
                new InlineQueryResultArticle(
                    id: "3",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent(
                        "hello"
                    )
                )
            };
            await Bot.AnswerInlineQueryAsync(
                inlineQueryId: inlineQueryEventArgs.InlineQuery.Id,
                results: results,
                isPersonal: true,
                cacheTime: 0
            );
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
            System.IO.File.AppendAllText("log.txt", $"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}" + Environment.NewLine);
        }

        #endregion

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
            System.IO.File.AppendAllText("log.txt", "Received error: " + receiveErrorEventArgs.ApiRequestException.ErrorCode + " — " + receiveErrorEventArgs.ApiRequestException.Message + Environment.NewLine);

        }
    }
}
