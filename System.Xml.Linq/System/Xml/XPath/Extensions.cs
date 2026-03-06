using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace System.Xml.XPath
{
	/// <summary>This class contains the LINQ to XML extension methods that enable you to evaluate XPath expressions.</summary>
	public static class Extensions
	{
		/// <summary>Creates an <see cref="T:System.Xml.XPath.XPathNavigator" /> for an <see cref="T:System.Xml.Linq.XNode" />.</summary>
		/// <param name="node">An <see cref="T:System.Xml.Linq.XNode" /> that can process XPath queries.</param>
		/// <returns>An <see cref="T:System.Xml.XPath.XPathNavigator" /> that can process XPath queries.</returns>
		public static XPathNavigator CreateNavigator(this XNode node)
		{
			return node.CreateNavigator(null);
		}

		/// <summary>Creates an <see cref="T:System.Xml.XPath.XPathNavigator" /> for an <see cref="T:System.Xml.Linq.XNode" />. The <see cref="T:System.Xml.XmlNameTable" /> enables more efficient XPath expression processing.</summary>
		/// <param name="node">An <see cref="T:System.Xml.Linq.XNode" /> that can process an XPath query.</param>
		/// <param name="nameTable">A <see cref="T:System.Xml.XmlNameTable" /> to be used by <see cref="T:System.Xml.XPath.XPathNavigator" />.</param>
		/// <returns>An <see cref="T:System.Xml.XPath.XPathNavigator" /> that can process XPath queries.</returns>
		public static XPathNavigator CreateNavigator(this XNode node, XmlNameTable nameTable)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node is XDocumentType)
			{
				throw new ArgumentException(SR.Format("This XPathNavigator cannot be created on a node of type {0}.", XmlNodeType.DocumentType));
			}
			XText xtext = node as XText;
			if (xtext != null)
			{
				if (xtext.GetParent() is XDocument)
				{
					throw new ArgumentException(SR.Format("This XPathNavigator cannot be created on a node of type {0}.", XmlNodeType.Whitespace));
				}
				node = Extensions.CalibrateText(xtext);
			}
			return new XNodeNavigator(node, nameTable);
		}

		/// <summary>Evaluates an XPath expression.</summary>
		/// <param name="node">The <see cref="T:System.Xml.Linq.XNode" /> on which to evaluate the XPath expression.</param>
		/// <param name="expression">A <see cref="T:System.String" /> that contains an XPath expression.</param>
		/// <returns>An object that can contain a <see langword="bool" />, a <see langword="double" />, a <see langword="string" />, or an <see cref="T:System.Collections.Generic.IEnumerable`1" />.</returns>
		public static object XPathEvaluate(this XNode node, string expression)
		{
			return node.XPathEvaluate(expression, null);
		}

		/// <summary>Evaluates an XPath expression, resolving namespace prefixes using the specified <see cref="T:System.Xml.IXmlNamespaceResolver" />.</summary>
		/// <param name="node">The <see cref="T:System.Xml.Linq.XNode" /> on which to evaluate the XPath expression.</param>
		/// <param name="expression">A <see cref="T:System.String" /> that contains an XPath expression.</param>
		/// <param name="resolver">A <see cref="T:System.Xml.IXmlNamespaceResolver" /> for the namespace prefixes in the XPath expression.</param>
		/// <returns>An object that contains the result of evaluating the expression. The object can be a <see langword="bool" />, a <see langword="double" />, a <see langword="string" />, or an <see cref="T:System.Collections.Generic.IEnumerable`1" />.</returns>
		public static object XPathEvaluate(this XNode node, string expression, IXmlNamespaceResolver resolver)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			return default(XPathEvaluator).Evaluate<object>(node, expression, resolver);
		}

		/// <summary>Selects an <see cref="T:System.Xml.Linq.XElement" /> using a XPath expression.</summary>
		/// <param name="node">The <see cref="T:System.Xml.Linq.XNode" /> on which to evaluate the XPath expression.</param>
		/// <param name="expression">A <see cref="T:System.String" /> that contains an XPath expression.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XElement" />, or null.</returns>
		public static XElement XPathSelectElement(this XNode node, string expression)
		{
			return node.XPathSelectElement(expression, null);
		}

		/// <summary>Selects an <see cref="T:System.Xml.Linq.XElement" /> using a XPath expression, resolving namespace prefixes using the specified <see cref="T:System.Xml.IXmlNamespaceResolver" />.</summary>
		/// <param name="node">The <see cref="T:System.Xml.Linq.XNode" /> on which to evaluate the XPath expression.</param>
		/// <param name="expression">A <see cref="T:System.String" /> that contains an XPath expression.</param>
		/// <param name="resolver">An <see cref="T:System.Xml.IXmlNamespaceResolver" /> for the namespace prefixes in the XPath expression.</param>
		/// <returns>An <see cref="T:System.Xml.Linq.XElement" />, or null.</returns>
		public static XElement XPathSelectElement(this XNode node, string expression, IXmlNamespaceResolver resolver)
		{
			return node.XPathSelectElements(expression, resolver).FirstOrDefault<XElement>();
		}

		/// <summary>Selects a collection of elements using an XPath expression.</summary>
		/// <param name="node">The <see cref="T:System.Xml.Linq.XNode" /> on which to evaluate the XPath expression.</param>
		/// <param name="expression">A <see cref="T:System.String" /> that contains an XPath expression.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the selected elements.</returns>
		public static IEnumerable<XElement> XPathSelectElements(this XNode node, string expression)
		{
			return node.XPathSelectElements(expression, null);
		}

		/// <summary>Selects a collection of elements using an XPath expression, resolving namespace prefixes using the specified <see cref="T:System.Xml.IXmlNamespaceResolver" />.</summary>
		/// <param name="node">The <see cref="T:System.Xml.Linq.XNode" /> on which to evaluate the XPath expression.</param>
		/// <param name="expression">A <see cref="T:System.String" /> that contains an XPath expression.</param>
		/// <param name="resolver">A <see cref="T:System.Xml.IXmlNamespaceResolver" /> for the namespace prefixes in the XPath expression.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the selected elements.</returns>
		public static IEnumerable<XElement> XPathSelectElements(this XNode node, string expression, IXmlNamespaceResolver resolver)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			return (IEnumerable<XElement>)default(XPathEvaluator).Evaluate<XElement>(node, expression, resolver);
		}

		private static XText CalibrateText(XText n)
		{
			XContainer parent = n.GetParent();
			if (parent == null)
			{
				return n;
			}
			foreach (XNode xnode in parent.Nodes())
			{
				XText xtext = xnode as XText;
				if (xtext != null && xnode == n)
				{
					return xtext;
				}
			}
			return null;
		}
	}
}
