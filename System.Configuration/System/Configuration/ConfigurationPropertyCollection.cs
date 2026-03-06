using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Configuration
{
	/// <summary>Represents a collection of configuration-element properties.</summary>
	public class ConfigurationPropertyCollection : ICollection, IEnumerable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationPropertyCollection" /> class.</summary>
		public ConfigurationPropertyCollection()
		{
			this.collection = new List<ConfigurationProperty>();
		}

		/// <summary>Gets the number of properties in the collection.</summary>
		/// <returns>The number of properties in the collection.</returns>
		public int Count
		{
			get
			{
				return this.collection.Count;
			}
		}

		/// <summary>Gets the collection item with the specified name.</summary>
		/// <param name="name">The <see cref="T:System.Configuration.ConfigurationProperty" /> to return.</param>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationProperty" /> with the specified <paramref name="name" />.</returns>
		public ConfigurationProperty this[string name]
		{
			get
			{
				foreach (ConfigurationProperty configurationProperty in this.collection)
				{
					if (configurationProperty.Name == name)
					{
						return configurationProperty;
					}
				}
				return null;
			}
		}

		/// <summary>Gets a value indicating whether access to the collection is synchronized (thread safe).</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Configuration.ConfigurationPropertyCollection" /> is synchronized; otherwise, <see langword="false" />.</returns>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets the object to synchronize access to the collection.</summary>
		/// <returns>The object to synchronize access to the collection.</returns>
		public object SyncRoot
		{
			get
			{
				return this.collection;
			}
		}

		/// <summary>Adds a configuration property to the collection.</summary>
		/// <param name="property">The <see cref="T:System.Configuration.ConfigurationProperty" /> to add.</param>
		public void Add(ConfigurationProperty property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			this.collection.Add(property);
		}

		/// <summary>Specifies whether the configuration property is contained in this collection.</summary>
		/// <param name="name">An identifier for the <see cref="T:System.Configuration.ConfigurationProperty" /> to verify.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Configuration.ConfigurationProperty" /> is contained in the collection; otherwise, <see langword="false" />.</returns>
		public bool Contains(string name)
		{
			ConfigurationProperty configurationProperty = this[name];
			return configurationProperty != null && this.collection.Contains(configurationProperty);
		}

		/// <summary>Copies this ConfigurationPropertyCollection to an array.</summary>
		/// <param name="array">Array to which to copy.</param>
		/// <param name="index">Index at which to begin copying.</param>
		public void CopyTo(ConfigurationProperty[] array, int index)
		{
			this.collection.CopyTo(array, index);
		}

		/// <summary>Copies this collection to an array.</summary>
		/// <param name="array">The array to which to copy.</param>
		/// <param name="index">The index location at which to begin copying.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this.collection).CopyTo(array, index);
		}

		/// <summary>Gets the <see cref="T:System.Collections.IEnumerator" /> object as it applies to the collection.</summary>
		/// <returns>The <see cref="T:System.Collections.IEnumerator" /> object as it applies to the collection</returns>
		public IEnumerator GetEnumerator()
		{
			return this.collection.GetEnumerator();
		}

		/// <summary>Removes a configuration property from the collection.</summary>
		/// <param name="name">The <see cref="T:System.Configuration.ConfigurationProperty" /> to remove.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Configuration.ConfigurationProperty" /> was removed; otherwise, <see langword="false" />.</returns>
		public bool Remove(string name)
		{
			return this.collection.Remove(this[name]);
		}

		/// <summary>Removes all configuration property objects from the collection.</summary>
		public void Clear()
		{
			this.collection.Clear();
		}

		private List<ConfigurationProperty> collection;
	}
}
