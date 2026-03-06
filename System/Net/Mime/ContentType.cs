using System;
using System.Collections.Specialized;
using System.Net.Mail;
using System.Text;

namespace System.Net.Mime
{
	/// <summary>Represents a MIME protocol Content-Type header.</summary>
	public class ContentType
	{
		/// <summary>Initializes a new default instance of the <see cref="T:System.Net.Mime.ContentType" /> class.</summary>
		public ContentType() : this("application/octet-stream")
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Mime.ContentType" /> class using the specified string.</summary>
		/// <param name="contentType">A <see cref="T:System.String" />, for example, <c>"text/plain; charset=us-ascii"</c>, that contains the MIME media type, subtype, and optional parameters.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="contentType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="contentType" /> is <see cref="F:System.String.Empty" /> ("").</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="contentType" /> is in a form that cannot be parsed.</exception>
		public ContentType(string contentType)
		{
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType == string.Empty)
			{
				throw new ArgumentException(SR.Format("The parameter '{0}' cannot be an empty string.", "contentType"), "contentType");
			}
			this._isChanged = true;
			this._type = contentType;
			this.ParseValue();
		}

		/// <summary>Gets or sets the value of the boundary parameter included in the Content-Type header represented by this instance.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the value associated with the boundary parameter.</returns>
		public string Boundary
		{
			get
			{
				return this.Parameters["boundary"];
			}
			set
			{
				if (value == null || value == string.Empty)
				{
					this.Parameters.Remove("boundary");
					return;
				}
				this.Parameters["boundary"] = value;
			}
		}

		/// <summary>Gets or sets the value of the charset parameter included in the Content-Type header represented by this instance.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the value associated with the charset parameter.</returns>
		public string CharSet
		{
			get
			{
				return this.Parameters["charset"];
			}
			set
			{
				if (value == null || value == string.Empty)
				{
					this.Parameters.Remove("charset");
					return;
				}
				this.Parameters["charset"] = value;
			}
		}

		/// <summary>Gets or sets the media type value included in the Content-Type header represented by this instance.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the media type and subtype value. This value does not include the semicolon (;) separator that follows the subtype.</returns>
		/// <exception cref="T:System.ArgumentNullException">The value specified for a set operation is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The value specified for a set operation is <see cref="F:System.String.Empty" /> ("").</exception>
		/// <exception cref="T:System.FormatException">The value specified for a set operation is in a form that cannot be parsed.</exception>
		public string MediaType
		{
			get
			{
				return this._mediaType + "/" + this._subType;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value == string.Empty)
				{
					throw new ArgumentException("This property cannot be set to an empty string.", "value");
				}
				int num = 0;
				this._mediaType = MailBnfHelper.ReadToken(value, ref num, null);
				if (this._mediaType.Length == 0 || num >= value.Length || value[num++] != '/')
				{
					throw new FormatException("The specified media type is invalid.");
				}
				this._subType = MailBnfHelper.ReadToken(value, ref num, null);
				if (this._subType.Length == 0 || num < value.Length)
				{
					throw new FormatException("The specified media type is invalid.");
				}
				this._isChanged = true;
				this._isPersisted = false;
			}
		}

		/// <summary>Gets or sets the value of the name parameter included in the Content-Type header represented by this instance.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the value associated with the name parameter.</returns>
		public string Name
		{
			get
			{
				string text = this.Parameters["name"];
				if (MimeBasePart.DecodeEncoding(text) != null)
				{
					text = MimeBasePart.DecodeHeaderValue(text);
				}
				return text;
			}
			set
			{
				if (value == null || value == string.Empty)
				{
					this.Parameters.Remove("name");
					return;
				}
				this.Parameters["name"] = value;
			}
		}

		/// <summary>Gets the dictionary that contains the parameters included in the Content-Type header represented by this instance.</summary>
		/// <returns>A writable <see cref="T:System.Collections.Specialized.StringDictionary" /> that contains name and value pairs.</returns>
		public StringDictionary Parameters
		{
			get
			{
				return this._parameters;
			}
		}

		internal void Set(string contentType, HeaderCollection headers)
		{
			this._type = contentType;
			this.ParseValue();
			headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentType), this.ToString());
			this._isPersisted = true;
		}

		internal void PersistIfNeeded(HeaderCollection headers, bool forcePersist)
		{
			if (this.IsChanged || !this._isPersisted || forcePersist)
			{
				headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentType), this.ToString());
				this._isPersisted = true;
			}
		}

		internal bool IsChanged
		{
			get
			{
				return this._isChanged || (this._parameters != null && this._parameters.IsChanged);
			}
		}

		/// <summary>Returns a string representation of this <see cref="T:System.Net.Mime.ContentType" /> object.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the current settings for this <see cref="T:System.Net.Mime.ContentType" />.</returns>
		public override string ToString()
		{
			if (this._type == null || this.IsChanged)
			{
				this._type = this.Encode(false);
				this._isChanged = false;
				this._parameters.IsChanged = false;
				this._isPersisted = false;
			}
			return this._type;
		}

		internal string Encode(bool allowUnicode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this._mediaType);
			stringBuilder.Append('/');
			stringBuilder.Append(this._subType);
			foreach (object obj in this.Parameters.Keys)
			{
				string text = (string)obj;
				stringBuilder.Append("; ");
				ContentType.EncodeToBuffer(text, stringBuilder, allowUnicode);
				stringBuilder.Append('=');
				ContentType.EncodeToBuffer(this._parameters[text], stringBuilder, allowUnicode);
			}
			return stringBuilder.ToString();
		}

		private static void EncodeToBuffer(string value, StringBuilder builder, bool allowUnicode)
		{
			Encoding encoding = MimeBasePart.DecodeEncoding(value);
			if (encoding != null)
			{
				builder.Append('"').Append(value).Append('"');
				return;
			}
			if ((allowUnicode && !MailBnfHelper.HasCROrLF(value)) || MimeBasePart.IsAscii(value, false))
			{
				MailBnfHelper.GetTokenOrQuotedString(value, builder, allowUnicode);
				return;
			}
			encoding = Encoding.GetEncoding("utf-8");
			builder.Append('"').Append(MimeBasePart.EncodeHeaderValue(value, encoding, MimeBasePart.ShouldUseBase64Encoding(encoding))).Append('"');
		}

		/// <summary>Determines whether the content-type header of the specified <see cref="T:System.Net.Mime.ContentType" /> object is equal to the content-type header of this object.</summary>
		/// <param name="rparam">The <see cref="T:System.Net.Mime.ContentType" /> object to compare with this object.</param>
		/// <returns>
		///   <see langword="true" /> if the content-type headers are the same; otherwise <see langword="false" />.</returns>
		public override bool Equals(object rparam)
		{
			return rparam != null && string.Equals(this.ToString(), rparam.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>Determines the hash code of the specified <see cref="T:System.Net.Mime.ContentType" /> object</summary>
		/// <returns>An integer hash value.</returns>
		public override int GetHashCode()
		{
			return this.ToString().ToLowerInvariant().GetHashCode();
		}

		private void ParseValue()
		{
			int num = 0;
			Exception ex = null;
			try
			{
				this._mediaType = MailBnfHelper.ReadToken(this._type, ref num, null);
				if (this._mediaType == null || this._mediaType.Length == 0 || num >= this._type.Length || this._type[num++] != '/')
				{
					ex = new FormatException("The specified content type is invalid.");
				}
				if (ex == null)
				{
					this._subType = MailBnfHelper.ReadToken(this._type, ref num, null);
					if (this._subType == null || this._subType.Length == 0)
					{
						ex = new FormatException("The specified content type is invalid.");
					}
				}
				if (ex == null)
				{
					while (MailBnfHelper.SkipCFWS(this._type, ref num))
					{
						if (this._type[num++] != ';')
						{
							ex = new FormatException("The specified content type is invalid.");
							break;
						}
						if (!MailBnfHelper.SkipCFWS(this._type, ref num))
						{
							break;
						}
						string text = MailBnfHelper.ReadParameterAttribute(this._type, ref num, null);
						if (text == null || text.Length == 0)
						{
							ex = new FormatException("The specified content type is invalid.");
							break;
						}
						if (num >= this._type.Length || this._type[num++] != '=')
						{
							ex = new FormatException("The specified content type is invalid.");
							break;
						}
						if (!MailBnfHelper.SkipCFWS(this._type, ref num))
						{
							ex = new FormatException("The specified content type is invalid.");
							break;
						}
						string text2 = (this._type[num] == '"') ? MailBnfHelper.ReadQuotedString(this._type, ref num, null) : MailBnfHelper.ReadToken(this._type, ref num, null);
						if (text2 == null)
						{
							ex = new FormatException("The specified content type is invalid.");
							break;
						}
						this._parameters.Add(text, text2);
					}
				}
				this._parameters.IsChanged = false;
			}
			catch (FormatException)
			{
				throw new FormatException("The specified content type is invalid.");
			}
			if (ex != null)
			{
				throw new FormatException("The specified content type is invalid.");
			}
		}

		private readonly TrackingStringDictionary _parameters = new TrackingStringDictionary();

		private string _mediaType;

		private string _subType;

		private bool _isChanged;

		private string _type;

		private bool _isPersisted;

		internal const string Default = "application/octet-stream";
	}
}
