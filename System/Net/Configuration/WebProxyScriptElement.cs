using System;
using System.Configuration;
using Unity;

namespace System.Net.Configuration
{
	/// <summary>Represents information used to configure Web proxy scripts. This class cannot be inherited.</summary>
	public sealed class WebProxyScriptElement : ConfigurationElement
	{
		static WebProxyScriptElement()
		{
			WebProxyScriptElement.properties.Add(WebProxyScriptElement.downloadTimeoutProp);
		}

		protected override void PostDeserialize()
		{
		}

		/// <summary>Gets or sets the Web proxy script download timeout using the format hours:minutes:seconds.</summary>
		/// <returns>A <see cref="T:System.TimeSpan" /> object that contains the timeout value. The default download timeout is one minute.</returns>
		[ConfigurationProperty("downloadTimeout", DefaultValue = "00:02:00")]
		public TimeSpan DownloadTimeout
		{
			get
			{
				return (TimeSpan)base[WebProxyScriptElement.downloadTimeoutProp];
			}
			set
			{
				base[WebProxyScriptElement.downloadTimeoutProp] = value;
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return WebProxyScriptElement.properties;
			}
		}

		/// <summary>Gets or sets a value that defines the frequency (in seconds) that the WinHttpAutoProxySvc service attempts to retry the download of an AutoConfigUrl script.</summary>
		/// <returns>the frequency (in seconds) that the WinHttpAutoProxySvc service attempts to retry the download of an AutoConfigUrl script.</returns>
		public int AutoConfigUrlRetryInterval
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		private static ConfigurationProperty downloadTimeoutProp = new ConfigurationProperty("downloadTimeout", typeof(TimeSpan), new TimeSpan(0, 0, 2, 0));

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
