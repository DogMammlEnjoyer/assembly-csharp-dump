using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Xml.Schema;
using System.Xml.XPath;

namespace System.Xml
{
	/// <summary>Represents an XML document. You can use this class to load, validate, edit, add, and position XML in a document.</summary>
	public class XmlDocument : XmlNode
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlDocument" /> class.</summary>
		public XmlDocument() : this(new XmlImplementation())
		{
		}

		/// <summary>Initializes a new instance of the <see langword="XmlDocument" /> class with the specified <see cref="T:System.Xml.XmlNameTable" />.</summary>
		/// <param name="nt">The <see langword="XmlNameTable" /> to use. </param>
		public XmlDocument(XmlNameTable nt) : this(new XmlImplementation(nt))
		{
		}

		/// <summary>Initializes a new instance of the <see langword="XmlDocument" /> class with the specified <see cref="T:System.Xml.XmlImplementation" />.</summary>
		/// <param name="imp">The <see langword="XmlImplementation" /> to use. </param>
		protected internal XmlDocument(XmlImplementation imp)
		{
			this.implementation = imp;
			this.domNameTable = new DomNameTable(this);
			XmlNameTable nameTable = this.NameTable;
			nameTable.Add(string.Empty);
			this.strDocumentName = nameTable.Add("#document");
			this.strDocumentFragmentName = nameTable.Add("#document-fragment");
			this.strCommentName = nameTable.Add("#comment");
			this.strTextName = nameTable.Add("#text");
			this.strCDataSectionName = nameTable.Add("#cdata-section");
			this.strEntityName = nameTable.Add("#entity");
			this.strID = nameTable.Add("id");
			this.strNonSignificantWhitespaceName = nameTable.Add("#whitespace");
			this.strSignificantWhitespaceName = nameTable.Add("#significant-whitespace");
			this.strXmlns = nameTable.Add("xmlns");
			this.strXml = nameTable.Add("xml");
			this.strSpace = nameTable.Add("space");
			this.strLang = nameTable.Add("lang");
			this.strReservedXmlns = nameTable.Add("http://www.w3.org/2000/xmlns/");
			this.strReservedXml = nameTable.Add("http://www.w3.org/XML/1998/namespace");
			this.strEmpty = nameTable.Add(string.Empty);
			this.baseURI = string.Empty;
			this.objLock = new object();
		}

		internal SchemaInfo DtdSchemaInfo
		{
			get
			{
				return this.schemaInfo;
			}
			set
			{
				this.schemaInfo = value;
			}
		}

		internal static void CheckName(string name)
		{
			int num = ValidateNames.ParseNmtoken(name, 0);
			if (num < name.Length)
			{
				throw new XmlException("The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(name, num));
			}
		}

		internal XmlName AddXmlName(string prefix, string localName, string namespaceURI, IXmlSchemaInfo schemaInfo)
		{
			return this.domNameTable.AddName(prefix, localName, namespaceURI, schemaInfo);
		}

		internal XmlName GetXmlName(string prefix, string localName, string namespaceURI, IXmlSchemaInfo schemaInfo)
		{
			return this.domNameTable.GetName(prefix, localName, namespaceURI, schemaInfo);
		}

		internal XmlName AddAttrXmlName(string prefix, string localName, string namespaceURI, IXmlSchemaInfo schemaInfo)
		{
			XmlName xmlName = this.AddXmlName(prefix, localName, namespaceURI, schemaInfo);
			if (!this.IsLoading)
			{
				object prefix2 = xmlName.Prefix;
				object namespaceURI2 = xmlName.NamespaceURI;
				object localName2 = xmlName.LocalName;
				if ((prefix2 == this.strXmlns || (prefix2 == this.strEmpty && localName2 == this.strXmlns)) ^ namespaceURI2 == this.strReservedXmlns)
				{
					throw new ArgumentException(Res.GetString("The namespace declaration attribute has an incorrect 'namespaceURI': '{0}'.", new object[]
					{
						namespaceURI
					}));
				}
			}
			return xmlName;
		}

		internal bool AddIdInfo(XmlName eleName, XmlName attrName)
		{
			if (this.htElementIDAttrDecl == null || this.htElementIDAttrDecl[eleName] == null)
			{
				if (this.htElementIDAttrDecl == null)
				{
					this.htElementIDAttrDecl = new Hashtable();
				}
				this.htElementIDAttrDecl.Add(eleName, attrName);
				return true;
			}
			return false;
		}

		private XmlName GetIDInfoByElement_(XmlName eleName)
		{
			XmlName xmlName = this.GetXmlName(eleName.Prefix, eleName.LocalName, string.Empty, null);
			if (xmlName != null)
			{
				return (XmlName)this.htElementIDAttrDecl[xmlName];
			}
			return null;
		}

		internal XmlName GetIDInfoByElement(XmlName eleName)
		{
			if (this.htElementIDAttrDecl == null)
			{
				return null;
			}
			return this.GetIDInfoByElement_(eleName);
		}

		private WeakReference GetElement(ArrayList elementList, XmlElement elem)
		{
			ArrayList arrayList = new ArrayList();
			foreach (object obj in elementList)
			{
				WeakReference weakReference = (WeakReference)obj;
				if (!weakReference.IsAlive)
				{
					arrayList.Add(weakReference);
				}
				else if ((XmlElement)weakReference.Target == elem)
				{
					return weakReference;
				}
			}
			foreach (object obj2 in arrayList)
			{
				WeakReference obj3 = (WeakReference)obj2;
				elementList.Remove(obj3);
			}
			return null;
		}

		internal void AddElementWithId(string id, XmlElement elem)
		{
			if (this.htElementIdMap == null || !this.htElementIdMap.Contains(id))
			{
				if (this.htElementIdMap == null)
				{
					this.htElementIdMap = new Hashtable();
				}
				ArrayList arrayList = new ArrayList();
				arrayList.Add(new WeakReference(elem));
				this.htElementIdMap.Add(id, arrayList);
				return;
			}
			ArrayList arrayList2 = (ArrayList)this.htElementIdMap[id];
			if (this.GetElement(arrayList2, elem) == null)
			{
				arrayList2.Add(new WeakReference(elem));
			}
		}

		internal void RemoveElementWithId(string id, XmlElement elem)
		{
			if (this.htElementIdMap != null && this.htElementIdMap.Contains(id))
			{
				ArrayList arrayList = (ArrayList)this.htElementIdMap[id];
				WeakReference element = this.GetElement(arrayList, elem);
				if (element != null)
				{
					arrayList.Remove(element);
					if (arrayList.Count == 0)
					{
						this.htElementIdMap.Remove(id);
					}
				}
			}
		}

		/// <summary>Creates a duplicate of this node.</summary>
		/// <param name="deep">
		///       <see langword="true" /> to recursively clone the subtree under the specified node; <see langword="false" /> to clone only the node itself. </param>
		/// <returns>The cloned <see langword="XmlDocument" /> node.</returns>
		public override XmlNode CloneNode(bool deep)
		{
			XmlDocument xmlDocument = this.Implementation.CreateDocument();
			xmlDocument.SetBaseURI(this.baseURI);
			if (deep)
			{
				xmlDocument.ImportChildren(this, xmlDocument, deep);
			}
			return xmlDocument;
		}

		/// <summary>Gets the type of the current node.</summary>
		/// <returns>The node type. For <see langword="XmlDocument" /> nodes, this value is XmlNodeType.Document.</returns>
		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.Document;
			}
		}

		/// <summary>Gets the parent node of this node (for nodes that can have parents).</summary>
		/// <returns>Always returns <see langword="null" />.</returns>
		public override XmlNode ParentNode
		{
			get
			{
				return null;
			}
		}

		/// <summary>Gets the node containing the DOCTYPE declaration.</summary>
		/// <returns>The <see cref="T:System.Xml.XmlNode" /> containing the DocumentType (DOCTYPE declaration).</returns>
		public virtual XmlDocumentType DocumentType
		{
			get
			{
				return (XmlDocumentType)this.FindChild(XmlNodeType.DocumentType);
			}
		}

		internal virtual XmlDeclaration Declaration
		{
			get
			{
				if (this.HasChildNodes)
				{
					return this.FirstChild as XmlDeclaration;
				}
				return null;
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.XmlImplementation" /> object for the current document.</summary>
		/// <returns>The <see langword="XmlImplementation" /> object for the current document.</returns>
		public XmlImplementation Implementation
		{
			get
			{
				return this.implementation;
			}
		}

		/// <summary>Gets the qualified name of the node.</summary>
		/// <returns>For <see langword="XmlDocument" /> nodes, the name is #document.</returns>
		public override string Name
		{
			get
			{
				return this.strDocumentName;
			}
		}

		/// <summary>Gets the local name of the node.</summary>
		/// <returns>For <see langword="XmlDocument" /> nodes, the local name is #document.</returns>
		public override string LocalName
		{
			get
			{
				return this.strDocumentName;
			}
		}

		/// <summary>Gets the root <see cref="T:System.Xml.XmlElement" /> for the document.</summary>
		/// <returns>The <see langword="XmlElement" /> that represents the root of the XML document tree. If no root exists, <see langword="null" /> is returned.</returns>
		public XmlElement DocumentElement
		{
			get
			{
				return (XmlElement)this.FindChild(XmlNodeType.Element);
			}
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

		/// <summary>Gets the <see cref="T:System.Xml.XmlDocument" /> to which the current node belongs.</summary>
		/// <returns>For <see langword="XmlDocument" /> nodes (<see cref="P:System.Xml.XmlDocument.NodeType" /> equals XmlNodeType.Document), this property always returns <see langword="null" />.</returns>
		public override XmlDocument OwnerDocument
		{
			get
			{
				return null;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Xml.Schema.XmlSchemaSet" /> object associated with this <see cref="T:System.Xml.XmlDocument" />.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaSet" /> object containing the XML Schema Definition Language (XSD) schemas associated with this <see cref="T:System.Xml.XmlDocument" />; otherwise, an empty <see cref="T:System.Xml.Schema.XmlSchemaSet" /> object.</returns>
		public XmlSchemaSet Schemas
		{
			get
			{
				if (this.schemas == null)
				{
					this.schemas = new XmlSchemaSet(this.NameTable);
				}
				return this.schemas;
			}
			set
			{
				this.schemas = value;
			}
		}

		internal bool CanReportValidity
		{
			get
			{
				return this.reportValidity;
			}
		}

		internal bool HasSetResolver
		{
			get
			{
				return this.bSetResolver;
			}
		}

		internal XmlResolver GetResolver()
		{
			return this.resolver;
		}

		/// <summary>Sets the <see cref="T:System.Xml.XmlResolver" /> to use for resolving external resources.</summary>
		/// <returns>The <see langword="XmlResolver" /> to use.In version 1.1 of the.NET Framework, the caller must be fully trusted in order to specify an <see langword="XmlResolver" />.</returns>
		/// <exception cref="T:System.Xml.XmlException">This property is set to <see langword="null" /> and an external DTD or entity is encountered. </exception>
		public virtual XmlResolver XmlResolver
		{
			set
			{
				if (value != null)
				{
					try
					{
						new NamedPermissionSet("FullTrust").Demand();
					}
					catch (SecurityException inner)
					{
						throw new SecurityException(Res.GetString("XmlResolver can be set only by fully trusted code."), inner);
					}
				}
				this.resolver = value;
				if (!this.bSetResolver)
				{
					this.bSetResolver = true;
				}
				XmlDocumentType documentType = this.DocumentType;
				if (documentType != null)
				{
					documentType.DtdSchemaInfo = null;
				}
			}
		}

		internal override bool IsValidChildType(XmlNodeType type)
		{
			if (type != XmlNodeType.Element)
			{
				switch (type)
				{
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Comment:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					return true;
				case XmlNodeType.DocumentType:
					if (this.DocumentType != null)
					{
						throw new InvalidOperationException(Res.GetString("This document already has a 'DocumentType' node."));
					}
					return true;
				case XmlNodeType.XmlDeclaration:
					if (this.Declaration != null)
					{
						throw new InvalidOperationException(Res.GetString("This document already has an 'XmlDeclaration' node."));
					}
					return true;
				}
				return false;
			}
			if (this.DocumentElement != null)
			{
				throw new InvalidOperationException(Res.GetString("This document already has a 'DocumentElement' node."));
			}
			return true;
		}

		private bool HasNodeTypeInPrevSiblings(XmlNodeType nt, XmlNode refNode)
		{
			if (refNode == null)
			{
				return false;
			}
			XmlNode xmlNode = null;
			if (refNode.ParentNode != null)
			{
				xmlNode = refNode.ParentNode.FirstChild;
			}
			while (xmlNode != null)
			{
				if (xmlNode.NodeType == nt)
				{
					return true;
				}
				if (xmlNode == refNode)
				{
					break;
				}
				xmlNode = xmlNode.NextSibling;
			}
			return false;
		}

		private bool HasNodeTypeInNextSiblings(XmlNodeType nt, XmlNode refNode)
		{
			for (XmlNode xmlNode = refNode; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.NodeType == nt)
				{
					return true;
				}
			}
			return false;
		}

		internal override bool CanInsertBefore(XmlNode newChild, XmlNode refChild)
		{
			if (refChild == null)
			{
				refChild = this.FirstChild;
			}
			if (refChild == null)
			{
				return true;
			}
			XmlNodeType nodeType = newChild.NodeType;
			if (nodeType <= XmlNodeType.Comment)
			{
				if (nodeType != XmlNodeType.Element)
				{
					if (nodeType - XmlNodeType.ProcessingInstruction <= 1)
					{
						return refChild.NodeType != XmlNodeType.XmlDeclaration;
					}
				}
				else if (refChild.NodeType != XmlNodeType.XmlDeclaration)
				{
					return !this.HasNodeTypeInNextSiblings(XmlNodeType.DocumentType, refChild);
				}
			}
			else if (nodeType != XmlNodeType.DocumentType)
			{
				if (nodeType == XmlNodeType.XmlDeclaration)
				{
					return refChild == this.FirstChild;
				}
			}
			else if (refChild.NodeType != XmlNodeType.XmlDeclaration)
			{
				return !this.HasNodeTypeInPrevSiblings(XmlNodeType.Element, refChild.PreviousSibling);
			}
			return false;
		}

		internal override bool CanInsertAfter(XmlNode newChild, XmlNode refChild)
		{
			if (refChild == null)
			{
				refChild = this.LastChild;
			}
			if (refChild == null)
			{
				return true;
			}
			XmlNodeType nodeType = newChild.NodeType;
			if (nodeType != XmlNodeType.Element)
			{
				switch (nodeType)
				{
				case XmlNodeType.ProcessingInstruction:
				case XmlNodeType.Comment:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					return true;
				case XmlNodeType.DocumentType:
					return !this.HasNodeTypeInPrevSiblings(XmlNodeType.Element, refChild);
				}
				return false;
			}
			return !this.HasNodeTypeInNextSiblings(XmlNodeType.DocumentType, refChild.NextSibling);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlAttribute" /> with the specified <see cref="P:System.Xml.XmlDocument.Name" />.</summary>
		/// <param name="name">The qualified name of the attribute. If the name contains a colon, the <see cref="P:System.Xml.XmlNode.Prefix" /> property reflects the part of the name preceding the first colon and the <see cref="P:System.Xml.XmlDocument.LocalName" /> property reflects the part of the name following the first colon. The <see cref="P:System.Xml.XmlNode.NamespaceURI" /> remains empty unless the prefix is a recognized built-in prefix such as xmlns. In this case <see langword="NamespaceURI" /> has a value of http://www.w3.org/2000/xmlns/. </param>
		/// <returns>The new <see langword="XmlAttribute" />.</returns>
		public XmlAttribute CreateAttribute(string name)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			XmlNode.SplitName(name, out empty, out empty2);
			this.SetDefaultNamespace(empty, empty2, ref empty3);
			return this.CreateAttribute(empty, empty2, empty3);
		}

		internal void SetDefaultNamespace(string prefix, string localName, ref string namespaceURI)
		{
			if (prefix == this.strXmlns || (prefix.Length == 0 && localName == this.strXmlns))
			{
				namespaceURI = this.strReservedXmlns;
				return;
			}
			if (prefix == this.strXml)
			{
				namespaceURI = this.strReservedXml;
			}
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlCDataSection" /> containing the specified data.</summary>
		/// <param name="data">The content of the new <see langword="XmlCDataSection" />. </param>
		/// <returns>The new <see langword="XmlCDataSection" />.</returns>
		public virtual XmlCDataSection CreateCDataSection(string data)
		{
			this.fCDataNodesPresent = true;
			return new XmlCDataSection(data, this);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlComment" /> containing the specified data.</summary>
		/// <param name="data">The content of the new <see langword="XmlComment" />. </param>
		/// <returns>The new <see langword="XmlComment" />.</returns>
		public virtual XmlComment CreateComment(string data)
		{
			return new XmlComment(data, this);
		}

		/// <summary>Returns a new <see cref="T:System.Xml.XmlDocumentType" /> object.</summary>
		/// <param name="name">Name of the document type. </param>
		/// <param name="publicId">The public identifier of the document type or <see langword="null" />. You can specify a public URI and also a system identifier to identify the location of the external DTD subset.</param>
		/// <param name="systemId">The system identifier of the document type or <see langword="null" />. Specifies the URL of the file location for the external DTD subset.</param>
		/// <param name="internalSubset">The DTD internal subset of the document type or <see langword="null" />. </param>
		/// <returns>The new <see langword="XmlDocumentType" />.</returns>
		[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
		public virtual XmlDocumentType CreateDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			return new XmlDocumentType(name, publicId, systemId, internalSubset, this);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlDocumentFragment" />.</summary>
		/// <returns>The new <see langword="XmlDocumentFragment" />.</returns>
		public virtual XmlDocumentFragment CreateDocumentFragment()
		{
			return new XmlDocumentFragment(this);
		}

		/// <summary>Creates an element with the specified name.</summary>
		/// <param name="name">The qualified name of the element. If the name contains a colon then the <see cref="P:System.Xml.XmlNode.Prefix" /> property reflects the part of the name preceding the colon and the <see cref="P:System.Xml.XmlDocument.LocalName" /> property reflects the part of the name after the colon. The qualified name cannot include a prefix of'xmlns'. </param>
		/// <returns>The new <see langword="XmlElement" />.</returns>
		public XmlElement CreateElement(string name)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			XmlNode.SplitName(name, out empty, out empty2);
			return this.CreateElement(empty, empty2, string.Empty);
		}

		internal void AddDefaultAttributes(XmlElement elem)
		{
			SchemaInfo dtdSchemaInfo = this.DtdSchemaInfo;
			SchemaElementDecl schemaElementDecl = this.GetSchemaElementDecl(elem);
			if (schemaElementDecl != null && schemaElementDecl.AttDefs != null)
			{
				IDictionaryEnumerator dictionaryEnumerator = schemaElementDecl.AttDefs.GetEnumerator();
				while (dictionaryEnumerator.MoveNext())
				{
					SchemaAttDef schemaAttDef = (SchemaAttDef)dictionaryEnumerator.Value;
					if (schemaAttDef.Presence == SchemaDeclBase.Use.Default || schemaAttDef.Presence == SchemaDeclBase.Use.Fixed)
					{
						string attrPrefix = string.Empty;
						string name = schemaAttDef.Name.Name;
						string attrNamespaceURI = string.Empty;
						if (dtdSchemaInfo.SchemaType == SchemaType.DTD)
						{
							attrPrefix = schemaAttDef.Name.Namespace;
						}
						else
						{
							attrPrefix = schemaAttDef.Prefix;
							attrNamespaceURI = schemaAttDef.Name.Namespace;
						}
						XmlAttribute attributeNode = this.PrepareDefaultAttribute(schemaAttDef, attrPrefix, name, attrNamespaceURI);
						elem.SetAttributeNode(attributeNode);
					}
				}
			}
		}

		private SchemaElementDecl GetSchemaElementDecl(XmlElement elem)
		{
			SchemaInfo dtdSchemaInfo = this.DtdSchemaInfo;
			if (dtdSchemaInfo != null)
			{
				XmlQualifiedName key = new XmlQualifiedName(elem.LocalName, (dtdSchemaInfo.SchemaType == SchemaType.DTD) ? elem.Prefix : elem.NamespaceURI);
				SchemaElementDecl result;
				if (dtdSchemaInfo.ElementDecls.TryGetValue(key, out result))
				{
					return result;
				}
			}
			return null;
		}

		private XmlAttribute PrepareDefaultAttribute(SchemaAttDef attdef, string attrPrefix, string attrLocalname, string attrNamespaceURI)
		{
			this.SetDefaultNamespace(attrPrefix, attrLocalname, ref attrNamespaceURI);
			XmlAttribute xmlAttribute = this.CreateDefaultAttribute(attrPrefix, attrLocalname, attrNamespaceURI);
			xmlAttribute.InnerXml = attdef.DefaultValueRaw;
			XmlUnspecifiedAttribute xmlUnspecifiedAttribute = xmlAttribute as XmlUnspecifiedAttribute;
			if (xmlUnspecifiedAttribute != null)
			{
				xmlUnspecifiedAttribute.SetSpecified(false);
			}
			return xmlAttribute;
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlEntityReference" /> with the specified name.</summary>
		/// <param name="name">The name of the entity reference. </param>
		/// <returns>The new <see langword="XmlEntityReference" />.</returns>
		/// <exception cref="T:System.ArgumentException">The name is invalid (for example, names starting with'#' are invalid.) </exception>
		public virtual XmlEntityReference CreateEntityReference(string name)
		{
			return new XmlEntityReference(name, this);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlProcessingInstruction" /> with the specified name and data.</summary>
		/// <param name="target">The name of the processing instruction. </param>
		/// <param name="data">The data for the processing instruction. </param>
		/// <returns>The new <see langword="XmlProcessingInstruction" />.</returns>
		public virtual XmlProcessingInstruction CreateProcessingInstruction(string target, string data)
		{
			return new XmlProcessingInstruction(target, data, this);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlDeclaration" /> node with the specified values.</summary>
		/// <param name="version">The version must be "1.0". </param>
		/// <param name="encoding">The value of the encoding attribute. This is the encoding that is used when you save the <see cref="T:System.Xml.XmlDocument" /> to a file or a stream; therefore, it must be set to a string supported by the <see cref="T:System.Text.Encoding" /> class, otherwise <see cref="M:System.Xml.XmlDocument.Save(System.String)" /> fails. If this is <see langword="null" /> or String.Empty, the <see langword="Save" /> method does not write an encoding attribute on the XML declaration and therefore the default encoding, UTF-8, is used.Note: If the <see langword="XmlDocument" /> is saved to either a <see cref="T:System.IO.TextWriter" /> or an <see cref="T:System.Xml.XmlTextWriter" />, this encoding value is discarded. Instead, the encoding of the <see langword="TextWriter" /> or the <see langword="XmlTextWriter" /> is used. This ensures that the XML written out can be read back using the correct encoding. </param>
		/// <param name="standalone">The value must be either "yes" or "no". If this is <see langword="null" /> or String.Empty, the <see langword="Save" /> method does not write a standalone attribute on the XML declaration. </param>
		/// <returns>The new <see langword="XmlDeclaration" /> node.</returns>
		/// <exception cref="T:System.ArgumentException">The values of <paramref name="version" /> or <paramref name="standalone" /> are something other than the ones specified above. </exception>
		public virtual XmlDeclaration CreateXmlDeclaration(string version, string encoding, string standalone)
		{
			return new XmlDeclaration(version, encoding, standalone, this);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlText" /> with the specified text.</summary>
		/// <param name="text">The text for the Text node. </param>
		/// <returns>The new <see langword="XmlText" /> node.</returns>
		public virtual XmlText CreateTextNode(string text)
		{
			return new XmlText(text, this);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlSignificantWhitespace" /> node.</summary>
		/// <param name="text">The string must contain only the following characters &amp;#20; &amp;#10; &amp;#13; and &amp;#9; </param>
		/// <returns>A new <see langword="XmlSignificantWhitespace" /> node.</returns>
		public virtual XmlSignificantWhitespace CreateSignificantWhitespace(string text)
		{
			return new XmlSignificantWhitespace(text, this);
		}

		/// <summary>Creates a new <see cref="T:System.Xml.XPath.XPathNavigator" /> object for navigating this document.</summary>
		/// <returns>An <see cref="T:System.Xml.XPath.XPathNavigator" /> object.</returns>
		public override XPathNavigator CreateNavigator()
		{
			return this.CreateNavigator(this);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XPath.XPathNavigator" /> object for navigating this document positioned on the <see cref="T:System.Xml.XmlNode" /> specified.</summary>
		/// <param name="node">The <see cref="T:System.Xml.XmlNode" /> you want the navigator initially positioned on. </param>
		/// <returns>An <see cref="T:System.Xml.XPath.XPathNavigator" /> object.</returns>
		protected internal virtual XPathNavigator CreateNavigator(XmlNode node)
		{
			switch (node.NodeType)
			{
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.SignificantWhitespace:
			{
				XmlNode parentNode = node.ParentNode;
				if (parentNode != null)
				{
					for (;;)
					{
						XmlNodeType nodeType = parentNode.NodeType;
						if (nodeType == XmlNodeType.Attribute)
						{
							break;
						}
						if (nodeType != XmlNodeType.EntityReference)
						{
							goto IL_74;
						}
						parentNode = parentNode.ParentNode;
						if (parentNode == null)
						{
							goto IL_74;
						}
					}
					return null;
				}
				IL_74:
				node = this.NormalizeText(node);
				break;
			}
			case XmlNodeType.EntityReference:
			case XmlNodeType.Entity:
			case XmlNodeType.DocumentType:
			case XmlNodeType.Notation:
			case XmlNodeType.XmlDeclaration:
				return null;
			case XmlNodeType.Whitespace:
			{
				XmlNode parentNode = node.ParentNode;
				if (parentNode != null)
				{
					for (;;)
					{
						XmlNodeType nodeType = parentNode.NodeType;
						if (nodeType == XmlNodeType.Document || nodeType == XmlNodeType.Attribute)
						{
							break;
						}
						if (nodeType != XmlNodeType.EntityReference)
						{
							goto IL_A9;
						}
						parentNode = parentNode.ParentNode;
						if (parentNode == null)
						{
							goto IL_A9;
						}
					}
					return null;
				}
				IL_A9:
				node = this.NormalizeText(node);
				break;
			}
			}
			return new DocumentXPathNavigator(this, node);
		}

		internal static bool IsTextNode(XmlNodeType nt)
		{
			return nt - XmlNodeType.Text <= 1 || nt - XmlNodeType.Whitespace <= 1;
		}

		private XmlNode NormalizeText(XmlNode n)
		{
			XmlNode xmlNode = null;
			while (XmlDocument.IsTextNode(n.NodeType))
			{
				xmlNode = n;
				n = n.PreviousSibling;
				if (n == null)
				{
					XmlNode xmlNode2 = xmlNode;
					while (xmlNode2.ParentNode != null && xmlNode2.ParentNode.NodeType == XmlNodeType.EntityReference)
					{
						if (xmlNode2.ParentNode.PreviousSibling != null)
						{
							n = xmlNode2.ParentNode.PreviousSibling;
							break;
						}
						xmlNode2 = xmlNode2.ParentNode;
						if (xmlNode2 == null)
						{
							break;
						}
					}
				}
				if (n == null)
				{
					break;
				}
				while (n.NodeType == XmlNodeType.EntityReference)
				{
					n = n.LastChild;
				}
			}
			return xmlNode;
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlWhitespace" /> node.</summary>
		/// <param name="text">The string must contain only the following characters &amp;#20; &amp;#10; &amp;#13; and &amp;#9; </param>
		/// <returns>A new <see langword="XmlWhitespace" /> node.</returns>
		public virtual XmlWhitespace CreateWhitespace(string text)
		{
			return new XmlWhitespace(text, this);
		}

		/// <summary>Returns an <see cref="T:System.Xml.XmlNodeList" /> containing a list of all descendant elements that match the specified <see cref="P:System.Xml.XmlDocument.Name" />.</summary>
		/// <param name="name">The qualified name to match. It is matched against the <see langword="Name" /> property of the matching node. The special value "*" matches all tags. </param>
		/// <returns>An <see cref="T:System.Xml.XmlNodeList" /> containing a list of all matching nodes. If no nodes match <paramref name="name" />, the returned collection will be empty.</returns>
		public virtual XmlNodeList GetElementsByTagName(string name)
		{
			return new XmlElementList(this, name);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlAttribute" /> with the specified qualified name and <see cref="P:System.Xml.XmlNode.NamespaceURI" />.</summary>
		/// <param name="qualifiedName">The qualified name of the attribute. If the name contains a colon then the <see cref="P:System.Xml.XmlNode.Prefix" /> property will reflect the part of the name preceding the colon and the <see cref="P:System.Xml.XmlDocument.LocalName" /> property will reflect the part of the name after the colon. </param>
		/// <param name="namespaceURI">The namespaceURI of the attribute. If the qualified name includes a prefix of xmlns, then this parameter must be http://www.w3.org/2000/xmlns/. </param>
		/// <returns>The new <see langword="XmlAttribute" />.</returns>
		public XmlAttribute CreateAttribute(string qualifiedName, string namespaceURI)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			XmlNode.SplitName(qualifiedName, out empty, out empty2);
			return this.CreateAttribute(empty, empty2, namespaceURI);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlElement" /> with the qualified name and <see cref="P:System.Xml.XmlNode.NamespaceURI" />.</summary>
		/// <param name="qualifiedName">The qualified name of the element. If the name contains a colon then the <see cref="P:System.Xml.XmlNode.Prefix" /> property will reflect the part of the name preceding the colon and the <see cref="P:System.Xml.XmlDocument.LocalName" /> property will reflect the part of the name after the colon. The qualified name cannot include a prefix of'xmlns'. </param>
		/// <param name="namespaceURI">The namespace URI of the element. </param>
		/// <returns>The new <see langword="XmlElement" />.</returns>
		public XmlElement CreateElement(string qualifiedName, string namespaceURI)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			XmlNode.SplitName(qualifiedName, out empty, out empty2);
			return this.CreateElement(empty, empty2, namespaceURI);
		}

		/// <summary>Returns an <see cref="T:System.Xml.XmlNodeList" /> containing a list of all descendant elements that match the specified <see cref="P:System.Xml.XmlDocument.LocalName" /> and <see cref="P:System.Xml.XmlNode.NamespaceURI" />.</summary>
		/// <param name="localName">The LocalName to match. The special value "*" matches all tags. </param>
		/// <param name="namespaceURI">NamespaceURI to match. </param>
		/// <returns>An <see cref="T:System.Xml.XmlNodeList" /> containing a list of all matching nodes. If no nodes match the specified <paramref name="localName" /> and <paramref name="namespaceURI" />, the returned collection will be empty.</returns>
		public virtual XmlNodeList GetElementsByTagName(string localName, string namespaceURI)
		{
			return new XmlElementList(this, localName, namespaceURI);
		}

		/// <summary>Gets the <see cref="T:System.Xml.XmlElement" /> with the specified ID.</summary>
		/// <param name="elementId">The attribute ID to match. </param>
		/// <returns>The <see langword="XmlElement" /> with the matching ID or <see langword="null" /> if no matching element is found.</returns>
		public virtual XmlElement GetElementById(string elementId)
		{
			if (this.htElementIdMap != null)
			{
				ArrayList arrayList = (ArrayList)this.htElementIdMap[elementId];
				if (arrayList != null)
				{
					foreach (object obj in arrayList)
					{
						XmlElement xmlElement = (XmlElement)((WeakReference)obj).Target;
						if (xmlElement != null && xmlElement.IsConnected())
						{
							return xmlElement;
						}
					}
				}
			}
			return null;
		}

		/// <summary>Imports a node from another document to the current document.</summary>
		/// <param name="node">The node being imported. </param>
		/// <param name="deep">
		///       <see langword="true" /> to perform a deep clone; otherwise, <see langword="false" />. </param>
		/// <returns>The imported <see cref="T:System.Xml.XmlNode" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">Calling this method on a node type which cannot be imported. </exception>
		public virtual XmlNode ImportNode(XmlNode node, bool deep)
		{
			return this.ImportNodeInternal(node, deep);
		}

		private XmlNode ImportNodeInternal(XmlNode node, bool deep)
		{
			if (node == null)
			{
				throw new InvalidOperationException(Res.GetString("Cannot import a null node."));
			}
			switch (node.NodeType)
			{
			case XmlNodeType.Element:
			{
				XmlNode xmlNode = this.CreateElement(node.Prefix, node.LocalName, node.NamespaceURI);
				this.ImportAttributes(node, xmlNode);
				if (deep)
				{
					this.ImportChildren(node, xmlNode, deep);
					return xmlNode;
				}
				return xmlNode;
			}
			case XmlNodeType.Attribute:
			{
				XmlNode xmlNode = this.CreateAttribute(node.Prefix, node.LocalName, node.NamespaceURI);
				this.ImportChildren(node, xmlNode, true);
				return xmlNode;
			}
			case XmlNodeType.Text:
				return this.CreateTextNode(node.Value);
			case XmlNodeType.CDATA:
				return this.CreateCDataSection(node.Value);
			case XmlNodeType.EntityReference:
				return this.CreateEntityReference(node.Name);
			case XmlNodeType.ProcessingInstruction:
				return this.CreateProcessingInstruction(node.Name, node.Value);
			case XmlNodeType.Comment:
				return this.CreateComment(node.Value);
			case XmlNodeType.DocumentType:
			{
				XmlDocumentType xmlDocumentType = (XmlDocumentType)node;
				return this.CreateDocumentType(xmlDocumentType.Name, xmlDocumentType.PublicId, xmlDocumentType.SystemId, xmlDocumentType.InternalSubset);
			}
			case XmlNodeType.DocumentFragment:
			{
				XmlNode xmlNode = this.CreateDocumentFragment();
				if (deep)
				{
					this.ImportChildren(node, xmlNode, deep);
					return xmlNode;
				}
				return xmlNode;
			}
			case XmlNodeType.Whitespace:
				return this.CreateWhitespace(node.Value);
			case XmlNodeType.SignificantWhitespace:
				return this.CreateSignificantWhitespace(node.Value);
			case XmlNodeType.XmlDeclaration:
			{
				XmlDeclaration xmlDeclaration = (XmlDeclaration)node;
				return this.CreateXmlDeclaration(xmlDeclaration.Version, xmlDeclaration.Encoding, xmlDeclaration.Standalone);
			}
			}
			throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, Res.GetString("Cannot import nodes of type '{0}'."), node.NodeType.ToString()));
		}

		private void ImportAttributes(XmlNode fromElem, XmlNode toElem)
		{
			int count = fromElem.Attributes.Count;
			for (int i = 0; i < count; i++)
			{
				if (fromElem.Attributes[i].Specified)
				{
					toElem.Attributes.SetNamedItem(this.ImportNodeInternal(fromElem.Attributes[i], true));
				}
			}
		}

		private void ImportChildren(XmlNode fromNode, XmlNode toNode, bool deep)
		{
			for (XmlNode xmlNode = fromNode.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				toNode.AppendChild(this.ImportNodeInternal(xmlNode, deep));
			}
		}

		/// <summary>Gets the <see cref="T:System.Xml.XmlNameTable" /> associated with this implementation.</summary>
		/// <returns>An <see langword="XmlNameTable" /> enabling you to get the atomized version of a string within the document.</returns>
		public XmlNameTable NameTable
		{
			get
			{
				return this.implementation.NameTable;
			}
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlAttribute" /> with the specified <see cref="P:System.Xml.XmlNode.Prefix" />, <see cref="P:System.Xml.XmlDocument.LocalName" />, and <see cref="P:System.Xml.XmlNode.NamespaceURI" />.</summary>
		/// <param name="prefix">The prefix of the attribute (if any). String.Empty and <see langword="null" /> are equivalent. </param>
		/// <param name="localName">The local name of the attribute. </param>
		/// <param name="namespaceURI">The namespace URI of the attribute (if any). String.Empty and <see langword="null" /> are equivalent. If <paramref name="prefix" /> is xmlns, then this parameter must be http://www.w3.org/2000/xmlns/; otherwise an exception is thrown. </param>
		/// <returns>The new <see langword="XmlAttribute" />.</returns>
		public virtual XmlAttribute CreateAttribute(string prefix, string localName, string namespaceURI)
		{
			return new XmlAttribute(this.AddAttrXmlName(prefix, localName, namespaceURI, null), this);
		}

		/// <summary>Creates a default attribute with the specified prefix, local name and namespace URI.</summary>
		/// <param name="prefix">The prefix of the attribute (if any). </param>
		/// <param name="localName">The local name of the attribute. </param>
		/// <param name="namespaceURI">The namespace URI of the attribute (if any). </param>
		/// <returns>The new <see cref="T:System.Xml.XmlAttribute" />.</returns>
		protected internal virtual XmlAttribute CreateDefaultAttribute(string prefix, string localName, string namespaceURI)
		{
			return new XmlUnspecifiedAttribute(prefix, localName, namespaceURI, this);
		}

		/// <summary>Creates an element with the specified <see cref="P:System.Xml.XmlNode.Prefix" />, <see cref="P:System.Xml.XmlDocument.LocalName" />, and <see cref="P:System.Xml.XmlNode.NamespaceURI" />.</summary>
		/// <param name="prefix">The prefix of the new element (if any). String.Empty and <see langword="null" /> are equivalent. </param>
		/// <param name="localName">The local name of the new element. </param>
		/// <param name="namespaceURI">The namespace URI of the new element (if any). String.Empty and <see langword="null" /> are equivalent. </param>
		/// <returns>The new <see cref="T:System.Xml.XmlElement" />.</returns>
		public virtual XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			XmlElement xmlElement = new XmlElement(this.AddXmlName(prefix, localName, namespaceURI, null), true, this);
			if (!this.IsLoading)
			{
				this.AddDefaultAttributes(xmlElement);
			}
			return xmlElement;
		}

		/// <summary>Gets or sets a value indicating whether to preserve white space in element content.</summary>
		/// <returns>
		///     <see langword="true" /> to preserve white space; otherwise <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool PreserveWhitespace
		{
			get
			{
				return this.preserveWhitespace;
			}
			set
			{
				this.preserveWhitespace = value;
			}
		}

		/// <summary>Gets a value indicating whether the current node is read-only.</summary>
		/// <returns>
		///     <see langword="true" /> if the current node is read-only; otherwise <see langword="false" />. <see langword="XmlDocument" /> nodes always return <see langword="false" />.</returns>
		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		internal XmlNamedNodeMap Entities
		{
			get
			{
				if (this.entities == null)
				{
					this.entities = new XmlNamedNodeMap(this);
				}
				return this.entities;
			}
			set
			{
				this.entities = value;
			}
		}

		internal bool IsLoading
		{
			get
			{
				return this.isLoading;
			}
			set
			{
				this.isLoading = value;
			}
		}

		internal bool ActualLoadingStatus
		{
			get
			{
				return this.actualLoadingStatus;
			}
			set
			{
				this.actualLoadingStatus = value;
			}
		}

		/// <summary>Creates a <see cref="T:System.Xml.XmlNode" /> with the specified <see cref="T:System.Xml.XmlNodeType" />, <see cref="P:System.Xml.XmlNode.Prefix" />, <see cref="P:System.Xml.XmlDocument.Name" />, and <see cref="P:System.Xml.XmlNode.NamespaceURI" />.</summary>
		/// <param name="type">The <see langword="XmlNodeType" /> of the new node. </param>
		/// <param name="prefix">The prefix of the new node. </param>
		/// <param name="name">The local name of the new node. </param>
		/// <param name="namespaceURI">The namespace URI of the new node. </param>
		/// <returns>The new <see langword="XmlNode" />.</returns>
		/// <exception cref="T:System.ArgumentException">The name was not provided and the <see langword="XmlNodeType" /> requires a name. </exception>
		public virtual XmlNode CreateNode(XmlNodeType type, string prefix, string name, string namespaceURI)
		{
			switch (type)
			{
			case XmlNodeType.Element:
				if (prefix != null)
				{
					return this.CreateElement(prefix, name, namespaceURI);
				}
				return this.CreateElement(name, namespaceURI);
			case XmlNodeType.Attribute:
				if (prefix != null)
				{
					return this.CreateAttribute(prefix, name, namespaceURI);
				}
				return this.CreateAttribute(name, namespaceURI);
			case XmlNodeType.Text:
				return this.CreateTextNode(string.Empty);
			case XmlNodeType.CDATA:
				return this.CreateCDataSection(string.Empty);
			case XmlNodeType.EntityReference:
				return this.CreateEntityReference(name);
			case XmlNodeType.ProcessingInstruction:
				return this.CreateProcessingInstruction(name, string.Empty);
			case XmlNodeType.Comment:
				return this.CreateComment(string.Empty);
			case XmlNodeType.Document:
				return new XmlDocument();
			case XmlNodeType.DocumentType:
				return this.CreateDocumentType(name, string.Empty, string.Empty, string.Empty);
			case XmlNodeType.DocumentFragment:
				return this.CreateDocumentFragment();
			case XmlNodeType.Whitespace:
				return this.CreateWhitespace(string.Empty);
			case XmlNodeType.SignificantWhitespace:
				return this.CreateSignificantWhitespace(string.Empty);
			case XmlNodeType.XmlDeclaration:
				return this.CreateXmlDeclaration("1.0", null, null);
			}
			throw new ArgumentException(Res.GetString("Cannot create node of type {0}.", new object[]
			{
				type
			}));
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlNode" /> with the specified node type, <see cref="P:System.Xml.XmlDocument.Name" />, and <see cref="P:System.Xml.XmlNode.NamespaceURI" />.</summary>
		/// <param name="nodeTypeString">String version of the <see cref="T:System.Xml.XmlNodeType" /> of the new node. This parameter must be one of the values listed in the table below. </param>
		/// <param name="name">The qualified name of the new node. If the name contains a colon, it is parsed into <see cref="P:System.Xml.XmlNode.Prefix" /> and <see cref="P:System.Xml.XmlDocument.LocalName" /> components. </param>
		/// <param name="namespaceURI">The namespace URI of the new node. </param>
		/// <returns>The new <see langword="XmlNode" />.</returns>
		/// <exception cref="T:System.ArgumentException">The name was not provided and the <see langword="XmlNodeType" /> requires a name; or <paramref name="nodeTypeString" /> is not one of the strings listed below. </exception>
		public virtual XmlNode CreateNode(string nodeTypeString, string name, string namespaceURI)
		{
			return this.CreateNode(this.ConvertToNodeType(nodeTypeString), name, namespaceURI);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlNode" /> with the specified <see cref="T:System.Xml.XmlNodeType" />, <see cref="P:System.Xml.XmlDocument.Name" />, and <see cref="P:System.Xml.XmlNode.NamespaceURI" />.</summary>
		/// <param name="type">The <see langword="XmlNodeType" /> of the new node. </param>
		/// <param name="name">The qualified name of the new node. If the name contains a colon then it is parsed into <see cref="P:System.Xml.XmlNode.Prefix" /> and <see cref="P:System.Xml.XmlDocument.LocalName" /> components. </param>
		/// <param name="namespaceURI">The namespace URI of the new node. </param>
		/// <returns>The new <see langword="XmlNode" />.</returns>
		/// <exception cref="T:System.ArgumentException">The name was not provided and the <see langword="XmlNodeType" /> requires a name. </exception>
		public virtual XmlNode CreateNode(XmlNodeType type, string name, string namespaceURI)
		{
			return this.CreateNode(type, null, name, namespaceURI);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XmlNode" /> object based on the information in the <see cref="T:System.Xml.XmlReader" />. The reader must be positioned on a node or attribute.</summary>
		/// <param name="reader">The XML source </param>
		/// <returns>The new <see langword="XmlNode" /> or <see langword="null" /> if no more nodes exist.</returns>
		/// <exception cref="T:System.NullReferenceException">The reader is positioned on a node type that does not translate to a valid DOM node (for example, EndElement or EndEntity). </exception>
		[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
		public virtual XmlNode ReadNode(XmlReader reader)
		{
			XmlNode result = null;
			try
			{
				this.IsLoading = true;
				result = new XmlLoader().ReadCurrentNode(this, reader);
			}
			finally
			{
				this.IsLoading = false;
			}
			return result;
		}

		internal XmlNodeType ConvertToNodeType(string nodeTypeString)
		{
			if (nodeTypeString == "element")
			{
				return XmlNodeType.Element;
			}
			if (nodeTypeString == "attribute")
			{
				return XmlNodeType.Attribute;
			}
			if (nodeTypeString == "text")
			{
				return XmlNodeType.Text;
			}
			if (nodeTypeString == "cdatasection")
			{
				return XmlNodeType.CDATA;
			}
			if (nodeTypeString == "entityreference")
			{
				return XmlNodeType.EntityReference;
			}
			if (nodeTypeString == "entity")
			{
				return XmlNodeType.Entity;
			}
			if (nodeTypeString == "processinginstruction")
			{
				return XmlNodeType.ProcessingInstruction;
			}
			if (nodeTypeString == "comment")
			{
				return XmlNodeType.Comment;
			}
			if (nodeTypeString == "document")
			{
				return XmlNodeType.Document;
			}
			if (nodeTypeString == "documenttype")
			{
				return XmlNodeType.DocumentType;
			}
			if (nodeTypeString == "documentfragment")
			{
				return XmlNodeType.DocumentFragment;
			}
			if (nodeTypeString == "notation")
			{
				return XmlNodeType.Notation;
			}
			if (nodeTypeString == "significantwhitespace")
			{
				return XmlNodeType.SignificantWhitespace;
			}
			if (nodeTypeString == "whitespace")
			{
				return XmlNodeType.Whitespace;
			}
			throw new ArgumentException(Res.GetString("'{0}' does not represent any 'XmlNodeType'.", new object[]
			{
				nodeTypeString
			}));
		}

		private XmlTextReader SetupReader(XmlTextReader tr)
		{
			tr.XmlValidatingReaderCompatibilityMode = true;
			tr.EntityHandling = EntityHandling.ExpandCharEntities;
			if (this.HasSetResolver)
			{
				tr.XmlResolver = this.GetResolver();
			}
			return tr;
		}

		/// <summary>Loads the XML document from the specified URL.</summary>
		/// <param name="filename">URL for the file containing the XML document to load. The URL can be either a local file or an HTTP URL (a Web address).</param>
		/// <exception cref="T:System.Xml.XmlException">There is a load or parse error in the XML. In this case, a <see cref="T:System.IO.FileNotFoundException" /> is raised. </exception>
		/// <exception cref="T:System.ArgumentException">
		///         <paramref name="filename" /> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars" />. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="filename" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters. </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurred while opening the file. </exception>
		/// <exception cref="T:System.UnauthorizedAccessException">
		///         <paramref name="filename" /> specified a file that is read-only.-or- This operation is not supported on the current platform.-or- 
		///         <paramref name="filename" /> specified a directory.-or- The caller does not have the required permission. </exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The file specified in <paramref name="filename" /> was not found. </exception>
		/// <exception cref="T:System.NotSupportedException">
		///         <paramref name="filename" /> is in an invalid format. </exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
		public virtual void Load(string filename)
		{
			XmlTextReader xmlTextReader = this.SetupReader(new XmlTextReader(filename, this.NameTable));
			try
			{
				this.Load(xmlTextReader);
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		/// <summary>Loads the XML document from the specified stream.</summary>
		/// <param name="inStream">The stream containing the XML document to load. </param>
		/// <exception cref="T:System.Xml.XmlException">There is a load or parse error in the XML. In this case, a <see cref="T:System.IO.FileNotFoundException" /> is raised. </exception>
		public virtual void Load(Stream inStream)
		{
			XmlTextReader xmlTextReader = this.SetupReader(new XmlTextReader(inStream, this.NameTable));
			try
			{
				this.Load(xmlTextReader);
			}
			finally
			{
				xmlTextReader.Impl.Close(false);
			}
		}

		/// <summary>Loads the XML document from the specified <see cref="T:System.IO.TextReader" />.</summary>
		/// <param name="txtReader">The <see langword="TextReader" /> used to feed the XML data into the document. </param>
		/// <exception cref="T:System.Xml.XmlException">There is a load or parse error in the XML. In this case, the document remains empty. </exception>
		public virtual void Load(TextReader txtReader)
		{
			XmlTextReader xmlTextReader = this.SetupReader(new XmlTextReader(txtReader, this.NameTable));
			try
			{
				this.Load(xmlTextReader);
			}
			finally
			{
				xmlTextReader.Impl.Close(false);
			}
		}

		/// <summary>Loads the XML document from the specified <see cref="T:System.Xml.XmlReader" />.</summary>
		/// <param name="reader">The <see langword="XmlReader" /> used to feed the XML data into the document. </param>
		/// <exception cref="T:System.Xml.XmlException">There is a load or parse error in the XML. In this case, the document remains empty. </exception>
		public virtual void Load(XmlReader reader)
		{
			try
			{
				this.IsLoading = true;
				this.actualLoadingStatus = true;
				this.RemoveAll();
				this.fEntRefNodesPresent = false;
				this.fCDataNodesPresent = false;
				this.reportValidity = true;
				new XmlLoader().Load(this, reader, this.preserveWhitespace);
			}
			finally
			{
				this.IsLoading = false;
				this.actualLoadingStatus = false;
				this.reportValidity = true;
			}
		}

		/// <summary>Loads the XML document from the specified string.</summary>
		/// <param name="xml">String containing the XML document to load. </param>
		/// <exception cref="T:System.Xml.XmlException">There is a load or parse error in the XML. In this case, the document remains empty. </exception>
		public virtual void LoadXml(string xml)
		{
			XmlTextReader xmlTextReader = this.SetupReader(new XmlTextReader(new StringReader(xml), this.NameTable));
			try
			{
				this.Load(xmlTextReader);
			}
			finally
			{
				xmlTextReader.Close();
			}
		}

		internal Encoding TextEncoding
		{
			get
			{
				if (this.Declaration != null)
				{
					string encoding = this.Declaration.Encoding;
					if (encoding.Length > 0)
					{
						return System.Text.Encoding.GetEncoding(encoding);
					}
				}
				return null;
			}
		}

		/// <summary>
		///     Throws an <see cref="T:System.InvalidOperationException" /> in all cases.</summary>
		/// <returns>The values of the node and all its child nodes.</returns>
		/// <exception cref="T:System.InvalidOperationException">In all cases.</exception>
		public override string InnerText
		{
			set
			{
				throw new InvalidOperationException(Res.GetString("The 'InnerText' of a 'Document' node is read-only and cannot be set."));
			}
		}

		/// <summary>Gets or sets the markup representing the children of the current node.</summary>
		/// <returns>The markup of the children of the current node.</returns>
		/// <exception cref="T:System.Xml.XmlException">The XML specified when setting this property is not well-formed. </exception>
		public override string InnerXml
		{
			get
			{
				return base.InnerXml;
			}
			set
			{
				this.LoadXml(value);
			}
		}

		/// <summary>Saves the XML document to the specified file. If the specified file exists, this method overwrites it.</summary>
		/// <param name="filename">The location of the file where you want to save the document. </param>
		/// <exception cref="T:System.Xml.XmlException">The operation would not result in a well formed XML document (for example, no document element or duplicate XML declarations). </exception>
		public virtual void Save(string filename)
		{
			if (this.DocumentElement == null)
			{
				throw new XmlException("Invalid XML document. {0}", Res.GetString("The document does not have a root element."));
			}
			XmlDOMTextWriter xmlDOMTextWriter = new XmlDOMTextWriter(filename, this.TextEncoding);
			try
			{
				if (!this.preserveWhitespace)
				{
					xmlDOMTextWriter.Formatting = Formatting.Indented;
				}
				this.WriteTo(xmlDOMTextWriter);
				xmlDOMTextWriter.Flush();
			}
			finally
			{
				xmlDOMTextWriter.Close();
			}
		}

		/// <summary>Saves the XML document to the specified stream.</summary>
		/// <param name="outStream">The stream to which you want to save. </param>
		/// <exception cref="T:System.Xml.XmlException">The operation would not result in a well formed XML document (for example, no document element or duplicate XML declarations). </exception>
		public virtual void Save(Stream outStream)
		{
			XmlDOMTextWriter xmlDOMTextWriter = new XmlDOMTextWriter(outStream, this.TextEncoding);
			if (!this.preserveWhitespace)
			{
				xmlDOMTextWriter.Formatting = Formatting.Indented;
			}
			this.WriteTo(xmlDOMTextWriter);
			xmlDOMTextWriter.Flush();
		}

		/// <summary>Saves the XML document to the specified <see cref="T:System.IO.TextWriter" />.</summary>
		/// <param name="writer">The <see langword="TextWriter" /> to which you want to save. </param>
		/// <exception cref="T:System.Xml.XmlException">The operation would not result in a well formed XML document (for example, no document element or duplicate XML declarations). </exception>
		public virtual void Save(TextWriter writer)
		{
			XmlDOMTextWriter xmlDOMTextWriter = new XmlDOMTextWriter(writer);
			if (!this.preserveWhitespace)
			{
				xmlDOMTextWriter.Formatting = Formatting.Indented;
			}
			this.Save(xmlDOMTextWriter);
		}

		/// <summary>Saves the XML document to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		/// <exception cref="T:System.Xml.XmlException">The operation would not result in a well formed XML document (for example, no document element or duplicate XML declarations). </exception>
		public virtual void Save(XmlWriter w)
		{
			XmlNode xmlNode = this.FirstChild;
			if (xmlNode == null)
			{
				return;
			}
			if (w.WriteState == WriteState.Start)
			{
				if (xmlNode is XmlDeclaration)
				{
					if (this.Standalone.Length == 0)
					{
						w.WriteStartDocument();
					}
					else if (this.Standalone == "yes")
					{
						w.WriteStartDocument(true);
					}
					else if (this.Standalone == "no")
					{
						w.WriteStartDocument(false);
					}
					xmlNode = xmlNode.NextSibling;
				}
				else
				{
					w.WriteStartDocument();
				}
			}
			while (xmlNode != null)
			{
				xmlNode.WriteTo(w);
				xmlNode = xmlNode.NextSibling;
			}
			w.Flush();
		}

		/// <summary>Saves the <see langword="XmlDocument" /> node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="w">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteTo(XmlWriter w)
		{
			this.WriteContentTo(w);
		}

		/// <summary>Saves all the children of the <see langword="XmlDocument" /> node to the specified <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="xw">The <see langword="XmlWriter" /> to which you want to save. </param>
		public override void WriteContentTo(XmlWriter xw)
		{
			foreach (object obj in this)
			{
				((XmlNode)obj).WriteTo(xw);
			}
		}

		/// <summary>Validates the <see cref="T:System.Xml.XmlDocument" /> against the XML Schema Definition Language (XSD) schemas contained in the <see cref="P:System.Xml.XmlDocument.Schemas" /> property.</summary>
		/// <param name="validationEventHandler">The <see cref="T:System.Xml.Schema.ValidationEventHandler" /> object that receives information about schema validation warnings and errors.</param>
		/// <exception cref="T:System.Xml.Schema.XmlSchemaValidationException">A schema validation event occurred and no <see cref="T:System.Xml.Schema.ValidationEventHandler" /> object was specified.</exception>
		public void Validate(ValidationEventHandler validationEventHandler)
		{
			this.Validate(validationEventHandler, this);
		}

		/// <summary>Validates the <see cref="T:System.Xml.XmlNode" /> object specified against the XML Schema Definition Language (XSD) schemas in the <see cref="P:System.Xml.XmlDocument.Schemas" /> property.</summary>
		/// <param name="validationEventHandler">The <see cref="T:System.Xml.Schema.ValidationEventHandler" /> object that receives information about schema validation warnings and errors.</param>
		/// <param name="nodeToValidate">The <see cref="T:System.Xml.XmlNode" /> object created from an <see cref="T:System.Xml.XmlDocument" /> to validate.</param>
		/// <exception cref="T:System.ArgumentException">The <see cref="T:System.Xml.XmlNode" /> object parameter was not created from an <see cref="T:System.Xml.XmlDocument" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Xml.XmlNode" /> object parameter is not an element, attribute, document fragment, or the root node.</exception>
		/// <exception cref="T:System.Xml.Schema.XmlSchemaValidationException">A schema validation event occurred and no <see cref="T:System.Xml.Schema.ValidationEventHandler" /> object was specified.</exception>
		public void Validate(ValidationEventHandler validationEventHandler, XmlNode nodeToValidate)
		{
			if (this.schemas == null || this.schemas.Count == 0)
			{
				throw new InvalidOperationException(Res.GetString("The XmlSchemaSet on the document is either null or has no schemas in it. Provide schema information before calling Validate."));
			}
			if (nodeToValidate.Document != this)
			{
				throw new ArgumentException(Res.GetString("Cannot validate '{0}' because its owner document is not the current document.", new object[]
				{
					"nodeToValidate"
				}));
			}
			if (nodeToValidate == this)
			{
				this.reportValidity = false;
			}
			new DocumentSchemaValidator(this, this.schemas, validationEventHandler).Validate(nodeToValidate);
			if (nodeToValidate == this)
			{
				this.reportValidity = true;
			}
		}

		/// <summary>Occurs when a node belonging to this document is about to be inserted into another node.</summary>
		public event XmlNodeChangedEventHandler NodeInserting
		{
			add
			{
				this.onNodeInsertingDelegate = (XmlNodeChangedEventHandler)Delegate.Combine(this.onNodeInsertingDelegate, value);
			}
			remove
			{
				this.onNodeInsertingDelegate = (XmlNodeChangedEventHandler)Delegate.Remove(this.onNodeInsertingDelegate, value);
			}
		}

		/// <summary>Occurs when a node belonging to this document has been inserted into another node.</summary>
		public event XmlNodeChangedEventHandler NodeInserted
		{
			add
			{
				this.onNodeInsertedDelegate = (XmlNodeChangedEventHandler)Delegate.Combine(this.onNodeInsertedDelegate, value);
			}
			remove
			{
				this.onNodeInsertedDelegate = (XmlNodeChangedEventHandler)Delegate.Remove(this.onNodeInsertedDelegate, value);
			}
		}

		/// <summary>Occurs when a node belonging to this document is about to be removed from the document.</summary>
		public event XmlNodeChangedEventHandler NodeRemoving
		{
			add
			{
				this.onNodeRemovingDelegate = (XmlNodeChangedEventHandler)Delegate.Combine(this.onNodeRemovingDelegate, value);
			}
			remove
			{
				this.onNodeRemovingDelegate = (XmlNodeChangedEventHandler)Delegate.Remove(this.onNodeRemovingDelegate, value);
			}
		}

		/// <summary>Occurs when a node belonging to this document has been removed from its parent.</summary>
		public event XmlNodeChangedEventHandler NodeRemoved
		{
			add
			{
				this.onNodeRemovedDelegate = (XmlNodeChangedEventHandler)Delegate.Combine(this.onNodeRemovedDelegate, value);
			}
			remove
			{
				this.onNodeRemovedDelegate = (XmlNodeChangedEventHandler)Delegate.Remove(this.onNodeRemovedDelegate, value);
			}
		}

		/// <summary>Occurs when the <see cref="P:System.Xml.XmlNode.Value" /> of a node belonging to this document is about to be changed.</summary>
		public event XmlNodeChangedEventHandler NodeChanging
		{
			add
			{
				this.onNodeChangingDelegate = (XmlNodeChangedEventHandler)Delegate.Combine(this.onNodeChangingDelegate, value);
			}
			remove
			{
				this.onNodeChangingDelegate = (XmlNodeChangedEventHandler)Delegate.Remove(this.onNodeChangingDelegate, value);
			}
		}

		/// <summary>Occurs when the <see cref="P:System.Xml.XmlNode.Value" /> of a node belonging to this document has been changed.</summary>
		public event XmlNodeChangedEventHandler NodeChanged
		{
			add
			{
				this.onNodeChangedDelegate = (XmlNodeChangedEventHandler)Delegate.Combine(this.onNodeChangedDelegate, value);
			}
			remove
			{
				this.onNodeChangedDelegate = (XmlNodeChangedEventHandler)Delegate.Remove(this.onNodeChangedDelegate, value);
			}
		}

		internal override XmlNodeChangedEventArgs GetEventArgs(XmlNode node, XmlNode oldParent, XmlNode newParent, string oldValue, string newValue, XmlNodeChangedAction action)
		{
			this.reportValidity = false;
			switch (action)
			{
			case XmlNodeChangedAction.Insert:
				if (this.onNodeInsertingDelegate == null && this.onNodeInsertedDelegate == null)
				{
					return null;
				}
				break;
			case XmlNodeChangedAction.Remove:
				if (this.onNodeRemovingDelegate == null && this.onNodeRemovedDelegate == null)
				{
					return null;
				}
				break;
			case XmlNodeChangedAction.Change:
				if (this.onNodeChangingDelegate == null && this.onNodeChangedDelegate == null)
				{
					return null;
				}
				break;
			}
			return new XmlNodeChangedEventArgs(node, oldParent, newParent, oldValue, newValue, action);
		}

		internal XmlNodeChangedEventArgs GetInsertEventArgsForLoad(XmlNode node, XmlNode newParent)
		{
			if (this.onNodeInsertingDelegate == null && this.onNodeInsertedDelegate == null)
			{
				return null;
			}
			string value = node.Value;
			return new XmlNodeChangedEventArgs(node, null, newParent, value, value, XmlNodeChangedAction.Insert);
		}

		internal override void BeforeEvent(XmlNodeChangedEventArgs args)
		{
			if (args != null)
			{
				switch (args.Action)
				{
				case XmlNodeChangedAction.Insert:
					if (this.onNodeInsertingDelegate != null)
					{
						this.onNodeInsertingDelegate(this, args);
						return;
					}
					break;
				case XmlNodeChangedAction.Remove:
					if (this.onNodeRemovingDelegate != null)
					{
						this.onNodeRemovingDelegate(this, args);
						return;
					}
					break;
				case XmlNodeChangedAction.Change:
					if (this.onNodeChangingDelegate != null)
					{
						this.onNodeChangingDelegate(this, args);
					}
					break;
				default:
					return;
				}
			}
		}

		internal override void AfterEvent(XmlNodeChangedEventArgs args)
		{
			if (args != null)
			{
				switch (args.Action)
				{
				case XmlNodeChangedAction.Insert:
					if (this.onNodeInsertedDelegate != null)
					{
						this.onNodeInsertedDelegate(this, args);
						return;
					}
					break;
				case XmlNodeChangedAction.Remove:
					if (this.onNodeRemovedDelegate != null)
					{
						this.onNodeRemovedDelegate(this, args);
						return;
					}
					break;
				case XmlNodeChangedAction.Change:
					if (this.onNodeChangedDelegate != null)
					{
						this.onNodeChangedDelegate(this, args);
					}
					break;
				default:
					return;
				}
			}
		}

		internal XmlAttribute GetDefaultAttribute(XmlElement elem, string attrPrefix, string attrLocalname, string attrNamespaceURI)
		{
			SchemaInfo dtdSchemaInfo = this.DtdSchemaInfo;
			SchemaElementDecl schemaElementDecl = this.GetSchemaElementDecl(elem);
			if (schemaElementDecl != null && schemaElementDecl.AttDefs != null)
			{
				IDictionaryEnumerator dictionaryEnumerator = schemaElementDecl.AttDefs.GetEnumerator();
				while (dictionaryEnumerator.MoveNext())
				{
					SchemaAttDef schemaAttDef = (SchemaAttDef)dictionaryEnumerator.Value;
					if ((schemaAttDef.Presence == SchemaDeclBase.Use.Default || schemaAttDef.Presence == SchemaDeclBase.Use.Fixed) && schemaAttDef.Name.Name == attrLocalname && ((dtdSchemaInfo.SchemaType == SchemaType.DTD && schemaAttDef.Name.Namespace == attrPrefix) || (dtdSchemaInfo.SchemaType != SchemaType.DTD && schemaAttDef.Name.Namespace == attrNamespaceURI)))
					{
						return this.PrepareDefaultAttribute(schemaAttDef, attrPrefix, attrLocalname, attrNamespaceURI);
					}
				}
			}
			return null;
		}

		internal string Version
		{
			get
			{
				XmlDeclaration declaration = this.Declaration;
				if (declaration != null)
				{
					return declaration.Version;
				}
				return null;
			}
		}

		internal string Encoding
		{
			get
			{
				XmlDeclaration declaration = this.Declaration;
				if (declaration != null)
				{
					return declaration.Encoding;
				}
				return null;
			}
		}

		internal string Standalone
		{
			get
			{
				XmlDeclaration declaration = this.Declaration;
				if (declaration != null)
				{
					return declaration.Standalone;
				}
				return null;
			}
		}

		internal XmlEntity GetEntityNode(string name)
		{
			if (this.DocumentType != null)
			{
				XmlNamedNodeMap xmlNamedNodeMap = this.DocumentType.Entities;
				if (xmlNamedNodeMap != null)
				{
					return (XmlEntity)xmlNamedNodeMap.GetNamedItem(name);
				}
			}
			return null;
		}

		/// <summary>Returns the Post-Schema-Validation-Infoset (PSVI) of the node.</summary>
		/// <returns>The <see cref="T:System.Xml.Schema.IXmlSchemaInfo" /> object representing the PSVI of the node.</returns>
		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				if (this.reportValidity)
				{
					XmlElement documentElement = this.DocumentElement;
					if (documentElement != null)
					{
						XmlSchemaValidity validity = documentElement.SchemaInfo.Validity;
						if (validity == XmlSchemaValidity.Valid)
						{
							return XmlDocument.ValidSchemaInfo;
						}
						if (validity == XmlSchemaValidity.Invalid)
						{
							return XmlDocument.InvalidSchemaInfo;
						}
					}
				}
				return XmlDocument.NotKnownSchemaInfo;
			}
		}

		/// <summary>Gets the base URI of the current node.</summary>
		/// <returns>The location from which the node was loaded.</returns>
		public override string BaseURI
		{
			get
			{
				return this.baseURI;
			}
		}

		internal void SetBaseURI(string inBaseURI)
		{
			this.baseURI = inBaseURI;
		}

		internal override XmlNode AppendChildForLoad(XmlNode newChild, XmlDocument doc)
		{
			if (!this.IsValidChildType(newChild.NodeType))
			{
				throw new InvalidOperationException(Res.GetString("The specified node cannot be inserted as the valid child of this node, because the specified node is the wrong type."));
			}
			if (!this.CanInsertAfter(newChild, this.LastChild))
			{
				throw new InvalidOperationException(Res.GetString("Cannot insert the node in the specified location."));
			}
			XmlNodeChangedEventArgs insertEventArgsForLoad = this.GetInsertEventArgsForLoad(newChild, this);
			if (insertEventArgsForLoad != null)
			{
				this.BeforeEvent(insertEventArgsForLoad);
			}
			XmlLinkedNode xmlLinkedNode = (XmlLinkedNode)newChild;
			if (this.lastChild == null)
			{
				xmlLinkedNode.next = xmlLinkedNode;
			}
			else
			{
				xmlLinkedNode.next = this.lastChild.next;
				this.lastChild.next = xmlLinkedNode;
			}
			this.lastChild = xmlLinkedNode;
			xmlLinkedNode.SetParentForLoad(this);
			if (insertEventArgsForLoad != null)
			{
				this.AfterEvent(insertEventArgsForLoad);
			}
			return xmlLinkedNode;
		}

		internal override XPathNodeType XPNodeType
		{
			get
			{
				return XPathNodeType.Root;
			}
		}

		internal bool HasEntityReferences
		{
			get
			{
				return this.fEntRefNodesPresent;
			}
		}

		internal XmlAttribute NamespaceXml
		{
			get
			{
				if (this.namespaceXml == null)
				{
					this.namespaceXml = new XmlAttribute(this.AddAttrXmlName(this.strXmlns, this.strXml, this.strReservedXmlns, null), this);
					this.namespaceXml.Value = this.strReservedXml;
				}
				return this.namespaceXml;
			}
		}

		private XmlImplementation implementation;

		private DomNameTable domNameTable;

		private XmlLinkedNode lastChild;

		private XmlNamedNodeMap entities;

		private Hashtable htElementIdMap;

		private Hashtable htElementIDAttrDecl;

		private SchemaInfo schemaInfo;

		private XmlSchemaSet schemas;

		private bool reportValidity;

		private bool actualLoadingStatus;

		private XmlNodeChangedEventHandler onNodeInsertingDelegate;

		private XmlNodeChangedEventHandler onNodeInsertedDelegate;

		private XmlNodeChangedEventHandler onNodeRemovingDelegate;

		private XmlNodeChangedEventHandler onNodeRemovedDelegate;

		private XmlNodeChangedEventHandler onNodeChangingDelegate;

		private XmlNodeChangedEventHandler onNodeChangedDelegate;

		internal bool fEntRefNodesPresent;

		internal bool fCDataNodesPresent;

		private bool preserveWhitespace;

		private bool isLoading;

		internal string strDocumentName;

		internal string strDocumentFragmentName;

		internal string strCommentName;

		internal string strTextName;

		internal string strCDataSectionName;

		internal string strEntityName;

		internal string strID;

		internal string strXmlns;

		internal string strXml;

		internal string strSpace;

		internal string strLang;

		internal string strEmpty;

		internal string strNonSignificantWhitespaceName;

		internal string strSignificantWhitespaceName;

		internal string strReservedXmlns;

		internal string strReservedXml;

		internal string baseURI;

		private XmlResolver resolver;

		internal bool bSetResolver;

		internal object objLock;

		private XmlAttribute namespaceXml;

		internal static EmptyEnumerator EmptyEnumerator = new EmptyEnumerator();

		internal static IXmlSchemaInfo NotKnownSchemaInfo = new XmlSchemaInfo(XmlSchemaValidity.NotKnown);

		internal static IXmlSchemaInfo ValidSchemaInfo = new XmlSchemaInfo(XmlSchemaValidity.Valid);

		internal static IXmlSchemaInfo InvalidSchemaInfo = new XmlSchemaInfo(XmlSchemaValidity.Invalid);
	}
}
