using System;
using System.ComponentModel;
using System.Reflection;

namespace System.Xml.Serialization
{
	/// <summary>Represents a collection of attribute objects that control how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes and deserializes SOAP methods.</summary>
	public class SoapAttributes
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.SoapAttributes" /> class.</summary>
		public SoapAttributes()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.SoapAttributes" /> class using the specified custom type.</summary>
		/// <param name="provider">Any object that implements the <see cref="T:System.Reflection.ICustomAttributeProvider" /> interface, such as the <see cref="T:System.Type" /> class.</param>
		public SoapAttributes(ICustomAttributeProvider provider)
		{
			object[] customAttributes = provider.GetCustomAttributes(false);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				if (customAttributes[i] is SoapIgnoreAttribute || customAttributes[i] is ObsoleteAttribute)
				{
					this.soapIgnore = true;
					break;
				}
				if (customAttributes[i] is SoapElementAttribute)
				{
					this.soapElement = (SoapElementAttribute)customAttributes[i];
				}
				else if (customAttributes[i] is SoapAttributeAttribute)
				{
					this.soapAttribute = (SoapAttributeAttribute)customAttributes[i];
				}
				else if (customAttributes[i] is SoapTypeAttribute)
				{
					this.soapType = (SoapTypeAttribute)customAttributes[i];
				}
				else if (customAttributes[i] is SoapEnumAttribute)
				{
					this.soapEnum = (SoapEnumAttribute)customAttributes[i];
				}
				else if (customAttributes[i] is DefaultValueAttribute)
				{
					this.soapDefaultValue = ((DefaultValueAttribute)customAttributes[i]).Value;
				}
			}
			if (this.soapIgnore)
			{
				this.soapElement = null;
				this.soapAttribute = null;
				this.soapType = null;
				this.soapEnum = null;
				this.soapDefaultValue = null;
			}
		}

		internal SoapAttributeFlags SoapFlags
		{
			get
			{
				SoapAttributeFlags soapAttributeFlags = (SoapAttributeFlags)0;
				if (this.soapElement != null)
				{
					soapAttributeFlags |= SoapAttributeFlags.Element;
				}
				if (this.soapAttribute != null)
				{
					soapAttributeFlags |= SoapAttributeFlags.Attribute;
				}
				if (this.soapEnum != null)
				{
					soapAttributeFlags |= SoapAttributeFlags.Enum;
				}
				if (this.soapType != null)
				{
					soapAttributeFlags |= SoapAttributeFlags.Type;
				}
				return soapAttributeFlags;
			}
		}

		/// <summary>Gets or sets an object that instructs the <see cref="T:System.Xml.Serialization.XmlSerializer" /> how to serialize an object type into encoded SOAP XML.</summary>
		/// <returns>A <see cref="T:System.Xml.Serialization.SoapTypeAttribute" /> that either overrides a <see cref="T:System.Xml.Serialization.SoapTypeAttribute" /> applied to a class declaration, or is applied to a class declaration.</returns>
		public SoapTypeAttribute SoapType
		{
			get
			{
				return this.soapType;
			}
			set
			{
				this.soapType = value;
			}
		}

		/// <summary>Gets or sets an object that specifies how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes a SOAP enumeration.</summary>
		/// <returns>An object that specifies how the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes an enumeration member.</returns>
		public SoapEnumAttribute SoapEnum
		{
			get
			{
				return this.soapEnum;
			}
			set
			{
				this.soapEnum = value;
			}
		}

		/// <summary>Gets or sets a value that specifies whether the <see cref="T:System.Xml.Serialization.XmlSerializer" /> serializes a public field or property as encoded SOAP XML.</summary>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.Xml.Serialization.XmlSerializer" /> must not serialize the field or property; otherwise, <see langword="false" />.</returns>
		public bool SoapIgnore
		{
			get
			{
				return this.soapIgnore;
			}
			set
			{
				this.soapIgnore = value;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Xml.Serialization.SoapElementAttribute" /> to override.</summary>
		/// <returns>The <see cref="T:System.Xml.Serialization.SoapElementAttribute" /> to override.</returns>
		public SoapElementAttribute SoapElement
		{
			get
			{
				return this.soapElement;
			}
			set
			{
				this.soapElement = value;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Xml.Serialization.SoapAttributeAttribute" /> to override.</summary>
		/// <returns>A <see cref="T:System.Xml.Serialization.SoapAttributeAttribute" /> that overrides the behavior of the <see cref="T:System.Xml.Serialization.XmlSerializer" /> when the member is serialized.</returns>
		public SoapAttributeAttribute SoapAttribute
		{
			get
			{
				return this.soapAttribute;
			}
			set
			{
				this.soapAttribute = value;
			}
		}

		/// <summary>Gets or sets the default value of an XML element or attribute.</summary>
		/// <returns>An object that represents the default value of an XML element or attribute.</returns>
		public object SoapDefaultValue
		{
			get
			{
				return this.soapDefaultValue;
			}
			set
			{
				this.soapDefaultValue = value;
			}
		}

		private bool soapIgnore;

		private SoapTypeAttribute soapType;

		private SoapElementAttribute soapElement;

		private SoapAttributeAttribute soapAttribute;

		private SoapEnumAttribute soapEnum;

		private object soapDefaultValue;
	}
}
