using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChatopsBot.Commands;
using ChatopsBot.Core.Controllers;
using CommandLine;

namespace ChatopsBot.Core.Util
{
    public class SlackFormatter : IReplyMessageFormatter
    {
        public object FormatAttachments(Command command, MessageMeta meta)
        {

            var result = new CustomSlackChannelData();

            var attachment = new CustomSlackAttachment();
            attachment.Color = command.ParsingSuccess ? "good" : (command.IsHelp ? "#dddddd" : "warning");
            if (command.ParsingSuccess && !command.ExecutingSuccess) attachment.Color = "danger";

            attachment.Title = string.Format(command.Title, meta.FromName);

            var lines1 =
                command.Output.Select(line => line.Replace(Constants.EnvironmentNewLine, Constants.SlackNewLine));
            var lines2 =
                lines1.Select(line => line.Replace("&", "&amp;").Replace(">", "&gt;").Replace("<", "&lt;")).ToList();
            var lines3 = lines2.Select(line => line.StartsWith("   ") ? ">" + line.Substring(2) : line);

            attachment.Text = string.Join(Constants.SlackNewLine, lines3);
            result.Attachments.Add(attachment);

            //var attach = new Attachment()
            //{
            //Title = command.Title,

            //Text = string.Join(Constants.SlackNewLine, command.Output),
            //};
            //return new List<Attachment>() {attach};

            return result;
        }

        public string Format(Command command, MessageMeta meta)
        {
            return (command.IsHelp && !string.IsNullOrWhiteSpace(meta.ReplyHeader)) ? meta.ReplyHeader : "";
        }

    }
}