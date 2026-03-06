using System;
using System.Collections;

namespace System.Configuration
{
	/// <summary>Contains a collection of <see cref="T:System.Configuration.SettingsProperty" /> objects.</summary>
	public class SettingsPropertyCollection : ICloneable, ICollection, IEnumerable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> class.</summary>
		public SettingsPropertyCollection()
		{
			this.items = new Hashtable();
		}

		/// <summary>Adds a <see cref="T:System.Configuration.SettingsProperty" /> object to the collection.</summary>
		/// <param name="property">A <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		public void Add(SettingsProperty property)
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException();
			}
			this.OnAdd(property);
			this.items.Add(property.Name, property);
			this.OnAddComplete(property);
		}

		/// <summary>Removes all <see cref="T:System.Configuration.SettingsProperty" /> objects from the collection.</summary>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		public void Clear()
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException();
			}
			this.OnClear();
			this.items.Clear();
			this.OnClearComplete();
		}

		/// <summary>Creates a copy of the existing collection.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsPropertyCollection" /> class.</returns>
		public object Clone()
		{
			return new SettingsPropertyCollection
			{
				items = (Hashtable)this.items.Clone()
			};
		}

		/// <summary>Copies this <see cref="T:System.Configuration.SettingsPropertyCollection" /> object to an array.</summary>
		/// <param name="array">The array to copy the object to.</param>
		/// <param name="index">The index at which to begin copying.</param>
		public void CopyTo(Array array, int index)
		{
			this.items.Values.CopyTo(array, index);
		}

		/// <summary>Gets the <see cref="T:System.Collections.IEnumerator" /> object as it applies to the collection.</summary>
		/// <returns>The <see cref="T:System.Collections.IEnumerator" /> object as it applies to the collection.</returns>
		public IEnumerator GetEnumerator()
		{
			return this.items.Values.GetEnumerator();
		}

		/// <summary>Removes a <see cref="T:System.Configuration.SettingsProperty" /> object from the collection.</summary>
		/// <param name="name">The name of the <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		public void Remove(string name)
		{
			if (this.isReadOnly)
			{
				throw new NotSupportedException();
			}
			SettingsProperty property = (SettingsProperty)this.items[name];
			this.OnRemove(property);
			this.items.Remove(name);
			this.OnRemoveComplete(property);
		}

		/// <summary>Sets the collection to be read-only.</summary>
		public void SetReadOnly()
		{
			this.isReadOnly = true;
		}

		/// <summary>Performs additional, custom processing when adding to the contents of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> instance.</summary>
		/// <param name="property">A <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
		protected virtual void OnAdd(SettingsProperty property)
		{
		}

		/// <summary>Performs additional, custom processing after adding to the contents of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> instance.</summary>
		/// <param name="property">A <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
		protected virtual void OnAddComplete(SettingsProperty property)
		{
		}

		/// <summary>Performs additional, custom processing when clearing the contents of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> instance.</summary>
		protected virtual void OnClear()
		{
		}

		/// <summary>Performs additional, custom processing after clearing the contents of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> instance.</summary>
		protected virtual void OnClearComplete()
		{
		}

		/// <summary>Performs additional, custom processing when removing the contents of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> instance.</summary>
		/// <param name="property">A <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
		protected virtual void OnRemove(SettingsProperty property)
		{
		}

		/// <summary>Performs additional, custom processing after removing the contents of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> instance.</summary>
		/// <param name="property">A <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
		protected virtual void OnRemoveComplete(SettingsProperty property)
		{
		}

		/// <summary>Gets a value that specifies the number of <see cref="T:System.Configuration.SettingsProperty" /> objects in the collection.</summary>
		/// <returns>The number of <see cref="T:System.Configuration.SettingsProperty" /> objects in the collection.</returns>
		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		/// <summary>Gets a value that indicates whether access to the collection is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Configuration.SettingsPropertyCollection" /> is synchronized; otherwise, <see langword="false" />.</returns>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets the collection item with the specified name.</summary>
		/// <param name="name">The name of the <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
		/// <returns>The <see cref="T:System.Configuration.SettingsProperty" /> object with the specified <paramref name="name" />.</returns>
		public SettingsProperty this[string name]
		{
			get
			{
				return (SettingsProperty)this.items[name];
			}
		}

		/// <summary>Gets the object to synchronize access to the collection.</summary>
		/// <returns>The object to synchronize access to the collection.</returns>
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		private Hashtable items;

		private bool isReadOnly;
	}
}
