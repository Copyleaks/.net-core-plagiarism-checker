using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Copyleaks.SDK.V3.API.Models.Responses.Webhooks.HelperModels.ErrorModels
{
    public class Error
    {
        [JsonProperty("code")]
        public int Code {  get; set; }
        [JsonProperty("message")]
        public string Message { get; set; } 
    }
}
