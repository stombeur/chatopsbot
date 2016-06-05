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
        public static async Task<Command> RunCommand(WhoamiCommand command, MessageMeta meta, BuildBotState state)
        {
            var message = meta.Message;
            var a = message.From;

            command.Output.Add($"name \t\t\t\t\t {a.Name}");
            command.Output.Add($"address \t\t\t\t\t {a.Address}");
            command.Output.Add($"id \t\t\t\t\t {a.Id}");
            command.Output.Add($"conversationid \t\t\t\t\t {message.ConversationId}");
            command.Output.Add($"channelconversationid \t\t\t\t\t {message.ChannelConversationId}");

            command.ExecutingSuccess = true;

            return await Task.FromResult((Command)command);
        }

        public static async Task<Command> RunCommand(SetStateCommand command, MessageMeta message, BuildBotState state)
        {
            command.Output.Add($"set current project to '{command.Project}'");
            state.CurrentProjectId = command.Project;

            command.ExecutingSuccess = true;

            return await Task.FromResult((Command)command);
        }

        public static async Task<Command> RunCommand(ListStateCommand command, MessageMeta message, BuildBotState state)
        {
            command.Output.Add($"current project = '{state.CurrentProjectId}'");
            command.Output.Add($"known aliases:");
            if (state.Aliases != null)
            {
                foreach (var alias in state.Aliases)
                {
                    command.Output.Add($"   {alias.Name} = {alias.Command}");
                }
            }
            else if (state.Aliases == null || state.Aliases.Count == 0)
                command.Output.Add($"   none");

            command.ExecutingSuccess = true;

            return await Task.FromResult((Command)command);
        }

        public static async Task<Command> RunCommand(SetAliasCommand command, MessageMeta message, BuildBotState state)
        {
            var commandText = command.Command ?? string.Join(" ", command.CommandSeq);
            if (commandText.StartsWith("\"") && commandText.EndsWith("\""))
            {
                commandText = commandText.Substring(1, commandText.Length - 2);
            }

            if(state.Aliases == null) state.Aliases = new List<CommandAlias>();
            var existing = state.Aliases.FirstOrDefault(a => String.Equals(a.Name, command.Name, StringComparison.CurrentCultureIgnoreCase));
            if (existing != null) existing.Command = commandText;
            else state.Aliases.Add(new CommandAlias() {Name = command.Name, Command = commandText });

            command.Output.Add($"created alias with name '{command.Name}' for command '{commandText}'");

            command.ExecutingSuccess = true;

            return await Task.FromResult((Command)command);
        }


    }
}