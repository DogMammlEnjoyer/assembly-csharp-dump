using System;
using System.Configuration.Internal;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Xml;

namespace System.Configuration
{
	/// <summary>Wraps the corresponding <see cref="T:System.Xml.XmlDocument" /> type and also carries the necessary information for reporting file-name and line numbers.</summary>
	[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
	public sealed class ConfigXmlDocument : XmlDocument, IConfigXmlNode, IConfigErrorInfo
	{
		/// <summary>Creates a configuration element attribute.</summary>
		/// <param name="prefix">The prefix definition.</param>
		/// <param name="localName">The name that is used locally.</param>
		/// <param name="namespaceUri">The URL that is assigned to the namespace.</param>
		/// <returns>The <see cref="P:System.Xml.Serialization.XmlAttributes.XmlAttribute" /> attribute.</returns>
		public override XmlAttribute CreateAttribute(string prefix, string localName, string namespaceUri)
		{
			return new ConfigXmlDocument.ConfigXmlAttribute(this, prefix, localName, namespaceUri);
		}

		/// <summary>Creates an XML CData section.</summary>
		/// <param name="data">The data to use.</param>
		/// <returns>The <see cref="T:System.Xml.XmlCDataSection" /> value.</returns>
		public override XmlCDataSection CreateCDataSection(string data)
		{
			return new ConfigXmlDocument.ConfigXmlCDataSection(this, data);
		}

		/// <summary>Create an XML comment.</summary>
		/// <param name="data">The comment data.</param>
		/// <returns>The <see cref="T:System.Xml.XmlComment" /> value.</returns>
		public override XmlComment CreateComment(string data)
		{
			return new ConfigXmlDocument.ConfigXmlComment(this, data);
		}

		/// <summary>Creates a configuration element.</summary>
		/// <param name="prefix">The prefix definition.</param>
		/// <param name="localName">The name used locally.</param>
		/// <param name="namespaceUri">The namespace for the URL.</param>
		/// <returns>The <see cref="T:System.Xml.XmlElement" /> value.</returns>
		public override XmlElement CreateElement(string prefix, string localName, string namespaceUri)
		{
			return new ConfigXmlDocument.ConfigXmlElement(this, prefix, localName, namespaceUri);
		}

		/// <summary>Creates white spaces.</summary>
		/// <param name="data">The data to use.</param>
		/// <returns>The <see cref="T:System.Xml.XmlSignificantWhitespace" /> value.</returns>
		public override XmlSignificantWhitespace CreateSignificantWhitespace(string data)
		{
			return base.CreateSignificantWhitespace(data);
		}

		/// <summary>Create a text node.</summary>
		/// <param name="text">The text to use.</param>
		/// <returns>The <see cref="T:System.Xml.XmlText" /> value.</returns>
		public override XmlText CreateTextNode(string text)
		{
			return new ConfigXmlDocument.ConfigXmlText(this, text);
		}

		/// <summary>Creates white space.</summary>
		/// <param name="data">The data to use.</param>
		/// <returns>The <see cref="T:System.Xml.XmlWhitespace" /> value.</returns>
		public override XmlWhitespace CreateWhitespace(string data)
		{
			return base.CreateWhitespace(data);
		}

		/// <summary>Loads the configuration file.</summary>
		/// <param name="filename">The name of the file.</param>
		public override void Load(string filename)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(filename);
			try
			{
				xmlTextReader.MoveToContent();
				this.LoadSingleElement(filename, xmlTextReader);
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		/// <summary>Loads a single configuration element.</summary>
		/// <param name="filename">The name of the file.</param>
		/// <param name="sourceReader">The source for the reader.</param>
		public void LoadSingleElement(string filename, XmlTextReader sourceReader)
		{
			this.fileName = filename;
			this.lineNumber = sourceReader.LineNumber;
			string s = sourceReader.ReadOuterXml();
			this.reader = new XmlTextReader(new StringReader(s), sourceReader.NameTable);
			this.Load(this.reader);
			this.reader.Close();
		}

		/// <summary>Gets the configuration file name.</summary>
		/// <returns>The configuration file name.</returns>
		public string Filename
		{
			get
			{
				if (this.fileName != null && this.fileName.Length > 0 && SecurityManager.SecurityEnabled)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.fileName).Demand();
				}
				return this.fileName;
			}
		}

		/// <summary>Gets the current node line number.</summary>
		/// <returns>The line number for the current node.</returns>
		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		/// <summary>Gets the configuration file name.</summary>
		/// <returns>The file name.</returns>
		string IConfigErrorInfo.Filename
		{
			get
			{
				return this.Filename;
			}
		}

		/// <summary>Gets the configuration line number.</summary>
		/// <returns>The line number.</returns>
		int IConfigErrorInfo.LineNumber
		{
			get
			{
				return this.LineNumber;
			}
		}

		string IConfigXmlNode.Filename
		{
			get
			{
				return this.Filename;
			}
		}

		int IConfigXmlNode.LineNumber
		{
			get
			{
				return this.LineNumber;
			}
		}

		private XmlTextReader reader;

		private string fileName;

		private int lineNumber;

		private class ConfigXmlAttribute : XmlAttribute, IConfigXmlNode, IConfigErrorInfo
		{
			public ConfigXmlAttribute(ConfigXmlDocument document, string prefix, string localName, string namespaceUri) : base(prefix, localName, namespaceUri, document)
			{
				this.fileName = document.fileName;
				this.lineNumber = document.LineNumber;
			}

			public string Filename
			{
				get
				{
					if (this.fileName != null && this.fileName.Length > 0 && SecurityManager.SecurityEnabled)
					{
						new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.fileName).Demand();
					}
					return this.fileName;
				}
			}

			public int LineNumber
			{
				get
				{
					return this.lineNumber;
				}
			}

			private string fileName;

			private int lineNumber;
		}

		private class ConfigXmlCDataSection : XmlCDataSection, IConfigXmlNode, IConfigErrorInfo
		{
			public ConfigXmlCDataSection(ConfigXmlDocument document, string data) : base(data, document)
			{
				this.fileName = document.fileName;
				this.lineNumber = document.LineNumber;
			}

			public string Filename
			{
				get
				{
					if (this.fileName != null && this.fileName.Length > 0 && SecurityManager.SecurityEnabled)
					{
						new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.fileName).Demand();
					}
					return this.fileName;
				}
			}

			public int LineNumber
			{
				get
				{
					return this.lineNumber;
				}
			}

			private string fileName;

			private int lineNumber;
		}

		private class ConfigXmlComment : XmlComment, IConfigXmlNode
		{
			public ConfigXmlComment(ConfigXmlDocument document, string comment) : base(comment, document)
			{
				this.fileName = document.fileName;
				this.lineNumber = document.LineNumber;
			}

			public string Filename
			{
				get
				{
					if (this.fileName != null && this.fileName.Length > 0 && SecurityManager.SecurityEnabled)
					{
						new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.fileName).Demand();
					}
					return this.fileName;
				}
			}

			public int LineNumber
			{
				get
				{
					return this.lineNumber;
				}
			}

			private string fileName;

			private int lineNumber;
		}

		private class ConfigXmlElement : XmlElement, IConfigXmlNode, IConfigErrorInfo
		{
			public ConfigXmlElement(ConfigXmlDocument document, string prefix, string localName, string namespaceUri) : base(prefix, localName, namespaceUri, document)
			{
				this.fileName = document.fileName;
				this.lineNumber = document.LineNumber;
			}

			public string Filename
			{
				get
				{
					if (this.fileName != null && this.fileName.Length > 0 && SecurityManager.SecurityEnabled)
					{
						new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.fileName).Demand();
					}
					return this.fileName;
				}
			}

			public int LineNumber
			{
				get
				{
					return this.lineNumber;
				}
			}

			private string fileName;

			private int lineNumber;
		}

		private class ConfigXmlText : XmlText, IConfigXmlNode, IConfigErrorInfo
		{
			public ConfigXmlText(ConfigXmlDocument document, string data) : base(data, document)
			{
				this.fileName = document.fileName;
				this.lineNumber = document.LineNumber;
			}

			public string Filename
			{
				get
				{
					if (this.fileName != null && this.fileName.Length > 0 && SecurityManager.SecurityEnabled)
					{
						new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.fileName).Demand();
					}
					return this.fileName;
				}
			}

			public int LineNumber
			{
				get
				{
					return this.lineNumber;
				}
			}

			private string fileName;

			private int lineNumber;
		}
	}
}
