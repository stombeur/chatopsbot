using ChatopsBot.Commands;
using ChatopsBot.Core.Controllers;

namespace ChatopsBot.Core.Util
{
    public interface IReplyMessageFormatter
    {
        object FormatAttachments(Command command, MessageMeta meta, BuildBotState state);
        string Format(Command command, MessageMeta meta, BuildBotState state);
    }
}