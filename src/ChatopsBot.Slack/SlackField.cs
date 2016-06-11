﻿using System.Runtime.Serialization;

namespace ChatopsBot.Slack
{
    /// <summary>
    /// Fields are defined as an array, and hashes contained within it will be displayed in a table inside the message attachment.
    /// </summary>
    [DataContract]
    public class SlackField
    {

        /// <summary>
        ///Shown as a bold heading above the value text. It cannot contain markup and will be escaped for you.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// The text value of the field. It may contain standard message markup and must be escaped as normal. May be multi-line.
        /// </summary>
        [DataMember(Name = "value")]
        public string Value { get; set; }

        /// <summary>
        /// An optional flag indicating whether the value is short enough to be displayed side-by-side with other values.
        /// </summary>
        [DataMember(Name = "short", EmitDefaultValue = false)]
        public bool IsShort { get; set; }
    }
}