using System;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	/// <summary>Contains the LINQ to XML extension methods.</summary>
	public static class Extensions
	{
		/// <summary>Returns a collection of the attributes of every element in the source collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the source collection.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XAttribute" /> that contains the attributes of every element in the source collection.</returns>
		public static IEnumerable<XAttribute> Attributes(this IEnumerable<XElement> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetAttributes(source, null);
		}

		/// <summary>Returns a filtered collection of the attributes of every element in the source collection. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the source collection.</param>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> to match.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XAttribute" /> that contains a filtered collection of the attributes of every element in the source collection. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</returns>
		public static IEnumerable<XAttribute> Attributes(this IEnumerable<XElement> source, XName name)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(name != null))
			{
				return XAttribute.EmptySequence;
			}
			return Extensions.GetAttributes(source, name);
		}

		/// <summary>Returns a collection of elements that contains the ancestors of every node in the source collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XNode" /> that contains the source collection.</param>
		/// <typeparam name="T">The type of the objects in <paramref name="source" />, constrained to <see cref="T:System.Xml.Linq.XNode" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the ancestors of every node in the source collection.</returns>
		public static IEnumerable<XElement> Ancestors<T>(this IEnumerable<T> source) where T : XNode
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetAncestors<T>(source, null, false);
		}

		/// <summary>Returns a filtered collection of elements that contains the ancestors of every node in the source collection. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XNode" /> that contains the source collection.</param>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> to match.</param>
		/// <typeparam name="T">The type of the objects in <paramref name="source" />, constrained to <see cref="T:System.Xml.Linq.XNode" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the ancestors of every node in the source collection. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</returns>
		public static IEnumerable<XElement> Ancestors<T>(this IEnumerable<T> source, XName name) where T : XNode
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return Extensions.GetAncestors<T>(source, name, false);
		}

		/// <summary>Returns a collection of elements that contains every element in the source collection, and the ancestors of every element in the source collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the source collection.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains every element in the source collection, and the ancestors of every element in the source collection.</returns>
		public static IEnumerable<XElement> AncestorsAndSelf(this IEnumerable<XElement> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetAncestors<XElement>(source, null, true);
		}

		/// <summary>Returns a filtered collection of elements that contains every element in the source collection, and the ancestors of every element in the source collection. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the source collection.</param>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> to match.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains every element in the source collection, and the ancestors of every element in the source collection. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</returns>
		public static IEnumerable<XElement> AncestorsAndSelf(this IEnumerable<XElement> source, XName name)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return Extensions.GetAncestors<XElement>(source, name, true);
		}

		/// <summary>Returns a collection of the child nodes of every document and element in the source collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XNode" /> that contains the source collection.</param>
		/// <typeparam name="T">The type of the objects in <paramref name="source" />, constrained to <see cref="T:System.Xml.Linq.XContainer" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XNode" /> of the child nodes of every document and element in the source collection.</returns>
		public static IEnumerable<XNode> Nodes<T>(this IEnumerable<T> source) where T : XContainer
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.NodesIterator<T>(source);
		}

		private static IEnumerable<XNode> NodesIterator<T>(IEnumerable<T> source) where T : XContainer
		{
			foreach (T t in source)
			{
				XContainer root = t;
				if (root != null)
				{
					XNode i = root.LastNode;
					if (i != null)
					{
						do
						{
							i = i.next;
							yield return i;
						}
						while (i.parent == root && i != root.content);
					}
					i = null;
				}
				root = null;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}

		/// <summary>Returns a collection of the descendant nodes of every document and element in the source collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XContainer" /> that contains the source collection.</param>
		/// <typeparam name="T">The type of the objects in <paramref name="source" />, constrained to <see cref="T:System.Xml.Linq.XContainer" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XNode" /> of the descendant nodes of every document and element in the source collection.</returns>
		public static IEnumerable<XNode> DescendantNodes<T>(this IEnumerable<T> source) where T : XContainer
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetDescendantNodes<T>(source, false);
		}

		/// <summary>Returns a collection of elements that contains the descendant elements of every element and document in the source collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XContainer" /> that contains the source collection.</param>
		/// <typeparam name="T">The type of the objects in <paramref name="source" />, constrained to <see cref="T:System.Xml.Linq.XContainer" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the descendant elements of every element and document in the source collection.</returns>
		public static IEnumerable<XElement> Descendants<T>(this IEnumerable<T> source) where T : XContainer
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetDescendants<T>(source, null, false);
		}

		/// <summary>Returns a filtered collection of elements that contains the descendant elements of every element and document in the source collection. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XContainer" /> that contains the source collection.</param>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> to match.</param>
		/// <typeparam name="T">The type of the objects in <paramref name="source" />, constrained to <see cref="T:System.Xml.Linq.XContainer" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the descendant elements of every element and document in the source collection. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</returns>
		public static IEnumerable<XElement> Descendants<T>(this IEnumerable<T> source, XName name) where T : XContainer
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return Extensions.GetDescendants<T>(source, name, false);
		}

		/// <summary>Returns a collection of nodes that contains every element in the source collection, and the descendant nodes of every element in the source collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the source collection.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XNode" /> that contains every element in the source collection, and the descendant nodes of every element in the source collection.</returns>
		public static IEnumerable<XNode> DescendantNodesAndSelf(this IEnumerable<XElement> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetDescendantNodes<XElement>(source, true);
		}

		/// <summary>Returns a collection of elements that contains every element in the source collection, and the descendent elements of every element in the source collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the source collection.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains every element in the source collection, and the descendent elements of every element in the source collection.</returns>
		public static IEnumerable<XElement> DescendantsAndSelf(this IEnumerable<XElement> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetDescendants<XElement>(source, null, true);
		}

		/// <summary>Returns a filtered collection of elements that contains every element in the source collection, and the descendents of every element in the source collection. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the source collection.</param>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> to match.</param>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains every element in the source collection, and the descendents of every element in the source collection. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</returns>
		public static IEnumerable<XElement> DescendantsAndSelf(this IEnumerable<XElement> source, XName name)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return Extensions.GetDescendants<XElement>(source, name, true);
		}

		/// <summary>Returns a collection of the child elements of every element and document in the source collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the source collection.</param>
		/// <typeparam name="T">The type of the objects in <paramref name="source" />, constrained to <see cref="T:System.Xml.Linq.XContainer" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> of the child elements of every element or document in the source collection.</returns>
		public static IEnumerable<XElement> Elements<T>(this IEnumerable<T> source) where T : XContainer
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.GetElements<T>(source, null);
		}

		/// <summary>Returns a filtered collection of the child elements of every element and document in the source collection. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> that contains the source collection.</param>
		/// <param name="name">The <see cref="T:System.Xml.Linq.XName" /> to match.</param>
		/// <typeparam name="T">The type of the objects in <paramref name="source" />, constrained to <see cref="T:System.Xml.Linq.XContainer" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XElement" /> of the child elements of every element and document in the source collection. Only elements that have a matching <see cref="T:System.Xml.Linq.XName" /> are included in the collection.</returns>
		public static IEnumerable<XElement> Elements<T>(this IEnumerable<T> source, XName name) where T : XContainer
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(name != null))
			{
				return XElement.EmptySequence;
			}
			return Extensions.GetElements<T>(source, name);
		}

		/// <summary>Returns a collection of nodes that contains all nodes in the source collection, sorted in document order.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XNode" /> that contains the source collection.</param>
		/// <typeparam name="T">The type of the objects in <paramref name="source" />, constrained to <see cref="T:System.Xml.Linq.XNode" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XNode" /> that contains all nodes in the source collection, sorted in document order.</returns>
		public static IEnumerable<T> InDocumentOrder<T>(this IEnumerable<T> source) where T : XNode
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return Extensions.DocumentOrderIterator<T>(source);
		}

		private static IEnumerable<T> DocumentOrderIterator<T>(IEnumerable<T> source) where T : XNode
		{
			int count;
			T[] items = EnumerableHelpers.ToArray<T>(source, out count);
			if (count > 0)
			{
				XNode[] array = items;
				Array.Sort<XNode>(array, 0, count, XNode.DocumentOrderComparer);
				int num;
				for (int i = 0; i != count; i = num)
				{
					yield return items[i];
					num = i + 1;
				}
			}
			yield break;
		}

		/// <summary>Removes every attribute in the source collection from its parent element.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XAttribute" /> that contains the source collection.</param>
		public static void Remove(this IEnumerable<XAttribute> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			int num;
			XAttribute[] array = EnumerableHelpers.ToArray<XAttribute>(source, out num);
			for (int i = 0; i < num; i++)
			{
				XAttribute xattribute = array[i];
				if (xattribute != null)
				{
					xattribute.Remove();
				}
			}
		}

		/// <summary>Removes every node in the source collection from its parent node.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:System.Xml.Linq.XNode" /> that contains the source collection.</param>
		/// <typeparam name="T">The type of the objects in <paramref name="source" />, constrained to <see cref="T:System.Xml.Linq.XNode" />.</typeparam>
		public static void Remove<T>(this IEnumerable<T> source) where T : XNode
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			int num;
			T[] array = EnumerableHelpers.ToArray<T>(source, out num);
			for (int i = 0; i < num; i++)
			{
				T t = array[i];
				if (t != null)
				{
					t.Remove();
				}
			}
		}

		private static IEnumerable<XAttribute> GetAttributes(IEnumerable<XElement> source, XName name)
		{
			foreach (XElement e in source)
			{
				if (e != null)
				{
					XAttribute a = e.lastAttr;
					if (a != null)
					{
						do
						{
							a = a.next;
							if (name == null || a.name == name)
							{
								yield return a;
							}
						}
						while (a.parent == e && a != e.lastAttr);
					}
					a = null;
				}
				e = null;
			}
			IEnumerator<XElement> enumerator = null;
			yield break;
			yield break;
		}

		private static IEnumerable<XElement> GetAncestors<T>(IEnumerable<T> source, XName name, bool self) where T : XNode
		{
			foreach (T t in source)
			{
				XNode xnode = t;
				if (xnode != null)
				{
					XElement e;
					for (e = ((self ? xnode : xnode.parent) as XElement); e != null; e = (e.parent as XElement))
					{
						if (name == null || e.name == name)
						{
							yield return e;
						}
					}
					e = null;
				}
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}

		private static IEnumerable<XNode> GetDescendantNodes<T>(IEnumerable<T> source, bool self) where T : XContainer
		{
			foreach (T t in source)
			{
				XContainer root = t;
				if (root != null)
				{
					if (self)
					{
						yield return root;
					}
					XNode i = root;
					for (;;)
					{
						XContainer xcontainer = i as XContainer;
						XNode firstNode;
						if (xcontainer != null && (firstNode = xcontainer.FirstNode) != null)
						{
							i = firstNode;
						}
						else
						{
							while (i != null && i != root && i == i.parent.content)
							{
								i = i.parent;
							}
							if (i == null || i == root)
							{
								break;
							}
							i = i.next;
						}
						yield return i;
					}
					i = null;
				}
				root = null;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}

		private static IEnumerable<XElement> GetDescendants<T>(IEnumerable<T> source, XName name, bool self) where T : XContainer
		{
			foreach (T t in source)
			{
				XContainer root = t;
				if (root != null)
				{
					if (self)
					{
						XElement xelement = (XElement)root;
						if (name == null || xelement.name == name)
						{
							yield return xelement;
						}
					}
					XNode i = root;
					XContainer xcontainer = root;
					for (;;)
					{
						if (xcontainer != null && xcontainer.content is XNode)
						{
							i = ((XNode)xcontainer.content).next;
						}
						else
						{
							while (i != null && i != root && i == i.parent.content)
							{
								i = i.parent;
							}
							if (i == null || i == root)
							{
								break;
							}
							i = i.next;
						}
						XElement e = i as XElement;
						if (e != null && (name == null || e.name == name))
						{
							yield return e;
						}
						xcontainer = e;
						e = null;
					}
					i = null;
				}
				root = null;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}

		private static IEnumerable<XElement> GetElements<T>(IEnumerable<T> source, XName name) where T : XContainer
		{
			foreach (T t in source)
			{
				XContainer root = t;
				if (root != null)
				{
					XNode i = root.content as XNode;
					if (i != null)
					{
						do
						{
							i = i.next;
							XElement xelement = i as XElement;
							if (xelement != null && (name == null || xelement.name == name))
							{
								yield return xelement;
							}
						}
						while (i.parent == root && i != root.content);
					}
					i = null;
				}
				root = null;
			}
			IEnumerator<T> enumerator = null;
			yield break;
			yield break;
		}
	}
}
