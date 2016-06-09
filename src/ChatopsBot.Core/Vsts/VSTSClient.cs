using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using CommandLine.Infrastructure;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace ChatopsBot.Core.Vsts
{
    public class VSTSClient
    {
        //WorkItemTrackingHttpClient
        //BuildHttpClient
        //ProjectHttpClient
        //TeamHttpClient

        public VSTSClient()
        {

        }

        public static async Task<List<Build>> GetRunningBuilds(string projectid)
        {
            VssConnection connection = new VssConnection(new Uri(ConfigurationManager.AppSettings["vstsurl"]), new VssBasicCredential(string.Empty, ConfigurationManager.AppSettings["devopsbottoken"]));
            var client = connection.GetClient<BuildHttpClient>();

            return await client.GetBuildsAsync(Guid.Parse(projectid), null, null, null, null, null, null, null, BuildStatus.InProgress | BuildStatus.NotStarted);
        }

        public static async Task<List<BuildDefinitionReference>>  GetBuildDefinitions(string projectid)
        {
            VssConnection connection = new VssConnection(new Uri(ConfigurationManager.AppSettings["vstsurl"]), new VssBasicCredential(string.Empty, ConfigurationManager.AppSettings["devopsbottoken"]));
            var client = connection.GetClient<BuildHttpClient>();

            return await client.GetDefinitionsAsync(Guid.Parse(projectid));
        }

        public static async Task<List<BuildDefinitionReference>> GetBuildDefinitionsByProjectNameOrId(string projectNameOrId)
        {
            VssConnection connection = new VssConnection(new Uri(ConfigurationManager.AppSettings["vstsurl"]), new VssBasicCredential(string.Empty, ConfigurationManager.AppSettings["devopsbottoken"]));
            var projClient = connection.GetClient<ProjectHttpClient>();
            var projects = await projClient.GetProjects();

            var projectid = projects.Where(p => p.Name.ToLower() == projectNameOrId || p.Id.ToString("N") == projectNameOrId).Select(p => p.Id).First();

            var buildClient = connection.GetClient<BuildHttpClient>();

            return await buildClient.GetDefinitionsAsync(projectid);
        }

        public static async Task<Build> StartBuild(int buildid, string projectid, string requestedFor = null, string buildConfig = null)
        {
            VssConnection connection = new VssConnection(new Uri(ConfigurationManager.AppSettings["vstsurl"]), new VssBasicCredential(string.Empty, ConfigurationManager.AppSettings["devopsbottoken"]));
            var client = connection.GetClient<BuildHttpClient>();
            var def = await client.GetDefinitionAsync(Guid.Parse(projectid), buildid);
            var build = new Build {Definition = def};

            if (!string.IsNullOrWhiteSpace(requestedFor))
            {
                build.RequestedFor = await GetIdentity(requestedFor, projectid);
            }

            if (!string.IsNullOrWhiteSpace(buildConfig))
            {
                var buildPlatform = def.Variables["BuildPlatform"].Value;
                var targetProfile = def.Variables["TargetProfile"].Value;

                build.Parameters = $"{{\"BuildConfiguration\":\"{buildConfig}\",\"BuildPlatform\":\"{buildPlatform}\",\"TargetProfile\":\"{targetProfile}\"}}";
                //{"BuildConfiguration":"Release.Azure","BuildPlatform":"any cpu","TargetProfile":"Cloud"}
            }

            return await client.QueueBuildAsync(build, Guid.Parse(projectid));
        }

        public static async Task<int> GetBuildId(string buildName, string projectid)
        {
            VssConnection connection = new VssConnection(new Uri(ConfigurationManager.AppSettings["vstsurl"]), new VssBasicCredential(string.Empty, ConfigurationManager.AppSettings["devopsbottoken"]));
            var client = connection.GetClient<BuildHttpClient>();

            var definitions = await client.GetDefinitionsAsync(projectid, name: buildName);
            return definitions.Select(b => b.Id).FirstOrDefault();
        }

        public static async Task<IdentityRef> GetIdentity(string name, string projectid)
        {
            IdentityRef result = null;

            VssConnection connection = new VssConnection(new Uri(ConfigurationManager.AppSettings["vstsurl"]), new VssBasicCredential(string.Empty, ConfigurationManager.AppSettings["devopsbottoken"]));
            var client = connection.GetClient<TeamHttpClient>();
           
            var teams = await client.GetTeamsAsync(projectid);
            foreach (var t in teams)
            {
                var members = await client.GetTeamMembersAsync(projectid, t.Id.ToString("N"));
                result = members.Find(m => m.Id.Equals(name, StringComparison.CurrentCultureIgnoreCase) || m.UniqueName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                if (result != null) break;
            }

            return result;
        }

        public static async Task<Build> StartBuildByName(string buildName, string projectNameOrId)
        {
            VssConnection connection = new VssConnection(new Uri(ConfigurationManager.AppSettings["vstsurl"]), new VssBasicCredential(string.Empty, ConfigurationManager.AppSettings["devopsbottoken"]));
            var projClient = connection.GetClient<ProjectHttpClient>();
            var projects = await projClient.GetProjects();
            var projectid = projects.Where(p => p.Name.ToLower() == projectNameOrId || p.Id.ToString("N") == projectNameOrId).Select(p => p.Id).First();
            var buildClient = connection.GetClient<BuildHttpClient>();

            var defs = await buildClient.GetDefinitionsAsync(projectid);
            var namedDef = defs.Where(b => b.Name.ToLower() == buildName).First();
            var build = new Build();
            build.Definition = namedDef;

            return await buildClient.QueueBuildAsync(build,projectid);
        }


        public static async Task<IEnumerable<TeamProjectReference>> GetProjects()
        {
            VssConnection connection = new VssConnection(new Uri(ConfigurationManager.AppSettings["vstsurl"]), new VssBasicCredential(string.Empty, ConfigurationManager.AppSettings["devopsbottoken"]));
            var client = connection.GetClient<ProjectHttpClient>();

            return await client.GetProjects();
        }

        public static async Task<IEnumerable<Build>> CancelBuild(int buildid, string projectid)
        {
            VssConnection connection = new VssConnection(new Uri(ConfigurationManager.AppSettings["vstsurl"]), new VssBasicCredential(string.Empty, ConfigurationManager.AppSettings["devopsbottoken"]));
            var client = connection.GetClient<BuildHttpClient>();

            // Get the specified name build definition to get the id of build definition
            var def = await client.GetDefinitionAsync(Guid.Parse(projectid), buildid);
            if (def == null)
            {
                throw new ArgumentException($"No build definition with name: {buildid}");
            }
            //Get the builds of the specified name build definiion
            var builds = await client.GetBuildsAsync(projectid, new[] { def.Id });
            var result = new List<Build>();
            //Update the builds with the status other than "Completed" and "Cancelling" to cancel status
            foreach (var b in builds.Where(b => !(b.Status == BuildStatus.Completed || b.Status == BuildStatus.Cancelling)))
            {
                b.Status = BuildStatus.Cancelling;
                result.Add(await client.UpdateBuildAsync(b, b.Id));
            }

            return result;
        }
    }
}