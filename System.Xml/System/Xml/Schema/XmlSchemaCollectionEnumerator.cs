using System;
using System.Collections;
using Unity;

namespace System.Xml.Schema
{
	/// <summary>Supports a simple iteration over a collection. This class cannot be inherited. </summary>
	public sealed class XmlSchemaCollectionEnumerator : IEnumerator
	{
		internal XmlSchemaCollectionEnumerator(Hashtable collection)
		{
			this.enumerator = collection.GetEnumerator();
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.Schema.XmlSchemaCollectionEnumerator.System#Collections#IEnumerator#Reset" />.</summary>
		void IEnumerator.Reset()
		{
			this.enumerator.Reset();
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.Schema.XmlSchemaCollectionEnumerator.MoveNext" />.</summary>
		/// <returns>Returns the next node.</returns>
		bool IEnumerator.MoveNext()
		{
			return this.enumerator.MoveNext();
		}

		/// <summary>Advances the enumerator to the next schema in the collection.</summary>
		/// <returns>
		///     <see langword="true" /> if the move was successful; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
		public bool MoveNext()
		{
			return this.enumerator.MoveNext();
		}

		/// <summary>For a description of this member, see <see cref="P:System.Xml.Schema.XmlSchemaCollectionEnumerator.Current" />.</summary>
		/// <returns>Returns the current node.</returns>
		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		/// <summary>Gets the current <see cref="T:System.Xml.Schema.XmlSchema" /> in the collection.</summary>
		/// <returns>The current <see langword="XmlSchema" /> in the collection.</returns>
		public XmlSchema Current
		{
			get
			{
				XmlSchemaCollectionNode xmlSchemaCollectionNode = (XmlSchemaCollectionNode)this.enumerator.Value;
				if (xmlSchemaCollectionNode != null)
				{
					return xmlSchemaCollectionNode.Schema;
				}
				return null;
			}
		}

		internal XmlSchemaCollectionNode CurrentNode
		{
			get
			{
				return (XmlSchemaCollectionNode)this.enumerator.Value;
			}
		}

		internal XmlSchemaCollectionEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private IDictionaryEnumerator enumerator;
	}
}
