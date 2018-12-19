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
<li>Add <i>CopyleaksAPI</i> NuGet by running the following command in the <a href="http://docs.nuget.org/consume/package-manager-console">Package Manager Console</a></li>
<pre>
Install-Package CopyleaksAPI
</pre>
</ol>
<h3>Signing Up and Getting Your API Key</h3>
 <p>To use the Copyleaks API you need to be a registered user. Signing up is quick and free of charge.</p>
 <p><a href="https://copyleaks.com/Account/Register">Signup</a> to Copyleaks and confirm your account by clicking the link on the confirmation email. Generate your personal API key on your dashboard (<a href="https://api.copyleaks.com/businessesapi">Businesses dashboard/</a><a href="https://api.copyleaks.com/academicapi">Academic dashboard/</a><a href="https://api.copyleaks.com/websitesapi">Websites dashboard</a>) under 'Access Keys'. </p>
 <p>For more information check out our <a href="https://api.copyleaks.com/Guides/HowToUse">API guide</a>.</p>
<h3>Example</h3>
<p>This code will show you how to scan files or URLs for plagiarism:</p>

<pre>
using System;
using System.Threading;
using Copyleaks.SDK.API;
using Copyleaks.SDK.API.Exceptions;
using Copyleaks.SDK.API.Models;
//...

private static void Scan(string email, string apiKey)
{
	CopyleaksCloud copyleaks = new CopyleaksCloud(eProduct.Businesses);
	CopyleaksProcess createdProcess;
	ProcessOptions scanOptions = new ProcessOptions();
	scanOptions.SandboxMode = true; // Sandbox mode --> Read more https://api.copyleaks.com/GeneralDocumentation/RequestHeaders#sandbox-mode
	ResultRecord[] results;
	try
	{
		#region Login to Copyleaks cloud
		Console.Write("Login to Copyleaks cloud...");
		copyleaks.Login(email, apiKey);
		Console.WriteLine("Done!");
		
		#endregion
		
		#region Checking account balance
		
		Console.Write("Checking account balance...");
		uint creditsBalance = copyleaks.Credits;
		Console.WriteLine("Done ({0} credits)!", creditsBalance);
		if (!scanOptions.SandboxMode && creditsBalance == 0)
		{
			Console.WriteLine("ERROR: You do not have enough credits to complete this scan. Your balance is {0}).", creditsBalance);
			Environment.Exit(2);
		}
		
		#endregion
		
		#region callbacks
		
		// Add a URL address to get notified, using callbacks, once the scan results are ready. 
		//Read more https://api.copyleaks.com/GeneralDocumentation/RequestHeaders#callbacks
		//scanOptions.HttpCallback = new Uri("http://callbackurl.com?pid={PID}");
		//scanOptions.InProgressResultsCallback = new Uri("http://callbackurl.com?pid={PID}");
		
		#endregion
		
		#region Submitting a new scan process to the server
		
		// Insert here the URL that you'd like to scan for plagiarism
		createdProcess = copyleaks.CreateByUrl(new Uri("http://urltoscan.com"), scanOptions);
		
		// Insert here the file that you'd like to scan for plagiarism
		//createdProcess = copyleaks.CreateByFile(new FileInfo("C:\FiletoScan.pdf"), scanOptions);
		
		Console.WriteLine("Done (PID={0})!", createdProcess.PID);
		
		#endregion
		
		#region Waiting for server's process completion
		
		// Use this if you are not using callback
		Console.Write("Scanning... ");
		ushort currentProgress;
		while (!createdProcess.IsCompleted(out currentProgress))
		{
			Console.WriteLine(currentProgress + "%");
			Thread.Sleep(5000);
		}
		Console.WriteLine("Done.");
		
		#endregion
		
		#region Processing finished. Getting results
		
		results = createdProcess.GetResults();
		if (results.Length == 0)
		{
			Console.WriteLine("No results.");
		}
		else
		{
			for (int i = 0; i < results.Length; ++i)
			{
				if (results[i].URL != null)
				{
					Console.WriteLine("Url: {0}", results[i].URL);
				}
				Console.WriteLine("Information: {0} copied words ({1}%)", results[i].NumberOfCopiedWords, results[i].Percents);
				Console.WriteLine("Comparison report: {0}", results[i].ComparisonReport);
				Console.WriteLine("Title: {0}", results[i].Title);
				Console.WriteLine("Introduction: {0}", results[i].Introduction);
				Console.WriteLine("Embeded comparison: {0}", results[i].EmbededComparison);
			}
		}
		
		#endregion
	}
	catch (UnauthorizedAccessException)
	{
		Console.WriteLine("Failed!");
		Console.WriteLine("Authentication with the server failed!");
		Console.WriteLine("Possible reasons:");
		Console.WriteLine("* You did not log in to Copyleaks cloud");
		Console.WriteLine("* Your login token has expired");
	}
	catch (CommandFailedException theError)
	{
		Console.WriteLine("Failed!");
		Console.WriteLine("*** Error {0}:", theError.CopyleaksErrorCode);
		Console.WriteLine("{0}", theError.Message);
	}
}
</pre>

<h3>Dependencies:</h3>
<ul>
<li><a href="http://www.microsoft.com/en-us/download/details.aspx?id=30653">.Net Core 2.2</a></li>
</ul>
<h5>Referenced Assemblies:</h5>
<ul>
<li><a href="https://www.nuget.org/packages/Microsoft.Extensions.Configuration">Microsoft.Extensions.Configuration</a></li>
<li><a href="https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json">Microsoft.Extensions.Configuration.Json</a></li>
<li><a href="https://www.nuget.org/packages/Newtonsoft.Json">Newtonsoft.Json</a></li>
</ul>

<h3>Read More</h3>
<ul>
<li><a href="https://api.copyleaks.com/Guides/HowToUse">Copyleaks API guide</a></li>
<li><a href="https://www.nuget.org/packages/CopyleaksAPI/">Copyleaks NuGet package</a></li>
</ul>
