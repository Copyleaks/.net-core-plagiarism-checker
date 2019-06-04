<h2>Copyleaks API C# SDK</h2>
<p>
Copyleaks SDK is a simple framework that allows you to perform plagiarism scans and track content distribution around the web, using Copyleaks API.
</p>
<p>
With Copyleaks SDK you can submit for scan:  
<ul>
<li>Webpages</li>
<li>Local files - pdf, doc, docx, rtf and more <a href="https://api.copyleaks.com/GeneralDocumentation/TechnicalSpecifications#supportedfiletypes">(see full list)</a></li>
<li>Free text</li>
<li>OCR (Optical Character Recognition) - scanning pictures containing textual content <a href="https://api.copyleaks.com/GeneralDocumentation/TechnicalSpecifications#supportedfiletypes">(see full list)</a></li>
</ul>
Instructions for using the SDK are below. For a quick example demonstrating the SDK capabilities just look at the code examples under “examples”.
</p>
<h3>Integration</h3>
<p>You can integrate with the Copyleaks SDK in one of two ways:</p>
<ol>
<li>Download the code from here, compile it and add reference to the assembly.</li>
<li>Add <i>Copyleaks</i> NuGet by running the following command in the <a href="http://docs.nuget.org/consume/package-manager-console">Package Manager Console</a></li>
<pre>
Install-Package Copyleaks
</pre>
</ol>
<h3>Signing Up and Getting Your API Key</h3>
 <p>To use the Copyleaks API you need to be a registered user. Signing up is quick and free of charge.</p>
 <p><a href="https://copyleaks.com/Account/Register">Signup</a> to Copyleaks and confirm your account by clicking the link on the confirmation email. Generate your personal API key on your dashboard (<a href="https://api.copyleaks.com/businessesapi">Businesses dashboard/</a><a href="https://api.copyleaks.com/academicapi">Academic dashboard/</a>) under 'Access Keys'. </p>
 <p>For more information check out our <a href="https://api.copyleaks.com/documentation/v3">API guide</a>.</p>
<h3>Example</h3>

<p>The Copyleaks system architecture was built to support the asynchronous model.<br>
The asynchronous model allows you to get notified immediately when your scan status changes, without having to call any other methods.<br>
For more details see: <a href="https://api.copyleaks.com/documentation/v3/webhooks">webhooks</a> and <a href="https://api.copyleaks.com/documentation/v3/activities/single-file-scan">single file scan</a></p>
<p>Submitting a scan:</p>
<pre>
using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Callbacks;
using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Copyleaks.SDK.V3.API.Models.Responses;
using Copyleaks.SDK.V3.API.Models.Types;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ...


#region  Acquiring an API token
LoginResponse loginResponse;
using (var id = new CopyleaksIdentityApi())
{
    loginResponse = await id.LoginAsync(email, apiKey);
}
#endregion


using (var api = new CopyleaksScansApi(eProduct.Businesses, loginResponse.Token))
{
    #region Checking account balance
    var creditBalance = await api.CreditBalanceAsync();
    Console.WriteLine($"You have {creditBalance} credits");
    #endregion

    #region Submitting a file for plagiarism detection
    string fileName = "myFile.txt";
    string filePath = $"/path/to/file/{fileName}";
    // Convert the file to base64 string
    string base64 = Convert.ToBase64String(await File.ReadAllBytesAsync(filePath));
    string scanId = Guid.NewGuid().ToString();
    api.SubmitFileAsync(scanId, new FileDocument {
        // A base64 data string of a file.
        // If you would like to scan plain text, encode it as base64 and submit it.
        Base64 = base64,
        // The name of the file as it will appear in the Copyleaks scan report
        // Make sure to include the right extension for your filetype.
        Filename = fileName,

        PropertiesSection = new EducationScanProperties
        {
            // You can test the integration with the Copyleaks API for free using the sandbox mode.
            Sandbox = true,
            // Register to receive asynchronous notifications
            Webhooks = new Webhooks
            {
                // Triggered on every new result
                NewResult = new Uri($"http://webhook.url.com/results/{scanId}"),
                // Triggerd on every status change, i.e: completed, error, creditsChecked , indexed
                Status = new Uri($"http://webhook.url.com/{{STATUS}}/{scanId}"),
            },
                        
        }
    }).Wait();
    #endregion
</pre>

<p>Waiting for webhooks:</p>

<pre>
[HttpPost]
[Route("http://webhook.url.com/completed/{scanId}")]
public ActionResult Completed([FromRoute]string scanId, [FromBody] CompletedCallback model)
{
	Console.WriteLine($"scan {scanId} has found {model.Results.Score.IdenticalWords} identical copied words");
	// Do something with completed scan...
	return Ok();
}

[HttpPost]
[Route("http://webhook.url.com/indexed/{scanId}")]
public ActionResult Indexed([FromRoute]string scanId, [FromBody] IndexOnlyCallback model)
{
	Console.WriteLine($"scan {scanId} was indexed");
	return Ok();
}

[HttpPost]
[Route("http://webhook.url.com/creditsChecked/{scanId}")]
public ActionResult Credits([FromRoute]string scanId, [FromBody] CreditsCheckCallback model)
{
	Console.WriteLine($"scan {scanId} will consume {model.Credits}");
	// Decide whether or not to trigger the scan...
	return Ok();
}

[HttpPost]
[Route("http://webhook.url.com/error/{scanId}")]
public ActionResult Error([FromRoute]string scanId, [FromBody] ErrorCallback model)
{
	Console.WriteLine($"scan {scanId} completed with error: {model.Error.Message}");
	// Do something  with the error...
	return Ok();
}

[HttpPost]
[Route("http://webhook.url.com/results/{scanId}")]
public ActionResult NewResultWebhook([FromRoute]string scanId, [FromBody] NewResultCallback model)
{
	Console.WriteLine($"scan {scanId} got a new result");
	// Do something with the result...
	return Ok();
}
</pre>

<h3>Dependencies:</h3>
<ul>
<li><a href="https://dotnet.microsoft.com/download/dotnet-core/2.0">.Net Core 2.0</a></li>
</ul>
<h5>Referenced Assemblies:</h5>
<ul>
<li><a href="https://www.nuget.org/packages/Microsoft.Extensions.Configuration">Microsoft.Extensions.Configuration</a></li>
<li><a href="https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json">Microsoft.Extensions.Configuration.Json</a></li>
<li><a href="https://www.nuget.org/packages/Newtonsoft.Json">Newtonsoft.Json</a></li>
<li><a href="https://www.nuget.org/packages/Microsoft.NETCore.App/2.0.0/">Microsoft.NETCore.App</a></li>
</ul>

<h3>Read More</h3>
<ul>
<li><a href="https://api.copyleaks.com/documentation/v3">Copyleaks API guide</a></li>
<li><a href="https://www.nuget.org/packages/Copyleaks/">Copyleaks NuGet package</a></li>
</ul>
