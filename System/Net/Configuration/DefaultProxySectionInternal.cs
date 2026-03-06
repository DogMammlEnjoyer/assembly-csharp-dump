using System;
using System.Configuration;
using System.Threading;

namespace System.Net.Configuration
{
	internal sealed class DefaultProxySectionInternal
	{
		private static IWebProxy GetDefaultProxy_UsingOldMonoCode()
		{
			DefaultProxySection defaultProxySection = ConfigurationManager.GetSection("system.net/defaultProxy") as DefaultProxySection;
			if (defaultProxySection == null)
			{
				return DefaultProxySectionInternal.GetSystemWebProxy();
			}
			ProxyElement proxy = defaultProxySection.Proxy;
			WebProxy webProxy;
			if (proxy.UseSystemDefault != ProxyElement.UseSystemDefaultValues.False && proxy.ProxyAddress == null)
			{
				IWebProxy systemWebProxy = DefaultProxySectionInternal.GetSystemWebProxy();
				if (!(systemWebProxy is WebProxy))
				{
					return systemWebProxy;
				}
				webProxy = (WebProxy)systemWebProxy;
			}
			else
			{
				webProxy = new WebProxy();
			}
			if (proxy.ProxyAddress != null)
			{
				webProxy.Address = proxy.ProxyAddress;
			}
			if (proxy.BypassOnLocal != ProxyElement.BypassOnLocalValues.Unspecified)
			{
				webProxy.BypassProxyOnLocal = (proxy.BypassOnLocal == ProxyElement.BypassOnLocalValues.True);
			}
			foreach (object obj in defaultProxySection.BypassList)
			{
				BypassElement bypassElement = (BypassElement)obj;
				webProxy.BypassArrayList.Add(bypassElement.Address);
			}
			return webProxy;
		}

		private static IWebProxy GetSystemWebProxy()
		{
			return System.Net.WebProxy.CreateDefaultProxy();
		}

		internal static object ClassSyncObject
		{
			get
			{
				if (DefaultProxySectionInternal.classSyncObject == null)
				{
					object value = new object();
					Interlocked.CompareExchange(ref DefaultProxySectionInternal.classSyncObject, value, null);
				}
				return DefaultProxySectionInternal.classSyncObject;
			}
		}

		internal static DefaultProxySectionInternal GetSection()
		{
			object obj = DefaultProxySectionInternal.ClassSyncObject;
			DefaultProxySectionInternal result;
			lock (obj)
			{
				result = new DefaultProxySectionInternal
				{
					webProxy = DefaultProxySectionInternal.GetDefaultProxy_UsingOldMonoCode()
				};
			}
			return result;
		}

		internal IWebProxy WebProxy
		{
			get
			{
				return this.webProxy;
			}
		}

		private IWebProxy webProxy;

		private static object classSyncObject;
	}
}
