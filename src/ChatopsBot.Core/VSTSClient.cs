using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;

namespace ChatopsBot.Core
{
    public class VSTSClient
    {
        public VSTSClient()
        {

        }

        public static async Task<List<BuildDefinitionReference>>  GetBuildDefinitions(string projectid)
        {
            VssConnection connection = new VssConnection(new Uri(ConfigurationManager.AppSettings["vstsurl"]), new VssBasicCredential(string.Empty, ConfigurationManager.AppSettings["devopsbottoken"]));
            var client = connection.GetClient<BuildHttpClient>();

            return await client.GetDefinitionsAsync(Guid.Parse(projectid));
        }

        public static async Task<Build> StartBuild(int buildid, string projectid)
        {
            VssConnection connection = new VssConnection(new Uri(ConfigurationManager.AppSettings["vstsurl"]), new VssBasicCredential(string.Empty, ConfigurationManager.AppSettings["devopsbottoken"]));
            var client = connection.GetClient<BuildHttpClient>();

            var def = await client.GetDefinitionAsync(Guid.Parse(projectid), buildid);
            var build = new Build();
            build.Definition = def;

            return await client.QueueBuildAsync(build, Guid.Parse(projectid));
        }


        public static async Task<IEnumerable<TeamProjectReference>> GetProjects()
        {
            VssConnection connection = new VssConnection(new Uri(ConfigurationManager.AppSettings["vstsurl"]), new VssBasicCredential(string.Empty, ConfigurationManager.AppSettings["devopsbottoken"]));
            var client = connection.GetClient<ProjectHttpClient>();

            return await client.GetProjects();
        }
    }
}