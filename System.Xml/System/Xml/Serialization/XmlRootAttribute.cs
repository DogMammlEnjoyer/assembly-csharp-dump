using System;

namespace System.Xml.Serialization
{
	/// <summary>Controls XML serialization of the attribute target as an XML root element.</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.ReturnValue)]
	public class XmlRootAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.XmlRootAttribute" /> class.</summary>
		public XmlRootAttribute()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.XmlRootAttribute" /> class and specifies the name of the XML root element.</summary>
		/// <param name="elementName">The name of the XML root element. </param>
		public XmlRootAttribute(string elementName)
		{
			this.elementName = elementName;
		}

		/// <summary>Gets or sets the name of the XML element that is generated and recognized by the <see cref="T:System.Xml.Serialization.XmlSerializer" /> class's <see cref="M:System.Xml.Serialization.XmlSerializer.Serialize(System.IO.TextWriter,System.Object)" /> and <see cref="M:System.Xml.Serialization.XmlSerializer.Deserialize(System.IO.Stream)" /> methods, respectively.</summary>
		/// <returns>The name of the XML root element that is generated and recognized in an XML-document instance. The default is the name of the serialized class.</returns>
		public string ElementName
		{
			get
			{
				if (this.elementName != null)
				{
					return this.elementName;
				}
				return string.Empty;
			}
			set
			{
				this.elementName = value;
			}
		}

		/// <summary>Gets or sets the namespace for the XML root element.</summary>
		/// <returns>The namespace for the XML element.</returns>
		public string Namespace
		{
			get
			{
				return this.ns;
			}
			set
			{
				this.ns = value;
			}
		}

		/// <summary>Gets or sets the XSD data type of the XML root element.</summary>
		/// <returns>An XSD (XML Schema Document) data type, as defined by the World Wide Web Consortium (www.w3.org) document named "XML Schema: DataTypes".</returns>
		public string DataType
		{
			get
			{
				if (this.dataType != null)
				{
					return this.dataType;
				}
				return string.Empty;
			}
			set
			{
				this.dataType = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether the <see cref="T:System.Xml.Serialization.XmlSerializer" /> must serialize a member that is set to <see langword="null" /> into the <see langword="xsi:nil" /> attribute set to <see langword="true" />.</summary>
		/// <returns>
		///     <see langword="true" /> if the <see cref="T:System.Xml.Serialization.XmlSerializer" /> generates the <see langword="xsi:nil" /> attribute; otherwise, <see langword="false" />.</returns>
		public bool IsNullable
		{
			get
			{
				return this.nullable;
			}
			set
			{
				this.nullable = value;
				this.nullableSpecified = true;
			}
		}

		internal bool IsNullableSpecified
		{
			get
			{
				return this.nullableSpecified;
			}
		}

		internal string Key
		{
			get
			{
				return string.Concat(new string[]
				{
					(this.ns == null) ? string.Empty : this.ns,
					":",
					this.ElementName,
					":",
					this.nullable.ToString()
				});
			}
		}

		private string elementName;

		private string ns;

		private string dataType;

		private bool nullable = true;

		private bool nullableSpecified;
	}
}
