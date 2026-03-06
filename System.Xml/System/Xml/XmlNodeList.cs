using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Xml
{
	/// <summary>Represents an ordered collection of nodes.</summary>
	public abstract class XmlNodeList : IEnumerable, IDisposable
	{
		/// <summary>Retrieves a node at the given index.</summary>
		/// <param name="index">The zero-based index into the list of nodes.</param>
		/// <returns>The <see cref="T:System.Xml.XmlNode" /> with the specified index in the collection. If <paramref name="index" /> is greater than or equal to the number of nodes in the list, this returns <see langword="null" />.</returns>
		public abstract XmlNode Item(int index);

		/// <summary>Gets the number of nodes in the <see langword="XmlNodeList" />.</summary>
		/// <returns>The number of nodes in the <see langword="XmlNodeList" />.</returns>
		public abstract int Count { get; }

		/// <summary>Gets an enumerator that iterates through the collection of nodes.</summary>
		/// <returns>An enumerator used to iterate through the collection of nodes.</returns>
		public abstract IEnumerator GetEnumerator();

		/// <summary>Gets a node at the given index.</summary>
		/// <param name="i">The zero-based index into the list of nodes.</param>
		/// <returns>The <see cref="T:System.Xml.XmlNode" /> with the specified index in the collection. If index is greater than or equal to the number of nodes in the list, this returns <see langword="null" />.</returns>
		[IndexerName("ItemOf")]
		public virtual XmlNode this[int i]
		{
			get
			{
				return this.Item(i);
			}
		}

		/// <summary>Releases all resources used by the <see cref="T:System.Xml.XmlNodeList" /> class.</summary>
		void IDisposable.Dispose()
		{
			this.PrivateDisposeNodeList();
		}

		/// <summary>Disposes resources in the node list privately.</summary>
		protected virtual void PrivateDisposeNodeList()
		{
		}
	}
}
