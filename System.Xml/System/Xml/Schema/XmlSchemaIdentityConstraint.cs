using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Class for the identity constraints: <see langword="key" />, <see langword="keyref" />, and <see langword="unique" /> elements.</summary>
	public class XmlSchemaIdentityConstraint : XmlSchemaAnnotated
	{
		/// <summary>Gets or sets the name of the identity constraint.</summary>
		/// <returns>The name of the identity constraint.</returns>
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

		/// <summary>Gets or sets the XPath expression <see langword="selector" /> element.</summary>
		/// <returns>The XPath expression <see langword="selector" /> element.</returns>
		[XmlElement("selector", typeof(XmlSchemaXPath))]
		public XmlSchemaXPath Selector
		{
			get
			{
				return this.selector;
			}
			set
			{
				this.selector = value;
			}
		}

		/// <summary>Gets the collection of fields that apply as children for the XML Path Language (XPath) expression selector.</summary>
		/// <returns>The collection of fields.</returns>
		[XmlElement("field", typeof(XmlSchemaXPath))]
		public XmlSchemaObjectCollection Fields
		{
			get
			{
				return this.fields;
			}
		}

		/// <summary>Gets the qualified name of the identity constraint, which holds the post-compilation value of the <see langword="QualifiedName" /> property.</summary>
		/// <returns>The post-compilation value of the <see langword="QualifiedName" /> property.</returns>
		[XmlIgnore]
		public XmlQualifiedName QualifiedName
		{
			get
			{
				return this.qualifiedName;
			}
		}

		internal void SetQualifiedName(XmlQualifiedName value)
		{
			this.qualifiedName = value;
		}

		[XmlIgnore]
		internal CompiledIdentityConstraint CompiledConstraint
		{
			get
			{
				return this.compiledConstraint;
			}
			set
			{
				this.compiledConstraint = value;
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

		private string name;

		private XmlSchemaXPath selector;

		private XmlSchemaObjectCollection fields = new XmlSchemaObjectCollection();

		private XmlQualifiedName qualifiedName = XmlQualifiedName.Empty;

		private CompiledIdentityConstraint compiledConstraint;
	}
}
