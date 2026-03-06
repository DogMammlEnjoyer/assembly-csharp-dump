using System;
using System.Text;

namespace System.Xml
{
	/// <summary>Represents the XML declaration node &lt;?xml version='1.0'...?&gt;.</summary>
	public class XmlDeclaration : XmlLinkedNode
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlDeclaration" /> class.</summary>
		/// <param name="version">The XML version; see the <see cref="P:System.Xml.XmlDeclaration.Version" /> property.</param>
		/// <param name="encoding">The encoding scheme; see the <see cref="P:System.Xml.XmlDeclaration.Encoding" /> property.</param>
		/// <param name="standalone">Indicates whether the XML document depends on an external DTD; see the <see cref="P:System.Xml.XmlDeclaration.Standalone" /> property.</param>
		/// <param name="doc">The parent XML document.</param>
		protected internal XmlDeclaration(string version, string encoding, string standalone, XmlDocument doc) : base(doc)
		{
			if (!this.IsValidXmlVersion(version))
			{
				throw new ArgumentException(Res.GetString("Wrong XML version information. The XML must match production \"VersionNum ::= '1.' [0-9]+\"."));
			}
			if (standalone != null && standalone.Length > 0 && standalone != "yes" && standalone != "no")
			{
				throw new ArgumentException(Res.GetString("Wrong value for the XML declaration standalone attribute of '{0}'.", new object[]
				{
					standalone
				}));
			}
			this.Encoding = encoding;
			this.Standalone = standalone;
			this.Version = version;
		}

		/// <summary>Gets the XML version of the document.</summary>
		/// <returns>The value is always <see langword="1.0" />.</returns>
		public string Version
		{
			get
			{
				return this.version;
			}
			internal set
			{
				this.version = value;
			}
		}

		/// <summary>Gets or sets the encoding level of the XML document.</summary>
		/// <returns>The valid character encoding name. The most commonly supported character encoding names for XML are the following: Category Encoding Names Unicode UTF-8, UTF-16 ISO 10646 ISO-10646-UCS-2, ISO-10646-UCS-4 ISO 8859 ISO-8859-n (where "n" is a digit from 1 to 9) JIS X-0208-1997 ISO-2022-JP, Shift_JIS, EUC-JP This value is optional. If a value is not set, this property returns String.Empty.If an encoding attribute is not included, UTF-8 encoding is assumed when the document is written or saved out.</returns>
		public string Encoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				this.encoding = ((value == null) ? string.Empty : value);
			}
		}

		/// <summary>Gets or sets the value of the standalone attribute.</summary>
		/// <returns>Valid values are <see langword="yes" /> if all entity declarations required by the XML document are contained within the document or <see langword="no" /> if an external document type definition (DTD) is required. If a standalone attribute is not present in the XML declaration, this property returns String.Empty.</returns>
		public string Standalone
		{
			get
			{
				return this.standalone;
			}
			set
			{
				if (value == null)
				{
					this.standalone = string.Empty;
					return;
				}
				if (value.Length == 0 || value == "yes" || value == "no")
				{
					this.standalone = value;
					return;
				}
				throw new ArgumentException(Res.GetString("Wrong value for the XML declaration standalone attribute of '{0}'.", new object[]
				{
					value
				}));
			}
		}

		/// <summary>Gets or sets the value of the <see langword="XmlDeclaration" />.</summary>
		/// <returns>The contents of the <see langword="XmlDeclaration" /> (that is, everything between &lt;?xml and ?&gt;).</returns>
		public override string Value
		{
			get
			{
				return this.InnerText;
			}
			set
			{
				this.InnerText = value;
			}
		}

		/// <summary>Gets or sets the concatenated values of the <see langword="XmlDeclaration" />.</summary>
		/// <returns>The concatenated values of the <see langword="XmlDeclaration" /> (that is, everything between &lt;?xml and ?&gt;).</returns>
		public override string InnerText
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder("version=\"" + this.Version + "\"");
				if (this.Encoding.Length > 0)
				{
					stringBuilder.Append(" encoding=\"");
					stringBuilder.Append(this.Encoding);
					stringBuilder.Append("\"");
				}
				if (this.Standalone.Length > 0)
				{
					stringBuilder.Append(" standalone=\"");
					stringBuilder.Append(this.Standalone);
					stringBuilder.Append("\"");
				}
				return stringBuilder.ToString();
			}
			set
			{
				string text = null;
				string text2 = null;
				string text3 = null;
				string text4 = this.Encoding;
				string text5 = this.Standalone;
				string text6 = this.Version;
				XmlLoader.ParseXmlDeclarationValue(value, out text, out text2, out text3);
				try
				{
					if (text != null && !this.IsValidXmlVersion(text))
					{
						throw new ArgumentException(Res.GetString("Wrong XML version information. The XML must match production \"VersionNum ::= '1.' [0-9]+\"."));
					}
					this.Version = text;
					if (text2 != null)
					{
						this.Encoding = text2;
					}
					if (text3 != null)
					{
						this.Standalone = text3;
					}
				}
				catch
				{
					this.Encoding = text4;
					this.Standalone = text5;
					this.Version = text6;
					throw;
				}
			}
		}

		/// <summary>Gets the qualified name of the node.</summary>
		/// <returns>For <see langword="XmlDeclaration" /> nodes, the name is <see langword="xml" />.</returns>
		public override string Name
		{
			get
			{
				return "xml";
			}
		}

		/// <summary>Gets the local name of the node.</summary>
		/// <returns>For <see langword="XmlDeclaration" /> nodes, the local name is <see langword="xml" />.</returns>
		public override string LocalName
		{
			get
			{
				return this.Name;
			}
		}

		/// <summary>Gets the type of the current node.</summary>
		/// <returns>For <see langword="XmlDeclaration" /> nodes, this value is XmlNodeType.XmlDeclaration.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.XmlDeclaration;
			}
		}

		/// <summary>Creates a duplicate of this node.</summary>
		/// <param name="deep">
		///       <see langword="true" /> to recursively clone the subtree under the specified node; <see langword="false" /> to clone only the node itself. Because <see langword="XmlDeclaration" /> nodes do not have children, the cloned node always includes the data value, regardless of the parameter setting. </param>
		/// <returns>The cloned node.</returns>
		public override XmlNode CloneNode(bool deep)
		{
			return this.OwnerDocument.CreateXmlDeclaration(this.Version, this.Encoding, this.Standalone);
		}

		/// <summary>Saves the node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteTo(XmlWriter w)
		{
			w.WriteProcessingInstruction(this.Name, this.InnerText);
		}

		/// <summary>Saves the children of the node to the specified <see cref="T:System.Xml.XmlWriter" />. Because <see langword="XmlDeclaration" /> nodes do not have children, this method has no effect.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteContentTo(XmlWriter w)
		{
		}

		private bool IsValidXmlVersion(string ver)
		{
			return ver.Length >= 3 && ver[0] == '1' && ver[1] == '.' && XmlCharType.IsOnlyDigits(ver, 2, ver.Length - 2);
		}

		private const string YES = "yes";

		private const string NO = "no";

		private string version;

		private string encoding;

		private string standalone;
	}
}
