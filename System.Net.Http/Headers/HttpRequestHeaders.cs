using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	/// <summary>Represents the collection of Request Headers as defined in RFC 2616.</summary>
	public sealed class HttpRequestHeaders : HttpHeaders
	{
		internal HttpRequestHeaders() : base(HttpHeaderKind.Request)
		{
		}

		/// <summary>Gets the value of the <see langword="Accept" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Accept" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> Accept
		{
			get
			{
				return base.GetValues<MediaTypeWithQualityHeaderValue>("Accept");
			}
		}

		/// <summary>Gets the value of the <see langword="Accept-Charset" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Accept-Charset" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptCharset
		{
			get
			{
				return base.GetValues<StringWithQualityHeaderValue>("Accept-Charset");
			}
		}

		/// <summary>Gets the value of the <see langword="Accept-Encoding" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Accept-Encoding" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptEncoding
		{
			get
			{
				return base.GetValues<StringWithQualityHeaderValue>("Accept-Encoding");
			}
		}

		/// <summary>Gets the value of the <see langword="Accept-Language" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Accept-Language" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptLanguage
		{
			get
			{
				return base.GetValues<StringWithQualityHeaderValue>("Accept-Language");
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Authorization" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Authorization" /> header for an HTTP request.</returns>
		public AuthenticationHeaderValue Authorization
		{
			get
			{
				return base.GetValue<AuthenticationHeaderValue>("Authorization");
			}
			set
			{
				base.AddOrRemove<AuthenticationHeaderValue>("Authorization", value, null);
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Cache-Control" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Cache-Control" /> header for an HTTP request.</returns>
		public CacheControlHeaderValue CacheControl
		{
			get
			{
				return base.GetValue<CacheControlHeaderValue>("Cache-Control");
			}
			set
			{
				base.AddOrRemove<CacheControlHeaderValue>("Cache-Control", value, null);
			}
		}

		/// <summary>Gets the value of the <see langword="Connection" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Connection" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<string> Connection
		{
			get
			{
				return base.GetValues<string>("Connection");
			}
		}

		/// <summary>Gets or sets a value that indicates if the <see langword="Connection" /> header for an HTTP request contains Close.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see langword="Connection" /> header contains Close, otherwise <see langword="false" />.</returns>
		public bool? ConnectionClose
		{
			get
			{
				bool? connectionclose = this.connectionclose;
				bool flag = true;
				if (!(connectionclose.GetValueOrDefault() == flag & connectionclose != null))
				{
					if (this.Connection.Find((string l) => string.Equals(l, "close", StringComparison.OrdinalIgnoreCase)) == null)
					{
						return this.connectionclose;
					}
				}
				return new bool?(true);
			}
			set
			{
				bool? connectionclose = this.connectionclose;
				bool? flag = value;
				if (connectionclose.GetValueOrDefault() == flag.GetValueOrDefault() & connectionclose != null == (flag != null))
				{
					return;
				}
				this.Connection.Remove("close");
				flag = value;
				bool flag2 = true;
				if (flag.GetValueOrDefault() == flag2 & flag != null)
				{
					this.Connection.Add("close");
				}
				this.connectionclose = value;
			}
		}

		internal bool ConnectionKeepAlive
		{
			get
			{
				return this.Connection.Find((string l) => string.Equals(l, "Keep-Alive", StringComparison.OrdinalIgnoreCase)) != null;
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Date" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Date" /> header for an HTTP request.</returns>
		public DateTimeOffset? Date
		{
			get
			{
				return base.GetValue<DateTimeOffset?>("Date");
			}
			set
			{
				base.AddOrRemove<DateTimeOffset>("Date", value, Parser.DateTime.ToString);
			}
		}

		/// <summary>Gets the value of the <see langword="Expect" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Expect" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<NameValueWithParametersHeaderValue> Expect
		{
			get
			{
				return base.GetValues<NameValueWithParametersHeaderValue>("Expect");
			}
		}

		/// <summary>Gets or sets a value that indicates if the <see langword="Expect" /> header for an HTTP request contains Continue.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see langword="Expect" /> header contains Continue, otherwise <see langword="false" />.</returns>
		public bool? ExpectContinue
		{
			get
			{
				if (this.expectContinue != null)
				{
					return this.expectContinue;
				}
				if (this.TransferEncoding.Find((TransferCodingHeaderValue l) => string.Equals(l.Value, "100-continue", StringComparison.OrdinalIgnoreCase)) == null)
				{
					return null;
				}
				return new bool?(true);
			}
			set
			{
				bool? flag = this.expectContinue;
				bool? flag2 = value;
				if (flag.GetValueOrDefault() == flag2.GetValueOrDefault() & flag != null == (flag2 != null))
				{
					return;
				}
				this.Expect.Remove((NameValueWithParametersHeaderValue l) => l.Name == "100-continue");
				flag2 = value;
				bool flag3 = true;
				if (flag2.GetValueOrDefault() == flag3 & flag2 != null)
				{
					this.Expect.Add(new NameValueWithParametersHeaderValue("100-continue"));
				}
				this.expectContinue = value;
			}
		}

		/// <summary>Gets or sets the value of the <see langword="From" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="From" /> header for an HTTP request.</returns>
		public string From
		{
			get
			{
				return base.GetValue<string>("From");
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && !Parser.EmailAddress.TryParse(value, out value))
				{
					throw new FormatException();
				}
				base.AddOrRemove("From", value);
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Host" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Host" /> header for an HTTP request.</returns>
		public string Host
		{
			get
			{
				return base.GetValue<string>("Host");
			}
			set
			{
				base.AddOrRemove("Host", value);
			}
		}

		/// <summary>Gets the value of the <see langword="If-Match" /> header for an HTTP request.</summary>
		/// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.  
		///  The value of the <see langword="If-Match" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<EntityTagHeaderValue> IfMatch
		{
			get
			{
				return base.GetValues<EntityTagHeaderValue>("If-Match");
			}
		}

		/// <summary>Gets or sets the value of the <see langword="If-Modified-Since" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="If-Modified-Since" /> header for an HTTP request.</returns>
		public DateTimeOffset? IfModifiedSince
		{
			get
			{
				return base.GetValue<DateTimeOffset?>("If-Modified-Since");
			}
			set
			{
				base.AddOrRemove<DateTimeOffset>("If-Modified-Since", value, Parser.DateTime.ToString);
			}
		}

		/// <summary>Gets the value of the <see langword="If-None-Match" /> header for an HTTP request.</summary>
		/// <returns>Gets the value of the <see langword="If-None-Match" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<EntityTagHeaderValue> IfNoneMatch
		{
			get
			{
				return base.GetValues<EntityTagHeaderValue>("If-None-Match");
			}
		}

		/// <summary>Gets or sets the value of the <see langword="If-Range" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="If-Range" /> header for an HTTP request.</returns>
		public RangeConditionHeaderValue IfRange
		{
			get
			{
				return base.GetValue<RangeConditionHeaderValue>("If-Range");
			}
			set
			{
				base.AddOrRemove<RangeConditionHeaderValue>("If-Range", value, null);
			}
		}

		/// <summary>Gets or sets the value of the <see langword="If-Unmodified-Since" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="If-Unmodified-Since" /> header for an HTTP request.</returns>
		public DateTimeOffset? IfUnmodifiedSince
		{
			get
			{
				return base.GetValue<DateTimeOffset?>("If-Unmodified-Since");
			}
			set
			{
				base.AddOrRemove<DateTimeOffset>("If-Unmodified-Since", value, Parser.DateTime.ToString);
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Max-Forwards" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Max-Forwards" /> header for an HTTP request.</returns>
		public int? MaxForwards
		{
			get
			{
				return base.GetValue<int?>("Max-Forwards");
			}
			set
			{
				base.AddOrRemove<int>("Max-Forwards", value);
			}
		}

		/// <summary>Gets the value of the <see langword="Pragma" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Pragma" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<NameValueHeaderValue> Pragma
		{
			get
			{
				return base.GetValues<NameValueHeaderValue>("Pragma");
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Proxy-Authorization" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Proxy-Authorization" /> header for an HTTP request.</returns>
		public AuthenticationHeaderValue ProxyAuthorization
		{
			get
			{
				return base.GetValue<AuthenticationHeaderValue>("Proxy-Authorization");
			}
			set
			{
				base.AddOrRemove<AuthenticationHeaderValue>("Proxy-Authorization", value, null);
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Range" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Range" /> header for an HTTP request.</returns>
		public RangeHeaderValue Range
		{
			get
			{
				return base.GetValue<RangeHeaderValue>("Range");
			}
			set
			{
				base.AddOrRemove<RangeHeaderValue>("Range", value, null);
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Referer" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Referer" /> header for an HTTP request.</returns>
		public Uri Referrer
		{
			get
			{
				return base.GetValue<Uri>("Referer");
			}
			set
			{
				base.AddOrRemove<Uri>("Referer", value, null);
			}
		}

		/// <summary>Gets the value of the <see langword="TE" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="TE" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue> TE
		{
			get
			{
				return base.GetValues<TransferCodingWithQualityHeaderValue>("TE");
			}
		}

		/// <summary>Gets the value of the <see langword="Trailer" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Trailer" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<string> Trailer
		{
			get
			{
				return base.GetValues<string>("Trailer");
			}
		}

		/// <summary>Gets the value of the <see langword="Transfer-Encoding" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Transfer-Encoding" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncoding
		{
			get
			{
				return base.GetValues<TransferCodingHeaderValue>("Transfer-Encoding");
			}
		}

		/// <summary>Gets or sets a value that indicates if the <see langword="Transfer-Encoding" /> header for an HTTP request contains chunked.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see langword="Transfer-Encoding" /> header contains chunked, otherwise <see langword="false" />.</returns>
		public bool? TransferEncodingChunked
		{
			get
			{
				if (this.transferEncodingChunked != null)
				{
					return this.transferEncodingChunked;
				}
				if (this.TransferEncoding.Find((TransferCodingHeaderValue l) => string.Equals(l.Value, "chunked", StringComparison.OrdinalIgnoreCase)) == null)
				{
					return null;
				}
				return new bool?(true);
			}
			set
			{
				bool? flag = value;
				bool? flag2 = this.transferEncodingChunked;
				if (flag.GetValueOrDefault() == flag2.GetValueOrDefault() & flag != null == (flag2 != null))
				{
					return;
				}
				this.TransferEncoding.Remove((TransferCodingHeaderValue l) => l.Value == "chunked");
				flag2 = value;
				bool flag3 = true;
				if (flag2.GetValueOrDefault() == flag3 & flag2 != null)
				{
					this.TransferEncoding.Add(new TransferCodingHeaderValue("chunked"));
				}
				this.transferEncodingChunked = value;
			}
		}

		/// <summary>Gets the value of the <see langword="Upgrade" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Upgrade" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<ProductHeaderValue> Upgrade
		{
			get
			{
				return base.GetValues<ProductHeaderValue>("Upgrade");
			}
		}

		/// <summary>Gets the value of the <see langword="User-Agent" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="User-Agent" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<ProductInfoHeaderValue> UserAgent
		{
			get
			{
				return base.GetValues<ProductInfoHeaderValue>("User-Agent");
			}
		}

		/// <summary>Gets the value of the <see langword="Via" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Via" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<ViaHeaderValue> Via
		{
			get
			{
				return base.GetValues<ViaHeaderValue>("Via");
			}
		}

		/// <summary>Gets the value of the <see langword="Warning" /> header for an HTTP request.</summary>
		/// <returns>The value of the <see langword="Warning" /> header for an HTTP request.</returns>
		public HttpHeaderValueCollection<WarningHeaderValue> Warning
		{
			get
			{
				return base.GetValues<WarningHeaderValue>("Warning");
			}
		}

		internal void AddHeaders(HttpRequestHeaders headers)
		{
			foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in headers)
			{
				base.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);
			}
		}

		private bool? expectContinue;
	}
}
