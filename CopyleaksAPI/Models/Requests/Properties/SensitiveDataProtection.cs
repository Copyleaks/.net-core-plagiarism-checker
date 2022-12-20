using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Copyleaks.SDK.V3.API.Models.Requests.Properties
{
	public class SensitiveDataProtection
	{
		/// <summary>
		/// Hide email address
		/// </summary>
		[JsonProperty("emailAddress")]
		public bool EmailAddress { get; set; } = false;

		/// <summary>
		/// Hide passport number
		/// </summary>
		[JsonProperty("passport")]
		public bool Passport { get; set; } = false;

		/// <summary>
		/// Hide credit card number
		/// </summary>
		[JsonProperty("creditCard")]
		public bool CreditCard { get; set; } = false;

		/// <summary>
		/// Hide ip and mac addresses
		/// </summary>
		[JsonProperty("network")]
		public bool Network { get; set; } = false;

		/// <summary>
		/// Hide url
		/// </summary>
		[JsonProperty("url")]
		public bool Url { get; set; } = false;

		/// <summary>
		/// Hide phone number
		/// </summary>
		[JsonProperty("phoneNumber")]
		public bool PhoneNumber { get; set; } = false;

		/// <summary>
		/// Hide credentials
		/// </summary>
		[JsonProperty("credentials")]
		public bool Credentials { get; set; } = false;

		/// <summary>
		/// Hide credentials
		/// </summary>
		[JsonProperty("driversLicense")]
		public bool DriversLicense { get; set; } = false;
	}
}
