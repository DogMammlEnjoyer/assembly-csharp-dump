using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace System.Xml
{
	/// <summary>Represents a reader that provides fast, noncached, forward-only access to XML data.To browse the .NET Framework source code for this type, see the Reference Source.</summary>
	[DebuggerDisplay("{debuggerDisplayProxy}")]
	[DebuggerDisplay("{debuggerDisplayProxy}")]
	public abstract class XmlReader : IDisposable
	{
		/// <summary>Gets the <see cref="T:System.Xml.XmlReaderSettings" /> object used to create this <see cref="T:System.Xml.XmlReader" /> instance.</summary>
		/// <returns>The <see cref="T:System.Xml.XmlReaderSettings" /> object used to create this reader instance. If this reader was not created using the <see cref="Overload:System.Xml.XmlReader.Create" /> method, this property returns <see langword="null" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual XmlReaderSettings Settings
		{
			get
			{
				return null;
			}
		}

		/// <summary>When overridden in a derived class, gets the type of the current node.</summary>
		/// <returns>One of the enumeration values that specify the type of the current node.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract XmlNodeType NodeType { get; }

		/// <summary>When overridden in a derived class, gets the qualified name of the current node.</summary>
		/// <returns>The qualified name of the current node. For example, <see langword="Name" /> is <see langword="bk:book" /> for the element &lt;bk:book&gt;.The name returned is dependent on the <see cref="P:System.Xml.XmlReader.NodeType" /> of the node. The following node types return the listed values. All other node types return an empty string.Node type Name 
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
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual string Name
		{
			get
			{
				if (this.Prefix.Length == 0)
				{
					return this.LocalName;
				}
				return this.NameTable.Add(this.Prefix + ":" + this.LocalName);
			}
		}

		/// <summary>When overridden in a derived class, gets the local name of the current node.</summary>
		/// <returns>The name of the current node with the prefix removed. For example, <see langword="LocalName" /> is <see langword="book" /> for the element &lt;bk:book&gt;.For node types that do not have a name (like <see langword="Text" />, <see langword="Comment" />, and so on), this property returns <see langword="String.Empty" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract string LocalName { get; }

		/// <summary>When overridden in a derived class, gets the namespace URI (as defined in the W3C Namespace specification) of the node on which the reader is positioned.</summary>
		/// <returns>The namespace URI of the current node; otherwise an empty string.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract string NamespaceURI { get; }

		/// <summary>When overridden in a derived class, gets the namespace prefix associated with the current node.</summary>
		/// <returns>The namespace prefix associated with the current node.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract string Prefix { get; }

		/// <summary>When overridden in a derived class, gets a value indicating whether the current node can have a <see cref="P:System.Xml.XmlReader.Value" />.</summary>
		/// <returns>
		///     <see langword="true" /> if the node on which the reader is currently positioned can have a <see langword="Value" />; otherwise, <see langword="false" />. If <see langword="false" />, the node has a value of <see langword="String.Empty" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual bool HasValue
		{
			get
			{
				return XmlReader.HasValueInternal(this.NodeType);
			}
		}

		/// <summary>When overridden in a derived class, gets the text value of the current node.</summary>
		/// <returns>The value returned depends on the <see cref="P:System.Xml.XmlReader.NodeType" /> of the node. The following table lists node types that have a value to return. All other node types return <see langword="String.Empty" />.Node type Value 
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
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract string Value { get; }

		/// <summary>When overridden in a derived class, gets the depth of the current node in the XML document.</summary>
		/// <returns>The depth of the current node in the XML document.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract int Depth { get; }

		/// <summary>When overridden in a derived class, gets the base URI of the current node.</summary>
		/// <returns>The base URI of the current node.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract string BaseURI { get; }

		/// <summary>When overridden in a derived class, gets a value indicating whether the current node is an empty element (for example, &lt;MyElement/&gt;).</summary>
		/// <returns>
		///     <see langword="true" /> if the current node is an element (<see cref="P:System.Xml.XmlReader.NodeType" /> equals <see langword="XmlNodeType.Element" />) that ends with /&gt;; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract bool IsEmptyElement { get; }

		/// <summary>When overridden in a derived class, gets a value indicating whether the current node is an attribute that was generated from the default value defined in the DTD or schema.</summary>
		/// <returns>
		///     <see langword="true" /> if the current node is an attribute whose value was generated from the default value defined in the DTD or schema; <see langword="false" /> if the attribute value was explicitly set.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual bool IsDefault
		{
			get
			{
				return false;
			}
		}

		/// <summary>When overridden in a derived class, gets the quotation mark character used to enclose the value of an attribute node.</summary>
		/// <returns>The quotation mark character (" or ') used to enclose the value of an attribute node.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual char QuoteChar
		{
			get
			{
				return '"';
			}
		}

		/// <summary>When overridden in a derived class, gets the current <see langword="xml:space" /> scope.</summary>
		/// <returns>One of the <see cref="T:System.Xml.XmlSpace" /> values. If no <see langword="xml:space" /> scope exists, this property defaults to <see langword="XmlSpace.None" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual XmlSpace XmlSpace
		{
			get
			{
				return XmlSpace.None;
			}
		}

		/// <summary>When overridden in a derived class, gets the current <see langword="xml:lang" /> scope.</summary>
		/// <returns>The current <see langword="xml:lang" /> scope.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual string XmlLang
		{
			get
			{
				return string.Empty;
			}
		}

		/// <summary>Gets the schema information that has been assigned to the current node as a result of schema validation.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.IXmlSchemaInfo" /> object containing the schema information for the current node. Schema information can be set on elements, attributes, or on text nodes with a non-null <see cref="P:System.Xml.XmlReader.ValueType" /> (typed values).If the current node is not one of the above node types, or if the <see langword="XmlReader" /> instance does not report schema information, this property returns <see langword="null" />.If this property is called from an <see cref="T:System.Xml.XmlTextReader" /> or an <see cref="T:System.Xml.XmlValidatingReader" /> object, this property always returns <see langword="null" />. These <see langword="XmlReader" /> implementations do not expose schema information through the <see langword="SchemaInfo" /> property.If you have to get the post-schema-validation information set (PSVI) for an element, position the reader on the end tag of the element, rather than on the start tag. You get the PSVI through the <see langword="SchemaInfo" /> property of a reader. The validating reader that is created through <see cref="Overload:System.Xml.XmlReader.Create" /> with the <see cref="P:System.Xml.XmlReaderSettings.ValidationType" /> property set to <see cref="F:System.Xml.ValidationType.Schema" /> has complete PSVI for an element only when the reader is positioned on the end tag of an element.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual IXmlSchemaInfo SchemaInfo
		{
			get
			{
				return this as IXmlSchemaInfo;
			}
		}

		/// <summary>Gets The Common Language Runtime (CLR) type for the current node.</summary>
		/// <returns>The CLR type that corresponds to the typed value of the node. The default is <see langword="System.String" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual Type ValueType
		{
			get
			{
				return typeof(string);
			}
		}

		/// <summary>Reads the text content at the current position as an <see cref="T:System.Object" />.</summary>
		/// <returns>The text content as the most appropriate common language runtime (CLR) object.</returns>
		/// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
		/// <exception cref="T:System.FormatException">The string format is not valid.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual object ReadContentAsObject()
		{
			if (!this.CanReadContentAs())
			{
				throw this.CreateReadContentAsException("ReadContentAsObject");
			}
			return this.InternalReadContentAsString();
		}

		/// <summary>Reads the text content at the current position as a <see langword="Boolean" />.</summary>
		/// <returns>The text content as a <see cref="T:System.Boolean" /> object.</returns>
		/// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
		/// <exception cref="T:System.FormatException">The string format is not valid.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual bool ReadContentAsBoolean()
		{
			if (!this.CanReadContentAs())
			{
				throw this.CreateReadContentAsException("ReadContentAsBoolean");
			}
			bool result;
			try
			{
				result = XmlConvert.ToBoolean(this.InternalReadContentAsString());
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Boolean", innerException, this as IXmlLineInfo);
			}
			return result;
		}

		/// <summary>Reads the text content at the current position as a <see cref="T:System.DateTime" /> object.</summary>
		/// <returns>The text content as a <see cref="T:System.DateTime" /> object.</returns>
		/// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
		/// <exception cref="T:System.FormatException">The string format is not valid.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual DateTime ReadContentAsDateTime()
		{
			if (!this.CanReadContentAs())
			{
				throw this.CreateReadContentAsException("ReadContentAsDateTime");
			}
			DateTime result;
			try
			{
				result = XmlConvert.ToDateTime(this.InternalReadContentAsString(), XmlDateTimeSerializationMode.RoundtripKind);
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTime", innerException, this as IXmlLineInfo);
			}
			return result;
		}

		/// <summary>Reads the text content at the current position as a <see cref="T:System.DateTimeOffset" /> object.</summary>
		/// <returns>The text content as a <see cref="T:System.DateTimeOffset" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual DateTimeOffset ReadContentAsDateTimeOffset()
		{
			if (!this.CanReadContentAs())
			{
				throw this.CreateReadContentAsException("ReadContentAsDateTimeOffset");
			}
			DateTimeOffset result;
			try
			{
				result = XmlConvert.ToDateTimeOffset(this.InternalReadContentAsString());
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "DateTimeOffset", innerException, this as IXmlLineInfo);
			}
			return result;
		}

		/// <summary>Reads the text content at the current position as a double-precision floating-point number.</summary>
		/// <returns>The text content as a double-precision floating-point number.</returns>
		/// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
		/// <exception cref="T:System.FormatException">The string format is not valid.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual double ReadContentAsDouble()
		{
			if (!this.CanReadContentAs())
			{
				throw this.CreateReadContentAsException("ReadContentAsDouble");
			}
			double result;
			try
			{
				result = XmlConvert.ToDouble(this.InternalReadContentAsString());
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Double", innerException, this as IXmlLineInfo);
			}
			return result;
		}

		/// <summary>Reads the text content at the current position as a single-precision floating point number.</summary>
		/// <returns>The text content at the current position as a single-precision floating point number.</returns>
		/// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
		/// <exception cref="T:System.FormatException">The string format is not valid.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual float ReadContentAsFloat()
		{
			if (!this.CanReadContentAs())
			{
				throw this.CreateReadContentAsException("ReadContentAsFloat");
			}
			float result;
			try
			{
				result = XmlConvert.ToSingle(this.InternalReadContentAsString());
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Float", innerException, this as IXmlLineInfo);
			}
			return result;
		}

		/// <summary>Reads the text content at the current position as a <see cref="T:System.Decimal" /> object.</summary>
		/// <returns>The text content at the current position as a <see cref="T:System.Decimal" /> object.</returns>
		/// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
		/// <exception cref="T:System.FormatException">The string format is not valid.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual decimal ReadContentAsDecimal()
		{
			if (!this.CanReadContentAs())
			{
				throw this.CreateReadContentAsException("ReadContentAsDecimal");
			}
			decimal result;
			try
			{
				result = XmlConvert.ToDecimal(this.InternalReadContentAsString());
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Decimal", innerException, this as IXmlLineInfo);
			}
			return result;
		}

		/// <summary>Reads the text content at the current position as a 32-bit signed integer.</summary>
		/// <returns>The text content as a 32-bit signed integer.</returns>
		/// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
		/// <exception cref="T:System.FormatException">The string format is not valid.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual int ReadContentAsInt()
		{
			if (!this.CanReadContentAs())
			{
				throw this.CreateReadContentAsException("ReadContentAsInt");
			}
			int result;
			try
			{
				result = XmlConvert.ToInt32(this.InternalReadContentAsString());
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Int", innerException, this as IXmlLineInfo);
			}
			return result;
		}

		/// <summary>Reads the text content at the current position as a 64-bit signed integer.</summary>
		/// <returns>The text content as a 64-bit signed integer.</returns>
		/// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
		/// <exception cref="T:System.FormatException">The string format is not valid.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual long ReadContentAsLong()
		{
			if (!this.CanReadContentAs())
			{
				throw this.CreateReadContentAsException("ReadContentAsLong");
			}
			long result;
			try
			{
				result = XmlConvert.ToInt64(this.InternalReadContentAsString());
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", "Long", innerException, this as IXmlLineInfo);
			}
			return result;
		}

		/// <summary>Reads the text content at the current position as a <see cref="T:System.String" /> object.</summary>
		/// <returns>The text content as a <see cref="T:System.String" /> object.</returns>
		/// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
		/// <exception cref="T:System.FormatException">The string format is not valid.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual string ReadContentAsString()
		{
			if (!this.CanReadContentAs())
			{
				throw this.CreateReadContentAsException("ReadContentAsString");
			}
			return this.InternalReadContentAsString();
		}

		/// <summary>Reads the content as an object of the type specified.</summary>
		/// <param name="returnType">The type of the value to be returned.
		///       Note   With the release of the .NET Framework 3.5, the value of the <paramref name="returnType" /> parameter can now be the <see cref="T:System.DateTimeOffset" /> type.</param>
		/// <param name="namespaceResolver">An <see cref="T:System.Xml.IXmlNamespaceResolver" /> object that is used to resolve any namespace prefixes related to type conversion. For example, this can be used when converting an <see cref="T:System.Xml.XmlQualifiedName" /> object to an xs:string.This value can be <see langword="null" />.</param>
		/// <returns>The concatenated text content or attribute value converted to the requested type.</returns>
		/// <exception cref="T:System.FormatException">The content is not in the correct format for the target type.</exception>
		/// <exception cref="T:System.InvalidCastException">The attempted cast is not valid.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="returnType" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The current node is not a supported node type. See the table below for details.</exception>
		/// <exception cref="T:System.OverflowException">Read <see langword="Decimal.MaxValue" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			if (!this.CanReadContentAs())
			{
				throw this.CreateReadContentAsException("ReadContentAs");
			}
			string text = this.InternalReadContentAsString();
			if (returnType == typeof(string))
			{
				return text;
			}
			object result;
			try
			{
				result = XmlUntypedConverter.Untyped.ChangeType(text, returnType, (namespaceResolver == null) ? (this as IXmlNamespaceResolver) : namespaceResolver);
			}
			catch (FormatException innerException)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), innerException, this as IXmlLineInfo);
			}
			catch (InvalidCastException innerException2)
			{
				throw new XmlException("Content cannot be converted to the type {0}.", returnType.ToString(), innerException2, this as IXmlLineInfo);
			}
			return result;
		}

		/// <summary>Reads the current element and returns the contents as an <see cref="T:System.Object" />.</summary>
		/// <returns>A boxed common language runtime (CLR) object of the most appropriate type. The <see cref="P:System.Xml.XmlReader.ValueType" /> property determines the appropriate CLR type. If the content is typed as a list type, this method returns an array of boxed objects of the appropriate type.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to the requested type</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual object ReadElementContentAsObject()
		{
			if (this.SetupReadElementContentAsXxx("ReadElementContentAsObject"))
			{
				object result = this.ReadContentAsObject();
				this.FinishReadElementContentAsXxx();
				return result;
			}
			return string.Empty;
		}

		/// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as an <see cref="T:System.Object" />.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceURI">The namespace URI of the element.</param>
		/// <returns>A boxed common language runtime (CLR) object of the most appropriate type. The <see cref="P:System.Xml.XmlReader.ValueType" /> property determines the appropriate CLR type. If the content is typed as a list type, this method returns an array of boxed objects of the appropriate type.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to the requested type.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual object ReadElementContentAsObject(string localName, string namespaceURI)
		{
			this.CheckElement(localName, namespaceURI);
			return this.ReadElementContentAsObject();
		}

		/// <summary>Reads the current element and returns the contents as a <see cref="T:System.Boolean" /> object.</summary>
		/// <returns>The element content as a <see cref="T:System.Boolean" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a <see cref="T:System.Boolean" /> object.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual bool ReadElementContentAsBoolean()
		{
			if (this.SetupReadElementContentAsXxx("ReadElementContentAsBoolean"))
			{
				bool result = this.ReadContentAsBoolean();
				this.FinishReadElementContentAsXxx();
				return result;
			}
			return XmlConvert.ToBoolean(string.Empty);
		}

		/// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a <see cref="T:System.Boolean" /> object.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceURI">The namespace URI of the element.</param>
		/// <returns>The element content as a <see cref="T:System.Boolean" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to the requested type.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual bool ReadElementContentAsBoolean(string localName, string namespaceURI)
		{
			this.CheckElement(localName, namespaceURI);
			return this.ReadElementContentAsBoolean();
		}

		/// <summary>Reads the current element and returns the contents as a <see cref="T:System.DateTime" /> object.</summary>
		/// <returns>The element content as a <see cref="T:System.DateTime" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a <see cref="T:System.DateTime" /> object.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual DateTime ReadElementContentAsDateTime()
		{
			if (this.SetupReadElementContentAsXxx("ReadElementContentAsDateTime"))
			{
				DateTime result = this.ReadContentAsDateTime();
				this.FinishReadElementContentAsXxx();
				return result;
			}
			return XmlConvert.ToDateTime(string.Empty, XmlDateTimeSerializationMode.RoundtripKind);
		}

		/// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a <see cref="T:System.DateTime" /> object.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceURI">The namespace URI of the element.</param>
		/// <returns>The element contents as a <see cref="T:System.DateTime" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to the requested type.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual DateTime ReadElementContentAsDateTime(string localName, string namespaceURI)
		{
			this.CheckElement(localName, namespaceURI);
			return this.ReadElementContentAsDateTime();
		}

		/// <summary>Reads the current element and returns the contents as a double-precision floating-point number.</summary>
		/// <returns>The element content as a double-precision floating-point number.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a double-precision floating-point number.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual double ReadElementContentAsDouble()
		{
			if (this.SetupReadElementContentAsXxx("ReadElementContentAsDouble"))
			{
				double result = this.ReadContentAsDouble();
				this.FinishReadElementContentAsXxx();
				return result;
			}
			return XmlConvert.ToDouble(string.Empty);
		}

		/// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a double-precision floating-point number.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceURI">The namespace URI of the element.</param>
		/// <returns>The element content as a double-precision floating-point number.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to the requested type.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual double ReadElementContentAsDouble(string localName, string namespaceURI)
		{
			this.CheckElement(localName, namespaceURI);
			return this.ReadElementContentAsDouble();
		}

		/// <summary>Reads the current element and returns the contents as single-precision floating-point number.</summary>
		/// <returns>The element content as a single-precision floating point number.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a single-precision floating-point number.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual float ReadElementContentAsFloat()
		{
			if (this.SetupReadElementContentAsXxx("ReadElementContentAsFloat"))
			{
				float result = this.ReadContentAsFloat();
				this.FinishReadElementContentAsXxx();
				return result;
			}
			return XmlConvert.ToSingle(string.Empty);
		}

		/// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a single-precision floating-point number.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceURI">The namespace URI of the element.</param>
		/// <returns>The element content as a single-precision floating point number.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a single-precision floating-point number.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual float ReadElementContentAsFloat(string localName, string namespaceURI)
		{
			this.CheckElement(localName, namespaceURI);
			return this.ReadElementContentAsFloat();
		}

		/// <summary>Reads the current element and returns the contents as a <see cref="T:System.Decimal" /> object.</summary>
		/// <returns>The element content as a <see cref="T:System.Decimal" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a <see cref="T:System.Decimal" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual decimal ReadElementContentAsDecimal()
		{
			if (this.SetupReadElementContentAsXxx("ReadElementContentAsDecimal"))
			{
				decimal result = this.ReadContentAsDecimal();
				this.FinishReadElementContentAsXxx();
				return result;
			}
			return XmlConvert.ToDecimal(string.Empty);
		}

		/// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a <see cref="T:System.Decimal" /> object.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceURI">The namespace URI of the element.</param>
		/// <returns>The element content as a <see cref="T:System.Decimal" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a <see cref="T:System.Decimal" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual decimal ReadElementContentAsDecimal(string localName, string namespaceURI)
		{
			this.CheckElement(localName, namespaceURI);
			return this.ReadElementContentAsDecimal();
		}

		/// <summary>Reads the current element and returns the contents as a 32-bit signed integer.</summary>
		/// <returns>The element content as a 32-bit signed integer.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a 32-bit signed integer.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual int ReadElementContentAsInt()
		{
			if (this.SetupReadElementContentAsXxx("ReadElementContentAsInt"))
			{
				int result = this.ReadContentAsInt();
				this.FinishReadElementContentAsXxx();
				return result;
			}
			return XmlConvert.ToInt32(string.Empty);
		}

		/// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a 32-bit signed integer.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceURI">The namespace URI of the element.</param>
		/// <returns>The element content as a 32-bit signed integer.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a 32-bit signed integer.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual int ReadElementContentAsInt(string localName, string namespaceURI)
		{
			this.CheckElement(localName, namespaceURI);
			return this.ReadElementContentAsInt();
		}

		/// <summary>Reads the current element and returns the contents as a 64-bit signed integer.</summary>
		/// <returns>The element content as a 64-bit signed integer.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a 64-bit signed integer.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual long ReadElementContentAsLong()
		{
			if (this.SetupReadElementContentAsXxx("ReadElementContentAsLong"))
			{
				long result = this.ReadContentAsLong();
				this.FinishReadElementContentAsXxx();
				return result;
			}
			return XmlConvert.ToInt64(string.Empty);
		}

		/// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a 64-bit signed integer.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceURI">The namespace URI of the element.</param>
		/// <returns>The element content as a 64-bit signed integer.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a 64-bit signed integer.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual long ReadElementContentAsLong(string localName, string namespaceURI)
		{
			this.CheckElement(localName, namespaceURI);
			return this.ReadElementContentAsLong();
		}

		/// <summary>Reads the current element and returns the contents as a <see cref="T:System.String" /> object.</summary>
		/// <returns>The element content as a <see cref="T:System.String" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a <see cref="T:System.String" /> object.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual string ReadElementContentAsString()
		{
			if (this.SetupReadElementContentAsXxx("ReadElementContentAsString"))
			{
				string result = this.ReadContentAsString();
				this.FinishReadElementContentAsXxx();
				return result;
			}
			return string.Empty;
		}

		/// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the current element and returns the contents as a <see cref="T:System.String" /> object.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceURI">The namespace URI of the element.</param>
		/// <returns>The element content as a <see cref="T:System.String" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to a <see cref="T:System.String" /> object.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual string ReadElementContentAsString(string localName, string namespaceURI)
		{
			this.CheckElement(localName, namespaceURI);
			return this.ReadElementContentAsString();
		}

		/// <summary>Reads the element content as the requested type.</summary>
		/// <param name="returnType">The type of the value to be returned.
		///       Note   With the release of the .NET Framework 3.5, the value of the <paramref name="returnType" /> parameter can now be the <see cref="T:System.DateTimeOffset" /> type.</param>
		/// <param name="namespaceResolver">An <see cref="T:System.Xml.IXmlNamespaceResolver" /> object that is used to resolve any namespace prefixes related to type conversion.</param>
		/// <returns>The element content converted to the requested typed object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to the requested type.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.OverflowException">Read <see langword="Decimal.MaxValue" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			if (this.SetupReadElementContentAsXxx("ReadElementContentAs"))
			{
				object result = this.ReadContentAs(returnType, namespaceResolver);
				this.FinishReadElementContentAsXxx();
				return result;
			}
			if (!(returnType == typeof(string)))
			{
				return XmlUntypedConverter.Untyped.ChangeType(string.Empty, returnType, namespaceResolver);
			}
			return string.Empty;
		}

		/// <summary>Checks that the specified local name and namespace URI matches that of the current element, then reads the element content as the requested type.</summary>
		/// <param name="returnType">The type of the value to be returned.
		///       Note   With the release of the .NET Framework 3.5, the value of the <paramref name="returnType" /> parameter can now be the <see cref="T:System.DateTimeOffset" /> type.</param>
		/// <param name="namespaceResolver">An <see cref="T:System.Xml.IXmlNamespaceResolver" /> object that is used to resolve any namespace prefixes related to type conversion.</param>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceURI">The namespace URI of the element.</param>
		/// <returns>The element content converted to the requested typed object.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlReader" /> is not positioned on an element.</exception>
		/// <exception cref="T:System.Xml.XmlException">The current element contains child elements.-or-The element content cannot be converted to the requested type.</exception>
		/// <exception cref="T:System.ArgumentNullException">The method is called with <see langword="null" /> arguments.</exception>
		/// <exception cref="T:System.ArgumentException">The specified local name and namespace URI do not match that of the current element being read.</exception>
		/// <exception cref="T:System.OverflowException">Read <see langword="Decimal.MaxValue" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver, string localName, string namespaceURI)
		{
			this.CheckElement(localName, namespaceURI);
			return this.ReadElementContentAs(returnType, namespaceResolver);
		}

		/// <summary>When overridden in a derived class, gets the number of attributes on the current node.</summary>
		/// <returns>The number of attributes on the current node.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract int AttributeCount { get; }

		/// <summary>When overridden in a derived class, gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.Name" />.</summary>
		/// <param name="name">The qualified name of the attribute.</param>
		/// <returns>The value of the specified attribute. If the attribute is not found or the value is <see langword="String.Empty" />, <see langword="null" /> is returned.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract string GetAttribute(string name);

		/// <summary>When overridden in a derived class, gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" />.</summary>
		/// <param name="name">The local name of the attribute.</param>
		/// <param name="namespaceURI">The namespace URI of the attribute.</param>
		/// <returns>The value of the specified attribute. If the attribute is not found or the value is <see langword="String.Empty" />, <see langword="null" /> is returned. This method does not move the reader.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract string GetAttribute(string name, string namespaceURI);

		/// <summary>When overridden in a derived class, gets the value of the attribute with the specified index.</summary>
		/// <param name="i">The index of the attribute. The index is zero-based. (The first attribute has index 0.)</param>
		/// <returns>The value of the specified attribute. This method does not move the reader.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///         <paramref name="i" /> is out of range. It must be non-negative and less than the size of the attribute collection.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract string GetAttribute(int i);

		/// <summary>When overridden in a derived class, gets the value of the attribute with the specified index.</summary>
		/// <param name="i">The index of the attribute.</param>
		/// <returns>The value of the specified attribute.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual string this[int i]
		{
			get
			{
				return this.GetAttribute(i);
			}
		}

		/// <summary>When overridden in a derived class, gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.Name" />.</summary>
		/// <param name="name">The qualified name of the attribute.</param>
		/// <returns>The value of the specified attribute. If the attribute is not found, <see langword="null" /> is returned.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual string this[string name]
		{
			get
			{
				return this.GetAttribute(name);
			}
		}

		/// <summary>When overridden in a derived class, gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" />.</summary>
		/// <param name="name">The local name of the attribute.</param>
		/// <param name="namespaceURI">The namespace URI of the attribute.</param>
		/// <returns>The value of the specified attribute. If the attribute is not found, <see langword="null" /> is returned.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual string this[string name, string namespaceURI]
		{
			get
			{
				return this.GetAttribute(name, namespaceURI);
			}
		}

		/// <summary>When overridden in a derived class, moves to the attribute with the specified <see cref="P:System.Xml.XmlReader.Name" />.</summary>
		/// <param name="name">The qualified name of the attribute.</param>
		/// <returns>
		///     <see langword="true" /> if the attribute is found; otherwise, <see langword="false" />. If <see langword="false" />, the reader's position does not change.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.ArgumentException">The parameter is an empty string.</exception>
		public abstract bool MoveToAttribute(string name);

		/// <summary>When overridden in a derived class, moves to the attribute with the specified <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" />.</summary>
		/// <param name="name">The local name of the attribute.</param>
		/// <param name="ns">The namespace URI of the attribute.</param>
		/// <returns>
		///     <see langword="true" /> if the attribute is found; otherwise, <see langword="false" />. If <see langword="false" />, the reader's position does not change.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.ArgumentNullException">Both parameter values are <see langword="null" />.</exception>
		public abstract bool MoveToAttribute(string name, string ns);

		/// <summary>When overridden in a derived class, moves to the attribute with the specified index.</summary>
		/// <param name="i">The index of the attribute.</param>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The parameter has a negative value.</exception>
		public virtual void MoveToAttribute(int i)
		{
			if (i < 0 || i >= this.AttributeCount)
			{
				throw new ArgumentOutOfRangeException("i");
			}
			this.MoveToElement();
			this.MoveToFirstAttribute();
			for (int j = 0; j < i; j++)
			{
				this.MoveToNextAttribute();
			}
		}

		/// <summary>When overridden in a derived class, moves to the first attribute.</summary>
		/// <returns>
		///     <see langword="true" /> if an attribute exists (the reader moves to the first attribute); otherwise, <see langword="false" /> (the position of the reader does not change).</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract bool MoveToFirstAttribute();

		/// <summary>When overridden in a derived class, moves to the next attribute.</summary>
		/// <returns>
		///     <see langword="true" /> if there is a next attribute; <see langword="false" /> if there are no more attributes.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract bool MoveToNextAttribute();

		/// <summary>When overridden in a derived class, moves to the element that contains the current attribute node.</summary>
		/// <returns>
		///     <see langword="true" /> if the reader is positioned on an attribute (the reader moves to the element that owns the attribute); <see langword="false" /> if the reader is not positioned on an attribute (the position of the reader does not change).</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract bool MoveToElement();

		/// <summary>When overridden in a derived class, parses the attribute value into one or more <see langword="Text" />, <see langword="EntityReference" />, or <see langword="EndEntity" /> nodes.</summary>
		/// <returns>
		///     <see langword="true" /> if there are nodes to return.
		///     <see langword="false" /> if the reader is not positioned on an attribute node when the initial call is made or if all the attribute values have been read.An empty attribute, such as, misc="", returns <see langword="true" /> with a single node with a value of <see langword="String.Empty" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract bool ReadAttributeValue();

		/// <summary>When overridden in a derived class, reads the next node from the stream.</summary>
		/// <returns>
		///     <see langword="true" /> if the next node was read successfully; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.Xml.XmlException">An error occurred while parsing the XML.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract bool Read();

		/// <summary>When overridden in a derived class, gets a value indicating whether the reader is positioned at the end of the stream.</summary>
		/// <returns>
		///     <see langword="true" /> if the reader is positioned at the end of the stream; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract bool EOF { get; }

		/// <summary>When overridden in a derived class, changes the <see cref="P:System.Xml.XmlReader.ReadState" /> to <see cref="F:System.Xml.ReadState.Closed" />.</summary>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void Close()
		{
		}

		/// <summary>When overridden in a derived class, gets the state of the reader.</summary>
		/// <returns>One of the enumeration values that specifies the state of the reader.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract ReadState ReadState { get; }

		/// <summary>Skips the children of the current node.</summary>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void Skip()
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return;
			}
			this.SkipSubtree();
		}

		/// <summary>When overridden in a derived class, gets the <see cref="T:System.Xml.XmlNameTable" /> associated with this implementation.</summary>
		/// <returns>The <see langword="XmlNameTable" /> enabling you to get the atomized version of a string within the node.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract XmlNameTable NameTable { get; }

		/// <summary>When overridden in a derived class, resolves a namespace prefix in the current element's scope.</summary>
		/// <param name="prefix">The prefix whose namespace URI you want to resolve. To match the default namespace, pass an empty string. </param>
		/// <returns>The namespace URI to which the prefix maps or <see langword="null" /> if no matching prefix is found.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract string LookupNamespace(string prefix);

		/// <summary>Gets a value indicating whether this reader can parse and resolve entities.</summary>
		/// <returns>
		///     <see langword="true" /> if the reader can parse and resolve entities; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual bool CanResolveEntity
		{
			get
			{
				return false;
			}
		}

		/// <summary>When overridden in a derived class, resolves the entity reference for <see langword="EntityReference" /> nodes.</summary>
		/// <exception cref="T:System.InvalidOperationException">The reader is not positioned on an <see langword="EntityReference" /> node; this implementation of the reader cannot resolve entities (<see cref="P:System.Xml.XmlReader.CanResolveEntity" /> returns <see langword="false" />).</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public abstract void ResolveEntity();

		/// <summary>Gets a value indicating whether the <see cref="T:System.Xml.XmlReader" /> implements the binary content read methods.</summary>
		/// <returns>
		///     <see langword="true" /> if the binary content read methods are implemented; otherwise <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual bool CanReadBinaryContent
		{
			get
			{
				return false;
			}
		}

		/// <summary>Reads the content and returns the Base64 decoded binary bytes.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <see cref="M:System.Xml.XmlReader.ReadContentAsBase64(System.Byte[],System.Int32,System.Int32)" /> is not supported on the current node.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Xml.XmlReader" /> implementation does not support this method.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			throw new NotSupportedException(Res.GetString("{0} method is not supported on this XmlReader. Use CanReadBinaryContent property to find out if a reader implements it.", new object[]
			{
				"ReadContentAsBase64"
			}));
		}

		/// <summary>Reads the element and decodes the <see langword="Base64" /> content.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The current node is not an element node.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Xml.XmlReader" /> implementation does not support this method.</exception>
		/// <exception cref="T:System.Xml.XmlException">The element contains mixed-content.</exception>
		/// <exception cref="T:System.FormatException">The content cannot be converted to the requested type.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			throw new NotSupportedException(Res.GetString("{0} method is not supported on this XmlReader. Use CanReadBinaryContent property to find out if a reader implements it.", new object[]
			{
				"ReadElementContentAsBase64"
			}));
		}

		/// <summary>Reads the content and returns the <see langword="BinHex" /> decoded binary bytes.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///         <see cref="M:System.Xml.XmlReader.ReadContentAsBinHex(System.Byte[],System.Int32,System.Int32)" /> is not supported on the current node.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Xml.XmlReader" /> implementation does not support this method.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			throw new NotSupportedException(Res.GetString("{0} method is not supported on this XmlReader. Use CanReadBinaryContent property to find out if a reader implements it.", new object[]
			{
				"ReadContentAsBinHex"
			}));
		}

		/// <summary>Reads the element and decodes the <see langword="BinHex" /> content.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The current node is not an element node.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer or index + count is larger than the allocated buffer size.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Xml.XmlReader" /> implementation does not support this method.</exception>
		/// <exception cref="T:System.Xml.XmlException">The element contains mixed-content.</exception>
		/// <exception cref="T:System.FormatException">The content cannot be converted to the requested type.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			throw new NotSupportedException(Res.GetString("{0} method is not supported on this XmlReader. Use CanReadBinaryContent property to find out if a reader implements it.", new object[]
			{
				"ReadElementContentAsBinHex"
			}));
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Xml.XmlReader" /> implements the <see cref="M:System.Xml.XmlReader.ReadValueChunk(System.Char[],System.Int32,System.Int32)" /> method.</summary>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.Xml.XmlReader" /> implements the <see cref="M:System.Xml.XmlReader.ReadValueChunk(System.Char[],System.Int32,System.Int32)" /> method; otherwise <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual bool CanReadValueChunk
		{
			get
			{
				return false;
			}
		}

		/// <summary>Reads large streams of text embedded in an XML document.</summary>
		/// <param name="buffer">The array of characters that serves as the buffer to which the text contents are written. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset within the buffer where the <see cref="T:System.Xml.XmlReader" /> can start to copy the results.</param>
		/// <param name="count">The maximum number of characters to copy into the buffer. The actual number of characters copied is returned from this method.</param>
		/// <returns>The number of characters read into the buffer. The value zero is returned when there is no more text content.</returns>
		/// <exception cref="T:System.InvalidOperationException">The current node does not have a value (<see cref="P:System.Xml.XmlReader.HasValue" /> is <see langword="false" />).</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The index into the buffer, or index + count is larger than the allocated buffer size.</exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Xml.XmlReader" /> implementation does not support this method.</exception>
		/// <exception cref="T:System.Xml.XmlException">The XML data is not well-formed.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual int ReadValueChunk(char[] buffer, int index, int count)
		{
			throw new NotSupportedException(Res.GetString("ReadValueChunk method is not supported on this XmlReader. Use CanReadValueChunk property to find out if an XmlReader implements it."));
		}

		/// <summary>When overridden in a derived class, reads the contents of an element or text node as a string. However, we recommend that you use the <see cref="Overload:System.Xml.XmlReader.ReadElementContentAsString" /> method instead, because it provides a more straightforward way to handle this operation.</summary>
		/// <returns>The contents of the element or an empty string.</returns>
		/// <exception cref="T:System.Xml.XmlException">An error occurred while parsing the XML.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string ReadString()
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return string.Empty;
			}
			this.MoveToElement();
			if (this.NodeType == XmlNodeType.Element)
			{
				if (this.IsEmptyElement)
				{
					return string.Empty;
				}
				if (!this.Read())
				{
					throw new InvalidOperationException(Res.GetString("Operation is not valid due to the current state of the object."));
				}
				if (this.NodeType == XmlNodeType.EndElement)
				{
					return string.Empty;
				}
			}
			string text = string.Empty;
			while (XmlReader.IsTextualNode(this.NodeType))
			{
				text += this.Value;
				if (!this.Read())
				{
					break;
				}
			}
			return text;
		}

		/// <summary>Checks whether the current node is a content (non-white space text, <see langword="CDATA" />, <see langword="Element" />, <see langword="EndElement" />, <see langword="EntityReference" />, or <see langword="EndEntity" />) node. If the node is not a content node, the reader skips ahead to the next content node or end of file. It skips over nodes of the following type: <see langword="ProcessingInstruction" />, <see langword="DocumentType" />, <see langword="Comment" />, <see langword="Whitespace" />, or <see langword="SignificantWhitespace" />.</summary>
		/// <returns>The <see cref="P:System.Xml.XmlReader.NodeType" /> of the current node found by the method or <see langword="XmlNodeType.None" /> if the reader has reached the end of the input stream.</returns>
		/// <exception cref="T:System.Xml.XmlException">Incorrect XML encountered in the input stream.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual XmlNodeType MoveToContent()
		{
			for (;;)
			{
				XmlNodeType nodeType = this.NodeType;
				switch (nodeType)
				{
				case XmlNodeType.Element:
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.EntityReference:
					goto IL_33;
				case XmlNodeType.Attribute:
					goto IL_2C;
				default:
					if (nodeType - XmlNodeType.EndElement <= 1)
					{
						goto IL_33;
					}
					if (!this.Read())
					{
						goto Block_2;
					}
					break;
				}
			}
			IL_2C:
			this.MoveToElement();
			IL_33:
			return this.NodeType;
			Block_2:
			return this.NodeType;
		}

		/// <summary>Checks that the current node is an element and advances the reader to the next node.</summary>
		/// <exception cref="T:System.Xml.XmlException">Incorrect XML was encountered in the input stream.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void ReadStartElement()
		{
			if (this.MoveToContent() != XmlNodeType.Element)
			{
				throw new XmlException("'{0}' is an invalid XmlNodeType.", this.NodeType.ToString(), this as IXmlLineInfo);
			}
			this.Read();
		}

		/// <summary>Checks that the current content node is an element with the given <see cref="P:System.Xml.XmlReader.Name" /> and advances the reader to the next node.</summary>
		/// <param name="name">The qualified name of the element.</param>
		/// <exception cref="T:System.Xml.XmlException">Incorrect XML was encountered in the input stream. -or- The <see cref="P:System.Xml.XmlReader.Name" /> of the element does not match the given <paramref name="name" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void ReadStartElement(string name)
		{
			if (this.MoveToContent() != XmlNodeType.Element)
			{
				throw new XmlException("'{0}' is an invalid XmlNodeType.", this.NodeType.ToString(), this as IXmlLineInfo);
			}
			if (this.Name == name)
			{
				this.Read();
				return;
			}
			throw new XmlException("Element '{0}' was not found.", name, this as IXmlLineInfo);
		}

		/// <summary>Checks that the current content node is an element with the given <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" /> and advances the reader to the next node.</summary>
		/// <param name="localname">The local name of the element.</param>
		/// <param name="ns">The namespace URI of the element.</param>
		/// <exception cref="T:System.Xml.XmlException">Incorrect XML was encountered in the input stream.-or-The <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" /> properties of the element found do not match the given arguments.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void ReadStartElement(string localname, string ns)
		{
			if (this.MoveToContent() != XmlNodeType.Element)
			{
				throw new XmlException("'{0}' is an invalid XmlNodeType.", this.NodeType.ToString(), this as IXmlLineInfo);
			}
			if (this.LocalName == localname && this.NamespaceURI == ns)
			{
				this.Read();
				return;
			}
			throw new XmlException("Element '{0}' with namespace name '{1}' was not found.", new string[]
			{
				localname,
				ns
			}, this as IXmlLineInfo);
		}

		/// <summary>Reads a text-only element. However, we recommend that you use the <see cref="M:System.Xml.XmlReader.ReadElementContentAsString" /> method instead, because it provides a more straightforward way to handle this operation.</summary>
		/// <returns>The text contained in the element that was read. An empty string if the element is empty.</returns>
		/// <exception cref="T:System.Xml.XmlException">The next content node is not a start tag; or the element found does not contain a simple text value.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string ReadElementString()
		{
			string result = string.Empty;
			if (this.MoveToContent() != XmlNodeType.Element)
			{
				throw new XmlException("'{0}' is an invalid XmlNodeType.", this.NodeType.ToString(), this as IXmlLineInfo);
			}
			if (!this.IsEmptyElement)
			{
				this.Read();
				result = this.ReadString();
				if (this.NodeType != XmlNodeType.EndElement)
				{
					throw new XmlException("Unexpected node type {0}. {1} method can only be called on elements with simple or empty content.", new string[]
					{
						this.NodeType.ToString(),
						"ReadElementString"
					}, this as IXmlLineInfo);
				}
				this.Read();
			}
			else
			{
				this.Read();
			}
			return result;
		}

		/// <summary>Checks that the <see cref="P:System.Xml.XmlReader.Name" /> property of the element found matches the given string before reading a text-only element. However, we recommend that you use the <see cref="M:System.Xml.XmlReader.ReadElementContentAsString" /> method instead, because it provides a more straightforward way to handle this operation.</summary>
		/// <param name="name">The name to check.</param>
		/// <returns>The text contained in the element that was read. An empty string if the element is empty.</returns>
		/// <exception cref="T:System.Xml.XmlException">If the next content node is not a start tag; if the element <see langword="Name" /> does not match the given argument; or if the element found does not contain a simple text value.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string ReadElementString(string name)
		{
			string result = string.Empty;
			if (this.MoveToContent() != XmlNodeType.Element)
			{
				throw new XmlException("'{0}' is an invalid XmlNodeType.", this.NodeType.ToString(), this as IXmlLineInfo);
			}
			if (this.Name != name)
			{
				throw new XmlException("Element '{0}' was not found.", name, this as IXmlLineInfo);
			}
			if (!this.IsEmptyElement)
			{
				result = this.ReadString();
				if (this.NodeType != XmlNodeType.EndElement)
				{
					throw new XmlException("'{0}' is an invalid XmlNodeType.", this.NodeType.ToString(), this as IXmlLineInfo);
				}
				this.Read();
			}
			else
			{
				this.Read();
			}
			return result;
		}

		/// <summary>Checks that the <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" /> properties of the element found matches the given strings before reading a text-only element. However, we recommend that you use the <see cref="M:System.Xml.XmlReader.ReadElementContentAsString(System.String,System.String)" /> method instead, because it provides a more straightforward way to handle this operation.</summary>
		/// <param name="localname">The local name to check.</param>
		/// <param name="ns">The namespace URI to check.</param>
		/// <returns>The text contained in the element that was read. An empty string if the element is empty.</returns>
		/// <exception cref="T:System.Xml.XmlException">If the next content node is not a start tag; if the element <see langword="LocalName" /> or <see langword="NamespaceURI" /> do not match the given arguments; or if the element found does not contain a simple text value.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string ReadElementString(string localname, string ns)
		{
			string result = string.Empty;
			if (this.MoveToContent() != XmlNodeType.Element)
			{
				throw new XmlException("'{0}' is an invalid XmlNodeType.", this.NodeType.ToString(), this as IXmlLineInfo);
			}
			if (this.LocalName != localname || this.NamespaceURI != ns)
			{
				throw new XmlException("Element '{0}' with namespace name '{1}' was not found.", new string[]
				{
					localname,
					ns
				}, this as IXmlLineInfo);
			}
			if (!this.IsEmptyElement)
			{
				result = this.ReadString();
				if (this.NodeType != XmlNodeType.EndElement)
				{
					throw new XmlException("'{0}' is an invalid XmlNodeType.", this.NodeType.ToString(), this as IXmlLineInfo);
				}
				this.Read();
			}
			else
			{
				this.Read();
			}
			return result;
		}

		/// <summary>Checks that the current content node is an end tag and advances the reader to the next node.</summary>
		/// <exception cref="T:System.Xml.XmlException">The current node is not an end tag or if incorrect XML is encountered in the input stream.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual void ReadEndElement()
		{
			if (this.MoveToContent() != XmlNodeType.EndElement)
			{
				throw new XmlException("'{0}' is an invalid XmlNodeType.", this.NodeType.ToString(), this as IXmlLineInfo);
			}
			this.Read();
		}

		/// <summary>Calls <see cref="M:System.Xml.XmlReader.MoveToContent" /> and tests if the current content node is a start tag or empty element tag.</summary>
		/// <returns>
		///     <see langword="true" /> if <see cref="M:System.Xml.XmlReader.MoveToContent" /> finds a start tag or empty element tag; <see langword="false" /> if a node type other than <see langword="XmlNodeType.Element" /> was found.</returns>
		/// <exception cref="T:System.Xml.XmlException">Incorrect XML is encountered in the input stream.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual bool IsStartElement()
		{
			return this.MoveToContent() == XmlNodeType.Element;
		}

		/// <summary>Calls <see cref="M:System.Xml.XmlReader.MoveToContent" /> and tests if the current content node is a start tag or empty element tag and if the <see cref="P:System.Xml.XmlReader.Name" /> property of the element found matches the given argument.</summary>
		/// <param name="name">The string matched against the <see langword="Name" /> property of the element found.</param>
		/// <returns>
		///     <see langword="true" /> if the resulting node is an element and the <see langword="Name" /> property matches the specified string. <see langword="false" /> if a node type other than <see langword="XmlNodeType.Element" /> was found or if the element <see langword="Name" /> property does not match the specified string.</returns>
		/// <exception cref="T:System.Xml.XmlException">Incorrect XML is encountered in the input stream.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual bool IsStartElement(string name)
		{
			return this.MoveToContent() == XmlNodeType.Element && this.Name == name;
		}

		/// <summary>Calls <see cref="M:System.Xml.XmlReader.MoveToContent" /> and tests if the current content node is a start tag or empty element tag and if the <see cref="P:System.Xml.XmlReader.LocalName" /> and <see cref="P:System.Xml.XmlReader.NamespaceURI" /> properties of the element found match the given strings.</summary>
		/// <param name="localname">The string to match against the <see langword="LocalName" /> property of the element found.</param>
		/// <param name="ns">The string to match against the <see langword="NamespaceURI" /> property of the element found.</param>
		/// <returns>
		///     <see langword="true" /> if the resulting node is an element. <see langword="false" /> if a node type other than <see langword="XmlNodeType.Element" /> was found or if the <see langword="LocalName" /> and <see langword="NamespaceURI" /> properties of the element do not match the specified strings.</returns>
		/// <exception cref="T:System.Xml.XmlException">Incorrect XML is encountered in the input stream.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual bool IsStartElement(string localname, string ns)
		{
			return this.MoveToContent() == XmlNodeType.Element && this.LocalName == localname && this.NamespaceURI == ns;
		}

		/// <summary>Reads until an element with the specified qualified name is found.</summary>
		/// <param name="name">The qualified name of the element.</param>
		/// <returns>
		///     <see langword="true" /> if a matching element is found; otherwise <see langword="false" /> and the <see cref="T:System.Xml.XmlReader" /> is in an end of file state.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.ArgumentException">The parameter is an empty string.</exception>
		public virtual bool ReadToFollowing(string name)
		{
			if (name == null || name.Length == 0)
			{
				throw XmlConvert.CreateInvalidNameArgumentException(name, "name");
			}
			name = this.NameTable.Add(name);
			while (this.Read())
			{
				if (this.NodeType == XmlNodeType.Element && Ref.Equal(name, this.Name))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Reads until an element with the specified local name and namespace URI is found.</summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="namespaceURI">The namespace URI of the element.</param>
		/// <returns>
		///     <see langword="true" /> if a matching element is found; otherwise <see langword="false" /> and the <see cref="T:System.Xml.XmlReader" /> is in an end of file state.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.ArgumentNullException">Both parameter values are <see langword="null" />.</exception>
		public virtual bool ReadToFollowing(string localName, string namespaceURI)
		{
			if (localName == null || localName.Length == 0)
			{
				throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
			}
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			localName = this.NameTable.Add(localName);
			namespaceURI = this.NameTable.Add(namespaceURI);
			while (this.Read())
			{
				if (this.NodeType == XmlNodeType.Element && Ref.Equal(localName, this.LocalName) && Ref.Equal(namespaceURI, this.NamespaceURI))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Advances the <see cref="T:System.Xml.XmlReader" /> to the next descendant element with the specified qualified name.</summary>
		/// <param name="name">The qualified name of the element you wish to move to.</param>
		/// <returns>
		///     <see langword="true" /> if a matching descendant element is found; otherwise <see langword="false" />. If a matching child element is not found, the <see cref="T:System.Xml.XmlReader" /> is positioned on the end tag (<see cref="P:System.Xml.XmlReader.NodeType" /> is <see langword="XmlNodeType.EndElement" />) of the element.If the <see cref="T:System.Xml.XmlReader" /> is not positioned on an element when <see cref="M:System.Xml.XmlReader.ReadToDescendant(System.String)" /> was called, this method returns <see langword="false" /> and the position of the <see cref="T:System.Xml.XmlReader" /> is not changed.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.ArgumentException">The parameter is an empty string.</exception>
		public virtual bool ReadToDescendant(string name)
		{
			if (name == null || name.Length == 0)
			{
				throw XmlConvert.CreateInvalidNameArgumentException(name, "name");
			}
			int num = this.Depth;
			if (this.NodeType != XmlNodeType.Element)
			{
				if (this.ReadState != ReadState.Initial)
				{
					return false;
				}
				num--;
			}
			else if (this.IsEmptyElement)
			{
				return false;
			}
			name = this.NameTable.Add(name);
			while (this.Read() && this.Depth > num)
			{
				if (this.NodeType == XmlNodeType.Element && Ref.Equal(name, this.Name))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Advances the <see cref="T:System.Xml.XmlReader" /> to the next descendant element with the specified local name and namespace URI.</summary>
		/// <param name="localName">The local name of the element you wish to move to.</param>
		/// <param name="namespaceURI">The namespace URI of the element you wish to move to.</param>
		/// <returns>
		///     <see langword="true" /> if a matching descendant element is found; otherwise <see langword="false" />. If a matching child element is not found, the <see cref="T:System.Xml.XmlReader" /> is positioned on the end tag (<see cref="P:System.Xml.XmlReader.NodeType" /> is <see langword="XmlNodeType.EndElement" />) of the element.If the <see cref="T:System.Xml.XmlReader" /> is not positioned on an element when <see cref="M:System.Xml.XmlReader.ReadToDescendant(System.String,System.String)" /> was called, this method returns <see langword="false" /> and the position of the <see cref="T:System.Xml.XmlReader" /> is not changed.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.ArgumentNullException">Both parameter values are <see langword="null" />.</exception>
		public virtual bool ReadToDescendant(string localName, string namespaceURI)
		{
			if (localName == null || localName.Length == 0)
			{
				throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
			}
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			int num = this.Depth;
			if (this.NodeType != XmlNodeType.Element)
			{
				if (this.ReadState != ReadState.Initial)
				{
					return false;
				}
				num--;
			}
			else if (this.IsEmptyElement)
			{
				return false;
			}
			localName = this.NameTable.Add(localName);
			namespaceURI = this.NameTable.Add(namespaceURI);
			while (this.Read() && this.Depth > num)
			{
				if (this.NodeType == XmlNodeType.Element && Ref.Equal(localName, this.LocalName) && Ref.Equal(namespaceURI, this.NamespaceURI))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Advances the <see langword="XmlReader" /> to the next sibling element with the specified qualified name.</summary>
		/// <param name="name">The qualified name of the sibling element you wish to move to.</param>
		/// <returns>
		///     <see langword="true" /> if a matching sibling element is found; otherwise <see langword="false" />. If a matching sibling element is not found, the <see langword="XmlReader" /> is positioned on the end tag (<see cref="P:System.Xml.XmlReader.NodeType" /> is <see langword="XmlNodeType.EndElement" />) of the parent element.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.ArgumentException">The parameter is an empty string.</exception>
		public virtual bool ReadToNextSibling(string name)
		{
			if (name == null || name.Length == 0)
			{
				throw XmlConvert.CreateInvalidNameArgumentException(name, "name");
			}
			name = this.NameTable.Add(name);
			while (this.SkipSubtree())
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType == XmlNodeType.Element && Ref.Equal(name, this.Name))
				{
					return true;
				}
				if (nodeType == XmlNodeType.EndElement || this.EOF)
				{
					break;
				}
			}
			return false;
		}

		/// <summary>Advances the <see langword="XmlReader" /> to the next sibling element with the specified local name and namespace URI.</summary>
		/// <param name="localName">The local name of the sibling element you wish to move to.</param>
		/// <param name="namespaceURI">The namespace URI of the sibling element you wish to move to.</param>
		/// <returns>
		///     <see langword="true" /> if a matching sibling element is found; otherwise, <see langword="false" />. If a matching sibling element is not found, the <see langword="XmlReader" /> is positioned on the end tag (<see cref="P:System.Xml.XmlReader.NodeType" /> is <see langword="XmlNodeType.EndElement" />) of the parent element.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.ArgumentNullException">Both parameter values are <see langword="null" />.</exception>
		public virtual bool ReadToNextSibling(string localName, string namespaceURI)
		{
			if (localName == null || localName.Length == 0)
			{
				throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
			}
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			localName = this.NameTable.Add(localName);
			namespaceURI = this.NameTable.Add(namespaceURI);
			while (this.SkipSubtree())
			{
				XmlNodeType nodeType = this.NodeType;
				if (nodeType == XmlNodeType.Element && Ref.Equal(localName, this.LocalName) && Ref.Equal(namespaceURI, this.NamespaceURI))
				{
					return true;
				}
				if (nodeType == XmlNodeType.EndElement || this.EOF)
				{
					break;
				}
			}
			return false;
		}

		/// <summary>Returns a value indicating whether the string argument is a valid XML name.</summary>
		/// <param name="str">The name to validate.</param>
		/// <returns>
		///     <see langword="true" /> if the name is valid; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="str" /> value is <see langword="null" />.</exception>
		public static bool IsName(string str)
		{
			if (str == null)
			{
				throw new NullReferenceException();
			}
			return ValidateNames.IsNameNoNamespaces(str);
		}

		/// <summary>Returns a value indicating whether or not the string argument is a valid XML name token.</summary>
		/// <param name="str">The name token to validate.</param>
		/// <returns>
		///     <see langword="true" /> if it is a valid name token; otherwise <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="str" /> value is <see langword="null" />.</exception>
		public static bool IsNameToken(string str)
		{
			if (str == null)
			{
				throw new NullReferenceException();
			}
			return ValidateNames.IsNmtokenNoNamespaces(str);
		}

		/// <summary>When overridden in a derived class, reads all the content, including markup, as a string.</summary>
		/// <returns>All the XML content, including markup, in the current node. If the current node has no children, an empty string is returned.If the current node is neither an element nor attribute, an empty string is returned.</returns>
		/// <exception cref="T:System.Xml.XmlException">The XML was not well-formed, or an error occurred while parsing the XML.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual string ReadInnerXml()
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return string.Empty;
			}
			if (this.NodeType != XmlNodeType.Attribute && this.NodeType != XmlNodeType.Element)
			{
				this.Read();
				return string.Empty;
			}
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlWriter xmlWriter = this.CreateWriterForInnerOuterXml(stringWriter);
			try
			{
				if (this.NodeType == XmlNodeType.Attribute)
				{
					((XmlTextWriter)xmlWriter).QuoteChar = this.QuoteChar;
					this.WriteAttributeValue(xmlWriter);
				}
				if (this.NodeType == XmlNodeType.Element)
				{
					this.WriteNode(xmlWriter, false);
				}
			}
			finally
			{
				xmlWriter.Close();
			}
			return stringWriter.ToString();
		}

		private void WriteNode(XmlWriter xtw, bool defattr)
		{
			int num = (this.NodeType == XmlNodeType.None) ? -1 : this.Depth;
			while (this.Read() && num < this.Depth)
			{
				switch (this.NodeType)
				{
				case XmlNodeType.Element:
					xtw.WriteStartElement(this.Prefix, this.LocalName, this.NamespaceURI);
					((XmlTextWriter)xtw).QuoteChar = this.QuoteChar;
					xtw.WriteAttributes(this, defattr);
					if (this.IsEmptyElement)
					{
						xtw.WriteEndElement();
					}
					break;
				case XmlNodeType.Text:
					xtw.WriteString(this.Value);
					break;
				case XmlNodeType.CDATA:
					xtw.WriteCData(this.Value);
					break;
				case XmlNodeType.EntityReference:
					xtw.WriteEntityRef(this.Name);
					break;
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.XmlDeclaration:
					xtw.WriteProcessingInstruction(this.Name, this.Value);
					break;
				case XmlNodeType.Comment:
					xtw.WriteComment(this.Value);
					break;
				case XmlNodeType.DocumentType:
					xtw.WriteDocType(this.Name, this.GetAttribute("PUBLIC"), this.GetAttribute("SYSTEM"), this.Value);
					break;
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					xtw.WriteWhitespace(this.Value);
					break;
				case XmlNodeType.EndElement:
					xtw.WriteFullEndElement();
					break;
				}
			}
			if (num == this.Depth && this.NodeType == XmlNodeType.EndElement)
			{
				this.Read();
			}
		}

		private void WriteAttributeValue(XmlWriter xtw)
		{
			string name = this.Name;
			while (this.ReadAttributeValue())
			{
				if (this.NodeType == XmlNodeType.EntityReference)
				{
					xtw.WriteEntityRef(this.Name);
				}
				else
				{
					xtw.WriteString(this.Value);
				}
			}
			this.MoveToAttribute(name);
		}

		/// <summary>When overridden in a derived class, reads the content, including markup, representing this node and all its children.</summary>
		/// <returns>If the reader is positioned on an element or an attribute node, this method returns all the XML content, including markup, of the current node and all its children; otherwise, it returns an empty string.</returns>
		/// <exception cref="T:System.Xml.XmlException">The XML was not well-formed, or an error occurred while parsing the XML.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual string ReadOuterXml()
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return string.Empty;
			}
			if (this.NodeType != XmlNodeType.Attribute && this.NodeType != XmlNodeType.Element)
			{
				this.Read();
				return string.Empty;
			}
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			XmlWriter xmlWriter = this.CreateWriterForInnerOuterXml(stringWriter);
			try
			{
				if (this.NodeType == XmlNodeType.Attribute)
				{
					xmlWriter.WriteStartAttribute(this.Prefix, this.LocalName, this.NamespaceURI);
					this.WriteAttributeValue(xmlWriter);
					xmlWriter.WriteEndAttribute();
				}
				else
				{
					xmlWriter.WriteNode(this, false);
				}
			}
			finally
			{
				xmlWriter.Close();
			}
			return stringWriter.ToString();
		}

		private XmlWriter CreateWriterForInnerOuterXml(StringWriter sw)
		{
			XmlTextWriter xmlTextWriter = new XmlTextWriter(sw);
			this.SetNamespacesFlag(xmlTextWriter);
			return xmlTextWriter;
		}

		private void SetNamespacesFlag(XmlTextWriter xtw)
		{
			XmlTextReader xmlTextReader = this as XmlTextReader;
			if (xmlTextReader != null)
			{
				xtw.Namespaces = xmlTextReader.Namespaces;
				return;
			}
			XmlValidatingReader xmlValidatingReader = this as XmlValidatingReader;
			if (xmlValidatingReader != null)
			{
				xtw.Namespaces = xmlValidatingReader.Namespaces;
			}
		}

		/// <summary>Returns a new <see langword="XmlReader" /> instance that can be used to read the current node, and all its descendants.</summary>
		/// <returns>A new XML reader instance set to <see cref="F:System.Xml.ReadState.Initial" />. Calling the <see cref="M:System.Xml.XmlReader.Read" /> method positions the new reader on the node that was current before the call to the <see cref="M:System.Xml.XmlReader.ReadSubtree" /> method.</returns>
		/// <exception cref="T:System.InvalidOperationException">The XML reader isn't positioned on an element when this method is called.</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual XmlReader ReadSubtree()
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw new InvalidOperationException(Res.GetString("ReadSubtree() can be called only if the reader is on an element node."));
			}
			return new XmlSubtreeReader(this);
		}

		/// <summary>Gets a value indicating whether the current node has any attributes.</summary>
		/// <returns>
		///     <see langword="true" /> if the current node has attributes; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public virtual bool HasAttributes
		{
			get
			{
				return this.AttributeCount > 0;
			}
		}

		/// <summary>Releases all resources used by the current instance of the <see cref="T:System.Xml.XmlReader" /> class.</summary>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>Releases the unmanaged resources used by the <see cref="T:System.Xml.XmlReader" /> and optionally releases the managed resources.</summary>
		/// <param name="disposing">
		///       <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.ReadState != ReadState.Closed)
			{
				this.Close();
			}
		}

		internal virtual XmlNamespaceManager NamespaceManager
		{
			get
			{
				return null;
			}
		}

		internal static bool IsTextualNode(XmlNodeType nodeType)
		{
			return ((ulong)XmlReader.IsTextualNodeBitmap & (ulong)(1L << (int)(nodeType & (XmlNodeType)31))) > 0UL;
		}

		internal static bool CanReadContentAs(XmlNodeType nodeType)
		{
			return ((ulong)XmlReader.CanReadContentAsBitmap & (ulong)(1L << (int)(nodeType & (XmlNodeType)31))) > 0UL;
		}

		internal static bool HasValueInternal(XmlNodeType nodeType)
		{
			return ((ulong)XmlReader.HasValueBitmap & (ulong)(1L << (int)(nodeType & (XmlNodeType)31))) > 0UL;
		}

		private bool SkipSubtree()
		{
			this.MoveToElement();
			if (this.NodeType == XmlNodeType.Element && !this.IsEmptyElement)
			{
				int depth = this.Depth;
				while (this.Read() && depth < this.Depth)
				{
				}
				return this.NodeType == XmlNodeType.EndElement && this.Read();
			}
			return this.Read();
		}

		internal void CheckElement(string localName, string namespaceURI)
		{
			if (localName == null || localName.Length == 0)
			{
				throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
			}
			if (namespaceURI == null)
			{
				throw new ArgumentNullException("namespaceURI");
			}
			if (this.NodeType != XmlNodeType.Element)
			{
				throw new XmlException("'{0}' is an invalid XmlNodeType.", this.NodeType.ToString(), this as IXmlLineInfo);
			}
			if (this.LocalName != localName || this.NamespaceURI != namespaceURI)
			{
				throw new XmlException("Element '{0}' with namespace name '{1}' was not found.", new string[]
				{
					localName,
					namespaceURI
				}, this as IXmlLineInfo);
			}
		}

		internal Exception CreateReadContentAsException(string methodName)
		{
			return XmlReader.CreateReadContentAsException(methodName, this.NodeType, this as IXmlLineInfo);
		}

		internal Exception CreateReadElementContentAsException(string methodName)
		{
			return XmlReader.CreateReadElementContentAsException(methodName, this.NodeType, this as IXmlLineInfo);
		}

		internal bool CanReadContentAs()
		{
			return XmlReader.CanReadContentAs(this.NodeType);
		}

		internal static Exception CreateReadContentAsException(string methodName, XmlNodeType nodeType, IXmlLineInfo lineInfo)
		{
			string name = "The {0} method is not supported on node type {1}. If you want to read typed content of an element, use the ReadElementContentAs method.";
			object[] args = new string[]
			{
				methodName,
				nodeType.ToString()
			};
			return new InvalidOperationException(XmlReader.AddLineInfo(Res.GetString(name, args), lineInfo));
		}

		internal static Exception CreateReadElementContentAsException(string methodName, XmlNodeType nodeType, IXmlLineInfo lineInfo)
		{
			string name = "The {0} method is not supported on node type {1}.";
			object[] args = new string[]
			{
				methodName,
				nodeType.ToString()
			};
			return new InvalidOperationException(XmlReader.AddLineInfo(Res.GetString(name, args), lineInfo));
		}

		private static string AddLineInfo(string message, IXmlLineInfo lineInfo)
		{
			if (lineInfo != null)
			{
				string[] array = new string[]
				{
					lineInfo.LineNumber.ToString(CultureInfo.InvariantCulture),
					lineInfo.LinePosition.ToString(CultureInfo.InvariantCulture)
				};
				string str = message;
				string str2 = " ";
				string name = "Line {0}, position {1}.";
				object[] args = array;
				message = str + str2 + Res.GetString(name, args);
			}
			return message;
		}

		internal string InternalReadContentAsString()
		{
			string text = string.Empty;
			StringBuilder stringBuilder = null;
			do
			{
				switch (this.NodeType)
				{
				case XmlNodeType.Attribute:
					goto IL_55;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					if (text.Length == 0)
					{
						text = this.Value;
						goto IL_9B;
					}
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
						stringBuilder.Append(text);
					}
					stringBuilder.Append(this.Value);
					goto IL_9B;
				case XmlNodeType.EntityReference:
					if (this.CanResolveEntity)
					{
						this.ResolveEntity();
						goto IL_9B;
					}
					break;
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Comment:
				case XmlNodeType.EndEntity:
					goto IL_9B;
				}
				break;
				IL_9B:;
			}
			while ((this.AttributeCount != 0) ? this.ReadAttributeValue() : this.Read());
			goto IL_B6;
			IL_55:
			return this.Value;
			IL_B6:
			if (stringBuilder != null)
			{
				return stringBuilder.ToString();
			}
			return text;
		}

		private bool SetupReadElementContentAsXxx(string methodName)
		{
			if (this.NodeType != XmlNodeType.Element)
			{
				throw this.CreateReadElementContentAsException(methodName);
			}
			bool isEmptyElement = this.IsEmptyElement;
			this.Read();
			if (isEmptyElement)
			{
				return false;
			}
			XmlNodeType nodeType = this.NodeType;
			if (nodeType == XmlNodeType.EndElement)
			{
				this.Read();
				return false;
			}
			if (nodeType == XmlNodeType.Element)
			{
				throw new XmlException("ReadElementContentAs() methods cannot be called on an element that has child elements.", string.Empty, this as IXmlLineInfo);
			}
			return true;
		}

		private void FinishReadElementContentAsXxx()
		{
			if (this.NodeType != XmlNodeType.EndElement)
			{
				throw new XmlException("'{0}' is an invalid XmlNodeType.", this.NodeType.ToString());
			}
			this.Read();
		}

		internal bool IsDefaultInternal
		{
			get
			{
				if (this.IsDefault)
				{
					return true;
				}
				IXmlSchemaInfo schemaInfo = this.SchemaInfo;
				return schemaInfo != null && schemaInfo.IsDefault;
			}
		}

		internal virtual IDtdInfo DtdInfo
		{
			get
			{
				return null;
			}
		}

		internal static Encoding GetEncoding(XmlReader reader)
		{
			XmlTextReaderImpl xmlTextReaderImpl = XmlReader.GetXmlTextReaderImpl(reader);
			if (xmlTextReaderImpl == null)
			{
				return null;
			}
			return xmlTextReaderImpl.Encoding;
		}

		internal static ConformanceLevel GetV1ConformanceLevel(XmlReader reader)
		{
			XmlTextReaderImpl xmlTextReaderImpl = XmlReader.GetXmlTextReaderImpl(reader);
			if (xmlTextReaderImpl == null)
			{
				return ConformanceLevel.Document;
			}
			return xmlTextReaderImpl.V1ComformanceLevel;
		}

		private static XmlTextReaderImpl GetXmlTextReaderImpl(XmlReader reader)
		{
			XmlTextReaderImpl xmlTextReaderImpl = reader as XmlTextReaderImpl;
			if (xmlTextReaderImpl != null)
			{
				return xmlTextReaderImpl;
			}
			XmlTextReader xmlTextReader = reader as XmlTextReader;
			if (xmlTextReader != null)
			{
				return xmlTextReader.Impl;
			}
			XmlValidatingReaderImpl xmlValidatingReaderImpl = reader as XmlValidatingReaderImpl;
			if (xmlValidatingReaderImpl != null)
			{
				return xmlValidatingReaderImpl.ReaderImpl;
			}
			XmlValidatingReader xmlValidatingReader = reader as XmlValidatingReader;
			if (xmlValidatingReader != null)
			{
				return xmlValidatingReader.Impl.ReaderImpl;
			}
			return null;
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance with specified URI.</summary>
		/// <param name="inputUri">The URI for the file that contains the XML data. The <see cref="T:System.Xml.XmlUrlResolver" /> class is used to convert the path to a canonical data representation.</param>
		/// <returns>An object that is used to read the XML data in the stream.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="inputUri" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The <see cref="T:System.Xml.XmlReader" /> does not have sufficient permissions to access the location of the XML data.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file identified by the URI does not exist.</exception>
		/// <exception cref="T:System.UriFormatException">
		///           In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.FormatException" />, instead.The URI format is not correct.</exception>
		public static XmlReader Create(string inputUri)
		{
			return XmlReader.Create(inputUri, null, null);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance by using the specified URI and settings.</summary>
		/// <param name="inputUri">The URI for the file containing the XML data. The <see cref="T:System.Xml.XmlResolver" /> object on the <see cref="T:System.Xml.XmlReaderSettings" /> object is used to convert the path to a canonical data representation. If <see cref="P:System.Xml.XmlReaderSettings.XmlResolver" /> is <see langword="null" />, a new <see cref="T:System.Xml.XmlUrlResolver" /> object is used.</param>
		/// <param name="settings">The settings for the new <see cref="T:System.Xml.XmlReader" /> instance. This value can be <see langword="null" />.</param>
		/// <returns>An object that is used to read the XML data in the stream.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="inputUri" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file specified by the URI cannot be found.</exception>
		/// <exception cref="T:System.UriFormatException">
		///           In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.FormatException" />, instead.The URI format is not correct.</exception>
		public static XmlReader Create(string inputUri, XmlReaderSettings settings)
		{
			return XmlReader.Create(inputUri, settings, null);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance by using the specified URI, settings, and context information for parsing.</summary>
		/// <param name="inputUri">The URI for the file containing the XML data. The <see cref="T:System.Xml.XmlResolver" /> object on the <see cref="T:System.Xml.XmlReaderSettings" /> object is used to convert the path to a canonical data representation. If <see cref="P:System.Xml.XmlReaderSettings.XmlResolver" /> is <see langword="null" />, a new <see cref="T:System.Xml.XmlUrlResolver" /> object is used.</param>
		/// <param name="settings">The settings for the new <see cref="T:System.Xml.XmlReader" /> instance. This value can be <see langword="null" />.</param>
		/// <param name="inputContext">The context information required to parse the XML fragment. The context information can include the <see cref="T:System.Xml.XmlNameTable" /> to use, encoding, namespace scope, the current xml:lang and xml:space scope, base URI, and document type definition. This value can be <see langword="null" />.</param>
		/// <returns>An object that is used to read the XML data in the stream.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <see langword="inputUri" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The <see cref="T:System.Xml.XmlReader" /> does not have sufficient permissions to access the location of the XML data.</exception>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Xml.XmlReaderSettings.NameTable" />  and <see cref="P:System.Xml.XmlParserContext.NameTable" /> properties both contain values. (Only one of these <see langword="NameTable" /> properties can be set and used).</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file specified by the URI cannot be found.</exception>
		/// <exception cref="T:System.UriFormatException">The URI format is not correct.</exception>
		public static XmlReader Create(string inputUri, XmlReaderSettings settings, XmlParserContext inputContext)
		{
			if (settings == null)
			{
				settings = new XmlReaderSettings();
			}
			return settings.CreateReader(inputUri, inputContext);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance using the specified stream with default settings.</summary>
		/// <param name="input">The stream that contains the XML data.The <see cref="T:System.Xml.XmlReader" /> scans the first bytes of the stream looking for a byte order mark or other sign of encoding. When encoding is determined, the encoding is used to continue reading the stream, and processing continues parsing the input as a stream of (Unicode) characters.</param>
		/// <returns>An object that is used to read the XML data in the stream.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.SecurityException">The <see cref="T:System.Xml.XmlReader" /> does not have sufficient permissions to access the location of the XML data.</exception>
		public static XmlReader Create(Stream input)
		{
			return XmlReader.Create(input, null, string.Empty);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance with the specified stream and settings.</summary>
		/// <param name="input">The stream that contains the XML data.The <see cref="T:System.Xml.XmlReader" /> scans the first bytes of the stream looking for a byte order mark or other sign of encoding. When encoding is determined, the encoding is used to continue reading the stream, and processing continues parsing the input as a stream of (Unicode) characters.</param>
		/// <param name="settings">The settings for the new <see cref="T:System.Xml.XmlReader" /> instance. This value can be <see langword="null" />.</param>
		/// <returns>An object that is used to read the XML data in the stream.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> value is <see langword="null" />.</exception>
		public static XmlReader Create(Stream input, XmlReaderSettings settings)
		{
			return XmlReader.Create(input, settings, string.Empty);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance using the specified stream, base URI, and settings.</summary>
		/// <param name="input">The stream that contains the XML data. The <see cref="T:System.Xml.XmlReader" /> scans the first bytes of the stream looking for a byte order mark or other sign of encoding. When encoding is determined, the encoding is used to continue reading the stream, and processing continues parsing the input as a stream of (Unicode) characters.</param>
		/// <param name="settings">The settings for the new <see cref="T:System.Xml.XmlReader" /> instance. This value can be <see langword="null" />.</param>
		/// <param name="baseUri">The base URI for the entity or document being read. This value can be <see langword="null" />.
		///       Security Note   The base URI is used to resolve the relative URI of the XML document. Do not use a base URI from an untrusted source.</param>
		/// <returns>An object that is used to read the XML data in the stream.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> value is <see langword="null" />.</exception>
		public static XmlReader Create(Stream input, XmlReaderSettings settings, string baseUri)
		{
			if (settings == null)
			{
				settings = new XmlReaderSettings();
			}
			return settings.CreateReader(input, null, baseUri, null);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance using the specified stream, settings, and context information for parsing.</summary>
		/// <param name="input">The stream that contains the XML data. The <see cref="T:System.Xml.XmlReader" /> scans the first bytes of the stream looking for a byte order mark or other sign of encoding. When encoding is determined, the encoding is used to continue reading the stream, and processing continues parsing the input as a stream of (Unicode) characters.</param>
		/// <param name="settings">The settings for the new <see cref="T:System.Xml.XmlReader" /> instance. This value can be <see langword="null" />.</param>
		/// <param name="inputContext">The context information required to parse the XML fragment. The context information can include the <see cref="T:System.Xml.XmlNameTable" /> to use, encoding, namespace scope, the current xml:lang and xml:space scope, base URI, and document type definition. This value can be <see langword="null" />.</param>
		/// <returns>An object that is used to read the XML data in the stream.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> value is <see langword="null" />.</exception>
		public static XmlReader Create(Stream input, XmlReaderSettings settings, XmlParserContext inputContext)
		{
			if (settings == null)
			{
				settings = new XmlReaderSettings();
			}
			return settings.CreateReader(input, null, string.Empty, inputContext);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance by using the specified text reader.</summary>
		/// <param name="input">The text reader from which to read the XML data. A text reader returns a stream of Unicode characters, so the encoding specified in the XML declaration is not used by the XML reader to decode the data stream.</param>
		/// <returns>An object that is used to read the XML data in the stream.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> value is <see langword="null" />.</exception>
		public static XmlReader Create(TextReader input)
		{
			return XmlReader.Create(input, null, string.Empty);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance by using the specified text reader and settings.</summary>
		/// <param name="input">The text reader from which to read the XML data. A text reader returns a stream of Unicode characters, so the encoding specified in the XML declaration isn't used by the XML reader to decode the data stream.</param>
		/// <param name="settings">The settings for the new <see cref="T:System.Xml.XmlReader" />. This value can be <see langword="null" />.</param>
		/// <returns>An object that is used to read the XML data in the stream.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> value is <see langword="null" />.</exception>
		public static XmlReader Create(TextReader input, XmlReaderSettings settings)
		{
			return XmlReader.Create(input, settings, string.Empty);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance by using the specified text reader, settings, and base URI.</summary>
		/// <param name="input">The text reader from which to read the XML data. A text reader returns a stream of Unicode characters, so the encoding specified in the XML declaration isn't used by the <see cref="T:System.Xml.XmlReader" /> to decode the data stream.</param>
		/// <param name="settings">The settings for the new <see cref="T:System.Xml.XmlReader" /> instance. This value can be <see langword="null" />.</param>
		/// <param name="baseUri">The base URI for the entity or document being read. This value can be <see langword="null" />.
		///       Security Note   The base URI is used to resolve the relative URI of the XML document. Do not use a base URI from an untrusted source.</param>
		/// <returns>An object that is used to read the XML data in the stream.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> value is <see langword="null" />.</exception>
		public static XmlReader Create(TextReader input, XmlReaderSettings settings, string baseUri)
		{
			if (settings == null)
			{
				settings = new XmlReaderSettings();
			}
			return settings.CreateReader(input, baseUri, null);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance by using the specified text reader, settings, and context information for parsing.</summary>
		/// <param name="input">The text reader from which to read the XML data. A text reader returns a stream of Unicode characters, so the encoding specified in the XML declaration isn't used by the XML reader to decode the data stream.</param>
		/// <param name="settings">The settings for the new <see cref="T:System.Xml.XmlReader" /> instance. This value can be <see langword="null" />.</param>
		/// <param name="inputContext">The context information required to parse the XML fragment. The context information can include the <see cref="T:System.Xml.XmlNameTable" /> to use, encoding, namespace scope, the current xml:lang and xml:space scope, base URI, and document type definition.This value can be <see langword="null" />.</param>
		/// <returns>An object that is used to read the XML data in the stream.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="input" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Xml.XmlReaderSettings.NameTable" />  and <see cref="P:System.Xml.XmlParserContext.NameTable" /> properties both contain values. (Only one of these <see langword="NameTable" /> properties can be set and used).</exception>
		public static XmlReader Create(TextReader input, XmlReaderSettings settings, XmlParserContext inputContext)
		{
			if (settings == null)
			{
				settings = new XmlReaderSettings();
			}
			return settings.CreateReader(input, string.Empty, inputContext);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XmlReader" /> instance by using the specified XML reader and settings.</summary>
		/// <param name="reader">The object that you want to use as the underlying XML reader.</param>
		/// <param name="settings">The settings for the new <see cref="T:System.Xml.XmlReader" /> instance.The conformance level of the <see cref="T:System.Xml.XmlReaderSettings" /> object must either match the conformance level of the underlying reader, or it must be set to <see cref="F:System.Xml.ConformanceLevel.Auto" />.</param>
		/// <returns>An object that is wrapped around the specified <see cref="T:System.Xml.XmlReader" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="reader" /> value is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">If the <see cref="T:System.Xml.XmlReaderSettings" /> object specifies a conformance level that is not consistent with conformance level of the underlying reader.-or-The underlying <see cref="T:System.Xml.XmlReader" /> is in an <see cref="F:System.Xml.ReadState.Error" /> or <see cref="F:System.Xml.ReadState.Closed" /> state.</exception>
		public static XmlReader Create(XmlReader reader, XmlReaderSettings settings)
		{
			if (settings == null)
			{
				settings = new XmlReaderSettings();
			}
			return settings.CreateReader(reader);
		}

		internal static XmlReader CreateSqlReader(Stream input, XmlReaderSettings settings, XmlParserContext inputContext)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (settings == null)
			{
				settings = new XmlReaderSettings();
			}
			byte[] array = new byte[XmlReader.CalcBufferSize(input)];
			int num = 0;
			int num2;
			do
			{
				num2 = input.Read(array, num, array.Length - num);
				num += num2;
			}
			while (num2 > 0 && num < 2);
			XmlReader xmlReader;
			if (num >= 2 && array[0] == 223 && array[1] == 255)
			{
				if (inputContext != null)
				{
					throw new ArgumentException(Res.GetString("BinaryXml Parser does not support initialization with XmlParserContext."), "inputContext");
				}
				xmlReader = new XmlSqlBinaryReader(input, array, num, string.Empty, settings.CloseInput, settings);
			}
			else
			{
				xmlReader = new XmlTextReaderImpl(input, array, num, settings, null, string.Empty, inputContext, settings.CloseInput);
			}
			if (settings.ValidationType != ValidationType.None)
			{
				xmlReader = settings.AddValidation(xmlReader);
			}
			if (settings.Async)
			{
				xmlReader = XmlAsyncCheckReader.CreateAsyncCheckWrapper(xmlReader);
			}
			return xmlReader;
		}

		internal static int CalcBufferSize(Stream input)
		{
			int num = 4096;
			if (input.CanSeek)
			{
				long length = input.Length;
				if (length < (long)num)
				{
					num = checked((int)length);
				}
				else if (length > 65536L)
				{
					num = 8192;
				}
			}
			return num;
		}

		private object debuggerDisplayProxy
		{
			get
			{
				return new XmlReader.XmlReaderDebuggerDisplayProxy(this);
			}
		}

		/// <summary>Asynchronously gets the value of the current node.</summary>
		/// <returns>The value of the current node.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<string> GetValueAsync()
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously reads the text content at the current position as an <see cref="T:System.Object" />.</summary>
		/// <returns>The text content as the most appropriate common language runtime (CLR) object.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<object> ReadContentAsObjectAsync()
		{
			XmlReader.<ReadContentAsObjectAsync>d__184 <ReadContentAsObjectAsync>d__;
			<ReadContentAsObjectAsync>d__.<>4__this = this;
			<ReadContentAsObjectAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<ReadContentAsObjectAsync>d__.<>1__state = -1;
			<ReadContentAsObjectAsync>d__.<>t__builder.Start<XmlReader.<ReadContentAsObjectAsync>d__184>(ref <ReadContentAsObjectAsync>d__);
			return <ReadContentAsObjectAsync>d__.<>t__builder.Task;
		}

		/// <summary>Asynchronously reads the text content at the current position as a <see cref="T:System.String" /> object.</summary>
		/// <returns>The text content as a <see cref="T:System.String" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<string> ReadContentAsStringAsync()
		{
			if (!this.CanReadContentAs())
			{
				throw this.CreateReadContentAsException("ReadContentAsString");
			}
			return this.InternalReadContentAsStringAsync();
		}

		/// <summary>Asynchronously reads the content as an object of the type specified.</summary>
		/// <param name="returnType">The type of the value to be returned.</param>
		/// <param name="namespaceResolver">An <see cref="T:System.Xml.IXmlNamespaceResolver" /> object that is used to resolve any namespace prefixes related to type conversion.</param>
		/// <returns>The concatenated text content or attribute value converted to the requested type.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<object> ReadContentAsAsync(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			XmlReader.<ReadContentAsAsync>d__186 <ReadContentAsAsync>d__;
			<ReadContentAsAsync>d__.<>4__this = this;
			<ReadContentAsAsync>d__.returnType = returnType;
			<ReadContentAsAsync>d__.namespaceResolver = namespaceResolver;
			<ReadContentAsAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<ReadContentAsAsync>d__.<>1__state = -1;
			<ReadContentAsAsync>d__.<>t__builder.Start<XmlReader.<ReadContentAsAsync>d__186>(ref <ReadContentAsAsync>d__);
			return <ReadContentAsAsync>d__.<>t__builder.Task;
		}

		/// <summary>Asynchronously reads the current element and returns the contents as an <see cref="T:System.Object" />.</summary>
		/// <returns>A boxed common language runtime (CLR) object of the most appropriate type. The <see cref="P:System.Xml.XmlReader.ValueType" /> property determines the appropriate CLR type. If the content is typed as a list type, this method returns an array of boxed objects of the appropriate type.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<object> ReadElementContentAsObjectAsync()
		{
			XmlReader.<ReadElementContentAsObjectAsync>d__187 <ReadElementContentAsObjectAsync>d__;
			<ReadElementContentAsObjectAsync>d__.<>4__this = this;
			<ReadElementContentAsObjectAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<ReadElementContentAsObjectAsync>d__.<>1__state = -1;
			<ReadElementContentAsObjectAsync>d__.<>t__builder.Start<XmlReader.<ReadElementContentAsObjectAsync>d__187>(ref <ReadElementContentAsObjectAsync>d__);
			return <ReadElementContentAsObjectAsync>d__.<>t__builder.Task;
		}

		/// <summary>Asynchronously reads the current element and returns the contents as a <see cref="T:System.String" /> object.</summary>
		/// <returns>The element content as a <see cref="T:System.String" /> object.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<string> ReadElementContentAsStringAsync()
		{
			XmlReader.<ReadElementContentAsStringAsync>d__188 <ReadElementContentAsStringAsync>d__;
			<ReadElementContentAsStringAsync>d__.<>4__this = this;
			<ReadElementContentAsStringAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<ReadElementContentAsStringAsync>d__.<>1__state = -1;
			<ReadElementContentAsStringAsync>d__.<>t__builder.Start<XmlReader.<ReadElementContentAsStringAsync>d__188>(ref <ReadElementContentAsStringAsync>d__);
			return <ReadElementContentAsStringAsync>d__.<>t__builder.Task;
		}

		/// <summary>Asynchronously reads the element content as the requested type.</summary>
		/// <param name="returnType">The type of the value to be returned.</param>
		/// <param name="namespaceResolver">An <see cref="T:System.Xml.IXmlNamespaceResolver" /> object that is used to resolve any namespace prefixes related to type conversion.</param>
		/// <returns>The element content converted to the requested typed object.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<object> ReadElementContentAsAsync(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			XmlReader.<ReadElementContentAsAsync>d__189 <ReadElementContentAsAsync>d__;
			<ReadElementContentAsAsync>d__.<>4__this = this;
			<ReadElementContentAsAsync>d__.returnType = returnType;
			<ReadElementContentAsAsync>d__.namespaceResolver = namespaceResolver;
			<ReadElementContentAsAsync>d__.<>t__builder = AsyncTaskMethodBuilder<object>.Create();
			<ReadElementContentAsAsync>d__.<>1__state = -1;
			<ReadElementContentAsAsync>d__.<>t__builder.Start<XmlReader.<ReadElementContentAsAsync>d__189>(ref <ReadElementContentAsAsync>d__);
			return <ReadElementContentAsAsync>d__.<>t__builder.Task;
		}

		/// <summary>Asynchronously reads the next node from the stream.</summary>
		/// <returns>
		///     <see langword="true" /> if the next node was read successfully; <see langword="false" /> if there are no more nodes to read.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<bool> ReadAsync()
		{
			throw new NotImplementedException();
		}

		/// <summary>Asynchronously skips the children of the current node.</summary>
		/// <returns>The current node.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task SkipAsync()
		{
			if (this.ReadState != ReadState.Interactive)
			{
				return AsyncHelper.DoneTask;
			}
			return this.SkipSubtreeAsync();
		}

		/// <summary>Asynchronously reads the content and returns the Base64 decoded binary bytes.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<int> ReadContentAsBase64Async(byte[] buffer, int index, int count)
		{
			throw new NotSupportedException(Res.GetString("{0} method is not supported on this XmlReader. Use CanReadBinaryContent property to find out if a reader implements it.", new object[]
			{
				"ReadContentAsBase64"
			}));
		}

		/// <summary>Asynchronously reads the element and decodes the <see langword="Base64" /> content.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<int> ReadElementContentAsBase64Async(byte[] buffer, int index, int count)
		{
			throw new NotSupportedException(Res.GetString("{0} method is not supported on this XmlReader. Use CanReadBinaryContent property to find out if a reader implements it.", new object[]
			{
				"ReadElementContentAsBase64"
			}));
		}

		/// <summary>Asynchronously reads the content and returns the <see langword="BinHex" /> decoded binary bytes.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<int> ReadContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			throw new NotSupportedException(Res.GetString("{0} method is not supported on this XmlReader. Use CanReadBinaryContent property to find out if a reader implements it.", new object[]
			{
				"ReadContentAsBinHex"
			}));
		}

		/// <summary>Asynchronously reads the element and decodes the <see langword="BinHex" /> content.</summary>
		/// <param name="buffer">The buffer into which to copy the resulting text. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset into the buffer where to start copying the result.</param>
		/// <param name="count">The maximum number of bytes to copy into the buffer. The actual number of bytes copied is returned from this method.</param>
		/// <returns>The number of bytes written to the buffer.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<int> ReadElementContentAsBinHexAsync(byte[] buffer, int index, int count)
		{
			throw new NotSupportedException(Res.GetString("{0} method is not supported on this XmlReader. Use CanReadBinaryContent property to find out if a reader implements it.", new object[]
			{
				"ReadElementContentAsBinHex"
			}));
		}

		/// <summary>Asynchronously reads large streams of text embedded in an XML document.</summary>
		/// <param name="buffer">The array of characters that serves as the buffer to which the text contents are written. This value cannot be <see langword="null" />.</param>
		/// <param name="index">The offset within the buffer where the <see cref="T:System.Xml.XmlReader" /> can start to copy the results.</param>
		/// <param name="count">The maximum number of characters to copy into the buffer. The actual number of characters copied is returned from this method.</param>
		/// <returns>The number of characters read into the buffer. The value zero is returned when there is no more text content.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<int> ReadValueChunkAsync(char[] buffer, int index, int count)
		{
			throw new NotSupportedException(Res.GetString("ReadValueChunk method is not supported on this XmlReader. Use CanReadValueChunk property to find out if an XmlReader implements it."));
		}

		/// <summary>Asynchronously checks whether the current node is a content node. If the node is not a content node, the reader skips ahead to the next content node or end of file.</summary>
		/// <returns>The <see cref="P:System.Xml.XmlReader.NodeType" /> of the current node found by the method or <see langword="XmlNodeType.None" /> if the reader has reached the end of the input stream.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<XmlNodeType> MoveToContentAsync()
		{
			XmlReader.<MoveToContentAsync>d__197 <MoveToContentAsync>d__;
			<MoveToContentAsync>d__.<>4__this = this;
			<MoveToContentAsync>d__.<>t__builder = AsyncTaskMethodBuilder<XmlNodeType>.Create();
			<MoveToContentAsync>d__.<>1__state = -1;
			<MoveToContentAsync>d__.<>t__builder.Start<XmlReader.<MoveToContentAsync>d__197>(ref <MoveToContentAsync>d__);
			return <MoveToContentAsync>d__.<>t__builder.Task;
		}

		/// <summary>Asynchronously reads all the content, including markup, as a string.</summary>
		/// <returns>All the XML content, including markup, in the current node. If the current node has no children, an empty string is returned.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<string> ReadInnerXmlAsync()
		{
			XmlReader.<ReadInnerXmlAsync>d__198 <ReadInnerXmlAsync>d__;
			<ReadInnerXmlAsync>d__.<>4__this = this;
			<ReadInnerXmlAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<ReadInnerXmlAsync>d__.<>1__state = -1;
			<ReadInnerXmlAsync>d__.<>t__builder.Start<XmlReader.<ReadInnerXmlAsync>d__198>(ref <ReadInnerXmlAsync>d__);
			return <ReadInnerXmlAsync>d__.<>t__builder.Task;
		}

		private Task WriteNodeAsync(XmlWriter xtw, bool defattr)
		{
			XmlReader.<WriteNodeAsync>d__199 <WriteNodeAsync>d__;
			<WriteNodeAsync>d__.<>4__this = this;
			<WriteNodeAsync>d__.xtw = xtw;
			<WriteNodeAsync>d__.defattr = defattr;
			<WriteNodeAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteNodeAsync>d__.<>1__state = -1;
			<WriteNodeAsync>d__.<>t__builder.Start<XmlReader.<WriteNodeAsync>d__199>(ref <WriteNodeAsync>d__);
			return <WriteNodeAsync>d__.<>t__builder.Task;
		}

		/// <summary>Asynchronously reads the content, including markup, representing this node and all its children.</summary>
		/// <returns>If the reader is positioned on an element or an attribute node, this method returns all the XML content, including markup, of the current node and all its children; otherwise, it returns an empty string.</returns>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> method was called before a previous asynchronous operation finished. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “An asynchronous operation is already in progress.”</exception>
		/// <exception cref="T:System.InvalidOperationException">An <see cref="T:System.Xml.XmlReader" /> asynchronous method was called without setting the <see cref="P:System.Xml.XmlReaderSettings.Async" /> flag to <see langword="true" />. In this case, <see cref="T:System.InvalidOperationException" /> is thrown with the message “Set XmlReaderSettings.Async to true if you want to use Async Methods.”</exception>
		public virtual Task<string> ReadOuterXmlAsync()
		{
			XmlReader.<ReadOuterXmlAsync>d__200 <ReadOuterXmlAsync>d__;
			<ReadOuterXmlAsync>d__.<>4__this = this;
			<ReadOuterXmlAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<ReadOuterXmlAsync>d__.<>1__state = -1;
			<ReadOuterXmlAsync>d__.<>t__builder.Start<XmlReader.<ReadOuterXmlAsync>d__200>(ref <ReadOuterXmlAsync>d__);
			return <ReadOuterXmlAsync>d__.<>t__builder.Task;
		}

		private Task<bool> SkipSubtreeAsync()
		{
			XmlReader.<SkipSubtreeAsync>d__201 <SkipSubtreeAsync>d__;
			<SkipSubtreeAsync>d__.<>4__this = this;
			<SkipSubtreeAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SkipSubtreeAsync>d__.<>1__state = -1;
			<SkipSubtreeAsync>d__.<>t__builder.Start<XmlReader.<SkipSubtreeAsync>d__201>(ref <SkipSubtreeAsync>d__);
			return <SkipSubtreeAsync>d__.<>t__builder.Task;
		}

		internal Task<string> InternalReadContentAsStringAsync()
		{
			XmlReader.<InternalReadContentAsStringAsync>d__202 <InternalReadContentAsStringAsync>d__;
			<InternalReadContentAsStringAsync>d__.<>4__this = this;
			<InternalReadContentAsStringAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<InternalReadContentAsStringAsync>d__.<>1__state = -1;
			<InternalReadContentAsStringAsync>d__.<>t__builder.Start<XmlReader.<InternalReadContentAsStringAsync>d__202>(ref <InternalReadContentAsStringAsync>d__);
			return <InternalReadContentAsStringAsync>d__.<>t__builder.Task;
		}

		private Task<bool> SetupReadElementContentAsXxxAsync(string methodName)
		{
			XmlReader.<SetupReadElementContentAsXxxAsync>d__203 <SetupReadElementContentAsXxxAsync>d__;
			<SetupReadElementContentAsXxxAsync>d__.<>4__this = this;
			<SetupReadElementContentAsXxxAsync>d__.methodName = methodName;
			<SetupReadElementContentAsXxxAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SetupReadElementContentAsXxxAsync>d__.<>1__state = -1;
			<SetupReadElementContentAsXxxAsync>d__.<>t__builder.Start<XmlReader.<SetupReadElementContentAsXxxAsync>d__203>(ref <SetupReadElementContentAsXxxAsync>d__);
			return <SetupReadElementContentAsXxxAsync>d__.<>t__builder.Task;
		}

		private Task FinishReadElementContentAsXxxAsync()
		{
			if (this.NodeType != XmlNodeType.EndElement)
			{
				throw new XmlException("'{0}' is an invalid XmlNodeType.", this.NodeType.ToString());
			}
			return this.ReadAsync();
		}

		private static uint IsTextualNodeBitmap = 24600U;

		private static uint CanReadContentAsBitmap = 123324U;

		private static uint HasValueBitmap = 157084U;

		internal const int DefaultBufferSize = 4096;

		internal const int BiggerBufferSize = 8192;

		internal const int MaxStreamLengthForDefaultBufferSize = 65536;

		internal const int AsyncBufferSize = 65536;

		[DebuggerDisplay("{ToString()}")]
		private struct XmlReaderDebuggerDisplayProxy
		{
			internal XmlReaderDebuggerDisplayProxy(XmlReader reader)
			{
				this.reader = reader;
			}

			public override string ToString()
			{
				XmlNodeType nodeType = this.reader.NodeType;
				string text = nodeType.ToString();
				switch (nodeType)
				{
				case XmlNodeType.Element:
				case XmlNodeType.EntityReference:
				case XmlNodeType.EndElement:
				case XmlNodeType.EndEntity:
					text = text + ", Name=\"" + this.reader.Name + "\"";
					break;
				case XmlNodeType.Attribute:
				case XmlNodeType.ProcessingInstruction:
					text = string.Concat(new string[]
					{
						text,
						", Name=\"",
						this.reader.Name,
						"\", Value=\"",
						XmlConvert.EscapeValueForDebuggerDisplay(this.reader.Value),
						"\""
					});
					break;
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.Comment:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
				case XmlNodeType.XmlDeclaration:
					text = text + ", Value=\"" + XmlConvert.EscapeValueForDebuggerDisplay(this.reader.Value) + "\"";
					break;
				case XmlNodeType.DocumentType:
					text = text + ", Name=\"" + this.reader.Name + "'";
					text = text + ", SYSTEM=\"" + this.reader.GetAttribute("SYSTEM") + "\"";
					text = text + ", PUBLIC=\"" + this.reader.GetAttribute("PUBLIC") + "\"";
					text = text + ", Value=\"" + XmlConvert.EscapeValueForDebuggerDisplay(this.reader.Value) + "\"";
					break;
				}
				return text;
			}

			private XmlReader reader;
		}
	}
}
