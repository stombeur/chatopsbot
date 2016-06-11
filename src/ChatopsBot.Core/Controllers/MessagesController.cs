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
                var metaMessage = MessageMeta.DigestMessage(message);

                var state = BuildBotState.CreateState(message);

                var command = await DefaultCommandRunner.ParseCommand(metaMessage)
                                                        .AndRunCommandIfParsingSuccessfull(metaMessage, state)
                                                        .AndTryAliasIfParsingUnsuccessfull(metaMessage, state)
                                                        .AndFormatHelpIfParsingUnsuccessfull(metaMessage, state);

                IReplyMessageFormatter formatter = (metaMessage.IsSlack ? (IReplyMessageFormatter)new SlackFormatter() : (IReplyMessageFormatter)new DefaultFormatter());

                var returnMessage = message.CreateReplyMessage(formatter.Format(command, metaMessage, state));
                returnMessage.ChannelData = formatter.FormatAttachments(command, metaMessage, state);
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