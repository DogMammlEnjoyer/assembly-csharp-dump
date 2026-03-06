using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;

namespace System.Configuration
{
	/// <summary>The exception that is thrown when a configuration system error has occurred.</summary>
	[Serializable]
	public class ConfigurationException : SystemException
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
		[Obsolete("This class is obsolete.  Use System.Configuration.ConfigurationErrorsException")]
		public ConfigurationException() : this(null)
		{
			this.filename = null;
			this.line = 0;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
		/// <param name="message">A message describing why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</param>
		[Obsolete("This class is obsolete.  Use System.Configuration.ConfigurationErrorsException")]
		public ConfigurationException(string message) : base(message)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
		/// <param name="info">The object that holds the information to deserialize.</param>
		/// <param name="context">Contextual information about the source or destination.</param>
		protected ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filename = info.GetString("filename");
			this.line = info.GetInt32("line");
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
		/// <param name="message">A message describing why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</param>
		/// <param name="inner">The inner exception that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown, if any.</param>
		[Obsolete("This class is obsolete.  Use System.Configuration.ConfigurationErrorsException")]
		public ConfigurationException(string message, Exception inner) : base(message, inner)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
		/// <param name="message">A message describing why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</param>
		/// <param name="node">The <see cref="T:System.Xml.XmlNode" /> that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown.</param>
		[Obsolete("This class is obsolete.  Use System.Configuration.ConfigurationErrorsException")]
		public ConfigurationException(string message, XmlNode node) : base(message)
		{
			this.filename = ConfigurationException.GetXmlNodeFilename(node);
			this.line = ConfigurationException.GetXmlNodeLineNumber(node);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
		/// <param name="message">A message describing why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</param>
		/// <param name="inner">The inner exception that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown, if any.</param>
		/// <param name="node">The <see cref="T:System.Xml.XmlNode" /> that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown.</param>
		[Obsolete("This class is obsolete.  Use System.Configuration.ConfigurationErrorsException")]
		public ConfigurationException(string message, Exception inner, XmlNode node) : base(message, inner)
		{
			this.filename = ConfigurationException.GetXmlNodeFilename(node);
			this.line = ConfigurationException.GetXmlNodeLineNumber(node);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
		/// <param name="message">A message describing why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</param>
		/// <param name="filename">The path to the configuration file that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown.</param>
		/// <param name="line">The line number within the configuration file at which this <see cref="T:System.Configuration.ConfigurationException" /> was thrown.</param>
		[Obsolete("This class is obsolete.  Use System.Configuration.ConfigurationErrorsException")]
		public ConfigurationException(string message, string filename, int line) : base(message)
		{
			this.filename = filename;
			this.line = line;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationException" /> class.</summary>
		/// <param name="message">A message describing why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</param>
		/// <param name="inner">The inner exception that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown, if any.</param>
		/// <param name="filename">The path to the configuration file that caused this <see cref="T:System.Configuration.ConfigurationException" /> to be thrown.</param>
		/// <param name="line">The line number within the configuration file at which this <see cref="T:System.Configuration.ConfigurationException" /> was thrown.</param>
		[Obsolete("This class is obsolete.  Use System.Configuration.ConfigurationErrorsException")]
		public ConfigurationException(string message, Exception inner, string filename, int line) : base(message, inner)
		{
			this.filename = filename;
			this.line = line;
		}

		/// <summary>Gets a description of why this configuration exception was thrown.</summary>
		/// <returns>A description of why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</returns>
		public virtual string BareMessage
		{
			get
			{
				return base.Message;
			}
		}

		/// <summary>Gets the path to the configuration file that caused this configuration exception to be thrown.</summary>
		/// <returns>The path to the configuration file that caused this <see cref="T:System.Configuration.ConfigurationException" /> exception to be thrown.</returns>
		public virtual string Filename
		{
			get
			{
				return this.filename;
			}
		}

		/// <summary>Gets the line number within the configuration file at which this configuration exception was thrown.</summary>
		/// <returns>The line number within the configuration file at which this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</returns>
		public virtual int Line
		{
			get
			{
				return this.line;
			}
		}

		/// <summary>Gets an extended description of why this configuration exception was thrown.</summary>
		/// <returns>An extended description of why this <see cref="T:System.Configuration.ConfigurationException" /> exception was thrown.</returns>
		public override string Message
		{
			get
			{
				string result;
				if (this.filename != null && this.filename.Length != 0)
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

		/// <summary>Gets the path to the configuration file from which the internal <see cref="T:System.Xml.XmlNode" /> object was loaded when this configuration exception was thrown.</summary>
		/// <param name="node">The <see cref="T:System.Xml.XmlNode" /> that caused this <see cref="T:System.Configuration.ConfigurationException" /> exception to be thrown.</param>
		/// <returns>A <see langword="string" /> representing the node file name.</returns>
		[Obsolete("This class is obsolete.  Use System.Configuration.ConfigurationErrorsException")]
		public static string GetXmlNodeFilename(XmlNode node)
		{
			if (!(node is IConfigXmlNode))
			{
				return string.Empty;
			}
			return ((IConfigXmlNode)node).Filename;
		}

		/// <summary>Gets the line number within the configuration file that the internal <see cref="T:System.Xml.XmlNode" /> object represented when this configuration exception was thrown.</summary>
		/// <param name="node">The <see cref="T:System.Xml.XmlNode" /> that caused this <see cref="T:System.Configuration.ConfigurationException" /> exception to be thrown.</param>
		/// <returns>An <see langword="int" /> representing the node line number.</returns>
		[Obsolete("This class is obsolete.  Use System.Configuration.ConfigurationErrorsException")]
		public static int GetXmlNodeLineNumber(XmlNode node)
		{
			if (!(node is IConfigXmlNode))
			{
				return 0;
			}
			return ((IConfigXmlNode)node).LineNumber;
		}

		/// <summary>Sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the file name and line number at which this configuration exception occurred.</summary>
		/// <param name="info">The object that holds the information to be serialized.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("filename", this.filename);
			info.AddValue("line", this.line);
		}

		private readonly string filename;

		private readonly int line;
	}
}
