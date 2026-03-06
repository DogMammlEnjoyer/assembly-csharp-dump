using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	/// <summary>Contains functionality to compare nodes for their document order. This class cannot be inherited.</summary>
	public sealed class XNodeDocumentOrderComparer : IComparer, IComparer<XNode>
	{
		/// <summary>Compares two nodes to determine their relative document order.</summary>
		/// <param name="x">The first <see cref="T:System.Xml.Linq.XNode" /> to compare.</param>
		/// <param name="y">The second <see cref="T:System.Xml.Linq.XNode" /> to compare.</param>
		/// <returns>An <see cref="T:System.Int32" /> that contains 0 if the nodes are equal; -1 if <paramref name="x" /> is before <paramref name="y" />; 1 if <paramref name="x" /> is after <paramref name="y" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The two nodes do not share a common ancestor.</exception>
		public int Compare(XNode x, XNode y)
		{
			return XNode.CompareDocumentOrder(x, y);
		}

		/// <summary>Compares two nodes to determine their relative document order.</summary>
		/// <param name="x">The first <see cref="T:System.Xml.Linq.XNode" /> to compare.</param>
		/// <param name="y">The second <see cref="T:System.Xml.Linq.XNode" /> to compare.</param>
		/// <returns>An <see cref="T:System.Int32" /> that contains 0 if the nodes are equal; -1 if <paramref name="x" /> is before <paramref name="y" />; 1 if <paramref name="x" /> is after <paramref name="y" />.</returns>
		/// <exception cref="T:System.InvalidOperationException">The two nodes do not share a common ancestor.</exception>
		/// <exception cref="T:System.ArgumentException">The two nodes are not derived from <see cref="T:System.Xml.Linq.XNode" />.</exception>
		int IComparer.Compare(object x, object y)
		{
			XNode xnode = x as XNode;
			if (xnode == null && x != null)
			{
				throw new ArgumentException(SR.Format("The argument must be derived from {0}.", typeof(XNode)), "x");
			}
			XNode xnode2 = y as XNode;
			if (xnode2 == null && y != null)
			{
				throw new ArgumentException(SR.Format("The argument must be derived from {0}.", typeof(XNode)), "y");
			}
			return this.Compare(xnode, xnode2);
		}
	}
}
