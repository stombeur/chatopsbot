using ChatopsBot.Commands;
using ChatopsBot.Core.Controllers;

namespace ChatopsBot.Core.Util
{
    public interface IReplyMessageFormatter
    {
        object FormatAttachments(Command command, MessageMeta meta);
        string Format(Command command, MessageMeta meta);
    }
}