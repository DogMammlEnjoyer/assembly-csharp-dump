using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="attribute" /> element from the XML Schema as specified by the World Wide Web Consortium (W3C). Attributes provide additional information for other document elements. The attribute tag is nested between the tags of a document's element for the schema. The XML document displays attributes as named items in the opening tag of an element.</summary>
	public class XmlSchemaAttribute : XmlSchemaAnnotated
	{
		/// <summary>Gets or sets the default value for the attribute.</summary>
		/// <returns>The default value for the attribute. The default is a null reference.Optional.</returns>
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

		/// <summary>Gets or sets the fixed value for the attribute.</summary>
		/// <returns>The fixed value for the attribute. The default is null.Optional.</returns>
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

		/// <summary>Gets or sets the form for the attribute.</summary>
		/// <returns>One of the <see cref="T:System.Xml.Schema.XmlSchemaForm" /> values. The default is the value of the <see cref="P:System.Xml.Schema.XmlSchema.AttributeFormDefault" /> of the schema element containing the attribute.Optional.</returns>
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

		/// <summary>Gets or sets the name of the attribute.</summary>
		/// <returns>The name of the attribute.</returns>
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

		/// <summary>Gets or sets the name of an attribute declared in this schema (or another schema indicated by the specified namespace).</summary>
		/// <returns>The name of the attribute declared.</returns>
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

		/// <summary>Gets or sets the name of the simple type defined in this schema (or another schema indicated by the specified namespace).</summary>
		/// <returns>The name of the simple type.</returns>
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

		/// <summary>Gets or sets the attribute type to a simple type.</summary>
		/// <returns>The simple type defined in this schema.</returns>
		[XmlElement("simpleType")]
		public XmlSchemaSimpleType SchemaType
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

		/// <summary>Gets or sets information about how the attribute is used.</summary>
		/// <returns>One of the following values: None, Prohibited, Optional, or Required. The default is Optional.Optional.</returns>
		[XmlAttribute("use")]
		[DefaultValue(XmlSchemaUse.None)]
		public XmlSchemaUse Use
		{
			get
			{
				return this.use;
			}
			set
			{
				this.use = value;
			}
		}

		/// <summary>Gets the qualified name for the attribute.</summary>
		/// <returns>The post-compilation value of the <see langword="QualifiedName" /> property.</returns>
		[XmlIgnore]
		public XmlQualifiedName QualifiedName
		{
			get
			{
				return this.qualifiedName;
			}
		}

		/// <summary>Gets the common language runtime (CLR) object based on the <see cref="P:System.Xml.Schema.XmlSchemaAttribute.SchemaType" /> or <see cref="P:System.Xml.Schema.XmlSchemaAttribute.SchemaTypeName" /> of the attribute that holds the post-compilation value of the <see langword="AttributeType" /> property.</summary>
		/// <returns>The common runtime library (CLR) object that holds the post-compilation value of the <see langword="AttributeType" /> property.</returns>
		[XmlIgnore]
		[Obsolete("This property has been deprecated. Please use AttributeSchemaType property that returns a strongly typed attribute type. http://go.microsoft.com/fwlink/?linkid=14202")]
		public object AttributeType
		{
			get
			{
				if (this.attributeType == null)
				{
					return null;
				}
				if (this.attributeType.QualifiedName.Namespace == "http://www.w3.org/2001/XMLSchema")
				{
					return this.attributeType.Datatype;
				}
				return this.attributeType;
			}
		}

		/// <summary>Gets an <see cref="T:System.Xml.Schema.XmlSchemaSimpleType" /> object representing the type of the attribute based on the <see cref="P:System.Xml.Schema.XmlSchemaAttribute.SchemaType" /> or <see cref="P:System.Xml.Schema.XmlSchemaAttribute.SchemaTypeName" /> of the attribute.</summary>
		/// <returns>An <see cref="T:System.Xml.Schema.XmlSchemaSimpleType" /> object.</returns>
		[XmlIgnore]
		public XmlSchemaSimpleType AttributeSchemaType
		{
			get
			{
				return this.attributeType;
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

		[XmlIgnore]
		internal XmlSchemaDatatype Datatype
		{
			get
			{
				if (this.attributeType != null)
				{
					return this.attributeType.Datatype;
				}
				return null;
			}
		}

		internal void SetQualifiedName(XmlQualifiedName value)
		{
			this.qualifiedName = value;
		}

		internal void SetAttributeType(XmlSchemaSimpleType value)
		{
			this.attributeType = value;
		}

		internal SchemaAttDef AttDef
		{
			get
			{
				return this.attDef;
			}
			set
			{
				this.attDef = value;
			}
		}

		internal bool HasDefault
		{
			get
			{
				return this.defaultValue != null;
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

		internal override XmlSchemaObject Clone()
		{
			XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)base.MemberwiseClone();
			xmlSchemaAttribute.refName = this.refName.Clone();
			xmlSchemaAttribute.typeName = this.typeName.Clone();
			xmlSchemaAttribute.qualifiedName = this.qualifiedName.Clone();
			return xmlSchemaAttribute;
		}

		private string defaultValue;

		private string fixedValue;

		private string name;

		private XmlSchemaForm form;

		private XmlSchemaUse use;

		private XmlQualifiedName refName = XmlQualifiedName.Empty;

		private XmlQualifiedName typeName = XmlQualifiedName.Empty;

		private XmlQualifiedName qualifiedName = XmlQualifiedName.Empty;

		private XmlSchemaSimpleType type;

		private XmlSchemaSimpleType attributeType;

		private SchemaAttDef attDef;
	}
}
