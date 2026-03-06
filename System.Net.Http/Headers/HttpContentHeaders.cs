using System;
using System.Collections.Generic;
using Unity;

namespace System.Net.Http.Headers
{
	/// <summary>Represents the collection of Content Headers as defined in RFC 2616.</summary>
	public sealed class HttpContentHeaders : HttpHeaders
	{
		internal HttpContentHeaders(HttpContent content) : base(HttpHeaderKind.Content)
		{
			this.content = content;
		}

		/// <summary>Gets the value of the <see langword="Allow" /> content header on an HTTP response.</summary>
		/// <returns>The value of the <see langword="Allow" /> header on an HTTP response.</returns>
		public ICollection<string> Allow
		{
			get
			{
				return base.GetValues<string>("Allow");
			}
		}

		/// <summary>Gets the value of the <see langword="Content-Encoding" /> content header on an HTTP response.</summary>
		/// <returns>The value of the <see langword="Content-Encoding" /> content header on an HTTP response.</returns>
		public ICollection<string> ContentEncoding
		{
			get
			{
				return base.GetValues<string>("Content-Encoding");
			}
		}

		/// <summary>Gets the value of the <see langword="Content-Disposition" /> content header on an HTTP response.</summary>
		/// <returns>The value of the <see langword="Content-Disposition" /> content header on an HTTP response.</returns>
		public ContentDispositionHeaderValue ContentDisposition
		{
			get
			{
				return base.GetValue<ContentDispositionHeaderValue>("Content-Disposition");
			}
			set
			{
				base.AddOrRemove<ContentDispositionHeaderValue>("Content-Disposition", value, null);
			}
		}

		/// <summary>Gets the value of the <see langword="Content-Language" /> content header on an HTTP response.</summary>
		/// <returns>The value of the <see langword="Content-Language" /> content header on an HTTP response.</returns>
		public ICollection<string> ContentLanguage
		{
			get
			{
				return base.GetValues<string>("Content-Language");
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Content-Length" /> content header on an HTTP response.</summary>
		/// <returns>The value of the <see langword="Content-Length" /> content header on an HTTP response.</returns>
		public long? ContentLength
		{
			get
			{
				long? result = base.GetValue<long?>("Content-Length");
				if (result != null)
				{
					return result;
				}
				result = this.content.LoadedBufferLength;
				if (result != null)
				{
					return result;
				}
				long value;
				if (this.content.TryComputeLength(out value))
				{
					base.SetValue<long>("Content-Length", value, null);
					return new long?(value);
				}
				return null;
			}
			set
			{
				base.AddOrRemove<long>("Content-Length", value);
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Content-Location" /> content header on an HTTP response.</summary>
		/// <returns>The value of the <see langword="Content-Location" /> content header on an HTTP response.</returns>
		public Uri ContentLocation
		{
			get
			{
				return base.GetValue<Uri>("Content-Location");
			}
			set
			{
				base.AddOrRemove<Uri>("Content-Location", value, null);
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Content-MD5" /> content header on an HTTP response.</summary>
		/// <returns>The value of the <see langword="Content-MD5" /> content header on an HTTP response.</returns>
		public byte[] ContentMD5
		{
			get
			{
				return base.GetValue<byte[]>("Content-MD5");
			}
			set
			{
				base.AddOrRemove<byte[]>("Content-MD5", value, Parser.MD5.ToString);
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Content-Range" /> content header on an HTTP response.</summary>
		/// <returns>The value of the <see langword="Content-Range" /> content header on an HTTP response.</returns>
		public ContentRangeHeaderValue ContentRange
		{
			get
			{
				return base.GetValue<ContentRangeHeaderValue>("Content-Range");
			}
			set
			{
				base.AddOrRemove<ContentRangeHeaderValue>("Content-Range", value, null);
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Content-Type" /> content header on an HTTP response.</summary>
		/// <returns>The value of the <see langword="Content-Type" /> content header on an HTTP response.</returns>
		public MediaTypeHeaderValue ContentType
		{
			get
			{
				return base.GetValue<MediaTypeHeaderValue>("Content-Type");
			}
			set
			{
				base.AddOrRemove<MediaTypeHeaderValue>("Content-Type", value, null);
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Expires" /> content header on an HTTP response.</summary>
		/// <returns>The value of the <see langword="Expires" /> content header on an HTTP response.</returns>
		public DateTimeOffset? Expires
		{
			get
			{
				return base.GetValue<DateTimeOffset?>("Expires");
			}
			set
			{
				base.AddOrRemove<DateTimeOffset>("Expires", value, Parser.DateTime.ToString);
			}
		}

		/// <summary>Gets or sets the value of the <see langword="Last-Modified" /> content header on an HTTP response.</summary>
		/// <returns>The value of the <see langword="Last-Modified" /> content header on an HTTP response.</returns>
		public DateTimeOffset? LastModified
		{
			get
			{
				return base.GetValue<DateTimeOffset?>("Last-Modified");
			}
			set
			{
				base.AddOrRemove<DateTimeOffset>("Last-Modified", value, Parser.DateTime.ToString);
			}
		}

		internal HttpContentHeaders()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private readonly HttpContent content;
	}
}
