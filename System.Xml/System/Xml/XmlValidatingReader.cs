using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Xml.Schema;

namespace System.Xml
{
	/// <summary>Represents a reader that provides document type definition (DTD), XML-Data Reduced (XDR) schema, and XML Schema definition language (XSD) validation.This class is obsolete. Starting with the .NET Framework 2.0, we recommend that you use the <see cref="T:System.Xml.XmlReaderSettings" /> class and the <see cref="Overload:System.Xml.XmlReader.Create" /> method to create a validating XML reader.</summary>
	[Obsolete("Use XmlReader created by XmlReader.Create() method using appropriate XmlReaderSettings instead. https://go.microsoft.com/fwlink/?linkid=14202")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class XmlValidatingReader : XmlReader, IXmlLineInfo, IXmlNamespaceResolver
	{
		/// <summary>Initializes a new instance of the <see langword="XmlValidatingReader" /> class that validates the content returned from the given <see cref="T:System.Xml.XmlReader" />.</summary>
		/// <param name="reader">The <see langword="XmlReader" /> to read from while validating. The current implementation supports only <see cref="T:System.Xml.XmlTextReader" />. </param>
		/// <exception cref="T:System.ArgumentException">The reader specified is not an <see langword="XmlTextReader" />. </exception>
		public XmlValidatingReader(XmlReader reader)
		{
			this.impl = new XmlValidatingReaderImpl(reader);
			this.impl.OuterReader = this;
		}

		/// <summary>Initializes a new instance of the <see langword="XmlValidatingReader" /> class with the specified values.</summary>
		/// <param name="xmlFragment">The string containing the XML fragment to parse. </param>
		/// <param name="fragType">The <see cref="T:System.Xml.XmlNodeType" /> of the XML fragment. This also determines what the fragment string can contain (see table below). </param>
		/// <param name="context">The <see cref="T:System.Xml.XmlParserContext" /> in which the XML fragment is to be parsed. This includes the <see cref="T:System.Xml.NameTable" /> to use, encoding, namespace scope, current xml:lang, and xml:space scope. </param>
		/// <exception cref="T:System.Xml.XmlException">
		///         <paramref name="fragType" /> is not one of the node types listed in the table below. </exception>
		public XmlValidatingReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context)
		{
			if (xmlFragment == null)
			{
				throw new ArgumentNullException("xmlFragment");
			}
			this.impl = new XmlValidatingReaderImpl(xmlFragment, fragType, context);
			this.impl.OuterReader = this;
		}

		/// <summary>Initializes a new instance of the <see langword="XmlValidatingReader" /> class with the specified values.</summary>
		/// <param name="xmlFragment">The stream containing the XML fragment to parse. </param>
		/// <param name="fragType">The <see cref="T:System.Xml.XmlNodeType" /> of the XML fragment. This determines what the fragment can contain (see table below). </param>
		/// <param name="context">The <see cref="T:System.Xml.XmlParserContext" /> in which the XML fragment is to be parsed. This includes the <see cref="T:System.Xml.XmlNameTable" /> to use, encoding, namespace scope, current <see langword="xml:lang" />, and <see langword="xml:space" /> scope. </param>
		/// <exception cref="T:System.Xml.XmlException">
		///         <paramref name="fragType" /> is not one of the node types listed in the table below. </exception>
		public XmlValidatingReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context)
		{
			if (xmlFragment == null)
			{
				throw new ArgumentNullException("xmlFragment");
			}
			this.impl = new XmlValidatingReaderImpl(xmlFragment, fragType, context);
			this.impl.OuterReader = this;
		}

		/// <summary>Gets the type of the current node.</summary>
		/// <returns>One of the <see cref="T:System.Xml.XmlNodeType" /> values representing the type of the current node.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return this.impl.NodeType;
			}
		}

		/// <summary>Gets the qualified name of the current node.</summary>
		/// <returns>The qualified name of the current node. For example, <see langword="Name" /> is <see langword="bk:book" /> for the element &lt;bk:book&gt;.The name returned is dependent on the <see cref="P:System.Xml.XmlValidatingReader.NodeType" /> of the node. The following node types return the listed values. All other node types return an empty string.Node Type Name 
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
				return this.impl.Name;
			}
		}

		/// <summary>Gets the local name of the current node.</summary>
		/// <returns>The name of the current node with the prefix removed. For example, <see langword="LocalName" /> is <see langword="book" /> for the element &lt;bk:book&gt;.For node types that do not have a name (like <see langword="Text" />, <see langword="Comment" />, and so on), this property returns String.Empty.</returns>
		public override string LocalName
		{
			get
			{
				return this.impl.LocalName;
			}
		}

		/// <summary>Gets the namespace Uniform Resource Identifier (URI) (as defined in the World Wide Web Consortium (W3C) Namespace specification) of the node on which the reader is positioned.</summary>
		/// <returns>The namespace URI of the current node; otherwise an empty string.</returns>
		public override string NamespaceURI
		{
			get
			{
				return this.impl.NamespaceURI;
			}
		}

		/// <summary>Gets the namespace prefix associated with the current node.</summary>
		/// <returns>The namespace prefix associated with the current node.</returns>
		public override string Prefix
		{
			get
			{
				return this.impl.Prefix;
			}
		}

		/// <summary>Gets a value indicating whether the current node can have a <see cref="P:System.Xml.XmlValidatingReader.Value" /> other than String.Empty.</summary>
		/// <returns>
		///     <see langword="true" /> if the node on which the reader is currently positioned can have a <see langword="Value" />; otherwise, <see langword="false" />.</returns>
		public override bool HasValue
		{
			get
			{
				return this.impl.HasValue;
			}
		}

		/// <summary>Gets the text value of the current node.</summary>
		/// <returns>The value returned depends on the <see cref="P:System.Xml.XmlValidatingReader.NodeType" /> of the node. The following table lists node types that have a value to return. All other node types return String.Empty.Node Type Value 
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
				return this.impl.Value;
			}
		}

		/// <summary>Gets the depth of the current node in the XML document.</summary>
		/// <returns>The depth of the current node in the XML document.</returns>
		public override int Depth
		{
			get
			{
				return this.impl.Depth;
			}
		}

		/// <summary>Gets the base URI of the current node.</summary>
		/// <returns>The base URI of the current node.</returns>
		public override string BaseURI
		{
			get
			{
				return this.impl.BaseURI;
			}
		}

		/// <summary>Gets a value indicating whether the current node is an empty element (for example, &lt;MyElement/&gt;).</summary>
		/// <returns>
		///     <see langword="true" /> if the current node is an element (<see cref="P:System.Xml.XmlValidatingReader.NodeType" /> equals <see langword="XmlNodeType.Element" />) that ends with /&gt;; otherwise, <see langword="false" />.</returns>
		public override bool IsEmptyElement
		{
			get
			{
				return this.impl.IsEmptyElement;
			}
		}

		/// <summary>Gets a value indicating whether the current node is an attribute that was generated from the default value defined in the document type definition (DTD) or schema.</summary>
		/// <returns>
		///     <see langword="true" /> if the current node is an attribute whose value was generated from the default value defined in the DTD or schema; <see langword="false" /> if the attribute value was explicitly set.</returns>
		public override bool IsDefault
		{
			get
			{
				return this.impl.IsDefault;
			}
		}

		/// <summary>Gets the quotation mark character used to enclose the value of an attribute node.</summary>
		/// <returns>The quotation mark character (" or ') used to enclose the value of an attribute node.</returns>
		public override char QuoteChar
		{
			get
			{
				return this.impl.QuoteChar;
			}
		}

		/// <summary>Gets the current <see langword="xml:space" /> scope.</summary>
		/// <returns>One of the <see cref="T:System.Xml.XmlSpace" /> values. If no <see langword="xml:space" /> scope exists, this property defaults to <see langword="XmlSpace.None" />.</returns>
		public override XmlSpace XmlSpace
		{
			get
			{
				return this.impl.XmlSpace;
			}
		}

		/// <summary>Gets the current <see langword="xml:lang" /> scope.</summary>
		/// <returns>The current <see langword="xml:lang" /> scope.</returns>
		public override string XmlLang
		{
			get
			{
				return this.impl.XmlLang;
			}
		}

		/// <summary>Gets the number of attributes on the current node.</summary>
		/// <returns>The number of attributes on the current node. This number includes default attributes.</returns>
		public override int AttributeCount
		{
			get
			{
				return this.impl.AttributeCount;
			}
		}

		/// <summary>Gets the value of the attribute with the specified name.</summary>
		/// <param name="name">The qualified name of the attribute. </param>
		/// <returns>The value of the specified attribute. If the attribute is not found, <see langword="null" /> is returned.</returns>
		public override string GetAttribute(string name)
		{
			return this.impl.GetAttribute(name);
		}

		/// <summary>Gets the value of the attribute with the specified local name and namespace Uniform Resource Identifier (URI).</summary>
		/// <param name="localName">The local name of the attribute. </param>
		/// <param name="namespaceURI">The namespace URI of the attribute. </param>
		/// <returns>The value of the specified attribute. If the attribute is not found, <see langword="null" /> is returned. This method does not move the reader.</returns>
		public override string GetAttribute(string localName, string namespaceURI)
		{
			return this.impl.GetAttribute(localName, namespaceURI);
		}

		/// <summary>Gets the value of the attribute with the specified index.</summary>
		/// <param name="i">The index of the attribute. The index is zero-based. (The first attribute has index 0.) </param>
		/// <returns>The value of the specified attribute.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="i" /> parameter is less than 0 or greater than or equal to <see cref="P:System.Xml.XmlValidatingReader.AttributeCount" />. </exception>
		public override string GetAttribute(int i)
		{
			return this.impl.GetAttribute(i);
		}

		/// <summary>Moves to the attribute with the specified name.</summary>
		/// <param name="name">The qualified name of the attribute. </param>
		/// <returns>
		///     <see langword="true" /> if the attribute is found; otherwise, <see langword="false" />. If <see langword="false" />, the position of the reader does not change.</returns>
		public override bool MoveToAttribute(string name)
		{
			return this.impl.MoveToAttribute(name);
		}

		/// <summary>Moves to the attribute with the specified local name and namespace Uniform Resource Identifier (URI).</summary>
		/// <param name="localName">The local name of the attribute. </param>
		/// <param name="namespaceURI">The namespace URI of the attribute. </param>
		/// <returns>
		///     <see langword="true" /> if the attribute is found; otherwise, <see langword="false" />. If <see langword="false" />, the position of the reader does not change.</returns>
		public override bool MoveToAttribute(string localName, string namespaceURI)
		{
			return this.impl.MoveToAttribute(localName, namespaceURI);
		}

		/// <summary>Moves to the attribute with the specified index.</summary>
		/// <param name="i">The index of the attribute. </param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="i" /> parameter is less than 0 or greater than or equal to <see cref="P:System.Xml.XmlReader.AttributeCount" />. </exception>
		public override void MoveToAttribute(int i)
		{
			this.impl.MoveToAttribute(i);
		}

		/// <summary>Moves to the first attribute.</summary>
		/// <returns>
		///     <see langword="true" /> if an attribute exists (the reader moves to the first attribute); otherwise, <see langword="false" /> (the position of the reader does not change).</returns>
		public override bool MoveToFirstAttribute()
		{
			return this.impl.MoveToFirstAttribute();
		}

		/// <summary>Moves to the next attribute.</summary>
		/// <returns>
		///     <see langword="true" /> if there is a next attribute; <see langword="false" /> if there are no more attributes.</returns>
		public override bool MoveToNextAttribute()
		{
			return this.impl.MoveToNextAttribute();
		}

		/// <summary>Moves to the element that contains the current attribute node.</summary>
		/// <returns>
		///     <see langword="true" /> if the reader is positioned on an attribute (the reader moves to the element that owns the attribute); <see langword="false" /> if the reader is not positioned on an attribute (the position of the reader does not change).</returns>
		public override bool MoveToElement()
		{
			return this.impl.MoveToElement();
		}

		/// <summary>Parses the attribute value into one or more <see langword="Text" />, <see langword="EntityReference" />, or <see langword="EndEntity" /> nodes.</summary>
		/// <returns>
		///     <see langword="true" /> if there are nodes to return.
		///     <see langword="false" /> if the reader is not positioned on an attribute node when the initial call is made or if all the attribute values have been read.An empty attribute, such as, misc="", returns <see langword="true" /> with a single node with a value of String.Empty.</returns>
		public override bool ReadAttributeValue()
		{
			return this.impl.ReadAttributeValue();
		}

		/// <summary>Reads the next node from the stream.</summary>
		/// <returns>
		///     <see langword="true" /> if the next node was read successfully; <see langword="false" /> if there are no more nodes to read.</returns>
		public override bool Read()
		{
			return this.impl.Read();
		}

		/// <summary>Gets a value indicating whether the reader is positioned at the end of the stream.</summary>
		/// <returns>
		///     <see langword="true" /> if the reader is positioned at the end of the stream; otherwise, <see langword="false" />.</returns>
		public override bool EOF
		{
			get
			{
				return this.impl.EOF;
			}
		}

		/// <summary>Changes the <see cref="P:System.Xml.XmlReader.ReadState" /> to Closed.</summary>
		public override void Close()
		{
			this.impl.Close();
		}

		/// <summary>Gets the state of the reader.</summary>
		/// <returns>One of the <see cref="T:System.Xml.ReadState" /> values.</returns>
		public override ReadState ReadState
		{
			get
			{
				return this.impl.ReadState;
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.XmlNameTable" /> associated with this implementation.</summary>
		/// <returns>
		///     <see langword="XmlNameTable" /> that enables you to get the atomized version of a string within the node.</returns>
		public override XmlNameTable NameTable
		{
			get
			{
				return this.impl.NameTable;
			}
		}

		/// <summary>Resolves a namespace prefix in the current element's scope.</summary>
		/// <param name="prefix">The prefix whose namespace Uniform Resource Identifier (URI) you want to resolve. To match the default namespace, pass an empty string. </param>
		/// <returns>The namespace URI to which the prefix maps or <see langword="null" /> if no matching prefix is found.</returns>
		public override string LookupNamespace(string prefix)
		{
			string text = this.impl.LookupNamespace(prefix);
			if (text != null && text.Length == 0)
			{
				text = null;
			}
			return text;
		}

		/// <summary>Gets a value indicating whether this reader can parse and resolve entities.</summary>
		/// <returns>
		///     <see langword="true" /> if the reader can parse and resolve entities; otherwise, <see langword="false" />. <see langword="XmlValidatingReader" /> always returns <see langword="true" />.</returns>
		public override bool CanResolveEntity
		{
			get
			{
				return true;
			}
		}

		/// <summary>Resolves the entity reference for <see langword="EntityReference" /> nodes.</summary>
		/// <exception cref="T:System.InvalidOperationException">The reader is not positioned on an <see langword="EntityReference" /> node. </exception>
		public override void ResolveEntity()
		{
			this.impl.ResolveEntity();
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Xml.XmlValidatingReader" /> implements the binary content read methods.</summary>
		/// <returns>
		///     <see langword="true" /> if the binary content read methods are implemented; otherwise <see langword="false" />. The <see cref="T:System.Xml.XmlValidatingReader" /> class returns <see langword="true" />.</returns>
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
		///         <see cref="M:System.Xml.XmlValidatingReader.ReadContentAsBase64(System.Byte[],System.Int32,System.Int32)" />  is not supported on the current node.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
		public override int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			return this.impl.ReadContentAsBase64(buffer, index, count);
		}

		/// <summary>Reads the element and decodes the Base64 content.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The current node is not an element node.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Xml.XmlValidatingReader" /> implementation does not support this method.</exception>
		/// <exception cref="T:System.Xml.XmlException">The element contains mixed-content.</exception>
		/// <exception cref="T:System.FormatException">The content cannot be converted to the requested type.</exception>
		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			return this.impl.ReadElementContentAsBase64(buffer, index, count);
		}

		/// <summary>Reads the content and returns the BinHex decoded binary bytes.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <see cref="M:System.Xml.XmlValidatingReader.ReadContentAsBinHex(System.Byte[],System.Int32,System.Int32)" />  is not supported on the current node.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Xml.XmlValidatingReader" /> implementation does not support this method.</exception>
		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			return this.impl.ReadContentAsBinHex(buffer, index, count);
		}

		/// <summary>Reads the element and decodes the BinHex content.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The current node is not an element node.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Xml.XmlValidatingReader" /> implementation does not support this method.</exception>
		/// <exception cref="T:System.Xml.XmlException">The element contains mixed-content.</exception>
		/// <exception cref="T:System.FormatException">The content cannot be converted to the requested type.</exception>
		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			return this.impl.ReadElementContentAsBinHex(buffer, index, count);
		}

		/// <summary>Reads the contents of an element or text node as a string.</summary>
		/// <returns>The contents of the element or text node. This can be an empty string if the reader is positioned on something other than an element or text node, or if there is no more text content to return in the current context.The text node can be either an element or an attribute text node.</returns>
		public override string ReadString()
		{
			this.impl.MoveOffEntityReference();
			return base.ReadString();
		}

		/// <summary>Gets a value indicating whether the class can return line information.</summary>
		/// <returns>
		///     <see langword="true" /> if the class can return line information; otherwise, <see langword="false" />.</returns>
		public bool HasLineInfo()
		{
			return true;
		}

		/// <summary>Gets the current line number.</summary>
		/// <returns>The current line number. The starting value for this property is 1.</returns>
		public int LineNumber
		{
			get
			{
				return this.impl.LineNumber;
			}
		}

		/// <summary>Gets the current line position.</summary>
		/// <returns>The current line position. The starting value for this property is 1.</returns>
		public int LinePosition
		{
			get
			{
				return this.impl.LinePosition;
			}
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.IXmlNamespaceResolver.GetNamespacesInScope(System.Xml.XmlNamespaceScope)" />.</summary>
		/// <param name="scope">An <see cref="T:System.Xml.XmlNamespaceScope" /> object that identifies the scope of the reader.</param>
		/// <returns>An T:System.Collections.IDictionary object that identifies the namespaces in scope.</returns>
		IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
		{
			return this.impl.GetNamespacesInScope(scope);
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.IXmlNamespaceResolver.LookupNamespace(System.String)" />.</summary>
		/// <param name="prefix">The namespace prefix.</param>
		/// <returns>A string value that contains the namespace Uri that is associated with the prefix.</returns>
		string IXmlNamespaceResolver.LookupNamespace(string prefix)
		{
			return this.impl.LookupNamespace(prefix);
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.IXmlNamespaceResolver.LookupPrefix(System.String)" />.</summary>
		/// <param name="namespaceName">The namespace that is associated with the prefix.</param>
		/// <returns>A string value that contains the namespace prefix that is associated with the <paramref name="namespaceName" />.</returns>
		string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
		{
			return this.impl.LookupPrefix(namespaceName);
		}

		/// <summary>Sets an event handler for receiving information about document type definition (DTD), XML-Data Reduced (XDR) schema, and XML Schema definition language (XSD) schema validation errors.</summary>
		public event ValidationEventHandler ValidationEventHandler
		{
			add
			{
				this.impl.ValidationEventHandler += value;
			}
			remove
			{
				this.impl.ValidationEventHandler -= value;
			}
		}

		/// <summary>Gets a schema type object.</summary>
		/// <returns>
		///     <see cref="T:System.Xml.Schema.XmlSchemaDatatype" />, <see cref="T:System.Xml.Schema.XmlSchemaSimpleType" />, or <see cref="T:System.Xml.Schema.XmlSchemaComplexType" /> depending whether the node value is a built in XML Schema definition language (XSD) type or a user defined simpleType or complexType; <see langword="null" /> if the current node has no schema type.</returns>
		public object SchemaType
		{
			get
			{
				return this.impl.SchemaType;
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.XmlReader" /> used to construct this <see langword="XmlValidatingReader" />.</summary>
		/// <returns>The <see langword="XmlReader" /> specified in the constructor.</returns>
		public XmlReader Reader
		{
			get
			{
				return this.impl.Reader;
			}
		}

		/// <summary>Gets or sets a value indicating the type of validation to perform.</summary>
		/// <returns>One of the <see cref="T:System.Xml.ValidationType" /> values. If this property is not set, it defaults to ValidationType.Auto.</returns>
		/// <exception cref="T:System.InvalidOperationException">Setting the property after a Read has been called. </exception>
		public ValidationType ValidationType
		{
			get
			{
				return this.impl.ValidationType;
			}
			set
			{
				this.impl.ValidationType = value;
			}
		}

		/// <summary>Gets a <see cref="T:System.Xml.Schema.XmlSchemaCollection" /> to use for validation.</summary>
		/// <returns>The <see langword="XmlSchemaCollection" /> to use for validation.</returns>
		public XmlSchemaCollection Schemas
		{
			get
			{
				return this.impl.Schemas;
			}
		}

		/// <summary>Gets or sets a value that specifies how the reader handles entities.</summary>
		/// <returns>One of the <see cref="T:System.Xml.EntityHandling" /> values. If no <see langword="EntityHandling" /> is specified, it defaults to EntityHandling.ExpandEntities.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">Invalid value was specified. </exception>
		public EntityHandling EntityHandling
		{
			get
			{
				return this.impl.EntityHandling;
			}
			set
			{
				this.impl.EntityHandling = value;
			}
		}

		/// <summary>Sets the <see cref="T:System.Xml.XmlResolver" /> used for resolving external document type definition (DTD) and schema location references. The <see langword="XmlResolver" /> is also used to handle any import or include elements found in XML Schema definition language (XSD) schemas.</summary>
		/// <returns>The <see langword="XmlResolver" /> to use. If set to <see langword="null" />, external resources are not resolved.In version 1.1 of the .NET Framework, the caller must be fully trusted to specify an <see langword="XmlResolver" />.</returns>
		public XmlResolver XmlResolver
		{
			set
			{
				this.impl.XmlResolver = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether to do namespace support.</summary>
		/// <returns>
		///     <see langword="true" /> to do namespace support; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		public bool Namespaces
		{
			get
			{
				return this.impl.Namespaces;
			}
			set
			{
				this.impl.Namespaces = value;
			}
		}

		/// <summary>Gets the common language runtime type for the specified XML Schema definition language (XSD) type.</summary>
		/// <returns>The common language runtime type for the specified XML Schema type.</returns>
		public object ReadTypedValue()
		{
			return this.impl.ReadTypedValue();
		}

		/// <summary>Gets the encoding attribute for the document.</summary>
		/// <returns>The encoding value. If no encoding attribute exists, and there is not byte-order mark, this defaults to UTF-8.</returns>
		public Encoding Encoding
		{
			get
			{
				return this.impl.Encoding;
			}
		}

		internal XmlValidatingReaderImpl Impl
		{
			get
			{
				return this.impl;
			}
		}

		internal override IDtdInfo DtdInfo
		{
			get
			{
				return this.impl.DtdInfo;
			}
		}

		private XmlValidatingReaderImpl impl;
	}
}
