using System;

namespace System.Configuration
{
	/// <summary>Provides programmatic access to the connection strings configuration-file section.</summary>
	public sealed class ConnectionStringsSection : ConfigurationSection
	{
		static ConnectionStringsSection()
		{
			ConnectionStringsSection._properties.Add(ConnectionStringsSection._propConnectionStrings);
		}

		/// <summary>Gets a <see cref="T:System.Configuration.ConnectionStringSettingsCollection" /> collection of <see cref="T:System.Configuration.ConnectionStringSettings" /> objects.</summary>
		/// <returns>A <see cref="T:System.Configuration.ConnectionStringSettingsCollection" /> collection of <see cref="T:System.Configuration.ConnectionStringSettings" /> objects.</returns>
		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public ConnectionStringSettingsCollection ConnectionStrings
		{
			get
			{
				return (ConnectionStringSettingsCollection)base[ConnectionStringsSection._propConnectionStrings];
			}
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ConnectionStringsSection._properties;
			}
		}

		protected internal override object GetRuntimeObject()
		{
			return base.GetRuntimeObject();
		}

		private static readonly ConfigurationProperty _propConnectionStrings = new ConfigurationProperty("", typeof(ConnectionStringSettingsCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

		private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
	}
}
