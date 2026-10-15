using System;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses
{
    /// <summary>The Copyleaks API release notes.</summary>
    public class ReleaseNotesResponse
    {
        /// <summary>The updates, newest first.</summary>
        [JsonProperty("updates")]
        public ReleaseNote[] Updates { get; set; }
    }

    /// <summary>One API release note.</summary>
    public class ReleaseNote
    {
        /// <summary>The kind of update, for example "NEW" or "FEATURE".</summary>
        [JsonProperty("msgType")]
        public string MsgType { get; set; }

        /// <summary>When the update was released.</summary>
        [JsonProperty("date")]
        public DateTimeOffset? Date { get; set; }

        /// <summary>The description of the update, in HTML.</summary>
        [JsonProperty("htmlDescription")]
        public string HtmlDescription { get; set; }
    }
}
