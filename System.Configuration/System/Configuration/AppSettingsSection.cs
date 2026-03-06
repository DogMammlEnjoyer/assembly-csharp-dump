using System;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace System.Configuration
{
	/// <summary>Provides configuration system support for the <see langword="appSettings" /> configuration section. This class cannot be inherited.</summary>
	public sealed class AppSettingsSection : ConfigurationSection
	{
		static AppSettingsSection()
		{
			AppSettingsSection._properties = new ConfigurationPropertyCollection();
			AppSettingsSection._properties.Add(AppSettingsSection._propFile);
			AppSettingsSection._properties.Add(AppSettingsSection._propSettings);
		}

		protected internal override bool IsModified()
		{
			return this.Settings.IsModified();
		}

		[MonoInternalNote("file path?  do we use a System.Configuration api for opening it?  do we keep it open?  do we open it writable?")]
		protected internal override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			base.DeserializeElement(reader, serializeCollectionKey);
			if (this.File != "")
			{
				try
				{
					string text = this.File;
					if (!Path.IsPathRooted(text))
					{
						text = Path.Combine(Path.GetDirectoryName(base.Configuration.FilePath), text);
					}
					FileStream fileStream = System.IO.File.OpenRead(text);
					XmlReader reader2 = new ConfigXmlTextReader(fileStream, text);
					base.DeserializeElement(reader2, serializeCollectionKey);
					fileStream.Close();
				}
				catch
				{
				}
			}
		}

		protected internal override void Reset(ConfigurationElement parentSection)
		{
			AppSettingsSection appSettingsSection = parentSection as AppSettingsSection;
			if (appSettingsSection != null)
			{
				this.Settings.Reset(appSettingsSection.Settings);
			}
		}

		[MonoTODO]
		protected internal override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
		{
			if (this.File == "")
			{
				return base.SerializeSection(parentElement, name, saveMode);
			}
			throw new NotImplementedException();
		}

		/// <summary>Gets or sets a configuration file that provides additional settings or overrides the settings specified in the <see langword="appSettings" /> element.</summary>
		/// <returns>A configuration file that provides additional settings or overrides the settings specified in the <see langword="appSettings" /> element.</returns>
		[ConfigurationProperty("file", DefaultValue = "")]
		public string File
		{
			get
			{
				return (string)base[AppSettingsSection._propFile];
			}
			set
			{
				base[AppSettingsSection._propFile] = value;
			}
		}

		/// <summary>Gets a collection of key/value pairs that contains application settings.</summary>
		/// <returns>A collection of key/value pairs that contains the application settings from the configuration file.</returns>
		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public KeyValueConfigurationCollection Settings
		{
			get
			{
				return (KeyValueConfigurationCollection)base[AppSettingsSection._propSettings];
			}
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return AppSettingsSection._properties;
			}
		}

		protected internal override object GetRuntimeObject()
		{
			KeyValueInternalCollection keyValueInternalCollection = new KeyValueInternalCollection();
			foreach (string key in this.Settings.AllKeys)
			{
				KeyValueConfigurationElement keyValueConfigurationElement = this.Settings[key];
				keyValueInternalCollection.Add(keyValueConfigurationElement.Key, keyValueConfigurationElement.Value);
			}
			if (!ConfigurationManager.ConfigurationSystem.SupportsUserConfig)
			{
				keyValueInternalCollection.SetReadOnly();
			}
			return keyValueInternalCollection;
		}

		private static ConfigurationPropertyCollection _properties;

		private static readonly ConfigurationProperty _propFile = new ConfigurationProperty("file", typeof(string), "", new StringConverter(), null, ConfigurationPropertyOptions.None);

		private static readonly ConfigurationProperty _propSettings = new ConfigurationProperty("", typeof(KeyValueConfigurationCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection);
	}
}
