using System;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Copyleaks.SDK.Demo.Helpers;
using Copyleaks.SDK.Demo.Models.Requests;
using Copyleaks.SDK.Demo.Models.Responses;
using Copyleaks.SDK.V3.API;
using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Models.Callbacks;
using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Copyleaks.SDK.V3.API.Models.Types;
using Microsoft.AspNetCore.Mvc;
using SampleWebApplication.Models;

namespace Copyleaks.SDK.Demo.Controllers
{
    public class CopyleaksDemoController : Controller
    {
        private HttpClient httpClient { get; set; }

        public CopyleaksDemoController()
        {
            httpClient = new HttpClient();
        }
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>Log in to Copyleaks API; a successful login returns the login token.</summary>
        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var response = new LoginResponse();
            try
            {
                Guid temp;
                var validOrEmptyKey = Guid.TryParse(loginModel.Key, out temp) ? loginModel.Key : Guid.Empty.ToString();

                // Request an API token from https://id.copyleaks.com/
                var identity = CopyleaksSDKHttpClients.IdentityClient();
                var loginResponse = await identity.LoginAsync(loginModel.Email, validOrEmptyKey).ConfigureAwait(false);

                var clientApi = CopyleaksSDKHttpClients.APIClient();
                var clientCredits = await clientApi.CreditBalanceAsync(loginResponse.Token).ConfigureAwait(false);

                var submitResponse = new SubmitResponse()
                {
                    Token = loginResponse.Token,
                    ClientCredits = clientCredits,
                };

                return View("Submit", submitResponse);
            }
            catch (CopyleaksHttpException cfe)
            {
                response.ErrorMessage = cfe.Message;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }
            return View("Index", response);
        }

        [HttpPost]
        [Route("/submit")]
        public async Task<IActionResult> Submit(SubmitModel submitModel)
        {
            // Must be unique: a scan id that already exists for this user gets HTTP 409 Conflict.
            string scanId = Guid.NewGuid().ToString();
            var response = new SubmitResponse()
            {
                ScanId = scanId,
                Token = submitModel.Token
            };

            try
            {
                using (var api = new CopyleaksScansApi())
                {
                    await api.SubmitFileAsync(scanId, new FileDocument
                    {
                        Base64 = TextToBase64(submitModel.Text),
                        // Shown in the scan result.
                        Filename = "text.txt",
                        PropertiesSection = GetScanProperties(scanId, submitModel)
                    },
                    submitModel.Token).ConfigureAwait(false);
                }
                var checkResult = new CheckResultsResponse
                {
                    ScanId = scanId
                };
                return View("CheckResult", checkResult);
            }
            catch (CopyleaksHttpException cfe)
            {
                response.ErrorMessage = cfe.Message;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }
            return View(response);
        }

        private ScanProperties GetScanProperties(string scanId, SubmitModel submitModel)
        {
            ScanProperties scanProperties = new ClientScanProperties();

            // Scan, checkCredits (only count the credits it would use) or Index (only store it for future scans).
            // Each one reports to the 'Completion' callback.
            scanProperties.Action = eSubmitAction.Scan;
            scanProperties.Webhooks = new Webhooks
            {
                // Copyleaks API POSTs the scan results to the 'completed' callback; see CompletedProcess.
                NewResult = new Uri($"{submitModel.WebHookHost}/new-results/{scanId}"),
                Status = new Uri($"{submitModel.WebHookHost}/{{status}}/{scanId}")
            };
            // Sandbox mode does not take any credits
            scanProperties.Sandbox = submitModel.Sandbox;

            return scanProperties;
        }

        [HttpPost]
        [Route("/{scanId}/checkResult")]
        public IActionResult CheckResult(string scanId)
        {
            var response = new CheckResultsResponse()
            {
                ScanId = scanId,
                CompletedCallback = ResultFile.GetResults(scanId)
            };
            return View(response);
        }

        /// <summary>Receives the completed callback with the scan results.</summary>
        [HttpPost]
        [Route("/{scanId}/completed")]
        public IActionResult CompletedProcess([FromRoute]string scanId, [FromBody] CompletedCallback scanResults)
        {
            // Do something with the scan results
            ResultFile.SaveResults(scanResults, scanId);
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static string TextToBase64(string text) => Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

        public static X509Certificate2 CreateCertificate(string certificateFilePath, string certificatePassword = null)
        {
            if (!System.IO.File.Exists(certificateFilePath))
                return null;
            if (certificatePassword != null)
                return new X509Certificate2(certificateFilePath, certificatePassword);
            else
                return new X509Certificate2(certificateFilePath);
        }
    }
}