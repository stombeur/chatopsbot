using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Utilities;
using Newtonsoft.Json;

namespace ChatopsBot.Core
{
    public class BuildBotState
    {
        public string CurrentProjectId { get; set; }
    }

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
                var state = new BuildBotState();

                if (message.BotUserData != null)
                {
                    state.CurrentProjectId = message.GetBotUserData<string>("currentProjectId");
                }

                var replyMessage = "";

                if (message.Text.ToLower() == "help")
                {
                    var reply = DisplayHelpMessage();


                    replyMessage = reply;
                }

                if (message.Text.ToLower() == "whoami")
                {
                    var reply = "";
                    var a = message.From;

                    reply += $"name={a.Name} \n\n";
                    reply += $"address={a.Address} \n\n";
                    reply += $"id={a.Id} \n\n";
                    reply += $"conversationid={message.ConversationId} \n\n";
                    reply += $"channelconversationid={message.ChannelConversationId} \n\n";


                    replyMessage = reply;

                }

                if (message.Text.ToLower() == "state")
                {
                    //state.CurrentProjectId = message.GetBotUserData<string>("currentProjectId");
                    replyMessage = DisplayState(state);
                }

                if (message.Text.StartsWith("set project"))
                {
                    var projectid = message.Text.ToLower().Split(' ').Last();
                    state.CurrentProjectId = projectid;

                    replyMessage = DisplayState(state);

                }

                if (message.Text.ToLower() == "list projects")
                {
                    var projects = await VSTSClient.GetProjects();
                    var reply = "";
                    foreach (var p in projects)
                    {
                        reply += $"{p.Name} - projectguid={p.Id:N} \n\n";
                    }
                    replyMessage = reply;

                }

                if (message.Text.ToLower().StartsWith("list builds"))
                {
                    var split = message.Text.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    var projectid = "";
                    if (split.Length == 3)
                    {
                        projectid = split.Last();
                    }
                    else if (split.Length == 2)
                    {
                        projectid = state.CurrentProjectId;
                    }

                    if (string.IsNullOrWhiteSpace(projectid))
                       replyMessage = "could not get projectid from state or message";
                    else
                    {

                        var builds = await VSTSClient.GetBuildDefinitions(projectid);
                        var reply = $"listing builds for projectid={projectid}\n\n";
                        foreach (var p in builds)
                        {
                            reply += $"{p.Name} - buildid={p.Id} \n\n";
                        }
                        replyMessage = reply;
                    }
                }

                if (message.Text.ToLower().StartsWith("queue build"))
                {
                    var split = message.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var projectid = "";
                    var buildId = -1;
                    if (split.Length == 4)
                    {
                        projectid = split.Last();
                        buildId = Convert.ToInt32(message.Text.Split(' ').Reverse().Skip(1).Take(1).First());
                    }
                    else if (split.Length == 3)
                    {
                        projectid = state.CurrentProjectId;
                        buildId = Convert.ToInt32(message.Text.Split(' ').Last());
                    }


                    if (string.IsNullOrWhiteSpace(projectid))
                        replyMessage = "could not get projectid from state or message";
                    else if (buildId < 0)
                        replyMessage = "could not get buildId from state or message";
                    else
                    {
                        var reply = $"build queued with buildId={buildId} projectId={projectid}";
                        try
                        {
                            var build = await VSTSClient.StartBuild(buildId, projectid);
                            reply += $" -> build number {build.BuildNumber}";
                        }
                        catch (Exception ex)
                        {
                            reply += "/n/n - there was an ERROR - " + ex;
                        }
                        replyMessage = reply;
                    }
                }

                //replyMessage.BotUserData = state;
                if (string.IsNullOrWhiteSpace(replyMessage))
                { replyMessage =
                    $"I'm sorry, {message.From.Name}, my vocabulary is limited. Type help for a list of commands.";}

                var msg = message.CreateReplyMessage(replyMessage);
                msg.BotUserData = state;
                return msg;
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        private static string DisplayHelpMessage()
        {
            var reply = "";

            reply += $"list projects \n\n";
            reply += $"list builds <projectguid> \n\n";
            reply += $"list builds - [list all build definitions from the current project in state] \n\n";
            reply += $"queue build <buildid> <projectguid> \n\n";
            reply += $"queue build <buildid> - [queue build with id <buildid> from the current project in state]\n\n";
            reply += $"whoami \n\n";
            reply += $"help \n\n";
            reply += $"set project <projectguid> [select current project to be <projectguid> for subsequent commands]\n\n";
            reply += $"state [display your current state]\n\n";

            return reply;
        }

        private static string DisplayState(BuildBotState state)
        {
            var reply = "";

            reply += $"current project = {state.CurrentProjectId} \n";

            return reply;
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