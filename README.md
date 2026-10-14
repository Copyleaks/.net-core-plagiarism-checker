# Copyleaks SDK
The official [Copyleaks](https://copyleaks.com/) .NET library, supporting .NET 8 and later.

Copyleaks is the integrity layer for your tech stack that helps organizations and individuals understand what's human-made and original, AI-generated, or manipulated across text, image, audio and video.

This SDK gives you access to the following Copyleaks products:

*   AI Text Detection
*   AI Image Detection (synchronous, file upload and asynchronous)
*   AI Video Detection
*   Plagiarism Checker (Authenticity scans: file, URL, OCR)
*   Image Plagiarism Detection
*   Grammar Checker
*   Text Moderation

Upgrading from 4.x? See the upgrade notes in the [CHANGELOG](https://github.com/Copyleaks/.net-core-plagiarism-checker/blob/master/CHANGELOG.md).

## 🚀 Getting Started
Before you start, ensure you have the following:

*   An active Copyleaks account. If you don’t have one, [Sign up for free](https://copyleaks.com/signup).
*   You can find your API key on the [API Dashboard](https://api.copyleaks.com/dashboard).

Once you have your account and API key:

**Install the SDK**:

```bash
dotnet add package Copyleaks
```

Or, in the Package Manager Console:
```bash
Install-Package Copyleaks
```

## 📚 Documentation
To learn more about how to use Copyleaks API please check out our [Documentation](https://docs.copyleaks.com/resources/sdks/csharp/).

## Creating the API clients
The SDK has one class per product: `CopyleaksIdentityApi` (login), `CopyleaksScansApi` (plagiarism scans, exports and lookups), `CopyleaksAIDetectionApi`, `CopyleaksAiImageDetectionApi`, `CopyleaksAiVideoDetectionApi`, `CopyleaksImagePlagiarismApi`, `CopyleaksWritingAssistantApi` and `CopyleaksTextModerationApi`.

API objects are thread-safe. When you do not pass an `HttpClient`, they all share one pooled `HttpClient`, so creating them is cheap. Still, create them once and reuse them:

```csharp
using Copyleaks.SDK.V3.API;

var identity = new CopyleaksIdentityApi();
var scans = new CopyleaksScansApi();
```

In ASP.NET Core, register them as typed clients with `IHttpClientFactory`:

```csharp
using Copyleaks.SDK.V3.API;

builder.Services.AddSingleton(new CopyleaksClientOptions { MaxRetries = 3 }); // optional
builder.Services.AddHttpClient<CopyleaksIdentityApi>();
builder.Services.AddHttpClient<CopyleaksScansApi>();
builder.Services.AddHttpClient<CopyleaksAIDetectionApi>();

// Then inject them:
public class ScanService(CopyleaksIdentityApi identity, CopyleaksScansApi scans) { /* ... */ }
```

You can also pass your own `HttpClient`: `new CopyleaksScansApi(httpClient)`. The SDK never disposes a client you pass in.

### Timeouts, retries and cancellation
`CopyleaksClientOptions` controls the HTTP behavior:

```csharp
var options = new CopyleaksClientOptions
{
    Timeout = TimeSpan.FromSeconds(60),          // per attempt; the default comes from ConfigurationManager ("RequestsTimeout", 60 seconds)
    MaxRetries = 3,                              // 0 turns retries off
    RetryBaseDelay = TimeSpan.FromSeconds(1),    // exponential backoff with jitter
    MaxRetryDelay = TimeSpan.FromSeconds(30),    // longest wait, including Retry-After
};
var scans = new CopyleaksScansApi(options: options);
```

* Calls that are safe to repeat (GET requests, login, start, delete, and submits to a fixed scan id) are retried on network errors, timeouts and HTTP 408, 429, 500, 502, 503 and 504.
* Calls that are charged per call or trigger an action (AI Text Detection, Grammar Checker, Text Moderation, AI Image Detection, Image Plagiarism Detection, export and resend webhook) are retried only on HTTP 429.
* The SDK honors the `Retry-After` header. When it asks for a longer wait than `MaxRetryDelay`, the SDK returns the error instead of waiting.
* A request that runs out of time throws `TimeoutException`. An error response throws `CopyleaksHttpException`, with `HttpErrorCode`, `ResponseBody`, `ErrorType`, `ErrorId`, `ErrorCode`, `RetryAfter` and `Attempt` (the 1-based attempt that got the error).
* An attempt can time out after the server accepted it. The retry of such a submit (or start) then fails with 409 Conflict. A 409 with `Attempt` greater than 1 means an earlier attempt was accepted, so treat the call as successful:

```csharp
try
{
    await scans.SubmitFileAsync(scanId, document, token);
}
catch (CopyleaksHttpException ex) when (ex.HttpErrorCode == HttpStatusCode.Conflict && ex.Attempt > 1)
{
    // An earlier attempt created the scan; only its response was lost.
}
```

* The PDF report and usage history downloads are streamed. Their timeout also covers reading the body, and a failure while the body is read is not retried.
* Every async method takes an optional `CancellationToken`:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
uint credits = await scans.CreditBalanceAsync(token, cts.Token);
```

## 💡 Usage Examples
Here are some common usage examples for the Copyleaks SDK. You can also see a comprehensive code example in the SampleWebApplication.csproj on our GitHub repository: [SampleWebApplication](https://github.com/Copyleaks/.net-core-plagiarism-checker/tree/master/SampleWebApplication).

### Get Authentication Token

This example demonstrates how to log in to the Copyleaks API and obtain an authentication token.

```csharp
using System;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API;

public class Program
{
    // --- Your Credentials ---
    private const string USER_EMAIL = "YOUR_EMAIL_ADDRESS";
    private const string USER_KEY = "YOUR_API_KEY";
    // --------------------

    public static async Task Main(string[] args)
    {
        // Log in to the Copyleaks API
        Console.WriteLine("Authenticating...");
        var identityClient = new CopyleaksIdentityApi();
        var loginResponse = await identityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
        string authToken = loginResponse.Token; // valid for 48 hours; reuse it
        Console.WriteLine("Logged in successfully!");
    }
}
```
For a detailed understanding of the authentication process, refer to the Copyleaks Login Endpoint [Documentation](https://docs.copyleaks.com/reference/actions/account/login).
##
### Submit Text for Plagiarism Scan
This example shows how to prepare and submit raw text content for a plagiarism scan.

```csharp
using System;
using System.Text;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Copyleaks.SDK.V3.API.Models.Types;

public class Program
{
    // --- Your Credentials ---
    private const string USER_EMAIL = "YOUR_EMAIL_ADDRESS";
    private const string USER_KEY = "YOUR_API_KEY";
    private const string WEBHOOK_URL = "https://your-server.com/webhook/{STATUS}";
    // --------------------

    public static async Task Main(string[] args)
    {
        var identityClient = new CopyleaksIdentityApi();
        var scansClient = new CopyleaksScansApi();

        var loginResponse = await identityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
        var authToken = loginResponse.Token;

        // A unique scan ID for the scan
        // In case this scan ID already exists for this user Copyleaks API will return HTTP 409 Conflict result
        string scanId = Guid.NewGuid().ToString();
        string scannedText = "Hello world";

        ClientScanProperties clientScanProperties = new ClientScanProperties();

        clientScanProperties.Action = eSubmitAction.Scan;
        clientScanProperties.Webhooks = new Webhooks
        {
            // Copyleaks API will POST the scan results to the 'completed' callback
            Status = new Uri(WEBHOOK_URL)
        };

        // Sandbox mode does not take any credits
        clientScanProperties.Sandbox = true;

        clientScanProperties.ReportSection.Create = true;

        // Submit a file for scan in https://api.copyleaks.com
        await scansClient.SubmitFileAsync(scanId, new FileDocument
        {
            Base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(scannedText)),
            Filename = "text.txt",
            PropertiesSection = clientScanProperties
        },
        authToken).ConfigureAwait(false);
    }
}
```

For large files, submit a stream instead of a base64 string. The SDK encodes the file while it reads it:

```csharp
using (var file = File.OpenRead("thesis.pdf"))
    await scansClient.SubmitFileAsync(scanId, file, "thesis.pdf", clientScanProperties, authToken);
```

For a full guide please refer to our step by step [Guide](https://docs.copyleaks.com/guides/authenticity/detect-plagiarism-text)

For a detailed understanding of the plagiarism detection process, refer to the Copyleaks Submit Endpoint [Documentation](https://docs.copyleaks.com/reference/actions/authenticity/submit-file)
##
### More scan features
Every feature below is optional. Properties you leave unset are not sent, so the API default applies.

```csharp
using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Copyleaks.SDK.V3.API.Models.Requests.WritingAssistant;

var properties = new ClientScanProperties
{
    Sandbox = true,
    Webhooks = new Webhooks { Status = new Uri("https://your-server.com/webhook/{STATUS}") },

    // AI detection, with AI Logic explanations and a sensitivity from 1 to 3
    AIGeneratedText = new AIGeneratedText { Detect = true, Sensitivity = 2, Explain = new AIExplainSettings { Enable = true } },

    // Grammar Checker inside the scan (weights default to 1.0)
    WritingFeedback = new WritingFeedbackSettings { Enable = true, Score = new Score { GrammarScoreWeight = 0.5 } },

    // Gen-AI Overview of the scan
    Overview = new OverviewSettings { Enable = true },

    // Citation Checker
    References = new ReferencesSettings { Validate = true },

    // AI Source Match (online) and Internal AI Source Match
    AISourceMatch = new AISourceMatch { Enable = true },
    InternalAISourceMatch = new InternalAISourceMatch { Enable = true },

    // PDF report time zone, and analytics ids
    ScanTimeZone = "America/New_York",
    Course = new CourseEntity { Id = "course-101" },
    Assignment = new AssignmentEntity { Id = "essay-3" },
    Institution = new InstitutionEntity { Id = "my-university" },
};

// Only compare against internal database and repository results whose scan id matches a pattern,
// and drop results that link to some domains or contain some phrases
properties.ProtectionLayersSection.Include = new ScanningInclude { IdPattern = "2026-*" };
properties.ProtectionLayersSection.Exclude.BacklinksDomains = new[] { "example.com" };
properties.ProtectionLayersSection.Exclude.Text = new[] { "lorem ipsum" };

// PDF report: pick the version and the highlight colors
properties.ReportSection.Create = true;
properties.ReportSection.ReportVersion = "latest";
properties.ReportSection.Colors = new ReportCustomizationColors { AIDetection = "#FE5758", WritingFeedback = "#FF9A02" };
```

The completed webhook has `ReferencesValidation` (Citation Checker summary) and `Results.InternalAIData`. Export the AI detection, Grammar Checker and overview reports with the export call:

```csharp
await scansClient.ExportAsync(scanId, "export-1", new ExportRequest
{
    CompletionWebhook = new Uri("https://your-server.com/export/completed"),
    AIDetection = new ExportTask { Verb = "PUT", Endpoint = new Uri("https://your-server.com/export/ai") },
    WritingFeedback = new ExportTask { Verb = "PUT", Endpoint = new Uri("https://your-server.com/export/grammar") },
    Overview = new ExportTask { Verb = "PUT", Endpoint = new Uri("https://your-server.com/export/overview") },
    CrawledVersion = new ExportTask { Verb = "PUT", Endpoint = new Uri("https://your-server.com/export/crawled") },
}, authToken);
```

The uploaded files are an `AIDetectionResult`, a `WritingAssistantResult`, an `OverviewResult` and a `DownloadSourceResponse` (which includes the per-reference Citation Checker results).

If a webhook did not arrive, ask for it again (for scans that completed, failed, were indexed or were price-checked):

```csharp
await scansClient.ResendWebhookAsync(scanId, authToken);
```
##
### Reading the AI alert from the completed webhook
When a scan runs AI detection (`clientScanProperties.AIGeneratedText.Detect = true`), the completed webhook reports AI-generated text as a `suspected-ai-text` alert. The alert's `additionalData` is a JSON string, and the SDK decodes it into an `AIDetectionResult`.

```csharp
using System;
using Copyleaks.SDK.V3.API.Models.Responses.Webhooks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[ApiController]
[Route("/webhook")]
public class WebhookController : ControllerBase
{
    [HttpPost]
    [Route("completed/{scanId}")]
    public IActionResult CompletedWebhookProcessor([FromRoute] string scanId, [FromBody] CompletedWebhookModel completedWebhook)
    {
        // The first "suspected-ai-text" alert, or null when the scan produced no AI alert
        var aiAlert = completedWebhook.GetAIDetectionAlert();
        if (aiAlert != null)
        {
            try
            {
                // additionalData decoded into AIDetectionResult. Same as completedWebhook.GetAIDetectionResult().
                // null when the alert has no data; throws a Newtonsoft JsonException when additionalData is not valid JSON or does not match the AIDetectionResult shape.
                var aiResult = aiAlert.GetAIDetectionResult();
                if (aiResult != null && aiResult.Summary != null)
                {
                    Console.WriteLine($"AI: {aiResult.Summary.AI:P0}, human: {aiResult.Summary.Human:P0}, model: {aiResult.ModelVersion}");
                    foreach (var section in aiResult.Results ?? new())
                    {
                        // Classification: 1 = human, 2 = AI
                        Console.WriteLine($"Classification {section.Classification}, matches: {section.Matches?.Count ?? 0}");
                    }
                }
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"Could not decode the AI alert of scan {scanId}: {ex.Message}");
            }
        }
        // Acknowledge the webhook even when the AI alert could not be decoded.
        return Ok();
    }
}
```
* `GetAIDetectionAlert()` returns null when the completed webhook contains no suspected-ai-text alert.
* `GetAIDetectionResult()` returns null when the alert has no data or when the data is valid JSON that is not a JSON object. It throws a Newtonsoft `JsonException` when `additionalData` is not valid JSON or does not match the `AIDetectionResult` shape. The raw string stays available in `aiAlert.AdditionalData`.
* The legacy `CompletedCallback` model has the same `GetAIDetectionResult()` method on each `AlertNotification`.

For more details, refer to the Copyleaks scan completed webhook [Documentation](https://docs.copyleaks.com/reference/data-types/authenticity/webhooks/scan-completed)
##
##
### AI Text Detection
Use the AI detection client to determine if content was generated by artificial intelligence.

```csharp
using System;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Requests.AIDetection;

public class Program
{
    // --- Your Credentials ---
    private const string USER_EMAIL = "YOUR_EMAIL_ADDRESS";
    private const string USER_KEY = "YOUR_API_KEY";
    // --------------------

    public static async Task Main(string[] args)
    {
        var identityClient = new CopyleaksIdentityApi();
        var aiDetectionClient = new CopyleaksAIDetectionApi();

        var loginResponse = await identityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
        var authToken = loginResponse.Token;
        string scanId = Guid.NewGuid().ToString();
        var text = @"Lions are social animals, living in groups called prides, typically consisting of several females, their offspring, and a few males. Female lions are the primary hunters, working together to catch prey. Lions are known for their strength, teamwork, and complex social structures.";

        var naturalLanguageDocument = new NaturalLanguageDocument()
        {
            Text = text,
            Sandbox = true,
            Explain = true,     // AI Logic (optional)
            Sensitivity = 2,    // 1 to 3 (optional)
        };

        var res = await aiDetectionClient.SubmitNaturalLanguageAsync(scanId, naturalLanguageDocument, authToken).ConfigureAwait(false);
        Console.WriteLine($"AI: {res.Summary.AI:P0}");

        // The AI Text Detection credit balance, and the file types AI detection supports in scans
        uint aiCredits = await aiDetectionClient.CreditBalanceAsync(authToken);
        var aiFileTypes = await aiDetectionClient.GetSupportedFileTypesAsync(); // no login needed
    }
}
```
For a full guide please refer to our step by step [Guide](https://docs.copyleaks.com/guides/ai-detector/ai-text-detection/)

For a detailed understanding of the AI Text Detection process, refer to the Copyleaks detect natural language Endpoint [Documentation](https://docs.copyleaks.com/reference/actions/writer-detector/check/)
##
### Grammar Checker (CopyleaksWritingAssistantApi)
Get intelligent suggestions for improving grammar, spelling, style, and overall writing quality.

```csharp
using System;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Requests.WritingAssistant;

public class Program
{
    // --- Your Credentials ---
    private const string USER_EMAIL = "YOUR_EMAIL_ADDRESS";
    private const string USER_KEY = "YOUR_API_KEY";
    // --------------------

    public static async Task Main(string[] args)
    {
        var identityClient = new CopyleaksIdentityApi();
        var writingAssistantClient = new CopyleaksWritingAssistantApi();

        var loginResponse = await identityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
        var authToken = loginResponse.Token;
        string scanId = Guid.NewGuid().ToString();
        var text = @"Lions are the only cat that live in groups, called pride. A prides typically consists of a few adult males, several feales, and their offspring. This social structure is essential for hunting and raising young cubs. Female lions, or lionesses are the primary hunters of the prid. They work together in cordinated groups to take down prey usually targeting large herbiores like zbras, wildebeest and buffalo. Their teamwork and strategy during hunts highlight the intelligence and coperation that are key to their survival.";

        var document = new WritingAssistantDocument()
        {
            Sandbox = true,
            Text = text,
            // Optional. Every weight you do not set stays at 1.0.
            Score = new Score()
            {
                SentenceStructureScoreWeight = 0.2,
                GrammarScoreWeight = 0.3,
                MechanicsScoreWeight = 0.4,
                WordChoiceScoreWeight = 0.5,
            },
        };

        var res = await writingAssistantClient.SubmitTextAsync(scanId, document, authToken).ConfigureAwait(false);

        // The correction types are public, so no login is needed
        var correctionTypes = await writingAssistantClient.GetCorrectionTypesAsync("en");
    }
}
```
For a full guide please refer to our step by step [Guide](https://docs.copyleaks.com/guides/writing/check-grammar/)

For a detailed understanding of the Grammar Checker (CopyleaksWritingAssistantApi) process, refer to the Copyleaks writing feedback Endpoint [Documentation](https://docs.copyleaks.com/reference/actions/writing-assistant/check/)
##
### Text Moderation
Scan and moderate text content for unsafe, inappropriate, or policy-violating material across various categories. Send between 1 and 16 labels.

```csharp
using System;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Requests.TextModeration;

public class Program
{
    // --- Your Credentials ---
    private const string USER_EMAIL = "YOUR_EMAIL_ADDRESS";
    private const string USER_KEY = "YOUR_API_KEY";
    // --------------------

    public static async Task Main(string[] args)
    {
        var identityClient = new CopyleaksIdentityApi();
        var textModerationClient = new CopyleaksTextModerationApi();

        var loginResponse = await identityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
        var authToken = loginResponse.Token;
        string scanId = Guid.NewGuid().ToString();

        var labelsArray = new CopyleaksTextModerationLabel[]
        {
            new CopyleaksTextModerationLabel(CopyleaksTextModerationConstants.ADULT_V1),
            new CopyleaksTextModerationLabel(CopyleaksTextModerationConstants.TOXIC_V1),
            new CopyleaksTextModerationLabel(CopyleaksTextModerationConstants.VIOLENT_V1),
            new CopyleaksTextModerationLabel(CopyleaksTextModerationConstants.PROFANITY_V1),
            new CopyleaksTextModerationLabel(CopyleaksTextModerationConstants.SELF_HARM_V1),
            new CopyleaksTextModerationLabel(CopyleaksTextModerationConstants.HARASSMENT_V1),
            new CopyleaksTextModerationLabel(CopyleaksTextModerationConstants.HATE_SPEECH_V1),
            new CopyleaksTextModerationLabel(CopyleaksTextModerationConstants.DRUGS_V1),
            new CopyleaksTextModerationLabel(CopyleaksTextModerationConstants.FIREARMS_V1),
            new CopyleaksTextModerationLabel(CopyleaksTextModerationConstants.CYBERSECURITY_V1)
        };

        var model = new CopyleaksTextModerationRequestModel(
            text: "This is some text to scan.",
            sandbox: true,
            language: CopyleaksTextModerationLanguages.ENGLISH,
            labels: labelsArray
        );

        var result = await textModerationClient.SubmitTextAsync(scanId, model, authToken).ConfigureAwait(false);
    }
}
```
For a full guide please refer to our step by step [Guide](https://docs.copyleaks.com/guides/moderation/moderate-text/)

For a detailed understanding of the Text moderation process, refer to the Copyleaks text moderation Endpoint [Documentation](https://docs.copyleaks.com/reference/actions/text-moderation/check/)
##

### AI Image Detection
Determine if a given image was generated or partially generated by an AI.

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Requests.AiImageDetection;

public class Program
{
    // --- Your Credentials ---
    private const string USER_EMAIL = "YOUR_EMAIL_ADDRESS";
    private const string USER_KEY = "YOUR_API_KEY";
    // --------------------

    public static async Task Main(string[] args)
    {
        var identityClient = new CopyleaksIdentityApi();
        var imageDetectionClient = new CopyleaksAiImageDetectionApi();

        var loginResponse = await identityClient.LoginAsync(USER_EMAIL, USER_KEY).ConfigureAwait(false);
        var authToken = loginResponse.Token;
        string scanId = Guid.NewGuid().ToString();

        // Upload the image as a file (recommended: no base64 overhead)
        var model = new CopyleaksAiImageDetectionRequestModel(
            fileName: "my-image.png",
            model: CopyleaksAiImageDetectionModels.AI_IMAGE_1_ULTRA,
            sandbox: true)
        {
            MaskType = CopyleaksAiImageMaskTypes.HEATMAP,     // optional: "binary" (default), "heatmap" or "none"
            ObjectDetection = new List<CopyleaksAiImageObjectDetectionItem>   // optional: up to 3 objects
            {
                new CopyleaksAiImageObjectDetectionItem("face"),
            },
        };

        using (var image = File.OpenRead("PATH TO YOUR IMAGE"))
        {
            var result = await imageDetectionClient.SubmitFileAsync(scanId, image, model, authToken).ConfigureAwait(false);
            Console.WriteLine($"AI detected: {result.IsAiDetected}, AI share: {result.Summary.Ai:P0}");
        }

        // Or send base64 JSON, as before:
        // var json = new CopyleaksAiImageDetectionRequestModel(base64Image, "my-image.png", CopyleaksAiImageDetectionModels.AI_IMAGE_1_ULTRA, sandbox: true);
        // var result = await imageDetectionClient.SubmitAsync(scanId, json, authToken);
    }
}
```

To analyze an image URL asynchronously, submit it and receive the result on your webhook (with the same shape as `CopyleaksAiImageDetectionResponseModel`):

```csharp
using Copyleaks.SDK.V3.API.Models.Requests.Shared;

await imageDetectionClient.SubmitUrlAsync(scanId, new CopyleaksAiImageDetectionAsyncRequestModel
{
    Url = new Uri("https://example.com/image1.png"),
    FileName = "image1.png",
    Model = CopyleaksAiImageDetectionModels.AI_IMAGE_1_ULTRA,
    Webhooks = new CopyleaksAsyncWebhooks { Url = new Uri("https://your-server.com/webhook/ai-image") },
    Sandbox = true,
}, authToken);
```
If the asynchronous detection failed, the webhook body has `Error` (`Type`, `Id`, `Code`, `Message`, and the optional `Url` and `Details`) instead of the result, so check `Error` first. `DeveloperPayload` echoes the value from the submit request.

For a full guide please refer to our step by step [Guide](https://docs.copyleaks.com/guides/ai-detector/ai-image-detection/)

For a detailed understanding of the AI Image Detection process, refer to the Copyleaks image detection Endpoint [Documentation](https://docs.copyleaks.com/reference/actions/ai-image-detector/check/)
##
### AI Video Detection
Submit a video URL. The result is posted to your webhook as a `CopyleaksAiVideoDetectionResponseModel`.

```csharp
using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Constants;
using Copyleaks.SDK.V3.API.Models.Requests.AiVideoDetection;
using Copyleaks.SDK.V3.API.Models.Requests.Shared;

var videoClient = new CopyleaksAiVideoDetectionApi();
await videoClient.SubmitAsync("my-video-scan-1", new CopyleaksAiVideoDetectionRequestModel
{
    Url = new Uri("https://example.com/my-video.mp4"),
    Filename = "my-video.mp4",
    Model = CopyleaksAiVideoDetectionModels.AI_VIDEO_1_PRO,
    Webhooks = new CopyleaksAsyncWebhooks { Url = new Uri("https://your-server.com/webhook/ai-video") },
    Sandbox = true,
}, authToken);
```

In your webhook, read the body as a `CopyleaksAiVideoDetectionResponseModel` (namespace `Copyleaks.SDK.V3.API.Models.Responses.AiVideoDetection`). Check `Error` first: when the detection failed, the body has only `Error` (`Type`, `Id`, `Code`, `Message`, and the optional `Url` and `Details`) and `DeveloperPayload`, which echoes the value from the submit request.

```csharp
var result = JsonConvert.DeserializeObject<CopyleaksAiVideoDetectionResponseModel>(body);
if (result.Error != null)
    Console.WriteLine($"Detection failed: {result.Error.Id} ({result.Error.Code}): {result.Error.Message}");
else
    Console.WriteLine($"AI detected: {result.IsAiDetected}, overall AI share: {result.Summary.OverallAIRatio:P0}");
```

For a detailed understanding of the AI Video Detection process, refer to the Copyleaks AI video detection Endpoint [Documentation](https://docs.copyleaks.com/reference/actions/ai-video-detector/submit/)
##
### Image Plagiarism Detection
Search the web for copies of an image, in one synchronous call.

```csharp
using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Models.Requests.ImagePlagiarism;

var imagePlagiarismClient = new CopyleaksImagePlagiarismApi();
using (var image = File.OpenRead("my-photo.jpg"))
{
    var result = await imagePlagiarismClient.SubmitAsync("my-scan-1", image,
        new CopyleaksImagePlagiarismRequestModel("my-photo.jpg", sandbox: true), authToken);

    Console.WriteLine($"Total matches: {result.Matches.Score.TotalMatches}");
    foreach (var match in result.Matches.Internet)
        Console.WriteLine($"{(match.MatchType == 0 ? "Full" : "Partial")} match: {match.Url}");
}
```

For a detailed understanding of the Image Plagiarism Detection process, refer to the Copyleaks image plagiarism Endpoint [Documentation](https://docs.copyleaks.com/reference/actions/image-plagiarism-detector/check/)
##
### Account and lookup calls

```csharp
var scansClient = new CopyleaksScansApi();

uint credits = await scansClient.CreditBalanceAsync(authToken);

// Private Cloud Hub repository information (requires the Admin or Super Admin role)
var repository = await scansClient.GetRepositoryInfoAsync("my-repository-id", authToken);

// Public lookups, no login needed
var fileTypes = await scansClient.GetSupportedFileTypesAsync();
string[] ocrLanguages = await scansClient.GetOcrLanguageListAsync();
var crossLanguages = await scansClient.GetAllowedCrossLanguagesAsync();   // codes in crossLanguages.ResultLanguageCodes
var releaseNotes = await scansClient.GetReleaseNotesAsync();
```
##
### Further Resources

*   **Copyleaks API Dashboard:** Manage your API keys, monitor usage, and view analytics from your personalized dashboard. [Access Dashboard](https://api.copyleaks.com/dashboard)
*   **Copyleaks SDK Documentation:** Explore comprehensive guides, API references, and code examples for seamless integration. [Read Documentation](https://docs.copyleaks.com/resources/sdks/overview/)

##
### Support
* If you need assistance, please contact Copyleaks Support via our support portal: Contact Copyleaks [Support](https://help.copyleaks.com/s/contactsupport).
* To arrange a product demonstration, book a demo here: [Booking Link](https://copyleaks.com/book-a-demo).
