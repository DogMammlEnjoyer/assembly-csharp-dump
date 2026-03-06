using System;
using System.Resources;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Xml.Schema
{
	/// <summary>Returns detailed information about the schema exception.</summary>
	[Serializable]
	public class XmlSchemaException : SystemException
	{
		/// <summary>Constructs a new <see langword="XmlSchemaException" /> object with the given <see langword="SerializationInfo" /> and <see langword="StreamingContext" /> information that contains all the properties of the <see langword="XmlSchemaException" />.</summary>
		/// <param name="info">SerializationInfo.</param>
		/// <param name="context">StreamingContext.</param>
		protected XmlSchemaException(SerializationInfo info, StreamingContext context) : base(info, context)
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
				this.message = XmlSchemaException.CreateMessage(this.res, this.args);
				return;
			}
			this.message = null;
		}

		/// <summary>Streams all the <see langword="XmlSchemaException" /> properties into the <see langword="SerializationInfo" /> class for the given <see langword="StreamingContext" />.</summary>
		/// <param name="info">The <see langword="SerializationInfo" />. </param>
		/// <param name="context">The <see langword="StreamingContext" /> information. </param>
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

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Schema.XmlSchemaException" /> class.</summary>
		public XmlSchemaException() : this(null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Schema.XmlSchemaException" /> class with the exception message specified.</summary>
		/// <param name="message">A <see langword="string" /> description of the error condition.</param>
		public XmlSchemaException(string message) : this(message, null, 0, 0)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Schema.XmlSchemaException" /> class with the exception message and original <see cref="T:System.Exception" /> object that caused this exception specified.</summary>
		/// <param name="message">A <see langword="string" /> description of the error condition.</param>
		/// <param name="innerException">The original T:System.Exception object that caused this exception.</param>
		public XmlSchemaException(string message, Exception innerException) : this(message, innerException, 0, 0)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Schema.XmlSchemaException" /> class with the exception message specified, and the original <see cref="T:System.Exception" /> object, line number, and line position of the XML that cause this exception specified.</summary>
		/// <param name="message">A <see langword="string" /> description of the error condition.</param>
		/// <param name="innerException">The original T:System.Exception object that caused this exception.</param>
		/// <param name="lineNumber">The line number of the XML that caused this exception.</param>
		/// <param name="linePosition">The line position of the XML that caused this exception.</param>
		public XmlSchemaException(string message, Exception innerException, int lineNumber, int linePosition) : this((message == null) ? "A schema error occurred." : "{0}", new string[]
		{
			message
		}, innerException, null, lineNumber, linePosition, null)
		{
		}

		internal XmlSchemaException(string res, string[] args) : this(res, args, null, null, 0, 0, null)
		{
		}

		internal XmlSchemaException(string res, string arg) : this(res, new string[]
		{
			arg
		}, null, null, 0, 0, null)
		{
		}

		internal XmlSchemaException(string res, string arg, string sourceUri, int lineNumber, int linePosition) : this(res, new string[]
		{
			arg
		}, null, sourceUri, lineNumber, linePosition, null)
		{
		}

		internal XmlSchemaException(string res, string sourceUri, int lineNumber, int linePosition) : this(res, null, null, sourceUri, lineNumber, linePosition, null)
		{
		}

		internal XmlSchemaException(string res, string[] args, string sourceUri, int lineNumber, int linePosition) : this(res, args, null, sourceUri, lineNumber, linePosition, null)
		{
		}

		internal XmlSchemaException(string res, XmlSchemaObject source) : this(res, null, source)
		{
		}

		internal XmlSchemaException(string res, string arg, XmlSchemaObject source) : this(res, new string[]
		{
			arg
		}, source)
		{
		}

		internal XmlSchemaException(string res, string[] args, XmlSchemaObject source) : this(res, args, null, source.SourceUri, source.LineNumber, source.LinePosition, source)
		{
		}

		internal XmlSchemaException(string res, string[] args, Exception innerException, string sourceUri, int lineNumber, int linePosition, XmlSchemaObject source) : base(XmlSchemaException.CreateMessage(res, args), innerException)
		{
			base.HResult = -2146231999;
			this.res = res;
			this.args = args;
			this.sourceUri = sourceUri;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
			this.sourceSchemaObject = source;
		}

		internal static string CreateMessage(string res, string[] args)
		{
			string result;
			try
			{
				result = Res.GetString(res, args);
			}
			catch (MissingManifestResourceException)
			{
				result = "UNKNOWN(" + res + ")";
			}
			return result;
		}

		internal string GetRes
		{
			get
			{
				return this.res;
			}
		}

		internal string[] Args
		{
			get
			{
				return this.args;
			}
		}

		/// <summary>Gets the Uniform Resource Identifier (URI) location of the schema that caused the exception.</summary>
		/// <returns>The URI location of the schema that caused the exception.</returns>
		public string SourceUri
		{
			get
			{
				return this.sourceUri;
			}
		}

		/// <summary>Gets the line number indicating where the error occurred.</summary>
		/// <returns>The line number indicating where the error occurred.</returns>
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		/// <summary>Gets the line position indicating where the error occurred.</summary>
		/// <returns>The line position indicating where the error occurred.</returns>
		public int LinePosition
		{
			get
			{
				return this.linePosition;
			}
		}

		/// <summary>The <see langword="XmlSchemaObject" /> that produced the <see langword="XmlSchemaException" />.</summary>
		/// <returns>A valid object instance represents a structural validation error in the XML Schema Object Model (SOM).</returns>
		public XmlSchemaObject SourceSchemaObject
		{
			get
			{
				return this.sourceSchemaObject;
			}
		}

		internal void SetSource(string sourceUri, int lineNumber, int linePosition)
		{
			this.sourceUri = sourceUri;
			this.lineNumber = lineNumber;
			this.linePosition = linePosition;
		}

		internal void SetSchemaObject(XmlSchemaObject source)
		{
			this.sourceSchemaObject = source;
		}

		internal void SetSource(XmlSchemaObject source)
		{
			this.sourceSchemaObject = source;
			this.sourceUri = source.SourceUri;
			this.lineNumber = source.LineNumber;
			this.linePosition = source.LinePosition;
		}

		internal void SetResourceId(string resourceId)
		{
			this.res = resourceId;
		}

		/// <summary>Gets the description of the error condition of this exception.</summary>
		/// <returns>The description of the error condition of this exception.</returns>
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

		private string res;

		private string[] args;

		private string sourceUri;

		private int lineNumber;

		private int linePosition;

		[NonSerialized]
		private XmlSchemaObject sourceSchemaObject;

		private string message;
	}
}
