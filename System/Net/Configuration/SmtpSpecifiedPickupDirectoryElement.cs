using System;
using System.Configuration;

namespace System.Net.Configuration
{
	/// <summary>Represents an SMTP pickup directory configuration element.</summary>
	public sealed class SmtpSpecifiedPickupDirectoryElement : ConfigurationElement
	{
		static SmtpSpecifiedPickupDirectoryElement()
		{
			SmtpSpecifiedPickupDirectoryElement.properties.Add(SmtpSpecifiedPickupDirectoryElement.pickupDirectoryLocationProp);
		}

		/// <summary>Gets or sets the folder where applications save mail messages to be processed by the SMTP server.</summary>
		/// <returns>A string that specifies the pickup directory for email messages.</returns>
		[ConfigurationProperty("pickupDirectoryLocation")]
		public string PickupDirectoryLocation
		{
			get
			{
				return (string)base[SmtpSpecifiedPickupDirectoryElement.pickupDirectoryLocationProp];
			}
			set
			{
				base[SmtpSpecifiedPickupDirectoryElement.pickupDirectoryLocationProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return SmtpSpecifiedPickupDirectoryElement.properties;
			}
		}

		private static ConfigurationProperty pickupDirectoryLocationProp = new ConfigurationProperty("pickupDirectoryLocation", typeof(string));

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
