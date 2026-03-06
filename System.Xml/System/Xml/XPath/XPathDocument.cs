using System;
using System.Collections.Generic;
using System.IO;
using MS.Internal.Xml.Cache;

namespace System.Xml.XPath
{
	/// <summary>Provides a fast, read-only, in-memory representation of an XML document by using the XPath data model.</summary>
	public class XPathDocument : IXPathNavigable
	{
		internal XPathDocument()
		{
			this.nameTable = new NameTable();
		}

		internal XPathDocument(XmlNameTable nameTable)
		{
			if (nameTable == null)
			{
				throw new ArgumentNullException("nameTable");
			}
			this.nameTable = nameTable;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XPath.XPathDocument" /> class from the XML data that is contained in the specified <see cref="T:System.Xml.XmlReader" /> object.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> object that contains the XML data. </param>
		/// <exception cref="T:System.Xml.XmlException">An error was encountered in the XML data. The <see cref="T:System.Xml.XPath.XPathDocument" /> remains empty. </exception>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="T:System.Xml.XmlReader" /> object passed as a parameter is <see langword="null" />.</exception>
		public XPathDocument(XmlReader reader) : this(reader, XmlSpace.Default)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XPath.XPathDocument" /> class from the XML data that is contained in the specified <see cref="T:System.Xml.XmlReader" /> object with the specified white space handling.</summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> object that contains the XML data.</param>
		/// <param name="space">An <see cref="T:System.Xml.XmlSpace" /> object.</param>
		/// <exception cref="T:System.Xml.XmlException">An error was encountered in the XML data. The <see cref="T:System.Xml.XPath.XPathDocument" /> remains empty. </exception>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="T:System.Xml.XmlReader" /> object parameter or <see cref="T:System.Xml.XmlSpace" /> object parameter is <see langword="null" />.</exception>
		public XPathDocument(XmlReader reader, XmlSpace space)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.LoadFromReader(reader, space);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XPath.XPathDocument" /> class from the XML data that is contained in the specified <see cref="T:System.IO.TextReader" /> object.</summary>
		/// <param name="textReader">The <see cref="T:System.IO.TextReader" /> object that contains the XML data.</param>
		/// <exception cref="T:System.Xml.XmlException">An error was encountered in the XML data. The <see cref="T:System.Xml.XPath.XPathDocument" /> remains empty. </exception>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="T:System.IO.TextReader" /> object passed as a parameter is <see langword="null" />.</exception>
		public XPathDocument(TextReader textReader)
		{
			XmlTextReaderImpl xmlTextReaderImpl = this.SetupReader(new XmlTextReaderImpl(string.Empty, textReader));
			try
			{
				this.LoadFromReader(xmlTextReaderImpl, XmlSpace.Default);
			}
			finally
			{
				xmlTextReaderImpl.Close();
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XPath.XPathDocument" /> class from the XML data in the specified <see cref="T:System.IO.Stream" /> object.</summary>
		/// <param name="stream">The <see cref="T:System.IO.Stream" /> object that contains the XML data.</param>
		/// <exception cref="T:System.Xml.XmlException">An error was encountered in the XML data. The <see cref="T:System.Xml.XPath.XPathDocument" /> remains empty. </exception>
		/// <exception cref="T:System.ArgumentNullException">The <see cref="T:System.IO.Stream" /> object passed as a parameter is <see langword="null" />.</exception>
		public XPathDocument(Stream stream)
		{
			XmlTextReaderImpl xmlTextReaderImpl = this.SetupReader(new XmlTextReaderImpl(string.Empty, stream));
			try
			{
				this.LoadFromReader(xmlTextReaderImpl, XmlSpace.Default);
			}
			finally
			{
				xmlTextReaderImpl.Close();
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XPath.XPathDocument" /> class from the XML data in the specified file.</summary>
		/// <param name="uri">The path of the file that contains the XML data.</param>
		/// <exception cref="T:System.Xml.XmlException">An error was encountered in the XML data. The <see cref="T:System.Xml.XPath.XPathDocument" /> remains empty. </exception>
		/// <exception cref="T:System.ArgumentNullException">The file path parameter is <see langword="null" />.</exception>
		public XPathDocument(string uri) : this(uri, XmlSpace.Default)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XPath.XPathDocument" /> class from the XML data in the file specified with the white space handling specified.</summary>
		/// <param name="uri">The path of the file that contains the XML data.</param>
		/// <param name="space">An <see cref="T:System.Xml.XmlSpace" /> object.</param>
		/// <exception cref="T:System.Xml.XmlException">An error was encountered in the XML data. The <see cref="T:System.Xml.XPath.XPathDocument" /> remains empty. </exception>
		/// <exception cref="T:System.ArgumentNullException">The file path parameter or <see cref="T:System.Xml.XmlSpace" /> object parameter is <see langword="null" />.</exception>
		public XPathDocument(string uri, XmlSpace space)
		{
			XmlTextReaderImpl xmlTextReaderImpl = this.SetupReader(new XmlTextReaderImpl(uri));
			try
			{
				this.LoadFromReader(xmlTextReaderImpl, space);
			}
			finally
			{
				xmlTextReaderImpl.Close();
			}
		}

		internal XmlRawWriter LoadFromWriter(XPathDocument.LoadFlags flags, string baseUri)
		{
			return new XPathDocumentBuilder(this, null, baseUri, flags);
		}

		internal void LoadFromReader(XmlReader reader, XmlSpace space)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
			if (xmlLineInfo == null || !xmlLineInfo.HasLineInfo())
			{
				xmlLineInfo = null;
			}
			this.hasLineInfo = (xmlLineInfo != null);
			this.nameTable = reader.NameTable;
			XPathDocumentBuilder xpathDocumentBuilder = new XPathDocumentBuilder(this, xmlLineInfo, reader.BaseURI, XPathDocument.LoadFlags.None);
			try
			{
				bool flag = reader.ReadState == ReadState.Initial;
				int depth = reader.Depth;
				string text = this.nameTable.Get("http://www.w3.org/2000/xmlns/");
				if (!flag || reader.Read())
				{
					while (flag || reader.Depth >= depth)
					{
						switch (reader.NodeType)
						{
						case XmlNodeType.Element:
						{
							bool isEmptyElement = reader.IsEmptyElement;
							xpathDocumentBuilder.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI, reader.BaseURI);
							while (reader.MoveToNextAttribute())
							{
								string namespaceURI = reader.NamespaceURI;
								if (namespaceURI == text)
								{
									if (reader.Prefix.Length == 0)
									{
										xpathDocumentBuilder.WriteNamespaceDeclaration(string.Empty, reader.Value);
									}
									else
									{
										xpathDocumentBuilder.WriteNamespaceDeclaration(reader.LocalName, reader.Value);
									}
								}
								else
								{
									xpathDocumentBuilder.WriteStartAttribute(reader.Prefix, reader.LocalName, namespaceURI);
									xpathDocumentBuilder.WriteString(reader.Value, TextBlockType.Text);
									xpathDocumentBuilder.WriteEndAttribute();
								}
							}
							if (isEmptyElement)
							{
								xpathDocumentBuilder.WriteEndElement(true);
							}
							break;
						}
						case XmlNodeType.Text:
						case XmlNodeType.CDATA:
							xpathDocumentBuilder.WriteString(reader.Value, TextBlockType.Text);
							break;
						case XmlNodeType.EntityReference:
							reader.ResolveEntity();
							break;
						case XmlNodeType.ProcessingInstruction:
							xpathDocumentBuilder.WriteProcessingInstruction(reader.LocalName, reader.Value, reader.BaseURI);
							break;
						case XmlNodeType.Comment:
							xpathDocumentBuilder.WriteComment(reader.Value);
							break;
						case XmlNodeType.DocumentType:
						{
							IDtdInfo dtdInfo = reader.DtdInfo;
							if (dtdInfo != null)
							{
								xpathDocumentBuilder.CreateIdTables(dtdInfo);
							}
							break;
						}
						case XmlNodeType.Whitespace:
							goto IL_1C6;
						case XmlNodeType.SignificantWhitespace:
							if (reader.XmlSpace != XmlSpace.Preserve)
							{
								goto IL_1C6;
							}
							xpathDocumentBuilder.WriteString(reader.Value, TextBlockType.SignificantWhitespace);
							break;
						case XmlNodeType.EndElement:
							xpathDocumentBuilder.WriteEndElement(false);
							break;
						}
						IL_228:
						if (!reader.Read())
						{
							break;
						}
						continue;
						IL_1C6:
						if (space == XmlSpace.Preserve && (!flag || reader.Depth != 0))
						{
							xpathDocumentBuilder.WriteString(reader.Value, TextBlockType.Whitespace);
							goto IL_228;
						}
						goto IL_228;
					}
				}
			}
			finally
			{
				xpathDocumentBuilder.Close();
			}
		}

		/// <summary>Initializes a read-only <see cref="T:System.Xml.XPath.XPathNavigator" /> object for navigating through nodes in this <see cref="T:System.Xml.XPath.XPathDocument" />.</summary>
		/// <returns>A read-only <see cref="T:System.Xml.XPath.XPathNavigator" /> object.</returns>
		public XPathNavigator CreateNavigator()
		{
			return new XPathDocumentNavigator(this.pageRoot, this.idxRoot, null, 0);
		}

		internal XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		internal bool HasLineInfo
		{
			get
			{
				return this.hasLineInfo;
			}
		}

		internal int GetCollapsedTextNode(out XPathNode[] pageText)
		{
			pageText = this.pageText;
			return this.idxText;
		}

		internal void SetCollapsedTextNode(XPathNode[] pageText, int idxText)
		{
			this.pageText = pageText;
			this.idxText = idxText;
		}

		internal int GetRootNode(out XPathNode[] pageRoot)
		{
			pageRoot = this.pageRoot;
			return this.idxRoot;
		}

		internal void SetRootNode(XPathNode[] pageRoot, int idxRoot)
		{
			this.pageRoot = pageRoot;
			this.idxRoot = idxRoot;
		}

		internal int GetXmlNamespaceNode(out XPathNode[] pageXmlNmsp)
		{
			pageXmlNmsp = this.pageXmlNmsp;
			return this.idxXmlNmsp;
		}

		internal void SetXmlNamespaceNode(XPathNode[] pageXmlNmsp, int idxXmlNmsp)
		{
			this.pageXmlNmsp = pageXmlNmsp;
			this.idxXmlNmsp = idxXmlNmsp;
		}

		internal void AddNamespace(XPathNode[] pageElem, int idxElem, XPathNode[] pageNmsp, int idxNmsp)
		{
			if (this.mapNmsp == null)
			{
				this.mapNmsp = new Dictionary<XPathNodeRef, XPathNodeRef>();
			}
			this.mapNmsp.Add(new XPathNodeRef(pageElem, idxElem), new XPathNodeRef(pageNmsp, idxNmsp));
		}

		internal int LookupNamespaces(XPathNode[] pageElem, int idxElem, out XPathNode[] pageNmsp)
		{
			XPathNodeRef key = new XPathNodeRef(pageElem, idxElem);
			if (this.mapNmsp == null || !this.mapNmsp.ContainsKey(key))
			{
				pageNmsp = null;
				return 0;
			}
			key = this.mapNmsp[key];
			pageNmsp = key.Page;
			return key.Index;
		}

		internal void AddIdElement(string id, XPathNode[] pageElem, int idxElem)
		{
			if (this.idValueMap == null)
			{
				this.idValueMap = new Dictionary<string, XPathNodeRef>();
			}
			if (!this.idValueMap.ContainsKey(id))
			{
				this.idValueMap.Add(id, new XPathNodeRef(pageElem, idxElem));
			}
		}

		internal int LookupIdElement(string id, out XPathNode[] pageElem)
		{
			if (this.idValueMap == null || !this.idValueMap.ContainsKey(id))
			{
				pageElem = null;
				return 0;
			}
			XPathNodeRef xpathNodeRef = this.idValueMap[id];
			pageElem = xpathNodeRef.Page;
			return xpathNodeRef.Index;
		}

		private XmlTextReaderImpl SetupReader(XmlTextReaderImpl reader)
		{
			reader.EntityHandling = EntityHandling.ExpandEntities;
			reader.XmlValidatingReaderCompatibilityMode = true;
			return reader;
		}

		private XPathNode[] pageText;

		private XPathNode[] pageRoot;

		private XPathNode[] pageXmlNmsp;

		private int idxText;

		private int idxRoot;

		private int idxXmlNmsp;

		private XmlNameTable nameTable;

		private bool hasLineInfo;

		private Dictionary<XPathNodeRef, XPathNodeRef> mapNmsp;

		private Dictionary<string, XPathNodeRef> idValueMap;

		internal enum LoadFlags
		{
			None,
			AtomizeNames,
			Fragment
		}
	}
}
