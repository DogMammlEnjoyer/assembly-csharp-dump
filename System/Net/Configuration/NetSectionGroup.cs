using System;
using System.Configuration;

namespace System.Net.Configuration
{
	/// <summary>Gets the section group information for the networking namespaces. This class cannot be inherited.</summary>
	public sealed class NetSectionGroup : ConfigurationSectionGroup
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.NetSectionGroup" /> class.</summary>
		[MonoTODO]
		public NetSectionGroup()
		{
		}

		/// <summary>Gets the configuration section containing the authentication modules registered for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.AuthenticationModulesSection" /> object.</returns>
		[ConfigurationProperty("authenticationModules")]
		public AuthenticationModulesSection AuthenticationModules
		{
			get
			{
				return (AuthenticationModulesSection)base.Sections["authenticationModules"];
			}
		}

		/// <summary>Gets the configuration section containing the connection management settings for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.ConnectionManagementSection" /> object.</returns>
		[ConfigurationProperty("connectionManagement")]
		public ConnectionManagementSection ConnectionManagement
		{
			get
			{
				return (ConnectionManagementSection)base.Sections["connectionManagement"];
			}
		}

		/// <summary>Gets the configuration section containing the default Web proxy server settings for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.DefaultProxySection" /> object.</returns>
		[ConfigurationProperty("defaultProxy")]
		public DefaultProxySection DefaultProxy
		{
			get
			{
				return (DefaultProxySection)base.Sections["defaultProxy"];
			}
		}

		/// <summary>Gets the configuration section containing the SMTP client email settings for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.MailSettingsSectionGroup" /> object.</returns>
		public MailSettingsSectionGroup MailSettings
		{
			get
			{
				return (MailSettingsSectionGroup)base.SectionGroups["mailSettings"];
			}
		}

		/// <summary>Gets the configuration section containing the cache configuration settings for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.RequestCachingSection" /> object.</returns>
		[ConfigurationProperty("requestCaching")]
		public RequestCachingSection RequestCaching
		{
			get
			{
				return (RequestCachingSection)base.Sections["requestCaching"];
			}
		}

		/// <summary>Gets the configuration section containing the network settings for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.SettingsSection" /> object.</returns>
		[ConfigurationProperty("settings")]
		public SettingsSection Settings
		{
			get
			{
				return (SettingsSection)base.Sections["settings"];
			}
		}

		/// <summary>Gets the configuration section containing the modules registered for use with the <see cref="T:System.Net.WebRequest" /> class.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.WebRequestModulesSection" /> object.</returns>
		[ConfigurationProperty("webRequestModules")]
		public WebRequestModulesSection WebRequestModules
		{
			get
			{
				return (WebRequestModulesSection)base.Sections["webRequestModules"];
			}
		}

		/// <summary>Gets the <see langword="System.Net" /> configuration section group from the specified configuration file.</summary>
		/// <param name="config">A <see cref="T:System.Configuration.Configuration" /> that represents a configuration file.</param>
		/// <returns>A <see cref="T:System.Net.Configuration.NetSectionGroup" /> that represents the <see langword="System.Net" /> settings in <paramref name="config" />.</returns>
		[MonoTODO]
		public static NetSectionGroup GetSectionGroup(Configuration config)
		{
			throw new NotImplementedException();
		}
	}
}
