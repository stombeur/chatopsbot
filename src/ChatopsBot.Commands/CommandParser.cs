using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace ChatopsBot.Commands
{
    public class CommandParser
    {
        public static Command ParseCommand(string textToParse)
        {
            var args = textToParse.Split(new[] { ' ' }, StringSplitOptions.None);
            var helpString = new StringBuilder();
            var parser = new Parser(config => config.HelpWriter = new StringWriter(helpString));

            var result = parser
                .ParseArguments
                <WhoamiCommand, WhoamiCommand2, QueueCommand, ListBuildCommand, ListProjectCommand, SetAliasCommand,
                    RunAliasCommand, SetStateCommand, ListStateCommand, CancelBuildCommand>(args)
                .MapResult(
                    (Command opts) => opts,
                    errs => new Command() {ParsingSuccess = false, Output = new List<string>() { helpString.ToString() } });

            result.Args = args;
            result.Input = textToParse;

            if (string.IsNullOrWhiteSpace(result.Title))
            {
                try
                {
                    var a = result.GetType().GetCustomAttributes(typeof(VerbAttribute), false).FirstOrDefault() as VerbAttribute;
                    if (a != null) result.Title = a.Name;
                }
                catch { }
            }

            return result;
        }

        public static Command ParseCommand(string[] args)
        {
            var helpString = new StringBuilder();
            var parser = new Parser(config => config.HelpWriter = new StringWriter(helpString));
            Command command = null;
            IEnumerable<Error> errors = null;

            var result = parser
                .ParseArguments
                <WhoamiCommand, WhoamiCommand2, QueueCommand, ListBuildCommand, ListProjectCommand, SetAliasCommand,
                    RunAliasCommand, SetStateCommand, ListStateCommand, CancelBuildCommand>(args)
                .WithParsed(options => command = options as Command)
                .WithNotParsed(e => errors = e);

            if (result.Tag == ParserResultType.NotParsed)
            {
                command = new Command() {ParsingSuccess = false, Output = new List<string>() {HelpText.AutoBuild(result)} };

                var helpText = HelpText.AutoBuild(result);
                helpText.Copyright = "";
                helpText.Heading = "";
                helpText.AdditionalNewLineAfterOption = false;

                //helpText = helpText.AddOptions(result).AddVerbs(typeof(Command).Assembly.GetTypes().Where(t => t.GetCustomAttributes(typeof(VerbAttribute), true).Length > 0).ToArray());

                command.Output = new List<string>() { helpText.ToString() };
            }
            command.Args = args;
            command.Input = string.Join(" ", args);

            return command;
        }
    }
}
