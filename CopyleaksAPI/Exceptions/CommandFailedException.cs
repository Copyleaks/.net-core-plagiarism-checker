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

using Copyleaks.SDK.V3.API.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Copyleaks.SDK.V3.API.Exceptions
{
	// CR : Documentation is missing. 
	// CR : CopyleaksErrorCode\Message is still in use?
    public class CommandFailedException : ApplicationException
    {
        const short UNDEFINED_COPYLEAKS_HEADER_ERROR_CODE = 9999;

        public HttpStatusCode HttpErrorCode { get; private set; }

        public short CopyleaksErrorCode { get; private set; }

        public bool IsManagedError
        {
            get
            {
                return CopyleaksErrorCode != UNDEFINED_COPYLEAKS_HEADER_ERROR_CODE;
            }
        }

        public CommandFailedException(HttpResponseMessage response) :
            base(GetMessage(response))
        {
            this.HttpErrorCode = response.StatusCode;
            this.CopyleaksErrorCode = parseCopyleaksErrorCode(response);
        }

        private static string GetMessage(HttpResponseMessage response)
        {

            string errorResponse = response.Content.ReadAsStringAsync().Result;

            BadResponse error = null;
            try
            {
                error = JsonConvert.DeserializeObject<BadResponse>(errorResponse);
            }
            catch (JsonReaderException)
            {
                if (parseCopyleaksErrorCode(response) != UNDEFINED_COPYLEAKS_HEADER_ERROR_CODE)
                    return errorResponse;
            }

            if (error == null)
                return "The application has encountered an unknown error. Please try again later.";
            else
                return error.Message;
        }

		// CR : "parseCopyleaksErrorCode" -> "ParseCopyleaksErrorCode"
		private static short parseCopyleaksErrorCode(HttpResponseMessage response)
        {
            const string COPYLEAKS_ERROR_CODE_HEADER_NAME = "Copyleaks-Error-Code";
            if (response.Headers.Contains(COPYLEAKS_ERROR_CODE_HEADER_NAME))
            {
                string[] values = response.Headers.GetValues(COPYLEAKS_ERROR_CODE_HEADER_NAME).ToArray();
				if (values != null && values.Length > 0 && short.TryParse(values[0], out short code))
					return code;
			}
            return UNDEFINED_COPYLEAKS_HEADER_ERROR_CODE;
        }
    }
}
