using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Abstract class for all facets that are used when simple types are derived by restriction.</summary>
	public abstract class XmlSchemaFacet : XmlSchemaAnnotated
	{
		/// <summary>Gets or sets the <see langword="value" /> attribute of the facet.</summary>
		/// <returns>The value attribute.</returns>
		[XmlAttribute("value")]
		public string Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		/// <summary>Gets or sets information that indicates that this facet is fixed.</summary>
		/// <returns>If <see langword="true" />, value is fixed; otherwise, <see langword="false" />. The default is <see langword="false" />.Optional.</returns>
		[XmlAttribute("fixed")]
		[DefaultValue(false)]
		public virtual bool IsFixed
		{
			get
			{
				return this.isFixed;
			}
			set
			{
				if (!(this is XmlSchemaEnumerationFacet) && !(this is XmlSchemaPatternFacet))
				{
					this.isFixed = value;
				}
			}
		}

		internal FacetType FacetType
		{
			get
			{
				return this.facetType;
			}
			set
			{
				this.facetType = value;
			}
		}

		private string value;

		private bool isFixed;

		private FacetType facetType;
	}
}
