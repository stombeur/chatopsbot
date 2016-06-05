using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using ChatopsBot.Commands;
using ChatopsBot.Core.Commands;
using ChatopsBot.Core.Util;
using CommandLine;
using Microsoft.Bot.Connector;

namespace ChatopsBot.Core.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {


        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {


            if (message.Type == "Message")
            {
                var metaMessage = MessageMeta.SanitizeMessage(message);

                var state = BuildBotState.CreateState(message);

                var command = await DefaultCommandRunner.ParseCommand(metaMessage)
                                                        .AndRunCommandIfParsingSuccessfull(metaMessage, state)
                                                        .AndTryAliasIfParsingUnsuccessfull(metaMessage, state)
                                                        .AndFormatHelpIfParsingUnsuccessfull(metaMessage, state);

                //var command = DefaultCommandRunner.ParseCommand(cleanMessage.SanitizedText);

                //if (command.ParsingSuccess) command = await DefaultCommandRunner.RunCommand((dynamic) command, message, state);
                //else
                //{
                //    //check if it's an alias
                //    var alias =
                //        state.Aliases.FirstOrDefault(
                //            a => a.Name.Equals(cleanMessage.SanitizedText.Trim(), StringComparison.CurrentCultureIgnoreCase));
                //    if (alias != null)
                //    {
                //        var command2 = DefaultCommandRunner.ParseCommand("run-alias " + alias.Name);
                //        if (command2.ParsingSuccess) command2 = await DefaultCommandRunner.RunCommand((dynamic)command2, message, state);
                //        if (command2.ExecutingSuccess) command = command2;
                //        else
                //        {
                //            command.Output.Add($"   tried running the command as alias '{alias.Name}'");
                //            command.Output.AddRange(command2.Output.Select(l => "   "+l));
                //        }
                //    }
                //}

                //if (!command.ParsingSuccess)
                //{
                //    command.IsHelp = cleanMessage.IsHelp;
                //    command.Title = command.IsHelp ? "Help" : "Error";
                //    if (!command.IsHelp)
                //        command.Output.Add($"error while parsing >>{command.Input}<<"); //and botName={botName} and original text >>{message.Text}<< and textToParse >>{textToParse}<<");
                //    command.Output.Add($"{Constants.EnvironmentNewLine}{Constants.EnvironmentNewLine}help           display a list of commands");
                //    command.Output.Add($"help xyz       display detailed info for the command xyz");
                //}


                IReplyMessageFormatter formatter = (metaMessage.IsSlack ? (IReplyMessageFormatter)new SlackFormatter() : (IReplyMessageFormatter)new DefaultFormatter());

                var returnMessage = message.CreateReplyMessage(formatter.Format(command, metaMessage));
                returnMessage.ChannelData = formatter.FormatAttachments(command, metaMessage);
                returnMessage.BotUserData = state;

                return returnMessage;
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }


        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
            }

            return null;
        }
    }
}