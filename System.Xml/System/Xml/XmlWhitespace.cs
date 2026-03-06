using System;
using System.Xml.XPath;

namespace System.Xml
{
	/// <summary>Represents white space in element content.</summary>
	public class XmlWhitespace : XmlCharacterData
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlWhitespace" /> class.</summary>
		/// <param name="strData">The white space characters of the node.</param>
		/// <param name="doc">The <see cref="T:System.Xml.XmlDocument" /> object.</param>
		protected internal XmlWhitespace(string strData, XmlDocument doc) : base(strData, doc)
		{
			if (!doc.IsLoading && !base.CheckOnData(strData))
			{
				throw new ArgumentException(Res.GetString("The string for white space contains an invalid character."));
			}
		}

		/// <summary>Gets the qualified name of the node.</summary>
		/// <returns>For <see langword="XmlWhitespace" /> nodes, this property returns <see langword="#whitespace" />.</returns>
		public override string Name
		{
			get
			{
				return this.OwnerDocument.strNonSignificantWhitespaceName;
			}
		}

		/// <summary>Gets the local name of the node.</summary>
		/// <returns>For <see langword="XmlWhitespace" /> nodes, this property returns <see langword="#whitespace" />.</returns>
		public override string LocalName
		{
			get
			{
				return this.OwnerDocument.strNonSignificantWhitespaceName;
			}
		}

		/// <summary>Gets the type of the node.</summary>
		/// <returns>For <see langword="XmlWhitespace" /> nodes, the value is <see cref="F:System.Xml.XmlNodeType.Whitespace" />.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Whitespace;
			}
		}

		/// <summary>Gets the parent of the current node.</summary>
		/// <returns>The <see cref="T:System.Xml.XmlNode" /> parent node of the current node.</returns>
		public override XmlNode ParentNode
		{
			get
			{
				XmlNodeType nodeType = this.parentNode.NodeType;
				if (nodeType - XmlNodeType.Text > 1)
				{
					if (nodeType == XmlNodeType.Document)
					{
						return base.ParentNode;
					}
					if (nodeType - XmlNodeType.Whitespace > 1)
					{
						return this.parentNode;
					}
				}
				XmlNode parentNode = this.parentNode.parentNode;
				while (parentNode.IsText)
				{
					parentNode = parentNode.parentNode;
				}
				return parentNode;
			}
		}

		/// <summary>Gets or sets the value of the node.</summary>
		/// <returns>The white space characters found in the node.</returns>
		/// <exception cref="T:System.ArgumentException">Setting <see cref="P:System.Xml.XmlWhitespace.Value" /> to invalid white space characters. </exception>
		public override string Value
		{
			get
			{
				return this.Data;
			}
			set
			{
				if (base.CheckOnData(value))
				{
					this.Data = value;
					return;
				}
				throw new ArgumentException(Res.GetString("The string for white space contains an invalid character."));
			}
		}

		/// <summary>Creates a duplicate of this node.</summary>
		/// <param name="deep">
		///       <see langword="true" /> to recursively clone the subtree under the specified node; <see langword="false" /> to clone only the node itself. For white space nodes, the cloned node always includes the data value, regardless of the parameter setting. </param>
		/// <returns>The cloned node.</returns>
		public override XmlNode CloneNode(bool deep)
		{
			return this.OwnerDocument.CreateWhitespace(this.Data);
		}

		/// <summary>Saves the node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see cref="T:System.Xml.XmlWriter" /> to which you want to save.</param>
		public override void WriteTo(XmlWriter w)
		{
			w.WriteWhitespace(this.Data);
		}

		/// <summary>Saves all the children of the node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see cref="T:System.Xml.XmlWriter" /> to which you want to save. </param>
		public override void WriteContentTo(XmlWriter w)
		{
		}

		internal override XPathNodeType XPNodeType
		{
			get
			{
				XPathNodeType result = XPathNodeType.Whitespace;
				base.DecideXPNodeTypeForTextNodes(this, ref result);
				return result;
			}
		}

		internal override bool IsText
		{
			get
			{
				return true;
			}
		}

		/// <summary>Gets the text node that immediately precedes this node.</summary>
		/// <returns>Returns <see cref="T:System.Xml.XmlNode" />.</returns>
		public override XmlNode PreviousText
		{
			get
			{
				if (this.parentNode.IsText)
				{
					return this.parentNode;
				}
				return null;
			}
		}
	}
}
