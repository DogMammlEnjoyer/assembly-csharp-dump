using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using System.Xml.XmlConfiguration;

namespace System.Xml.Schema
{
	/// <summary>An in-memory representation of an XML Schema, as specified in the World Wide Web Consortium (W3C) XML Schema Part 1: Structures and XML Schema Part 2: Datatypes specifications.</summary>
	[XmlRoot("schema", Namespace = "http://www.w3.org/2001/XMLSchema")]
	public class XmlSchema : XmlSchemaObject
	{
		/// <summary>Reads an XML Schema from the supplied <see cref="T:System.IO.TextReader" />.</summary>
		/// <param name="reader">The <see langword="TextReader" /> containing the XML Schema to read. </param>
		/// <param name="validationEventHandler">The validation event handler that receives information about the XML Schema syntax errors. </param>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchema" /> object representing the XML Schema.</returns>
		/// <exception cref="T:System.Xml.Schema.XmlSchemaException">An <see cref="T:System.Xml.Schema.XmlSchemaException" /> is raised if no <see cref="T:System.Xml.Schema.ValidationEventHandler" /> is specified.</exception>
		public static XmlSchema Read(TextReader reader, ValidationEventHandler validationEventHandler)
		{
			return XmlSchema.Read(new XmlTextReader(reader), validationEventHandler);
		}

		/// <summary>Reads an XML Schema  from the supplied stream.</summary>
		/// <param name="stream">The supplied data stream. </param>
		/// <param name="validationEventHandler">The validation event handler that receives information about XML Schema syntax errors. </param>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchema" /> object representing the XML Schema.</returns>
		/// <exception cref="T:System.Xml.Schema.XmlSchemaException">An <see cref="T:System.Xml.Schema.XmlSchemaException" /> is raised if no <see cref="T:System.Xml.Schema.ValidationEventHandler" /> is specified.</exception>
		public static XmlSchema Read(Stream stream, ValidationEventHandler validationEventHandler)
		{
			return XmlSchema.Read(new XmlTextReader(stream), validationEventHandler);
		}

		/// <summary>Reads an XML Schema from the supplied <see cref="T:System.Xml.XmlReader" />.</summary>
		/// <param name="reader">The <see langword="XmlReader" /> containing the XML Schema to read. </param>
		/// <param name="validationEventHandler">The validation event handler that receives information about the XML Schema syntax errors. </param>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchema" /> object representing the XML Schema.</returns>
		/// <exception cref="T:System.Xml.Schema.XmlSchemaException">An <see cref="T:System.Xml.Schema.XmlSchemaException" /> is raised if no <see cref="T:System.Xml.Schema.ValidationEventHandler" /> is specified.</exception>
		public static XmlSchema Read(XmlReader reader, ValidationEventHandler validationEventHandler)
		{
			XmlNameTable xmlNameTable = reader.NameTable;
			Parser parser = new Parser(SchemaType.XSD, xmlNameTable, new SchemaNames(xmlNameTable), validationEventHandler);
			try
			{
				parser.Parse(reader, null);
			}
			catch (XmlSchemaException ex)
			{
				if (validationEventHandler != null)
				{
					validationEventHandler(null, new ValidationEventArgs(ex));
					return null;
				}
				throw ex;
			}
			return parser.XmlSchema;
		}

		/// <summary>Writes the XML Schema to the supplied data stream.</summary>
		/// <param name="stream">The supplied data stream. </param>
		public void Write(Stream stream)
		{
			this.Write(stream, null);
		}

		/// <summary>Writes the XML Schema to the supplied <see cref="T:System.IO.Stream" /> using the <see cref="T:System.Xml.XmlNamespaceManager" /> specified.</summary>
		/// <param name="stream">The supplied data stream. </param>
		/// <param name="namespaceManager">The <see cref="T:System.Xml.XmlNamespaceManager" />.</param>
		public void Write(Stream stream, XmlNamespaceManager namespaceManager)
		{
			this.Write(new XmlTextWriter(stream, null)
			{
				Formatting = Formatting.Indented
			}, namespaceManager);
		}

		/// <summary>Writes the XML Schema to the supplied <see cref="T:System.IO.TextWriter" />.</summary>
		/// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to write to.</param>
		public void Write(TextWriter writer)
		{
			this.Write(writer, null);
		}

		/// <summary>Writes the XML Schema to the supplied <see cref="T:System.IO.TextWriter" />.</summary>
		/// <param name="writer">The <see cref="T:System.IO.TextWriter" /> to write to.</param>
		/// <param name="namespaceManager">The <see cref="T:System.Xml.XmlNamespaceManager" />. </param>
		public void Write(TextWriter writer, XmlNamespaceManager namespaceManager)
		{
			this.Write(new XmlTextWriter(writer)
			{
				Formatting = Formatting.Indented
			}, namespaceManager);
		}

		/// <summary>Writes the XML Schema to the supplied <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> to write to. </param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="writer" /> parameter is null.</exception>
		public void Write(XmlWriter writer)
		{
			this.Write(writer, null);
		}

		/// <summary>Writes the XML Schema to the supplied <see cref="T:System.Xml.XmlWriter" />.</summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> to write to.</param>
		/// <param name="namespaceManager">The <see cref="T:System.Xml.XmlNamespaceManager" />. </param>
		public void Write(XmlWriter writer, XmlNamespaceManager namespaceManager)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlSchema));
			XmlSerializerNamespaces xmlSerializerNamespaces;
			if (namespaceManager != null)
			{
				xmlSerializerNamespaces = new XmlSerializerNamespaces();
				bool flag = false;
				if (base.Namespaces != null)
				{
					flag = (base.Namespaces.Namespaces["xs"] != null || base.Namespaces.Namespaces.ContainsValue("http://www.w3.org/2001/XMLSchema"));
				}
				if (!flag && namespaceManager.LookupPrefix("http://www.w3.org/2001/XMLSchema") == null && namespaceManager.LookupNamespace("xs") == null)
				{
					xmlSerializerNamespaces.Add("xs", "http://www.w3.org/2001/XMLSchema");
				}
				using (IEnumerator enumerator = namespaceManager.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						string text = (string)obj;
						if (text != "xml" && text != "xmlns")
						{
							xmlSerializerNamespaces.Add(text, namespaceManager.LookupNamespace(text));
						}
					}
					goto IL_17B;
				}
			}
			if (base.Namespaces != null && base.Namespaces.Count > 0)
			{
				Hashtable namespaces = base.Namespaces.Namespaces;
				if (namespaces["xs"] == null && !namespaces.ContainsValue("http://www.w3.org/2001/XMLSchema"))
				{
					namespaces.Add("xs", "http://www.w3.org/2001/XMLSchema");
				}
				xmlSerializerNamespaces = base.Namespaces;
			}
			else
			{
				xmlSerializerNamespaces = new XmlSerializerNamespaces();
				xmlSerializerNamespaces.Add("xs", "http://www.w3.org/2001/XMLSchema");
				if (this.targetNs != null && this.targetNs.Length != 0)
				{
					xmlSerializerNamespaces.Add("tns", this.targetNs);
				}
			}
			IL_17B:
			xmlSerializer.Serialize(writer, this, xmlSerializerNamespaces);
		}

		/// <summary>Compiles the XML Schema Object Model (SOM) into schema information for validation. Used to check the syntactic and semantic structure of the programmatically built SOM. Semantic validation checking is performed during compilation.</summary>
		/// <param name="validationEventHandler">The validation event handler that receives information about XML Schema validation errors. </param>
		[Obsolete("Use System.Xml.Schema.XmlSchemaSet for schema compilation and validation. http://go.microsoft.com/fwlink/?linkid=14202")]
		public void Compile(ValidationEventHandler validationEventHandler)
		{
			SchemaInfo schemaInfo = new SchemaInfo();
			schemaInfo.SchemaType = SchemaType.XSD;
			this.CompileSchema(null, XmlReaderSection.CreateDefaultResolver(), schemaInfo, null, validationEventHandler, this.NameTable, false);
		}

		/// <summary>Compiles the XML Schema Object Model (SOM) into schema information for validation. Used to check the syntactic and semantic structure of the programmatically built SOM. Semantic validation checking is performed during compilation.</summary>
		/// <param name="validationEventHandler">The validation event handler that receives information about the XML Schema validation errors. </param>
		/// <param name="resolver">The <see langword="XmlResolver" /> used to resolve namespaces referenced in <see langword="include" /> and <see langword="import" /> elements. </param>
		[Obsolete("Use System.Xml.Schema.XmlSchemaSet for schema compilation and validation. http://go.microsoft.com/fwlink/?linkid=14202")]
		public void Compile(ValidationEventHandler validationEventHandler, XmlResolver resolver)
		{
			this.CompileSchema(null, resolver, new SchemaInfo
			{
				SchemaType = SchemaType.XSD
			}, null, validationEventHandler, this.NameTable, false);
		}

		internal bool CompileSchema(XmlSchemaCollection xsc, XmlResolver resolver, SchemaInfo schemaInfo, string ns, ValidationEventHandler validationEventHandler, XmlNameTable nameTable, bool CompileContentModel)
		{
			bool result;
			lock (this)
			{
				if (!new SchemaCollectionPreprocessor(nameTable, null, validationEventHandler)
				{
					XmlResolver = resolver
				}.Execute(this, ns, true, xsc))
				{
					result = false;
				}
				else
				{
					SchemaCollectionCompiler schemaCollectionCompiler = new SchemaCollectionCompiler(nameTable, validationEventHandler);
					this.isCompiled = schemaCollectionCompiler.Execute(this, schemaInfo, CompileContentModel);
					this.SetIsCompiled(this.isCompiled);
					result = this.isCompiled;
				}
			}
			return result;
		}

		internal void CompileSchemaInSet(XmlNameTable nameTable, ValidationEventHandler eventHandler, XmlSchemaCompilationSettings compilationSettings)
		{
			Compiler compiler = new Compiler(nameTable, eventHandler, null, compilationSettings);
			compiler.Prepare(this, true);
			this.isCompiledBySet = compiler.Compile();
		}

		/// <summary>Gets or sets the form for attributes declared in the target namespace of the schema.</summary>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchemaForm" /> value that indicates if attributes from the target namespace are required to be qualified with the namespace prefix. The default is <see cref="F:System.Xml.Schema.XmlSchemaForm.None" />.</returns>
		[DefaultValue(XmlSchemaForm.None)]
		[XmlAttribute("attributeFormDefault")]
		public XmlSchemaForm AttributeFormDefault
		{
			get
			{
				return this.attributeFormDefault;
			}
			set
			{
				this.attributeFormDefault = value;
			}
		}

		/// <summary>Gets or sets the <see langword="blockDefault" /> attribute which sets the default value of the <see langword="block" /> attribute on element and complex types in the <see langword="targetNamespace" /> of the schema.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaDerivationMethod" /> value representing the different methods for preventing derivation. The default value is <see langword="XmlSchemaDerivationMethod.None" />.</returns>
		[XmlAttribute("blockDefault")]
		[DefaultValue(XmlSchemaDerivationMethod.None)]
		public XmlSchemaDerivationMethod BlockDefault
		{
			get
			{
				return this.blockDefault;
			}
			set
			{
				this.blockDefault = value;
			}
		}

		/// <summary>Gets or sets the <see langword="finalDefault" /> attribute which sets the default value of the <see langword="final" /> attribute on elements and complex types in the target namespace of the schema.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaDerivationMethod" /> value representing the different methods for preventing derivation. The default value is <see langword="XmlSchemaDerivationMethod.None" />.</returns>
		[XmlAttribute("finalDefault")]
		[DefaultValue(XmlSchemaDerivationMethod.None)]
		public XmlSchemaDerivationMethod FinalDefault
		{
			get
			{
				return this.finalDefault;
			}
			set
			{
				this.finalDefault = value;
			}
		}

		/// <summary>Gets or sets the form for elements declared in the target namespace of the schema.</summary>
		/// <returns>The <see cref="T:System.Xml.Schema.XmlSchemaForm" /> value that indicates if elements from the target namespace are required to be qualified with the namespace prefix. The default is <see cref="F:System.Xml.Schema.XmlSchemaForm.None" />.</returns>
		[XmlAttribute("elementFormDefault")]
		[DefaultValue(XmlSchemaForm.None)]
		public XmlSchemaForm ElementFormDefault
		{
			get
			{
				return this.elementFormDefault;
			}
			set
			{
				this.elementFormDefault = value;
			}
		}

		/// <summary>Gets or sets the Uniform Resource Identifier (URI) of the schema target namespace.</summary>
		/// <returns>The schema target namespace.</returns>
		[XmlAttribute("targetNamespace", DataType = "anyURI")]
		public string TargetNamespace
		{
			get
			{
				return this.targetNs;
			}
			set
			{
				this.targetNs = value;
			}
		}

		/// <summary>Gets or sets the version of the schema.</summary>
		/// <returns>The version of the schema. The default value is <see langword="String.Empty" />.</returns>
		[XmlAttribute("version", DataType = "token")]
		public string Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		/// <summary>Gets the collection of included and imported schemas.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaObjectCollection" /> of the included and imported schemas.</returns>
		[XmlElement("include", typeof(XmlSchemaInclude))]
		[XmlElement("import", typeof(XmlSchemaImport))]
		[XmlElement("redefine", typeof(XmlSchemaRedefine))]
		public XmlSchemaObjectCollection Includes
		{
			get
			{
				return this.includes;
			}
		}

		/// <summary>Gets the collection of schema elements in the schema and is used to add new element types at the <see langword="schema" /> element level.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaObjectCollection" /> of schema elements in the schema.</returns>
		[XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroup))]
		[XmlElement("element", typeof(XmlSchemaElement))]
		[XmlElement("group", typeof(XmlSchemaGroup))]
		[XmlElement("attribute", typeof(XmlSchemaAttribute))]
		[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
		[XmlElement("notation", typeof(XmlSchemaNotation))]
		[XmlElement("complexType", typeof(XmlSchemaComplexType))]
		[XmlElement("annotation", typeof(XmlSchemaAnnotation))]
		public XmlSchemaObjectCollection Items
		{
			get
			{
				return this.items;
			}
		}

		/// <summary>Indicates if the schema has been compiled.</summary>
		/// <returns>
		///     <see langword="true" /> if schema has been compiled, otherwise, <see langword="false" />. The default value is <see langword="false" />.</returns>
		[XmlIgnore]
		public bool IsCompiled
		{
			get
			{
				return this.isCompiled || this.isCompiledBySet;
			}
		}

		[XmlIgnore]
		internal bool IsCompiledBySet
		{
			get
			{
				return this.isCompiledBySet;
			}
			set
			{
				this.isCompiledBySet = value;
			}
		}

		[XmlIgnore]
		internal bool IsPreprocessed
		{
			get
			{
				return this.isPreprocessed;
			}
			set
			{
				this.isPreprocessed = value;
			}
		}

		[XmlIgnore]
		internal bool IsRedefined
		{
			get
			{
				return this.isRedefined;
			}
			set
			{
				this.isRedefined = value;
			}
		}

		/// <summary>Gets the post-schema-compilation value for all the attributes in the schema.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaObjectTable" /> collection of all the attributes in the schema.</returns>
		[XmlIgnore]
		public XmlSchemaObjectTable Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new XmlSchemaObjectTable();
				}
				return this.attributes;
			}
		}

		/// <summary>Gets the post-schema-compilation value of all the global attribute groups in the schema.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaObjectTable" /> collection of all the global attribute groups in the schema.</returns>
		[XmlIgnore]
		public XmlSchemaObjectTable AttributeGroups
		{
			get
			{
				if (this.attributeGroups == null)
				{
					this.attributeGroups = new XmlSchemaObjectTable();
				}
				return this.attributeGroups;
			}
		}

		/// <summary>Gets the post-schema-compilation value of all schema types in the schema.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaObjectCollection" /> of all schema types in the schema.</returns>
		[XmlIgnore]
		public XmlSchemaObjectTable SchemaTypes
		{
			get
			{
				if (this.types == null)
				{
					this.types = new XmlSchemaObjectTable();
				}
				return this.types;
			}
		}

		/// <summary>Gets the post-schema-compilation value for all the elements in the schema.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaObjectTable" /> collection of all the elements in the schema.</returns>
		[XmlIgnore]
		public XmlSchemaObjectTable Elements
		{
			get
			{
				if (this.elements == null)
				{
					this.elements = new XmlSchemaObjectTable();
				}
				return this.elements;
			}
		}

		/// <summary>Gets or sets the string ID.</summary>
		/// <returns>The ID of the string. The default value is <see langword="String.Empty" />.</returns>
		[XmlAttribute("id", DataType = "ID")]
		public string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		/// <summary>Gets and sets the qualified attributes which do not belong to the schema target namespace.</summary>
		/// <returns>An array of qualified <see cref="T:System.Xml.XmlAttribute" /> objects that do not belong to the schema target namespace.</returns>
		[XmlAnyAttribute]
		public XmlAttribute[] UnhandledAttributes
		{
			get
			{
				return this.moreAttributes;
			}
			set
			{
				this.moreAttributes = value;
			}
		}

		/// <summary>Gets the post-schema-compilation value of all the groups in the schema.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaObjectTable" /> collection of all the groups in the schema.</returns>
		[XmlIgnore]
		public XmlSchemaObjectTable Groups
		{
			get
			{
				return this.groups;
			}
		}

		/// <summary>Gets the post-schema-compilation value for all notations in the schema.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaObjectTable" /> collection of all notations in the schema.</returns>
		[XmlIgnore]
		public XmlSchemaObjectTable Notations
		{
			get
			{
				return this.notations;
			}
		}

		[XmlIgnore]
		internal XmlSchemaObjectTable IdentityConstraints
		{
			get
			{
				return this.identityConstraints;
			}
		}

		[XmlIgnore]
		internal Uri BaseUri
		{
			get
			{
				return this.baseUri;
			}
			set
			{
				this.baseUri = value;
			}
		}

		[XmlIgnore]
		internal int SchemaId
		{
			get
			{
				if (this.schemaId == -1)
				{
					this.schemaId = Interlocked.Increment(ref XmlSchema.globalIdCounter);
				}
				return this.schemaId;
			}
		}

		[XmlIgnore]
		internal bool IsChameleon
		{
			get
			{
				return this.isChameleon;
			}
			set
			{
				this.isChameleon = value;
			}
		}

		[XmlIgnore]
		internal Hashtable Ids
		{
			get
			{
				return this.ids;
			}
		}

		[XmlIgnore]
		internal XmlDocument Document
		{
			get
			{
				if (this.document == null)
				{
					this.document = new XmlDocument();
				}
				return this.document;
			}
		}

		[XmlIgnore]
		internal int ErrorCount
		{
			get
			{
				return this.errorCount;
			}
			set
			{
				this.errorCount = value;
			}
		}

		internal new XmlSchema Clone()
		{
			XmlSchema xmlSchema = new XmlSchema();
			xmlSchema.attributeFormDefault = this.attributeFormDefault;
			xmlSchema.elementFormDefault = this.elementFormDefault;
			xmlSchema.blockDefault = this.blockDefault;
			xmlSchema.finalDefault = this.finalDefault;
			xmlSchema.targetNs = this.targetNs;
			xmlSchema.version = this.version;
			xmlSchema.includes = this.includes;
			xmlSchema.Namespaces = base.Namespaces;
			xmlSchema.items = this.items;
			xmlSchema.BaseUri = this.BaseUri;
			SchemaCollectionCompiler.Cleanup(xmlSchema);
			return xmlSchema;
		}

		internal XmlSchema DeepClone()
		{
			XmlSchema xmlSchema = new XmlSchema();
			xmlSchema.attributeFormDefault = this.attributeFormDefault;
			xmlSchema.elementFormDefault = this.elementFormDefault;
			xmlSchema.blockDefault = this.blockDefault;
			xmlSchema.finalDefault = this.finalDefault;
			xmlSchema.targetNs = this.targetNs;
			xmlSchema.version = this.version;
			xmlSchema.isPreprocessed = this.isPreprocessed;
			for (int i = 0; i < this.items.Count; i++)
			{
				XmlSchemaComplexType xmlSchemaComplexType;
				XmlSchemaObject item;
				XmlSchemaElement xmlSchemaElement;
				XmlSchemaGroup xmlSchemaGroup;
				if ((xmlSchemaComplexType = (this.items[i] as XmlSchemaComplexType)) != null)
				{
					item = xmlSchemaComplexType.Clone(this);
				}
				else if ((xmlSchemaElement = (this.items[i] as XmlSchemaElement)) != null)
				{
					item = xmlSchemaElement.Clone(this);
				}
				else if ((xmlSchemaGroup = (this.items[i] as XmlSchemaGroup)) != null)
				{
					item = xmlSchemaGroup.Clone(this);
				}
				else
				{
					item = this.items[i].Clone();
				}
				xmlSchema.Items.Add(item);
			}
			for (int j = 0; j < this.includes.Count; j++)
			{
				XmlSchemaExternal item2 = (XmlSchemaExternal)this.includes[j].Clone();
				xmlSchema.Includes.Add(item2);
			}
			xmlSchema.Namespaces = base.Namespaces;
			xmlSchema.BaseUri = this.BaseUri;
			return xmlSchema;
		}

		[XmlIgnore]
		internal override string IdAttribute
		{
			get
			{
				return this.Id;
			}
			set
			{
				this.Id = value;
			}
		}

		internal void SetIsCompiled(bool isCompiled)
		{
			this.isCompiled = isCompiled;
		}

		internal override void SetUnhandledAttributes(XmlAttribute[] moreAttributes)
		{
			this.moreAttributes = moreAttributes;
		}

		internal override void AddAnnotation(XmlSchemaAnnotation annotation)
		{
			this.items.Add(annotation);
		}

		internal XmlNameTable NameTable
		{
			get
			{
				if (this.nameTable == null)
				{
					this.nameTable = new NameTable();
				}
				return this.nameTable;
			}
		}

		internal ArrayList ImportedSchemas
		{
			get
			{
				if (this.importedSchemas == null)
				{
					this.importedSchemas = new ArrayList();
				}
				return this.importedSchemas;
			}
		}

		internal ArrayList ImportedNamespaces
		{
			get
			{
				if (this.importedNamespaces == null)
				{
					this.importedNamespaces = new ArrayList();
				}
				return this.importedNamespaces;
			}
		}

		internal void GetExternalSchemasList(IList extList, XmlSchema schema)
		{
			if (extList.Contains(schema))
			{
				return;
			}
			extList.Add(schema);
			for (int i = 0; i < schema.Includes.Count; i++)
			{
				XmlSchemaExternal xmlSchemaExternal = (XmlSchemaExternal)schema.Includes[i];
				if (xmlSchemaExternal.Schema != null)
				{
					this.GetExternalSchemasList(extList, xmlSchemaExternal.Schema);
				}
			}
		}

		/// <summary>The XML schema namespace. This field is constant.</summary>
		public const string Namespace = "http://www.w3.org/2001/XMLSchema";

		/// <summary>The XML schema instance namespace. This field is constant. </summary>
		public const string InstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";

		private XmlSchemaForm attributeFormDefault;

		private XmlSchemaForm elementFormDefault;

		private XmlSchemaDerivationMethod blockDefault = XmlSchemaDerivationMethod.None;

		private XmlSchemaDerivationMethod finalDefault = XmlSchemaDerivationMethod.None;

		private string targetNs;

		private string version;

		private XmlSchemaObjectCollection includes = new XmlSchemaObjectCollection();

		private XmlSchemaObjectCollection items = new XmlSchemaObjectCollection();

		private string id;

		private XmlAttribute[] moreAttributes;

		private bool isCompiled;

		private bool isCompiledBySet;

		private bool isPreprocessed;

		private bool isRedefined;

		private int errorCount;

		private XmlSchemaObjectTable attributes;

		private XmlSchemaObjectTable attributeGroups = new XmlSchemaObjectTable();

		private XmlSchemaObjectTable elements = new XmlSchemaObjectTable();

		private XmlSchemaObjectTable types = new XmlSchemaObjectTable();

		private XmlSchemaObjectTable groups = new XmlSchemaObjectTable();

		private XmlSchemaObjectTable notations = new XmlSchemaObjectTable();

		private XmlSchemaObjectTable identityConstraints = new XmlSchemaObjectTable();

		private static int globalIdCounter = -1;

		private ArrayList importedSchemas;

		private ArrayList importedNamespaces;

		private int schemaId = -1;

		private Uri baseUri;

		private bool isChameleon;

		private Hashtable ids = new Hashtable();

		private XmlDocument document;

		private XmlNameTable nameTable;
	}
}
