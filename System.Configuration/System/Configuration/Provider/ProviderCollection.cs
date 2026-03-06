using System;
using System.Collections;

namespace System.Configuration.Provider
{
	/// <summary>Represents a collection of provider objects that inherit from <see cref="T:System.Configuration.Provider.ProviderBase" />.</summary>
	public class ProviderCollection : ICollection, IEnumerable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.Provider.ProviderCollection" /> class.</summary>
		public ProviderCollection()
		{
			this.lookup = new Hashtable(10, StringComparer.InvariantCultureIgnoreCase);
			this.values = new ArrayList();
		}

		/// <summary>Adds a provider to the collection.</summary>
		/// <param name="provider">The provider to be added.</param>
		/// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="provider" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Configuration.Provider.ProviderBase.Name" /> of <paramref name="provider" /> is <see langword="null" />.  
		/// -or-
		///  The length of the <see cref="P:System.Configuration.Provider.ProviderBase.Name" /> of <paramref name="provider" /> is less than 1.</exception>
		public virtual void Add(ProviderBase provider)
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			if (provider == null || provider.Name == null)
			{
				throw new ArgumentNullException();
			}
			int num = this.values.Add(provider);
			try
			{
				this.lookup.Add(provider.Name, num);
			}
			catch
			{
				this.values.RemoveAt(num);
				throw;
			}
		}

		/// <summary>Removes all items from the collection.</summary>
		/// <exception cref="T:System.NotSupportedException">The collection is set to read-only.</exception>
		public void Clear()
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			this.values.Clear();
			this.lookup.Clear();
		}

		/// <summary>Copies the contents of the collection to the given array starting at the specified index.</summary>
		/// <param name="array">The array to copy the elements of the collection to.</param>
		/// <param name="index">The index of the collection item at which to start the copying process.</param>
		public void CopyTo(ProviderBase[] array, int index)
		{
			this.values.CopyTo(array, index);
		}

		/// <summary>Copies the elements of the <see cref="T:System.Configuration.Provider.ProviderCollection" /> to an array, starting at a particular array index.</summary>
		/// <param name="array">The array to copy the elements of the collection to.</param>
		/// <param name="index">The index of the array at which to start copying provider instances from the collection.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			this.values.CopyTo(array, index);
		}

		/// <summary>Returns an object that implements the <see cref="T:System.Collections.IEnumerator" /> interface to iterate through the collection.</summary>
		/// <returns>An object that implements <see cref="T:System.Collections.IEnumerator" /> to iterate through the collection.</returns>
		public IEnumerator GetEnumerator()
		{
			return this.values.GetEnumerator();
		}

		/// <summary>Removes a provider from the collection.</summary>
		/// <param name="name">The name of the provider to be removed.</param>
		/// <exception cref="T:System.NotSupportedException">The collection has been set to read-only.</exception>
		public void Remove(string name)
		{
			if (this.readOnly)
			{
				throw new NotSupportedException();
			}
			object obj = this.lookup[name];
			if (obj == null || !(obj is int))
			{
				throw new ArgumentException();
			}
			int num = (int)obj;
			if (num >= this.values.Count)
			{
				throw new ArgumentException();
			}
			this.values.RemoveAt(num);
			this.lookup.Remove(name);
			ArrayList arrayList = new ArrayList();
			foreach (object obj2 in this.lookup)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj2;
				if ((int)dictionaryEntry.Value > num)
				{
					arrayList.Add(dictionaryEntry.Key);
				}
			}
			foreach (object obj3 in arrayList)
			{
				string key = (string)obj3;
				this.lookup[key] = (int)this.lookup[key] - 1;
			}
		}

		/// <summary>Sets the collection to be read-only.</summary>
		public void SetReadOnly()
		{
			this.readOnly = true;
		}

		/// <summary>Gets the number of providers in the collection.</summary>
		/// <returns>The number of providers in the collection.</returns>
		public int Count
		{
			get
			{
				return this.values.Count;
			}
		}

		/// <summary>Gets a value indicating whether access to the collection is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="false" /> in all cases.</returns>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets the current object.</summary>
		/// <returns>The current object.</returns>
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		/// <summary>Gets the provider with the specified name.</summary>
		/// <param name="name">The key by which the provider is identified.</param>
		/// <returns>The provider with the specified name.</returns>
		public ProviderBase this[string name]
		{
			get
			{
				object obj = this.lookup[name];
				if (obj == null)
				{
					return null;
				}
				return this.values[(int)obj] as ProviderBase;
			}
		}

		private Hashtable lookup;

		private bool readOnly;

		private ArrayList values;
	}
}
