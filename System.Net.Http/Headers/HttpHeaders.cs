using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace System.Net.Http.Headers
{
	/// <summary>A collection of headers and their values as defined in RFC 2616.</summary>
	public abstract class HttpHeaders : IEnumerable<KeyValuePair<string, IEnumerable<string>>>, IEnumerable
	{
		static HttpHeaders()
		{
			HeaderInfo[] array = new HeaderInfo[]
			{
				HeaderInfo.CreateMulti<MediaTypeWithQualityHeaderValue>("Accept", new TryParseListDelegate<MediaTypeWithQualityHeaderValue>(MediaTypeWithQualityHeaderValue.TryParse), HttpHeaderKind.Request, 1, ", "),
				HeaderInfo.CreateMulti<StringWithQualityHeaderValue>("Accept-Charset", new TryParseListDelegate<StringWithQualityHeaderValue>(StringWithQualityHeaderValue.TryParse), HttpHeaderKind.Request, 1, ", "),
				HeaderInfo.CreateMulti<StringWithQualityHeaderValue>("Accept-Encoding", new TryParseListDelegate<StringWithQualityHeaderValue>(StringWithQualityHeaderValue.TryParse), HttpHeaderKind.Request, 1, ", "),
				HeaderInfo.CreateMulti<StringWithQualityHeaderValue>("Accept-Language", new TryParseListDelegate<StringWithQualityHeaderValue>(StringWithQualityHeaderValue.TryParse), HttpHeaderKind.Request, 1, ", "),
				HeaderInfo.CreateMulti<string>("Accept-Ranges", new TryParseListDelegate<string>(CollectionParser.TryParse), HttpHeaderKind.Response, 1, ", "),
				HeaderInfo.CreateSingle<TimeSpan>("Age", new TryParseDelegate<TimeSpan>(Parser.TimeSpanSeconds.TryParse), HttpHeaderKind.Response, null),
				HeaderInfo.CreateMulti<string>("Allow", new TryParseListDelegate<string>(CollectionParser.TryParse), HttpHeaderKind.Content, 0, ", "),
				HeaderInfo.CreateSingle<AuthenticationHeaderValue>("Authorization", new TryParseDelegate<AuthenticationHeaderValue>(AuthenticationHeaderValue.TryParse), HttpHeaderKind.Request, null),
				HeaderInfo.CreateSingle<CacheControlHeaderValue>("Cache-Control", new TryParseDelegate<CacheControlHeaderValue>(CacheControlHeaderValue.TryParse), HttpHeaderKind.Request | HttpHeaderKind.Response, null),
				HeaderInfo.CreateMulti<string>("Connection", new TryParseListDelegate<string>(CollectionParser.TryParse), HttpHeaderKind.Request | HttpHeaderKind.Response, 1, ", "),
				HeaderInfo.CreateSingle<ContentDispositionHeaderValue>("Content-Disposition", new TryParseDelegate<ContentDispositionHeaderValue>(ContentDispositionHeaderValue.TryParse), HttpHeaderKind.Content, null),
				HeaderInfo.CreateMulti<string>("Content-Encoding", new TryParseListDelegate<string>(CollectionParser.TryParse), HttpHeaderKind.Content, 1, ", "),
				HeaderInfo.CreateMulti<string>("Content-Language", new TryParseListDelegate<string>(CollectionParser.TryParse), HttpHeaderKind.Content, 1, ", "),
				HeaderInfo.CreateSingle<long>("Content-Length", new TryParseDelegate<long>(Parser.Long.TryParse), HttpHeaderKind.Content, null),
				HeaderInfo.CreateSingle<Uri>("Content-Location", new TryParseDelegate<Uri>(Parser.Uri.TryParse), HttpHeaderKind.Content, null),
				HeaderInfo.CreateSingle<byte[]>("Content-MD5", new TryParseDelegate<byte[]>(Parser.MD5.TryParse), HttpHeaderKind.Content, null),
				HeaderInfo.CreateSingle<ContentRangeHeaderValue>("Content-Range", new TryParseDelegate<ContentRangeHeaderValue>(ContentRangeHeaderValue.TryParse), HttpHeaderKind.Content, null),
				HeaderInfo.CreateSingle<MediaTypeHeaderValue>("Content-Type", new TryParseDelegate<MediaTypeHeaderValue>(MediaTypeHeaderValue.TryParse), HttpHeaderKind.Content, null),
				HeaderInfo.CreateSingle<DateTimeOffset>("Date", new TryParseDelegate<DateTimeOffset>(Parser.DateTime.TryParse), HttpHeaderKind.Request | HttpHeaderKind.Response, Parser.DateTime.ToString),
				HeaderInfo.CreateSingle<EntityTagHeaderValue>("ETag", new TryParseDelegate<EntityTagHeaderValue>(EntityTagHeaderValue.TryParse), HttpHeaderKind.Response, null),
				HeaderInfo.CreateMulti<NameValueWithParametersHeaderValue>("Expect", new TryParseListDelegate<NameValueWithParametersHeaderValue>(NameValueWithParametersHeaderValue.TryParse), HttpHeaderKind.Request, 1, ", "),
				HeaderInfo.CreateSingle<DateTimeOffset>("Expires", new TryParseDelegate<DateTimeOffset>(Parser.DateTime.TryParse), HttpHeaderKind.Content, Parser.DateTime.ToString),
				HeaderInfo.CreateSingle<string>("From", new TryParseDelegate<string>(Parser.EmailAddress.TryParse), HttpHeaderKind.Request, null),
				HeaderInfo.CreateSingle<string>("Host", new TryParseDelegate<string>(Parser.Host.TryParse), HttpHeaderKind.Request, null),
				HeaderInfo.CreateMulti<EntityTagHeaderValue>("If-Match", new TryParseListDelegate<EntityTagHeaderValue>(EntityTagHeaderValue.TryParse), HttpHeaderKind.Request, 1, ", "),
				HeaderInfo.CreateSingle<DateTimeOffset>("If-Modified-Since", new TryParseDelegate<DateTimeOffset>(Parser.DateTime.TryParse), HttpHeaderKind.Request, Parser.DateTime.ToString),
				HeaderInfo.CreateMulti<EntityTagHeaderValue>("If-None-Match", new TryParseListDelegate<EntityTagHeaderValue>(EntityTagHeaderValue.TryParse), HttpHeaderKind.Request, 1, ", "),
				HeaderInfo.CreateSingle<RangeConditionHeaderValue>("If-Range", new TryParseDelegate<RangeConditionHeaderValue>(RangeConditionHeaderValue.TryParse), HttpHeaderKind.Request, null),
				HeaderInfo.CreateSingle<DateTimeOffset>("If-Unmodified-Since", new TryParseDelegate<DateTimeOffset>(Parser.DateTime.TryParse), HttpHeaderKind.Request, Parser.DateTime.ToString),
				HeaderInfo.CreateSingle<DateTimeOffset>("Last-Modified", new TryParseDelegate<DateTimeOffset>(Parser.DateTime.TryParse), HttpHeaderKind.Content, Parser.DateTime.ToString),
				HeaderInfo.CreateSingle<Uri>("Location", new TryParseDelegate<Uri>(Parser.Uri.TryParse), HttpHeaderKind.Response, null),
				HeaderInfo.CreateSingle<int>("Max-Forwards", new TryParseDelegate<int>(Parser.Int.TryParse), HttpHeaderKind.Request, null),
				HeaderInfo.CreateMulti<NameValueHeaderValue>("Pragma", new TryParseListDelegate<NameValueHeaderValue>(NameValueHeaderValue.TryParsePragma), HttpHeaderKind.Request | HttpHeaderKind.Response, 1, ", "),
				HeaderInfo.CreateMulti<AuthenticationHeaderValue>("Proxy-Authenticate", new TryParseListDelegate<AuthenticationHeaderValue>(AuthenticationHeaderValue.TryParse), HttpHeaderKind.Response, 1, ", "),
				HeaderInfo.CreateSingle<AuthenticationHeaderValue>("Proxy-Authorization", new TryParseDelegate<AuthenticationHeaderValue>(AuthenticationHeaderValue.TryParse), HttpHeaderKind.Request, null),
				HeaderInfo.CreateSingle<RangeHeaderValue>("Range", new TryParseDelegate<RangeHeaderValue>(RangeHeaderValue.TryParse), HttpHeaderKind.Request, null),
				HeaderInfo.CreateSingle<Uri>("Referer", new TryParseDelegate<Uri>(Parser.Uri.TryParse), HttpHeaderKind.Request, null),
				HeaderInfo.CreateSingle<RetryConditionHeaderValue>("Retry-After", new TryParseDelegate<RetryConditionHeaderValue>(RetryConditionHeaderValue.TryParse), HttpHeaderKind.Response, null),
				HeaderInfo.CreateMulti<ProductInfoHeaderValue>("Server", new TryParseListDelegate<ProductInfoHeaderValue>(ProductInfoHeaderValue.TryParse), HttpHeaderKind.Response, 1, " "),
				HeaderInfo.CreateMulti<TransferCodingWithQualityHeaderValue>("TE", new TryParseListDelegate<TransferCodingWithQualityHeaderValue>(TransferCodingWithQualityHeaderValue.TryParse), HttpHeaderKind.Request, 0, ", "),
				HeaderInfo.CreateMulti<string>("Trailer", new TryParseListDelegate<string>(CollectionParser.TryParse), HttpHeaderKind.Request | HttpHeaderKind.Response, 1, ", "),
				HeaderInfo.CreateMulti<TransferCodingHeaderValue>("Transfer-Encoding", new TryParseListDelegate<TransferCodingHeaderValue>(TransferCodingHeaderValue.TryParse), HttpHeaderKind.Request | HttpHeaderKind.Response, 1, ", "),
				HeaderInfo.CreateMulti<ProductHeaderValue>("Upgrade", new TryParseListDelegate<ProductHeaderValue>(ProductHeaderValue.TryParse), HttpHeaderKind.Request | HttpHeaderKind.Response, 1, ", "),
				HeaderInfo.CreateMulti<ProductInfoHeaderValue>("User-Agent", new TryParseListDelegate<ProductInfoHeaderValue>(ProductInfoHeaderValue.TryParse), HttpHeaderKind.Request, 1, " "),
				HeaderInfo.CreateMulti<string>("Vary", new TryParseListDelegate<string>(CollectionParser.TryParse), HttpHeaderKind.Response, 1, ", "),
				HeaderInfo.CreateMulti<ViaHeaderValue>("Via", new TryParseListDelegate<ViaHeaderValue>(ViaHeaderValue.TryParse), HttpHeaderKind.Request | HttpHeaderKind.Response, 1, ", "),
				HeaderInfo.CreateMulti<WarningHeaderValue>("Warning", new TryParseListDelegate<WarningHeaderValue>(WarningHeaderValue.TryParse), HttpHeaderKind.Request | HttpHeaderKind.Response, 1, ", "),
				HeaderInfo.CreateMulti<AuthenticationHeaderValue>("WWW-Authenticate", new TryParseListDelegate<AuthenticationHeaderValue>(AuthenticationHeaderValue.TryParse), HttpHeaderKind.Response, 1, ", ")
			};
			HttpHeaders.known_headers = new Dictionary<string, HeaderInfo>(StringComparer.OrdinalIgnoreCase);
			foreach (HeaderInfo headerInfo in array)
			{
				HttpHeaders.known_headers.Add(headerInfo.Name, headerInfo);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Http.Headers.HttpHeaders" /> class.</summary>
		protected HttpHeaders()
		{
			this.headers = new Dictionary<string, HttpHeaders.HeaderBucket>(StringComparer.OrdinalIgnoreCase);
		}

		internal HttpHeaders(HttpHeaderKind headerKind) : this()
		{
			this.HeaderKind = headerKind;
		}

		/// <summary>Adds the specified header and its value into the <see cref="T:System.Net.Http.Headers.HttpHeaders" /> collection.</summary>
		/// <param name="name">The header to add to the collection.</param>
		/// <param name="value">The content of the header.</param>
		public void Add(string name, string value)
		{
			this.Add(name, new string[]
			{
				value
			});
		}

		/// <summary>Adds the specified header and its values into the <see cref="T:System.Net.Http.Headers.HttpHeaders" /> collection.</summary>
		/// <param name="name">The header to add to the collection.</param>
		/// <param name="values">A list of header values to add to the collection.</param>
		public void Add(string name, IEnumerable<string> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			this.AddInternal(name, values, this.CheckName(name), false);
		}

		internal bool AddValue(string value, HeaderInfo headerInfo, bool ignoreInvalid)
		{
			return this.AddInternal(headerInfo.Name, new string[]
			{
				value
			}, headerInfo, ignoreInvalid);
		}

		private bool AddInternal(string name, IEnumerable<string> values, HeaderInfo headerInfo, bool ignoreInvalid)
		{
			HttpHeaders.HeaderBucket headerBucket;
			this.headers.TryGetValue(name, out headerBucket);
			bool result = true;
			foreach (string text in values)
			{
				bool flag = headerBucket == null;
				if (headerInfo != null)
				{
					object obj;
					if (!headerInfo.TryParse(text, out obj))
					{
						if (ignoreInvalid)
						{
							result = false;
							continue;
						}
						throw new FormatException("Could not parse value for header '" + name + "'");
					}
					else if (headerInfo.AllowsMany)
					{
						if (headerBucket == null)
						{
							headerBucket = new HttpHeaders.HeaderBucket(headerInfo.CreateCollection(this), headerInfo.CustomToString);
						}
						headerInfo.AddToCollection(headerBucket.Parsed, obj);
					}
					else
					{
						if (headerBucket != null)
						{
							throw new FormatException();
						}
						headerBucket = new HttpHeaders.HeaderBucket(obj, headerInfo.CustomToString);
					}
				}
				else
				{
					if (headerBucket == null)
					{
						headerBucket = new HttpHeaders.HeaderBucket(null, null);
					}
					headerBucket.Values.Add(text ?? string.Empty);
				}
				if (flag)
				{
					this.headers.Add(name, headerBucket);
				}
			}
			return result;
		}

		/// <summary>Returns a value that indicates whether the specified header and its value were added to the <see cref="T:System.Net.Http.Headers.HttpHeaders" /> collection without validating the provided information.</summary>
		/// <param name="name">The header to add to the collection.</param>
		/// <param name="value">The content of the header.</param>
		/// <returns>
		///   <see langword="true" /> if the specified header <paramref name="name" /> and <paramref name="value" /> could be added to the collection; otherwise <see langword="false" />.</returns>
		public bool TryAddWithoutValidation(string name, string value)
		{
			return this.TryAddWithoutValidation(name, new string[]
			{
				value
			});
		}

		/// <summary>Returns a value that indicates whether the specified header and its values were added to the <see cref="T:System.Net.Http.Headers.HttpHeaders" /> collection without validating the provided information.</summary>
		/// <param name="name">The header to add to the collection.</param>
		/// <param name="values">The values of the header.</param>
		/// <returns>
		///   <see langword="true" /> if the specified header <paramref name="name" /> and <paramref name="values" /> could be added to the collection; otherwise <see langword="false" />.</returns>
		public bool TryAddWithoutValidation(string name, IEnumerable<string> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			HeaderInfo headerInfo;
			if (!this.TryCheckName(name, out headerInfo))
			{
				return false;
			}
			this.AddInternal(name, values, null, true);
			return true;
		}

		private HeaderInfo CheckName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}
			Parser.Token.Check(name);
			HeaderInfo headerInfo;
			if (!HttpHeaders.known_headers.TryGetValue(name, out headerInfo) || (headerInfo.HeaderKind & this.HeaderKind) != HttpHeaderKind.None)
			{
				return headerInfo;
			}
			if (this.HeaderKind != HttpHeaderKind.None && ((this.HeaderKind | headerInfo.HeaderKind) & HttpHeaderKind.Content) != HttpHeaderKind.None)
			{
				throw new InvalidOperationException(name);
			}
			return null;
		}

		private bool TryCheckName(string name, out HeaderInfo headerInfo)
		{
			if (!Parser.Token.TryCheck(name))
			{
				headerInfo = null;
				return false;
			}
			return !HttpHeaders.known_headers.TryGetValue(name, out headerInfo) || (headerInfo.HeaderKind & this.HeaderKind) != HttpHeaderKind.None || this.HeaderKind == HttpHeaderKind.None || ((this.HeaderKind | headerInfo.HeaderKind) & HttpHeaderKind.Content) == HttpHeaderKind.None;
		}

		/// <summary>Removes all headers from the <see cref="T:System.Net.Http.Headers.HttpHeaders" /> collection.</summary>
		public void Clear()
		{
			this.connectionclose = null;
			this.transferEncodingChunked = null;
			this.headers.Clear();
		}

		/// <summary>Returns if  a specific header exists in the <see cref="T:System.Net.Http.Headers.HttpHeaders" /> collection.</summary>
		/// <param name="name">The specific header.</param>
		/// <returns>
		///   <see langword="true" /> is the specified header exists in the collection; otherwise <see langword="false" />.</returns>
		public bool Contains(string name)
		{
			this.CheckName(name);
			return this.headers.ContainsKey(name);
		}

		/// <summary>Returns an enumerator that can iterate through the <see cref="T:System.Net.Http.Headers.HttpHeaders" /> instance.</summary>
		/// <returns>An enumerator for the <see cref="T:System.Net.Http.Headers.HttpHeaders" />.</returns>
		public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
		{
			foreach (KeyValuePair<string, HttpHeaders.HeaderBucket> keyValuePair in this.headers)
			{
				HttpHeaders.HeaderBucket bucket = this.headers[keyValuePair.Key];
				HeaderInfo headerInfo;
				HttpHeaders.known_headers.TryGetValue(keyValuePair.Key, out headerInfo);
				List<string> allHeaderValues = this.GetAllHeaderValues(bucket, headerInfo);
				if (allHeaderValues != null)
				{
					yield return new KeyValuePair<string, IEnumerable<string>>(keyValuePair.Key, allHeaderValues);
				}
			}
			Dictionary<string, HttpHeaders.HeaderBucket>.Enumerator enumerator = default(Dictionary<string, HttpHeaders.HeaderBucket>.Enumerator);
			yield break;
			yield break;
		}

		/// <summary>Gets an enumerator that can iterate through a <see cref="T:System.Net.Http.Headers.HttpHeaders" />.</summary>
		/// <returns>An instance of an implementation of an <see cref="T:System.Collections.IEnumerator" /> that can iterate through a <see cref="T:System.Net.Http.Headers.HttpHeaders" />.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>Returns all header values for a specified header stored in the <see cref="T:System.Net.Http.Headers.HttpHeaders" /> collection.</summary>
		/// <param name="name">The specified header to return values for.</param>
		/// <returns>An array of header strings.</returns>
		/// <exception cref="T:System.InvalidOperationException">The header cannot be found.</exception>
		public IEnumerable<string> GetValues(string name)
		{
			this.CheckName(name);
			IEnumerable<string> result;
			if (!this.TryGetValues(name, out result))
			{
				throw new InvalidOperationException();
			}
			return result;
		}

		/// <summary>Removes the specified header from the <see cref="T:System.Net.Http.Headers.HttpHeaders" /> collection.</summary>
		/// <param name="name">The name of the header to remove from the collection.</param>
		/// <returns>Returns <see cref="T:System.Boolean" />.</returns>
		public bool Remove(string name)
		{
			this.CheckName(name);
			return this.headers.Remove(name);
		}

		/// <summary>Return if a specified header and specified values are stored in the <see cref="T:System.Net.Http.Headers.HttpHeaders" /> collection.</summary>
		/// <param name="name">The specified header.</param>
		/// <param name="values">The specified header values.</param>
		/// <returns>
		///   <see langword="true" /> is the specified header <paramref name="name" /> and <see langword="values" /> are stored in the collection; otherwise <see langword="false" />.</returns>
		public bool TryGetValues(string name, out IEnumerable<string> values)
		{
			HeaderInfo headerInfo;
			if (!this.TryCheckName(name, out headerInfo))
			{
				values = null;
				return false;
			}
			HttpHeaders.HeaderBucket bucket;
			if (!this.headers.TryGetValue(name, out bucket))
			{
				values = null;
				return false;
			}
			values = this.GetAllHeaderValues(bucket, headerInfo);
			return true;
		}

		internal static string GetSingleHeaderString(string key, IEnumerable<string> values)
		{
			string text = ",";
			HeaderInfo headerInfo;
			if (HttpHeaders.known_headers.TryGetValue(key, out headerInfo) && headerInfo.AllowsMany)
			{
				text = headerInfo.Separator;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (string value in values)
			{
				if (!flag)
				{
					stringBuilder.Append(text);
					if (text != " ")
					{
						stringBuilder.Append(" ");
					}
				}
				stringBuilder.Append(value);
				flag = false;
			}
			if (flag)
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.HttpHeaders" /> object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in this)
			{
				stringBuilder.Append(keyValuePair.Key);
				stringBuilder.Append(": ");
				stringBuilder.Append(HttpHeaders.GetSingleHeaderString(keyValuePair.Key, keyValuePair.Value));
				stringBuilder.Append("\r\n");
			}
			return stringBuilder.ToString();
		}

		internal void AddOrRemove(string name, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this.Remove(name);
				return;
			}
			this.SetValue<string>(name, value, null);
		}

		internal void AddOrRemove<T>(string name, T value, Func<object, string> converter = null) where T : class
		{
			if (value == null)
			{
				this.Remove(name);
				return;
			}
			this.SetValue<T>(name, value, converter);
		}

		internal void AddOrRemove<T>(string name, T? value) where T : struct
		{
			this.AddOrRemove<T>(name, value, null);
		}

		internal void AddOrRemove<T>(string name, T? value, Func<object, string> converter) where T : struct
		{
			if (value == null)
			{
				this.Remove(name);
				return;
			}
			this.SetValue<T?>(name, value, converter);
		}

		private List<string> GetAllHeaderValues(HttpHeaders.HeaderBucket bucket, HeaderInfo headerInfo)
		{
			List<string> list = null;
			if (headerInfo != null && headerInfo.AllowsMany)
			{
				list = headerInfo.ToStringCollection(bucket.Parsed);
			}
			else if (bucket.Parsed != null)
			{
				string text = bucket.ParsedToString();
				if (!string.IsNullOrEmpty(text))
				{
					list = new List<string>();
					list.Add(text);
				}
			}
			if (bucket.HasStringValues)
			{
				if (list == null)
				{
					list = new List<string>();
				}
				list.AddRange(bucket.Values);
			}
			return list;
		}

		internal static HttpHeaderKind GetKnownHeaderKind(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}
			HeaderInfo headerInfo;
			if (HttpHeaders.known_headers.TryGetValue(name, out headerInfo))
			{
				return headerInfo.HeaderKind;
			}
			return HttpHeaderKind.None;
		}

		internal T GetValue<T>(string name)
		{
			HttpHeaders.HeaderBucket headerBucket;
			if (!this.headers.TryGetValue(name, out headerBucket))
			{
				return default(T);
			}
			if (headerBucket.HasStringValues)
			{
				object parsed;
				if (!HttpHeaders.known_headers[name].TryParse(headerBucket.Values[0], out parsed))
				{
					if (!(typeof(T) == typeof(string)))
					{
						return default(T);
					}
					return (T)((object)headerBucket.Values[0]);
				}
				else
				{
					headerBucket.Parsed = parsed;
					headerBucket.Values = null;
				}
			}
			return (T)((object)headerBucket.Parsed);
		}

		internal HttpHeaderValueCollection<T> GetValues<T>(string name) where T : class
		{
			HttpHeaders.HeaderBucket headerBucket;
			if (!this.headers.TryGetValue(name, out headerBucket))
			{
				HeaderInfo headerInfo = HttpHeaders.known_headers[name];
				headerBucket = new HttpHeaders.HeaderBucket(new HttpHeaderValueCollection<T>(this, headerInfo), headerInfo.CustomToString);
				this.headers.Add(name, headerBucket);
			}
			HttpHeaderValueCollection<T> httpHeaderValueCollection = (HttpHeaderValueCollection<T>)headerBucket.Parsed;
			if (headerBucket.HasStringValues)
			{
				HeaderInfo headerInfo2 = HttpHeaders.known_headers[name];
				if (httpHeaderValueCollection == null)
				{
					httpHeaderValueCollection = (headerBucket.Parsed = new HttpHeaderValueCollection<T>(this, headerInfo2));
				}
				for (int i = 0; i < headerBucket.Values.Count; i++)
				{
					string text = headerBucket.Values[i];
					object value;
					if (!headerInfo2.TryParse(text, out value))
					{
						httpHeaderValueCollection.AddInvalidValue(text);
					}
					else
					{
						headerInfo2.AddToCollection(httpHeaderValueCollection, value);
					}
				}
				headerBucket.Values.Clear();
			}
			return httpHeaderValueCollection;
		}

		internal void SetValue<T>(string name, T value, Func<object, string> toStringConverter = null)
		{
			this.headers[name] = new HttpHeaders.HeaderBucket(value, toStringConverter);
		}

		private static readonly Dictionary<string, HeaderInfo> known_headers;

		private readonly Dictionary<string, HttpHeaders.HeaderBucket> headers;

		private readonly HttpHeaderKind HeaderKind;

		internal bool? connectionclose;

		internal bool? transferEncodingChunked;

		private class HeaderBucket
		{
			public HeaderBucket(object parsed, Func<object, string> converter)
			{
				this.Parsed = parsed;
				this.CustomToString = converter;
			}

			public bool HasStringValues
			{
				get
				{
					return this.values != null && this.values.Count > 0;
				}
			}

			public List<string> Values
			{
				get
				{
					List<string> result;
					if ((result = this.values) == null)
					{
						result = (this.values = new List<string>());
					}
					return result;
				}
				set
				{
					this.values = value;
				}
			}

			public string ParsedToString()
			{
				if (this.Parsed == null)
				{
					return null;
				}
				if (this.CustomToString != null)
				{
					return this.CustomToString(this.Parsed);
				}
				return this.Parsed.ToString();
			}

			public object Parsed;

			private List<string> values;

			public readonly Func<object, string> CustomToString;
		}
	}
}
