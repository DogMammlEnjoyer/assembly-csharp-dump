using System;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml
{
	/// <summary>Represents an element.</summary>
	public class XmlElement : XmlLinkedNode
	{
		internal XmlElement(XmlName name, bool empty, XmlDocument doc) : base(doc)
		{
			this.parentNode = null;
			if (!doc.IsLoading)
			{
				XmlDocument.CheckName(name.Prefix);
				XmlDocument.CheckName(name.LocalName);
			}
			if (name.LocalName.Length == 0)
			{
				throw new ArgumentException(Res.GetString("The local name for elements or attributes cannot be null or an empty string."));
			}
			this.name = name;
			if (empty)
			{
				this.lastChild = this;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlElement" /> class.</summary>
		/// <param name="prefix">The namespace prefix; see the <see cref="P:System.Xml.XmlElement.Prefix" /> property.</param>
		/// <param name="localName">The local name; see the <see cref="P:System.Xml.XmlElement.LocalName" /> property.</param>
		/// <param name="namespaceURI">The namespace URI; see the <see cref="P:System.Xml.XmlElement.NamespaceURI" /> property.</param>
		/// <param name="doc">The parent XML document.</param>
		protected internal XmlElement(string prefix, string localName, string namespaceURI, XmlDocument doc) : this(doc.AddXmlName(prefix, localName, namespaceURI, null), true, doc)
		{
		}

		internal XmlName XmlName
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		/// <summary>Creates a duplicate of this node.</summary>
		/// <param name="deep">
		///       <see langword="true" /> to recursively clone the subtree under the specified node; <see langword="false" /> to clone only the node itself (and its attributes if the node is an <see langword="XmlElement" />). </param>
		/// <returns>The cloned node.</returns>
		public override XmlNode CloneNode(bool deep)
		{
			XmlDocument ownerDocument = this.OwnerDocument;
			bool isLoading = ownerDocument.IsLoading;
			ownerDocument.IsLoading = true;
			XmlElement xmlElement = ownerDocument.CreateElement(this.Prefix, this.LocalName, this.NamespaceURI);
			ownerDocument.IsLoading = isLoading;
			if (xmlElement.IsEmpty != this.IsEmpty)
			{
				xmlElement.IsEmpty = this.IsEmpty;
			}
			if (this.HasAttributes)
			{
				foreach (object obj in this.Attributes)
				{
					XmlAttribute xmlAttribute = (XmlAttribute)obj;
					XmlAttribute xmlAttribute2 = (XmlAttribute)xmlAttribute.CloneNode(true);
					if (xmlAttribute is XmlUnspecifiedAttribute && !xmlAttribute.Specified)
					{
						((XmlUnspecifiedAttribute)xmlAttribute2).SetSpecified(false);
					}
					xmlElement.Attributes.InternalAppendAttribute(xmlAttribute2);
				}
			}
			if (deep)
			{
				xmlElement.CopyChildren(ownerDocument, this, deep);
			}
			return xmlElement;
		}

		/// <summary>Gets the qualified name of the node.</summary>
		/// <returns>The qualified name of the node. For <see langword="XmlElement" /> nodes, this is the tag name of the element.</returns>
		public override string Name
		{
			get
			{
				return this.name.Name;
			}
		}

		/// <summary>Gets the local name of the current node.</summary>
		/// <returns>The name of the current node with the prefix removed. For example, <see langword="LocalName" /> is book for the element &lt;bk:book&gt;.</returns>
		public override string LocalName
		{
			get
			{
				return this.name.LocalName;
			}
		}

		/// <summary>Gets the namespace URI of this node.</summary>
		/// <returns>The namespace URI of this node. If there is no namespace URI, this property returns String.Empty.</returns>
		public override string NamespaceURI
		{
			get
			{
				return this.name.NamespaceURI;
			}
		}

		/// <summary>Gets or sets the namespace prefix of this node.</summary>
		/// <returns>The namespace prefix of this node. If there is no prefix, this property returns String.Empty.</returns>
		/// <exception cref="T:System.ArgumentException">This node is read-only </exception>
		/// <exception cref="T:System.Xml.XmlException">The specified prefix contains an invalid character.The specified prefix is malformed.The namespaceURI of this node is <see langword="null" />.The specified prefix is "xml" and the namespaceURI of this node is different from http://www.w3.org/XML/1998/namespace. </exception>
		public override string Prefix
		{
			get
			{
				return this.name.Prefix;
			}
			set
			{
				this.name = this.name.OwnerDocument.AddXmlName(value, this.LocalName, this.NamespaceURI, this.SchemaInfo);
			}
		}

		/// <summary>Gets the type of the current node.</summary>
		/// <returns>The node type. For <see langword="XmlElement" /> nodes, this value is XmlNodeType.Element.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Element;
			}
		}

		/// <summary>Gets the parent of this node (for nodes that can have parents).</summary>
		/// <returns>The <see langword="XmlNode" /> that is the parent of the current node. If a node has just been created and not yet added to the tree, or if it has been removed from the tree, the parent is <see langword="null" />. For all other nodes, the value returned depends on the <see cref="P:System.Xml.XmlNode.NodeType" /> of the node. The following table describes the possible return values for the <see langword="ParentNode" /> property.</returns>
		public override XmlNode ParentNode
		{
			get
			{
				return this.parentNode;
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.XmlDocument" /> to which this node belongs.</summary>
		/// <returns>The <see langword="XmlDocument" /> to which this element belongs.</returns>
		public override XmlDocument OwnerDocument
		{
			get
			{
				return this.name.OwnerDocument;
			}
		}

		internal override bool IsContainer
		{
			get
			{
				return true;
			}
		}

		internal override XmlNode AppendChildForLoad(XmlNode newChild, XmlDocument doc)
		{
			XmlNodeChangedEventArgs insertEventArgsForLoad = doc.GetInsertEventArgsForLoad(newChild, this);
			if (insertEventArgsForLoad != null)
			{
				doc.BeforeEvent(insertEventArgsForLoad);
			}
			XmlLinkedNode xmlLinkedNode = (XmlLinkedNode)newChild;
			if (this.lastChild == null || this.lastChild == this)
			{
				xmlLinkedNode.next = xmlLinkedNode;
				this.lastChild = xmlLinkedNode;
				xmlLinkedNode.SetParentForLoad(this);
			}
			else
			{
				XmlLinkedNode xmlLinkedNode2 = this.lastChild;
				xmlLinkedNode.next = xmlLinkedNode2.next;
				xmlLinkedNode2.next = xmlLinkedNode;
				this.lastChild = xmlLinkedNode;
				if (xmlLinkedNode2.IsText && xmlLinkedNode.IsText)
				{
					XmlNode.NestTextNodes(xmlLinkedNode2, xmlLinkedNode);
				}
				else
				{
					xmlLinkedNode.SetParentForLoad(this);
				}
			}
			if (insertEventArgsForLoad != null)
			{
				doc.AfterEvent(insertEventArgsForLoad);
			}
			return xmlLinkedNode;
		}

		/// <summary>Gets or sets the tag format of the element.</summary>
		/// <returns>Returns <see langword="true" /> if the element is to be serialized in the short tag format "&lt;item/&gt;"; <see langword="false" /> for the long format "&lt;item&gt;&lt;/item&gt;".When setting this property, if set to <see langword="true" />, the children of the element are removed and the element is serialized in the short tag format. If set to <see langword="false" />, the value of the property is changed (regardless of whether or not the element has content); if the element is empty, it is serialized in the long format.This property is a Microsoft extension to the Document Object Model (DOM).</returns>
		public bool IsEmpty
		{
			get
			{
				return this.lastChild == this;
			}
			set
			{
				if (value)
				{
					if (this.lastChild != this)
					{
						this.RemoveAllChildren();
						this.lastChild = this;
						return;
					}
				}
				else if (this.lastChild == this)
				{
					this.lastChild = null;
				}
			}
		}

		internal override XmlLinkedNode LastNode
		{
			get
			{
				if (this.lastChild != this)
				{
					return this.lastChild;
				}
				return null;
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
			}
			return false;
		}

		/// <summary>Gets an <see cref="T:System.Xml.XmlAttributeCollection" /> containing the list of attributes for this node.</summary>
		/// <returns>
		///     <see cref="T:System.Xml.XmlAttributeCollection" /> containing the list of attributes for this node.</returns>
		public override XmlAttributeCollection Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					object objLock = this.OwnerDocument.objLock;
					lock (objLock)
					{
						if (this.attributes == null)
						{
							this.attributes = new XmlAttributeCollection(this);
						}
					}
				}
				return this.attributes;
			}
		}

		/// <summary>Gets a <see langword="boolean" /> value indicating whether the current node has any attributes.</summary>
		/// <returns>
		///     <see langword="true" /> if the current node has attributes; otherwise, <see langword="false" />.</returns>
		public virtual bool HasAttributes
		{
			get
			{
				return this.attributes != null && this.attributes.Count > 0;
			}
		}

		/// <summary>Returns the value for the attribute with the specified name.</summary>
		/// <param name="name">The name of the attribute to retrieve. This is a qualified name. It is matched against the <see langword="Name" /> property of the matching node. </param>
		/// <returns>The value of the specified attribute. An empty string is returned if a matching attribute is not found or if the attribute does not have a specified or default value.</returns>
		public virtual string GetAttribute(string name)
		{
			XmlAttribute attributeNode = this.GetAttributeNode(name);
			if (attributeNode != null)
			{
				return attributeNode.Value;
			}
			return string.Empty;
		}

		/// <summary>Sets the value of the attribute with the specified name.</summary>
		/// <param name="name">The name of the attribute to create or alter. This is a qualified name. If the name contains a colon it is parsed into prefix and local name components. </param>
		/// <param name="value">The value to set for the attribute. </param>
		/// <exception cref="T:System.Xml.XmlException">The specified name contains an invalid character. </exception>
		/// <exception cref="T:System.ArgumentException">The node is read-only. </exception>
		public virtual void SetAttribute(string name, string value)
		{
			XmlAttribute xmlAttribute = this.GetAttributeNode(name);
			if (xmlAttribute == null)
			{
				xmlAttribute = this.OwnerDocument.CreateAttribute(name);
				xmlAttribute.Value = value;
				this.Attributes.InternalAppendAttribute(xmlAttribute);
				return;
			}
			xmlAttribute.Value = value;
		}

		/// <summary>Removes an attribute by name.</summary>
		/// <param name="name">The name of the attribute to remove.This is a qualified name. It is matched against the <see langword="Name" /> property of the matching node. </param>
		/// <exception cref="T:System.ArgumentException">The node is read-only. </exception>
		public virtual void RemoveAttribute(string name)
		{
			if (this.HasAttributes)
			{
				this.Attributes.RemoveNamedItem(name);
			}
		}

		/// <summary>Returns the <see langword="XmlAttribute" /> with the specified name.</summary>
		/// <param name="name">The name of the attribute to retrieve. This is a qualified name. It is matched against the <see langword="Name" /> property of the matching node. </param>
		/// <returns>The specified <see langword="XmlAttribute" /> or <see langword="null" /> if a matching attribute was not found.</returns>
		public virtual XmlAttribute GetAttributeNode(string name)
		{
			if (this.HasAttributes)
			{
				return this.Attributes[name];
			}
			return null;
		}

		/// <summary>Adds the specified <see cref="T:System.Xml.XmlAttribute" />.</summary>
		/// <param name="newAttr">The <see langword="XmlAttribute" /> node to add to the attribute collection for this element. </param>
		/// <returns>If the attribute replaces an existing attribute with the same name, the old <see langword="XmlAttribute" /> is returned; otherwise, <see langword="null" /> is returned.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="newAttr" /> was created from a different document than the one that created this node. Or this node is read-only. </exception>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="newAttr" /> is already an attribute of another <see langword="XmlElement" /> object. You must explicitly clone <see langword="XmlAttribute" /> nodes to re-use them in other <see langword="XmlElement" /> objects. </exception>
		public virtual XmlAttribute SetAttributeNode(XmlAttribute newAttr)
		{
			if (newAttr.OwnerElement != null)
			{
				throw new InvalidOperationException(Res.GetString("The 'Attribute' node cannot be inserted because it is already an attribute of another element."));
			}
			return (XmlAttribute)this.Attributes.SetNamedItem(newAttr);
		}

		/// <summary>Removes the specified <see cref="T:System.Xml.XmlAttribute" />.</summary>
		/// <param name="oldAttr">The <see langword="XmlAttribute" /> node to remove. If the removed attribute has a default value, it is immediately replaced. </param>
		/// <returns>The removed <see langword="XmlAttribute" /> or <see langword="null" /> if <paramref name="oldAttr" /> is not an attribute node of the <see langword="XmlElement" />.</returns>
		/// <exception cref="T:System.ArgumentException">This node is read-only. </exception>
		public virtual XmlAttribute RemoveAttributeNode(XmlAttribute oldAttr)
		{
			if (this.HasAttributes)
			{
				return this.Attributes.Remove(oldAttr);
			}
			return null;
		}

		/// <summary>Returns an <see cref="T:System.Xml.XmlNodeList" /> containing a list of all descendant elements that match the specified <see cref="P:System.Xml.XmlElement.Name" />.</summary>
		/// <param name="name">The name tag to match. This is a qualified name. It is matched against the <see langword="Name" /> property of the matching node. The asterisk (*) is a special value that matches all tags. </param>
		/// <returns>An <see cref="T:System.Xml.XmlNodeList" /> containing a list of all matching nodes. The list is empty if there are no matching nodes.</returns>
		public virtual XmlNodeList GetElementsByTagName(string name)
		{
			return new XmlElementList(this, name);
		}

		/// <summary>Returns the value for the attribute with the specified local name and namespace URI.</summary>
		/// <param name="localName">The local name of the attribute to retrieve. </param>
		/// <param name="namespaceURI">The namespace URI of the attribute to retrieve. </param>
		/// <returns>The value of the specified attribute. An empty string is returned if a matching attribute is not found or if the attribute does not have a specified or default value.</returns>
		public virtual string GetAttribute(string localName, string namespaceURI)
		{
			XmlAttribute attributeNode = this.GetAttributeNode(localName, namespaceURI);
			if (attributeNode != null)
			{
				return attributeNode.Value;
			}
			return string.Empty;
		}

		/// <summary>Sets the value of the attribute with the specified local name and namespace URI.</summary>
		/// <param name="localName">The local name of the attribute. </param>
		/// <param name="namespaceURI">The namespace URI of the attribute. </param>
		/// <param name="value">The value to set for the attribute. </param>
		/// <returns>The attribute value.</returns>
		public virtual string SetAttribute(string localName, string namespaceURI, string value)
		{
			XmlAttribute xmlAttribute = this.GetAttributeNode(localName, namespaceURI);
			if (xmlAttribute == null)
			{
				xmlAttribute = this.OwnerDocument.CreateAttribute(string.Empty, localName, namespaceURI);
				xmlAttribute.Value = value;
				this.Attributes.InternalAppendAttribute(xmlAttribute);
			}
			else
			{
				xmlAttribute.Value = value;
			}
			return value;
		}

		/// <summary>Removes an attribute with the specified local name and namespace URI. (If the removed attribute has a default value, it is immediately replaced).</summary>
		/// <param name="localName">The local name of the attribute to remove. </param>
		/// <param name="namespaceURI">The namespace URI of the attribute to remove. </param>
		/// <exception cref="T:System.ArgumentException">The node is read-only. </exception>
		public virtual void RemoveAttribute(string localName, string namespaceURI)
		{
			this.RemoveAttributeNode(localName, namespaceURI);
		}

		/// <summary>Returns the <see cref="T:System.Xml.XmlAttribute" /> with the specified local name and namespace URI.</summary>
		/// <param name="localName">The local name of the attribute. </param>
		/// <param name="namespaceURI">The namespace URI of the attribute. </param>
		/// <returns>The specified <see langword="XmlAttribute" /> or <see langword="null" /> if a matching attribute was not found.</returns>
		public virtual XmlAttribute GetAttributeNode(string localName, string namespaceURI)
		{
			if (this.HasAttributes)
			{
				return this.Attributes[localName, namespaceURI];
			}
			return null;
		}

		/// <summary>Adds the specified <see cref="T:System.Xml.XmlAttribute" />.</summary>
		/// <param name="localName">The local name of the attribute. </param>
		/// <param name="namespaceURI">The namespace URI of the attribute. </param>
		/// <returns>The <see langword="XmlAttribute" /> to add.</returns>
		public virtual XmlAttribute SetAttributeNode(string localName, string namespaceURI)
		{
			XmlAttribute xmlAttribute = this.GetAttributeNode(localName, namespaceURI);
			if (xmlAttribute == null)
			{
				xmlAttribute = this.OwnerDocument.CreateAttribute(string.Empty, localName, namespaceURI);
				this.Attributes.InternalAppendAttribute(xmlAttribute);
			}
			return xmlAttribute;
		}

		/// <summary>Removes the <see cref="T:System.Xml.XmlAttribute" /> specified by the local name and namespace URI. (If the removed attribute has a default value, it is immediately replaced).</summary>
		/// <param name="localName">The local name of the attribute. </param>
		/// <param name="namespaceURI">The namespace URI of the attribute. </param>
		/// <returns>The removed <see langword="XmlAttribute" /> or <see langword="null" /> if the <see langword="XmlElement" /> does not have a matching attribute node.</returns>
		/// <exception cref="T:System.ArgumentException">This node is read-only. </exception>
		public virtual XmlAttribute RemoveAttributeNode(string localName, string namespaceURI)
		{
			if (this.HasAttributes)
			{
				XmlAttribute attributeNode = this.GetAttributeNode(localName, namespaceURI);
				this.Attributes.Remove(attributeNode);
				return attributeNode;
			}
			return null;
		}

		/// <summary>Returns an <see cref="T:System.Xml.XmlNodeList" /> containing a list of all descendant elements that match the specified <see cref="P:System.Xml.XmlElement.LocalName" /> and <see cref="P:System.Xml.XmlElement.NamespaceURI" />.</summary>
		/// <param name="localName">The local name to match. The asterisk (*) is a special value that matches all tags. </param>
		/// <param name="namespaceURI">The namespace URI to match. </param>
		/// <returns>An <see cref="T:System.Xml.XmlNodeList" /> containing a list of all matching nodes. The list is empty if there are no matching nodes.</returns>
		public virtual XmlNodeList GetElementsByTagName(string localName, string namespaceURI)
		{
			return new XmlElementList(this, localName, namespaceURI);
		}

		/// <summary>Determines whether the current node has an attribute with the specified name.</summary>
		/// <param name="name">The name of the attribute to find. This is a qualified name. It is matched against the <see langword="Name" /> property of the matching node. </param>
		/// <returns>
		///     <see langword="true" /> if the current node has the specified attribute; otherwise, <see langword="false" />.</returns>
		public virtual bool HasAttribute(string name)
		{
			return this.GetAttributeNode(name) != null;
		}

		/// <summary>Determines whether the current node has an attribute with the specified local name and namespace URI.</summary>
		/// <param name="localName">The local name of the attribute to find. </param>
		/// <param name="namespaceURI">The namespace URI of the attribute to find. </param>
		/// <returns>
		///     <see langword="true" /> if the current node has the specified attribute; otherwise, <see langword="false" />.</returns>
		public virtual bool HasAttribute(string localName, string namespaceURI)
		{
			return this.GetAttributeNode(localName, namespaceURI) != null;
		}

		/// <summary>Saves the current node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteTo(XmlWriter w)
		{
			if (base.GetType() == typeof(XmlElement))
			{
				XmlElement.WriteElementTo(w, this);
				return;
			}
			this.WriteStartElement(w);
			if (this.IsEmpty)
			{
				w.WriteEndElement();
				return;
			}
			this.WriteContentTo(w);
			w.WriteFullEndElement();
		}

		private static void WriteElementTo(XmlWriter writer, XmlElement e)
		{
			XmlNode xmlNode = e;
			XmlNode xmlNode2 = e;
			for (;;)
			{
				e = (xmlNode2 as XmlElement);
				if (e != null && e.GetType() == typeof(XmlElement))
				{
					e.WriteStartElement(writer);
					if (e.IsEmpty)
					{
						writer.WriteEndElement();
					}
					else
					{
						if (e.lastChild != null)
						{
							xmlNode2 = e.FirstChild;
							continue;
						}
						writer.WriteFullEndElement();
					}
				}
				else
				{
					xmlNode2.WriteTo(writer);
				}
				while (xmlNode2 != xmlNode && xmlNode2 == xmlNode2.ParentNode.LastChild)
				{
					xmlNode2 = xmlNode2.ParentNode;
					writer.WriteFullEndElement();
				}
				if (xmlNode2 == xmlNode)
				{
					break;
				}
				xmlNode2 = xmlNode2.NextSibling;
			}
		}

		private void WriteStartElement(XmlWriter w)
		{
			w.WriteStartElement(this.Prefix, this.LocalName, this.NamespaceURI);
			if (this.HasAttributes)
			{
				XmlAttributeCollection xmlAttributeCollection = this.Attributes;
				for (int i = 0; i < xmlAttributeCollection.Count; i++)
				{
					xmlAttributeCollection[i].WriteTo(w);
				}
			}
		}

		/// <summary>Saves all the children of the node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteContentTo(XmlWriter w)
		{
			for (XmlNode xmlNode = this.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				xmlNode.WriteTo(w);
			}
		}

		/// <summary>Removes the attribute node with the specified index from the element. (If the removed attribute has a default value, it is immediately replaced).</summary>
		/// <param name="i">The index of the node to remove. The first node has index 0. </param>
		/// <returns>The attribute node removed or <see langword="null" /> if there is no node at the given index.</returns>
		public virtual XmlNode RemoveAttributeAt(int i)
		{
			if (this.HasAttributes)
			{
				return this.attributes.RemoveAt(i);
			}
			return null;
		}

		/// <summary>Removes all specified attributes from the element. Default attributes are not removed.</summary>
		public virtual void RemoveAllAttributes()
		{
			if (this.HasAttributes)
			{
				this.attributes.RemoveAll();
			}
		}

		/// <summary>Removes all specified attributes and children of the current node. Default attributes are not removed.</summary>
		public override void RemoveAll()
		{
			base.RemoveAll();
			this.RemoveAllAttributes();
		}

		internal void RemoveAllChildren()
		{
			base.RemoveAll();
		}

		/// <summary>Gets the post schema validation infoset that has been assigned to this node as a result of schema validation.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.IXmlSchemaInfo" /> object containing the post schema validation infoset of this node.</returns>
		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>Gets or sets the markup representing just the children of this node.</summary>
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
				this.RemoveAllChildren();
				new XmlLoader().LoadInnerXmlElement(this, value);
			}
		}

		/// <summary>Gets or sets the concatenated values of the node and all its children.</summary>
		/// <returns>The concatenated values of the node and all its children.</returns>
		public override string InnerText
		{
			get
			{
				return base.InnerText;
			}
			set
			{
				XmlLinkedNode lastNode = this.LastNode;
				if (lastNode != null && lastNode.NodeType == XmlNodeType.Text && lastNode.next == lastNode)
				{
					lastNode.Value = value;
					return;
				}
				this.RemoveAllChildren();
				this.AppendChild(this.OwnerDocument.CreateTextNode(value));
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.XmlNode" /> immediately following this element.</summary>
		/// <returns>The <see langword="XmlNode" /> immediately following this element.</returns>
		public override XmlNode NextSibling
		{
			get
			{
				if (this.parentNode != null && this.parentNode.LastNode != this)
				{
					return this.next;
				}
				return null;
			}
		}

		internal override void SetParent(XmlNode node)
		{
			this.parentNode = node;
		}

		internal override XPathNodeType XPNodeType
		{
			get
			{
				return XPathNodeType.Element;
			}
		}

		internal override string XPLocalName
		{
			get
			{
				return this.LocalName;
			}
		}

		internal override string GetXPAttribute(string localName, string ns)
		{
			if (ns == this.OwnerDocument.strReservedXmlns)
			{
				return null;
			}
			XmlAttribute attributeNode = this.GetAttributeNode(localName, ns);
			if (attributeNode != null)
			{
				return attributeNode.Value;
			}
			return string.Empty;
		}

		private XmlName name;

		private XmlAttributeCollection attributes;

		private XmlLinkedNode lastChild;
	}
}
