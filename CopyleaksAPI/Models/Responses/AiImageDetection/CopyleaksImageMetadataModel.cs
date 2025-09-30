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

namespace Copyleaks.SDK.V3.API.Models.Responses.AiImageDetection
{
    /// <summary>
    /// Optional metadata extracted from the image.
    /// </summary>
    public class CopyleaksImageMetadataModel
    {
        /// <summary>
        /// Timestamp when the image was created (if available).
        /// </summary>
        [JsonProperty("issuedTime")]
        public string IssuedTime { get; set; }

        /// <summary>
        /// The AI service or tool that created the image (if detected).
        /// </summary>
        [JsonProperty("issuedBy")]
        public string IssuedBy { get; set; }

        /// <summary>
        /// The application or device used to create the image.
        /// </summary>
        [JsonProperty("appOrDeviceUsed")]
        public string AppOrDeviceUsed { get; set; }
    }
}
