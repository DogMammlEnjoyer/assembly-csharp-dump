using System;
using System.Configuration;

namespace System.Net.Configuration
{
	/// <summary>Determines whether Internet Protocol version 6 is enabled on the local computer. This class cannot be inherited.</summary>
	public sealed class Ipv6Element : ConfigurationElement
	{
		static Ipv6Element()
		{
			Ipv6Element.properties = new ConfigurationPropertyCollection();
			Ipv6Element.properties.Add(Ipv6Element.enabledProp);
		}

		/// <summary>Gets or sets a Boolean value that indicates whether Internet Protocol version 6 is enabled on the local computer.</summary>
		/// <returns>
		///   <see langword="true" /> if IPv6 is enabled; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("enabled", DefaultValue = "False")]
		public bool Enabled
		{
			get
			{
				return (bool)base[Ipv6Element.enabledProp];
			}
			set
			{
				base[Ipv6Element.enabledProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return Ipv6Element.properties;
			}
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty enabledProp = new ConfigurationProperty("enabled", typeof(bool), false);
	}
}
