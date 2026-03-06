using System;
using System.Configuration;

namespace System.Net.Configuration
{
	/// <summary>Represents the configuration section for connection management. This class cannot be inherited.</summary>
	public sealed class ConnectionManagementSection : ConfigurationSection
	{
		static ConnectionManagementSection()
		{
			ConnectionManagementSection.properties.Add(ConnectionManagementSection.connectionManagementProp);
		}

		/// <summary>Gets the collection of connection management objects in the section.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.ConnectionManagementElementCollection" /> that contains the connection management information for the local computer.</returns>
		[ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public ConnectionManagementElementCollection ConnectionManagement
		{
			get
			{
				return (ConnectionManagementElementCollection)base[ConnectionManagementSection.connectionManagementProp];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ConnectionManagementSection.properties;
			}
		}

		private static ConfigurationProperty connectionManagementProp = new ConfigurationProperty("ConnectionManagement", typeof(ConnectionManagementElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
