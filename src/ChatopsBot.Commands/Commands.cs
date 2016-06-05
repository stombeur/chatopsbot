using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace ChatopsBot.Commands
{
    public class Command
    {

        public bool ParsingSuccess { get; set; } = true;
        public bool ExecutingSuccess { get; set; } = false;
        public string[] Args { get; set; }
        public string Input { get; set; }
        public List<string> Output { get; set; } = new List<string>();
        public virtual string Title { get; set; }
        public bool IsHelp { get; set; } = false;
    }

    public class BotCommand : Command { }

    public class VstsCommand : Command { }

    [Verb("who", HelpText = WhoamiCommand.HelpText)]
    public class WhoamiCommand2:WhoamiCommand {}
    [Verb("whoami", HelpText = WhoamiCommand.HelpText)]
    public class WhoamiCommand : BotCommand
    {
        public const string HelpText = "info about the current user and conversation";
        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Example", new WhoamiCommand() { });
            }
        }
    }

    [Verb("list-state", HelpText = "list all the state settings in the current conversation")]
    public class ListStateCommand : BotCommand
    {
        public override string Title => "list-state for {0}";
        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Example", new ListStateCommand() { });
            }
        }
    }

    [Verb("set-state", HelpText = "initialize a state value")]
    public class SetStateCommand : BotCommand
    {
        public override string Title => "set-state for user {0}";
        [Option('p', "project", Required = false, HelpText = "set the current project to be used for subsequent commands")]
        public string Project { get; set; }
        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Example", new SetStateCommand() {Project = Guid.NewGuid().ToString("N")});
            }
        }
    }

    [Verb("run-alias", HelpText = RunAliasCommand.HelpText)]
    public class RunAliasCommand : BotCommand
    {
        public const string HelpText = "run an aliased command. You can also run the alias by just passing the alias without 'run-alias'. The command 'run-alias <aliasName>' is equivalent with the command '<aliasName>'";
        [Option('n', "name", Required = false, HelpText = "")]
        public string Name { get; set; }

        [Value(0)]
        public string Namepos { get; set; }
        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Example", new RunAliasCommand() {Name = "testme"});
                yield return new Example("Example", new RunAliasCommand() { Namepos = "testme" });
            }
        }
    }

    [Verb("set-alias", HelpText = "create an alias for another command")]
    public class SetAliasCommand : BotCommand
    {
        [Option('n', "name", Required = true, HelpText = "the name of the alias")]
        public string Name { get; set; }

        [Option('c', "command", Required = false, HelpText = "the command to alias")]
        public string Command { get; set; }

        [Value(0, HelpText = "the command to alias")]
        public IEnumerable<string> CommandSeq { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Example", new SetAliasCommand() { Name = "lp", Command = "list-projects" });
                yield return new Example("Example", new SetAliasCommand() { Name = "qb2", CommandSeq = "\"queue-build 42\"".Split(' ') });
             }
        }
    }

    [Verb("queue-build", HelpText = "queue a build")]
    public class QueueCommand : VstsCommand
    {
        [Value(0, HelpText = "pass the buildid as the first parameter")]
        public string BuildIdPos { get; set; }

        [Option('i', longName: "id", Required = false, HelpText = "the build id")]
        public string BuildId { get; set; }

        [Option('n', longName: "name", Required = false, HelpText = "the build name")]
        public string BuildName { get; set; }

        [Option("pi", Required = false, HelpText = "the project id")]
        public string ProjectId { get; set; }

        [Option("pn", Required = false, HelpText = "the project name")]
        public string ProjectName { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Example", new QueueCommand { BuildId = "42", ProjectId = Guid.NewGuid().ToString("N") });
                yield return new Example("Example using projectid from state", new QueueCommand { BuildIdPos = "42"});
                yield return new Example("Example using projectid from state", new QueueCommand { BuildId = "42" });


            }
        }
    }

    [Verb("cancel-build", HelpText = "cancel a queued or running build")]
    public class CancelBuildCommand : VstsCommand
    {
        [Value(0, HelpText = "pass the buildid as the first parameter")]
        public string BuildIdPos { get; set; }

        [Option('i', longName: "id", Required = false, HelpText = "the build id")]
        public string BuildId { get; set; }

        [Option('n', longName: "name", Required = false, HelpText = "the build name")]
        public string BuildName { get; set; }

        [Option("pi", Required = false, HelpText = "the project id")]
        public string ProjectId { get; set; }

        [Option("pn", Required = false, HelpText = "the project name")]
        public string ProjectName { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Example", new CancelBuildCommand { BuildId = "42", ProjectId = Guid.NewGuid().ToString("N") });
                yield return new Example("Example using projectid from state", new CancelBuildCommand { BuildIdPos = "42" });
                yield return new Example("Example using projectid from state", new CancelBuildCommand { BuildId = "42" });
            }
        }
    }

    [Verb("list-builds", HelpText = "list all builds in a project. If no project is specified, we use the one in state.")]
    public class ListBuildCommand : VstsCommand
    {

        [Value(0, HelpText = "the project id passed as the first positional parameter")]
        public string ProjectIdPos { get; set; }

        [Option('i', "id", Required = false, HelpText = "the project id")]
        public string ProjectId { get; set; }

        [Option('n', "name", Required = false, HelpText = "the project name")]
        public string ProjectName { get; set; }
        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Example", new ListBuildCommand() { ProjectId = Guid.NewGuid().ToString("N") });
                yield return new Example("Example", new ListBuildCommand() { ProjectIdPos = Guid.NewGuid().ToString("N") });
                yield return new Example("Example using projectid from state", new ListBuildCommand() { });
            }
        }
    }

    [Verb("list-projects", HelpText = "list all projects")]
    public class ListProjectCommand : VstsCommand
    {
        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Example", new ListProjectCommand() { });
            }
        }
    }
}
