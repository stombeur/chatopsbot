using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using ChatopsBot.Commands;
using ChatopsBot.Core.Controllers;
using ChatopsBot.Slack;
using CommandLine;

namespace ChatopsBot.Core.Util
{
    //https://api.slack.com/docs/formatting/builder
    public class SlackFormatter : IReplyMessageFormatter
    {
        public object FormatAttachments(Command command, MessageMeta meta, BuildBotState state)
        {

            var result = new SlackMessage();

            var attachment = new SlackAttachment();
            attachment.Color = command.ParsingSuccess ? Colors.Good : (command.IsHelp ? Color.Gray.ToRgb() : Colors.Warning);
            if (command.ParsingSuccess && !command.ExecutingSuccess) attachment.Color = Colors.Danger;

            attachment.Title = string.Format(command.Title, meta.FromName);

            var text = string.Concat(command.Output.Select(s => s.SlackNewline().HtmlEncode().ReplaceWithIndent("   ").AppendNewLine()));

            attachment.Text = text;
            attachment.Fallback = text;

            result.Attachments.Add(attachment);

            if (state.TfsUserPromptSince == null) state.TfsUserPromptSince = DateTime.UtcNow.AddDays(-3);
            if (string.IsNullOrWhiteSpace(state.TfsUser) && state.TfsUserPromptSince.Value < DateTime.UtcNow.AddDays(-2))
            {
                state.TfsUserPromptSince = DateTime.UtcNow;

                var tfsUserPromptAttachment = new SlackAttachment();
                tfsUserPromptAttachment.Color = Colors.Warning;
                tfsUserPromptAttachment.Text = $"Hey {meta.FromName}, I noticed you have not set your tfs username.".AppendNewLine();
                tfsUserPromptAttachment.Text += $"If you do, I can request builds in your name.".AppendNewLine();
                tfsUserPromptAttachment.Text += $"Run this command to set your tfs username:".AppendNewLine();
                tfsUserPromptAttachment.Text += $"set --tfsuser [your username]".Pre().AppendNewLine();
                tfsUserPromptAttachment.Fallback = $"{meta.FromName}, please set your tfs username with the following command:".AppendNewLine() + "set --tfsuser [your username]";

                result.Attachments.Add(tfsUserPromptAttachment);
            }

            return result.ToJObject();
        }

        public string Format(Command command, MessageMeta meta, BuildBotState state)
        {
            return (command.IsHelp && !string.IsNullOrWhiteSpace(meta.ReplyHeader)) ? meta.ReplyHeader : "";
        }

    }
}