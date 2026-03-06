using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.Net.Http
{
	/// <summary>Provides a collection of <see cref="T:System.Net.Http.HttpContent" /> objects that get serialized using the multipart/* content type specification.</summary>
	public class MultipartContent : HttpContent, IEnumerable<HttpContent>, IEnumerable
	{
		/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.MultipartContent" /> class.</summary>
		public MultipartContent() : this("mixed")
		{
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.MultipartContent" /> class.</summary>
		/// <param name="subtype">The subtype of the multipart content.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="subtype" /> was <see langword="null" /> or contains only white space characters.</exception>
		public MultipartContent(string subtype) : this(subtype, Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture))
		{
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Net.Http.MultipartContent" /> class.</summary>
		/// <param name="subtype">The subtype of the multipart content.</param>
		/// <param name="boundary">The boundary string for the multipart content.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="subtype" /> was <see langword="null" /> or an empty string.  
		///  The <paramref name="boundary" /> was <see langword="null" /> or contains only white space characters.  
		///  -or-  
		///  The <paramref name="boundary" /> ends with a space character.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The length of the <paramref name="boundary" /> was greater than 70.</exception>
		public MultipartContent(string subtype, string boundary)
		{
			if (string.IsNullOrWhiteSpace(subtype))
			{
				throw new ArgumentException("boundary");
			}
			if (string.IsNullOrWhiteSpace(boundary))
			{
				throw new ArgumentException("boundary");
			}
			if (boundary.Length > 70)
			{
				throw new ArgumentOutOfRangeException("boundary");
			}
			if (boundary.Last<char>() == ' ' || !MultipartContent.IsValidRFC2049(boundary))
			{
				throw new ArgumentException("boundary");
			}
			this.boundary = boundary;
			this.nested_content = new List<HttpContent>(2);
			base.Headers.ContentType = new MediaTypeHeaderValue("multipart/" + subtype)
			{
				Parameters = 
				{
					new NameValueHeaderValue("boundary", "\"" + boundary + "\"")
				}
			};
		}

		private static bool IsValidRFC2049(string s)
		{
			foreach (char c in s)
			{
				if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && (c < '0' || c > '9'))
				{
					if (c <= ':')
					{
						switch (c)
						{
						case '\'':
						case '(':
						case ')':
						case '+':
						case ',':
						case '-':
						case '.':
						case '/':
							goto IL_71;
						case '*':
							break;
						default:
							if (c == ':')
							{
								goto IL_71;
							}
							break;
						}
					}
					else if (c == '=' || c == '?')
					{
						goto IL_71;
					}
					return false;
				}
				IL_71:;
			}
			return true;
		}

		/// <summary>Add multipart HTTP content to a collection of <see cref="T:System.Net.Http.HttpContent" /> objects that get serialized using the multipart/* content type specification.</summary>
		/// <param name="content">The HTTP content to add to the collection.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> was <see langword="null" />.</exception>
		public virtual void Add(HttpContent content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (this.nested_content == null)
			{
				this.nested_content = new List<HttpContent>();
			}
			this.nested_content.Add(content);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Http.MultipartContent" /> and optionally disposes of the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to releases only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (HttpContent httpContent in this.nested_content)
				{
					httpContent.Dispose();
				}
				this.nested_content = null;
			}
			base.Dispose(disposing);
		}

		/// <summary>Serialize the multipart HTTP content to a stream as an asynchronous operation.</summary>
		/// <param name="stream">The target stream.</param>
		/// <param name="context">Information about the transport (channel binding token, for example). This parameter may be <see langword="null" />.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			MultipartContent.<SerializeToStreamAsync>d__8 <SerializeToStreamAsync>d__;
			<SerializeToStreamAsync>d__.<>4__this = this;
			<SerializeToStreamAsync>d__.stream = stream;
			<SerializeToStreamAsync>d__.context = context;
			<SerializeToStreamAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SerializeToStreamAsync>d__.<>1__state = -1;
			<SerializeToStreamAsync>d__.<>t__builder.Start<MultipartContent.<SerializeToStreamAsync>d__8>(ref <SerializeToStreamAsync>d__);
			return <SerializeToStreamAsync>d__.<>t__builder.Task;
		}

		/// <summary>Determines whether the HTTP multipart content has a valid length in bytes.</summary>
		/// <param name="length">The length in bytes of the HHTP content.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="length" /> is a valid length; otherwise, <see langword="false" />.</returns>
		protected internal override bool TryComputeLength(out long length)
		{
			length = (long)(12 + 2 * this.boundary.Length);
			for (int i = 0; i < this.nested_content.Count; i++)
			{
				HttpContent httpContent = this.nested_content[i];
				foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in httpContent.Headers)
				{
					length += (long)keyValuePair.Key.Length;
					length += 4L;
					foreach (string text in keyValuePair.Value)
					{
						length += (long)text.Length;
					}
				}
				long num;
				if (!httpContent.TryComputeLength(out num))
				{
					return false;
				}
				length += 2L;
				length += num;
				if (i != this.nested_content.Count - 1)
				{
					length += 6L;
					length += (long)this.boundary.Length;
				}
			}
			return true;
		}

		/// <summary>Returns an enumerator that iterates through the collection of <see cref="T:System.Net.Http.HttpContent" /> objects that get serialized using the multipart/* content type specification.</summary>
		/// <returns>An object that can be used to iterate through the collection.</returns>
		public IEnumerator<HttpContent> GetEnumerator()
		{
			return this.nested_content.GetEnumerator();
		}

		/// <summary>The explicit implementation of the <see cref="M:System.Net.Http.MultipartContent.GetEnumerator" /> method.</summary>
		/// <returns>An object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.nested_content.GetEnumerator();
		}

		private List<HttpContent> nested_content;

		private readonly string boundary;
	}
}
