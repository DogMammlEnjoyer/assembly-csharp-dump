using System;
using System.Collections;
using System.Configuration.Internal;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;

namespace System.Configuration
{
	/// <summary>The exception that is thrown when a configuration error has occurred.</summary>
	[Serializable]
	public class ConfigurationErrorsException : ConfigurationException
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationErrorsException" /> class.</summary>
		public ConfigurationErrorsException()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationErrorsException" /> class.</summary>
		/// <param name="message">A message that describes why this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</param>
		public ConfigurationErrorsException(string message) : base(message)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationErrorsException" /> class.</summary>
		/// <param name="info">The object that holds the information to deserialize.</param>
		/// <param name="context">Contextual information about the source or destination.</param>
		/// <exception cref="T:System.InvalidOperationException">The current type is not a <see cref="T:System.Configuration.ConfigurationException" /> or a <see cref="T:System.Configuration.ConfigurationErrorsException" />.</exception>
		protected ConfigurationErrorsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filename = info.GetString("ConfigurationErrors_Filename");
			this.line = info.GetInt32("ConfigurationErrors_Line");
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationErrorsException" /> class.</summary>
		/// <param name="message">A message that describes why this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</param>
		/// <param name="inner">The exception that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		public ConfigurationErrorsException(string message, Exception inner) : base(message, inner)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationErrorsException" /> class.</summary>
		/// <param name="message">A message that describes why this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</param>
		/// <param name="node">The <see cref="T:System.Xml.XmlNode" /> object that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		public ConfigurationErrorsException(string message, XmlNode node) : this(message, null, ConfigurationErrorsException.GetFilename(node), ConfigurationErrorsException.GetLineNumber(node))
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationErrorsException" /> class.</summary>
		/// <param name="message">A message that describes why this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</param>
		/// <param name="inner">The inner exception that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		/// <param name="node">The <see cref="T:System.Xml.XmlNode" /> object that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		public ConfigurationErrorsException(string message, Exception inner, XmlNode node) : this(message, inner, ConfigurationErrorsException.GetFilename(node), ConfigurationErrorsException.GetLineNumber(node))
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationErrorsException" /> class.</summary>
		/// <param name="message">A message that describes why this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</param>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> object that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		public ConfigurationErrorsException(string message, XmlReader reader) : this(message, null, ConfigurationErrorsException.GetFilename(reader), ConfigurationErrorsException.GetLineNumber(reader))
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationErrorsException" /> class.</summary>
		/// <param name="message">A message that describes why this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</param>
		/// <param name="inner">The inner exception that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> object that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		public ConfigurationErrorsException(string message, Exception inner, XmlReader reader) : this(message, inner, ConfigurationErrorsException.GetFilename(reader), ConfigurationErrorsException.GetLineNumber(reader))
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationErrorsException" /> class.</summary>
		/// <param name="message">A message that describes why this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</param>
		/// <param name="filename">The path to the configuration file that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		/// <param name="line">The line number within the configuration file at which this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</param>
		public ConfigurationErrorsException(string message, string filename, int line) : this(message, null, filename, line)
		{
		}

		/// <summary>Initializes a new instance of a <see cref="T:System.Configuration.ConfigurationErrorsException" /> class.</summary>
		/// <param name="message">A message that describes why this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</param>
		/// <param name="inner">The inner exception that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		/// <param name="filename">The path to the configuration file that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		/// <param name="line">The line number within the configuration file at which this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</param>
		public ConfigurationErrorsException(string message, Exception inner, string filename, int line) : base(message, inner)
		{
			this.filename = filename;
			this.line = line;
		}

		/// <summary>Gets a description of why this configuration exception was thrown.</summary>
		/// <returns>A description of why this <see cref="T:System.Configuration.ConfigurationErrorsException" /> was thrown.</returns>
		public override string BareMessage
		{
			get
			{
				return base.BareMessage;
			}
		}

		/// <summary>Gets a collection of errors that detail the reasons this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> object that contains errors that identify the reasons this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</returns>
		public ICollection Errors
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the path to the configuration file that caused this configuration exception to be thrown.</summary>
		/// <returns>The path to the configuration file that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> to be thrown.</returns>
		public override string Filename
		{
			get
			{
				return this.filename;
			}
		}

		/// <summary>Gets the line number within the configuration file at which this configuration exception was thrown.</summary>
		/// <returns>The line number within the configuration file at which this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</returns>
		public override int Line
		{
			get
			{
				return this.line;
			}
		}

		/// <summary>Gets an extended description of why this configuration exception was thrown.</summary>
		/// <returns>An extended description of why this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception was thrown.</returns>
		public override string Message
		{
			get
			{
				string result;
				if (!string.IsNullOrEmpty(this.filename))
				{
					if (this.line != 0)
					{
						result = string.Concat(new string[]
						{
							this.BareMessage,
							" (",
							this.filename,
							" line ",
							this.line.ToString(),
							")"
						});
					}
					else
					{
						result = this.BareMessage + " (" + this.filename + ")";
					}
				}
				else if (this.line != 0)
				{
					result = this.BareMessage + " (line " + this.line.ToString() + ")";
				}
				else
				{
					result = this.BareMessage;
				}
				return result;
			}
		}

		/// <summary>Gets the path to the configuration file that the internal <see cref="T:System.Xml.XmlReader" /> was reading when this configuration exception was thrown.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> object that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		/// <returns>The path of the configuration file the internal <see cref="T:System.Xml.XmlReader" /> object was accessing when the exception occurred.</returns>
		public static string GetFilename(XmlReader reader)
		{
			if (reader is IConfigErrorInfo)
			{
				return ((IConfigErrorInfo)reader).Filename;
			}
			if (reader == null)
			{
				return null;
			}
			return reader.BaseURI;
		}

		/// <summary>Gets the line number within the configuration file that the internal <see cref="T:System.Xml.XmlReader" /> object was processing when this configuration exception was thrown.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> object that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		/// <returns>The line number within the configuration file that the <see cref="T:System.Xml.XmlReader" /> object was accessing when the exception occurred.</returns>
		public static int GetLineNumber(XmlReader reader)
		{
			if (reader is IConfigErrorInfo)
			{
				return ((IConfigErrorInfo)reader).LineNumber;
			}
			IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
			if (xmlLineInfo == null)
			{
				return 0;
			}
			return xmlLineInfo.LineNumber;
		}

		/// <summary>Gets the path to the configuration file from which the internal <see cref="T:System.Xml.XmlNode" /> object was loaded when this configuration exception was thrown.</summary>
		/// <param name="node">The <see cref="T:System.Xml.XmlNode" /> object that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		/// <returns>The path to the configuration file from which the internal <see cref="T:System.Xml.XmlNode" /> object was loaded when this configuration exception was thrown.</returns>
		public static string GetFilename(XmlNode node)
		{
			if (!(node is IConfigErrorInfo))
			{
				return null;
			}
			return ((IConfigErrorInfo)node).Filename;
		}

		/// <summary>Gets the line number within the configuration file that the internal <see cref="T:System.Xml.XmlNode" /> object represented when this configuration exception was thrown.</summary>
		/// <param name="node">The <see cref="T:System.Xml.XmlNode" /> object that caused this <see cref="T:System.Configuration.ConfigurationErrorsException" /> exception to be thrown.</param>
		/// <returns>The line number within the configuration file that contains the <see cref="T:System.Xml.XmlNode" /> object being parsed when this configuration exception was thrown.</returns>
		public static int GetLineNumber(XmlNode node)
		{
			if (!(node is IConfigErrorInfo))
			{
				return 0;
			}
			return ((IConfigErrorInfo)node).LineNumber;
		}

		/// <summary>Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the file name and line number at which this configuration exception occurred.</summary>
		/// <param name="info">The object that holds the information to be serialized.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ConfigurationErrors_Filename", this.filename);
			info.AddValue("ConfigurationErrors_Line", this.line);
		}

		private readonly string filename;

		private readonly int line;
	}
}
