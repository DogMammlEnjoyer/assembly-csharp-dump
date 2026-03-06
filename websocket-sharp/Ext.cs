using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WebSocketSharp.Net;

namespace WebSocketSharp
{
	public static class Ext
	{
		private static byte[] compress(this byte[] data)
		{
			bool flag = (long)data.Length == 0L;
			byte[] result;
			if (flag)
			{
				result = data;
			}
			else
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					result = memoryStream.compressToArray();
				}
			}
			return result;
		}

		private static MemoryStream compress(this Stream stream)
		{
			MemoryStream memoryStream = new MemoryStream();
			bool flag = stream.Length == 0L;
			MemoryStream result;
			if (flag)
			{
				result = memoryStream;
			}
			else
			{
				stream.Position = 0L;
				using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true))
				{
					stream.CopyTo(deflateStream, 1024);
					deflateStream.Close();
					memoryStream.Write(Ext._last, 0, 1);
					memoryStream.Position = 0L;
					result = memoryStream;
				}
			}
			return result;
		}

		private static byte[] compressToArray(this Stream stream)
		{
			byte[] result;
			using (MemoryStream memoryStream = stream.compress())
			{
				memoryStream.Close();
				result = memoryStream.ToArray();
			}
			return result;
		}

		private static byte[] decompress(this byte[] data)
		{
			bool flag = (long)data.Length == 0L;
			byte[] result;
			if (flag)
			{
				result = data;
			}
			else
			{
				using (MemoryStream memoryStream = new MemoryStream(data))
				{
					result = memoryStream.decompressToArray();
				}
			}
			return result;
		}

		private static MemoryStream decompress(this Stream stream)
		{
			MemoryStream memoryStream = new MemoryStream();
			bool flag = stream.Length == 0L;
			MemoryStream result;
			if (flag)
			{
				result = memoryStream;
			}
			else
			{
				stream.Position = 0L;
				using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true))
				{
					deflateStream.CopyTo(memoryStream, 1024);
					memoryStream.Position = 0L;
					result = memoryStream;
				}
			}
			return result;
		}

		private static byte[] decompressToArray(this Stream stream)
		{
			byte[] result;
			using (MemoryStream memoryStream = stream.decompress())
			{
				memoryStream.Close();
				result = memoryStream.ToArray();
			}
			return result;
		}

		private static bool isHttpMethod(this string value)
		{
			return value == "GET" || value == "HEAD" || value == "POST" || value == "PUT" || value == "DELETE" || value == "CONNECT" || value == "OPTIONS" || value == "TRACE";
		}

		private static bool isHttpMethod10(this string value)
		{
			return value == "GET" || value == "HEAD" || value == "POST";
		}

		private static bool isPredefinedScheme(this string value)
		{
			char c = value[0];
			bool flag = c == 'h';
			bool result;
			if (flag)
			{
				result = (value == "http" || value == "https");
			}
			else
			{
				bool flag2 = c == 'w';
				if (flag2)
				{
					result = (value == "ws" || value == "wss");
				}
				else
				{
					bool flag3 = c == 'f';
					if (flag3)
					{
						result = (value == "file" || value == "ftp");
					}
					else
					{
						bool flag4 = c == 'g';
						if (flag4)
						{
							result = (value == "gopher");
						}
						else
						{
							bool flag5 = c == 'm';
							if (flag5)
							{
								result = (value == "mailto");
							}
							else
							{
								bool flag6 = c == 'n';
								if (flag6)
								{
									c = value[1];
									result = ((c == 'e') ? (value == "news" || value == "net.pipe" || value == "net.tcp") : (value == "nntp"));
								}
								else
								{
									result = false;
								}
							}
						}
					}
				}
			}
			return result;
		}

		internal static byte[] Append(this ushort code, string reason)
		{
			byte[] array = code.ToByteArray(ByteOrder.Big);
			bool flag = reason == null || reason.Length == 0;
			byte[] result;
			if (flag)
			{
				result = array;
			}
			else
			{
				List<byte> list = new List<byte>(array);
				byte[] bytes = Encoding.UTF8.GetBytes(reason);
				list.AddRange(bytes);
				result = list.ToArray();
			}
			return result;
		}

		internal static byte[] Compress(this byte[] data, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate) ? data.compress() : data;
		}

		internal static Stream Compress(this Stream stream, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate) ? stream.compress() : stream;
		}

		internal static byte[] CompressToArray(this Stream stream, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate) ? stream.compressToArray() : stream.ToByteArray();
		}

		internal static bool Contains(this string value, params char[] anyOf)
		{
			return anyOf != null && anyOf.Length != 0 && value.IndexOfAny(anyOf) > -1;
		}

		internal static bool Contains(this NameValueCollection collection, string name)
		{
			return collection[name] != null;
		}

		internal static bool Contains(this NameValueCollection collection, string name, string value, StringComparison comparisonTypeForValue)
		{
			string text = collection[name];
			bool flag = text == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				foreach (string text2 in text.Split(new char[]
				{
					','
				}))
				{
					bool flag2 = text2.Trim().Equals(value, comparisonTypeForValue);
					if (flag2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		internal static bool Contains<T>(this IEnumerable<T> source, Func<T, bool> condition)
		{
			foreach (T arg in source)
			{
				bool flag = condition(arg);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool ContainsTwice(this string[] values)
		{
			int len = values.Length;
			int end = len - 1;
			Func<int, bool> seek = null;
			seek = delegate(int idx)
			{
				bool flag = idx == end;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					string b = values[idx];
					for (int i = idx + 1; i < len; i++)
					{
						bool flag2 = values[i] == b;
						if (flag2)
						{
							return true;
						}
					}
					result = seek(++idx);
				}
				return result;
			};
			return seek(0);
		}

		internal static T[] Copy<T>(this T[] sourceArray, int length)
		{
			T[] array = new T[length];
			Array.Copy(sourceArray, 0, array, 0, length);
			return array;
		}

		internal static T[] Copy<T>(this T[] sourceArray, long length)
		{
			T[] array = new T[length];
			Array.Copy(sourceArray, 0L, array, 0L, length);
			return array;
		}

		internal static void CopyTo(this Stream sourceStream, Stream destinationStream, int bufferLength)
		{
			byte[] buffer = new byte[bufferLength];
			for (;;)
			{
				int num = sourceStream.Read(buffer, 0, bufferLength);
				bool flag = num <= 0;
				if (flag)
				{
					break;
				}
				destinationStream.Write(buffer, 0, num);
			}
		}

		internal static void CopyToAsync(this Stream sourceStream, Stream destinationStream, int bufferLength, Action completed, Action<Exception> error)
		{
			byte[] buff = new byte[bufferLength];
			AsyncCallback callback = null;
			callback = delegate(IAsyncResult ar)
			{
				try
				{
					int num = sourceStream.EndRead(ar);
					bool flag2 = num <= 0;
					if (flag2)
					{
						bool flag3 = completed != null;
						if (flag3)
						{
							completed();
						}
					}
					else
					{
						destinationStream.Write(buff, 0, num);
						sourceStream.BeginRead(buff, 0, bufferLength, callback, null);
					}
				}
				catch (Exception obj2)
				{
					bool flag4 = error != null;
					if (flag4)
					{
						error(obj2);
					}
				}
			};
			try
			{
				sourceStream.BeginRead(buff, 0, bufferLength, callback, null);
			}
			catch (Exception obj)
			{
				bool flag = error != null;
				if (flag)
				{
					error(obj);
				}
			}
		}

		internal static byte[] Decompress(this byte[] data, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate) ? data.decompress() : data;
		}

		internal static Stream Decompress(this Stream stream, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate) ? stream.decompress() : stream;
		}

		internal static byte[] DecompressToArray(this Stream stream, CompressionMethod method)
		{
			return (method == CompressionMethod.Deflate) ? stream.decompressToArray() : stream.ToByteArray();
		}

		internal static void Emit(this EventHandler eventHandler, object sender, EventArgs e)
		{
			bool flag = eventHandler == null;
			if (!flag)
			{
				eventHandler(sender, e);
			}
		}

		internal static void Emit<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e) where TEventArgs : EventArgs
		{
			bool flag = eventHandler == null;
			if (!flag)
			{
				eventHandler(sender, e);
			}
		}

		internal static string GetAbsolutePath(this Uri uri)
		{
			bool isAbsoluteUri = uri.IsAbsoluteUri;
			string result;
			if (isAbsoluteUri)
			{
				result = uri.AbsolutePath;
			}
			else
			{
				string originalString = uri.OriginalString;
				bool flag = originalString[0] != '/';
				if (flag)
				{
					result = null;
				}
				else
				{
					int num = originalString.IndexOfAny(new char[]
					{
						'?',
						'#'
					});
					result = ((num > 0) ? originalString.Substring(0, num) : originalString);
				}
			}
			return result;
		}

		internal static WebSocketSharp.Net.CookieCollection GetCookies(this NameValueCollection headers, bool response)
		{
			string text = headers[response ? "Set-Cookie" : "Cookie"];
			return (text != null) ? WebSocketSharp.Net.CookieCollection.Parse(text, response) : new WebSocketSharp.Net.CookieCollection();
		}

		internal static string GetDnsSafeHost(this Uri uri, bool bracketIPv6)
		{
			return (bracketIPv6 && uri.HostNameType == UriHostNameType.IPv6) ? uri.Host : uri.DnsSafeHost;
		}

		internal static string GetMessage(this CloseStatusCode code)
		{
			return (code == CloseStatusCode.ProtocolError) ? "A WebSocket protocol error has occurred." : ((code == CloseStatusCode.UnsupportedData) ? "Unsupported data has been received." : ((code == CloseStatusCode.Abnormal) ? "An exception has occurred." : ((code == CloseStatusCode.InvalidData) ? "Invalid data has been received." : ((code == CloseStatusCode.PolicyViolation) ? "A policy violation has occurred." : ((code == CloseStatusCode.TooBig) ? "A too big message has been received." : ((code == CloseStatusCode.MandatoryExtension) ? "WebSocket client didn't receive expected extension(s)." : ((code == CloseStatusCode.ServerError) ? "WebSocket server got an internal error." : ((code == CloseStatusCode.TlsHandshakeFailure) ? "An error has occurred during a TLS handshake." : string.Empty))))))));
		}

		internal static string GetName(this string nameAndValue, char separator)
		{
			int num = nameAndValue.IndexOf(separator);
			return (num > 0) ? nameAndValue.Substring(0, num).Trim() : null;
		}

		internal static string GetUTF8DecodedString(this byte[] bytes)
		{
			return Encoding.UTF8.GetString(bytes);
		}

		internal static byte[] GetUTF8EncodedBytes(this string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}

		internal static string GetValue(this string nameAndValue, char separator)
		{
			return nameAndValue.GetValue(separator, false);
		}

		internal static string GetValue(this string nameAndValue, char separator, bool unquote)
		{
			int num = nameAndValue.IndexOf(separator);
			bool flag = num < 0 || num == nameAndValue.Length - 1;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string text = nameAndValue.Substring(num + 1).Trim();
				result = (unquote ? text.Unquote() : text);
			}
			return result;
		}

		internal static bool IsCompressionExtension(this string value, CompressionMethod method)
		{
			string value2 = method.ToExtensionString(new string[0]);
			StringComparison comparisonType = StringComparison.Ordinal;
			return value.StartsWith(value2, comparisonType);
		}

		internal static bool IsControl(this byte opcode)
		{
			return opcode > 7 && opcode < 16;
		}

		internal static bool IsControl(this Opcode opcode)
		{
			return opcode >= Opcode.Close;
		}

		internal static bool IsData(this byte opcode)
		{
			return opcode == 1 || opcode == 2;
		}

		internal static bool IsData(this Opcode opcode)
		{
			return opcode == Opcode.Text || opcode == Opcode.Binary;
		}

		internal static bool IsEqualTo(this int value, char c, Action<int> beforeComparing)
		{
			beforeComparing(value);
			return value == (int)c;
		}

		internal static bool IsHttpMethod(this string value, Version version)
		{
			return (version == WebSocketSharp.Net.HttpVersion.Version10) ? value.isHttpMethod10() : value.isHttpMethod();
		}

		internal static bool IsPortNumber(this int value)
		{
			return value > 0 && value < 65536;
		}

		internal static bool IsReserved(this ushort code)
		{
			return code == 1004 || code == 1005 || code == 1006 || code == 1015;
		}

		internal static bool IsReserved(this CloseStatusCode code)
		{
			return code == CloseStatusCode.Undefined || code == CloseStatusCode.NoStatus || code == CloseStatusCode.Abnormal || code == CloseStatusCode.TlsHandshakeFailure;
		}

		internal static bool IsSupported(this byte opcode)
		{
			return Enum.IsDefined(typeof(Opcode), opcode);
		}

		internal static bool IsText(this string value)
		{
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				char c = value[i];
				bool flag = c < ' ';
				if (flag)
				{
					bool flag2 = "\r\n\t".IndexOf(c) == -1;
					if (flag2)
					{
						return false;
					}
					bool flag3 = c == '\n';
					if (flag3)
					{
						i++;
						bool flag4 = i == length;
						if (flag4)
						{
							break;
						}
						c = value[i];
						bool flag5 = " \t".IndexOf(c) == -1;
						if (flag5)
						{
							return false;
						}
					}
				}
				else
				{
					bool flag6 = c == '\u007f';
					if (flag6)
					{
						return false;
					}
				}
			}
			return true;
		}

		internal static bool IsToken(this string value)
		{
			int i = 0;
			while (i < value.Length)
			{
				char c = value[i];
				bool flag = c < ' ';
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					bool flag2 = c > '~';
					if (flag2)
					{
						result = false;
					}
					else
					{
						bool flag3 = "()<>@,;:\\\"/[]?={} \t".IndexOf(c) > -1;
						if (!flag3)
						{
							i++;
							continue;
						}
						result = false;
					}
				}
				return result;
			}
			return true;
		}

		internal static bool KeepsAlive(this NameValueCollection headers, Version version)
		{
			StringComparison comparisonTypeForValue = StringComparison.OrdinalIgnoreCase;
			return (version < WebSocketSharp.Net.HttpVersion.Version11) ? headers.Contains("Connection", "keep-alive", comparisonTypeForValue) : (!headers.Contains("Connection", "close", comparisonTypeForValue));
		}

		internal static bool MaybeUri(this string value)
		{
			int num = value.IndexOf(':');
			bool flag = num < 2 || num > 9;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				string value2 = value.Substring(0, num);
				result = value2.isPredefinedScheme();
			}
			return result;
		}

		internal static string Quote(this string value)
		{
			string format = "\"{0}\"";
			string arg = value.Replace("\"", "\\\"");
			return string.Format(format, arg);
		}

		internal static byte[] ReadBytes(this Stream stream, int length)
		{
			byte[] array = new byte[length];
			int num = 0;
			int num2 = 0;
			while (length > 0)
			{
				int num3 = stream.Read(array, num, length);
				bool flag = num3 <= 0;
				if (flag)
				{
					bool flag2 = num2 < Ext._retry;
					if (!flag2)
					{
						return array.SubArray(0, num);
					}
					num2++;
				}
				else
				{
					num2 = 0;
					num += num3;
					length -= num3;
				}
			}
			return array;
		}

		internal static byte[] ReadBytes(this Stream stream, long length, int bufferLength)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				byte[] buffer = new byte[bufferLength];
				int num = 0;
				while (length > 0L)
				{
					bool flag = length < (long)bufferLength;
					if (flag)
					{
						bufferLength = (int)length;
					}
					int num2 = stream.Read(buffer, 0, bufferLength);
					bool flag2 = num2 <= 0;
					if (flag2)
					{
						bool flag3 = num < Ext._retry;
						if (!flag3)
						{
							break;
						}
						num++;
					}
					else
					{
						num = 0;
						memoryStream.Write(buffer, 0, num2);
						length -= (long)num2;
					}
				}
				memoryStream.Close();
				result = memoryStream.ToArray();
			}
			return result;
		}

		internal static void ReadBytesAsync(this Stream stream, int length, Action<byte[]> completed, Action<Exception> error)
		{
			byte[] buff = new byte[length];
			int offset = 0;
			int retry = 0;
			AsyncCallback callback = null;
			callback = delegate(IAsyncResult ar)
			{
				try
				{
					int num = stream.EndRead(ar);
					bool flag2 = num <= 0;
					if (flag2)
					{
						int retry;
						bool flag3 = retry < Ext._retry;
						if (flag3)
						{
							retry = retry;
							retry++;
							stream.BeginRead(buff, offset, length, callback, null);
						}
						else
						{
							bool flag4 = completed != null;
							if (flag4)
							{
								completed(buff.SubArray(0, offset));
							}
						}
					}
					else
					{
						bool flag5 = num == length;
						if (flag5)
						{
							bool flag6 = completed != null;
							if (flag6)
							{
								completed(buff);
							}
						}
						else
						{
							int retry = 0;
							offset += num;
							length -= num;
							stream.BeginRead(buff, offset, length, callback, null);
						}
					}
				}
				catch (Exception obj2)
				{
					bool flag7 = error != null;
					if (flag7)
					{
						error(obj2);
					}
				}
			};
			try
			{
				stream.BeginRead(buff, offset, length, callback, null);
			}
			catch (Exception obj)
			{
				bool flag = error != null;
				if (flag)
				{
					error(obj);
				}
			}
		}

		internal static void ReadBytesAsync(this Stream stream, long length, int bufferLength, Action<byte[]> completed, Action<Exception> error)
		{
			MemoryStream dest = new MemoryStream();
			byte[] buff = new byte[bufferLength];
			int retry = 0;
			Action<long> read = null;
			read = delegate(long len)
			{
				bool flag2 = len < (long)bufferLength;
				if (flag2)
				{
					bufferLength = (int)len;
				}
				stream.BeginRead(buff, 0, bufferLength, delegate(IAsyncResult ar)
				{
					try
					{
						int num = stream.EndRead(ar);
						bool flag3 = num <= 0;
						if (flag3)
						{
							int retry;
							bool flag4 = retry < Ext._retry;
							if (flag4)
							{
								retry = retry;
								retry++;
								read(len);
							}
							else
							{
								bool flag5 = completed != null;
								if (flag5)
								{
									dest.Close();
									completed(dest.ToArray());
								}
								dest.Dispose();
							}
						}
						else
						{
							dest.Write(buff, 0, num);
							bool flag6 = (long)num == len;
							if (flag6)
							{
								bool flag7 = completed != null;
								if (flag7)
								{
									dest.Close();
									completed(dest.ToArray());
								}
								dest.Dispose();
							}
							else
							{
								int retry = 0;
								read(len - (long)num);
							}
						}
					}
					catch (Exception obj2)
					{
						dest.Dispose();
						bool flag8 = error != null;
						if (flag8)
						{
							error(obj2);
						}
					}
				}, null);
			};
			try
			{
				read(length);
			}
			catch (Exception obj)
			{
				dest.Dispose();
				bool flag = error != null;
				if (flag)
				{
					error(obj);
				}
			}
		}

		internal static T[] Reverse<T>(this T[] array)
		{
			int num = array.Length;
			T[] array2 = new T[num];
			int num2 = num - 1;
			for (int i = 0; i <= num2; i++)
			{
				array2[i] = array[num2 - i];
			}
			return array2;
		}

		internal static IEnumerable<string> SplitHeaderValue(this string value, params char[] separators)
		{
			int len = value.Length;
			int end = len - 1;
			StringBuilder buff = new StringBuilder(32);
			bool escaped = false;
			bool quoted = false;
			int num;
			for (int i = 0; i <= end; i = num + 1)
			{
				char c = value[i];
				buff.Append(c);
				bool flag = c == '"';
				if (flag)
				{
					bool flag2 = escaped;
					if (flag2)
					{
						escaped = false;
					}
					else
					{
						quoted = !quoted;
					}
				}
				else
				{
					bool flag3 = c == '\\';
					if (flag3)
					{
						bool flag4 = i == end;
						if (flag4)
						{
							break;
						}
						bool flag5 = value[i + 1] == '"';
						if (flag5)
						{
							escaped = true;
						}
					}
					else
					{
						bool flag6 = Array.IndexOf<char>(separators, c) > -1;
						if (flag6)
						{
							bool flag7 = quoted;
							if (!flag7)
							{
								buff.Length--;
								yield return buff.ToString();
								buff.Length = 0;
							}
						}
					}
				}
				num = i;
			}
			yield return buff.ToString();
			yield break;
		}

		internal static byte[] ToByteArray(this Stream stream)
		{
			stream.Position = 0L;
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream, 1024);
				memoryStream.Close();
				result = memoryStream.ToArray();
			}
			return result;
		}

		internal static byte[] ToByteArray(this ushort value, ByteOrder order)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			bool flag = !order.IsHostOrder();
			if (flag)
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		internal static byte[] ToByteArray(this ulong value, ByteOrder order)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			bool flag = !order.IsHostOrder();
			if (flag)
			{
				Array.Reverse(bytes);
			}
			return bytes;
		}

		internal static CompressionMethod ToCompressionMethod(this string value)
		{
			Array values = Enum.GetValues(typeof(CompressionMethod));
			foreach (object obj in values)
			{
				CompressionMethod compressionMethod = (CompressionMethod)obj;
				bool flag = compressionMethod.ToExtensionString(new string[0]) == value;
				if (flag)
				{
					return compressionMethod;
				}
			}
			return CompressionMethod.None;
		}

		internal static string ToExtensionString(this CompressionMethod method, params string[] parameters)
		{
			bool flag = method == CompressionMethod.None;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				string text = string.Format("permessage-{0}", method.ToString().ToLower());
				result = ((parameters != null && parameters.Length != 0) ? string.Format("{0}; {1}", text, parameters.ToString("; ")) : text);
			}
			return result;
		}

		internal static IPAddress ToIPAddress(this string value)
		{
			bool flag = value == null || value.Length == 0;
			IPAddress result;
			if (flag)
			{
				result = null;
			}
			else
			{
				IPAddress ipaddress;
				bool flag2 = IPAddress.TryParse(value, out ipaddress);
				if (flag2)
				{
					result = ipaddress;
				}
				else
				{
					try
					{
						IPAddress[] hostAddresses = Dns.GetHostAddresses(value);
						result = hostAddresses[0];
					}
					catch
					{
						result = null;
					}
				}
			}
			return result;
		}

		internal static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
		{
			return new List<TSource>(source);
		}

		internal static string ToString(this IPAddress address, bool bracketIPv6)
		{
			return (bracketIPv6 && address.AddressFamily == AddressFamily.InterNetworkV6) ? string.Format("[{0}]", address.ToString()) : address.ToString();
		}

		internal static ushort ToUInt16(this byte[] source, ByteOrder sourceOrder)
		{
			return BitConverter.ToUInt16(source.ToHostOrder(sourceOrder), 0);
		}

		internal static ulong ToUInt64(this byte[] source, ByteOrder sourceOrder)
		{
			return BitConverter.ToUInt64(source.ToHostOrder(sourceOrder), 0);
		}

		internal static IEnumerable<string> TrimEach(this IEnumerable<string> source)
		{
			foreach (string elm in source)
			{
				yield return elm.Trim();
				elm = null;
			}
			IEnumerator<string> enumerator = null;
			yield break;
			yield break;
		}

		internal static string TrimSlashFromEnd(this string value)
		{
			string text = value.TrimEnd(new char[]
			{
				'/'
			});
			return (text.Length > 0) ? text : "/";
		}

		internal static string TrimSlashOrBackslashFromEnd(this string value)
		{
			string text = value.TrimEnd(new char[]
			{
				'/',
				'\\'
			});
			return (text.Length > 0) ? text : value[0].ToString();
		}

		internal static bool TryCreateVersion(this string versionString, out Version result)
		{
			result = null;
			try
			{
				result = new Version(versionString);
			}
			catch
			{
				return false;
			}
			return true;
		}

		internal static bool TryCreateWebSocketUri(this string uriString, out Uri result, out string message)
		{
			result = null;
			message = null;
			Uri uri = uriString.ToUri();
			bool flag = uri == null;
			bool result2;
			if (flag)
			{
				message = "An invalid URI string.";
				result2 = false;
			}
			else
			{
				bool flag2 = !uri.IsAbsoluteUri;
				if (flag2)
				{
					message = "A relative URI.";
					result2 = false;
				}
				else
				{
					string scheme = uri.Scheme;
					bool flag3 = !(scheme == "ws") && !(scheme == "wss");
					if (flag3)
					{
						message = "The scheme part is not 'ws' or 'wss'.";
						result2 = false;
					}
					else
					{
						int port = uri.Port;
						bool flag4 = port == 0;
						if (flag4)
						{
							message = "The port part is zero.";
							result2 = false;
						}
						else
						{
							bool flag5 = uri.Fragment.Length > 0;
							if (flag5)
							{
								message = "It includes the fragment component.";
								result2 = false;
							}
							else
							{
								result = ((port != -1) ? uri : new Uri(string.Format("{0}://{1}:{2}{3}", new object[]
								{
									scheme,
									uri.Host,
									(scheme == "ws") ? 80 : 443,
									uri.PathAndQuery
								})));
								result2 = true;
							}
						}
					}
				}
			}
			return result2;
		}

		internal static bool TryGetUTF8DecodedString(this byte[] bytes, out string s)
		{
			s = null;
			try
			{
				s = Encoding.UTF8.GetString(bytes);
			}
			catch
			{
				return false;
			}
			return true;
		}

		internal static bool TryGetUTF8EncodedBytes(this string s, out byte[] bytes)
		{
			bytes = null;
			try
			{
				bytes = Encoding.UTF8.GetBytes(s);
			}
			catch
			{
				return false;
			}
			return true;
		}

		internal static bool TryOpenRead(this FileInfo fileInfo, out FileStream fileStream)
		{
			fileStream = null;
			try
			{
				fileStream = fileInfo.OpenRead();
			}
			catch
			{
				return false;
			}
			return true;
		}

		internal static string Unquote(this string value)
		{
			int num = value.IndexOf('"');
			bool flag = num == -1;
			string result;
			if (flag)
			{
				result = value;
			}
			else
			{
				int num2 = value.LastIndexOf('"');
				bool flag2 = num2 == num;
				if (flag2)
				{
					result = value;
				}
				else
				{
					int num3 = num2 - num - 1;
					result = ((num3 > 0) ? value.Substring(num + 1, num3).Replace("\\\"", "\"") : string.Empty);
				}
			}
			return result;
		}

		internal static bool Upgrades(this NameValueCollection headers, string protocol)
		{
			StringComparison comparisonTypeForValue = StringComparison.OrdinalIgnoreCase;
			return headers.Contains("Upgrade", protocol, comparisonTypeForValue) && headers.Contains("Connection", "Upgrade", comparisonTypeForValue);
		}

		internal static string UrlDecode(this string value, Encoding encoding)
		{
			return HttpUtility.UrlDecode(value, encoding);
		}

		internal static string UrlEncode(this string value, Encoding encoding)
		{
			return HttpUtility.UrlEncode(value, encoding);
		}

		internal static void WriteBytes(this Stream stream, byte[] bytes, int bufferLength)
		{
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				memoryStream.CopyTo(stream, bufferLength);
			}
		}

		internal static void WriteBytesAsync(this Stream stream, byte[] bytes, int bufferLength, Action completed, Action<Exception> error)
		{
			MemoryStream src = new MemoryStream(bytes);
			src.CopyToAsync(stream, bufferLength, delegate
			{
				bool flag = completed != null;
				if (flag)
				{
					completed();
				}
				src.Dispose();
			}, delegate(Exception ex)
			{
				src.Dispose();
				bool flag = error != null;
				if (flag)
				{
					error(ex);
				}
			});
		}

		public static string GetDescription(this WebSocketSharp.Net.HttpStatusCode code)
		{
			return ((int)code).GetStatusDescription();
		}

		public static string GetStatusDescription(this int code)
		{
			if (code <= 207)
			{
				switch (code)
				{
				case 100:
					return "Continue";
				case 101:
					return "Switching Protocols";
				case 102:
					return "Processing";
				default:
					switch (code)
					{
					case 200:
						return "OK";
					case 201:
						return "Created";
					case 202:
						return "Accepted";
					case 203:
						return "Non-Authoritative Information";
					case 204:
						return "No Content";
					case 205:
						return "Reset Content";
					case 206:
						return "Partial Content";
					case 207:
						return "Multi-Status";
					}
					break;
				}
			}
			else
			{
				switch (code)
				{
				case 300:
					return "Multiple Choices";
				case 301:
					return "Moved Permanently";
				case 302:
					return "Found";
				case 303:
					return "See Other";
				case 304:
					return "Not Modified";
				case 305:
					return "Use Proxy";
				case 306:
					break;
				case 307:
					return "Temporary Redirect";
				default:
					switch (code)
					{
					case 400:
						return "Bad Request";
					case 401:
						return "Unauthorized";
					case 402:
						return "Payment Required";
					case 403:
						return "Forbidden";
					case 404:
						return "Not Found";
					case 405:
						return "Method Not Allowed";
					case 406:
						return "Not Acceptable";
					case 407:
						return "Proxy Authentication Required";
					case 408:
						return "Request Timeout";
					case 409:
						return "Conflict";
					case 410:
						return "Gone";
					case 411:
						return "Length Required";
					case 412:
						return "Precondition Failed";
					case 413:
						return "Request Entity Too Large";
					case 414:
						return "Request-Uri Too Long";
					case 415:
						return "Unsupported Media Type";
					case 416:
						return "Requested Range Not Satisfiable";
					case 417:
						return "Expectation Failed";
					case 418:
					case 419:
					case 420:
					case 421:
						break;
					case 422:
						return "Unprocessable Entity";
					case 423:
						return "Locked";
					case 424:
						return "Failed Dependency";
					default:
						switch (code)
						{
						case 500:
							return "Internal Server Error";
						case 501:
							return "Not Implemented";
						case 502:
							return "Bad Gateway";
						case 503:
							return "Service Unavailable";
						case 504:
							return "Gateway Timeout";
						case 505:
							return "Http Version Not Supported";
						case 507:
							return "Insufficient Storage";
						}
						break;
					}
					break;
				}
			}
			return string.Empty;
		}

		public static bool IsCloseStatusCode(this ushort value)
		{
			return value > 999 && value < 5000;
		}

		public static bool IsEnclosedIn(this string value, char c)
		{
			bool flag = value == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				int length = value.Length;
				result = (length > 1 && value[0] == c && value[length - 1] == c);
			}
			return result;
		}

		public static bool IsHostOrder(this ByteOrder order)
		{
			return BitConverter.IsLittleEndian == (order == ByteOrder.Little);
		}

		public static bool IsLocal(this IPAddress address)
		{
			bool flag = address == null;
			if (flag)
			{
				throw new ArgumentNullException("address");
			}
			bool flag2 = address.Equals(IPAddress.Any);
			bool result;
			if (flag2)
			{
				result = true;
			}
			else
			{
				bool flag3 = address.Equals(IPAddress.Loopback);
				if (flag3)
				{
					result = true;
				}
				else
				{
					bool ossupportsIPv = Socket.OSSupportsIPv6;
					if (ossupportsIPv)
					{
						bool flag4 = address.Equals(IPAddress.IPv6Any);
						if (flag4)
						{
							return true;
						}
						bool flag5 = address.Equals(IPAddress.IPv6Loopback);
						if (flag5)
						{
							return true;
						}
					}
					string hostName = Dns.GetHostName();
					IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
					foreach (IPAddress obj in hostAddresses)
					{
						bool flag6 = address.Equals(obj);
						if (flag6)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		public static bool IsNullOrEmpty(this string value)
		{
			return value == null || value.Length == 0;
		}

		public static T[] SubArray<T>(this T[] array, int startIndex, int length)
		{
			bool flag = array == null;
			if (flag)
			{
				throw new ArgumentNullException("array");
			}
			int num = array.Length;
			bool flag2 = num == 0;
			T[] result;
			if (flag2)
			{
				bool flag3 = startIndex != 0;
				if (flag3)
				{
					throw new ArgumentOutOfRangeException("startIndex");
				}
				bool flag4 = length != 0;
				if (flag4)
				{
					throw new ArgumentOutOfRangeException("length");
				}
				result = array;
			}
			else
			{
				bool flag5 = startIndex < 0 || startIndex >= num;
				if (flag5)
				{
					throw new ArgumentOutOfRangeException("startIndex");
				}
				bool flag6 = length < 0 || length > num - startIndex;
				if (flag6)
				{
					throw new ArgumentOutOfRangeException("length");
				}
				bool flag7 = length == 0;
				if (flag7)
				{
					result = new T[0];
				}
				else
				{
					bool flag8 = length == num;
					if (flag8)
					{
						result = array;
					}
					else
					{
						T[] array2 = new T[length];
						Array.Copy(array, startIndex, array2, 0, length);
						result = array2;
					}
				}
			}
			return result;
		}

		public static T[] SubArray<T>(this T[] array, long startIndex, long length)
		{
			bool flag = array == null;
			if (flag)
			{
				throw new ArgumentNullException("array");
			}
			long num = (long)array.Length;
			bool flag2 = num == 0L;
			T[] result;
			if (flag2)
			{
				bool flag3 = startIndex != 0L;
				if (flag3)
				{
					throw new ArgumentOutOfRangeException("startIndex");
				}
				bool flag4 = length != 0L;
				if (flag4)
				{
					throw new ArgumentOutOfRangeException("length");
				}
				result = array;
			}
			else
			{
				bool flag5 = startIndex < 0L || startIndex >= num;
				if (flag5)
				{
					throw new ArgumentOutOfRangeException("startIndex");
				}
				bool flag6 = length < 0L || length > num - startIndex;
				if (flag6)
				{
					throw new ArgumentOutOfRangeException("length");
				}
				bool flag7 = length == 0L;
				if (flag7)
				{
					result = new T[0];
				}
				else
				{
					bool flag8 = length == num;
					if (flag8)
					{
						result = array;
					}
					else
					{
						T[] array2 = new T[length];
						Array.Copy(array, startIndex, array2, 0L, length);
						result = array2;
					}
				}
			}
			return result;
		}

		public static void Times(this int n, Action<int> action)
		{
			bool flag = n <= 0;
			if (!flag)
			{
				bool flag2 = action == null;
				if (!flag2)
				{
					for (int i = 0; i < n; i++)
					{
						action(i);
					}
				}
			}
		}

		public static void Times(this long n, Action<long> action)
		{
			bool flag = n <= 0L;
			if (!flag)
			{
				bool flag2 = action == null;
				if (!flag2)
				{
					for (long num = 0L; num < n; num += 1L)
					{
						action(num);
					}
				}
			}
		}

		public static byte[] ToHostOrder(this byte[] source, ByteOrder sourceOrder)
		{
			bool flag = source == null;
			if (flag)
			{
				throw new ArgumentNullException("source");
			}
			bool flag2 = source.Length < 2;
			byte[] result;
			if (flag2)
			{
				result = source;
			}
			else
			{
				bool flag3 = sourceOrder.IsHostOrder();
				if (flag3)
				{
					result = source;
				}
				else
				{
					result = source.Reverse<byte>();
				}
			}
			return result;
		}

		public static string ToString<T>(this T[] array, string separator)
		{
			bool flag = array == null;
			if (flag)
			{
				throw new ArgumentNullException("array");
			}
			int num = array.Length;
			bool flag2 = num == 0;
			string result;
			if (flag2)
			{
				result = string.Empty;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder(64);
				int num2 = num - 1;
				for (int i = 0; i < num2; i++)
				{
					stringBuilder.AppendFormat("{0}{1}", array[i], separator);
				}
				stringBuilder.AppendFormat("{0}", array[num2]);
				result = stringBuilder.ToString();
			}
			return result;
		}

		public static Uri ToUri(this string value)
		{
			bool flag = value == null || value.Length == 0;
			Uri result;
			if (flag)
			{
				result = null;
			}
			else
			{
				UriKind uriKind = value.MaybeUri() ? UriKind.Absolute : UriKind.Relative;
				Uri uri;
				Uri.TryCreate(value, uriKind, out uri);
				result = uri;
			}
			return result;
		}

		private static readonly byte[] _last = new byte[1];

		private static readonly int _retry = 5;

		private const string _tspecials = "()<>@,;:\\\"/[]?={} \t";
	}
}
