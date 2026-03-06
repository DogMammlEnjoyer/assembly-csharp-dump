using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	/// <summary>Enables iteration over a collection of <see cref="T:System.Xml.Schema.XmlSchema" /> objects. </summary>
	public class XmlSchemaEnumerator : IEnumerator<XmlSchema>, IDisposable, IEnumerator
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.XmlSchemaEnumerator" /> class. </summary>
		/// <param name="list">The <see cref="T:System.Xml.Serialization.XmlSchemas" /> object you want to iterate over.</param>
		public XmlSchemaEnumerator(XmlSchemas list)
		{
			this.list = list;
			this.idx = -1;
			this.end = list.Count - 1;
		}

		/// <summary>Releases all resources used by the <see cref="T:System.Xml.Serialization.XmlSchemaEnumerator" />.</summary>
		public void Dispose()
		{
		}

		/// <summary>Advances the enumerator to the next item in the collection.</summary>
		/// <returns>
		///     <see langword="true" /> if the move is successful; otherwise, <see langword="false" />.</returns>
		public bool MoveNext()
		{
			if (this.idx >= this.end)
			{
				return false;
			}
			this.idx++;
			return true;
		}

		/// <summary>Gets the current element in the collection.</summary>
		/// <returns>The current <see cref="T:System.Xml.Schema.XmlSchema" /> object in the collection.</returns>
		public XmlSchema Current
		{
			get
			{
				return this.list[this.idx];
			}
		}

		/// <summary>Gets the current element in the collection of <see cref="T:System.Xml.Schema.XmlSchema" /> objects.</summary>
		/// <returns>The current element in the collection of <see cref="T:System.Xml.Schema.XmlSchema" /> objects.</returns>
		object IEnumerator.Current
		{
			get
			{
				return this.list[this.idx];
			}
		}

		/// <summary>Sets the enumerator to its initial position, which is before the first element in the collection of <see cref="T:System.Xml.Schema.XmlSchema" /> objects.</summary>
		void IEnumerator.Reset()
		{
			this.idx = -1;
		}

		private XmlSchemas list;

		private int idx;

		private int end;
	}
}
