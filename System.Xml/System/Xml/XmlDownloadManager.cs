using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Xml
{
	internal class XmlDownloadManager
	{
		internal Stream GetStream(Uri uri, ICredentials credentials, IWebProxy proxy, RequestCachePolicy cachePolicy)
		{
			if (uri.Scheme == "file")
			{
				return new FileStream(uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1);
			}
			return this.GetNonFileStream(uri, credentials, proxy, cachePolicy);
		}

		private Stream GetNonFileStream(Uri uri, ICredentials credentials, IWebProxy proxy, RequestCachePolicy cachePolicy)
		{
			WebRequest webRequest = WebRequest.Create(uri);
			if (credentials != null)
			{
				webRequest.Credentials = credentials;
			}
			if (proxy != null)
			{
				webRequest.Proxy = proxy;
			}
			if (cachePolicy != null)
			{
				webRequest.CachePolicy = cachePolicy;
			}
			WebResponse response = webRequest.GetResponse();
			HttpWebRequest httpWebRequest = webRequest as HttpWebRequest;
			if (httpWebRequest != null)
			{
				lock (this)
				{
					if (this.connections == null)
					{
						this.connections = new Hashtable();
					}
					OpenedHost openedHost = (OpenedHost)this.connections[httpWebRequest.Address.Host];
					if (openedHost == null)
					{
						openedHost = new OpenedHost();
					}
					if (openedHost.nonCachedConnectionsCount < httpWebRequest.ServicePoint.ConnectionLimit - 1)
					{
						if (openedHost.nonCachedConnectionsCount == 0)
						{
							this.connections.Add(httpWebRequest.Address.Host, openedHost);
						}
						openedHost.nonCachedConnectionsCount++;
						return new XmlRegisteredNonCachedStream(response.GetResponseStream(), this, httpWebRequest.Address.Host);
					}
					return new XmlCachedStream(response.ResponseUri, response.GetResponseStream());
				}
			}
			return response.GetResponseStream();
		}

		internal void Remove(string host)
		{
			lock (this)
			{
				OpenedHost openedHost = (OpenedHost)this.connections[host];
				if (openedHost != null)
				{
					OpenedHost openedHost2 = openedHost;
					int num = openedHost2.nonCachedConnectionsCount - 1;
					openedHost2.nonCachedConnectionsCount = num;
					if (num == 0)
					{
						this.connections.Remove(host);
					}
				}
			}
		}

		internal Task<Stream> GetStreamAsync(Uri uri, ICredentials credentials, IWebProxy proxy, RequestCachePolicy cachePolicy)
		{
			if (uri.Scheme == "file")
			{
				return Task.Run<Stream>(() => new FileStream(uri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1, true));
			}
			return this.GetNonFileStreamAsync(uri, credentials, proxy, cachePolicy);
		}

		private Task<Stream> GetNonFileStreamAsync(Uri uri, ICredentials credentials, IWebProxy proxy, RequestCachePolicy cachePolicy)
		{
			XmlDownloadManager.<GetNonFileStreamAsync>d__5 <GetNonFileStreamAsync>d__;
			<GetNonFileStreamAsync>d__.<>4__this = this;
			<GetNonFileStreamAsync>d__.uri = uri;
			<GetNonFileStreamAsync>d__.credentials = credentials;
			<GetNonFileStreamAsync>d__.proxy = proxy;
			<GetNonFileStreamAsync>d__.cachePolicy = cachePolicy;
			<GetNonFileStreamAsync>d__.<>t__builder = AsyncTaskMethodBuilder<Stream>.Create();
			<GetNonFileStreamAsync>d__.<>1__state = -1;
			<GetNonFileStreamAsync>d__.<>t__builder.Start<XmlDownloadManager.<GetNonFileStreamAsync>d__5>(ref <GetNonFileStreamAsync>d__);
			return <GetNonFileStreamAsync>d__.<>t__builder.Task;
		}

		private Hashtable connections;
	}
}
