using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ChatopsBot.Slack
{
    [DataContract]
    public class SlackMessage
    {
        [DataMember(Name = "channel")]
        public string Channel { get; set; }

        [DataMember(Name = "mrkdwn")]
        public bool Markdown { get; set; } = true;

        [DataMember(Name = "username")]
        public string Username { get; set; }

        [DataMember(Name = "icon_url")]
        public string IconUrl { get; set; }

        [DataMember(Name = "icon_emoji")]
        public string IconEmoji { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "unfurl_links")]
        public bool UnfurlLinks { get; set; } = true;

        [DataMember(Name = "unfurl_media")]
        public bool UnfurlMedia { get; set; } = true;

        [DataMember(Name = "attachments")]
        public List<SlackAttachment> Attachments { get; set; } = new List<SlackAttachment>();
    }
}
