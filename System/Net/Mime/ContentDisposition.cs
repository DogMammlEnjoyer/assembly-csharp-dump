using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net.Mail;
using System.Text;

namespace System.Net.Mime
{
	/// <summary>Represents a MIME protocol Content-Disposition header.</summary>
	public class ContentDisposition
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Mime.ContentDisposition" /> class with a <see cref="P:System.Net.Mime.ContentDisposition.DispositionType" /> of <see cref="F:System.Net.Mime.DispositionTypeNames.Attachment" />.</summary>
		public ContentDisposition()
		{
			this._isChanged = true;
			this._disposition = (this._dispositionType = "attachment");
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Mime.ContentDisposition" /> class with the specified disposition information.</summary>
		/// <param name="disposition">A <see cref="T:System.Net.Mime.DispositionTypeNames" /> value that contains the disposition.</param>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="disposition" /> is <see langword="null" /> or equal to <see cref="F:System.String.Empty" /> ("").</exception>
		public ContentDisposition(string disposition)
		{
			if (disposition == null)
			{
				throw new ArgumentNullException("disposition");
			}
			this._isChanged = true;
			this._disposition = disposition;
			this.ParseValue();
		}

		internal DateTime GetDateParameter(string parameterName)
		{
			SmtpDateTime smtpDateTime = ((TrackingValidationObjectDictionary)this.Parameters).InternalGet(parameterName) as SmtpDateTime;
			if (smtpDateTime != null)
			{
				return smtpDateTime.Date;
			}
			return DateTime.MinValue;
		}

		/// <summary>Gets or sets the disposition type for an email attachment.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the disposition type. The value is not restricted but is typically one of the <see cref="P:System.Net.Mime.ContentDisposition.DispositionType" /> values.</returns>
		/// <exception cref="T:System.ArgumentNullException">The value specified for a set operation is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The value specified for a set operation is equal to <see cref="F:System.String.Empty" /> ("").</exception>
		public string DispositionType
		{
			get
			{
				return this._dispositionType;
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
				this._isChanged = true;
				this._dispositionType = value;
			}
		}

		/// <summary>Gets the parameters included in the Content-Disposition header represented by this instance.</summary>
		/// <returns>A writable <see cref="T:System.Collections.Specialized.StringDictionary" /> that contains parameter name/value pairs.</returns>
		public StringDictionary Parameters
		{
			get
			{
				TrackingValidationObjectDictionary result;
				if ((result = this._parameters) == null)
				{
					result = (this._parameters = new TrackingValidationObjectDictionary(ContentDisposition.s_validators));
				}
				return result;
			}
		}

		/// <summary>Gets or sets the suggested file name for an email attachment.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the file name.</returns>
		public string FileName
		{
			get
			{
				return this.Parameters["filename"];
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.Parameters.Remove("filename");
					return;
				}
				this.Parameters["filename"] = value;
			}
		}

		/// <summary>Gets or sets the creation date for a file attachment.</summary>
		/// <returns>A <see cref="T:System.DateTime" /> value that indicates the file creation date; otherwise, <see cref="F:System.DateTime.MinValue" /> if no date was specified.</returns>
		public DateTime CreationDate
		{
			get
			{
				return this.GetDateParameter("creation-date");
			}
			set
			{
				SmtpDateTime value2 = new SmtpDateTime(value);
				((TrackingValidationObjectDictionary)this.Parameters).InternalSet("creation-date", value2);
			}
		}

		/// <summary>Gets or sets the modification date for a file attachment.</summary>
		/// <returns>A <see cref="T:System.DateTime" /> value that indicates the file modification date; otherwise, <see cref="F:System.DateTime.MinValue" /> if no date was specified.</returns>
		public DateTime ModificationDate
		{
			get
			{
				return this.GetDateParameter("modification-date");
			}
			set
			{
				SmtpDateTime value2 = new SmtpDateTime(value);
				((TrackingValidationObjectDictionary)this.Parameters).InternalSet("modification-date", value2);
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that determines the disposition type (Inline or Attachment) for an email attachment.</summary>
		/// <returns>
		///   <see langword="true" /> if content in the attachment is presented inline as part of the email body; otherwise, <see langword="false" />.</returns>
		public bool Inline
		{
			get
			{
				return this._dispositionType == "inline";
			}
			set
			{
				this._isChanged = true;
				this._dispositionType = (value ? "inline" : "attachment");
			}
		}

		/// <summary>Gets or sets the read date for a file attachment.</summary>
		/// <returns>A <see cref="T:System.DateTime" /> value that indicates the file read date; otherwise, <see cref="F:System.DateTime.MinValue" /> if no date was specified.</returns>
		public DateTime ReadDate
		{
			get
			{
				return this.GetDateParameter("read-date");
			}
			set
			{
				SmtpDateTime value2 = new SmtpDateTime(value);
				((TrackingValidationObjectDictionary)this.Parameters).InternalSet("read-date", value2);
			}
		}

		/// <summary>Gets or sets the size of a file attachment.</summary>
		/// <returns>A <see cref="T:System.Int32" /> that specifies the number of bytes in the file attachment. The default value is -1, which indicates that the file size is unknown.</returns>
		public long Size
		{
			get
			{
				object obj = ((TrackingValidationObjectDictionary)this.Parameters).InternalGet("size");
				if (obj != null)
				{
					return (long)obj;
				}
				return -1L;
			}
			set
			{
				((TrackingValidationObjectDictionary)this.Parameters).InternalSet("size", value);
			}
		}

		internal void Set(string contentDisposition, HeaderCollection headers)
		{
			this._disposition = contentDisposition;
			this.ParseValue();
			headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentDisposition), this.ToString());
			this._isPersisted = true;
		}

		internal void PersistIfNeeded(HeaderCollection headers, bool forcePersist)
		{
			if (this.IsChanged || !this._isPersisted || forcePersist)
			{
				headers.InternalSet(MailHeaderInfo.GetString(MailHeaderID.ContentDisposition), this.ToString());
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

		/// <summary>Returns a <see cref="T:System.String" /> representation of this instance.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the property values for this instance.</returns>
		public override string ToString()
		{
			if (this._disposition == null || this._isChanged || (this._parameters != null && this._parameters.IsChanged))
			{
				this._disposition = this.Encode(false);
				this._isChanged = false;
				this._parameters.IsChanged = false;
				this._isPersisted = false;
			}
			return this._disposition;
		}

		internal string Encode(bool allowUnicode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this._dispositionType);
			foreach (object obj in this.Parameters.Keys)
			{
				string text = (string)obj;
				stringBuilder.Append("; ");
				ContentDisposition.EncodeToBuffer(text, stringBuilder, allowUnicode);
				stringBuilder.Append('=');
				ContentDisposition.EncodeToBuffer(this._parameters[text], stringBuilder, allowUnicode);
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

		/// <summary>Determines whether the content-disposition header of the specified <see cref="T:System.Net.Mime.ContentDisposition" /> object is equal to the content-disposition header of this object.</summary>
		/// <param name="rparam">The <see cref="T:System.Net.Mime.ContentDisposition" /> object to compare with this object.</param>
		/// <returns>
		///   <see langword="true" /> if the content-disposition headers are the same; otherwise <see langword="false" />.</returns>
		public override bool Equals(object rparam)
		{
			return rparam != null && string.Equals(this.ToString(), rparam.ToString(), StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>Determines the hash code of the specified <see cref="T:System.Net.Mime.ContentDisposition" /> object</summary>
		/// <returns>An integer hash value.</returns>
		public override int GetHashCode()
		{
			return this.ToString().ToLowerInvariant().GetHashCode();
		}

		private void ParseValue()
		{
			int num = 0;
			try
			{
				this._dispositionType = MailBnfHelper.ReadToken(this._disposition, ref num, null);
				if (string.IsNullOrEmpty(this._dispositionType))
				{
					throw new FormatException("The mail header is malformed.");
				}
				if (this._parameters == null)
				{
					this._parameters = new TrackingValidationObjectDictionary(ContentDisposition.s_validators);
				}
				else
				{
					this._parameters.Clear();
				}
				while (MailBnfHelper.SkipCFWS(this._disposition, ref num))
				{
					if (this._disposition[num++] != ';')
					{
						throw new FormatException(SR.Format("An invalid character was found in the mail header: '{0}'.", this._disposition[num - 1]));
					}
					if (!MailBnfHelper.SkipCFWS(this._disposition, ref num))
					{
						break;
					}
					string text = MailBnfHelper.ReadParameterAttribute(this._disposition, ref num, null);
					if (this._disposition[num++] != '=')
					{
						throw new FormatException("The mail header is malformed.");
					}
					if (!MailBnfHelper.SkipCFWS(this._disposition, ref num))
					{
						throw new FormatException("The specified content disposition is invalid.");
					}
					string value = (this._disposition[num] == '"') ? MailBnfHelper.ReadQuotedString(this._disposition, ref num, null) : MailBnfHelper.ReadToken(this._disposition, ref num, null);
					if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(value))
					{
						throw new FormatException("The specified content disposition is invalid.");
					}
					this.Parameters.Add(text, value);
				}
			}
			catch (FormatException innerException)
			{
				throw new FormatException("The specified content disposition is invalid.", innerException);
			}
			this._parameters.IsChanged = false;
		}

		private const string CreationDateKey = "creation-date";

		private const string ModificationDateKey = "modification-date";

		private const string ReadDateKey = "read-date";

		private const string FileNameKey = "filename";

		private const string SizeKey = "size";

		private TrackingValidationObjectDictionary _parameters;

		private string _disposition;

		private string _dispositionType;

		private bool _isChanged;

		private bool _isPersisted;

		private static readonly TrackingValidationObjectDictionary.ValidateAndParseValue s_dateParser = (object v) => new SmtpDateTime(v.ToString());

		private static readonly TrackingValidationObjectDictionary.ValidateAndParseValue s_longParser = delegate(object value)
		{
			long num;
			if (!long.TryParse(value.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out num))
			{
				throw new FormatException("The specified content disposition is invalid.");
			}
			return num;
		};

		private static readonly Dictionary<string, TrackingValidationObjectDictionary.ValidateAndParseValue> s_validators = new Dictionary<string, TrackingValidationObjectDictionary.ValidateAndParseValue>
		{
			{
				"creation-date",
				ContentDisposition.s_dateParser
			},
			{
				"modification-date",
				ContentDisposition.s_dateParser
			},
			{
				"read-date",
				ContentDisposition.s_dateParser
			},
			{
				"size",
				ContentDisposition.s_longParser
			}
		};
	}
}
