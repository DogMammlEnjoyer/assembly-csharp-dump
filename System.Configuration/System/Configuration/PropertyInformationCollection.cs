using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Configuration
{
	/// <summary>Contains a collection of <see cref="T:System.Configuration.PropertyInformation" /> objects. This class cannot be inherited.</summary>
	[Serializable]
	public sealed class PropertyInformationCollection : NameObjectCollectionBase
	{
		internal PropertyInformationCollection() : base(StringComparer.Ordinal)
		{
		}

		/// <summary>Copies the entire <see cref="T:System.Configuration.PropertyInformationCollection" /> collection to a compatible one-dimensional <see cref="T:System.Array" />, starting at the specified index of the target array.</summary>
		/// <param name="array">A one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from the <see cref="T:System.Configuration.PropertyInformationCollection" /> collection. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <see cref="P:System.Array.Length" /> property of <paramref name="array" /> is less than <see cref="P:System.Collections.Specialized.NameObjectCollectionBase.Count" /> + <paramref name="index" />.</exception>
		public void CopyTo(PropertyInformation[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		/// <summary>Gets the <see cref="T:System.Configuration.PropertyInformation" /> object in the collection, based on the specified property name.</summary>
		/// <param name="propertyName">The name of the configuration attribute contained in the <see cref="T:System.Configuration.PropertyInformationCollection" /> object.</param>
		/// <returns>A <see cref="T:System.Configuration.PropertyInformation" /> object.</returns>
		public PropertyInformation this[string propertyName]
		{
			get
			{
				return (PropertyInformation)base.BaseGet(propertyName);
			}
		}

		/// <summary>Gets an <see cref="T:System.Collections.IEnumerator" /> object, which is used to iterate through this <see cref="T:System.Configuration.PropertyInformationCollection" /> collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object, which is used to iterate through this <see cref="T:System.Configuration.PropertyInformationCollection" />.</returns>
		public override IEnumerator GetEnumerator()
		{
			return new PropertyInformationCollection.PropertyInformationEnumerator(this);
		}

		internal void Add(PropertyInformation pi)
		{
			base.BaseAdd(pi.Name, pi);
		}

		/// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the data needed to serialize the <see cref="T:System.Configuration.PropertyInformationCollection" /> instance.</summary>
		/// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object that contains the information required to serialize the <see cref="T:System.Configuration.PropertyInformationCollection" /> instance.</param>
		/// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> object that contains the source and destination of the serialized stream associated with the <see cref="T:System.Configuration.PropertyInformationCollection" /> instance.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="info" /> is <see langword="null" />.</exception>
		[MonoTODO]
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		private class PropertyInformationEnumerator : IEnumerator
		{
			public PropertyInformationEnumerator(PropertyInformationCollection collection)
			{
				this.collection = collection;
				this.position = -1;
			}

			public object Current
			{
				get
				{
					if (this.position < this.collection.Count && this.position >= 0)
					{
						return this.collection.BaseGet(this.position);
					}
					throw new InvalidOperationException();
				}
			}

			public bool MoveNext()
			{
				int num = this.position + 1;
				this.position = num;
				return num < this.collection.Count;
			}

			public void Reset()
			{
				this.position = -1;
			}

			private PropertyInformationCollection collection;

			private int position;
		}
	}
}
