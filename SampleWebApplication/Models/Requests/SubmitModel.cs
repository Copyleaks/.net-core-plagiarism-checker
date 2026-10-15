using Copyleaks.SDK.V3.API.Models.Types;
using System;

namespace Copyleaks.SDK.Demo.Models.Requests
{
    public class SubmitModel
    {
        public Uri WebHookHost { get; set; }
        public string ServerName { get; set; }

        public bool Sandbox { get; set; } = false;

        public string Text { get; set; } = "hello world";

        public string Token { get; set; }

    }
}
