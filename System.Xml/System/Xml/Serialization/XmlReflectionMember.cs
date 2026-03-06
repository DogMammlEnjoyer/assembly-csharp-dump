using System;

namespace System.Xml.Serialization
{
	/// <summary>Provides mappings between code entities in .NET Framework Web service methods and the content of Web Services Description Language (WSDL) messages that are defined for SOAP Web services. </summary>
	public class XmlReflectionMember
	{
		/// <summary>Gets or sets the type of the Web service method member code entity that is represented by this mapping. </summary>
		/// <returns>The <see cref="T:System.Type" /> of the Web service method member code entity that is represented by this mapping.</returns>
		public Type MemberType
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

		/// <summary>Gets or sets an <see cref="T:System.Xml.Serialization.XmlAttributes" /> with the collection of <see cref="T:System.Xml.Serialization.XmlSerializer" />-related attributes that have been applied to the member code entity. </summary>
		/// <returns>An <see cref="T:System.Xml.Serialization.XmlAttributes" /> that represents XML attributes that have been applied to the member code.</returns>
		public XmlAttributes XmlAttributes
		{
			get
			{
				return this.xmlAttributes;
			}
			set
			{
				this.xmlAttributes = value;
			}
		}

		/// <summary>Gets or sets a <see cref="T:System.Xml.Serialization.SoapAttributes" /> with the collection of SOAP-related attributes that have been applied to the member code entity. </summary>
		/// <returns>A <see cref="T:System.Xml.Serialization.SoapAttributes" /> that contains the objects that represent SOAP attributes applied to the member.</returns>
		public SoapAttributes SoapAttributes
		{
			get
			{
				return this.soapAttributes;
			}
			set
			{
				this.soapAttributes = value;
			}
		}

		/// <summary>Gets or sets the name of the Web service method member for this mapping. </summary>
		/// <returns>The name of the Web service method.</returns>
		public string MemberName
		{
			get
			{
				if (this.memberName != null)
				{
					return this.memberName;
				}
				return string.Empty;
			}
			set
			{
				this.memberName = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether the <see cref="T:System.Xml.Serialization.XmlReflectionMember" /> represents a Web service method return value, as opposed to an output parameter. </summary>
		/// <returns>
		///     <see langword="true" />, if the member represents a Web service return value; otherwise, <see langword="false" />.</returns>
		public bool IsReturnValue
		{
			get
			{
				return this.isReturnValue;
			}
			set
			{
				this.isReturnValue = value;
			}
		}

		/// <summary>Gets or sets a value that indicates that the value of the corresponding XML element definition's isNullable attribute is <see langword="false" />.</summary>
		/// <returns>
		///     <see langword="True" /> to override the <see cref="P:System.Xml.Serialization.XmlElementAttribute.IsNullable" /> property; otherwise, <see langword="false" />.</returns>
		public bool OverrideIsNullable
		{
			get
			{
				return this.overrideIsNullable;
			}
			set
			{
				this.overrideIsNullable = value;
			}
		}

		private string memberName;

		private Type type;

		private XmlAttributes xmlAttributes = new XmlAttributes();

		private SoapAttributes soapAttributes = new SoapAttributes();

		private bool isReturnValue;

		private bool overrideIsNullable;
	}
}
