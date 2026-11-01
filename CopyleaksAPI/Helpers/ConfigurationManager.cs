using Copyleaks.SDK.V3.API.Models.Constants;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace Copyleaks.SDK.V3.API.Helpers
{
    /// <summary>SDK settings: server addresses, timeout and API versions.</summary>
    public static class ConfigurationManager
    {
        #region Constants
        public static string apiEndPoint = "https://api.copyleaks.com/";

        public static string IdEndPoint = "https://id.copyleaks.com/";

        public const string RequestsTimeout = "60000";

        public const string apiVersion = "v3";

        public const string aiDetectionApiVersion = "v2";

        public const string writingAssitantApiVersion = "v1";

        public const string textModerationApiVersion = "v1";

        public const string imageDetectionApiVersion = "v1";

        public const string videoDetectionApiVersion = "v1";

        public const string imagePlagiarismApiVersion = "v1";


        #endregion
        static ConfigurationManager()
        {
            Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { CopyleaksConstants.ApiEndPoint, apiEndPoint },
                    { CopyleaksConstants.IdEndPoint,  IdEndPoint },
                    { CopyleaksConstants.RequestsTimeout, RequestsTimeout},
                    { CopyleaksConstants.ApiVersion, apiVersion },
                    { CopyleaksConstants.AiDetectionApiVersion, aiDetectionApiVersion},
                    { CopyleaksConstants.WritingAssistantApiVersion, writingAssitantApiVersion},
                    { CopyleaksConstants.TextModerationApiVersion,textModerationApiVersion},
                    {CopyleaksConstants.ImageDetectionApiVersion,imageDetectionApiVersion },
                    { CopyleaksConstants.VideoDetectionApiVersion, videoDetectionApiVersion },
                    { CopyleaksConstants.ImagePlagiarismApiVersion, imagePlagiarismApiVersion }
                }).Build();
        }

        /// <summary>The settings, for example <c>ConfigurationManager.Configuration["apiEndPoint"]</c>.</summary>
        public static IConfiguration Configuration { get; set; }
    }
}
