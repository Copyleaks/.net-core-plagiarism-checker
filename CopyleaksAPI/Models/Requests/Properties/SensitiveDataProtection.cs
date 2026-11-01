using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	public class SensitiveDataProtection
	{
		/// <summary>Hide email addresses.</summary>
		[JsonProperty("emailAddress")]
		public bool EmailAddress { get; set; } = false;

		/// <summary>Hide passport numbers.</summary>
		[JsonProperty("passport")]
		public bool Passport { get; set; } = false;

		/// <summary>Hide credit card numbers.</summary>
		[JsonProperty("creditCard")]
		public bool CreditCard { get; set; } = false;

		/// <summary>Hide IP and MAC addresses.</summary>
		[JsonProperty("network")]
		public bool Network { get; set; } = false;

		/// <summary>Hide URLs.</summary>
		[JsonProperty("url")]
		public bool Url { get; set; } = false;

		/// <summary>Hide phone numbers.</summary>
		[JsonProperty("phoneNumber")]
		public bool PhoneNumber { get; set; } = false;

		/// <summary>Hide credentials.</summary>
		[JsonProperty("credentials")]
		public bool Credentials { get; set; } = false;

		/// <summary>Hide driver's license numbers.</summary>
		[JsonProperty("driversLicense")]
		public bool DriversLicense { get; set; } = false;
	}
}
