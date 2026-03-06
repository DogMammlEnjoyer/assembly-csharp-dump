using System;

namespace System.Configuration
{
	/// <summary>Represents a group of configuration elements that configure the providers for the <see langword="&lt;configProtectedData&gt;" /> configuration section.</summary>
	public class ProtectedProviderSettings : ConfigurationElement
	{
		static ProtectedProviderSettings()
		{
			ProtectedProviderSettings.properties.Add(ProtectedProviderSettings.providersProp);
		}

		/// <summary>Gets a <see cref="T:System.Configuration.ConfigurationPropertyCollection" /> collection that represents the properties of the providers for the protected configuration data.</summary>
		/// <returns>A <see cref="T:System.Configuration.ConfigurationPropertyCollection" /> that represents the properties of the providers for the protected configuration data.</returns>
		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ProtectedProviderSettings.properties;
			}
		}

		/// <summary>Gets a collection of <see cref="T:System.Configuration.ProviderSettings" /> objects that represent the properties of the providers for the protected configuration data.</summary>
		/// <returns>A collection of <see cref="T:System.Configuration.ProviderSettings" /> objects that represent the properties of the providers for the protected configuration data.</returns>
		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public ProviderSettingsCollection Providers
		{
			get
			{
				return (ProviderSettingsCollection)base[ProtectedProviderSettings.providersProp];
			}
		}

		private static ConfigurationProperty providersProp = new ConfigurationProperty("", typeof(ProviderSettingsCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
