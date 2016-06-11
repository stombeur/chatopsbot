﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ChatopsBot.Slack
{

    /// <summary>
    /// Messages can have zero or more attachments, defined as an array. Each hash in that array will contain multiple properties.
    /// </summary>
    [DataContract]
    public class SlackAttachment
    {
        /// <summary>
        /// A plain-text summary of the attachment. This text will be used in clients that don't show formatted text (eg. IRC, mobile notifications) and should not contain any markup.
        /// </summary>
        [DataMember(Name = "fallback")]
        public string Fallback { get; set; }

        /// <summary>
        /// This is the main text in a message attachment, and can contain standard message markup. The content will automatically collapse if it contains 700+ characters or 5+ linebreaks, and will display a "Show more..." link to expand the content.
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// This is optional text that appears above the message attachment block.
        /// </summary>
        [DataMember(Name = "pretext")]
        public string Pretext { get; set; }

        /// <summary>
        /// An optional value that can either be one of good, warning, danger, or any hex color code (eg. #439FE0). This value is used to color the border along the left side of the message attachment.
        /// </summary>
        [DataMember(Name = "color")]
        public string Color { get; set; }

        /// <summary>
        /// The title is displayed as larger, bold text near the top of a message attachment.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// By passing a valid URL in the title_link parameter (optional), the title text will be hyperlinked.
        /// </summary>
        [DataMember(Name = "title_link")]
        public string TitleLink { get; set; }

        /// <summary>
        /// The author parameters will display a small section at the top of a message attachment.
        /// Small text used to display the author's name.
        /// </summary>
        [DataMember(Name = "author_name")]
        public string AuthorName { get; set; }

        /// <summary>
        /// The author parameters will display a small section at the top of a message attachment.
        /// A valid URL that will hyperlink the author_name text mentioned above. Will only work if author_name is present.
        /// </summary>
        [DataMember(Name = "author_link")]
        public string AuthorLink { get; set; }

        /// <summary>
        /// The author parameters will display a small section at the top of a message attachment.
        /// A valid URL that displays a small 16x16px image to the left of the author_name text. Will only work if author_name is present.
        /// </summary>
        [DataMember(Name = "author_icon")]
        public string AuthorIcon { get; set; }

        /// <summary>
        /// A valid URL to an image file that will be displayed inside a message attachment. We currently support the following formats: GIF, JPEG, PNG, and BMP. Large images will be resized to a maximum width of 400px or a maximum height of 500px, while still maintaining the original aspect ratio.
        /// </summary>
        [DataMember(Name = "image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// A valid URL to an image file that will be displayed as a thumbnail on the right side of a message attachment. We currently support the following formats: GIF, JPEG, PNG, and BMP.
        /// The thumbnail's longest dimension will be scaled down to 75px while maintaining the aspect ratio of the image. The filesize of the image must also be less than 500 KB. For best results, please use images that are already 75px by 75px.
        /// </summary>
        [DataMember(Name = "thumb_url")]
        public string ThumbUrl { get; set; }

        /// <summary>
        /// Specify which parts of the message contain markdown formatting. Valid values are "pretext", "text", "fields". Default is "text".
        /// </summary>
        [DataMember(Name = "mrkdwn_in")]
        public string[] MarkdownIn { get; set; } = new[] { "text" };

        /// <summary>
        /// Add some brief text to help contextualize and identify an attachment. Limited to 300 characters, and may be truncated further when displayed to users in environments with limited screen real estate.
        /// </summary>
        [DataMember(Name = "footer")]
        public string Footer { get; set; }

        /// <summary>
        /// To render a small icon beside your footer text, provide a publicly accessible URL string in the footer_icon field. You must also provide a footer for the field to be recognized. We'll render what you provide at 16px by 16px. It's best to use an image that is similarly sized.
        /// </summary>
        [DataMember(Name = "footer_icon")]
        public string FooterIcon { get; set; }

        /// <summary>
        /// Does your attachment relate to something happening at a specific time?
        /// By providing the ts field with an integer value in "epoch time", the attachment will display an additional timestamp value as part of the attachment's footer.
        /// Use ts when referencing articles or happenings.Your message will have its own timestamp when published.
        /// Example: Providing 123456789 would result in a rendered timestamp of Nov 29th, 1973.
        /// </summary>
        [DataMember(Name = "ts")]
        public int FooterTimestamp { get; set; }

        /// <summary>
        /// Collection of <see cref="SlackField"/>
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public IList<SlackField> Fields { get; set; }

    }
}