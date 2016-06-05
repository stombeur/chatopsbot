using System.Collections.Generic;
using Microsoft.Bot.Connector;

namespace ChatopsBot.Core.Util
{
    public class BuildBotState
    {
        public string CurrentProjectId { get; set; }
        public const string CurrentProjectIdField = "currentProjectId";
        public List<CommandAlias> Aliases { get; set; } = new List<CommandAlias>();
        public const string AliasesField = "aliases";

        public static BuildBotState CreateState(Message message)
        {
            var state = new BuildBotState();

            if (message.BotUserData != null)
            {
                state.CurrentProjectId = message.GetBotUserData<string>(BuildBotState.CurrentProjectIdField);
                state.Aliases = message.GetBotUserData<List<CommandAlias>>(BuildBotState.AliasesField);
            }

            if (state.Aliases == null) state.Aliases = new List<CommandAlias>();
            return state;
        }
    }
}