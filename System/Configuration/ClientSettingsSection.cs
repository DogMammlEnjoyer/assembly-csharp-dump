using System;

namespace System.Configuration
{
	/// <summary>Represents a group of user-scoped application settings in a configuration file.</summary>
	public sealed class ClientSettingsSection : ConfigurationSection
	{
		static ClientSettingsSection()
		{
			ClientSettingsSection.properties = new ConfigurationPropertyCollection();
			ClientSettingsSection.properties.Add(ClientSettingsSection.settings_prop);
		}

		/// <summary>Gets the collection of client settings for the section.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingElementCollection" /> containing all the client settings found in the current configuration section.</returns>
		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public SettingElementCollection Settings
		{
			get
			{
				return (SettingElementCollection)base[ClientSettingsSection.settings_prop];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ClientSettingsSection.properties;
			}
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty settings_prop = new ConfigurationProperty("", typeof(SettingElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
	}
}
