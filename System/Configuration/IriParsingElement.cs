using System;

namespace System.Configuration
{
	/// <summary>Provides the configuration setting for International Resource Identifier (IRI) processing in the <see cref="T:System.Uri" /> class.</summary>
	public sealed class IriParsingElement : ConfigurationElement
	{
		static IriParsingElement()
		{
			IriParsingElement.properties = new ConfigurationPropertyCollection();
			IriParsingElement.properties.Add(IriParsingElement.enabled_prop);
		}

		/// <summary>Gets or sets the value of the <see cref="T:System.Configuration.IriParsingElement" /> configuration setting.</summary>
		/// <returns>A Boolean that indicates if International Resource Identifier (IRI) processing is enabled.</returns>
		[ConfigurationProperty("enabled", DefaultValue = false, Options = (ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey))]
		public bool Enabled
		{
			get
			{
				return (bool)base[IriParsingElement.enabled_prop];
			}
			set
			{
				base[IriParsingElement.enabled_prop] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return IriParsingElement.properties;
			}
		}

		public override bool Equals(object o)
		{
			IriParsingElement iriParsingElement = o as IriParsingElement;
			return iriParsingElement != null && iriParsingElement.Enabled == this.Enabled;
		}

		public override int GetHashCode()
		{
			return Convert.ToInt32(this.Enabled) ^ 127;
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty enabled_prop = new ConfigurationProperty("enabled", typeof(bool), false, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);
	}
}
