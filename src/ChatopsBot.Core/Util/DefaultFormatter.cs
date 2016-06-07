using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChatopsBot.Commands;
using ChatopsBot.Core.Controllers;

namespace ChatopsBot.Core.Util
{
    public class DefaultFormatter : IReplyMessageFormatter
    {
        public object FormatAttachments(Command command, MessageMeta meta, BuildBotState state)
        {
            return null;
        }

        public string Format(Command command, MessageMeta meta, BuildBotState state)
        {

            return string.Join(Constants.EnvironmentNewLine, command.Output);

        }

    }
}