# Changelog

All notable changes to the Copyleaks .NET SDK (NuGet package `Copyleaks`) are listed here.

## 4.2.0

This release adds new submit properties and a way to read the AI alert from the completed webhook. All changes are additions, so existing code keeps working without changes.

### Added

- **New submit properties (BAC-5378).**
  - `ScanProperties.AISourceMatch`, set to `new AISourceMatch { Enable = true }`: finds online sources suspected of containing AI-generated text. It currently applies only to documents detected as English. Leave it null to get the server default (disabled).
  - `ScanProperties.DisplayLanguage`: the language of the scan's PDF report. One of `"en"`, `"es"`, `"pt"`, `"fr"`, `"de"`, `"it"` (lowercase, case-sensitive). Leave it null to get the server default (`"en"`). Do not set an empty string, because it is sent as is.
  - `ReportCustomization.ReportVersion`: which version of the PDF report to generate. One of `"v1"`, `"v2"`, `"v3"`, `"latest"` (lowercase, case-sensitive). It overrides `Version` when set. When it is null, the server uses `Version`, which the SDK always sends (default V1).
  - The new properties are left out of the request when null.
- **AI alert in the completed webhook (BAC-5392).**
  - `CompletedWebhookModel.GetAIDetectionAlert()` returns the `suspected-ai-text` alert, or null when the scan has no AI alert.
  - `Alerts.GetAIDetectionResult()` and the shortcut `CompletedWebhookModel.GetAIDetectionResult()` decode the alert's `additionalData` JSON string into an `AIDetectionResult`, with `Summary.AI`, `Summary.Human`, `ModelVersion` and `Results`. They return null when there is no AI alert, or when the data is empty or is JSON that is not an object. They throw a Newtonsoft `JsonException` when `additionalData` is not valid JSON or does not match the `AIDetectionResult` shape. The raw string stays available in `AdditionalData`.
  - The legacy `CompletedCallback` model has the same `GetAIDetectionResult()` method on each `AlertNotification`.
  - New constant `CopyleaksAlertCodes.SUSPECTED_AI_TEXT` (`"suspected-ai-text"`) in `Copyleaks.SDK.V3.API.Models.Constants`.
  - `AIDetectionResult` has new properties: `TranslationProvider`, `Translation` and `Explain` (AI Logic patterns, as the new `AIExplain` classes). `Match` has a new `Html` property with the section positions in the HTML version of the document, and `MatchPositions` has a new `GroupIds` property.

### Changed

- **NuGet package metadata (BAC-5374).** The package description now reads "The official Copyleaks .NET SDK for AI Text Detection, AI Image Detection, Plagiarism Checker, Grammar Checker and Text Moderation." New package tags: `ai-text-detection`, `ai-image-detection`, `grammar-checker`, `text-moderation`. The package ID is still `Copyleaks`.
- **README (BAC-5374).** The README now describes the SDK as the Copyleaks platform SDK and lists the products it covers. Headings such as "AI Text Detection" now use the product names.
- **User-Agent.** The SDK sends `copyleaks-core-sdk/4.2.0`, because the User-Agent is built from the assembly file version.

### Upgrade notes

- Update the package: `dotnet add package Copyleaks --version 4.2.0`, or `Install-Package Copyleaks -Version 4.2.0`.
- No code changes are needed. The namespaces, the assembly name (`CopyleaksAPIV3`) and the existing APIs are unchanged.
