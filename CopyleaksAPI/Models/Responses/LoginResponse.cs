using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Responses
{
    /// <summary>Login response from `CopyleaksIdentityApi.LoginAsync`</summary>
    public class LoginResponse
    {
        /// <summary>API login token for the `CopyleaksScansApi` methods.</summary>
        [JsonProperty(PropertyName = "access_token")]
        public string Token { get; private set; }

        /// <summary>When the token was issued.</summary>
        [JsonProperty(PropertyName = ".issued")]
        public DateTime Issued { get; private set; }

        /// <summary>Token expiry time.</summary>
        [JsonProperty(PropertyName = ".expires")]
        public DateTime Expire { get; private set; }

        /// <summary>Throws if the token has expired.</summary>
        /// <exception cref="UnauthorizedAccessException">The token has expired.</exception>
        public void Validate()
        {
            if (DateTime.UtcNow > this.Expire.ToUniversalTime())
                throw new UnauthorizedAccessException(string.Format("This token expired on {0}", this.Expire));
        }

        public override string ToString()
        {
            return this.Token;
        }
    }
}
