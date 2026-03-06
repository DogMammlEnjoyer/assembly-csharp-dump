using System;
using System.ComponentModel;
using System.IO;
using System.Security.Permissions;
using System.Xml.Schema;
using System.Xml.XmlConfiguration;

namespace System.Xml
{
	/// <summary>Specifies a set of features to support on the <see cref="T:System.Xml.XmlReader" /> object created by the <see cref="Overload:System.Xml.XmlReader.Create" /> method. </summary>
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public sealed class XmlReaderSettings
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlReaderSettings" /> class.</summary>
		public XmlReaderSettings()
		{
			this.Initialize();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.XmlReaderSettings" /> class.</summary>
		/// <param name="resolver">The XML resolver.</param>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		public XmlReaderSettings(XmlResolver resolver)
		{
			this.Initialize(resolver);
		}

		/// <summary>Gets or sets whether asynchronous <see cref="T:System.Xml.XmlReader" /> methods can be used on a particular <see cref="T:System.Xml.XmlReader" /> instance.</summary>
		/// <returns>
		///     <see langword="true" /> if asynchronous methods can be used; otherwise, <see langword="false" />.</returns>
		public bool Async
		{
			get
			{
				return this.useAsync;
			}
			set
			{
				this.CheckReadOnly("Async");
				this.useAsync = value;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Xml.XmlNameTable" /> used for atomized string comparisons.</summary>
		/// <returns>The <see cref="T:System.Xml.XmlNameTable" /> that stores all the atomized strings used by all <see cref="T:System.Xml.XmlReader" /> instances created using this <see cref="T:System.Xml.XmlReaderSettings" /> object.The default is <see langword="null" />. The created <see cref="T:System.Xml.XmlReader" /> instance will use a new empty <see cref="T:System.Xml.NameTable" /> if this value is <see langword="null" />.</returns>
		public XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
			set
			{
				this.CheckReadOnly("NameTable");
				this.nameTable = value;
			}
		}

		internal bool IsXmlResolverSet { get; set; }

		/// <summary>Sets the <see cref="T:System.Xml.XmlResolver" /> used to access external documents.</summary>
		/// <returns>An <see cref="T:System.Xml.XmlResolver" /> used to access external documents. If set to <see langword="null" />, an <see cref="T:System.Xml.XmlException" /> is thrown when the <see cref="T:System.Xml.XmlReader" /> tries to access an external resource. The default is a new <see cref="T:System.Xml.XmlUrlResolver" /> with no credentials.  Starting with the .NET Framework 4.5.2, this setting has a default value of <see langword="null" />.</returns>
		public XmlResolver XmlResolver
		{
			set
			{
				this.CheckReadOnly("XmlResolver");
				this.xmlResolver = value;
				this.IsXmlResolverSet = true;
			}
		}

		internal XmlResolver GetXmlResolver()
		{
			return this.xmlResolver;
		}

		internal XmlResolver GetXmlResolver_CheckConfig()
		{
			if (XmlReaderSection.ProhibitDefaultUrlResolver && !this.IsXmlResolverSet)
			{
				return null;
			}
			return this.xmlResolver;
		}

		/// <summary>Gets or sets line number offset of the <see cref="T:System.Xml.XmlReader" /> object.</summary>
		/// <returns>The line number offset. The default is 0.</returns>
		public int LineNumberOffset
		{
			get
			{
				return this.lineNumberOffset;
			}
			set
			{
				this.CheckReadOnly("LineNumberOffset");
				this.lineNumberOffset = value;
			}
		}

		/// <summary>Gets or sets line position offset of the <see cref="T:System.Xml.XmlReader" /> object.</summary>
		/// <returns>The line position offset. The default is 0.</returns>
		public int LinePositionOffset
		{
			get
			{
				return this.linePositionOffset;
			}
			set
			{
				this.CheckReadOnly("LinePositionOffset");
				this.linePositionOffset = value;
			}
		}

		/// <summary>Gets or sets the level of conformance which the <see cref="T:System.Xml.XmlReader" /> will comply.</summary>
		/// <returns>One of the enumeration values that specifies the level of conformance that the XML reader will enforce. The default is <see cref="F:System.Xml.ConformanceLevel.Document" />.</returns>
		public ConformanceLevel ConformanceLevel
		{
			get
			{
				return this.conformanceLevel;
			}
			set
			{
				this.CheckReadOnly("ConformanceLevel");
				if (value > ConformanceLevel.Document)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.conformanceLevel = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether to do character checking.</summary>
		/// <returns>
		///     <see langword="true" /> to do character checking; otherwise <see langword="false" />. The default is <see langword="true" />.If the <see cref="T:System.Xml.XmlReader" /> is processing text data, it always checks that the XML names and text content are valid, regardless of the property setting. Setting <see cref="P:System.Xml.XmlReaderSettings.CheckCharacters" /> to <see langword="false" /> turns off character checking for character entity references.</returns>
		public bool CheckCharacters
		{
			get
			{
				return this.checkCharacters;
			}
			set
			{
				this.CheckReadOnly("CheckCharacters");
				this.checkCharacters = value;
			}
		}

		/// <summary>Gets or sets a value indicating the maximum allowable number of characters in an XML document. A zero (0) value means no limits on the size of the XML document. A non-zero value specifies the maximum size, in characters.</summary>
		/// <returns>The maximum allowable number of characters in an XML document. The default is 0.</returns>
		public long MaxCharactersInDocument
		{
			get
			{
				return this.maxCharactersInDocument;
			}
			set
			{
				this.CheckReadOnly("MaxCharactersInDocument");
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.maxCharactersInDocument = value;
			}
		}

		/// <summary>Gets or sets a value indicating the maximum allowable number of characters in a document that result from expanding entities.</summary>
		/// <returns>The maximum allowable number of characters from expanded entities. The default is 0.</returns>
		public long MaxCharactersFromEntities
		{
			get
			{
				return this.maxCharactersFromEntities;
			}
			set
			{
				this.CheckReadOnly("MaxCharactersFromEntities");
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.maxCharactersFromEntities = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether to ignore insignificant white space.</summary>
		/// <returns>
		///     <see langword="true" /> to ignore white space; otherwise <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool IgnoreWhitespace
		{
			get
			{
				return this.ignoreWhitespace;
			}
			set
			{
				this.CheckReadOnly("IgnoreWhitespace");
				this.ignoreWhitespace = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether to ignore processing instructions.</summary>
		/// <returns>
		///     <see langword="true" /> to ignore processing instructions; otherwise <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool IgnoreProcessingInstructions
		{
			get
			{
				return this.ignorePIs;
			}
			set
			{
				this.CheckReadOnly("IgnoreProcessingInstructions");
				this.ignorePIs = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether to ignore comments.</summary>
		/// <returns>
		///     <see langword="true" /> to ignore comments; otherwise <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool IgnoreComments
		{
			get
			{
				return this.ignoreComments;
			}
			set
			{
				this.CheckReadOnly("IgnoreComments");
				this.ignoreComments = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether to prohibit document type definition (DTD) processing. This property is obsolete. Use <see cref="P:System.Xml.XmlTextReader.DtdProcessing" /> instead.</summary>
		/// <returns>
		///     <see langword="true" /> to prohibit DTD processing; otherwise <see langword="false" />. The default is <see langword="true" />.</returns>
		[Obsolete("Use XmlReaderSettings.DtdProcessing property instead.")]
		public bool ProhibitDtd
		{
			get
			{
				return this.dtdProcessing == DtdProcessing.Prohibit;
			}
			set
			{
				this.CheckReadOnly("ProhibitDtd");
				this.dtdProcessing = (value ? DtdProcessing.Prohibit : DtdProcessing.Parse);
			}
		}

		/// <summary>Gets or sets a value that determines the processing of DTDs.</summary>
		/// <returns>One of the enumeration values that determines the processing of DTDs. The default is <see cref="F:System.Xml.DtdProcessing.Prohibit" />.</returns>
		public DtdProcessing DtdProcessing
		{
			get
			{
				return this.dtdProcessing;
			}
			set
			{
				this.CheckReadOnly("DtdProcessing");
				if (value > DtdProcessing.Parse)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.dtdProcessing = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether the underlying stream or <see cref="T:System.IO.TextReader" /> should be closed when the reader is closed.</summary>
		/// <returns>
		///     <see langword="true" /> to close the underlying stream or <see cref="T:System.IO.TextReader" /> when the reader is closed; otherwise <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool CloseInput
		{
			get
			{
				return this.closeInput;
			}
			set
			{
				this.CheckReadOnly("CloseInput");
				this.closeInput = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether the <see cref="T:System.Xml.XmlReader" /> will perform validation or type assignment when reading.</summary>
		/// <returns>One of the <see cref="T:System.Xml.ValidationType" /> values that indicates whether XmlReader will perform validation or type assignment when reading. The default is <see langword="ValidationType.None" />.</returns>
		public ValidationType ValidationType
		{
			get
			{
				return this.validationType;
			}
			set
			{
				this.CheckReadOnly("ValidationType");
				if (value > ValidationType.Schema)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.validationType = value;
			}
		}

		/// <summary>Gets or sets a value indicating the schema validation settings. This setting applies to <see cref="T:System.Xml.XmlReader" /> objects that validate schemas (<see cref="P:System.Xml.XmlReaderSettings.ValidationType" /> property set to <see langword="ValidationType.Schema" />).</summary>
		/// <returns>A bitwise combination of enumeration values that specify validation options. <see cref="F:System.Xml.Schema.XmlSchemaValidationFlags.ProcessIdentityConstraints" /> and <see cref="F:System.Xml.Schema.XmlSchemaValidationFlags.AllowXmlAttributes" /> are enabled by default. <see cref="F:System.Xml.Schema.XmlSchemaValidationFlags.ProcessInlineSchema" />, <see cref="F:System.Xml.Schema.XmlSchemaValidationFlags.ProcessSchemaLocation" />, and <see cref="F:System.Xml.Schema.XmlSchemaValidationFlags.ReportValidationWarnings" /> are disabled by default.</returns>
		public XmlSchemaValidationFlags ValidationFlags
		{
			get
			{
				return this.validationFlags;
			}
			set
			{
				this.CheckReadOnly("ValidationFlags");
				if (value > (XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.AllowXmlAttributes))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.validationFlags = value;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Xml.Schema.XmlSchemaSet" /> to use when performing schema validation.</summary>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchemaSet" /> to use when performing schema validation. The default is an empty <see cref="T:System.Xml.Schema.XmlSchemaSet" /> object.</returns>
		public XmlSchemaSet Schemas
		{
			get
			{
				if (this.schemas == null)
				{
					this.schemas = new XmlSchemaSet();
				}
				return this.schemas;
			}
			set
			{
				this.CheckReadOnly("Schemas");
				this.schemas = value;
			}
		}

		/// <summary>Occurs when the reader encounters validation errors.</summary>
		public event ValidationEventHandler ValidationEventHandler
		{
			add
			{
				this.CheckReadOnly("ValidationEventHandler");
				this.valEventHandler = (ValidationEventHandler)Delegate.Combine(this.valEventHandler, value);
			}
			remove
			{
				this.CheckReadOnly("ValidationEventHandler");
				this.valEventHandler = (ValidationEventHandler)Delegate.Remove(this.valEventHandler, value);
			}
		}

		/// <summary>Resets the members of the settings class to their default values.</summary>
		public void Reset()
		{
			this.CheckReadOnly("Reset");
			this.Initialize();
		}

		/// <summary>Creates a copy of the <see cref="T:System.Xml.XmlReaderSettings" /> instance.</summary>
		/// <returns>The cloned <see cref="T:System.Xml.XmlReaderSettings" /> object.</returns>
		public XmlReaderSettings Clone()
		{
			XmlReaderSettings xmlReaderSettings = base.MemberwiseClone() as XmlReaderSettings;
			xmlReaderSettings.ReadOnly = false;
			return xmlReaderSettings;
		}

		internal ValidationEventHandler GetEventHandler()
		{
			return this.valEventHandler;
		}

		internal XmlReader CreateReader(string inputUri, XmlParserContext inputContext)
		{
			if (inputUri == null)
			{
				throw new ArgumentNullException("inputUri");
			}
			if (inputUri.Length == 0)
			{
				throw new ArgumentException(Res.GetString("The string was not recognized as a valid Uri."), "inputUri");
			}
			XmlResolver xmlResolver = this.GetXmlResolver();
			if (xmlResolver == null)
			{
				xmlResolver = XmlReaderSettings.CreateDefaultResolver();
			}
			XmlReader xmlReader = new XmlTextReaderImpl(inputUri, this, inputContext, xmlResolver);
			if (this.ValidationType != ValidationType.None)
			{
				xmlReader = this.AddValidation(xmlReader);
			}
			if (this.useAsync)
			{
				xmlReader = XmlAsyncCheckReader.CreateAsyncCheckWrapper(xmlReader);
			}
			return xmlReader;
		}

		internal XmlReader CreateReader(Stream input, Uri baseUri, string baseUriString, XmlParserContext inputContext)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (baseUriString == null)
			{
				if (baseUri == null)
				{
					baseUriString = string.Empty;
				}
				else
				{
					baseUriString = baseUri.ToString();
				}
			}
			XmlReader xmlReader = new XmlTextReaderImpl(input, null, 0, this, baseUri, baseUriString, inputContext, this.closeInput);
			if (this.ValidationType != ValidationType.None)
			{
				xmlReader = this.AddValidation(xmlReader);
			}
			if (this.useAsync)
			{
				xmlReader = XmlAsyncCheckReader.CreateAsyncCheckWrapper(xmlReader);
			}
			return xmlReader;
		}

		internal XmlReader CreateReader(TextReader input, string baseUriString, XmlParserContext inputContext)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (baseUriString == null)
			{
				baseUriString = string.Empty;
			}
			XmlReader xmlReader = new XmlTextReaderImpl(input, this, baseUriString, inputContext);
			if (this.ValidationType != ValidationType.None)
			{
				xmlReader = this.AddValidation(xmlReader);
			}
			if (this.useAsync)
			{
				xmlReader = XmlAsyncCheckReader.CreateAsyncCheckWrapper(xmlReader);
			}
			return xmlReader;
		}

		internal XmlReader CreateReader(XmlReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			return this.AddValidationAndConformanceWrapper(reader);
		}

		internal bool ReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
			set
			{
				this.isReadOnly = value;
			}
		}

		private void CheckReadOnly(string propertyName)
		{
			if (this.isReadOnly)
			{
				throw new XmlException("The '{0}' property is read only and cannot be set.", base.GetType().Name + "." + propertyName);
			}
		}

		private void Initialize()
		{
			this.Initialize(null);
		}

		private void Initialize(XmlResolver resolver)
		{
			this.nameTable = null;
			if (!XmlReaderSettings.EnableLegacyXmlSettings())
			{
				this.xmlResolver = resolver;
				this.maxCharactersFromEntities = 10000000L;
			}
			else
			{
				this.xmlResolver = ((resolver == null) ? XmlReaderSettings.CreateDefaultResolver() : resolver);
				this.maxCharactersFromEntities = 0L;
			}
			this.lineNumberOffset = 0;
			this.linePositionOffset = 0;
			this.checkCharacters = true;
			this.conformanceLevel = ConformanceLevel.Document;
			this.ignoreWhitespace = false;
			this.ignorePIs = false;
			this.ignoreComments = false;
			this.dtdProcessing = DtdProcessing.Prohibit;
			this.closeInput = false;
			this.maxCharactersInDocument = 0L;
			this.schemas = null;
			this.validationType = ValidationType.None;
			this.validationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints;
			this.validationFlags |= XmlSchemaValidationFlags.AllowXmlAttributes;
			this.useAsync = false;
			this.isReadOnly = false;
			this.IsXmlResolverSet = false;
		}

		private static XmlResolver CreateDefaultResolver()
		{
			return new XmlUrlResolver();
		}

		internal XmlReader AddValidation(XmlReader reader)
		{
			if (this.validationType == ValidationType.Schema)
			{
				XmlResolver xmlResolver = this.GetXmlResolver_CheckConfig();
				if (xmlResolver == null && !this.IsXmlResolverSet && !XmlReaderSettings.EnableLegacyXmlSettings())
				{
					xmlResolver = new XmlUrlResolver();
				}
				reader = new XsdValidatingReader(reader, xmlResolver, this);
			}
			else if (this.validationType == ValidationType.DTD)
			{
				reader = this.CreateDtdValidatingReader(reader);
			}
			return reader;
		}

		private XmlReader AddValidationAndConformanceWrapper(XmlReader reader)
		{
			if (this.validationType == ValidationType.DTD)
			{
				reader = this.CreateDtdValidatingReader(reader);
			}
			reader = this.AddConformanceWrapper(reader);
			if (this.validationType == ValidationType.Schema)
			{
				reader = new XsdValidatingReader(reader, this.GetXmlResolver_CheckConfig(), this);
			}
			return reader;
		}

		private XmlValidatingReaderImpl CreateDtdValidatingReader(XmlReader baseReader)
		{
			return new XmlValidatingReaderImpl(baseReader, this.GetEventHandler(), (this.ValidationFlags & XmlSchemaValidationFlags.ProcessIdentityConstraints) > XmlSchemaValidationFlags.None);
		}

		internal XmlReader AddConformanceWrapper(XmlReader baseReader)
		{
			XmlReaderSettings settings = baseReader.Settings;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool ignorePis = false;
			DtdProcessing dtdProcessing = (DtdProcessing)(-1);
			bool flag4 = false;
			if (settings == null)
			{
				if (this.conformanceLevel != ConformanceLevel.Auto && this.conformanceLevel != XmlReader.GetV1ConformanceLevel(baseReader))
				{
					throw new InvalidOperationException(Res.GetString("Cannot change conformance checking to {0}. Make sure the ConformanceLevel in XmlReaderSettings is set to Auto for wrapping scenarios.", new object[]
					{
						this.conformanceLevel.ToString()
					}));
				}
				XmlTextReader xmlTextReader = baseReader as XmlTextReader;
				if (xmlTextReader == null)
				{
					XmlValidatingReader xmlValidatingReader = baseReader as XmlValidatingReader;
					if (xmlValidatingReader != null)
					{
						xmlTextReader = (XmlTextReader)xmlValidatingReader.Reader;
					}
				}
				if (this.ignoreWhitespace)
				{
					WhitespaceHandling whitespaceHandling = WhitespaceHandling.All;
					if (xmlTextReader != null)
					{
						whitespaceHandling = xmlTextReader.WhitespaceHandling;
					}
					if (whitespaceHandling == WhitespaceHandling.All)
					{
						flag2 = true;
						flag4 = true;
					}
				}
				if (this.ignoreComments)
				{
					flag3 = true;
					flag4 = true;
				}
				if (this.ignorePIs)
				{
					ignorePis = true;
					flag4 = true;
				}
				DtdProcessing dtdProcessing2 = DtdProcessing.Parse;
				if (xmlTextReader != null)
				{
					dtdProcessing2 = xmlTextReader.DtdProcessing;
				}
				if ((this.dtdProcessing == DtdProcessing.Prohibit && dtdProcessing2 != DtdProcessing.Prohibit) || (this.dtdProcessing == DtdProcessing.Ignore && dtdProcessing2 == DtdProcessing.Parse))
				{
					dtdProcessing = this.dtdProcessing;
					flag4 = true;
				}
			}
			else
			{
				if (this.conformanceLevel != settings.ConformanceLevel && this.conformanceLevel != ConformanceLevel.Auto)
				{
					throw new InvalidOperationException(Res.GetString("Cannot change conformance checking to {0}. Make sure the ConformanceLevel in XmlReaderSettings is set to Auto for wrapping scenarios.", new object[]
					{
						this.conformanceLevel.ToString()
					}));
				}
				if (this.checkCharacters && !settings.CheckCharacters)
				{
					flag = true;
					flag4 = true;
				}
				if (this.ignoreWhitespace && !settings.IgnoreWhitespace)
				{
					flag2 = true;
					flag4 = true;
				}
				if (this.ignoreComments && !settings.IgnoreComments)
				{
					flag3 = true;
					flag4 = true;
				}
				if (this.ignorePIs && !settings.IgnoreProcessingInstructions)
				{
					ignorePis = true;
					flag4 = true;
				}
				if ((this.dtdProcessing == DtdProcessing.Prohibit && settings.DtdProcessing != DtdProcessing.Prohibit) || (this.dtdProcessing == DtdProcessing.Ignore && settings.DtdProcessing == DtdProcessing.Parse))
				{
					dtdProcessing = this.dtdProcessing;
					flag4 = true;
				}
			}
			if (!flag4)
			{
				return baseReader;
			}
			IXmlNamespaceResolver xmlNamespaceResolver = baseReader as IXmlNamespaceResolver;
			if (xmlNamespaceResolver != null)
			{
				return new XmlCharCheckingReaderWithNS(baseReader, xmlNamespaceResolver, flag, flag2, flag3, ignorePis, dtdProcessing);
			}
			return new XmlCharCheckingReader(baseReader, flag, flag2, flag3, ignorePis, dtdProcessing);
		}

		internal static bool EnableLegacyXmlSettings()
		{
			if (XmlReaderSettings.s_enableLegacyXmlSettings != null)
			{
				return XmlReaderSettings.s_enableLegacyXmlSettings.Value;
			}
			if (!BinaryCompatibility.TargetsAtLeast_Desktop_V4_5_2)
			{
				XmlReaderSettings.s_enableLegacyXmlSettings = new bool?(true);
				return XmlReaderSettings.s_enableLegacyXmlSettings.Value;
			}
			XmlReaderSettings.s_enableLegacyXmlSettings = new bool?(false);
			return XmlReaderSettings.s_enableLegacyXmlSettings.Value;
		}

		private bool useAsync;

		private XmlNameTable nameTable;

		private XmlResolver xmlResolver;

		private int lineNumberOffset;

		private int linePositionOffset;

		private ConformanceLevel conformanceLevel;

		private bool checkCharacters;

		private long maxCharactersInDocument;

		private long maxCharactersFromEntities;

		private bool ignoreWhitespace;

		private bool ignorePIs;

		private bool ignoreComments;

		private DtdProcessing dtdProcessing;

		private ValidationType validationType;

		private XmlSchemaValidationFlags validationFlags;

		private XmlSchemaSet schemas;

		private ValidationEventHandler valEventHandler;

		private bool closeInput;

		private bool isReadOnly;

		private static bool? s_enableLegacyXmlSettings;
	}
}
