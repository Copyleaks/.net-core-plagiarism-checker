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
        public IActionResult CompletedWebhookProcessor([FromRoute]string scanId, [FromBody] CompletedWebhookModel completedWebhook)
        {
            // Do something with the scan results
            Console.WriteLine("Recieved Completed Webhook: " + JsonConvert.SerializeObject(completedWebhook, Formatting.Indented));
            return Ok(completedWebhook);
        }


        [HttpPost]
        [Route("error/{scanId}")]
        // this endpoint handles the error webhook response from copyleaks system
        public IActionResult ErrorWebhookProccessor([FromRoute] string scanId, [FromBody] ErrorWebhookModel errorWebhook)
        {
            // Do something with the scan results
            Console.WriteLine("Recieved Error Webhook: "+JsonConvert.SerializeObject(errorWebhook, Formatting.Indented));
            return Ok(errorWebhook);
        }

        [HttpPost]
        [Route("creditsChecked/{scanId}")]
        // this endpoint handles the credits-checked webhook response from copyleaks system
        public IActionResult CreditsCheckedWebhookProccessor([FromRoute] string scanId, [FromBody] CreditsCheckedWebhookModel creditsCheckedWebhook)
        {
            // Do something with the scan results
            Console.WriteLine("Recieved Credits Checked Webhook: " + JsonConvert.SerializeObject(creditsCheckedWebhook, Formatting.Indented));
            return Ok(creditsCheckedWebhook);
        }


        [HttpPost]
        [Route("indexed/{scanId}")]
        // this endpoint handles the indexed webhook response from copyleaks system
        public IActionResult IndexedWebhookProccessor([FromRoute] string scanId, [FromBody] IndexedWebhookModel indexedWebhook)
        {
            // Do something with the scan results
            Console.WriteLine("Recieved Indexed Webhook: " + JsonConvert.SerializeObject(indexedWebhook, Formatting.Indented));
            return Ok(indexedWebhook);
        }
        [HttpPost]
        [Route("new-results/{scanId}")]
        // this endpoint handles the new result webhook response from copyleaks system
        public IActionResult NewResultWebhookProccessor([FromRoute] string scanId, [FromBody] NewResultWebhookModel newResultsWebhook)
        {
            // Do something with the scan results
            Console.WriteLine("Recieved New Results Webhook: " + JsonConvert.SerializeObject(newResultsWebhook, Formatting.Indented));
            return Ok(newResultsWebhook);
        }
    }
}
