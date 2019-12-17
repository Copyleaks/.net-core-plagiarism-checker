using System;
using System.Collections.Generic;
using System.Text;

namespace Copyleaks.SDK.V3.API.Models.HttpCustomSend
{
    public class HttpMessagePayloads
    {
        public const string VERB_GET = "GET",
                            VERB_POST = "POST",
                            VERB_DELETE = "DELETE";

        public const string CONTENT_TYPE_JSON = "application/json";
        public const string CONTENT_TYPE_TEXT = "text/plain";
        public Uri TargetUri { get; set; }
        public string Verb { get; set; }
        public object Body { get; set; } = null;
        public string[,] Headers { get; set; } = null;
        public string AuthToken { get; set; } = null;
    }
}
