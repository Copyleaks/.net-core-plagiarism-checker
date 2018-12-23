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


namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
    /// <summary>
    /// Customizable colors for Copyleaks scan result report
    /// </summary>
    public class ReportCustomizationColors
    {
        /// <summary>
        /// The color of the main strip in the header
        /// </summary>
        public string MainStrip { get; set; }

        /// <summary>
        /// The color for titles in copyleaks result report
        /// </summary>
        public string titles { get; set; }

        /// <summary>
        /// The color for identical matches
        /// </summary>
        public string Identical { get; set; }

        /// <summary>
        /// The color for minor changes matches
        /// </summary>
        public string MinorChanges { get; set; }

        /// <summary>
        /// The color for related meaning matches
        /// </summary>
        public string RelatedMeaning { get; set; }
    }
}
