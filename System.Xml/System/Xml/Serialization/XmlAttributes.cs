using System;
using System.ComponentModel;
using System.Reflection;

namespace System.Xml.Serialization
{
	/// <summary>Represents a collection of attribute objects that control how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes and deserializes an object.</summary>
	public class XmlAttributes
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.XmlAttributes" /> class.</summary>
		public XmlAttributes()
		{
		}

		internal XmlAttributeFlags XmlFlags
		{
			get
			{
				XmlAttributeFlags xmlAttributeFlags = (XmlAttributeFlags)0;
				if (this.xmlElements.Count > 0)
				{
					xmlAttributeFlags |= XmlAttributeFlags.Elements;
				}
				if (this.xmlArrayItems.Count > 0)
				{
					xmlAttributeFlags |= XmlAttributeFlags.ArrayItems;
				}
				if (this.xmlAnyElements.Count > 0)
				{
					xmlAttributeFlags |= XmlAttributeFlags.AnyElements;
				}
				if (this.xmlArray != null)
				{
					xmlAttributeFlags |= XmlAttributeFlags.Array;
				}
				if (this.xmlAttribute != null)
				{
					xmlAttributeFlags |= XmlAttributeFlags.Attribute;
				}
				if (this.xmlText != null)
				{
					xmlAttributeFlags |= XmlAttributeFlags.Text;
				}
				if (this.xmlEnum != null)
				{
					xmlAttributeFlags |= XmlAttributeFlags.Enum;
				}
				if (this.xmlRoot != null)
				{
					xmlAttributeFlags |= XmlAttributeFlags.Root;
				}
				if (this.xmlType != null)
				{
					xmlAttributeFlags |= XmlAttributeFlags.Type;
				}
				if (this.xmlAnyAttribute != null)
				{
					xmlAttributeFlags |= XmlAttributeFlags.AnyAttribute;
				}
				if (this.xmlChoiceIdentifier != null)
				{
					xmlAttributeFlags |= XmlAttributeFlags.ChoiceIdentifier;
				}
				if (this.xmlns)
				{
					xmlAttributeFlags |= XmlAttributeFlags.XmlnsDeclarations;
				}
				return xmlAttributeFlags;
			}
		}

		private static Type IgnoreAttribute
		{
			get
			{
				if (XmlAttributes.ignoreAttributeType == null)
				{
					XmlAttributes.ignoreAttributeType = typeof(object).Assembly.GetType("System.XmlIgnoreMemberAttribute");
					if (XmlAttributes.ignoreAttributeType == null)
					{
						XmlAttributes.ignoreAttributeType = typeof(XmlIgnoreAttribute);
					}
				}
				return XmlAttributes.ignoreAttributeType;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.XmlAttributes" /> class and customizes how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes and deserializes an object. </summary>
		/// <param name="provider">A class that can provide alternative implementations of attributes that control XML serialization.</param>
		public XmlAttributes(ICustomAttributeProvider provider)
		{
			object[] customAttributes = provider.GetCustomAttributes(false);
			XmlAnyElementAttribute xmlAnyElementAttribute = null;
			for (int i = 0; i < customAttributes.Length; i++)
			{
				if (customAttributes[i] is XmlIgnoreAttribute || customAttributes[i] is ObsoleteAttribute || customAttributes[i].GetType() == XmlAttributes.IgnoreAttribute)
				{
					this.xmlIgnore = true;
					break;
				}
				if (customAttributes[i] is XmlElementAttribute)
				{
					this.xmlElements.Add((XmlElementAttribute)customAttributes[i]);
				}
				else if (customAttributes[i] is XmlArrayItemAttribute)
				{
					this.xmlArrayItems.Add((XmlArrayItemAttribute)customAttributes[i]);
				}
				else if (customAttributes[i] is XmlAnyElementAttribute)
				{
					XmlAnyElementAttribute xmlAnyElementAttribute2 = (XmlAnyElementAttribute)customAttributes[i];
					if ((xmlAnyElementAttribute2.Name == null || xmlAnyElementAttribute2.Name.Length == 0) && xmlAnyElementAttribute2.NamespaceSpecified && xmlAnyElementAttribute2.Namespace == null)
					{
						xmlAnyElementAttribute = xmlAnyElementAttribute2;
					}
					else
					{
						this.xmlAnyElements.Add((XmlAnyElementAttribute)customAttributes[i]);
					}
				}
				else if (customAttributes[i] is DefaultValueAttribute)
				{
					this.xmlDefaultValue = ((DefaultValueAttribute)customAttributes[i]).Value;
				}
				else if (customAttributes[i] is XmlAttributeAttribute)
				{
					this.xmlAttribute = (XmlAttributeAttribute)customAttributes[i];
				}
				else if (customAttributes[i] is XmlArrayAttribute)
				{
					this.xmlArray = (XmlArrayAttribute)customAttributes[i];
				}
				else if (customAttributes[i] is XmlTextAttribute)
				{
					this.xmlText = (XmlTextAttribute)customAttributes[i];
				}
				else if (customAttributes[i] is XmlEnumAttribute)
				{
					this.xmlEnum = (XmlEnumAttribute)customAttributes[i];
				}
				else if (customAttributes[i] is XmlRootAttribute)
				{
					this.xmlRoot = (XmlRootAttribute)customAttributes[i];
				}
				else if (customAttributes[i] is XmlTypeAttribute)
				{
					this.xmlType = (XmlTypeAttribute)customAttributes[i];
				}
				else if (customAttributes[i] is XmlAnyAttributeAttribute)
				{
					this.xmlAnyAttribute = (XmlAnyAttributeAttribute)customAttributes[i];
				}
				else if (customAttributes[i] is XmlChoiceIdentifierAttribute)
				{
					this.xmlChoiceIdentifier = (XmlChoiceIdentifierAttribute)customAttributes[i];
				}
				else if (customAttributes[i] is XmlNamespaceDeclarationsAttribute)
				{
					this.xmlns = true;
				}
			}
			if (this.xmlIgnore)
			{
				this.xmlElements.Clear();
				this.xmlArrayItems.Clear();
				this.xmlAnyElements.Clear();
				this.xmlDefaultValue = null;
				this.xmlAttribute = null;
				this.xmlArray = null;
				this.xmlText = null;
				this.xmlEnum = null;
				this.xmlType = null;
				this.xmlAnyAttribute = null;
				this.xmlChoiceIdentifier = null;
				this.xmlns = false;
				return;
			}
			if (xmlAnyElementAttribute != null)
			{
				this.xmlAnyElements.Add(xmlAnyElementAttribute);
			}
		}

		internal static object GetAttr(ICustomAttributeProvider provider, Type attrType)
		{
			object[] customAttributes = provider.GetCustomAttributes(attrType, false);
			if (customAttributes.Length == 0)
			{
				return null;
			}
			return customAttributes[0];
		}

		/// <summary>Gets a collection of objects that specify how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes a public field or read/write property as an XML element.</summary>
		/// <returns>An <see cref="T:System.Xml.Serialization.XmlElementAttributes" /> that contains a collection of <see cref="T:System.Xml.Serialization.XmlElementAttribute" /> objects.</returns>
		public XmlElementAttributes XmlElements
		{
			get
			{
				return this.xmlElements;
			}
		}

		/// <summary>Gets or sets an object that specifies how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes a public field or public read/write property as an XML attribute.</summary>
		/// <returns>An <see cref="T:System.Xml.Serialization.XmlAttributeAttribute" /> that controls the serialization of a public field or read/write property as an XML attribute.</returns>
		public XmlAttributeAttribute XmlAttribute
		{
			get
			{
				return this.xmlAttribute;
			}
			set
			{
				this.xmlAttribute = value;
			}
		}

		/// <summary>Gets or sets an object that specifies how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes an enumeration member.</summary>
		/// <returns>An <see cref="T:System.Xml.Serialization.XmlEnumAttribute" /> that specifies how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes an enumeration member.</returns>
		public XmlEnumAttribute XmlEnum
		{
			get
			{
				return this.xmlEnum;
			}
			set
			{
				this.xmlEnum = value;
			}
		}

		/// <summary>Gets or sets an object that instructs the <see cref="T:System.Xml.Serialization.XmlSerializer" /> to serialize a public field or public read/write property as XML text.</summary>
		/// <returns>An <see cref="T:System.Xml.Serialization.XmlTextAttribute" /> that overrides the default serialization of a public property or field.</returns>
		public XmlTextAttribute XmlText
		{
			get
			{
				return this.xmlText;
			}
			set
			{
				this.xmlText = value;
			}
		}

		/// <summary>Gets or sets an object that specifies how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes a public field or read/write property that returns an array.</summary>
		/// <returns>An <see cref="T:System.Xml.Serialization.XmlArrayAttribute" /> that specifies how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes a public field or read/write property that returns an array.</returns>
		public XmlArrayAttribute XmlArray
		{
			get
			{
				return this.xmlArray;
			}
			set
			{
				this.xmlArray = value;
			}
		}

		/// <summary>Gets or sets a collection of objects that specify how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes items inserted into an array returned by a public field or read/write property.</summary>
		/// <returns>An <see cref="T:System.Xml.Serialization.XmlArrayItemAttributes" /> object that contains a collection of <see cref="T:System.Xml.Serialization.XmlArrayItemAttribute" /> objects.</returns>
		public XmlArrayItemAttributes XmlArrayItems
		{
			get
			{
				return this.xmlArrayItems;
			}
		}

		/// <summary>Gets or sets the default value of an XML element or attribute.</summary>
		/// <returns>An <see cref="T:System.Object" /> that represents the default value of an XML element or attribute.</returns>
		public object XmlDefaultValue
		{
			get
			{
				return this.xmlDefaultValue;
			}
			set
			{
				this.xmlDefaultValue = value;
			}
		}

		/// <summary>Gets or sets a value that specifies whether or not the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes a public field or public read/write property.</summary>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.Xml.Serialization.XmlSerializer" /> must not serialize the field or property; otherwise, <see langword="false" />.</returns>
		public bool XmlIgnore
		{
			get
			{
				return this.xmlIgnore;
			}
			set
			{
				this.xmlIgnore = value;
			}
		}

		/// <summary>Gets or sets an object that specifies how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes a class to which the <see cref="T:System.Xml.Serialization.XmlTypeAttribute" /> has been applied.</summary>
		/// <returns>An <see cref="T:System.Xml.Serialization.XmlTypeAttribute" /> that overrides an <see cref="T:System.Xml.Serialization.XmlTypeAttribute" /> applied to a class declaration.</returns>
		public XmlTypeAttribute XmlType
		{
			get
			{
				return this.xmlType;
			}
			set
			{
				this.xmlType = value;
			}
		}

		/// <summary>Gets or sets an object that specifies how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes a class as an XML root element.</summary>
		/// <returns>An <see cref="T:System.Xml.Serialization.XmlRootAttribute" /> that overrides a class attributed as an XML root element.</returns>
		public XmlRootAttribute XmlRoot
		{
			get
			{
				return this.xmlRoot;
			}
			set
			{
				this.xmlRoot = value;
			}
		}

		/// <summary>Gets the collection of <see cref="T:System.Xml.Serialization.XmlAnyElementAttribute" /> objects to override.</summary>
		/// <returns>An <see cref="T:System.Xml.Serialization.XmlAnyElementAttributes" /> object that represents the collection of <see cref="T:System.Xml.Serialization.XmlAnyElementAttribute" /> objects.</returns>
		public XmlAnyElementAttributes XmlAnyElements
		{
			get
			{
				return this.xmlAnyElements;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Xml.Serialization.XmlAnyAttributeAttribute" /> to override.</summary>
		/// <returns>The <see cref="T:System.Xml.Serialization.XmlAnyAttributeAttribute" /> to override.</returns>
		public XmlAnyAttributeAttribute XmlAnyAttribute
		{
			get
			{
				return this.xmlAnyAttribute;
			}
			set
			{
				this.xmlAnyAttribute = value;
			}
		}

		/// <summary>Gets or sets an object that allows you to distinguish between a set of choices.</summary>
		/// <returns>An <see cref="T:System.Xml.Serialization.XmlChoiceIdentifierAttribute" /> that can be applied to a class member that is serialized as an <see langword="xsi:choice" /> element.</returns>
		public XmlChoiceIdentifierAttribute XmlChoiceIdentifier
		{
			get
			{
				return this.xmlChoiceIdentifier;
			}
		}

		/// <summary>Gets or sets a value that specifies whether to keep all namespace declarations when an object containing a member that returns an <see cref="T:System.Xml.Serialization.XmlSerializerNamespaces" /> object is overridden.</summary>
		/// <returns>
		///     <see langword="true" /> if the namespace declarations should be kept; otherwise, <see langword="false" />.</returns>
		public bool Xmlns
		{
			get
			{
				return this.xmlns;
			}
			set
			{
				this.xmlns = value;
			}
		}

		private XmlElementAttributes xmlElements = new XmlElementAttributes();

		private XmlArrayItemAttributes xmlArrayItems = new XmlArrayItemAttributes();

		private XmlAnyElementAttributes xmlAnyElements = new XmlAnyElementAttributes();

		private XmlArrayAttribute xmlArray;

		private XmlAttributeAttribute xmlAttribute;

		private XmlTextAttribute xmlText;

		private XmlEnumAttribute xmlEnum;

		private bool xmlIgnore;

		private bool xmlns;

		private object xmlDefaultValue;

		private XmlRootAttribute xmlRoot;

		private XmlTypeAttribute xmlType;

		private XmlAnyAttributeAttribute xmlAnyAttribute;

		private XmlChoiceIdentifierAttribute xmlChoiceIdentifier;

		private static volatile Type ignoreAttributeType;
	}
}
