using System;
using System.ComponentModel;

namespace System.Configuration
{
	/// <summary>Represents a single, named connection string in the connection strings configuration file section.</summary>
	public sealed class ConnectionStringSettings : ConfigurationElement
	{
		static ConnectionStringSettings()
		{
			ConnectionStringSettings._propConnectionString = new ConfigurationProperty("connectionString", typeof(string), "", ConfigurationPropertyOptions.IsRequired);
			ConnectionStringSettings._properties.Add(ConnectionStringSettings._propName);
			ConnectionStringSettings._properties.Add(ConnectionStringSettings._propProviderName);
			ConnectionStringSettings._properties.Add(ConnectionStringSettings._propConnectionString);
		}

		/// <summary>Initializes a new instance of a <see cref="T:System.Configuration.ConnectionStringSettings" /> class.</summary>
		public ConnectionStringSettings()
		{
		}

		/// <summary>Initializes a new instance of a <see cref="T:System.Configuration.ConnectionStringSettings" /> class.</summary>
		/// <param name="name">The name of the connection string.</param>
		/// <param name="connectionString">The connection string.</param>
		public ConnectionStringSettings(string name, string connectionString) : this(name, connectionString, "")
		{
		}

		/// <summary>Initializes a new instance of a <see cref="T:System.Configuration.ConnectionStringSettings" /> object.</summary>
		/// <param name="name">The name of the connection string.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <param name="providerName">The name of the provider to use with the connection string.</param>
		public ConnectionStringSettings(string name, string connectionString, string providerName)
		{
			this.Name = name;
			this.ConnectionString = connectionString;
			this.ProviderName = providerName;
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ConnectionStringSettings._properties;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Configuration.ConnectionStringSettings" /> name.</summary>
		/// <returns>The string value assigned to the <see cref="P:System.Configuration.ConnectionStringSettings.Name" /> property.</returns>
		[ConfigurationProperty("name", DefaultValue = "", Options = (ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey))]
		public string Name
		{
			get
			{
				return (string)base[ConnectionStringSettings._propName];
			}
			set
			{
				base[ConnectionStringSettings._propName] = value;
			}
		}

		/// <summary>Gets or sets the provider name property.</summary>
		/// <returns>The <see cref="P:System.Configuration.ConnectionStringSettings.ProviderName" /> property.</returns>
		[ConfigurationProperty("providerName", DefaultValue = "System.Data.SqlClient")]
		public string ProviderName
		{
			get
			{
				return (string)base[ConnectionStringSettings._propProviderName];
			}
			set
			{
				base[ConnectionStringSettings._propProviderName] = value;
			}
		}

		/// <summary>Gets or sets the connection string.</summary>
		/// <returns>The string value assigned to the <see cref="P:System.Configuration.ConnectionStringSettings.ConnectionString" /> property.</returns>
		[ConfigurationProperty("connectionString", DefaultValue = "", Options = ConfigurationPropertyOptions.IsRequired)]
		public string ConnectionString
		{
			get
			{
				return (string)base[ConnectionStringSettings._propConnectionString];
			}
			set
			{
				base[ConnectionStringSettings._propConnectionString] = value;
			}
		}

		/// <summary>Returns a string representation of the object.</summary>
		/// <returns>A string representation of the object.</returns>
		public override string ToString()
		{
			return this.ConnectionString;
		}

		private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

		private static readonly ConfigurationProperty _propConnectionString;

		private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), null, TypeDescriptor.GetConverter(typeof(string)), null, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);

		private static readonly ConfigurationProperty _propProviderName = new ConfigurationProperty("providerName", typeof(string), "", ConfigurationPropertyOptions.None);
	}
}
