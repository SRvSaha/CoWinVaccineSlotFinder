using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;

namespace CoWin.Core.Models
{
    public class Notifier
    {
        public void Notify(string message)
        {
            try
            {
                var botClient = new TelegramBotClient(Crypto.Decrypt(TelegramBotModel.SECRET, TelegramBotModel.ENCRYPTED_ACCESS_TOKEN));
            
                var output = botClient.SendTextMessageAsync(TelegramBotModel.CHANNEL_CHAT_ID, message, Telegram.Bot.Types.Enums.ParseMode.Markdown).Result;
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] Issue faced in Telegram API {e}");
                Console.ResetColor();
            }
        }
    }
}
