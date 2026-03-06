using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading;
using WebSocketSharp.Net;

namespace WebSocketSharp
{
	internal abstract class HttpBase
	{
		protected HttpBase(Version version, NameValueCollection headers)
		{
			this._version = version;
			this._headers = headers;
		}

		public string EntityBody
		{
			get
			{
				bool flag = this.EntityBodyData == null || (long)this.EntityBodyData.Length == 0L;
				string result;
				if (flag)
				{
					result = string.Empty;
				}
				else
				{
					Encoding encoding = null;
					string text = this._headers["Content-Type"];
					bool flag2 = text != null && text.Length > 0;
					if (flag2)
					{
						encoding = HttpUtility.GetEncoding(text);
					}
					result = (encoding ?? Encoding.UTF8).GetString(this.EntityBodyData);
				}
				return result;
			}
		}

		public NameValueCollection Headers
		{
			get
			{
				return this._headers;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return this._version;
			}
		}

		private static byte[] readEntityBody(Stream stream, string length)
		{
			long num;
			bool flag = !long.TryParse(length, out num);
			if (flag)
			{
				throw new ArgumentException("Cannot be parsed.", "length");
			}
			bool flag2 = num < 0L;
			if (flag2)
			{
				throw new ArgumentOutOfRangeException("length", "Less than zero.");
			}
			return (num > 1024L) ? stream.ReadBytes(num, 1024) : ((num > 0L) ? stream.ReadBytes((int)num) : null);
		}

		private static string[] readHeaders(Stream stream, int maxLength)
		{
			List<byte> buff = new List<byte>();
			int cnt = 0;
			Action<int> beforeComparing = delegate(int i)
			{
				bool flag4 = i == -1;
				if (flag4)
				{
					throw new EndOfStreamException("The header cannot be read from the data source.");
				}
				buff.Add((byte)i);
				int cnt = cnt;
				cnt++;
			};
			bool flag = false;
			while (cnt < maxLength)
			{
				bool flag2 = stream.ReadByte().IsEqualTo('\r', beforeComparing) && stream.ReadByte().IsEqualTo('\n', beforeComparing) && stream.ReadByte().IsEqualTo('\r', beforeComparing) && stream.ReadByte().IsEqualTo('\n', beforeComparing);
				if (flag2)
				{
					flag = true;
					break;
				}
			}
			bool flag3 = !flag;
			if (flag3)
			{
				throw new WebSocketException("The length of header part is greater than the max length.");
			}
			return Encoding.UTF8.GetString(buff.ToArray()).Replace("\r\n ", " ").Replace("\r\n\t", " ").Split(new string[]
			{
				"\r\n"
			}, StringSplitOptions.RemoveEmptyEntries);
		}

		protected static T Read<T>(Stream stream, Func<string[], T> parser, int millisecondsTimeout) where T : HttpBase
		{
			bool timeout = false;
			Timer timer = new Timer(delegate(object state)
			{
				timeout = true;
				stream.Close();
			}, null, millisecondsTimeout, -1);
			T t = default(T);
			Exception ex = null;
			try
			{
				t = parser(HttpBase.readHeaders(stream, 8192));
				string text = t.Headers["Content-Length"];
				bool flag = text != null && text.Length > 0;
				if (flag)
				{
					t.EntityBodyData = HttpBase.readEntityBody(stream, text);
				}
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			finally
			{
				timer.Change(-1, -1);
				timer.Dispose();
			}
			string text2 = timeout ? "A timeout has occurred while reading an HTTP request/response." : ((ex != null) ? "An exception has occurred while reading an HTTP request/response." : null);
			bool flag2 = text2 != null;
			if (flag2)
			{
				throw new WebSocketException(text2, ex);
			}
			return t;
		}

		public byte[] ToByteArray()
		{
			return Encoding.UTF8.GetBytes(this.ToString());
		}

		private NameValueCollection _headers;

		private const int _headersMaxLength = 8192;

		private Version _version;

		internal byte[] EntityBodyData;

		protected const string CrLf = "\r\n";
	}
}
