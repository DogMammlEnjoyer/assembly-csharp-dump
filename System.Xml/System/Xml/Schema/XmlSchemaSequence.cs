using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the <see langword="sequence" /> element (compositor) from the XML Schema as specified by the World Wide Web Consortium (W3C). The <see langword="sequence" /> requires the elements in the group to appear in the specified sequence within the containing element.</summary>
	public class XmlSchemaSequence : XmlSchemaGroupBase
	{
		/// <summary>The elements contained within the compositor. Collection of <see cref="T:System.Xml.Schema.XmlSchemaElement" />, <see cref="T:System.Xml.Schema.XmlSchemaGroupRef" />, <see cref="T:System.Xml.Schema.XmlSchemaChoice" />, <see cref="T:System.Xml.Schema.XmlSchemaSequence" />, or <see cref="T:System.Xml.Schema.XmlSchemaAny" />.</summary>
		/// <returns>The elements contained within the compositor.</returns>
		[XmlElement("any", typeof(XmlSchemaAny))]
		[XmlElement("sequence", typeof(XmlSchemaSequence))]
		[XmlElement("choice", typeof(XmlSchemaChoice))]
		[XmlElement("group", typeof(XmlSchemaGroupRef))]
		[XmlElement("element", typeof(XmlSchemaElement))]
		public override XmlSchemaObjectCollection Items
		{
			get
			{
				return this.items;
			}
		}

		internal override bool IsEmpty
		{
			get
			{
				return base.IsEmpty || this.items.Count == 0;
			}
		}

		internal override void SetItems(XmlSchemaObjectCollection newItems)
		{
			this.items = newItems;
		}

		private XmlSchemaObjectCollection items = new XmlSchemaObjectCollection();
	}
}
