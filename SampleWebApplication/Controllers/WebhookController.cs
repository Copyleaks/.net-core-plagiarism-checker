using System;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Copyleaks.SDK.Demo.Controllers
{
    [ApiController]
    [Route("/webhook")]
    /// Handles the webhooks Copyleaks sends.
    public class WebhookController : ControllerBase
    {

        [HttpPost]
        [Route("completed/{scanId}")]
        public IActionResult CompletedWebhookProcessor([FromRoute]string scanId, [FromBody] CompletedWebhookModel completedWebhook)
        {
            // Do something with the scan results
            Console.WriteLine("Recieved Completed Webhook: " + JsonConvert.SerializeObject(completedWebhook, Formatting.Indented));
            return Ok(completedWebhook);
        }


        [HttpPost]
        [Route("error/{scanId}")]
        public IActionResult ErrorWebhookProccessor([FromRoute] string scanId, [FromBody] ErrorWebhookModel errorWebhook)
        {
            // Do something with the scan results
            Console.WriteLine("Recieved Error Webhook: "+JsonConvert.SerializeObject(errorWebhook, Formatting.Indented));
            return Ok(errorWebhook);
        }

        [HttpPost]
        [Route("creditsChecked/{scanId}")]
        public IActionResult CreditsCheckedWebhookProccessor([FromRoute] string scanId, [FromBody] CreditsCheckedWebhookModel creditsCheckedWebhook)
        {
            // Do something with the scan results
            Console.WriteLine("Recieved Credits Checked Webhook: " + JsonConvert.SerializeObject(creditsCheckedWebhook, Formatting.Indented));
            return Ok(creditsCheckedWebhook);
        }


        [HttpPost]
        [Route("indexed/{scanId}")]
        public IActionResult IndexedWebhookProccessor([FromRoute] string scanId, [FromBody] IndexedWebhookModel indexedWebhook)
        {
            // Do something with the scan results
            Console.WriteLine("Recieved Indexed Webhook: " + JsonConvert.SerializeObject(indexedWebhook, Formatting.Indented));
            return Ok(indexedWebhook);
        }
        [HttpPost]
        [Route("new-results/{scanId}")]
        public IActionResult NewResultWebhookProccessor([FromRoute] string scanId, [FromBody] NewResultWebhookModel newResultsWebhook)
        {
            // Do something with the scan results
            Console.WriteLine("Recieved New Results Webhook: " + JsonConvert.SerializeObject(newResultsWebhook, Formatting.Indented));
            return Ok(newResultsWebhook);
        }
    }
}
