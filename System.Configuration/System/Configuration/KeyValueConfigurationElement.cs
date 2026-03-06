using System;

namespace System.Configuration
{
	/// <summary>Represents a configuration element that contains a key/value pair.</summary>
	public class KeyValueConfigurationElement : ConfigurationElement
	{
		static KeyValueConfigurationElement()
		{
			KeyValueConfigurationElement.properties.Add(KeyValueConfigurationElement.keyProp);
			KeyValueConfigurationElement.properties.Add(KeyValueConfigurationElement.valueProp);
		}

		internal KeyValueConfigurationElement()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.KeyValueConfigurationElement" /> class based on the supplied parameters.</summary>
		/// <param name="key">The key of the <see cref="T:System.Configuration.KeyValueConfigurationElement" />.</param>
		/// <param name="value">The value of the <see cref="T:System.Configuration.KeyValueConfigurationElement" />.</param>
		public KeyValueConfigurationElement(string key, string value)
		{
			base[KeyValueConfigurationElement.keyProp] = key;
			base[KeyValueConfigurationElement.valueProp] = value;
		}

		/// <summary>Gets the key of the <see cref="T:System.Configuration.KeyValueConfigurationElement" /> object.</summary>
		/// <returns>The key of the <see cref="T:System.Configuration.KeyValueConfigurationElement" />.</returns>
		[ConfigurationProperty("key", DefaultValue = "", Options = ConfigurationPropertyOptions.IsKey)]
		public string Key
		{
			get
			{
				return (string)base[KeyValueConfigurationElement.keyProp];
			}
		}

		/// <summary>Gets or sets the value of the <see cref="T:System.Configuration.KeyValueConfigurationElement" /> object.</summary>
		/// <returns>The value of the <see cref="T:System.Configuration.KeyValueConfigurationElement" />.</returns>
		[ConfigurationProperty("value", DefaultValue = "")]
		public string Value
		{
			get
			{
				return (string)base[KeyValueConfigurationElement.valueProp];
			}
			set
			{
				base[KeyValueConfigurationElement.valueProp] = value;
			}
		}

		/// <summary>Sets the <see cref="T:System.Configuration.KeyValueConfigurationElement" /> object to its initial state.</summary>
		[MonoTODO]
		protected internal override void Init()
		{
		}

		/// <summary>Gets the collection of properties.</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationPropertyCollection" /> of properties for the element.</returns>
		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return KeyValueConfigurationElement.properties;
			}
		}

		private static ConfigurationProperty keyProp = new ConfigurationProperty("key", typeof(string), "", ConfigurationPropertyOptions.IsKey);

		private static ConfigurationProperty valueProp = new ConfigurationProperty("value", typeof(string), "");

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
