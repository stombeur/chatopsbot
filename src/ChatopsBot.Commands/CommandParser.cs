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
        public static IEnumerable<string> SplitCommandLine(string commandLine)
        {
            bool inQuotes = false;

            return commandLine.Split(c =>
            {
                if (c == '\"')
                    inQuotes = !inQuotes;

                return !inQuotes && c == ' ';
            })
                              .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                              .Where(arg => !string.IsNullOrEmpty(arg));
        }

        public static Command ParseCommand(string textToParse)
        {
            var args = SplitCommandLine(textToParse); //textToParse.Split(new[] { ' ' }, StringSplitOptions.None);
            var helpString = new StringBuilder();
            var parser = new Parser(config => config.HelpWriter = new StringWriter(helpString));

            var result = parser
                .ParseArguments
                <ProjectCommand, AliasCommand,
                     StateCommand, BuildCommand, SetCommand>(args)
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

    }
}
