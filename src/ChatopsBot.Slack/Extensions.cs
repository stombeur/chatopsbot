using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChatopsBot.Slack
{
    public static class Extensions
    {
        public static SlackMessage AddAttachment(this SlackMessage message, SlackAttachment attachment)
        {
            if (message.Attachments == null) message.Attachments = new List<SlackAttachment>();
            message.Attachments.Add(attachment);
            return message;
        }

        public static SlackMessage AddField(this SlackMessage message, SlackField attachment)
        {
            return message;
        }

        public static object ToJObject(this SlackMessage message)
        {
            return JObject.FromObject(message);
        }

        public static void SetFooterTimestamp(this SlackAttachment attachment, DateTime datetime)
        {
            TimeSpan t = datetime - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            attachment.FooterTimestamp = secondsSinceEpoch;
        }
    }
}
