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
using System;
using System.Diagnostics;
namespace Copyleaks.SDK.V3.API.Services
{
    public static class DeprecationService
    {
       public static void ShowDeprecationMessgae()
        {
            Trace.WriteLine("DEPRECATION NOTICE: AI Code Detection will be discontinued on August 29, 2025. Please remove AI code detection integrations before the sunset date.");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("════════════════════════════════════════════════════════════════════");
            Console.WriteLine("DEPRECATION NOTICE !!!");
            Console.WriteLine("AI Code Detection will be discontinued on August 29, 2025.");
            Console.WriteLine("Please remove AI code detection integrations before the sunset date.");
            Console.WriteLine("════════════════════════════════════════════════════════════════════");
            Console.ResetColor();
        }
    }
}
