using System;
using System.IO;
using System.Net;
using UnityEngine;

namespace Meta.WitAi
{
	public class WrapHttpWebRequest : IRequest
	{
		public WrapHttpWebRequest(HttpWebRequest httpWebRequest)
		{
			if (Application.isBatchMode)
			{
				httpWebRequest.KeepAlive = false;
			}
			this._httpWebRequest = httpWebRequest;
		}

		public WebHeaderCollection Headers
		{
			get
			{
				return this._httpWebRequest.Headers;
			}
			set
			{
				this._httpWebRequest.Headers = value;
			}
		}

		public string Method
		{
			get
			{
				return this._httpWebRequest.Method;
			}
			set
			{
				this._httpWebRequest.Method = value;
			}
		}

		public string ContentType
		{
			get
			{
				return this._httpWebRequest.ContentType;
			}
			set
			{
				this._httpWebRequest.ContentType = value;
			}
		}

		public long ContentLength
		{
			get
			{
				return this._httpWebRequest.ContentLength;
			}
			set
			{
				this._httpWebRequest.ContentLength = value;
			}
		}

		public bool SendChunked
		{
			get
			{
				return this._httpWebRequest.SendChunked;
			}
			set
			{
				this._httpWebRequest.SendChunked = value;
			}
		}

		public string UserAgent
		{
			get
			{
				return this._httpWebRequest.UserAgent;
			}
			set
			{
				this._httpWebRequest.UserAgent = value;
			}
		}

		public int Timeout
		{
			get
			{
				return this._httpWebRequest.Timeout;
			}
			set
			{
				this._httpWebRequest.Timeout = value;
			}
		}

		public void Abort()
		{
			this._httpWebRequest.Abort();
		}

		public void Dispose()
		{
			this._httpWebRequest.Abort();
			this._httpWebRequest = null;
		}

		public IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			return this._httpWebRequest.BeginGetRequestStream(callback, state);
		}

		public IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			return this._httpWebRequest.BeginGetResponse(callback, state);
		}

		public Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			return this._httpWebRequest.EndGetRequestStream(asyncResult);
		}

		public WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			return this._httpWebRequest.EndGetResponse(asyncResult);
		}

		private HttpWebRequest _httpWebRequest;
	}
}
