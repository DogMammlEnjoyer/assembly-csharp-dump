using System;

namespace System.Configuration
{
	/// <summary>Represents a simplified configuration element used for updating elements in the configuration. This class cannot be inherited.</summary>
	public sealed class SettingElement : ConfigurationElement
	{
		static SettingElement()
		{
			SettingElement.properties = new ConfigurationPropertyCollection();
			SettingElement.properties.Add(SettingElement.name_prop);
			SettingElement.properties.Add(SettingElement.serialize_as_prop);
			SettingElement.properties.Add(SettingElement.value_prop);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingElement" /> class.</summary>
		public SettingElement()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingElement" /> class based on supplied parameters.</summary>
		/// <param name="name">The name of the <see cref="T:System.Configuration.SettingElement" /> object.</param>
		/// <param name="serializeAs">A <see cref="T:System.Configuration.SettingsSerializeAs" /> object. This object is an enumeration used as the serialization scheme to store configuration settings.</param>
		public SettingElement(string name, SettingsSerializeAs serializeAs)
		{
			this.Name = name;
			this.SerializeAs = serializeAs;
		}

		/// <summary>Gets or sets the name of the <see cref="T:System.Configuration.SettingElement" /> object.</summary>
		/// <returns>The name of the <see cref="T:System.Configuration.SettingElement" /> object.</returns>
		[ConfigurationProperty("name", DefaultValue = "", Options = (ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey))]
		public string Name
		{
			get
			{
				return (string)base[SettingElement.name_prop];
			}
			set
			{
				base[SettingElement.name_prop] = value;
			}
		}

		/// <summary>Gets or sets the value of a <see cref="T:System.Configuration.SettingElement" /> object by using a <see cref="T:System.Configuration.SettingValueElement" /> object.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingValueElement" /> object containing the value of the <see cref="T:System.Configuration.SettingElement" /> object.</returns>
		[ConfigurationProperty("value", DefaultValue = null, Options = ConfigurationPropertyOptions.IsRequired)]
		public SettingValueElement Value
		{
			get
			{
				return (SettingValueElement)base[SettingElement.value_prop];
			}
			set
			{
				base[SettingElement.value_prop] = value;
			}
		}

		/// <summary>Gets or sets the serialization mechanism used to persist the values of the <see cref="T:System.Configuration.SettingElement" /> object.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsSerializeAs" /> object.</returns>
		[ConfigurationProperty("serializeAs", DefaultValue = SettingsSerializeAs.String, Options = ConfigurationPropertyOptions.IsRequired)]
		public SettingsSerializeAs SerializeAs
		{
			get
			{
				if (base[SettingElement.serialize_as_prop] == null)
				{
					return SettingsSerializeAs.String;
				}
				return (SettingsSerializeAs)base[SettingElement.serialize_as_prop];
			}
			set
			{
				base[SettingElement.serialize_as_prop] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return SettingElement.properties;
			}
		}

		/// <summary>Compares the current <see cref="T:System.Configuration.SettingElement" /> instance to the specified object.</summary>
		/// <param name="settings">The object to compare with.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.SettingElement" /> instance is equal to the specified object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object settings)
		{
			SettingElement settingElement = settings as SettingElement;
			return settingElement != null && (settingElement.SerializeAs == this.SerializeAs && settingElement.Value == this.Value) && settingElement.Name == this.Name;
		}

		/// <summary>Gets a unique value representing the <see cref="T:System.Configuration.SettingElement" /> current instance.</summary>
		/// <returns>A unique value representing the <see cref="T:System.Configuration.SettingElement" /> current instance.</returns>
		public override int GetHashCode()
		{
			int num = (int)(this.SerializeAs ^ (SettingsSerializeAs)127);
			if (this.Name != null)
			{
				num += (this.Name.GetHashCode() ^ 127);
			}
			if (this.Value != null)
			{
				num += this.Value.GetHashCode();
			}
			return num;
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty name_prop = new ConfigurationProperty("name", typeof(string), string.Empty, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);

		private static ConfigurationProperty serialize_as_prop = new ConfigurationProperty("serializeAs", typeof(SettingsSerializeAs), null, ConfigurationPropertyOptions.IsRequired);

		private static ConfigurationProperty value_prop = new ConfigurationProperty("value", typeof(SettingValueElement), null, ConfigurationPropertyOptions.IsRequired);
	}
}
