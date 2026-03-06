using System;

namespace System.Net.Http.Headers
{
	/// <summary>Represents the collection of Response Headers as defined in RFC 2616.</summary>
	public sealed class HttpResponseHeaders : HttpHeaders
	{
		internal HttpResponseHeaders() : base(HttpHeaderKind.Response)
		{
		}

		/// <summary>Gets the value of the <see langword="Accept-Ranges" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Accept-Ranges" /> header for an HTTP response.</returns>
		public HttpHeaderValueCollection<string> AcceptRanges
		{
			get
			{
				return base.GetValues<string>("Accept-Ranges");
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Age" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Age" /> header for an HTTP response.</returns>
		public TimeSpan? Age
		{
			get
			{
				return base.GetValue<TimeSpan?>("Age");
			}
			set
			{
				base.AddOrRemove<TimeSpan>("Age", value, (object l) => ((long)((TimeSpan)l).TotalSeconds).ToString());
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Cache-Control" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Cache-Control" /> header for an HTTP response.</returns>
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

		/// <summary>Gets the value of the <see langword="Connection" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Connection" /> header for an HTTP response.</returns>
		public HttpHeaderValueCollection<string> Connection
		{
			get
			{
				return base.GetValues<string>("Connection");
			}
		}

		/// <summary>Gets or sets a value that indicates if the <see langword="Connection" /> header for an HTTP response contains Close.</summary>
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

		/// <summary>Gets or sets the value of the <see langword="Date" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Date" /> header for an HTTP response.</returns>
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

		/// <summary>Gets or sets the value of the <see langword="ETag" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="ETag" /> header for an HTTP response.</returns>
		public EntityTagHeaderValue ETag
		{
			get
			{
				return base.GetValue<EntityTagHeaderValue>("ETag");
			}
			set
			{
				base.AddOrRemove<EntityTagHeaderValue>("ETag", value, null);
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Location" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Location" /> header for an HTTP response.</returns>
		public Uri Location
		{
			get
			{
				return base.GetValue<Uri>("Location");
			}
			set
			{
				base.AddOrRemove<Uri>("Location", value, null);
			}
		}

		/// <summary>Gets the value of the <see langword="Pragma" /> header for an HTTP response.</summary>
		/// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.  
		///  The value of the <see langword="Pragma" /> header for an HTTP response.</returns>
		public HttpHeaderValueCollection<NameValueHeaderValue> Pragma
		{
			get
			{
				return base.GetValues<NameValueHeaderValue>("Pragma");
			}
		}

		/// <summary>Gets the value of the <see langword="Proxy-Authenticate" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Proxy-Authenticate" /> header for an HTTP response.</returns>
		public HttpHeaderValueCollection<AuthenticationHeaderValue> ProxyAuthenticate
		{
			get
			{
				return base.GetValues<AuthenticationHeaderValue>("Proxy-Authenticate");
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Retry-After" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Retry-After" /> header for an HTTP response.</returns>
		public RetryConditionHeaderValue RetryAfter
		{
			get
			{
				return base.GetValue<RetryConditionHeaderValue>("Retry-After");
			}
			set
			{
				base.AddOrRemove<RetryConditionHeaderValue>("Retry-After", value, null);
			}
		}

		/// <summary>Gets the value of the <see langword="Server" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Server" /> header for an HTTP response.</returns>
		public HttpHeaderValueCollection<ProductInfoHeaderValue> Server
		{
			get
			{
				return base.GetValues<ProductInfoHeaderValue>("Server");
			}
		}

		/// <summary>Gets the value of the <see langword="Trailer" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Trailer" /> header for an HTTP response.</returns>
		public HttpHeaderValueCollection<string> Trailer
		{
			get
			{
				return base.GetValues<string>("Trailer");
			}
		}

		/// <summary>Gets the value of the <see langword="Transfer-Encoding" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Transfer-Encoding" /> header for an HTTP response.</returns>
		public HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncoding
		{
			get
			{
				return base.GetValues<TransferCodingHeaderValue>("Transfer-Encoding");
			}
		}

		/// <summary>Gets or sets a value that indicates if the <see langword="Transfer-Encoding" /> header for an HTTP response contains chunked.</summary>
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
				if (this.TransferEncoding.Find((TransferCodingHeaderValue l) => StringComparer.OrdinalIgnoreCase.Equals(l.Value, "chunked")) == null)
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

		/// <summary>Gets the value of the <see langword="Upgrade" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Upgrade" /> header for an HTTP response.</returns>
		public HttpHeaderValueCollection<ProductHeaderValue> Upgrade
		{
			get
			{
				return base.GetValues<ProductHeaderValue>("Upgrade");
			}
		}

		/// <summary>Gets the value of the <see langword="Vary" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Vary" /> header for an HTTP response.</returns>
		public HttpHeaderValueCollection<string> Vary
		{
			get
			{
				return base.GetValues<string>("Vary");
			}
		}

		/// <summary>Gets the value of the <see langword="Via" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Via" /> header for an HTTP response.</returns>
		public HttpHeaderValueCollection<ViaHeaderValue> Via
		{
			get
			{
				return base.GetValues<ViaHeaderValue>("Via");
			}
		}

		/// <summary>Gets the value of the <see langword="Warning" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="Warning" /> header for an HTTP response.</returns>
		public HttpHeaderValueCollection<WarningHeaderValue> Warning
		{
			get
			{
				return base.GetValues<WarningHeaderValue>("Warning");
			}
		}

		/// <summary>Gets the value of the <see langword="WWW-Authenticate" /> header for an HTTP response.</summary>
		/// <returns>The value of the <see langword="WWW-Authenticate" /> header for an HTTP response.</returns>
		public HttpHeaderValueCollection<AuthenticationHeaderValue> WwwAuthenticate
		{
			get
			{
				return base.GetValues<AuthenticationHeaderValue>("WWW-Authenticate");
			}
		}
	}
}
