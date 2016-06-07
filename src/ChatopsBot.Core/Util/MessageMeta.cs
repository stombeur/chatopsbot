using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Bot.Connector;

namespace ChatopsBot.Core.Util
{
    public class MessageMeta
    {
        public Message Message { get; set; }
        public bool HasMentionOfBot { get; set; }
        public string BotName { get; set; } = "--BOT--";
        public string FromName { get; set; }
        public string ReplyHeader { get; set; }
        public string SanitizedText { get; set; }
        public bool IsHelp { get; set; }
        public bool IsSlack { get; set; }

        public static MessageMeta DigestMessage(Message message)
        {
            var result = new MessageMeta();
            result.Message = message;

            //is it slack?
            result.IsSlack = (message.To.ChannelId == "slack");

            result.FromName = message.From.Name;

            var inputText = message.Text;
            var botId = "";

            //remove bot name from start of message
            if (message.Mentions != null &&
                    message.Mentions.Any(
                        m => m.Mentioned.IsBot.HasValue && m.Mentioned.IsBot.Value && m.Mentioned.Id == botId))
            {
                result.HasMentionOfBot = true;

                result.BotName =
                    message.Mentions.First(
                        m => m.Mentioned.IsBot.HasValue && m.Mentioned.IsBot.Value && m.Mentioned.Id == botId).Text;

                //remove 'botname ' or 'botname: ' from the message
                inputText = inputText.Replace(result.BotName + ": ", "");
                inputText = inputText.Replace(result.BotName + " ", "");
                inputText = inputText.Replace(result.BotName, "");
            }

            //remove unwanted space at the start
            inputText = inputText.TrimStart();

            //remove funny html quotes
            inputText = inputText.Replace("“", "\"").Replace("”", "\"");

            //remove funny double dashes
            inputText = inputText.Replace("—", "--");

            //remove url formatting
            //Build <http://tombeur.be|tombeur.be>
            // <.*\|{1}(.*)>{1}
            var regex = new Regex(@"<.*\|{1}(.*)>{1}");
            var match = regex.Match(inputText);
            if (match.Success && match.Groups.Count > 1)
                inputText = inputText.Replace(match.Value, match.Groups[1].Value);
            //inputText = regex.Replace(inputText, match => match.Value);

            //if someone addresses the bot to start a conversation, just show help
            if (inputText.Trim().Equals("hi", StringComparison.CurrentCultureIgnoreCase)
                    || inputText.Trim().Equals("hello", StringComparison.CurrentCultureIgnoreCase)
                    || inputText.Trim().Equals("hallo", StringComparison.CurrentCultureIgnoreCase)
                    || inputText.Trim().Equals("hey", StringComparison.CurrentCultureIgnoreCase)
                    || inputText.Trim().Equals("hey there", StringComparison.CurrentCultureIgnoreCase)
                    || inputText.Trim().Equals("hi there", StringComparison.CurrentCultureIgnoreCase)
                    || inputText.Trim().Equals("goeiemorgen", StringComparison.CurrentCultureIgnoreCase)
                    || inputText.Trim().Equals("good morning", StringComparison.CurrentCultureIgnoreCase))
            {
                inputText = "help";
                result.ReplyHeader =
                    $"hi {message.From.Name}, I'm {message.To.Name} and I'm here to help. I understand these commands";
            }

            if (inputText.StartsWith("help", StringComparison.CurrentCultureIgnoreCase)) result.IsHelp = true;

            result.SanitizedText = inputText;
            return result;
        }
    }
}