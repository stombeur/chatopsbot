using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace ChatopsBot.Commands
{
    public class Command
    {

        public bool ParsingSuccess { get; set; } = true;
        public bool ExecutingSuccess { get; set; } = false;
        public IEnumerable<string> Args { get; set; }
        public string Input { get; set; }
        public List<string> Output { get; set; } = new List<string>();
        public string Title { get; set; }
        public bool IsHelp { get; set; } = false;
        public virtual bool Validate()
        {
            return false;
        }
    }

    public class BotCommand : Command { }

    public class VstsCommand : Command { }


    [Verb("build", HelpText = "Start or Cancel a build. List all available builds. Type 'help build' for more info.")]
    public class BuildCommand : VstsCommand
    {
        [Option("id", Required = false, HelpText = "the build id or name")]
        public string BuildId { get; set; }

        [Option("config", Required = false, HelpText = "override the default build configuration")]
        public string Config { get; set; }

        [Value(0, HelpText = "pass the build id or name as the first parameter")]
        public string BuildIdPos { get; set; }

        [Option("start", Required = false, HelpText = "start a build (the default switch if none is specified)")]
        public bool Start { get; set; }

        [Option("list", Required = false, HelpText = "list all builds for a project")]
        public bool List { get; set; }

        [Option("cancel", Required = false, HelpText = "cancel all builds")]
        public bool Cancel { get; set; }

        [Option("queue", Required = false, HelpText = "show all builds in the queue (inprogress or notstarted)")]
        public bool Queue { get; set; }

        [Option("project", Required = false, HelpText = "the project id or name")]
        public string ProjectId { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Build project with id=42 with projectid", new BuildCommand {Start = true, BuildId = "42", ProjectId = Guid.NewGuid().ToString("N") });
                yield return new Example("Build project with id=42 with projectid", new BuildCommand { Start = true, BuildIdPos = "42", ProjectId = Guid.NewGuid().ToString("N") });
                yield return new Example("Build project with id=42 using projectid from state", new BuildCommand { Start = true, BuildIdPos = "42" });
                yield return new Example("Build project with id=42 using projectid from state", new BuildCommand { BuildId = "42" });
                yield return new Example("Build project with id=42 with BuildConfiguration=Debug", new BuildCommand { BuildId = "42", Config = "Debug"});
                yield return new Example("List all knpwn builds using projectid from state", new BuildCommand {List = true,  });
                yield return new Example("Cancel all builds with id=42 using projectid from state", new BuildCommand { Cancel = true, BuildId = "42" });
                yield return new Example("Cancel all builds with id=42 using projectid from state", new BuildCommand { Cancel = true, BuildIdPos = "42" });
                yield return new Example("List all builds with projectid", new BuildCommand { List = true, ProjectId = Guid.NewGuid().ToString("N") });
                yield return new Example("List all builds using projectid from state", new BuildCommand { List = true });
            }
        }

        public override bool Validate()
        {
            var result = true;

            var all = new[] {Start, Cancel, List, Queue};

            //start = default
            if (!all.Any(b => b)) Start = true;

            if (all.Count(b => b) > 1)
            {
                Output.Add($"you cannot choose more than one from [Start({Start}), Cancel({Cancel}), List({List})]");
                result = false;
            }

            var buildIdString = BuildIdPos ?? BuildId;
            if (String.IsNullOrWhiteSpace(buildIdString) && (Start || Cancel))
            {
                Output.Add("could not get build id or name");
                result = false;
            }

            return result;
        }
    }


    [Verb("state", HelpText = "list all the state settings in the current conversation")]
    public class StateCommand : BotCommand
    {
        public StateCommand()
        {
            Title = "conversation state for {0}";
        }

        [Option("clear", Required = false, HelpText = "clear state settings")]
        public bool Clear { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Show all your state settings", new StateCommand() { });
                yield return new Example("Clear all your state settings", new StateCommand() {Clear = true });

            }
        }

        public override bool Validate()
        {
            return true;
        }
    }

    [Verb("set", HelpText = "add state settings in the current conversation")]
    public class SetCommand : BotCommand
    {

        [Option("tfsuser", Required = false, HelpText = "set your tfs username")]
        public string TfsUser { get; set; }

        [Option("project", Required = false, HelpText = "set your default tfs project")]
        public string Project { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Set your tfs user name", new SetCommand() { TfsUser = "me@my.com"});
                yield return new Example("Set your default project", new SetCommand() { Project = Guid.NewGuid().ToString("N")});
            }
        }

        public override bool Validate()
        {
            return true;
        }
    }

    [Verb("project", HelpText = "List all available projects. Type 'project help' for more info.")]
    public class ProjectCommand : VstsCommand
    {

        [Option("list", Required = false, HelpText = "list available projects (the default switch if none is specified)")]
        public bool List { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {

            get
            {
                yield return new Example("List available projects", new ProjectCommand() {List = true });
                yield return new Example("List available projects (--list is default)", new ProjectCommand() { });

            }
        }

        public override bool Validate()
        {
            var result = true;

            var all = new[] { List };

            //list is default
            if (!all.Any(b => b)) List = true;

            if (all.Count(b => b) > 1)
            {
                Output.Add($"you cannot choose more than one from [List({List})]");
                result = false;
            }

            return result;
        }
    }

    [Verb("alias", HelpText = "Create an alias for another command. Run an aliased command. List all known aliases. Type 'help alias' for more info.")]
    public class AliasCommand : Command
    {

        [Option("name", Required = false, HelpText = "the name of the alias")]
        public string Name { get; set; }

        [Option("command", Required = false, HelpText = "the aliased command")]
        public string Command { get; set; }

        [Value(0, HelpText = "the aliased command can also be passed without the --command switch")]
        public IEnumerable<string> CommandSeq { get; set; }

        [Option("create", Required = false, HelpText = "create a new alias or update an existing one (this is the default switch if none is specified). Requires you to also specify a name and command")]
        public bool Create { get; set; }

        [Option("run", Required = false, HelpText = "run an aliased command. You can also run the alias by just passing the alias without 'alias --run'. The command 'alias --run <aliasName>' is equivalent with the command '<aliasName>'")]
        public bool Run { get; set; }

        [Option("list", Required = false, HelpText = "list all known aliases")]
        public bool List { get; set; }

        [Option("clear", Required = false, HelpText = "clear all known aliases")]
        public bool Clear { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Create or update an alias with the --command switch", new AliasCommand() { Name = "lp", Command = "project --list" });
                yield return new Example("Create or update an alias. Everything after the name of the alias is the command", new AliasCommand() { Name = "qb2", CommandSeq = new[] { "build --start 42" } });
                yield return new Example("Create or update an alias", new AliasCommand() { Name = "qb2", CommandSeq = new [] { "build 42"} });
                yield return new Example("Run an alias. This is equivalent to simply typing 'my-alias'", new AliasCommand() { Run = true, Name = "my-alias" });
                yield return new Example("List all known aliases", new AliasCommand() { List = true });

            }
        }

        public override bool Validate()
        {
            var result = true;

            var all = new[] {Create, Run, List, Clear};

            //create is default
            if (!all.Any(b => b)) Create = true;

            if (all.Count(b => b) > 1)
            {
                Output.Add($"you cannot choose more than one from [Create({Create}), Run({Run}), List({List}), Clear({Clear})]");
                result = false;
            }

            //if create and no name or no command
            if (Create &&
                (String.IsNullOrWhiteSpace(Name) ||
                 (String.IsNullOrWhiteSpace(Command) && (CommandSeq == null || !CommandSeq.Any()))))
            {
                Output.Add("--create was specified but either a name or a command was not given");
                result = false;
            }

            //if run and no name
            if (Run && String.IsNullOrWhiteSpace(Name))
            {
                Output.Add("--run was specified but a name was not given");
                result = false;
            }

            return result;
        }
    }
}
