using System;
using System.Collections.Generic;
using Microsoft.Bot.Connector;

namespace ChatopsBot.Core.Util
{
    public class BuildBotState
    {
        public string DefaultProject { get; set; }
        public const string DefaultProjectField = "defaultProject";
        public List<CommandAlias> Aliases { get; set; } = new List<CommandAlias>();
        public const string AliasesField = "aliases";
        public string TfsUser { get; set; }
        public const string TfsUserField = "tfsUser";

        public DateTime? TfsUserPromptSince { get; set; }
        public const string TfsUserPromptField = "tfsUserPromptSince";

        public static BuildBotState CreateState(Message message)
        {
            var state = new BuildBotState();


                if (message.BotUserData != null)
                {
                    state.DefaultProject = message.GetBotUserData<string>(BuildBotState.DefaultProjectField);
                    state.Aliases = message.GetBotUserData<List<CommandAlias>>(BuildBotState.AliasesField);
                    state.TfsUser = message.GetBotUserData<string>(BuildBotState.TfsUserField);
                    state.TfsUserPromptSince = message.GetBotUserData<DateTime?>(BuildBotState.TfsUserPromptField);
                }


            if (state.Aliases == null) state.Aliases = new List<CommandAlias>();
            return state;
        }


    }


}