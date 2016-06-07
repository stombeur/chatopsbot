using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ChatopsBot.Commands;
using ChatopsBot.Core.Controllers;
using ChatopsBot.Core.Util;
using Microsoft.Bot.Connector;

namespace ChatopsBot.Core.Commands
{
    public class DefaultCommandRunner
    {
        public static Command ParseCommand(MessageMeta meta)
        {
            return CommandParser.ParseCommand(meta.SanitizedText);
        }

        public static Command ParseCommand(string text)
        {
            return CommandParser.ParseCommand(text);
        }

        public static async Task<Command> RunCommand(BotCommand command, MessageMeta message, BuildBotState state)
        {
            return await BotCommandRunner.RunCommand((dynamic)command, message, state);
        }

        public static async Task<Command> RunCommand(AliasCommand command, MessageMeta message, BuildBotState state)

        {
            if (!command.Validate()) return command;

            try
            {
                if (command.Create)
                {
                    command.Title += "-create";

                    var commandText = command.Command ?? string.Join(" ", command.CommandSeq);
                    if (commandText.StartsWith("\"") && commandText.EndsWith("\""))
                    {
                        commandText = commandText.Substring(1, commandText.Length - 2);
                    }

                    if (state.Aliases == null) state.Aliases = new List<CommandAlias>();
                    var existing =
                        state.Aliases.FirstOrDefault(
                            a => String.Equals(a.Name, command.Name, StringComparison.CurrentCultureIgnoreCase));
                    if (existing != null)
                    {
                        existing.Command = commandText;
                        command.Output.Add($"replaced alias with name '{command.Name}' for command '{commandText}'");
                    }
                    else
                    {
                        state.Aliases.Add(new CommandAlias() {Name = command.Name, Command = commandText});
                        command.Output.Add($"created alias with name '{command.Name}' for command '{commandText}'");
                    }

                    command.ExecutingSuccess = true;


                }
                else if (command.Run)
                {
                    command.Title += "-run";
                    var aliasName = command.Name;
                    var alias = state.Aliases?.FirstOrDefault(a => a.Name.ToLower() == aliasName);
                    if (alias == null)
                    {
                        command.Output.Add($"could not find alias '{aliasName}'");
                        return command;
                    }

                    var commandToRun = ParseCommand(alias.Command);
                    if (commandToRun.ParsingSuccess) return await RunCommand((dynamic) commandToRun, message, state);

                    return await Task.FromResult((Command) commandToRun);
                }
                else if (command.List)
                {
                    command.Title += "-list";
                    command.Output.Add($"known aliases:");
                    if (state.Aliases != null)
                    {
                        foreach (var a in state.Aliases)
                        {
                            command.Output.Add($"   {a.Name} = {a.Command}");
                        }
                    }
                    else if (state.Aliases == null || state.Aliases.Count == 0)
                        command.Output.Add($"   none");

                    command.ExecutingSuccess = true;
                }
                else if (command.Clear)
                {
                    command.Title += "-clear";
                    command.Output.Add($"removing all known aliases");
                    state.Aliases = new List<CommandAlias>();
                    command.Output.Add($"done - all clear!");

                    command.ExecutingSuccess = true;
                }
            }
            catch (Exception ex)
            {
                command.Output.Add("   there was an ERROR - " + ex.Message);
            }

            return await Task.FromResult((Command)command);
        }

        public static async Task<Command> RunCommand(VstsCommand command, MessageMeta message, BuildBotState state)
        {
            return await VstsCommandRunner.RunCommand((dynamic)command, message, state);
        }
    }
}