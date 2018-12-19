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

using Copyleaks.SDK.V3.API.Models.Callbacks;
using Newtonsoft.Json;
using System.IO;

namespace Copyleaks.SDK.Demo.Helpers
{
    public class ResultFile
    {

        public static string GetResultDirectory() => $"{Path.GetTempPath()}\\CopyleaksSdkDemo";

        public static string GetResultsFilePath(string scanId) => $"{GetResultDirectory()}\\{scanId}.json";

        public static void CreateResultDirectory() => Directory.CreateDirectory(GetResultDirectory());

        private static bool HasResults(string scanId) => File.Exists(GetResultsFilePath(scanId));
        
        public static CompletedCallback GetResults(string scanId)
        {
            if (HasResults(scanId))
            {
                string path = GetResultsFilePath(scanId);
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<CompletedCallback>(json);
            }
            else
                return null;
        }

        //public static void DeleteFileIfExists(string scanId)
        //{
        //    string path = GetResultsFilePath(scanId);
        //    if (File.Exists(path))
        //        File.Delete(path);
        //}

        public static void SaveResults(CompletedCallback completedCallback, string scanId)
        {
            string json = JsonConvert.SerializeObject(completedCallback);
            string resultFilePath = GetResultsFilePath(scanId);
            
            File.WriteAllText(resultFilePath, json);
        }
    }
}
