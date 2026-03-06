using System;
using System.Collections.Specialized;
using System.Net.Mime;
using System.Text;

namespace System.Net.Mail
{
	/// <summary>Represents an email message that can be sent using the <see cref="T:System.Net.Mail.SmtpClient" /> class.</summary>
	public class MailMessage : IDisposable
	{
		/// <summary>Initializes an empty instance of the <see cref="T:System.Net.Mail.MailMessage" /> class.</summary>
		public MailMessage()
		{
			this.to = new MailAddressCollection();
			this.alternateViews = new AlternateViewCollection();
			this.attachments = new AttachmentCollection();
			this.bcc = new MailAddressCollection();
			this.cc = new MailAddressCollection();
			this.replyTo = new MailAddressCollection();
			this.headers = new NameValueCollection();
			this.headers.Add("MIME-Version", "1.0");
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Mail.MailMessage" /> class by using the specified <see cref="T:System.Net.Mail.MailAddress" /> class objects.</summary>
		/// <param name="from">A <see cref="T:System.Net.Mail.MailAddress" /> that contains the address of the sender of the email message.</param>
		/// <param name="to">A <see cref="T:System.Net.Mail.MailAddress" /> that contains the address of the recipient of the email message.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="from" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="to" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="from" /> or <paramref name="to" /> is malformed.</exception>
		public MailMessage(MailAddress from, MailAddress to) : this()
		{
			if (from == null || to == null)
			{
				throw new ArgumentNullException();
			}
			this.From = from;
			this.to.Add(to);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Mail.MailMessage" /> class by using the specified <see cref="T:System.String" /> class objects.</summary>
		/// <param name="from">A <see cref="T:System.String" /> that contains the address of the sender of the email message.</param>
		/// <param name="to">A <see cref="T:System.String" /> that contains the addresses of the recipients of the email message. Multiple email addresses must be separated with a comma character (",").</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="from" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="to" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="from" /> is <see cref="F:System.String.Empty" /> ("").  
		/// -or-  
		/// <paramref name="to" /> is <see cref="F:System.String.Empty" /> ("").</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="from" /> or <paramref name="to" /> is malformed.</exception>
		public MailMessage(string from, string to) : this()
		{
			if (from == null || from == string.Empty)
			{
				throw new ArgumentNullException("from");
			}
			if (to == null || to == string.Empty)
			{
				throw new ArgumentNullException("to");
			}
			this.from = new MailAddress(from);
			foreach (string text in to.Split(new char[]
			{
				','
			}))
			{
				this.to.Add(new MailAddress(text.Trim()));
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Mail.MailMessage" /> class.</summary>
		/// <param name="from">A <see cref="T:System.String" /> that contains the address of the sender of the email message.</param>
		/// <param name="to">A <see cref="T:System.String" /> that contains the addresses of the recipients of the email message. Multiple email addresses must be separated with a comma character (",").</param>
		/// <param name="subject">A <see cref="T:System.String" /> that contains the subject text.</param>
		/// <param name="body">A <see cref="T:System.String" /> that contains the message body.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="from" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="to" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="from" /> is <see cref="F:System.String.Empty" /> ("").  
		/// -or-  
		/// <paramref name="to" /> is <see cref="F:System.String.Empty" /> ("").</exception>
		/// <exception cref="T:System.FormatException">
		///   <paramref name="from" /> or <paramref name="to" /> is malformed.</exception>
		public MailMessage(string from, string to, string subject, string body) : this()
		{
			if (from == null || from == string.Empty)
			{
				throw new ArgumentNullException("from");
			}
			if (to == null || to == string.Empty)
			{
				throw new ArgumentNullException("to");
			}
			this.from = new MailAddress(from);
			foreach (string text in to.Split(new char[]
			{
				','
			}))
			{
				this.to.Add(new MailAddress(text.Trim()));
			}
			this.Body = body;
			this.Subject = subject;
		}

		/// <summary>Gets the attachment collection used to store alternate forms of the message body.</summary>
		/// <returns>A writable <see cref="T:System.Net.Mail.AlternateViewCollection" />.</returns>
		public AlternateViewCollection AlternateViews
		{
			get
			{
				return this.alternateViews;
			}
		}

		/// <summary>Gets the attachment collection used to store data attached to this email message.</summary>
		/// <returns>A writable <see cref="T:System.Net.Mail.AttachmentCollection" />.</returns>
		public AttachmentCollection Attachments
		{
			get
			{
				return this.attachments;
			}
		}

		/// <summary>Gets the address collection that contains the blind carbon copy (BCC) recipients for this email message.</summary>
		/// <returns>A writable <see cref="T:System.Net.Mail.MailAddressCollection" /> object.</returns>
		public MailAddressCollection Bcc
		{
			get
			{
				return this.bcc;
			}
		}

		/// <summary>Gets or sets the message body.</summary>
		/// <returns>A <see cref="T:System.String" /> value that contains the body text.</returns>
		public string Body
		{
			get
			{
				return this.body;
			}
			set
			{
				if (value != null && this.bodyEncoding == null)
				{
					this.bodyEncoding = (this.GuessEncoding(value) ?? Encoding.ASCII);
				}
				this.body = value;
			}
		}

		internal ContentType BodyContentType
		{
			get
			{
				return new ContentType(this.isHtml ? "text/html" : "text/plain")
				{
					CharSet = (this.BodyEncoding ?? Encoding.ASCII).HeaderName
				};
			}
		}

		internal TransferEncoding ContentTransferEncoding
		{
			get
			{
				return MailMessage.GuessTransferEncoding(this.BodyEncoding);
			}
		}

		/// <summary>Gets or sets the encoding used to encode the message body.</summary>
		/// <returns>An <see cref="T:System.Text.Encoding" /> applied to the contents of the <see cref="P:System.Net.Mail.MailMessage.Body" />.</returns>
		public Encoding BodyEncoding
		{
			get
			{
				return this.bodyEncoding;
			}
			set
			{
				this.bodyEncoding = value;
			}
		}

		/// <summary>Gets or sets the transfer encoding used to encode the message body.</summary>
		/// <returns>A <see cref="T:System.Net.Mime.TransferEncoding" /> applied to the contents of the <see cref="P:System.Net.Mail.MailMessage.Body" />.</returns>
		public TransferEncoding BodyTransferEncoding
		{
			get
			{
				return MailMessage.GuessTransferEncoding(this.BodyEncoding);
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the address collection that contains the carbon copy (CC) recipients for this email message.</summary>
		/// <returns>A writable <see cref="T:System.Net.Mail.MailAddressCollection" /> object.</returns>
		public MailAddressCollection CC
		{
			get
			{
				return this.cc;
			}
		}

		/// <summary>Gets or sets the delivery notifications for this email message.</summary>
		/// <returns>A <see cref="T:System.Net.Mail.DeliveryNotificationOptions" /> value that contains the delivery notifications for this message.</returns>
		public DeliveryNotificationOptions DeliveryNotificationOptions
		{
			get
			{
				return this.deliveryNotificationOptions;
			}
			set
			{
				this.deliveryNotificationOptions = value;
			}
		}

		/// <summary>Gets or sets the from address for this email message.</summary>
		/// <returns>A <see cref="T:System.Net.Mail.MailAddress" /> that contains the from address information.</returns>
		public MailAddress From
		{
			get
			{
				return this.from;
			}
			set
			{
				this.from = value;
			}
		}

		/// <summary>Gets the email headers that are transmitted with this email message.</summary>
		/// <returns>A <see cref="T:System.Collections.Specialized.NameValueCollection" /> that contains the email headers.</returns>
		public NameValueCollection Headers
		{
			get
			{
				return this.headers;
			}
		}

		/// <summary>Gets or sets a value indicating whether the mail message body is in HTML.</summary>
		/// <returns>
		///   <see langword="true" /> if the message body is in HTML; else <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool IsBodyHtml
		{
			get
			{
				return this.isHtml;
			}
			set
			{
				this.isHtml = value;
			}
		}

		/// <summary>Gets or sets the priority of this email message.</summary>
		/// <returns>A <see cref="T:System.Net.Mail.MailPriority" /> that contains the priority of this message.</returns>
		public MailPriority Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				this.priority = value;
			}
		}

		/// <summary>Gets or sets the encoding used for the user-defined custom headers for this email message.</summary>
		/// <returns>The encoding used for user-defined custom headers for this email message.</returns>
		public Encoding HeadersEncoding
		{
			get
			{
				return this.headersEncoding;
			}
			set
			{
				this.headersEncoding = value;
			}
		}

		/// <summary>Gets the list of addresses to reply to for the mail message.</summary>
		/// <returns>The list of the addresses to reply to for the mail message.</returns>
		public MailAddressCollection ReplyToList
		{
			get
			{
				return this.replyTo;
			}
		}

		/// <summary>Gets or sets the ReplyTo address for the mail message.</summary>
		/// <returns>A MailAddress that indicates the value of the <see cref="P:System.Net.Mail.MailMessage.ReplyTo" /> field.</returns>
		[Obsolete("Use ReplyToList instead")]
		public MailAddress ReplyTo
		{
			get
			{
				if (this.replyTo.Count == 0)
				{
					return null;
				}
				return this.replyTo[0];
			}
			set
			{
				this.replyTo.Clear();
				this.replyTo.Add(value);
			}
		}

		/// <summary>Gets or sets the sender's address for this email message.</summary>
		/// <returns>A <see cref="T:System.Net.Mail.MailAddress" /> that contains the sender's address information.</returns>
		public MailAddress Sender
		{
			get
			{
				return this.sender;
			}
			set
			{
				this.sender = value;
			}
		}

		/// <summary>Gets or sets the subject line for this email message.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the subject content.</returns>
		public string Subject
		{
			get
			{
				return this.subject;
			}
			set
			{
				if (value != null && this.subjectEncoding == null)
				{
					this.subjectEncoding = this.GuessEncoding(value);
				}
				this.subject = value;
			}
		}

		/// <summary>Gets or sets the encoding used for the subject content for this email message.</summary>
		/// <returns>An <see cref="T:System.Text.Encoding" /> that was used to encode the <see cref="P:System.Net.Mail.MailMessage.Subject" /> property.</returns>
		public Encoding SubjectEncoding
		{
			get
			{
				return this.subjectEncoding;
			}
			set
			{
				this.subjectEncoding = value;
			}
		}

		/// <summary>Gets the address collection that contains the recipients of this email message.</summary>
		/// <returns>A writable <see cref="T:System.Net.Mail.MailAddressCollection" /> object.</returns>
		public MailAddressCollection To
		{
			get
			{
				return this.to;
			}
		}

		/// <summary>Releases all resources used by the <see cref="T:System.Net.Mail.MailMessage" />.</summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Net.Mail.MailMessage" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///   <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		private Encoding GuessEncoding(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] >= '\u0080')
				{
					return MailMessage.UTF8Unmarked;
				}
			}
			return null;
		}

		internal static TransferEncoding GuessTransferEncoding(Encoding enc)
		{
			if (Encoding.ASCII.Equals(enc))
			{
				return TransferEncoding.SevenBit;
			}
			if (Encoding.UTF8.CodePage == enc.CodePage || Encoding.Unicode.CodePage == enc.CodePage || Encoding.UTF32.CodePage == enc.CodePage)
			{
				return TransferEncoding.Base64;
			}
			return TransferEncoding.QuotedPrintable;
		}

		internal static string To2047(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in bytes)
			{
				if (b < 33 || b > 126 || b == 63 || b == 61 || b == 95)
				{
					stringBuilder.Append('=');
					stringBuilder.Append(MailMessage.hex[b >> 4 & 15]);
					stringBuilder.Append(MailMessage.hex[(int)(b & 15)]);
				}
				else
				{
					stringBuilder.Append((char)b);
				}
			}
			return stringBuilder.ToString();
		}

		internal static string EncodeSubjectRFC2047(string s, Encoding enc)
		{
			if (s == null || Encoding.ASCII.Equals(enc))
			{
				return s;
			}
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] >= '\u0080')
				{
					string text = MailMessage.To2047(enc.GetBytes(s));
					return string.Concat(new string[]
					{
						"=?",
						enc.HeaderName,
						"?Q?",
						text,
						"?="
					});
				}
			}
			return s;
		}

		private static Encoding UTF8Unmarked
		{
			get
			{
				if (MailMessage.utf8unmarked == null)
				{
					MailMessage.utf8unmarked = new UTF8Encoding(false);
				}
				return MailMessage.utf8unmarked;
			}
		}

		private AlternateViewCollection alternateViews;

		private AttachmentCollection attachments;

		private MailAddressCollection bcc;

		private MailAddressCollection replyTo;

		private string body;

		private MailPriority priority;

		private MailAddress sender;

		private DeliveryNotificationOptions deliveryNotificationOptions;

		private MailAddressCollection cc;

		private MailAddress from;

		private NameValueCollection headers;

		private MailAddressCollection to;

		private string subject;

		private Encoding subjectEncoding;

		private Encoding bodyEncoding;

		private Encoding headersEncoding = Encoding.UTF8;

		private bool isHtml;

		private static char[] hex = new char[]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};

		private static Encoding utf8unmarked;
	}
}
