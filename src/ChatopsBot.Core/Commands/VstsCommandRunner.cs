using System;
using System.Linq;
using System.Threading.Tasks;
using ChatopsBot.Commands;
using ChatopsBot.Core.Util;
using ChatopsBot.Core.Vsts;
using Microsoft.Bot.Connector;

namespace ChatopsBot.Core.Commands
{
    public class VstsCommandRunner
    {
        public static async Task<Command> RunCommand(ListProjectCommand command, MessageMeta message, BuildBotState state)
        {
            var projects = await VSTSClient.GetProjects();
            foreach (var p in projects)
            {
                command.Output.Add($"{p.Name} - projectid={p.Id:N}");
            }

            command.ExecutingSuccess = true;

            return await Task.FromResult((Command)command);
        }

        public static async Task<Command> RunCommand(ListBuildCommand command, MessageMeta message, BuildBotState state)
        {
            var projectid = command.ProjectIdPos ?? command.ProjectId ?? state.CurrentProjectId;

            if (string.IsNullOrWhiteSpace(projectid))
                command.Output.Add("could not get projectid from state or message");
            else
            {

                var builds = await VSTSClient.GetBuildDefinitions(projectid);
                command.Output.Add($"projectid={projectid}");
                foreach (var b in builds)
                {
                    command.Output.Add($"'{b.Name}' \t\t\t\t id={b.Id}");
                }
            }

            command.ExecutingSuccess = true;

            return await Task.FromResult((Command)command);
        }

        public static async Task<Command> RunCommand(QueueCommand command, MessageMeta message, BuildBotState state)
        {
            var projectid = command.ProjectId ?? state.CurrentProjectId;
            var buildIdString = command.BuildIdPos ?? command.BuildId;
            var buildId = -1;
            Int32.TryParse(buildIdString, out buildId);


            if (string.IsNullOrWhiteSpace(projectid))
                command.Output.Add("could not get projectid from state or message");
            else if (buildId < 0)
                command.Output.Add("could not get buildId from state or message");
            else
            {
                command.Output.Add($"buildId={buildId}");
                try
                {
                    var build = await VSTSClient.StartBuild(buildId, projectid, message.FromName);
                    command.Output.Add($"build name '{build.Definition.Name}'");
                    command.Output.Add($"build number {build.BuildNumber}");

                    command.ExecutingSuccess = true;
                }
                catch (Exception ex)
                {
                    command.Output.Add("   there was an ERROR - " + ex.Message);
                }
            }

            return await Task.FromResult((Command)command);
        }

        public static async Task<Command> RunCommand(CancelBuildCommand command, MessageMeta message, BuildBotState state)
        {
            var projectid = command.ProjectId ?? state.CurrentProjectId;
            var buildIdString = command.BuildIdPos ?? command.BuildId;
            var buildId = -1;
            Int32.TryParse(buildIdString, out buildId);


            if (string.IsNullOrWhiteSpace(projectid))
                command.Output.Add("could not get projectid from state or message");
            else if (buildId < 0)
                command.Output.Add("could not get buildId from state or message");
            else
            {
                command.Output.Add($"buildId={buildId}");
                try
                {
                    var builds = await VSTSClient.CancelBuild(buildId, projectid);
                    foreach (var b in builds)
                    {
                        command.Output.Add($"canceled build '{b.Definition.Name}' with number {b.BuildNumber}");
                    }
                    if (!builds.Any()) command.Output.Add("   there were no cancelable builds");

                    command.ExecutingSuccess = true;
                }
                catch (Exception ex)
                {
                    command.Output.Add("   there was an ERROR - " + ex.Message);
                }
            }

            return await Task.FromResult((Command)command);
        }
    }
}