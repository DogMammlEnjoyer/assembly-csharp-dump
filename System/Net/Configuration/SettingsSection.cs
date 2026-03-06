using System;
using System.Configuration;
using Unity;

namespace System.Net.Configuration
{
	/// <summary>Represents the configuration section for sockets, IPv6, response headers, and service points. This class cannot be inherited.</summary>
	public sealed class SettingsSection : ConfigurationSection
	{
		static SettingsSection()
		{
			SettingsSection.webProxyScriptProp = new ConfigurationProperty("webProxyScript", typeof(WebProxyScriptElement));
			SettingsSection.properties = new ConfigurationPropertyCollection();
			SettingsSection.properties.Add(SettingsSection.httpWebRequestProp);
			SettingsSection.properties.Add(SettingsSection.ipv6Prop);
			SettingsSection.properties.Add(SettingsSection.performanceCountersProp);
			SettingsSection.properties.Add(SettingsSection.servicePointManagerProp);
			SettingsSection.properties.Add(SettingsSection.socketProp);
			SettingsSection.properties.Add(SettingsSection.webProxyScriptProp);
		}

		/// <summary>Gets the configuration element that controls the settings used by an <see cref="T:System.Net.HttpWebRequest" /> object.</summary>
		/// <returns>The configuration element that controls the maximum response header length and other settings used by an <see cref="T:System.Net.HttpWebRequest" /> object.</returns>
		[ConfigurationProperty("httpWebRequest")]
		public HttpWebRequestElement HttpWebRequest
		{
			get
			{
				return (HttpWebRequestElement)base[SettingsSection.httpWebRequestProp];
			}
		}

		/// <summary>Gets the configuration element that enables Internet Protocol version 6 (IPv6).</summary>
		/// <returns>The configuration element that controls the setting used by IPv6.</returns>
		[ConfigurationProperty("ipv6")]
		public Ipv6Element Ipv6
		{
			get
			{
				return (Ipv6Element)base[SettingsSection.ipv6Prop];
			}
		}

		/// <summary>Gets the configuration element that controls whether network performance counters are enabled.</summary>
		/// <returns>The configuration element that controls usage of network performance counters.</returns>
		[ConfigurationProperty("performanceCounters")]
		public PerformanceCountersElement PerformanceCounters
		{
			get
			{
				return (PerformanceCountersElement)base[SettingsSection.performanceCountersProp];
			}
		}

		/// <summary>Gets the configuration element that controls settings for connections to remote host computers.</summary>
		/// <returns>The configuration element that controls settings for connections to remote host computers.</returns>
		[ConfigurationProperty("servicePointManager")]
		public ServicePointManagerElement ServicePointManager
		{
			get
			{
				return (ServicePointManagerElement)base[SettingsSection.servicePointManagerProp];
			}
		}

		/// <summary>Gets the configuration element that controls settings for sockets.</summary>
		/// <returns>The configuration element that controls settings for sockets.</returns>
		[ConfigurationProperty("socket")]
		public SocketElement Socket
		{
			get
			{
				return (SocketElement)base[SettingsSection.socketProp];
			}
		}

		/// <summary>Gets the configuration element that controls the execution timeout and download timeout of Web proxy scripts.</summary>
		/// <returns>The configuration element that controls settings for the execution timeout and download timeout used by the Web proxy scripts.</returns>
		[ConfigurationProperty("webProxyScript")]
		public WebProxyScriptElement WebProxyScript
		{
			get
			{
				return (WebProxyScriptElement)base[SettingsSection.webProxyScriptProp];
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return SettingsSection.properties;
			}
		}

		/// <summary>Gets the configuration element that controls the settings used by an <see cref="T:System.Net.HttpListener" /> object.</summary>
		/// <returns>An <see cref="T:System.Net.Configuration.HttpListenerElement" /> object.  
		///  The configuration element that controls the settings used by an <see cref="T:System.Net.HttpListener" /> object.</returns>
		public HttpListenerElement HttpListener
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the configuration element that controls the settings used by an <see cref="T:System.Net.WebUtility" /> object.</summary>
		/// <returns>The configuration element that controls the settings used by a <see cref="T:System.Net.WebUtility" /> object.</returns>
		public WebUtilityElement WebUtility
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		/// <summary>Gets the configuration element that controls the number of handles for default network credentials.</summary>
		/// <returns>The configuration element that controls the number of handles for default network credentials.</returns>
		public WindowsAuthenticationElement WindowsAuthentication
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		private static ConfigurationPropertyCollection properties;

		private static ConfigurationProperty httpWebRequestProp = new ConfigurationProperty("httpWebRequest", typeof(HttpWebRequestElement));

		private static ConfigurationProperty ipv6Prop = new ConfigurationProperty("ipv6", typeof(Ipv6Element));

		private static ConfigurationProperty performanceCountersProp = new ConfigurationProperty("performanceCounters", typeof(PerformanceCountersElement));

		private static ConfigurationProperty servicePointManagerProp = new ConfigurationProperty("servicePointManager", typeof(ServicePointManagerElement));

		private static ConfigurationProperty webProxyScriptProp;

		private static ConfigurationProperty socketProp = new ConfigurationProperty("socket", typeof(SocketElement));
	}
}
