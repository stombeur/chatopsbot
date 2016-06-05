using System;
using System.Linq;
using System.Threading.Tasks;
using ChatopsBot.Commands;
using ChatopsBot.Core.Util;

namespace ChatopsBot.Core.Commands
{
    public static class CommandExtensions
    {
        public static async Task<Command> AndTryAliasIfParsingUnsuccessfull(this Task<Command> commandTask, MessageMeta message, BuildBotState state)
        {
            var command = await commandTask;

            if (command.ParsingSuccess) return command;

            var alias =
                state.Aliases.FirstOrDefault(
                    a => a.Name.Equals(command.Input.Trim(), StringComparison.CurrentCultureIgnoreCase));
            if (alias != null)
            {
                var command2 = DefaultCommandRunner.ParseCommand("alias --run --name " + alias.Name);
                if (command2.ParsingSuccess)
                    command2 = await DefaultCommandRunner.RunCommand((dynamic) command2, message, state);
                if (command2.ExecutingSuccess) return command2;
                else
                {
                    command.Output.Add($"   tried running the command as alias '{alias.Name}'");
                    command.Output.AddRange(command2.Output.Select(l => "   " + l));
                }
            }

            return command;
        }

        public static async Task<Command> AndRunCommandIfParsingSuccessfull(this Command command, MessageMeta message,
            BuildBotState state)
        {
            if (command.ParsingSuccess)
            {
                return await DefaultCommandRunner.RunCommand((dynamic)command, message, state);
            }
            return command;
        }

        public static async Task<Command> AndFormatHelpIfParsingUnsuccessfull(this Task<Command> commandTask, MessageMeta meta,
            BuildBotState state)
        {
            var command = await commandTask;

            if (command.ParsingSuccess) return command;

            command.IsHelp = meta.IsHelp;
            command.Title = command.IsHelp ? "Help" : "Error";
            if (!command.IsHelp)
                command.Output.Add($"error while parsing >>{command.Input}<<"); //and botName={botName} and original text >>{message.Text}<< and textToParse >>{textToParse}<<");
            command.Output.Add($"{Constants.EnvironmentNewLine}{Constants.EnvironmentNewLine}help           display a list of commands");
            command.Output.Add($"help xyz       display detailed info for the command xyz");

            return command;
        }

    }
}