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

using Copyleaks.SDK.V3.API.Exceptions;
using Copyleaks.SDK.V3.API.Extensions;
using Copyleaks.SDK.V3.API.Helpers;
using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Responses;
using Copyleaks.SDK.V3.API.Models.Responses.Download;
using Copyleaks.SDK.V3.API.Models.Responses.Result;
using Copyleaks.SDK.V3.API.Models.Types;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Copyleaks.SDK.V3.API
{
    /// <summary>
    /// This class allows you to connect to Copyleaks API, 
    /// scan for plagiarism and get your scan results. 
    /// </summary>
    public class CopyleaksScansApi: CopyleaksBase
    {
        /// <summary>
        /// The product for scanning the documents
        /// </summary>
        public string Product { get; private set; }

        /// <summary>
        /// Login Token, aquire a login token by invoking by using `CopyleaksIdentityApi.LoginAsync`
        /// </summary>
        public string Token { private get; set; }

        public string CopyleaksApiServer { get; private set; }

        /// <summary>
        /// Conection to Copyleaks scan API
        /// </summary>
        /// <param name="product">The product for scanning the documents</param>
        /// <param name="token">Login Token, aquire a login token by invoking by using `CopyleaksIdentityApi.LoginAsync`</param>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.
        /// Configure you client's certificate at https://copyleaks.com/Manage </param>
        public CopyleaksScansApi(eProduct product, string token, X509Certificate2 clientCertificate = null) : this(product.ToString().ToLower(), token, clientCertificate) { }

        /// <summary>
        /// Conection to Copyleaks scan API
        /// </summary>
        /// <param name="product">The product for scanning the documents</param>
        /// <param name="token">Login Token, aquire a login token by invoking by using `Copyle
        /// <param name="client">Override the underlying http client with custom settings</param>
        public CopyleaksScansApi(string product, string token, HttpClient client) : base(client)
        {
            SetUpService(product, token);
        }

        /// <summary>
        /// Conection to Copyleaks scan API
        /// </summary>
        /// <param name="product">The product for scanning the documents</param>
        /// <param name="token">Login Token, aquire a login token by invoking by using `CopyleaksIdentityApi.LoginAsync`</param>
        /// <param name="clientCertificate">Optional Client certificate to be checked against.
        public CopyleaksScansApi(string product, string token, X509Certificate2 clientCertificate = null) : base(clientCertificate)
        {
            SetUpService(product, token);
        }

        private void SetUpService(string product, string token)
        {
            this.CopyleaksApiServer = ConfigurationManager.Configuration["apiEndPoint"];
            this.Product = product;
            this.Token = token;
        }

        /// <summary>
        /// Get your current credit balance
        /// </summary>
        /// <returns>Current credit balance</returns>
        public async Task<uint> CreditBalanceAsync()
        {
            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/credits";
            Client.AddAuthentication(this.Token);
            var response = await Client.GetAsync(requestUri);
            var credits = await response.ExtractJsonResultsAsync<CountCreditsResponse>();
            return credits.Amount;
        }

        private async Task SubmitAsync(Document model, string requestUri)
        {
            Client.AddAuthentication(this.Token);
            var response = await Client.PutAsync(requestUri,
                new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                throw new CommandFailedException(response);
            }
        }

        /// <summary>
        /// Submitting URL to plagiarism scan
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The url and scan properties</param>
        /// <returns>A task that represents the asynchronous submit operation.</returns>
        public async Task SubmitUrlAsync(string scanId, UrlDocument documentModel)
        {
            if(documentModel.Url == null)
                throw new ArgumentException("Url is mandatory.", nameof(documentModel.Url));
            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/submit/url/{scanId}";
            await SubmitAsync(documentModel, requestUri);
        }

        /// <summary>
        /// Submitting local file or free text to plagiarism scan
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The file or free text encoded in base64 and scan properties</param>
        /// <returns>A task that represents the asynchronous submit operation.</returns>
        public async Task SubmitFileAsync(string scanId, FileDocument documentModel)
        {
            if (documentModel.Base64 == null)
                throw new ArgumentException("Base64 is mandatory.", nameof(documentModel.Base64));
            if (documentModel.Filename == null)
                throw new ArgumentException("Filename is mandatory.", nameof(documentModel.Filename));
            if (this.Product == eProduct.Websites.ToString())
                throw new ArgumentException($"File submition is not applicable for {this.Product}"); 

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/submit/file/{scanId}";
            await SubmitAsync(documentModel, requestUri);
        }

        /// <summary>
        /// Submitting an image containing textual content to plagiarism scan
        /// </summary>
        /// <param name="scanId">A unique scan Id</param>
        /// <param name="documentModel">The image file encoded in base64 and scan properties</param>
        /// <returns>A task that represents the asynchronous submit operation.</returns>
        public async Task SubmitImageOCRAsync(string scanId, FileOcrDocument documentModel)
        {
            if (documentModel.Base64 == null)
                throw new ArgumentException("Base64 is mandatory.", nameof(documentModel.Base64));
            if (documentModel.Filename == null)
                throw new ArgumentException("Filename is mandatory.", nameof(documentModel.Filename));
            if (documentModel.Language == null)
                throw new ArgumentException("Language is mandatory.", nameof(documentModel.Language));
            if (this.Product == eProduct.Websites.ToString())
                throw new ArgumentException($"File submition is not applicable for {this.Product}");

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/submit/ocr/{scanId}";
            await SubmitAsync(documentModel, requestUri);
        }

        /// <summary>
        /// Get the perecent progress of the scan
        /// </summary>
        /// <param name="scanId">The scan Id</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the perecent progress of the scan </returns>
        public async Task<uint> ProgressAsync(string scanId)
        {
            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/{scanId}/progress";
            Client.AddAuthentication(this.Token);
            var response = await Client.GetAsync(requestUri);
            var progress = await response.ExtractJsonResultsAsync<ProgressResponse>();
            return progress.Percents;
        }

        /// <summary>
        /// Deletes the process once it has finished running 
        /// </summary>
        /// <param name="scanIds">A list of completed scan id's to be deleted</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of errors by scan id if errors have occurred</returns>
        public async Task<DeleteResponse> DeleteAsync(string [] scanIds)
        {
            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/delete";
            Client.AddAuthentication(this.Token);
            var response = await Client.PatchAsync(requestUri, new StringContent(JsonConvert.SerializeObject(new
            {
                id = scanIds
            }), Encoding.UTF8, "application/json"));

            return await response.ExtractJsonResultsAsync<DeleteResponse>();
        }

        /// <summary>
        /// Start processes which are in 'price checked' status
        /// </summary>
        /// <param name="token">Login Token, aquire a login token by invoking by using `CopyleaksIdentityApi.LoginAsync`</param>
        /// <param name="model">A model with the list of scan id's to start and error handeling defenition in case one or more scans have a starting error</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of successfully started scans and a list of scans that have failed starting</returns>
        public async Task<StartResponse> StartAsync(StartRequest model)
        {
            if (model.Trigger == null)
                throw new ArgumentException("Trigger is mandatory.", nameof(model.Trigger));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/start";
            Client.AddAuthentication(this.Token);
            var response = await Client.PatchAsync(requestUri, 
                new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
            return await response.ExtractJsonResultsAsync<StartResponse>();
        }

        /// <summary>
        /// Starts a batch scan for a list of 'proceChecked' scans 
        /// </summary>
        /// <param name="model">A model with the list of scan id's to start and error handeling defenition in case one or more scans have a starting error.
        /// The Model also contains an include list of completed scans that will be compared against</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a list of successfully started scans and a list of scans that have failed starting </returns>
        public async Task<StartResponse> StartBatchAsync(StartBatchRequest model)
        {
            if (model.Trigger == null)
                throw new ArgumentException("Trigger is mandatory.", nameof(model.Trigger));

            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/batch/start";
            Client.AddAuthentication(this.Token);
            var response = await Client.PatchAsync(requestUri,
                new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"));
            return await response.ExtractJsonResultsAsync<StartResponse>();
        }

        /// <summary>
        /// Get the scan results from Copyleaks servers.
        /// </summary>
        /// <param name="scanId">A completed scan Id</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a model of the scan result</returns>
        public async Task<Result> ResultAsync(string scanId)
        {
            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/{this.Product}/{scanId}/result";
            Client.AddAuthentication(this.Token);
            var response = await Client.GetAsync(requestUri);
            return await response.ExtractJsonResultsAsync<Result>();
        }

        /// <summary>
        /// Get a suspected result comparison report
        /// </summary>
        /// <param name="scanId">The scan id</param>
        /// <param name="resultId">The result id (can be taken from ResultAsync method)</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a model of the result comparison report</returns>
        public async Task<DownloadResultResponse> DownloadResultAsync(string scanId, string resultId)
        {
            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/downloads/{scanId}/results/{resultId}";
            Client.AddAuthentication(this.Token);
            var response = await Client.GetAsync(requestUri);
            return await response.ExtractJsonResultsAsync<DownloadResultResponse>();
        }

        /// <summary>
        /// Get the your source content comparison report 
        /// </summary>
        /// <param name="scanId">The scan id</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a model of your source content comparison report</returns>
        public async Task<DownloadSourceResponse> DownloadSourceReportAsync(string scanId)
        {
            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/downloads/{scanId}";
            Client.AddAuthentication(this.Token);
            var response = await Client.GetAsync(requestUri);
            return await response.ExtractJsonResultsAsync<DownloadSourceResponse>();
        }

        /// <summary>
        /// Get the pdf report for your scan request
        /// </summary>
        /// <param name="scanId">The scan id</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a stream of the pdf report</returns>
        public async Task<Stream> DownloadPdfReportAsync(string scanId)
        {
            if (this.Product != eProduct.Education.ToString())
                throw new InvalidOperationException($"A pdf report is only available for {eProduct.Education} product");
            string requestUri = $"{this.CopyleaksApiServer}{this.ApiVersion}/downloads/{scanId}/report.pdf";
            Client.AddAuthentication(this.Token);
            var response = await Client.GetAsync(requestUri);
            var reportStream = await response.Content.ReadAsStreamAsync();
            reportStream.Seek(0, SeekOrigin.Begin);
            return reportStream;
        }

        /// <summary>
        /// Get a list of the supported file types.
        /// </summary>
        /// <returns>List of textal file types supported and list of file types supported using OCR </returns>
        public async Task<SupportedTypesResult> GetSupportedFileTypesAsync()
        {
            string requestUri = $"{this.CopyleaksApiServer}v1/miscellaneous/supported-file-types";
            var response = await Client.GetAsync(requestUri);
            return await response.ExtractJsonResultsAsync<SupportedTypesResult>();
        }

        /// <summary>
        /// Get OCR Supported Langauges
        /// </summary>
        /// <returns>List of OCR Supported Langauges</returns>
        public async Task<string[]> GetOcrLanguageListAsync()
        {
            string requestUri = $"{this.CopyleaksApiServer}v1/miscellaneous/ocr-languages-list";
            var response = await Client.GetAsync(requestUri);
            return await response.ExtractJsonResultsAsync<string[]>();
        }
    }
}
