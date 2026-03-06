using System;
using System.Xml.XPath;

namespace System.Xml
{
	/// <summary>Represents a lightweight object that is useful for tree insert operations.</summary>
	public class XmlDocumentFragment : XmlNode
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlDocumentFragment" /> class.</summary>
		/// <param name="ownerDocument">The XML document that is the source of the fragment.</param>
		protected internal XmlDocumentFragment(XmlDocument ownerDocument)
		{
			if (ownerDocument == null)
			{
				throw new ArgumentException(Res.GetString("Cannot create a node without an owner document."));
			}
			this.parentNode = ownerDocument;
		}

		/// <summary>Gets the qualified name of the node.</summary>
		/// <returns>For <see langword="XmlDocumentFragment" />, the name is <see langword="#document-fragment" />.</returns>
		public override string Name
		{
			get
			{
				return this.OwnerDocument.strDocumentFragmentName;
			}
		}

		/// <summary>Gets the local name of the node.</summary>
		/// <returns>For <see langword="XmlDocumentFragment" /> nodes, the local name is <see langword="#document-fragment" />.</returns>
		public override string LocalName
		{
			get
			{
				return this.OwnerDocument.strDocumentFragmentName;
			}
		}

		/// <summary>Gets the type of the current node.</summary>
		/// <returns>For <see langword="XmlDocumentFragment" /> nodes, this value is XmlNodeType.DocumentFragment.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.DocumentFragment;
			}
		}

		/// <summary>Gets the parent of this node (for nodes that can have parents).</summary>
		/// <returns>The parent of this node.For <see langword="XmlDocumentFragment" /> nodes, this property is always <see langword="null" />.</returns>
		public override XmlNode ParentNode
		{
			get
			{
				return null;
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.XmlDocument" /> to which this node belongs.</summary>
		/// <returns>The <see langword="XmlDocument" /> to which this node belongs.</returns>
		public override XmlDocument OwnerDocument
		{
			get
			{
				return (XmlDocument)this.parentNode;
			}
		}

		/// <summary>Gets or sets the markup representing the children of this node.</summary>
		/// <returns>The markup of the children of this node.</returns>
		/// <exception cref="T:System.Xml.XmlException">The XML specified when setting this property is not well-formed. </exception>
		public override string InnerXml
		{
			get
			{
				return base.InnerXml;
			}
			set
			{
				this.RemoveAll();
				new XmlLoader().ParsePartialContent(this, value, XmlNodeType.Element);
			}
		}

		/// <summary>Creates a duplicate of this node.</summary>
		/// <param name="deep">
		///       <see langword="true" /> to recursively clone the subtree under the specified node; <see langword="false" /> to clone only the node itself. </param>
		/// <returns>The cloned node.</returns>
		public override XmlNode CloneNode(bool deep)
		{
			XmlDocument ownerDocument = this.OwnerDocument;
			XmlDocumentFragment xmlDocumentFragment = ownerDocument.CreateDocumentFragment();
			if (deep)
			{
				xmlDocumentFragment.CopyChildren(ownerDocument, this, deep);
			}
			return xmlDocumentFragment;
		}

		internal override bool IsContainer
		{
			get
			{
				return true;
			}
		}

		internal override XmlLinkedNode LastNode
		{
			get
			{
				return this.lastChild;
			}
			set
			{
				this.lastChild = value;
			}
		}

		internal override bool IsValidChildType(XmlNodeType type)
		{
			switch (type)
			{
			case XmlNodeType.Element:
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.EntityReference:
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.Comment:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				return true;
			case XmlNodeType.XmlDeclaration:
			{
				XmlNode firstChild = this.FirstChild;
				return firstChild == null || firstChild.NodeType != XmlNodeType.XmlDeclaration;
			}
			}
			return false;
		}

		internal override bool CanInsertAfter(XmlNode newChild, XmlNode refChild)
		{
			return newChild.NodeType != XmlNodeType.XmlDeclaration || (refChild == null && this.LastNode == null);
		}

		internal override bool CanInsertBefore(XmlNode newChild, XmlNode refChild)
		{
			return newChild.NodeType != XmlNodeType.XmlDeclaration || refChild == null || refChild == this.FirstChild;
		}

		/// <summary>Saves the node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteTo(XmlWriter w)
		{
			this.WriteContentTo(w);
		}

		/// <summary>Saves all the children of the node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteContentTo(XmlWriter w)
		{
			foreach (object obj in this)
			{
				((XmlNode)obj).WriteTo(w);
			}
		}

		internal override XPathNodeType XPNodeType
		{
			get
			{
				return XPathNodeType.Root;
			}
		}

		private XmlLinkedNode lastChild;
	}
}
