using System;

namespace System.Configuration
{
	/// <summary>Contains a collection of <see cref="T:System.Configuration.KeyValueConfigurationElement" /> objects.</summary>
	[ConfigurationCollection(typeof(KeyValueConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class KeyValueConfigurationCollection : ConfigurationElementCollection
	{
		/// <summary>Adds a <see cref="T:System.Configuration.KeyValueConfigurationElement" /> object to the collection based on the supplied parameters.</summary>
		/// <param name="keyValue">A <see cref="T:System.Configuration.KeyValueConfigurationElement" />.</param>
		public void Add(KeyValueConfigurationElement keyValue)
		{
			keyValue.Init();
			this.BaseAdd(keyValue);
		}

		/// <summary>Adds a <see cref="T:System.Configuration.KeyValueConfigurationElement" /> object to the collection based on the supplied parameters.</summary>
		/// <param name="key">A string specifying the key.</param>
		/// <param name="value">A string specifying the value.</param>
		public void Add(string key, string value)
		{
			this.Add(new KeyValueConfigurationElement(key, value));
		}

		/// <summary>Clears the <see cref="T:System.Configuration.KeyValueConfigurationCollection" /> collection.</summary>
		public void Clear()
		{
			base.BaseClear();
		}

		/// <summary>Removes a <see cref="T:System.Configuration.KeyValueConfigurationElement" /> object from the collection.</summary>
		/// <param name="key">A string specifying the <paramref name="key" />.</param>
		public void Remove(string key)
		{
			base.BaseRemove(key);
		}

		/// <summary>Gets the keys to all items contained in the <see cref="T:System.Configuration.KeyValueConfigurationCollection" /> collection.</summary>
		/// <returns>A string array.</returns>
		public string[] AllKeys
		{
			get
			{
				string[] array = new string[base.Count];
				int num = 0;
				foreach (object obj in this)
				{
					KeyValueConfigurationElement keyValueConfigurationElement = (KeyValueConfigurationElement)obj;
					array[num++] = keyValueConfigurationElement.Key;
				}
				return array;
			}
		}

		/// <summary>Gets the <see cref="T:System.Configuration.KeyValueConfigurationElement" /> object based on the supplied parameter.</summary>
		/// <param name="key">The key of the <see cref="T:System.Configuration.KeyValueConfigurationElement" /> contained in the collection.</param>
		/// <returns>A configuration element, or <see langword="null" /> if the key does not exist in the collection.</returns>
		public KeyValueConfigurationElement this[string key]
		{
			get
			{
				return (KeyValueConfigurationElement)base.BaseGet(key);
			}
		}

		/// <summary>When overridden in a derived class, the <see cref="M:System.Configuration.KeyValueConfigurationCollection.CreateNewElement" /> method creates a new <see cref="T:System.Configuration.KeyValueConfigurationElement" /> object.</summary>
		/// <returns>A newly created <see cref="T:System.Configuration.KeyValueConfigurationElement" />.</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new KeyValueConfigurationElement();
		}

		/// <summary>Gets the element key for a specified configuration element when overridden in a derived class.</summary>
		/// <param name="element">The <see cref="T:System.Configuration.KeyValueConfigurationElement" /> to which the key should be returned.</param>
		/// <returns>An object that acts as the key for the specified <see cref="T:System.Configuration.KeyValueConfigurationElement" />.</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((KeyValueConfigurationElement)element).Key;
		}

		/// <summary>Gets a collection of configuration properties.</summary>
		/// <returns>A collection of configuration properties.</returns>
		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new ConfigurationPropertyCollection();
				}
				return this.properties;
			}
		}

		/// <summary>Gets a value indicating whether an attempt to add a duplicate <see cref="T:System.Configuration.KeyValueConfigurationElement" /> object to the <see cref="T:System.Configuration.KeyValueConfigurationCollection" /> collection will cause an exception to be thrown.</summary>
		/// <returns>
		///   <see langword="true" /> if an attempt to add a duplicate <see cref="T:System.Configuration.KeyValueConfigurationElement" /> to the <see cref="T:System.Configuration.KeyValueConfigurationCollection" /> will cause an exception to be thrown; otherwise, <see langword="false" />.</returns>
		protected override bool ThrowOnDuplicate
		{
			get
			{
				return false;
			}
		}

		private ConfigurationPropertyCollection properties;
	}
}
