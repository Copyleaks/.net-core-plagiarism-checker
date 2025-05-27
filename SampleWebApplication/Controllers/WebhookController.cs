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
using Copyleaks.SDK.V3.API.Models.Callbacks;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Copyleaks.SDK.Demo.Controllers
{
    [ApiController]
    [Route("/webhook")]
    /// This controller for handling all the webhooks you might get from copyleks System
    public class WebhookController : ControllerBase
    {

        [HttpPost]
        [Route("completed/{scanId}")]
        // this endpoint handles the completed webhook response from copyleaks system
        public IActionResult CompletedWebhookProcessor(string scanId, [FromBody] CompletedWebhook scanResults)
        {
            // Do something with the scan results
            Console.WriteLine(JsonConvert.SerializeObject(scanResults, Formatting.Indented));
            return Ok(scanResults);
        }


        [HttpPost]
        [Route("error/{scanId}")]
        // this endpoint handles the error webhook response from copyleaks system
        public IActionResult ErrorWebhookProccessor(string scanId, [FromBody] ErrorWebhook scanResults)
        {
            // Do something with the scan results
            Console.WriteLine(JsonConvert.SerializeObject(scanResults, Formatting.Indented));
            return Ok(scanResults);
        }

        [HttpPost]
        [Route("creditsChecked/{scanId}")]
        // this endpoint handles the credits-checked webhook response from copyleaks system
        public IActionResult CreditsCheckedWebhookProccessor(string scanId, [FromBody] CreditsCheckedWebhook scanResults)
        {
            // Do something with the scan results
            Console.WriteLine(JsonConvert.SerializeObject(scanResults, Formatting.Indented));
            return Ok(scanResults);
        }


        [HttpPost]
        [Route("indexed/{scanId}")]
        // this endpoint handles the indexed webhook response from copyleaks system
        public IActionResult IndexedWebhookProccessor(string scanId, [FromBody] IndexedWebhook scanResults)
        {
            // Do something with the scan results
            Console.WriteLine(JsonConvert.SerializeObject(scanResults, Formatting.Indented));
            return Ok(scanResults);
        }
        [HttpPost]
        [Route("new-results/{scanId}")]
        // this endpoint handles the new result webhook response from copyleaks system
        public IActionResult NewResultWebhookProccessor(string scanId, [FromBody] NewResultWebhook scanResults)
        {
            // Do something with the scan results
            Console.WriteLine(JsonConvert.SerializeObject(scanResults, Formatting.Indented));
            return Ok(scanResults);
        }
    }
}
