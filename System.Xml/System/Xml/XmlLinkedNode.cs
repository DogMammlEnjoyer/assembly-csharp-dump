using System;

namespace System.Xml
{
	/// <summary>Gets the node immediately preceding or following this node.</summary>
	public abstract class XmlLinkedNode : XmlNode
	{
		internal XmlLinkedNode()
		{
			this.next = null;
		}

		internal XmlLinkedNode(XmlDocument doc) : base(doc)
		{
			this.next = null;
		}

		/// <summary>Gets the node immediately preceding this node.</summary>
		/// <returns>The preceding <see cref="T:System.Xml.XmlNode" /> or <see langword="null" /> if one does not exist.</returns>
		public override XmlNode PreviousSibling
		{
			get
			{
				XmlNode parentNode = this.ParentNode;
				if (parentNode != null)
				{
					XmlNode xmlNode;
					XmlNode nextSibling;
					for (xmlNode = parentNode.FirstChild; xmlNode != null; xmlNode = nextSibling)
					{
						nextSibling = xmlNode.NextSibling;
						if (nextSibling == this)
						{
							break;
						}
					}
					return xmlNode;
				}
				return null;
			}
		}

		/// <summary>Gets the node immediately following this node.</summary>
		/// <returns>The <see cref="T:System.Xml.XmlNode" /> immediately following this node or <see langword="null" /> if one does not exist.</returns>
		public override XmlNode NextSibling
		{
			get
			{
				XmlNode parentNode = this.ParentNode;
				if (parentNode != null && this.next != parentNode.FirstChild)
				{
					return this.next;
				}
				return null;
			}
		}

		internal XmlLinkedNode next;
	}
}
