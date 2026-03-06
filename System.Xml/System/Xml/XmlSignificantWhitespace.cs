using System;
using System.Xml.XPath;

namespace System.Xml
{
	/// <summary>Represents white space between markup in a mixed content node or white space within an xml:space= 'preserve' scope. This is also referred to as significant white space.</summary>
	public class XmlSignificantWhitespace : XmlCharacterData
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlSignificantWhitespace" /> class.</summary>
		/// <param name="strData">The white space characters of the node.</param>
		/// <param name="doc">The <see cref="T:System.Xml.XmlDocument" /> object.</param>
		protected internal XmlSignificantWhitespace(string strData, XmlDocument doc) : base(strData, doc)
		{
			if (!doc.IsLoading && !base.CheckOnData(strData))
			{
				throw new ArgumentException(Res.GetString("The string for white space contains an invalid character."));
			}
		}

		/// <summary>Gets the qualified name of the node.</summary>
		/// <returns>For <see langword="XmlSignificantWhitespace" /> nodes, this property returns <see langword="#significant-whitespace" />.</returns>
		public override string Name
		{
			get
			{
				return this.OwnerDocument.strSignificantWhitespaceName;
			}
		}

		/// <summary>Gets the local name of the node.</summary>
		/// <returns>For <see langword="XmlSignificantWhitespace" /> nodes, this property returns <see langword="#significant-whitespace" />.</returns>
		public override string LocalName
		{
			get
			{
				return this.OwnerDocument.strSignificantWhitespaceName;
			}
		}

		/// <summary>Gets the type of the current node.</summary>
		/// <returns>For <see langword="XmlSignificantWhitespace" /> nodes, this value is XmlNodeType.SignificantWhitespace.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.SignificantWhitespace;
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

		/// <summary>Creates a duplicate of this node.</summary>
		/// <param name="deep">
		///       <see langword="true" /> to recursively clone the subtree under the specified node; <see langword="false" /> to clone only the node itself. For significant white space nodes, the cloned node always includes the data value, regardless of the parameter setting. </param>
		/// <returns>The cloned node.</returns>
		public override XmlNode CloneNode(bool deep)
		{
			return this.OwnerDocument.CreateSignificantWhitespace(this.Data);
		}

		/// <summary>Gets or sets the value of the node.</summary>
		/// <returns>The white space characters found in the node.</returns>
		/// <exception cref="T:System.ArgumentException">Setting <see langword="Value" /> to invalid white space characters. </exception>
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

		/// <summary>Saves the node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteTo(XmlWriter w)
		{
			w.WriteString(this.Data);
		}

		/// <summary>Saves all the children of the node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteContentTo(XmlWriter w)
		{
		}

		internal override XPathNodeType XPNodeType
		{
			get
			{
				XPathNodeType result = XPathNodeType.SignificantWhitespace;
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
