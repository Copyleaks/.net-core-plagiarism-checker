using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CopyleaksAPITests
{
    [TestClass]
    public class PdfReportVersionTests
    {
        private static JObject SerializePdf(ClientScanProperties properties)
        {
            var document = new FileDocument { Base64 = "aGVsbG8=", Filename = "file.txt", PropertiesSection = properties };
            return (JObject)JObject.Parse(JsonConvert.SerializeObject(document))["properties"]["pdf"];
        }

        [TestMethod]
        public void Pdf_ReportVersionUnset_IsNotSerialized()
        {
            var properties = new ClientScanProperties();
            properties.ReportSection.Create = true;

            var pdf = SerializePdf(properties);

            Assert.IsFalse(pdf.ContainsKey("Version"), pdf.ToString());
            Assert.IsFalse(pdf.ContainsKey("reportVersion"), pdf.ToString());
        }

        [TestMethod]
        public void Pdf_ReportVersionV3_IsSerialized()
        {
            var properties = new ClientScanProperties();
            properties.ReportSection.Create = true;
            properties.ReportSection.ReportVersion = "v3";

            var pdf = SerializePdf(properties);

            Assert.AreEqual("v3", (string)pdf["reportVersion"]);
            Assert.IsFalse(pdf.ContainsKey("Version"), pdf.ToString());
        }

        [TestMethod]
        public void Pdf_LegacyVersionAndEnum_AreRemoved()
        {
            Assert.IsNull(typeof(ReportCustomization).GetProperty("Version"));
            Assert.IsNull(typeof(ReportCustomization).Assembly.GetType("Copyleaks.SDK.V3.API.Models.Types.ePdfReportVersion"));
        }
    }
}
