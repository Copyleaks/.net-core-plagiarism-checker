# Changelog

All notable changes to the Copyleaks .NET SDK (NuGet package `Copyleaks`) are listed here.

## 5.0.0

This is a major release. It makes the HTTP layer reliable (one shared connection pool, real retries, timeouts and cancellation), adds the endpoints and fields that were missing, and fixes several models. A few changes need small code updates; see the upgrade notes at the end.

### Added

- **New endpoints.**
  - Image Plagiarism Detection: `CopyleaksImagePlagiarismApi.SubmitAsync` uploads an image (a `Stream` or a `byte[]`) and returns the web pages where copies were found.
  - AI Video Detection: `CopyleaksAiVideoDetectionApi.SubmitAsync` submits a video URL. The result is posted to your webhook as a `CopyleaksAiVideoDetectionResponseModel`.
  - Asynchronous AI Image Detection: `CopyleaksAiImageDetectionApi.SubmitUrlAsync` submits an image URL. The result is posted to your webhook with the shape of `CopyleaksAiImageDetectionResponseModel`.
  - AI Image Detection file upload: `CopyleaksAiImageDetectionApi.SubmitFileAsync` uploads the image as a file (multipart) instead of base64.
  - Resend webhook: `CopyleaksScansApi.ResendWebhookAsync`.
  - AI Text Detection credit balance: `CopyleaksAIDetectionApi.CreditBalanceAsync`.
  - AI Text Detection supported file types: `CopyleaksAIDetectionApi.GetSupportedFileTypesAsync` (no login needed).
  - Cross-language plagiarism languages: `CopyleaksScansApi.GetAllowedCrossLanguagesAsync` (no login needed).
  - Private Cloud Hub repository information: `CopyleaksScansApi.GetRepositoryInfoAsync`.
  - API release notes: `CopyleaksScansApi.GetReleaseNotesAsync` (no login needed).
  - `CopyleaksWritingAssistantApi.GetCorrectionTypesAsync(languageCode)`, which needs no login.
  - Stream overloads for large files: `CopyleaksScansApi.SubmitFileAsync(scanId, stream, filename, properties, token)` and `SubmitImageOCRAsync(scanId, stream, filename, language, properties, token)`. The file is base64-encoded while it is read, so the SDK never holds the whole base64 string in memory.
- **New scan properties** (all optional; when you leave them unset they are not sent, so the API default applies):
  - `WritingFeedback` turns on the Grammar Checker inside a scan, with optional score weights.
  - `Overview` turns on the Gen-AI Overview, with its `Ignore...` flags.
  - `AIGeneratedText.Explain` turns on AI Logic, and `AIGeneratedText.Sensitivity` sets the AI detection sensitivity (1 to 3).
  - `References` turns on the Citation Checker (`Validate`).
  - `InternalAISourceMatch` turns on Internal AI Source Match.
  - `ScanTimeZone`, `Course`, `Assignment` and `Institution`.
  - `ProtectionLayersSection.Include.IdPattern`, `ProtectionLayersSection.Exclude.BacklinksDomains` and `ProtectionLayersSection.Exclude.Text`.
  - `ReportSection.Colors.AIDetection` and `ReportSection.Colors.WritingFeedback`, and `ePdfReportVersion.V3`.
- **New request fields.**
  - `UrlDocument.Verb` (the HTTP method used to fetch the URL).
  - `DeleteRequest.CompletionWebhook` and `DeleteRequest.Headers`.
  - `ExportRequest.AIDetection`, `ExportRequest.WritingFeedback`, `ExportRequest.Overview` and `ExportRequest.CompletionWebhookHeaders`. The exported overview file is modeled as the new `OverviewResult`.
  - `NaturalLanguageDocument.Explain` and `NaturalLanguageDocument.Sensitivity` for AI Text Detection.
  - `CopyleaksAiImageDetectionRequestModel.MaskType` (see `CopyleaksAiImageMaskTypes`) and `ObjectDetection`.
- **New response and webhook fields.**
  - AI Image Detection: `IsAiDetected`, `DetectedObjects`, `Result.Labels` and `ImageInfo.Metadata.ContentSummary`.
  - Completed webhook: `ReferencesValidation` and `Results.InternalAIData`.
  - New result webhook: `DeveloperPayload` and `InternalAIData`.
  - Error webhook: `error.Type`, `error.Id`, `error.Url` and `error.Details`.
  - Crawled version (`DownloadSourceResponse`): `ReferencesValidation`, with the per-reference Citation Checker results.
  - `CopyleaksHttpException` now has `ResponseBody`, `ErrorType`, `ErrorId`, `ErrorCode` and `RetryAfter`.
- **`CopyleaksClientOptions`** sets the per-attempt timeout, the number of retries, the retry delays and, optionally, the API and identity server addresses. Pass it to any API constructor, or register it in dependency injection.
- **`CancellationToken`**: every async method has an optional `cancellationToken` parameter.
- The API classes work as `IHttpClientFactory` typed clients, for example `services.AddHttpClient<CopyleaksScansApi>()`.

### Changed

- **One shared HttpClient.** When you do not pass an `HttpClient`, every API object now uses one shared, pooled client instead of creating a new one per object. You can create API objects freely.
- **Retries.** Calls that are safe to repeat (GET requests, login, start, delete, and submits to a fixed scan id) are retried up to 3 times on network errors, timeouts and HTTP 408, 429, 500, 502, 503 and 504, with exponential backoff and jitter. Calls that are charged per call or trigger an action (AI Text Detection, Grammar Checker, Text Moderation, AI Image Detection, Image Plagiarism Detection, export and resend webhook) are retried only on HTTP 429. The SDK waits as long as the `Retry-After` header asks, and does not retry when it asks for more than 30 seconds.
- **Timeout.** Each attempt now times out after 60 seconds (the `RequestsTimeout` configuration value) and throws a `TimeoutException`. Change it with `CopyleaksClientOptions.Timeout`.
- `pdf.version` is no longer sent unless you set it, so the server generates the latest PDF report version. `ReportCustomization.Version` is now nullable and marked obsolete; use `ReportVersion`.
- `ScanProperties.Priority` is no longer sent unless you set it, so the server default applies. It is now nullable.
- `ScanProperties.Expiration` now defaults to 2880 hours, the API default and maximum (it was 2800).
- Text Moderation requests must have between 1 and 16 labels (the limit was 32), matching the server.
- The supported file types and OCR languages calls use the documented `v3/miscellaneous` paths.
- AI Image Detection JSON requests send `filename`, as documented (it was `fileName`).
- Request headers no longer include `Connection` and `Keep-Alive`, and the client certificate path no longer forces TLS 1.2.
- `ClassificationResult.Probability` is marked obsolete, because the API is removing it.
- The package now declares its MIT license, has an embedded icon and includes the README.
- The package no longer depends on Polly.

### Fixed

- Retries never sent the request again: every retry returned the first result after a fixed delay. Each retry now builds and sends a fresh request.
- Writing score weights defaulted to 0.0, so setting one weight zeroed the others. They now default to 1.0, as documented.
- The new result webhook dropped `developerPayload`, and repository results lost their metadata (including `submittedBy`) because the SDK read a misspelled field name.
- The export completed webhook threw when `httpStatusCode` was null. `HttpStatusCode` is now nullable in `Task` and in the legacy `ExportRow`.
- The SDK version for the User-Agent header was read from disk on every request, which failed in single-file deployments. It is now read once from the assembly metadata.
- Large submits made 5 to 6 in-memory copies of the payload. The body is now serialized and compressed once, straight into pooled buffers.
- Error responses were read with a blocking call. They are now read asynchronously.
- `DownloadPdfReportAsync` returned a stream that belonged to a disposed response. It now returns a stream you own.
- The usage history dates depended on the current culture. They are now always formatted with the invariant culture.
- Scan ids and other path values are now URL-encoded.
- `dotnet pack` failed on a clean checkout (NU5026). It now builds the project first.

### Upgrade notes

- Update the package: `dotnet add package Copyleaks --version 5.0.0`, or `Install-Package Copyleaks -Version 5.0.0`. The package id, the assembly name (`CopyleaksAPIV3`) and the namespaces are unchanged.
- Rebuild your project. Every async method has a new optional `CancellationToken` parameter, and the constructors have a new optional `CopyleaksClientOptions` parameter. Existing calls compile unchanged, but code compiled against 4.x must be recompiled.
- `CopyleaksBase.RetryPolicy` (a Polly policy) was removed. Retries are configured with `CopyleaksClientOptions.MaxRetries`, `RetryBaseDelay` and `MaxRetryDelay`. If your own code used Polly through this package, add a direct `Polly` package reference.
- `Dispose()` on an API object never disposes an `HttpClient`. If you passed in your own client, you still own it. Creating an API object per call is now cheap, but reusing one, or registering it as a typed client, is still recommended.
- Calls now time out after 60 seconds per attempt (it was 100 seconds). If you send very large requests, set `CopyleaksClientOptions.Timeout`.
- `ReportCustomization.Version` is now `ePdfReportVersion?` and defaults to null. If you relied on the v1 report being the default, set `ReportVersion = "v1"`.
- `ScanProperties.Priority` is now `eScanPriority?` and defaults to null.
- `Task.HttpStatusCode` (export completed webhook) is now `int?`, and the legacy `ExportRow.HttpStatusCode` is now `uint?`.
- `NewResultWebhookModel` now derives from `Webhook`.
- `CopyleaksTextModerationRequestModel` throws an `ArgumentException` for more than 16 labels.
- A call that runs out of time now throws `TimeoutException` instead of `TaskCanceledException`. A call you cancel still throws `OperationCanceledException`.
- `WritingAssistantDocument.Score` weights you do not set are now sent as 1.0 instead of 0.0.
