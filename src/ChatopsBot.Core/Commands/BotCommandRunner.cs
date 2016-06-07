using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatopsBot.Commands;
using ChatopsBot.Core.Util;
using Microsoft.Bot.Connector;

namespace ChatopsBot.Core.Commands
{
    public class BotCommandRunner
    {

        public static async Task<Command> RunCommand(SetCommand command, MessageMeta meta, BuildBotState state)
        {
            if (!string.IsNullOrWhiteSpace(command.Project))
            {
                command.Title += "-default";
                command.Output.Add($"set default project for {meta.FromName} to '{command.Project}'");
                state.DefaultProject = command.Project;
            }
            else if (!string.IsNullOrWhiteSpace(command.TfsUser))
            {
                command.Title += "-tfsuser";
                command.Output.Add($"set tfs user for {meta.FromName} to '{command.TfsUser}'");
                state.TfsUser = command.TfsUser;
            }
            command.ExecutingSuccess = true;

            return await Task.FromResult((Command)command);
        }

        public static async Task<Command> RunCommand(StateCommand command, MessageMeta meta, BuildBotState state)
        {
            if (command.Clear)
            {
                command.Title = "clear settings for {0}";
                state.DefaultProject = null;
                state.TfsUser = null;
                command.Output.Add("done - all clear!");
            }
            else
            {
                command.Output.Add($"default project = '{state.DefaultProject}'");
                command.Output.Add($"tfs user = '{state.TfsUser}'");

                command.Output.Add("");

                var message = meta.Message;
                var user = message.From;
                var bot = message.To;

                command.Output.Add($"user.name={user.Name}");
                command.Output.Add($"user.address={user.Address}");
                command.Output.Add($"user.id={user.Id}");
                command.Output.Add($"user.channelid={user.ChannelId}");
                command.Output.Add($"user.isbot={user.IsBot}");
                command.Output.Add("");

                command.Output.Add($"bot.name={bot.Name}");
                command.Output.Add($"bot.address={bot.Address}");
                command.Output.Add($"bot.id={bot.Id}");
                command.Output.Add($"bot.channelid={bot.ChannelId}");
                command.Output.Add("");

                command.Output.Add($"conversationid={message.ConversationId}");
                command.Output.Add($"channelconversationid={message.ChannelConversationId}");

            }

            command.ExecutingSuccess = true;

            return await Task.FromResult((Command)command);
        }
    }
}