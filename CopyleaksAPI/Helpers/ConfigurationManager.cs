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
using System.Collections.Generic;
using System.IO;

namespace Copyleaks.SDK.V3.API.Helpers
{
    /// <summary>
    /// Copyleaks configuration manager
    /// </summary>
    public static class ConfigurationManager
    {
        static ConfigurationManager()
        {
            string configPath = "config.json";
            // If config file exists it is the default
            if (File.Exists(configPath))
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(configPath, optional: true, reloadOnChange: true)
                    .Build();
            // no config file, using the constants
            else
                Configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>()
                    {
                        { "apiEndPoint", SdkConsts.ApiEndPoint },
                        { "idEndPoint",  SdkConsts.IdEndPoint },
                        {  "RequestsTimeout", SdkConsts.RequestsTimeout},
                        { "apiVersion", SdkConsts.apiVersion }
                    }).Build();
        }

        /// <summary>
        /// The configuration object, Example usage: ConfigurationManager.Configuration["apiEndPoint"]
        /// </summary>
        public static IConfiguration Configuration { get; set; }
    }
}
