using System;

namespace System.Configuration
{
	/// <summary>A configuration element that contains a <see cref="T:System.String" /> name and <see cref="T:System.String" /> value. This class cannot be inherited.</summary>
	public sealed class NameValueConfigurationElement : ConfigurationElement
	{
		static NameValueConfigurationElement()
		{
			NameValueConfigurationElement._properties.Add(NameValueConfigurationElement._propName);
			NameValueConfigurationElement._properties.Add(NameValueConfigurationElement._propValue);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.NameValueConfigurationElement" /> class based on supplied parameters.</summary>
		/// <param name="name">The name of the <see cref="T:System.Configuration.NameValueConfigurationElement" /> object.</param>
		/// <param name="value">The value of the <see cref="T:System.Configuration.NameValueConfigurationElement" /> object.</param>
		public NameValueConfigurationElement(string name, string value)
		{
			base[NameValueConfigurationElement._propName] = name;
			base[NameValueConfigurationElement._propValue] = value;
		}

		/// <summary>Gets the name of the <see cref="T:System.Configuration.NameValueConfigurationElement" /> object.</summary>
		/// <returns>The name of the <see cref="T:System.Configuration.NameValueConfigurationElement" /> object.</returns>
		[ConfigurationProperty("name", DefaultValue = "", Options = ConfigurationPropertyOptions.IsKey)]
		public string Name
		{
			get
			{
				return (string)base[NameValueConfigurationElement._propName];
			}
		}

		/// <summary>Gets or sets the value of the <see cref="T:System.Configuration.NameValueConfigurationElement" /> object.</summary>
		/// <returns>The value of the <see cref="T:System.Configuration.NameValueConfigurationElement" /> object.</returns>
		[ConfigurationProperty("value", DefaultValue = "", Options = ConfigurationPropertyOptions.None)]
		public string Value
		{
			get
			{
				return (string)base[NameValueConfigurationElement._propValue];
			}
			set
			{
				base[NameValueConfigurationElement._propValue] = value;
			}
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return NameValueConfigurationElement._properties;
			}
		}

		private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

		private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), "", ConfigurationPropertyOptions.IsKey);

		private static readonly ConfigurationProperty _propValue = new ConfigurationProperty("value", typeof(string), "");
	}
}
