using System;
using System.Configuration;

namespace System.Net.Configuration
{
	/// <summary>Represents the configuration section for Web proxy server usage. This class cannot be inherited.</summary>
	public sealed class DefaultProxySection : ConfigurationSection
	{
		static DefaultProxySection()
		{
			DefaultProxySection.properties = new ConfigurationPropertyCollection();
			DefaultProxySection.properties.Add(DefaultProxySection.bypassListProp);
			DefaultProxySection.properties.Add(DefaultProxySection.enabledProp);
			DefaultProxySection.properties.Add(DefaultProxySection.moduleProp);
			DefaultProxySection.properties.Add(DefaultProxySection.proxyProp);
			DefaultProxySection.properties.Add(DefaultProxySection.useDefaultCredentialsProp);
		}

		/// <summary>Gets the collection of resources that are not obtained using the Web proxy server.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.BypassElementCollection" /> that contains the addresses of resources that bypass the Web proxy server.</returns>
		[ConfigurationProperty("bypasslist")]
		public BypassElementCollection BypassList
		{
			get
			{
				return (BypassElementCollection)base[DefaultProxySection.bypassListProp];
			}
		}

		/// <summary>Gets or sets whether a Web proxy is used.</summary>
		/// <returns>
		///   <see langword="true" /> if a Web proxy will be used; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("enabled", DefaultValue = "True")]
		public bool Enabled
		{
			get
			{
				return (bool)base[DefaultProxySection.enabledProp];
			}
			set
			{
				base[DefaultProxySection.enabledProp] = value;
			}
		}

		/// <summary>Gets the type information for a custom Web proxy implementation.</summary>
		/// <returns>The type information for a custom Web proxy implementation.</returns>
		[ConfigurationProperty("module")]
		public ModuleElement Module
		{
			get
			{
				return (ModuleElement)base[DefaultProxySection.moduleProp];
			}
		}

		/// <summary>Gets the URI that identifies the Web proxy server to use.</summary>
		/// <returns>The URI that identifies the Web proxy server.</returns>
		[ConfigurationProperty("proxy")]
		public ProxyElement Proxy
		{
			get
			{
				return (ProxyElement)base[DefaultProxySection.proxyProp];
			}
		}

		/// <summary>Gets or sets whether default credentials are to be used to access a Web proxy server.</summary>
		/// <returns>
		///   <see langword="true" /> if default credentials are to be used; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("useDefaultCredentials", DefaultValue = "False")]
		public bool UseDefaultCredentials
		{
			get
			{
				return (bool)base[DefaultProxySection.useDefaultCredentialsProp];
			}
			set
			{
				base[DefaultProxySection.useDefaultCredentialsProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return DefaultProxySection.properties;
			}
		}

		[MonoTODO]
		protected override void PostDeserialize()
		{
		}

		[MonoTODO]
		protected override void Reset(ConfigurationElement parentElement)
		{
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty bypassListProp = new ConfigurationProperty("bypasslist", typeof(BypassElementCollection), null);

		private static ConfigurationProperty enabledProp = new ConfigurationProperty("enabled", typeof(bool), true);

		private static ConfigurationProperty moduleProp = new ConfigurationProperty("module", typeof(ModuleElement), null);

		private static ConfigurationProperty proxyProp = new ConfigurationProperty("proxy", typeof(ProxyElement), null);

		private static ConfigurationProperty useDefaultCredentialsProp = new ConfigurationProperty("useDefaultCredentials", typeof(bool), false);
	}
}
