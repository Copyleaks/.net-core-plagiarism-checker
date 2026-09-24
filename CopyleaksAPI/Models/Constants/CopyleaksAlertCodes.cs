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
namespace Copyleaks.SDK.V3.API.Models.Constants
{
    /// <summary>
    /// Scan alert codes that can appear in the completed webhook, under notifications.alerts[].code.
    /// Codes are case-sensitive.
    /// </summary>
    public static class CopyleaksAlertCodes
    {
        /// <summary>
        /// AI-generated text was detected in the scanned document (category 2, severity 4).
        /// The alert's additionalData holds the AI detection result as a JSON string.
        /// </summary>
        public const string SUSPECTED_AI_TEXT = "suspected-ai-text";

        /// <summary>
        /// AI detection could not be completed for the scanned document (category 2).
        /// </summary>
        public const string AI_DETECTION_FAILED = "ai-detection-failed";

        /// <summary>
        /// AI detection was not executed because the document language is not supported (category 2).
        /// </summary>
        public const string AI_DETECTION_LANG_NOT_SUPPORTED = "ai-detection-lang-not-supported";

        /// <summary>
        /// AI detection was not executed because the text is too short (category 2).
        /// </summary>
        public const string AI_DETECTION_TEXT_TOO_SHORT = "ai-detection-text-too-short";

        /// <summary>
        /// AI detection was not executed because the file type is not supported (category 2).
        /// </summary>
        public const string FILE_TYPE_NOT_SUPPORTED = "file-type-not-supported";
    }
}
