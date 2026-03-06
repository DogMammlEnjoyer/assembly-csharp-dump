using System;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="pattern" /> element from XML Schema as specified by the World Wide Web Consortium (W3C). This class can be used to specify a restriction on the value entered for a <see langword="simpleType" /> element.</summary>
	public class XmlSchemaPatternFacet : XmlSchemaFacet
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Schema.XmlSchemaPatternFacet" /> class.</summary>
		public XmlSchemaPatternFacet()
		{
			base.FacetType = FacetType.Pattern;
		}
	}
}
