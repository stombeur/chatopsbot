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
        public object FormatAttachments(Command command, MessageMeta meta, BuildBotState state)
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
            attachment.Fallback = string.Join(Constants.SlackNewLine, command.Output);
            result.Attachments.Add(attachment);

            if (!state.TfsUserPrompt && string.IsNullOrWhiteSpace(state.TfsUser))
            {
                state.TfsUserPrompt = true;

                var tfsUserPromptAttachment = new CustomSlackAttachment();
                tfsUserPromptAttachment.Color = "warning";
                tfsUserPromptAttachment.Text = $"Hey {meta.FromName}, I noticed you have not set your tfs username.";
                tfsUserPromptAttachment.Text += $"{Constants.SlackNewLine}If you do, I can request builds in your name.";
                tfsUserPromptAttachment.Text += $"{Constants.SlackNewLine}Run this command to set your tfs username:";
                tfsUserPromptAttachment.Text += $"{Constants.SlackNewLine}```set --tfsuser [your username]```";
                attachment.Fallback = $"{meta.FromName}, please set your tfs username with the following command:{Constants.SlackNewLine}set --tfsuser [your username]";

                result.Attachments.Add(tfsUserPromptAttachment);
            }

            return result;
        }

        public string Format(Command command, MessageMeta meta, BuildBotState state)
        {
            return (command.IsHelp && !string.IsNullOrWhiteSpace(meta.ReplyHeader)) ? meta.ReplyHeader : "";
        }

    }
}