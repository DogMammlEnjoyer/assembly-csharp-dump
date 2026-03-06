using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	/// <summary>Compares nodes to determine whether they are equal. This class cannot be inherited.</summary>
	public sealed class XNodeEqualityComparer : IEqualityComparer, IEqualityComparer<XNode>
	{
		/// <summary>Compares the values of two nodes.</summary>
		/// <param name="x">The first <see cref="T:System.Xml.Linq.XNode" /> to compare.</param>
		/// <param name="y">The second <see cref="T:System.Xml.Linq.XNode" /> to compare.</param>
		/// <returns>A <see cref="T:System.Boolean" /> indicating if the nodes are equal.</returns>
		public bool Equals(XNode x, XNode y)
		{
			return XNode.DeepEquals(x, y);
		}

		/// <summary>Returns a hash code based on an <see cref="T:System.Xml.Linq.XNode" />.</summary>
		/// <param name="obj">The <see cref="T:System.Xml.Linq.XNode" /> to hash.</param>
		/// <returns>A <see cref="T:System.Int32" /> that contains a value-based hash code for the node.</returns>
		public int GetHashCode(XNode obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return obj.GetDeepHashCode();
		}

		/// <summary>Compares the values of two nodes.</summary>
		/// <param name="x">The first <see cref="T:System.Xml.Linq.XNode" /> to compare.</param>
		/// <param name="y">The second <see cref="T:System.Xml.Linq.XNode" /> to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the nodes are equal; otherwise <see langword="false" />.</returns>
		bool IEqualityComparer.Equals(object x, object y)
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
			return this.Equals(xnode, xnode2);
		}

		/// <summary>Returns a hash code based on the value of a node.</summary>
		/// <param name="obj">The node to hash.</param>
		/// <returns>A <see cref="T:System.Int32" /> that contains a value-based hash code for the node.</returns>
		int IEqualityComparer.GetHashCode(object obj)
		{
			XNode xnode = obj as XNode;
			if (xnode == null && obj != null)
			{
				throw new ArgumentException(SR.Format("The argument must be derived from {0}.", typeof(XNode)), "obj");
			}
			return this.GetHashCode(xnode);
		}
	}
}
