﻿/********************************************************************************
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
    /// <summary>
    /// This class is returned in case an error response was received from Copyleaks API
    /// </summary>
    public class CommandFailedException : ApplicationException
    {
        /// <summary>
        /// The Http status code
        /// </summary>
        public HttpStatusCode HttpErrorCode { get; private set; }

        /// <summary>
        /// Copyleaks error code, see https://api.copyleaks.com/documentation/errors#Managed-Errors for a full list of error codes
        /// </summary>
        public short CopyleaksErrorCode { get; private set; }

        /// <summary>
        /// A new error response from Copyleaks API
        /// </summary>
        /// <param name="response">The error response from Copyleaks API</param>
        public CommandFailedException(HttpResponseMessage response) :
            base(GetMessage(response))
        {
            this.HttpErrorCode = response.StatusCode;
        }

        private static string GetMessage(HttpResponseMessage response)
        {

            string errorResponse = response.Content.ReadAsStringAsync().Result;

            CopyleaksErrorResponse error = null;
            try
            {
                error = JsonConvert.DeserializeObject<CopyleaksErrorResponse>(errorResponse);
            }
            catch (JsonReaderException)
            {
                return errorResponse;
            }

            if (error == null)
                return "The application has encountered an unknown error. Please try again later.";
            else
                return error.Message;
        }

    }
}
