﻿using System;
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
        public static async Task<Command> RunCommand(ProjectCommand command, MessageMeta message, BuildBotState state)
        {
            if (!command.Validate()) return await Task.FromResult((Command)command);

            try
            {
                if (command.List)
                {
                    command.Title += "-list";
                    var projects = await VSTSClient.GetProjects();
                    foreach (var p in projects)
                    {
                        command.Output.Add($"{p.Name} - projectid={p.Id:N}");
                    }
                }
                else if (command.Default)
                {
                    command.Title += "-default";
                    command.Output.Add($"set default project to '{command.IdOrNamePos}'");
                    state.DefaultProject = command.IdOrNamePos;
                }
                command.ExecutingSuccess = true;

            }
            catch (Exception ex)
            {
                command.Output.Add("   there was an ERROR - " + ex.Message);
            }

            return await Task.FromResult((Command)command);
        }

        public static async Task<Command> RunCommand(BuildCommand command, MessageMeta message, BuildBotState state)
        {
            var projectid = command.ProjectId ?? state.DefaultProject;
            var buildIdString = command.BuildIdPos ?? command.BuildId;
            var buildId = -1;
            int.TryParse(buildIdString, out buildId);

            if (!command.Validate()) return await Task.FromResult((Command)command);

            if (string.IsNullOrWhiteSpace(projectid))
            {
                command.Output.Add("could not get projectid from state or message");
                return await Task.FromResult((Command)command);
            }

            try
            {
                //precedence = start, cancel, list
                if (command.Start || command.Cancel)
                {
                    //we need buildid or name
                    if (buildId <= 0) buildId = await VSTSClient.GetBuildId(buildIdString, projectid);

                    if (buildId <= 0)
                    {
                        command.Output.Add($"could not find a build with name '{buildIdString}'");
                        return await Task.FromResult((Command) command);
                    }

                    command.Output.Add($"buildId={buildId}");
                    command.Output.Add($"projectid={projectid}");

                    if (command.Start)
                    {
                        command.Title += "-start";
                        var build = await VSTSClient.StartBuild(buildId, projectid);
                        command.Output.Add($"build name '{build.Definition.Name}'");
                        command.Output.Add($"build number {build.BuildNumber}");
                    }
                    else if (command.Cancel)
                    {
                        command.Title += "-cancel";
                        var builds = await VSTSClient.CancelBuild(buildId, projectid);
                        var hasBuilds = false;
                        foreach (var b in builds)
                        {
                            command.Output.Add($"canceled build '{b.Definition.Name}' with number {b.BuildNumber}");
                            hasBuilds = true;
                        }
                        if (!hasBuilds) command.Output.Add("   there were no cancelable builds");
                    }
                }
                else if (command.List)
                {
                    command.Title += "-list";
                    var builds = await VSTSClient.GetBuildDefinitions(projectid);
                    foreach (var b in builds)
                    {
                        command.Output.Add($"'{b.Name}' id=*{b.Id}*");
                    }
                }

                command.ExecutingSuccess = true;
            }
            catch (Exception ex)
            {
                command.Output.Add("   there was an ERROR - " + ex.Message);
            }

            return await Task.FromResult((Command)command);
        }

 
    }
}