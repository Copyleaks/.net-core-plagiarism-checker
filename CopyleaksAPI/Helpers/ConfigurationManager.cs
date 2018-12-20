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

using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Copyleaks.SDK.V3.API.Helpers
{
	// CR : Documentation is missing. 
	public static class ConfigurationManager
    {
        static ConfigurationManager()
        {
            string configPath = "config.json";
            if (!File.Exists(configPath))
                throw new ConfigurationException($"Configuration file '{configPath}' is missing, Copy 'https://github.com/Copyleaks/.NET-Core-Plagiarism-Checker/{configPath}' to your projects root directory"); // CR: Change the URL
            Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(configPath, optional: true, reloadOnChange: true)
            .Build();
        }

        public static IConfiguration Configuration { get; set; }
    }

	// CR: Really needed? Maybe just throw generic error?
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string msg): base(msg) { }

    }
}
