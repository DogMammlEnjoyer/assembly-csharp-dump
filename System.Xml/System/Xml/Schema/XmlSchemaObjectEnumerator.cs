using System;
using System.Collections;
using Unity;

namespace System.Xml.Schema
{
	/// <summary>Represents the enumerator for the <see cref="T:System.Xml.Schema.XmlSchemaObjectCollection" />.</summary>
	public class XmlSchemaObjectEnumerator : IEnumerator
	{
		internal XmlSchemaObjectEnumerator(IEnumerator enumerator)
		{
			this.enumerator = enumerator;
		}

		/// <summary>Resets the enumerator to the start of the collection.</summary>
		public void Reset()
		{
			this.enumerator.Reset();
		}

		/// <summary>Moves to the next item in the collection.</summary>
		/// <returns>
		///     <see langword="false" /> at the end of the collection.</returns>
		public bool MoveNext()
		{
			return this.enumerator.MoveNext();
		}

		/// <summary>Gets the current <see cref="T:System.Xml.Schema.XmlSchemaObject" /> in the collection.</summary>
		/// <returns>The current <see cref="T:System.Xml.Schema.XmlSchemaObject" />.</returns>
		public XmlSchemaObject Current
		{
			get
			{
				return (XmlSchemaObject)this.enumerator.Current;
			}
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.Schema.XmlSchemaObjectEnumerator.Reset" />.</summary>
		void IEnumerator.Reset()
		{
			this.enumerator.Reset();
		}

		/// <summary>For a description of this member, see <see cref="M:System.Xml.Schema.XmlSchemaObjectEnumerator.MoveNext" />.</summary>
		/// <returns>The next <see cref="T:System.Xml.Schema.XmlSchemaObject" />.</returns>
		bool IEnumerator.MoveNext()
		{
			return this.enumerator.MoveNext();
		}

		/// <summary>For a description of this member, see <see cref="P:System.Xml.Schema.XmlSchemaObjectEnumerator.Current" />.</summary>
		/// <returns>The current <see cref="T:System.Xml.Schema.XmlSchemaObject" />.</returns>
		object IEnumerator.Current
		{
			get
			{
				return this.enumerator.Current;
			}
		}

		internal XmlSchemaObjectEnumerator()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private IEnumerator enumerator;
	}
}
