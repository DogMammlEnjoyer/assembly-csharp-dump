using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace System.Xml
{
	/// <summary>Represents a reader that provides fast, non-cached forward only access to XML data in an <see cref="T:System.Xml.XmlNode" />.</summary>
	public class XmlNodeReader : XmlReader, IXmlNamespaceResolver
	{
		/// <summary>Creates an instance of the <see langword="XmlNodeReader" /> class using the specified <see cref="T:System.Xml.XmlNode" />.</summary>
		/// <param name="node">The <see langword="XmlNode" /> you want to read. </param>
		public XmlNodeReader(XmlNode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			this.readerNav = new XmlNodeReaderNavigator(node);
			this.curDepth = 0;
			this.readState = ReadState.Initial;
			this.fEOF = false;
			this.nodeType = XmlNodeType.None;
			this.bResolveEntity = false;
			this.bStartFromDocument = false;
		}

		internal bool IsInReadingStates()
		{
			return this.readState == ReadState.Interactive;
		}

		/// <summary>Gets the type of the current node.</summary>
		/// <returns>One of the <see cref="T:System.Xml.XmlNodeType" /> values representing the type of the current node.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				if (!this.IsInReadingStates())
				{
					return XmlNodeType.None;
				}
				return this.nodeType;
			}
		}

		/// <summary>Gets the qualified name of the current node.</summary>
		/// <returns>The qualified name of the current node. For example, <see langword="Name" /> is <see langword="bk:book" /> for the element &lt;bk:book&gt;.The name returned is dependent on the <see cref="P:System.Xml.XmlNodeReader.NodeType" /> of the node. The following node types return the listed values. All other node types return an empty string.Node Type Name 
		///             <see langword="Attribute" />
		///           The name of the attribute. 
		///             <see langword="DocumentType" />
		///           The document type name. 
		///             <see langword="Element" />
		///           The tag name. 
		///             <see langword="EntityReference" />
		///           The name of the entity referenced. 
		///             <see langword="ProcessingInstruction" />
		///           The target of the processing instruction. 
		///             <see langword="XmlDeclaration" />
		///           The literal string <see langword="xml" />. </returns>
		public override string Name
		{
			get
			{
				if (!this.IsInReadingStates())
				{
					return string.Empty;
				}
				return this.readerNav.Name;
			}
		}

		/// <summary>Gets the local name of the current node.</summary>
		/// <returns>The name of the current node with the prefix removed. For example, <see langword="LocalName" /> is <see langword="book" /> for the element &lt;bk:book&gt;.For node types that do not have a name (like <see langword="Text" />, <see langword="Comment" />, and so on), this property returns String.Empty.</returns>
		public override string LocalName
		{
			get
			{
				if (!this.IsInReadingStates())
				{
					return string.Empty;
				}
				return this.readerNav.LocalName;
			}
		}

		/// <summary>Gets the namespace URI (as defined in the W3C Namespace specification) of the node on which the reader is positioned.</summary>
		/// <returns>The namespace URI of the current node; otherwise an empty string.</returns>
		public override string NamespaceURI
		{
			get
			{
				if (!this.IsInReadingStates())
				{
					return string.Empty;
				}
				return this.readerNav.NamespaceURI;
			}
		}

		/// <summary>Gets the namespace prefix associated with the current node.</summary>
		/// <returns>The namespace prefix associated with the current node.</returns>
		public override string Prefix
		{
			get
			{
				if (!this.IsInReadingStates())
				{
					return string.Empty;
				}
				return this.readerNav.Prefix;
			}
		}

		/// <summary>Gets a value indicating whether the current node can have a <see cref="P:System.Xml.XmlNodeReader.Value" />.</summary>
		/// <returns>
		///     <see langword="true" /> if the node on which the reader is currently positioned can have a <see langword="Value" />; otherwise, <see langword="false" />.</returns>
		public override bool HasValue
		{
			get
			{
				return this.IsInReadingStates() && this.readerNav.HasValue;
			}
		}

		/// <summary>Gets the text value of the current node.</summary>
		/// <returns>The value returned depends on the <see cref="P:System.Xml.XmlNodeReader.NodeType" /> of the node. The following table lists node types that have a value to return. All other node types return String.Empty.Node Type Value 
		///             <see langword="Attribute" />
		///           The value of the attribute. 
		///             <see langword="CDATA" />
		///           The content of the CDATA section. 
		///             <see langword="Comment" />
		///           The content of the comment. 
		///             <see langword="DocumentType" />
		///           The internal subset. 
		///             <see langword="ProcessingInstruction" />
		///           The entire content, excluding the target. 
		///             <see langword="SignificantWhitespace" />
		///           The white space between markup in a mixed content model. 
		///             <see langword="Text" />
		///           The content of the text node. 
		///             <see langword="Whitespace" />
		///           The white space between markup. 
		///             <see langword="XmlDeclaration" />
		///           The content of the declaration. </returns>
		public override string Value
		{
			get
			{
				if (!this.IsInReadingStates())
				{
					return string.Empty;
				}
				return this.readerNav.Value;
			}
		}

		/// <summary>Gets the depth of the current node in the XML document.</summary>
		/// <returns>The depth of the current node in the XML document.</returns>
		public override int Depth
		{
			get
			{
				return this.curDepth;
			}
		}

		/// <summary>Gets the base URI of the current node.</summary>
		/// <returns>The base URI of the current node.</returns>
		public override string BaseURI
		{
			get
			{
				return this.readerNav.BaseURI;
			}
		}

		/// <summary>Gets a value indicating whether this reader can parse and resolve entities.</summary>
		/// <returns>
		///     <see langword="true" /> if the reader can parse and resolve entities; otherwise, <see langword="false" />. <see langword="XmlNodeReader" /> always returns <see langword="true" />.</returns>
		public override bool CanResolveEntity
		{
			get
			{
				return true;
			}
		}

		/// <summary>Gets a value indicating whether the current node is an empty element (for example, &lt;MyElement/&gt;).</summary>
		/// <returns>
		///     <see langword="true" /> if the current node is an element (<see cref="P:System.Xml.XmlNodeReader.NodeType" /> equals <see langword="XmlNodeType.Element" />) and it ends with /&gt;; otherwise, <see langword="false" />.</returns>
		public override bool IsEmptyElement
		{
			get
			{
				return this.IsInReadingStates() && this.readerNav.IsEmptyElement;
			}
		}

		/// <summary>Gets a value indicating whether the current node is an attribute that was generated from the default value defined in the document type definition (DTD) or schema.</summary>
		/// <returns>
		///     <see langword="true" /> if the current node is an attribute whose value was generated from the default value defined in the DTD or schema; <see langword="false" /> if the attribute value was explicitly set.</returns>
		public override bool IsDefault
		{
			get
			{
				return this.IsInReadingStates() && this.readerNav.IsDefault;
			}
		}

		/// <summary>Gets the current <see langword="xml:space" /> scope.</summary>
		/// <returns>One of the <see cref="T:System.Xml.XmlSpace" /> values. If no <see langword="xml:space" /> scope exists, this property defaults to <see langword="XmlSpace.None" />.</returns>
		public override XmlSpace XmlSpace
		{
			get
			{
				if (!this.IsInReadingStates())
				{
					return XmlSpace.None;
				}
				return this.readerNav.XmlSpace;
			}
		}

		/// <summary>Gets the current <see langword="xml:lang" /> scope.</summary>
		/// <returns>The current <see langword="xml:lang" /> scope.</returns>
		public override string XmlLang
		{
			get
			{
				if (!this.IsInReadingStates())
				{
					return string.Empty;
				}
				return this.readerNav.XmlLang;
			}
		}

		/// <summary>Gets the schema information that has been assigned to the current node.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.IXmlSchemaInfo" /> object containing the schema information for the current node.</returns>
		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				if (!this.IsInReadingStates())
				{
					return null;
				}
				return this.readerNav.SchemaInfo;
			}
		}

		/// <summary>Gets the number of attributes on the current node.</summary>
		/// <returns>The number of attributes on the current node. This number includes default attributes.</returns>
		public override int AttributeCount
		{
			get
			{
				if (!this.IsInReadingStates() || this.nodeType == XmlNodeType.EndElement)
				{
					return 0;
				}
				return this.readerNav.AttributeCount;
			}
		}

		/// <summary>Gets the value of the attribute with the specified name.</summary>
		/// <param name="name">The qualified name of the attribute. </param>
		/// <returns>The value of the specified attribute. If the attribute is not found, <see langword="null" /> is returned.</returns>
		public override string GetAttribute(string name)
		{
			if (!this.IsInReadingStates())
			{
				return null;
			}
			return this.readerNav.GetAttribute(name);
		}

		/// <summary>Gets the value of the attribute with the specified local name and namespace URI.</summary>
		/// <param name="name">The local name of the attribute. </param>
		/// <param name="namespaceURI">The namespace URI of the attribute. </param>
		/// <returns>The value of the specified attribute. If the attribute is not found, <see langword="null" /> is returned.</returns>
		public override string GetAttribute(string name, string namespaceURI)
		{
			if (!this.IsInReadingStates())
			{
				return null;
			}
			string ns = (namespaceURI == null) ? string.Empty : namespaceURI;
			return this.readerNav.GetAttribute(name, ns);
		}

		/// <summary>Gets the value of the attribute with the specified index.</summary>
		/// <param name="attributeIndex">The index of the attribute. The index is zero-based. (The first attribute has index 0.) </param>
		/// <returns>The value of the specified attribute.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="i" /> parameter is less than 0 or greater than or equal to <see cref="P:System.Xml.XmlNodeReader.AttributeCount" />. </exception>
		public override string GetAttribute(int attributeIndex)
		{
			if (!this.IsInReadingStates())
			{
				throw new ArgumentOutOfRangeException("attributeIndex");
			}
			return this.readerNav.GetAttribute(attributeIndex);
		}

		/// <summary>Moves to the attribute with the specified name.</summary>
		/// <param name="name">The qualified name of the attribute. </param>
		/// <returns>
		///     <see langword="true" /> if the attribute is found; otherwise, <see langword="false" />. If <see langword="false" />, the reader's position does not change.</returns>
		public override bool MoveToAttribute(string name)
		{
			if (!this.IsInReadingStates())
			{
				return false;
			}
			this.readerNav.ResetMove(ref this.curDepth, ref this.nodeType);
			if (this.readerNav.MoveToAttribute(name))
			{
				this.curDepth++;
				this.nodeType = this.readerNav.NodeType;
				if (this.bInReadBinary)
				{
					this.FinishReadBinary();
				}
				return true;
			}
			this.readerNav.RollBackMove(ref this.curDepth);
			return false;
		}

		/// <summary>Moves to the attribute with the specified local name and namespace URI.</summary>
		/// <param name="name">The local name of the attribute. </param>
		/// <param name="namespaceURI">The namespace URI of the attribute. </param>
		/// <returns>
		///     <see langword="true" /> if the attribute is found; otherwise, <see langword="false" />. If <see langword="false" />, the reader's position does not change.</returns>
		public override bool MoveToAttribute(string name, string namespaceURI)
		{
			if (!this.IsInReadingStates())
			{
				return false;
			}
			this.readerNav.ResetMove(ref this.curDepth, ref this.nodeType);
			string namespaceURI2 = (namespaceURI == null) ? string.Empty : namespaceURI;
			if (this.readerNav.MoveToAttribute(name, namespaceURI2))
			{
				this.curDepth++;
				this.nodeType = this.readerNav.NodeType;
				if (this.bInReadBinary)
				{
					this.FinishReadBinary();
				}
				return true;
			}
			this.readerNav.RollBackMove(ref this.curDepth);
			return false;
		}

		/// <summary>Moves to the attribute with the specified index.</summary>
		/// <param name="attributeIndex">The index of the attribute. </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="i" /> parameter is less than 0 or greater than or equal to <see cref="P:System.Xml.XmlReader.AttributeCount" />. </exception>
		public override void MoveToAttribute(int attributeIndex)
		{
			if (!this.IsInReadingStates())
			{
				throw new ArgumentOutOfRangeException("attributeIndex");
			}
			this.readerNav.ResetMove(ref this.curDepth, ref this.nodeType);
			try
			{
				if (this.AttributeCount <= 0)
				{
					throw new ArgumentOutOfRangeException("attributeIndex");
				}
				this.readerNav.MoveToAttribute(attributeIndex);
				if (this.bInReadBinary)
				{
					this.FinishReadBinary();
				}
			}
			catch
			{
				this.readerNav.RollBackMove(ref this.curDepth);
				throw;
			}
			this.curDepth++;
			this.nodeType = this.readerNav.NodeType;
		}

		/// <summary>Moves to the first attribute.</summary>
		/// <returns>
		///     <see langword="true" /> if an attribute exists (the reader moves to the first attribute); otherwise, <see langword="false" /> (the position of the reader does not change).</returns>
		public override bool MoveToFirstAttribute()
		{
			if (!this.IsInReadingStates())
			{
				return false;
			}
			this.readerNav.ResetMove(ref this.curDepth, ref this.nodeType);
			if (this.AttributeCount > 0)
			{
				this.readerNav.MoveToAttribute(0);
				this.curDepth++;
				this.nodeType = this.readerNav.NodeType;
				if (this.bInReadBinary)
				{
					this.FinishReadBinary();
				}
				return true;
			}
			this.readerNav.RollBackMove(ref this.curDepth);
			return false;
		}

		/// <summary>Moves to the next attribute.</summary>
		/// <returns>
		///     <see langword="true" /> if there is a next attribute; <see langword="false" /> if there are no more attributes.</returns>
		public override bool MoveToNextAttribute()
		{
			if (!this.IsInReadingStates() || this.nodeType == XmlNodeType.EndElement)
			{
				return false;
			}
			this.readerNav.LogMove(this.curDepth);
			this.readerNav.ResetToAttribute(ref this.curDepth);
			if (this.readerNav.MoveToNextAttribute(ref this.curDepth))
			{
				this.nodeType = this.readerNav.NodeType;
				if (this.bInReadBinary)
				{
					this.FinishReadBinary();
				}
				return true;
			}
			this.readerNav.RollBackMove(ref this.curDepth);
			return false;
		}

		/// <summary>Moves to the element that contains the current attribute node.</summary>
		/// <returns>
		///     <see langword="true" /> if the reader is positioned on an attribute (the reader moves to the element that owns the attribute); <see langword="false" /> if the reader is not positioned on an attribute (the position of the reader does not change).</returns>
		public override bool MoveToElement()
		{
			if (!this.IsInReadingStates())
			{
				return false;
			}
			this.readerNav.LogMove(this.curDepth);
			this.readerNav.ResetToAttribute(ref this.curDepth);
			if (this.readerNav.MoveToElement())
			{
				this.curDepth--;
				this.nodeType = this.readerNav.NodeType;
				if (this.bInReadBinary)
				{
					this.FinishReadBinary();
				}
				return true;
			}
			this.readerNav.RollBackMove(ref this.curDepth);
			return false;
		}

		/// <summary>Reads the next node from the stream.</summary>
		/// <returns>
		///     <see langword="true" /> if the next node was read successfully; <see langword="false" /> if there are no more nodes to read.</returns>
		public override bool Read()
		{
			return this.Read(false);
		}

		private bool Read(bool fSkipChildren)
		{
			if (this.fEOF)
			{
				return false;
			}
			if (this.readState == ReadState.Initial)
			{
				if (this.readerNav.NodeType == XmlNodeType.Document || this.readerNav.NodeType == XmlNodeType.DocumentFragment)
				{
					this.bStartFromDocument = true;
					if (!this.ReadNextNode(fSkipChildren))
					{
						this.readState = ReadState.Error;
						return false;
					}
				}
				this.ReSetReadingMarks();
				this.readState = ReadState.Interactive;
				this.nodeType = this.readerNav.NodeType;
				this.curDepth = 0;
				return true;
			}
			if (this.bInReadBinary)
			{
				this.FinishReadBinary();
			}
			if (this.readerNav.CreatedOnAttribute)
			{
				return false;
			}
			this.ReSetReadingMarks();
			if (this.ReadNextNode(fSkipChildren))
			{
				return true;
			}
			if (this.readState == ReadState.Initial || this.readState == ReadState.Interactive)
			{
				this.readState = ReadState.Error;
			}
			if (this.readState == ReadState.EndOfFile)
			{
				this.nodeType = XmlNodeType.None;
			}
			return false;
		}

		private bool ReadNextNode(bool fSkipChildren)
		{
			if (this.readState != ReadState.Interactive && this.readState != ReadState.Initial)
			{
				this.nodeType = XmlNodeType.None;
				return false;
			}
			bool flag = !fSkipChildren;
			XmlNodeType xmlNodeType = this.readerNav.NodeType;
			if (flag && this.nodeType != XmlNodeType.EndElement && this.nodeType != XmlNodeType.EndEntity && (xmlNodeType == XmlNodeType.Element || (xmlNodeType == XmlNodeType.EntityReference && this.bResolveEntity) || ((this.readerNav.NodeType == XmlNodeType.Document || this.readerNav.NodeType == XmlNodeType.DocumentFragment) && this.readState == ReadState.Initial)))
			{
				if (this.readerNav.MoveToFirstChild())
				{
					this.nodeType = this.readerNav.NodeType;
					this.curDepth++;
					if (this.bResolveEntity)
					{
						this.bResolveEntity = false;
					}
					return true;
				}
				if (this.readerNav.NodeType == XmlNodeType.Element && !this.readerNav.IsEmptyElement)
				{
					this.nodeType = XmlNodeType.EndElement;
					return true;
				}
				if (this.readerNav.NodeType == XmlNodeType.EntityReference && this.bResolveEntity)
				{
					this.bResolveEntity = false;
					this.nodeType = XmlNodeType.EndEntity;
					return true;
				}
				return this.ReadForward(fSkipChildren);
			}
			else
			{
				if (this.readerNav.NodeType == XmlNodeType.EntityReference && this.bResolveEntity)
				{
					if (this.readerNav.MoveToFirstChild())
					{
						this.nodeType = this.readerNav.NodeType;
						this.curDepth++;
					}
					else
					{
						this.nodeType = XmlNodeType.EndEntity;
					}
					this.bResolveEntity = false;
					return true;
				}
				return this.ReadForward(fSkipChildren);
			}
		}

		private void SetEndOfFile()
		{
			this.fEOF = true;
			this.readState = ReadState.EndOfFile;
			this.nodeType = XmlNodeType.None;
		}

		private bool ReadAtZeroLevel(bool fSkipChildren)
		{
			if (!fSkipChildren && this.nodeType != XmlNodeType.EndElement && this.readerNav.NodeType == XmlNodeType.Element && !this.readerNav.IsEmptyElement)
			{
				this.nodeType = XmlNodeType.EndElement;
				return true;
			}
			this.SetEndOfFile();
			return false;
		}

		private bool ReadForward(bool fSkipChildren)
		{
			if (this.readState == ReadState.Error)
			{
				return false;
			}
			if (!this.bStartFromDocument && this.curDepth == 0)
			{
				return this.ReadAtZeroLevel(fSkipChildren);
			}
			if (this.readerNav.MoveToNext())
			{
				this.nodeType = this.readerNav.NodeType;
				return true;
			}
			if (this.curDepth == 0)
			{
				return this.ReadAtZeroLevel(fSkipChildren);
			}
			if (!this.readerNav.MoveToParent())
			{
				return false;
			}
			if (this.readerNav.NodeType == XmlNodeType.Element)
			{
				this.curDepth--;
				this.nodeType = XmlNodeType.EndElement;
				return true;
			}
			if (this.readerNav.NodeType == XmlNodeType.EntityReference)
			{
				this.curDepth--;
				this.nodeType = XmlNodeType.EndEntity;
				return true;
			}
			return true;
		}

		private void ReSetReadingMarks()
		{
			this.readerNav.ResetMove(ref this.curDepth, ref this.nodeType);
		}

		/// <summary>Gets a value indicating whether the reader is positioned at the end of the stream.</summary>
		/// <returns>
		///     <see langword="true" /> if the reader is positioned at the end of the stream; otherwise, <see langword="false" />.</returns>
		public override bool EOF
		{
			get
			{
				return this.readState != ReadState.Closed && this.fEOF;
			}
		}

		/// <summary>Changes the <see cref="P:System.Xml.XmlNodeReader.ReadState" /> to <see langword="Closed" />.</summary>
		public override void Close()
		{
			this.readState = ReadState.Closed;
		}

		/// <summary>Gets the state of the reader.</summary>
		/// <returns>One of the <see cref="T:System.Xml.ReadState" /> values.</returns>
		public override ReadState ReadState
		{
			get
			{
				return this.readState;
			}
		}

		/// <summary>Skips the children of the current node.</summary>
		public override void Skip()
		{
			this.Read(true);
		}

		/// <summary>Reads the contents of an element or text node as a string.</summary>
		/// <returns>The contents of the element or text-like node (This can include CDATA, Text nodes, and so on). This can be an empty string if the reader is positioned on something other than an element or text node, or if there is no more text content to return in the current context.
		///     <see langword="Note:" /> The text node can be either an element or an attribute text node.</returns>
		public override string ReadString()
		{
			if (this.NodeType == XmlNodeType.EntityReference && this.bResolveEntity && !this.Read())
			{
				throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
			}
			return base.ReadString();
		}

		/// <summary>Gets a value indicating whether the current node has any attributes.</summary>
		/// <returns>
		///     <see langword="true" /> if the current node has attributes; otherwise, <see langword="false" />.</returns>
		public override bool HasAttributes
		{
			get
			{
				return this.AttributeCount > 0;
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.XmlNameTable" /> associated with this implementation.</summary>
		/// <returns>The <see langword="XmlNameTable" /> enabling you to get the atomized version of a string within the node.</returns>
		public override XmlNameTable NameTable
		{
			get
			{
				return this.readerNav.NameTable;
			}
		}

		/// <summary>Resolves a namespace prefix in the current element's scope.</summary>
		/// <param name="prefix">The prefix whose namespace URI you want to resolve. To match the default namespace, pass an empty string. This string does not have to be atomized. </param>
		/// <returns>The namespace URI to which the prefix maps or <see langword="null" /> if no matching prefix is found.</returns>
		public override string LookupNamespace(string prefix)
		{
			if (!this.IsInReadingStates())
			{
				return null;
			}
			string text = this.readerNav.LookupNamespace(prefix);
			if (text != null && text.Length == 0)
			{
				return null;
			}
			return text;
		}

		/// <summary>Resolves the entity reference for <see langword="EntityReference" /> nodes.</summary>
		/// <exception cref="T:System.InvalidOperationException">The reader is not positioned on an <see langword="EntityReference" /> node. </exception>
		public override void ResolveEntity()
		{
			if (!this.IsInReadingStates() || this.nodeType != XmlNodeType.EntityReference)
			{
				throw new InvalidOperationException(Res.GetString("The node is not an expandable 'EntityReference' node."));
			}
			this.bResolveEntity = true;
		}

		/// <summary>Parses the attribute value into one or more <see langword="Text" />, <see langword="EntityReference" />, or <see langword="EndEntity" /> nodes.</summary>
		/// <returns>
		///     <see langword="true" /> if there are nodes to return.
		///     <see langword="false" /> if the reader is not positioned on an attribute node when the initial call is made or if all the attribute values have been read.An empty attribute, such as, misc="", returns <see langword="true" /> with a single node with a value of String.Empty.</returns>
		public override bool ReadAttributeValue()
		{
			if (!this.IsInReadingStates())
			{
				return false;
			}
			if (this.readerNav.ReadAttributeValue(ref this.curDepth, ref this.bResolveEntity, ref this.nodeType))
			{
				this.bInReadBinary = false;
				return true;
			}
			return false;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Xml.XmlNodeReader" /> implements the binary content read methods.</summary>
		/// <returns>
		///     <see langword="true" /> if the binary content read methods are implemented; otherwise <see langword="false" />. The <see cref="T:System.Xml.XmlNodeReader" /> class always returns <see langword="true" />.</returns>
		public override bool CanReadBinaryContent
		{
			get
			{
				return true;
			}
		}

		/// <summary>Reads the content and returns the Base64 decoded binary bytes.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <see cref="M:System.Xml.XmlNodeReader.ReadContentAsBase64(System.Byte[],System.Int32,System.Int32)" /> is not supported on the current node.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
		public override int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			if (this.readState != ReadState.Interactive)
			{
				return 0;
			}
			if (!this.bInReadBinary)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
			}
			this.bInReadBinary = false;
			int result = this.readBinaryHelper.ReadContentAsBase64(buffer, index, count);
			this.bInReadBinary = true;
			return result;
		}

		/// <summary>Reads the content and returns the BinHex decoded binary bytes.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <see cref="M:System.Xml.XmlNodeReader.ReadContentAsBinHex(System.Byte[],System.Int32,System.Int32)" />  is not supported on the current node.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (this.readState != ReadState.Interactive)
			{
				return 0;
			}
			if (!this.bInReadBinary)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
			}
			this.bInReadBinary = false;
			int result = this.readBinaryHelper.ReadContentAsBinHex(buffer, index, count);
			this.bInReadBinary = true;
			return result;
		}

		/// <summary>Reads the element and decodes the Base64 content.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The current node is not an element node.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
		/// <exception cref="T:System.Xml.XmlException">The element contains mixed content.</exception>
		/// <exception cref="T:System.FormatException">The content cannot be converted to the requested type.</exception>
		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			if (this.readState != ReadState.Interactive)
			{
				return 0;
			}
			if (!this.bInReadBinary)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
			}
			this.bInReadBinary = false;
			int result = this.readBinaryHelper.ReadElementContentAsBase64(buffer, index, count);
			this.bInReadBinary = true;
			return result;
		}

		/// <summary>Reads the element and decodes the BinHex content.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The current node is not an element node.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
		/// <exception cref="T:System.Xml.XmlException">The element contains mixed content.</exception>
		/// <exception cref="T:System.FormatException">The content cannot be converted to the requested type.</exception>
		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			if (this.readState != ReadState.Interactive)
			{
				return 0;
			}
			if (!this.bInReadBinary)
			{
				this.readBinaryHelper = ReadContentAsBinaryHelper.CreateOrReset(this.readBinaryHelper, this);
			}
			this.bInReadBinary = false;
			int result = this.readBinaryHelper.ReadElementContentAsBinHex(buffer, index, count);
			this.bInReadBinary = true;
			return result;
		}

		private void FinishReadBinary()
		{
			this.bInReadBinary = false;
			this.readBinaryHelper.Finish();
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.IXmlNamespaceResolver.GetNamespacesInScope(System.Xml.XmlNamespaceScope)" />.</summary>
		/// <param name="scope">
		///       <see cref="T:System.Xml.XmlNamespaceScope" /> object.</param>
		/// <returns>
		///     <see cref="T:System.Collections.IDictionary" /> object that contains the namespaces that are in scope.</returns>
		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return this.readerNav.GetNamespacesInScope(scope);
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.IXmlNamespaceResolver.LookupPrefix(System.String)" />.</summary>
		/// <param name="namespaceName">
		///       <see cref="T:System.String" /> object that identifies the namespace.</param>
		/// <returns>
		///     <see cref="T:System.String" /> object that contains the namespace prefix.</returns>
		string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
		{
			return this.readerNav.LookupPrefix(namespaceName);
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.IXmlNamespaceResolver.LookupNamespace(System.String)" />.</summary>
		/// <param name="prefix">
		///       <see cref="T:System.String" /> that contains the namespace prefix.</param>
		/// <returns>
		///     <see cref="T:System.String" /> that contains the namespace name.</returns>
		string IXmlNamespaceResolver.LookupNamespace(string prefix)
		{
			if (!this.IsInReadingStates())
			{
				return this.readerNav.DefaultLookupNamespace(prefix);
			}
			string text = this.readerNav.LookupNamespace(prefix);
			if (text != null)
			{
				text = this.readerNav.NameTable.Add(text);
			}
			return text;
		}

		internal override IDtdInfo DtdInfo
		{
			get
			{
				return this.readerNav.Document.DtdSchemaInfo;
			}
		}

		private XmlNodeReaderNavigator readerNav;

		private XmlNodeType nodeType;

		private int curDepth;

		private ReadState readState;

		private bool fEOF;

		private bool bResolveEntity;

		private bool bStartFromDocument;

		private bool bInReadBinary;

		private ReadContentAsBinaryHelper readBinaryHelper;
	}
}
