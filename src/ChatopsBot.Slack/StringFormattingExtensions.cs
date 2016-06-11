using System;

namespace ChatopsBot.Slack
{
    public static class StringFormattingExtensions
    {
        public static string Pre(this string s)
        {
            return $"```{s}```";
        }

        public static string Code(this string s)
        {
            return $"`{s}`";
        }

        public static string Italic(this string s)
        {
            return $"_{s}_";
        }

        public static string Bold(this string s)
        {
            return $"*{s}*";
        }

        public static string Strike(this string s)
        {
            return $"~{s}~";
        }

        public static string AppendNewLine(this string s)
        {
            return $"{s}\n";
        }
        public static string PrependNewLine(this string s)
        {
            return $"\n{s}";
        }

        public static string HtmlEncode(this string s)
        {
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public static string Indent(this string s)
        {
            return $">{s}";
        }

        public static string ReplaceWithIndent(this string s, string toReplace)
        {
            return s.StartsWith(toReplace) ? s.Replace(toReplace, ">") : s;
        }

        public static string SlackNewline(this string s, string oldNewLine = "\r\n")
        {
            return s.Replace(oldNewLine, "\n");
        }
    }
}