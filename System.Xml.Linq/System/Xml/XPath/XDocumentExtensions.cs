using System;
using System.Xml.Linq;

namespace System.Xml.XPath
{
	/// <summary>Extends the <see cref="T:System.Xml.Linq.XDocument" /> class by providing a method for navigating and editing an XML node.</summary>
	public static class XDocumentExtensions
	{
		/// <summary>Returns an accessor that allows you to navigate and edit the specified <see cref="T:System.Xml.Linq.XNode" />.</summary>
		/// <param name="node">The XML node to navigate.</param>
		/// <returns>An interface that provides an accessor to the <see cref="T:System.Xml.XPath.XPathNavigator" /> class.</returns>
		public static IXPathNavigable ToXPathNavigable(this XNode node)
		{
			return new XDocumentExtensions.XDocumentNavigable(node);
		}

		private class XDocumentNavigable : IXPathNavigable
		{
			public XDocumentNavigable(XNode n)
			{
				this._node = n;
			}

			public XPathNavigator CreateNavigator()
			{
				return this._node.CreateNavigator();
			}

			private XNode _node;
		}
	}
}
