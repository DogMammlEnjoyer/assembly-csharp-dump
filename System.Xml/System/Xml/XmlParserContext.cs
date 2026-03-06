using System;
using System.Text;

namespace System.Xml
{
	/// <summary>Provides all the context information required by the <see cref="T:System.Xml.XmlReader" /> to parse an XML fragment.</summary>
	public class XmlParserContext
	{
		/// <summary>Initializes a new instance of the <see langword="XmlParserContext" /> class with the specified <see cref="T:System.Xml.XmlNameTable" />, <see cref="T:System.Xml.XmlNamespaceManager" />, <see langword="xml:lang" />, and <see langword="xml:space" /> values.</summary>
		/// <param name="nt">The <see cref="T:System.Xml.XmlNameTable" /> to use to atomize strings. If this is <see langword="null" />, the name table used to construct the <paramref name="nsMgr" /> is used instead. For more information about atomized strings, see <see cref="T:System.Xml.XmlNameTable" />. </param>
		/// <param name="nsMgr">The <see cref="T:System.Xml.XmlNamespaceManager" /> to use for looking up namespace information, or <see langword="null" />. </param>
		/// <param name="xmlLang">The <see langword="xml:lang" /> scope. </param>
		/// <param name="xmlSpace">An <see cref="T:System.Xml.XmlSpace" /> value indicating the <see langword="xml:space" /> scope. </param>
		/// <exception cref="T:System.Xml.XmlException">
		///         <paramref name="nt" /> is not the same <see langword="XmlNameTable" /> used to construct <paramref name="nsMgr" />. </exception>
		public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, string xmlLang, XmlSpace xmlSpace) : this(nt, nsMgr, null, null, null, null, string.Empty, xmlLang, xmlSpace)
		{
		}

		/// <summary>Initializes a new instance of the <see langword="XmlParserContext" /> class with the specified <see cref="T:System.Xml.XmlNameTable" />, <see cref="T:System.Xml.XmlNamespaceManager" />, <see langword="xml:lang" />, <see langword="xml:space" />, and encoding.</summary>
		/// <param name="nt">The <see cref="T:System.Xml.XmlNameTable" /> to use to atomize strings. If this is <see langword="null" />, the name table used to construct the <paramref name="nsMgr" /> is used instead. For more information on atomized strings, see <see cref="T:System.Xml.XmlNameTable" />. </param>
		/// <param name="nsMgr">The <see cref="T:System.Xml.XmlNamespaceManager" /> to use for looking up namespace information, or <see langword="null" />. </param>
		/// <param name="xmlLang">The <see langword="xml:lang" /> scope. </param>
		/// <param name="xmlSpace">An <see cref="T:System.Xml.XmlSpace" /> value indicating the <see langword="xml:space" /> scope. </param>
		/// <param name="enc">An <see cref="T:System.Text.Encoding" /> object indicating the encoding setting. </param>
		/// <exception cref="T:System.Xml.XmlException">
		///         <paramref name="nt" /> is not the same <see langword="XmlNameTable" /> used to construct <paramref name="nsMgr" />. </exception>
		public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, string xmlLang, XmlSpace xmlSpace, Encoding enc) : this(nt, nsMgr, null, null, null, null, string.Empty, xmlLang, xmlSpace, enc)
		{
		}

		/// <summary>Initializes a new instance of the <see langword="XmlParserContext" /> class with the specified <see cref="T:System.Xml.XmlNameTable" />, <see cref="T:System.Xml.XmlNamespaceManager" />, base URI, <see langword="xml:lang" />, <see langword="xml:space" />, and document type values.</summary>
		/// <param name="nt">The <see cref="T:System.Xml.XmlNameTable" /> to use to atomize strings. If this is <see langword="null" />, the name table used to construct the <paramref name="nsMgr" /> is used instead. For more information about atomized strings, see <see cref="T:System.Xml.XmlNameTable" />. </param>
		/// <param name="nsMgr">The <see cref="T:System.Xml.XmlNamespaceManager" /> to use for looking up namespace information, or <see langword="null" />. </param>
		/// <param name="docTypeName">The name of the document type declaration. </param>
		/// <param name="pubId">The public identifier. </param>
		/// <param name="sysId">The system identifier. </param>
		/// <param name="internalSubset">The internal DTD subset. The DTD subset is used for entity resolution, not for document validation.</param>
		/// <param name="baseURI">The base URI for the XML fragment (the location from which the fragment was loaded). </param>
		/// <param name="xmlLang">The <see langword="xml:lang" /> scope. </param>
		/// <param name="xmlSpace">An <see cref="T:System.Xml.XmlSpace" /> value indicating the <see langword="xml:space" /> scope. </param>
		/// <exception cref="T:System.Xml.XmlException">
		///         <paramref name="nt" /> is not the same <see langword="XmlNameTable" /> used to construct <paramref name="nsMgr" />. </exception>
		public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, string docTypeName, string pubId, string sysId, string internalSubset, string baseURI, string xmlLang, XmlSpace xmlSpace) : this(nt, nsMgr, docTypeName, pubId, sysId, internalSubset, baseURI, xmlLang, xmlSpace, null)
		{
		}

		/// <summary>Initializes a new instance of the <see langword="XmlParserContext" /> class with the specified <see cref="T:System.Xml.XmlNameTable" />, <see cref="T:System.Xml.XmlNamespaceManager" />, base URI, <see langword="xml:lang" />, <see langword="xml:space" />, encoding, and document type values.</summary>
		/// <param name="nt">The <see cref="T:System.Xml.XmlNameTable" /> to use to atomize strings. If this is <see langword="null" />, the name table used to construct the <paramref name="nsMgr" /> is used instead. For more information about atomized strings, see <see cref="T:System.Xml.XmlNameTable" />. </param>
		/// <param name="nsMgr">The <see cref="T:System.Xml.XmlNamespaceManager" /> to use for looking up namespace information, or <see langword="null" />. </param>
		/// <param name="docTypeName">The name of the document type declaration. </param>
		/// <param name="pubId">The public identifier. </param>
		/// <param name="sysId">The system identifier. </param>
		/// <param name="internalSubset">The internal DTD subset. The DTD is used for entity resolution, not for document validation.</param>
		/// <param name="baseURI">The base URI for the XML fragment (the location from which the fragment was loaded). </param>
		/// <param name="xmlLang">The <see langword="xml:lang" /> scope. </param>
		/// <param name="xmlSpace">An <see cref="T:System.Xml.XmlSpace" /> value indicating the <see langword="xml:space" /> scope. </param>
		/// <param name="enc">An <see cref="T:System.Text.Encoding" /> object indicating the encoding setting. </param>
		/// <exception cref="T:System.Xml.XmlException">
		///         <paramref name="nt" /> is not the same <see langword="XmlNameTable" /> used to construct <paramref name="nsMgr" />. </exception>
		public XmlParserContext(XmlNameTable nt, XmlNamespaceManager nsMgr, string docTypeName, string pubId, string sysId, string internalSubset, string baseURI, string xmlLang, XmlSpace xmlSpace, Encoding enc)
		{
			if (nsMgr != null)
			{
				if (nt == null)
				{
					this._nt = nsMgr.NameTable;
				}
				else
				{
					if (nt != nsMgr.NameTable)
					{
						throw new XmlException("Not the same name table.", string.Empty);
					}
					this._nt = nt;
				}
			}
			else
			{
				this._nt = nt;
			}
			this._nsMgr = nsMgr;
			this._docTypeName = ((docTypeName == null) ? string.Empty : docTypeName);
			this._pubId = ((pubId == null) ? string.Empty : pubId);
			this._sysId = ((sysId == null) ? string.Empty : sysId);
			this._internalSubset = ((internalSubset == null) ? string.Empty : internalSubset);
			this._baseURI = ((baseURI == null) ? string.Empty : baseURI);
			this._xmlLang = ((xmlLang == null) ? string.Empty : xmlLang);
			this._xmlSpace = xmlSpace;
			this._encoding = enc;
		}

		/// <summary>Gets the <see cref="T:System.Xml.XmlNameTable" /> used to atomize strings. For more information on atomized strings, see <see cref="T:System.Xml.XmlNameTable" />.</summary>
		/// <returns>The <see langword="XmlNameTable" />.</returns>
		public XmlNameTable NameTable
		{
			get
			{
				return this._nt;
			}
			set
			{
				this._nt = value;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Xml.XmlNamespaceManager" />.</summary>
		/// <returns>The <see langword="XmlNamespaceManager" />.</returns>
		public XmlNamespaceManager NamespaceManager
		{
			get
			{
				return this._nsMgr;
			}
			set
			{
				this._nsMgr = value;
			}
		}

		/// <summary>Gets or sets the name of the document type declaration.</summary>
		/// <returns>The name of the document type declaration.</returns>
		public string DocTypeName
		{
			get
			{
				return this._docTypeName;
			}
			set
			{
				this._docTypeName = ((value == null) ? string.Empty : value);
			}
		}

		/// <summary>Gets or sets the public identifier.</summary>
		/// <returns>The public identifier.</returns>
		public string PublicId
		{
			get
			{
				return this._pubId;
			}
			set
			{
				this._pubId = ((value == null) ? string.Empty : value);
			}
		}

		/// <summary>Gets or sets the system identifier.</summary>
		/// <returns>The system identifier.</returns>
		public string SystemId
		{
			get
			{
				return this._sysId;
			}
			set
			{
				this._sysId = ((value == null) ? string.Empty : value);
			}
		}

		/// <summary>Gets or sets the base URI.</summary>
		/// <returns>The base URI to use to resolve the DTD file.</returns>
		public string BaseURI
		{
			get
			{
				return this._baseURI;
			}
			set
			{
				this._baseURI = ((value == null) ? string.Empty : value);
			}
		}

		/// <summary>Gets or sets the internal DTD subset.</summary>
		/// <returns>The internal DTD subset. For example, this property returns everything between the square brackets &lt;!DOCTYPE doc [...]&gt;.</returns>
		public string InternalSubset
		{
			get
			{
				return this._internalSubset;
			}
			set
			{
				this._internalSubset = ((value == null) ? string.Empty : value);
			}
		}

		/// <summary>Gets or sets the current <see langword="xml:lang" /> scope.</summary>
		/// <returns>The current <see langword="xml:lang" /> scope. If there is no <see langword="xml:lang" /> in scope, <see langword="String.Empty" /> is returned.</returns>
		public string XmlLang
		{
			get
			{
				return this._xmlLang;
			}
			set
			{
				this._xmlLang = ((value == null) ? string.Empty : value);
			}
		}

		/// <summary>Gets or sets the current <see langword="xml:space" /> scope.</summary>
		/// <returns>An <see cref="T:System.Xml.XmlSpace" /> value indicating the <see langword="xml:space" /> scope.</returns>
		public XmlSpace XmlSpace
		{
			get
			{
				return this._xmlSpace;
			}
			set
			{
				this._xmlSpace = value;
			}
		}

		/// <summary>Gets or sets the encoding type.</summary>
		/// <returns>An <see cref="T:System.Text.Encoding" /> object indicating the encoding type.</returns>
		public Encoding Encoding
		{
			get
			{
				return this._encoding;
			}
			set
			{
				this._encoding = value;
			}
		}

		internal bool HasDtdInfo
		{
			get
			{
				return this._internalSubset != string.Empty || this._pubId != string.Empty || this._sysId != string.Empty;
			}
		}

		private XmlNameTable _nt;

		private XmlNamespaceManager _nsMgr;

		private string _docTypeName = string.Empty;

		private string _pubId = string.Empty;

		private string _sysId = string.Empty;

		private string _internalSubset = string.Empty;

		private string _xmlLang = string.Empty;

		private XmlSpace _xmlSpace;

		private string _baseURI = string.Empty;

		private Encoding _encoding;
	}
}
