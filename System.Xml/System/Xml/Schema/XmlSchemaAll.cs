using System;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	/// <summary>Represents the World Wide Web Consortium (W3C) <see langword="all" /> element (compositor).</summary>
	public class XmlSchemaAll : XmlSchemaGroupBase
	{
		/// <summary>Gets the collection of <see langword="XmlSchemaElement" /> elements contained within the <see langword="all" /> compositor.</summary>
		/// <returns>The collection of elements contained in <see langword="XmlSchemaAll" />.</returns>
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
