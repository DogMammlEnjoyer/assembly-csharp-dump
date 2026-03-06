using System;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="minExclusive" /> element from XML Schema as specified by the World Wide Web Consortium (W3C). This class can be used to specify a restriction on the minimum value of a <see langword="simpleType" /> element. The element value must be greater than the value of the <see langword="minExclusive" /> element.</summary>
	public class XmlSchemaMinExclusiveFacet : XmlSchemaFacet
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Schema.XmlSchemaMinExclusiveFacet" /> class.</summary>
		public XmlSchemaMinExclusiveFacet()
		{
			base.FacetType = FacetType.MinExclusive;
		}
	}
}
