using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="group" /> element from XML Schema as specified by the World Wide Web Consortium (W3C). This class defines groups at the <see langword="schema" /> level that are referenced from the complex types. It groups a set of element declarations so that they can be incorporated as a group into complex type definitions.</summary>
	public class XmlSchemaGroup : XmlSchemaAnnotated
	{
		/// <summary>Gets or sets the name of the schema group.</summary>
		/// <returns>The name of the schema group.</returns>
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

		/// <summary>Gets or sets one of the <see cref="T:System.Xml.Schema.XmlSchemaChoice" />, <see cref="T:System.Xml.Schema.XmlSchemaAll" />, or <see cref="T:System.Xml.Schema.XmlSchemaSequence" /> classes.</summary>
		/// <returns>One of the <see cref="T:System.Xml.Schema.XmlSchemaChoice" />, <see cref="T:System.Xml.Schema.XmlSchemaAll" />, or <see cref="T:System.Xml.Schema.XmlSchemaSequence" /> classes.</returns>
		[XmlElement("choice", typeof(XmlSchemaChoice))]
		[XmlElement("sequence", typeof(XmlSchemaSequence))]
		[XmlElement("all", typeof(XmlSchemaAll))]
		public XmlSchemaGroupBase Particle
		{
			get
			{
				return this.particle;
			}
			set
			{
				this.particle = value;
			}
		}

		/// <summary>Gets the qualified name of the schema group.</summary>
		/// <returns>An <see cref="T:System.Xml.XmlQualifiedName" /> object representing the qualified name of the schema group.</returns>
		[XmlIgnore]
		public XmlQualifiedName QualifiedName
		{
			get
			{
				return this.qname;
			}
		}

		[XmlIgnore]
		internal XmlSchemaParticle CanonicalParticle
		{
			get
			{
				return this.canonicalParticle;
			}
			set
			{
				this.canonicalParticle = value;
			}
		}

		[XmlIgnore]
		internal XmlSchemaGroup Redefined
		{
			get
			{
				return this.redefined;
			}
			set
			{
				this.redefined = value;
			}
		}

		[XmlIgnore]
		internal int SelfReferenceCount
		{
			get
			{
				return this.selfReferenceCount;
			}
			set
			{
				this.selfReferenceCount = value;
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

		internal void SetQualifiedName(XmlQualifiedName value)
		{
			this.qname = value;
		}

		internal override XmlSchemaObject Clone()
		{
			return this.Clone(null);
		}

		internal XmlSchemaObject Clone(XmlSchema parentSchema)
		{
			XmlSchemaGroup xmlSchemaGroup = (XmlSchemaGroup)base.MemberwiseClone();
			if (XmlSchemaComplexType.HasParticleRef(this.particle, parentSchema))
			{
				xmlSchemaGroup.particle = (XmlSchemaComplexType.CloneParticle(this.particle, parentSchema) as XmlSchemaGroupBase);
			}
			xmlSchemaGroup.canonicalParticle = XmlSchemaParticle.Empty;
			return xmlSchemaGroup;
		}

		private string name;

		private XmlSchemaGroupBase particle;

		private XmlSchemaParticle canonicalParticle;

		private XmlQualifiedName qname = XmlQualifiedName.Empty;

		private XmlSchemaGroup redefined;

		private int selfReferenceCount;
	}
}
