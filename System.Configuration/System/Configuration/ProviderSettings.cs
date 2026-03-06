using System;
using System.Collections.Specialized;

namespace System.Configuration
{
	/// <summary>Represents the configuration elements associated with a provider.</summary>
	public sealed class ProviderSettings : ConfigurationElement
	{
		static ProviderSettings()
		{
			ProviderSettings.properties.Add(ProviderSettings.nameProp);
			ProviderSettings.properties.Add(ProviderSettings.typeProp);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ProviderSettings" /> class.</summary>
		public ProviderSettings()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ProviderSettings" /> class.</summary>
		/// <param name="name">The name of the provider to configure settings for.</param>
		/// <param name="type">The type of the provider to configure settings for.</param>
		public ProviderSettings(string name, string type)
		{
			this.Name = name;
			this.Type = type;
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			if (this.parameters == null)
			{
				this.parameters = new ConfigNameValueCollection();
			}
			this.parameters[name] = value;
			this.parameters.ResetModified();
			return true;
		}

		protected internal override bool IsModified()
		{
			return (this.parameters != null && this.parameters.IsModified) || base.IsModified();
		}

		protected internal override void Reset(ConfigurationElement parentElement)
		{
			base.Reset(parentElement);
			ProviderSettings providerSettings = parentElement as ProviderSettings;
			if (providerSettings != null && providerSettings.parameters != null)
			{
				this.parameters = new ConfigNameValueCollection(providerSettings.parameters);
				return;
			}
			this.parameters = null;
		}

		[MonoTODO]
		protected internal override void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
		{
			base.Unmerge(sourceElement, parentElement, saveMode);
		}

		/// <summary>Gets or sets the name of the provider configured by this class.</summary>
		/// <returns>The name of the provider.</returns>
		[ConfigurationProperty("name", Options = (ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey))]
		public string Name
		{
			get
			{
				return (string)base[ProviderSettings.nameProp];
			}
			set
			{
				base[ProviderSettings.nameProp] = value;
			}
		}

		/// <summary>Gets or sets the type of the provider configured by this class.</summary>
		/// <returns>The fully qualified namespace and class name for the type of provider configured by this <see cref="T:System.Configuration.ProviderSettings" /> instance.</returns>
		[ConfigurationProperty("type", Options = ConfigurationPropertyOptions.IsRequired)]
		public string Type
		{
			get
			{
				return (string)base[ProviderSettings.typeProp];
			}
			set
			{
				base[ProviderSettings.typeProp] = value;
			}
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ProviderSettings.properties;
			}
		}

		/// <summary>Gets a collection of user-defined parameters for the provider.</summary>
		/// <returns>A <see cref="T:System.Collections.Specialized.NameValueCollection" /> of parameters for the provider.</returns>
		public NameValueCollection Parameters
		{
			get
			{
				if (this.parameters == null)
				{
					this.parameters = new ConfigNameValueCollection();
				}
				return this.parameters;
			}
		}

		private ConfigNameValueCollection parameters;

		private static ConfigurationProperty nameProp = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);

		private static ConfigurationProperty typeProp = new ConfigurationProperty("type", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
