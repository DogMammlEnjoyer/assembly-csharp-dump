using System;
using System.Configuration;

namespace System.Net.Configuration
{
	/// <summary>Identifies the configuration settings for Web proxy server. This class cannot be inherited.</summary>
	public sealed class ProxyElement : ConfigurationElement
	{
		static ProxyElement()
		{
			ProxyElement.properties = new ConfigurationPropertyCollection();
			ProxyElement.properties.Add(ProxyElement.autoDetectProp);
			ProxyElement.properties.Add(ProxyElement.bypassOnLocalProp);
			ProxyElement.properties.Add(ProxyElement.proxyAddressProp);
			ProxyElement.properties.Add(ProxyElement.scriptLocationProp);
			ProxyElement.properties.Add(ProxyElement.useSystemDefaultProp);
		}

		/// <summary>Gets or sets an <see cref="T:System.Net.Configuration.ProxyElement.AutoDetectValues" /> value that controls whether the Web proxy is automatically detected.</summary>
		/// <returns>
		///   <see cref="F:System.Net.Configuration.ProxyElement.AutoDetectValues.True" /> if the <see cref="T:System.Net.WebProxy" /> is automatically detected; <see cref="F:System.Net.Configuration.ProxyElement.AutoDetectValues.False" /> if the <see cref="T:System.Net.WebProxy" /> is not automatically detected; or <see cref="F:System.Net.Configuration.ProxyElement.AutoDetectValues.Unspecified" />.</returns>
		[ConfigurationProperty("autoDetect", DefaultValue = "Unspecified")]
		public ProxyElement.AutoDetectValues AutoDetect
		{
			get
			{
				return (ProxyElement.AutoDetectValues)base[ProxyElement.autoDetectProp];
			}
			set
			{
				base[ProxyElement.autoDetectProp] = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether local resources are retrieved by using a Web proxy server.</summary>
		/// <returns>A value that indicates whether local resources are retrieved by using a Web proxy server.</returns>
		[ConfigurationProperty("bypassonlocal", DefaultValue = "Unspecified")]
		public ProxyElement.BypassOnLocalValues BypassOnLocal
		{
			get
			{
				return (ProxyElement.BypassOnLocalValues)base[ProxyElement.bypassOnLocalProp];
			}
			set
			{
				base[ProxyElement.bypassOnLocalProp] = value;
			}
		}

		/// <summary>Gets or sets the URI that identifies the Web proxy server to use.</summary>
		/// <returns>The URI that identifies the Web proxy server to use.</returns>
		[ConfigurationProperty("proxyaddress")]
		public Uri ProxyAddress
		{
			get
			{
				return (Uri)base[ProxyElement.proxyAddressProp];
			}
			set
			{
				base[ProxyElement.proxyAddressProp] = value;
			}
		}

		/// <summary>Gets or sets an <see cref="T:System.Uri" /> value that specifies the location of the automatic proxy detection script.</summary>
		/// <returns>A <see cref="T:System.Uri" /> specifying the location of the automatic proxy detection script.</returns>
		[ConfigurationProperty("scriptLocation")]
		public Uri ScriptLocation
		{
			get
			{
				return (Uri)base[ProxyElement.scriptLocationProp];
			}
			set
			{
				base[ProxyElement.scriptLocationProp] = value;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that controls whether the Internet Explorer Web proxy settings are used.</summary>
		/// <returns>
		///   <see langword="true" /> if the Internet Explorer LAN settings are used to detect and configure the default <see cref="T:System.Net.WebProxy" /> used for requests; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("usesystemdefault", DefaultValue = "Unspecified")]
		public ProxyElement.UseSystemDefaultValues UseSystemDefault
		{
			get
			{
				return (ProxyElement.UseSystemDefaultValues)base[ProxyElement.useSystemDefaultProp];
			}
			set
			{
				base[ProxyElement.useSystemDefaultProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return ProxyElement.properties;
			}
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty autoDetectProp = new ConfigurationProperty("autoDetect", typeof(ProxyElement.AutoDetectValues), ProxyElement.AutoDetectValues.Unspecified);

		private static ConfigurationProperty bypassOnLocalProp = new ConfigurationProperty("bypassonlocal", typeof(ProxyElement.BypassOnLocalValues), ProxyElement.BypassOnLocalValues.Unspecified);

		private static ConfigurationProperty proxyAddressProp = new ConfigurationProperty("proxyaddress", typeof(Uri), null);

		private static ConfigurationProperty scriptLocationProp = new ConfigurationProperty("scriptLocation", typeof(Uri), null);

		private static ConfigurationProperty useSystemDefaultProp = new ConfigurationProperty("usesystemdefault", typeof(ProxyElement.UseSystemDefaultValues), ProxyElement.UseSystemDefaultValues.Unspecified);

		/// <summary>Specifies whether the proxy is bypassed for local resources.</summary>
		public enum BypassOnLocalValues
		{
			/// <summary>Unspecified.</summary>
			Unspecified = -1,
			/// <summary>Access local resources directly.</summary>
			True = 1,
			/// <summary>All requests for local resources should go through the proxy</summary>
			False = 0
		}

		/// <summary>Specifies whether to use the local system proxy settings to determine whether the proxy is bypassed for local resources.</summary>
		public enum UseSystemDefaultValues
		{
			/// <summary>The system default proxy setting is unspecified.</summary>
			Unspecified = -1,
			/// <summary>Use system default proxy setting values.</summary>
			True = 1,
			/// <summary>Do not use system default proxy setting values</summary>
			False = 0
		}

		/// <summary>Specifies whether the proxy is automatically detected.</summary>
		public enum AutoDetectValues
		{
			/// <summary>Unspecified.</summary>
			Unspecified = -1,
			/// <summary>The proxy is automatically detected.</summary>
			True = 1,
			/// <summary>The proxy is not automatically detected.</summary>
			False = 0
		}
	}
}
