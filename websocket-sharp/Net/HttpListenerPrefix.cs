using System;

namespace WebSocketSharp.Net
{
	internal sealed class HttpListenerPrefix
	{
		internal HttpListenerPrefix(string uriPrefix, HttpListener listener)
		{
			this._original = uriPrefix;
			this._listener = listener;
			this.parse(uriPrefix);
		}

		public string Host
		{
			get
			{
				return this._host;
			}
		}

		public bool IsSecure
		{
			get
			{
				return this._secure;
			}
		}

		public HttpListener Listener
		{
			get
			{
				return this._listener;
			}
		}

		public string Original
		{
			get
			{
				return this._original;
			}
		}

		public string Path
		{
			get
			{
				return this._path;
			}
		}

		public string Port
		{
			get
			{
				return this._port;
			}
		}

		private void parse(string uriPrefix)
		{
			bool flag = uriPrefix.StartsWith("https");
			if (flag)
			{
				this._secure = true;
			}
			int length = uriPrefix.Length;
			int num = uriPrefix.IndexOf(':') + 3;
			int num2 = uriPrefix.IndexOf('/', num + 1, length - num - 1);
			int num3 = uriPrefix.LastIndexOf(':', num2 - 1, num2 - num - 1);
			bool flag2 = uriPrefix[num2 - 1] != ']' && num3 > num;
			if (flag2)
			{
				this._host = uriPrefix.Substring(num, num3 - num);
				this._port = uriPrefix.Substring(num3 + 1, num2 - num3 - 1);
			}
			else
			{
				this._host = uriPrefix.Substring(num, num2 - num);
				this._port = (this._secure ? "443" : "80");
			}
			this._path = uriPrefix.Substring(num2);
			this._prefix = string.Format("{0}://{1}:{2}{3}", new object[]
			{
				this._secure ? "https" : "http",
				this._host,
				this._port,
				this._path
			});
		}

		public static void CheckPrefix(string uriPrefix)
		{
			bool flag = uriPrefix == null;
			if (flag)
			{
				throw new ArgumentNullException("uriPrefix");
			}
			int length = uriPrefix.Length;
			bool flag2 = length == 0;
			if (flag2)
			{
				string message = "An empty string.";
				throw new ArgumentException(message, "uriPrefix");
			}
			bool flag3 = uriPrefix.StartsWith("http://") || uriPrefix.StartsWith("https://");
			bool flag4 = !flag3;
			if (flag4)
			{
				string message2 = "The scheme is not 'http' or 'https'.";
				throw new ArgumentException(message2, "uriPrefix");
			}
			int num = length - 1;
			bool flag5 = uriPrefix[num] != '/';
			if (flag5)
			{
				string message3 = "It ends without '/'.";
				throw new ArgumentException(message3, "uriPrefix");
			}
			int num2 = uriPrefix.IndexOf(':') + 3;
			bool flag6 = num2 >= num;
			if (flag6)
			{
				string message4 = "No host is specified.";
				throw new ArgumentException(message4, "uriPrefix");
			}
			bool flag7 = uriPrefix[num2] == ':';
			if (flag7)
			{
				string message5 = "No host is specified.";
				throw new ArgumentException(message5, "uriPrefix");
			}
			int num3 = uriPrefix.IndexOf('/', num2, length - num2);
			bool flag8 = num3 == num2;
			if (flag8)
			{
				string message6 = "No host is specified.";
				throw new ArgumentException(message6, "uriPrefix");
			}
			bool flag9 = uriPrefix[num3 - 1] == ':';
			if (flag9)
			{
				string message7 = "No port is specified.";
				throw new ArgumentException(message7, "uriPrefix");
			}
			bool flag10 = num3 == num - 1;
			if (flag10)
			{
				string message8 = "No path is specified.";
				throw new ArgumentException(message8, "uriPrefix");
			}
		}

		public override bool Equals(object obj)
		{
			HttpListenerPrefix httpListenerPrefix = obj as HttpListenerPrefix;
			return httpListenerPrefix != null && this._prefix.Equals(httpListenerPrefix._prefix);
		}

		public override int GetHashCode()
		{
			return this._prefix.GetHashCode();
		}

		public override string ToString()
		{
			return this._prefix;
		}

		private string _host;

		private HttpListener _listener;

		private string _original;

		private string _path;

		private string _port;

		private string _prefix;

		private bool _secure;
	}
}
