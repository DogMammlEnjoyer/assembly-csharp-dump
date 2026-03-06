using System;
using System.Globalization;
using System.Resources;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Utils;

namespace System.Xml.Xsl
{
	/// <summary>The exception that is thrown when an error occurs while processing an XSLT transformation.</summary>
	[Serializable]
	public class XsltException : SystemException
	{
		/// <summary>Initializes a new instance of the <see langword="XsltException" /> class using the information in the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> and <see cref="T:System.Runtime.Serialization.StreamingContext" /> objects.</summary>
		/// <param name="info">The <see langword="SerializationInfo" /> object containing all the properties of an <see langword="XsltException" />. </param>
		/// <param name="context">The <see langword="StreamingContext" /> object. </param>
		protected XsltException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.res = (string)info.GetValue("res", typeof(string));
			this.args = (string[])info.GetValue("args", typeof(string[]));
			this.sourceUri = (string)info.GetValue("sourceUri", typeof(string));
			this.lineNumber = (int)info.GetValue("lineNumber", typeof(int));
			this.linePosition = (int)info.GetValue("linePosition", typeof(int));
			string text = null;
			foreach (SerializationEntry serializationEntry in info)
			{
				if (serializationEntry.Name == "version")
				{
					text = (string)serializationEntry.Value;
				}
			}
			if (text == null)
			{
				this.message = XsltException.CreateMessage(this.res, this.args, this.sourceUri, this.lineNumber, this.linePosition);
				return;
			}
			this.message = null;
		}

		/// <summary>Streams all the <see langword="XsltException" /> properties into the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> class for the given <see cref="T:System.Runtime.Serialization.StreamingContext" />.</summary>
		/// <param name="info">The <see langword="SerializationInfo" /> object. </param>
		/// <param name="context">The <see langword="StreamingContext" /> object. </param>
		[SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("res", this.res);
			info.AddValue("args", this.args);
			info.AddValue("sourceUri", this.sourceUri);
			info.AddValue("lineNumber", this.lineNumber);
			info.AddValue("linePosition", this.linePosition);
			info.AddValue("version", "2.0");
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Xsl.XsltException" /> class.</summary>
		public XsltException() : this(string.Empty, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Xsl.XsltException" /> class with a specified error message. </summary>
		/// <param name="message">The message that describes the error.</param>
		public XsltException(string message) : this(message, null)
		{
		}

		/// <summary>Initializes a new instance of the <see langword="XsltException" /> class.</summary>
		/// <param name="message">The description of the error condition. </param>
		/// <param name="innerException">The <see cref="T:System.Exception" /> which threw the <see langword="XsltException" />, if any. This value can be <see langword="null" />. </param>
		public XsltException(string message, Exception innerException) : this("{0}", new string[]
		{
			message
		}, null, 0, 0, innerException)
		{
		}

		internal static XsltException Create(string res, params string[] args)
		{
			return new XsltException(res, args, null, 0, 0, null);
		}

		internal static XsltException Create(string res, string[] args, Exception inner)
		{
			return new XsltException(res, args, null, 0, 0, inner);
		}

		internal XsltException(string res, string[] args, string sourceUri, int lineNumber, int linePosition, Exception inner) : base(XsltException.CreateMessage(res, args, sourceUri, lineNumber, linePosition), inner)
		{
			base.HResult = -2146231998;
			this.res = res;
			this.sourceUri = sourceUri;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}

		/// <summary>Gets the location path of the style sheet.</summary>
		/// <returns>The location path of the style sheet.</returns>
		public virtual string SourceUri
		{
			get
			{
				return this.sourceUri;
			}
		}

		/// <summary>Gets the line number indicating where the error occurred in the style sheet.</summary>
		/// <returns>The line number indicating where the error occurred in the style sheet.</returns>
		public virtual int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		/// <summary>Gets the line position indicating where the error occurred in the style sheet.</summary>
		/// <returns>The line position indicating where the error occurred in the style sheet.</returns>
		public virtual int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		/// <summary>Gets the formatted error message describing the current exception.</summary>
		/// <returns>The formatted error message describing the current exception.</returns>
		public override string Message
		{
			get
			{
				if (this.message != null)
				{
					return this.message;
				}
				return base.Message;
			}
		}

		private static string CreateMessage(string res, string[] args, string sourceUri, int lineNumber, int linePosition)
		{
			string result;
			try
			{
				string text = XsltException.FormatMessage(res, args);
				if (res != "XSLT compile error at {0}({1},{2}). See InnerException for details." && lineNumber != 0)
				{
					text = text + " " + XsltException.FormatMessage("An error occurred at {0}({1},{2}).", new string[]
					{
						sourceUri,
						lineNumber.ToString(CultureInfo.InvariantCulture),
						linePosition.ToString(CultureInfo.InvariantCulture)
					});
				}
				result = text;
			}
			catch (MissingManifestResourceException)
			{
				result = "UNKNOWN(" + res + ")";
			}
			return result;
		}

		private static string FormatMessage(string key, params string[] args)
		{
			string text = Res.GetString(key);
			if (text != null && args != null)
			{
				text = string.Format(CultureInfo.InvariantCulture, text, args);
			}
			return text;
		}

		private string res;

		private string[] args;

		private string sourceUri;

		private int lineNumber;

		private int linePosition;

		private string message;
	}
}
