/********************************************************************************
 The MIT License(MIT)
 
 Copyright(c) 2016 Copyleaks LTD (https://copyleaks.com)
 
 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:
 
 The above copyright notice and this permission notice shall be included in all
 copies or substantial portions of the Software.
 
 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 SOFTWARE.
********************************************************************************/

using Newtonsoft.Json;
using System;

namespace Copyleaks.SDK.V3.API.Models.Responses
{
    public class LoginResponse
    {
        public LoginResponse(string token, DateTime issued, DateTime expire)
        {
            this.Token = token;
            this.Issued = issued;
            this.Expire = expire;
        }

        #region Members & Properties

        [JsonProperty(PropertyName = "access_token")]
        public string Token { get; private set; }

        [JsonProperty(PropertyName = ".issued")]
        public DateTime Issued { get; private set; }

        [JsonProperty(PropertyName = ".expires")]
        public DateTime Expire { get; private set; }

        #endregion

        /// <summary>
        /// Validate that the token is valid. If unvalid, throw UnauthorizedAccessException.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">This token is expired</exception>
        public void Validate()
        {
            if (DateTime.UtcNow > this.Expire)
                throw new UnauthorizedAccessException(string.Format("This token expired on {0}", this.Expire));
        }

        public override string ToString()
        {
            return this.Token;
        }
    }
}
