using System;

namespace System.Configuration
{
	/// <summary>Provides the configuration setting for International Domain Name (IDN) processing in the <see cref="T:System.Uri" /> class.</summary>
	public sealed class IdnElement : ConfigurationElement
	{
		static IdnElement()
		{
			IdnElement.properties = new ConfigurationPropertyCollection();
			IdnElement.properties.Add(IdnElement.enabled_prop);
		}

		/// <summary>Gets or sets the value of the <see cref="T:System.Configuration.IdnElement" /> configuration setting.</summary>
		/// <returns>A <see cref="T:System.UriIdnScope" /> that contains the current configuration setting for IDN processing.</returns>
		[ConfigurationProperty("enabled", DefaultValue = UriIdnScope.None, Options = (ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey))]
		public UriIdnScope Enabled
		{
			get
			{
				return (UriIdnScope)base[IdnElement.enabled_prop];
			}
			set
			{
				base[IdnElement.enabled_prop] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return IdnElement.properties;
			}
		}

		public override bool Equals(object o)
		{
			IdnElement idnElement = o as IdnElement;
			return idnElement != null && idnElement.Enabled == this.Enabled;
		}

		public override int GetHashCode()
		{
			return (int)(this.Enabled ^ (UriIdnScope)127);
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty enabled_prop = new ConfigurationProperty("enabled", typeof(UriIdnScope), UriIdnScope.None, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);

		internal const UriIdnScope EnabledDefaultValue = UriIdnScope.None;
	}
}
