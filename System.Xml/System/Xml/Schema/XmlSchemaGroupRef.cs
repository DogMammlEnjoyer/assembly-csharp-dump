using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="group" /> element with <see langword="ref" /> attribute from the XML Schema as specified by the World Wide Web Consortium (W3C). This class is used within complex types that reference a <see langword="group" /> defined at the <see langword="schema" /> level.</summary>
	public class XmlSchemaGroupRef : XmlSchemaParticle
	{
		/// <summary>Gets or sets the name of a group defined in this schema (or another schema indicated by the specified namespace).</summary>
		/// <returns>The name of a group defined in this schema.</returns>
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

		/// <summary>Gets one of the <see cref="T:System.Xml.Schema.XmlSchemaChoice" />, <see cref="T:System.Xml.Schema.XmlSchemaAll" />, or <see cref="T:System.Xml.Schema.XmlSchemaSequence" /> classes, which holds the post-compilation value of the <see langword="Particle" /> property.</summary>
		/// <returns>The post-compilation value of the <see langword="Particle" /> property, which is one of the <see cref="T:System.Xml.Schema.XmlSchemaChoice" />, <see cref="T:System.Xml.Schema.XmlSchemaAll" />, or <see cref="T:System.Xml.Schema.XmlSchemaSequence" /> classes.</returns>
		[XmlIgnore]
		public XmlSchemaGroupBase Particle
		{
			get
			{
				return this.particle;
			}
		}

		internal void SetParticle(XmlSchemaGroupBase value)
		{
			this.particle = value;
		}

		[XmlIgnore]
		internal XmlSchemaGroup Redefined
		{
			get
			{
				return this.refined;
			}
			set
			{
				this.refined = value;
			}
		}

		private XmlQualifiedName refName = XmlQualifiedName.Empty;

		private XmlSchemaGroupBase particle;

		private XmlSchemaGroup refined;
	}
}
