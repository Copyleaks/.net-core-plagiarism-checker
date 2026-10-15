using System;
using System.Reflection;
using Copyleaks.SDK.V3.API.Models.Requests;
using Copyleaks.SDK.V3.API.Models.Requests.Properties;
using Copyleaks.SDK.V3.API.Models.Types;
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
        public void Pdf_VersionUnset_IsNotSerialized()
        {
            var properties = new ClientScanProperties();
            properties.ReportSection.Create = true;

            var pdf = SerializePdf(properties);

            Assert.IsFalse(pdf.ContainsKey("Version"), pdf.ToString());
            Assert.IsFalse(pdf.ContainsKey("reportVersion"), pdf.ToString());
        }

        [TestMethod]
        public void Pdf_ReportVersionSet_IsSerializedWithoutVersion()
        {
            var properties = new ClientScanProperties();
            properties.ReportSection.Create = true;
            properties.ReportSection.ReportVersion = "v3";

            var pdf = SerializePdf(properties);

            Assert.AreEqual("v3", (string)pdf["reportVersion"]);
            Assert.IsFalse(pdf.ContainsKey("Version"), pdf.ToString());
        }

#pragma warning disable CS0618
        [TestMethod]
        public void Pdf_VersionSet_IsStillSerialized()
        {
            var properties = new ClientScanProperties();
            properties.ReportSection.Create = true;
            properties.ReportSection.Version = ePdfReportVersion.V2;

            var pdf = SerializePdf(properties);

            Assert.AreEqual(2, (int)pdf["Version"]);
        }

        [TestMethod]
        public void Pdf_VersionAndEnum_AreObsolete()
        {
            Assert.IsNotNull(typeof(ReportCustomization).GetProperty(nameof(ReportCustomization.Version)).GetCustomAttribute<ObsoleteAttribute>());
            Assert.IsNotNull(typeof(ePdfReportVersion).GetCustomAttribute<ObsoleteAttribute>());
        }
#pragma warning restore CS0618
    }
}
