using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatopsBot.Core.Util
{
    public class CustomSlackChannelData
    {
        /*
         "attachments": [
        {
            "fallback": "Required plain-text summary of the attachment.",
            "color": "#36a64f",
            "title": "Slack API Documentation",
            "text": "Optional text that appears within the attachment.\nTesting newlines."
         }
    ]
         */

        public List<CustomSlackAttachment> Attachments { get; set; } = new List<CustomSlackAttachment>();
    }

    public class CustomSlackAttachment
    {
        public string Color { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string[] mrkdwn_in { get; set; } = new[] {"text"};
        public string Fallback { get; set; }
    }
}