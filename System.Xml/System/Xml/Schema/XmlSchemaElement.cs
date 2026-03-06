using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="element" /> element from XML Schema as specified by the World Wide Web Consortium (W3C). This class is the base class for all particle types and is used to describe an element in an XML document.</summary>
	public class XmlSchemaElement : XmlSchemaParticle
	{
		/// <summary>Gets or sets information to indicate if the element can be used in an instance document.</summary>
		/// <returns>If <see langword="true" />, the element cannot appear in the instance document. The default is <see langword="false" />.Optional.</returns>
		[DefaultValue(false)]
		[XmlAttribute("abstract")]
		public bool IsAbstract
		{
			get
			{
				return this.isAbstract;
			}
			set
			{
				this.isAbstract = value;
				this.hasAbstractAttribute = true;
			}
		}

		/// <summary>Gets or sets a <see langword="Block" /> derivation.</summary>
		/// <returns>The attribute used to block a type derivation. Default value is <see langword="XmlSchemaDerivationMethod.None" />.Optional.</returns>
		[DefaultValue(XmlSchemaDerivationMethod.None)]
		[XmlAttribute("block")]
		public XmlSchemaDerivationMethod Block
		{
			get
			{
				return this.block;
			}
			set
			{
				this.block = value;
			}
		}

		/// <summary>Gets or sets the default value of the element if its content is a simple type or content of the element is <see langword="textOnly" />.</summary>
		/// <returns>The default value for the element. The default is a null reference.Optional.</returns>
		[DefaultValue(null)]
		[XmlAttribute("default")]
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				this.defaultValue = value;
			}
		}

		/// <summary>Gets or sets the <see langword="Final" /> property to indicate that no further derivations are allowed.</summary>
		/// <returns>The <see langword="Final" /> property. The default is <see langword="XmlSchemaDerivationMethod.None" />.Optional.</returns>
		[DefaultValue(XmlSchemaDerivationMethod.None)]
		[XmlAttribute("final")]
		public XmlSchemaDerivationMethod Final
		{
			get
			{
				return this.final;
			}
			set
			{
				this.final = value;
			}
		}

		/// <summary>Gets or sets the fixed value.</summary>
		/// <returns>The fixed value that is predetermined and unchangeable. The default is a null reference.Optional.</returns>
		[DefaultValue(null)]
		[XmlAttribute("fixed")]
		public string FixedValue
		{
			get
			{
				return this.fixedValue;
			}
			set
			{
				this.fixedValue = value;
			}
		}

		/// <summary>Gets or sets the form for the element.</summary>
		/// <returns>The form for the element. The default is the <see cref="P:System.Xml.Schema.XmlSchema.ElementFormDefault" /> value.Optional.</returns>
		[DefaultValue(XmlSchemaForm.None)]
		[XmlAttribute("form")]
		public XmlSchemaForm Form
		{
			get
			{
				return this.form;
			}
			set
			{
				this.form = value;
			}
		}

		/// <summary>Gets or sets the name of the element.</summary>
		/// <returns>The name of the element. The default is <see langword="String.Empty" />.</returns>
		[DefaultValue("")]
		[XmlAttribute("name")]
		public string Name
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

		/// <summary>Gets or sets information that indicates if <see langword="xsi:nil" /> can occur in the instance data. Indicates if an explicit nil value can be assigned to the element.</summary>
		/// <returns>If nillable is <see langword="true" />, this enables an instance of the element to have the <see langword="nil" /> attribute set to <see langword="true" />. The <see langword="nil" /> attribute is defined as part of the XML Schema namespace for instances. The default is <see langword="false" />.Optional.</returns>
		[DefaultValue(false)]
		[XmlAttribute("nillable")]
		public bool IsNillable
		{
			get
			{
				return this.isNillable;
			}
			set
			{
				this.isNillable = value;
				this.hasNillableAttribute = true;
			}
		}

		[XmlIgnore]
		internal bool HasNillableAttribute
		{
			get
			{
				return this.hasNillableAttribute;
			}
		}

		[XmlIgnore]
		internal bool HasAbstractAttribute
		{
			get
			{
				return this.hasAbstractAttribute;
			}
		}

		/// <summary>Gets or sets the reference name of an element declared in this schema (or another schema indicated by the specified namespace).</summary>
		/// <returns>The reference name of the element.</returns>
		[XmlAttribute("ref")]
		public XmlQualifiedName RefName
		{
			get
			{
				return this.refName;
			}
			set
			{
				this.refName = ((value == null) ? XmlQualifiedName.Empty : value);
			}
		}

		/// <summary>Gets or sets the name of an element that is being substituted by this element.</summary>
		/// <returns>The qualified name of an element that is being substituted by this element.Optional.</returns>
		[XmlAttribute("substitutionGroup")]
		public XmlQualifiedName SubstitutionGroup
		{
			get
			{
				return this.substitutionGroup;
			}
			set
			{
				this.substitutionGroup = ((value == null) ? XmlQualifiedName.Empty : value);
			}
		}

		/// <summary>Gets or sets the name of a built-in data type defined in this schema or another schema indicated by the specified namespace.</summary>
		/// <returns>The name of the built-in data type.</returns>
		[XmlAttribute("type")]
		public XmlQualifiedName SchemaTypeName
		{
			get
			{
				return this.typeName;
			}
			set
			{
				this.typeName = ((value == null) ? XmlQualifiedName.Empty : value);
			}
		}

		/// <summary>Gets or sets the type of the element. This can either be a complex type or a simple type.</summary>
		/// <returns>The type of the element.</returns>
		[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
		[XmlElement("complexType", typeof(XmlSchemaComplexType))]
		public XmlSchemaType SchemaType
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		/// <summary>Gets the collection of constraints on the element.</summary>
		/// <returns>The collection of constraints.</returns>
		[XmlElement("key", typeof(XmlSchemaKey))]
		[XmlElement("keyref", typeof(XmlSchemaKeyref))]
		[XmlElement("unique", typeof(XmlSchemaUnique))]
		public XmlSchemaObjectCollection Constraints
		{
			get
			{
				if (this.constraints == null)
				{
					this.constraints = new XmlSchemaObjectCollection();
				}
				return this.constraints;
			}
		}

		/// <summary>Gets the actual qualified name for the given element. </summary>
		/// <returns>The qualified name of the element. The post-compilation value of the <see langword="QualifiedName" /> property.</returns>
		[XmlIgnore]
		public XmlQualifiedName QualifiedName
		{
			get
			{
				return this.qualifiedName;
			}
		}

		/// <summary>Gets a common language runtime (CLR) object based on the <see cref="T:System.Xml.Schema.XmlSchemaElement" /> or <see cref="T:System.Xml.Schema.XmlSchemaElement" /> of the element, which holds the post-compilation value of the <see langword="ElementType" /> property.</summary>
		/// <returns>The common language runtime object. The post-compilation value of the <see langword="ElementType" /> property.</returns>
		[XmlIgnore]
		[Obsolete("This property has been deprecated. Please use ElementSchemaType property that returns a strongly typed element type. http://go.microsoft.com/fwlink/?linkid=14202")]
		public object ElementType
		{
			get
			{
				if (this.elementType == null)
				{
					return null;
				}
				if (this.elementType.QualifiedName.Namespace == "http://www.w3.org/2001/XMLSchema")
				{
					return this.elementType.Datatype;
				}
				return this.elementType;
			}
		}

		/// <summary>Gets an <see cref="T:System.Xml.Schema.XmlSchemaType" /> object representing the type of the element based on the <see cref="P:System.Xml.Schema.XmlSchemaElement.SchemaType" /> or <see cref="P:System.Xml.Schema.XmlSchemaElement.SchemaTypeName" /> values of the element.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaType" /> object.</returns>
		[XmlIgnore]
		public XmlSchemaType ElementSchemaType
		{
			get
			{
				return this.elementType;
			}
		}

		/// <summary>Gets the post-compilation value of the <see langword="Block" /> property.</summary>
		/// <returns>The post-compilation value of the <see langword="Block" /> property. The default is the <see langword="BlockDefault" /> value on the <see langword="schema" /> element.</returns>
		[XmlIgnore]
		public XmlSchemaDerivationMethod BlockResolved
		{
			get
			{
				return this.blockResolved;
			}
		}

		/// <summary>Gets the post-compilation value of the <see langword="Final" /> property.</summary>
		/// <returns>The post-compilation value of the <see langword="Final" /> property. Default value is the <see langword="FinalDefault" /> value on the <see langword="schema" /> element.</returns>
		[XmlIgnore]
		public XmlSchemaDerivationMethod FinalResolved
		{
			get
			{
				return this.finalResolved;
			}
		}

		internal XmlReader Validate(XmlReader reader, XmlResolver resolver, XmlSchemaSet schemaSet, ValidationEventHandler valEventHandler)
		{
			if (schemaSet != null)
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				xmlReaderSettings.Schemas = schemaSet;
				xmlReaderSettings.ValidationEventHandler += valEventHandler;
				return new XsdValidatingReader(reader, resolver, xmlReaderSettings, this);
			}
			return null;
		}

		internal void SetQualifiedName(XmlQualifiedName value)
		{
			this.qualifiedName = value;
		}

		internal void SetElementType(XmlSchemaType value)
		{
			this.elementType = value;
		}

		internal void SetBlockResolved(XmlSchemaDerivationMethod value)
		{
			this.blockResolved = value;
		}

		internal void SetFinalResolved(XmlSchemaDerivationMethod value)
		{
			this.finalResolved = value;
		}

		[XmlIgnore]
		internal bool HasDefault
		{
			get
			{
				return this.defaultValue != null && this.defaultValue.Length > 0;
			}
		}

		internal bool HasConstraints
		{
			get
			{
				return this.constraints != null && this.constraints.Count > 0;
			}
		}

		internal bool IsLocalTypeDerivationChecked
		{
			get
			{
				return this.isLocalTypeDerivationChecked;
			}
			set
			{
				this.isLocalTypeDerivationChecked = value;
			}
		}

		internal SchemaElementDecl ElementDecl
		{
			get
			{
				return this.elementDecl;
			}
			set
			{
				this.elementDecl = value;
			}
		}

		[XmlIgnore]
		internal override string NameAttribute
		{
			get
			{
				return this.Name;
			}
			set
			{
				this.Name = value;
			}
		}

		[XmlIgnore]
		internal override string NameString
		{
			get
			{
				return this.qualifiedName.ToString();
			}
		}

		internal override XmlSchemaObject Clone()
		{
			return this.Clone(null);
		}

		internal XmlSchemaObject Clone(XmlSchema parentSchema)
		{
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)base.MemberwiseClone();
			xmlSchemaElement.refName = this.refName.Clone();
			xmlSchemaElement.substitutionGroup = this.substitutionGroup.Clone();
			xmlSchemaElement.typeName = this.typeName.Clone();
			xmlSchemaElement.qualifiedName = this.qualifiedName.Clone();
			XmlSchemaComplexType xmlSchemaComplexType = this.type as XmlSchemaComplexType;
			if (xmlSchemaComplexType != null && xmlSchemaComplexType.QualifiedName.IsEmpty)
			{
				xmlSchemaElement.type = (XmlSchemaType)xmlSchemaComplexType.Clone(parentSchema);
			}
			xmlSchemaElement.constraints = null;
			return xmlSchemaElement;
		}

		private bool isAbstract;

		private bool hasAbstractAttribute;

		private bool isNillable;

		private bool hasNillableAttribute;

		private bool isLocalTypeDerivationChecked;

		private XmlSchemaDerivationMethod block = XmlSchemaDerivationMethod.None;

		private XmlSchemaDerivationMethod final = XmlSchemaDerivationMethod.None;

		private XmlSchemaForm form;

		private string defaultValue;

		private string fixedValue;

		private string name;

		private XmlQualifiedName refName = XmlQualifiedName.Empty;

		private XmlQualifiedName substitutionGroup = XmlQualifiedName.Empty;

		private XmlQualifiedName typeName = XmlQualifiedName.Empty;

		private XmlSchemaType type;

		private XmlQualifiedName qualifiedName = XmlQualifiedName.Empty;

		private XmlSchemaType elementType;

		private XmlSchemaDerivationMethod blockResolved;

		private XmlSchemaDerivationMethod finalResolved;

		private XmlSchemaObjectCollection constraints;

		private SchemaElementDecl elementDecl;
	}
}
