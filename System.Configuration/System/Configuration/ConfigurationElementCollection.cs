using System;
using System.Collections;
using System.Diagnostics;
using System.Xml;

namespace System.Configuration
{
	/// <summary>Represents a configuration element containing a collection of child elements.</summary>
	[DebuggerDisplay("Count = {Count}")]
	public abstract class ConfigurationElementCollection : ConfigurationElement, ICollection, IEnumerable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationElementCollection" /> class.</summary>
		protected ConfigurationElementCollection()
		{
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Configuration.ConfigurationElementCollection" /> class.</summary>
		/// <param name="comparer">The <see cref="T:System.Collections.IComparer" /> comparer to use.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="comparer" /> is <see langword="null" />.</exception>
		protected ConfigurationElementCollection(IComparer comparer)
		{
			this.comparer = comparer;
		}

		internal override void InitFromProperty(PropertyInformation propertyInfo)
		{
			ConfigurationCollectionAttribute configurationCollectionAttribute = propertyInfo.Property.CollectionAttribute;
			if (configurationCollectionAttribute == null)
			{
				configurationCollectionAttribute = (Attribute.GetCustomAttribute(propertyInfo.Type, typeof(ConfigurationCollectionAttribute)) as ConfigurationCollectionAttribute);
			}
			if (configurationCollectionAttribute != null)
			{
				this.addElementName = configurationCollectionAttribute.AddItemName;
				this.clearElementName = configurationCollectionAttribute.ClearItemsName;
				this.removeElementName = configurationCollectionAttribute.RemoveItemName;
			}
			base.InitFromProperty(propertyInfo);
		}

		/// <summary>Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationElementCollectionType" /> of this collection.</returns>
		public virtual ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.AddRemoveClearMap;
			}
		}

		private bool IsBasic
		{
			get
			{
				return this.CollectionType == ConfigurationElementCollectionType.BasicMap || this.CollectionType == ConfigurationElementCollectionType.BasicMapAlternate;
			}
		}

		private bool IsAlternate
		{
			get
			{
				return this.CollectionType == ConfigurationElementCollectionType.AddRemoveClearMapAlternate || this.CollectionType == ConfigurationElementCollectionType.BasicMapAlternate;
			}
		}

		/// <summary>Gets the number of elements in the collection.</summary>
		/// <returns>The number of elements in the collection.</returns>
		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		/// <summary>Gets the name used to identify this collection of elements in the configuration file when overridden in a derived class.</summary>
		/// <returns>The name of the collection; otherwise, an empty string. The default is an empty string.</returns>
		protected virtual string ElementName
		{
			get
			{
				return string.Empty;
			}
		}

		/// <summary>Gets or sets a value that specifies whether the collection has been cleared.</summary>
		/// <returns>
		///   <see langword="true" /> if the collection has been cleared; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration is read-only.</exception>
		public bool EmitClear
		{
			get
			{
				return this.emitClear;
			}
			set
			{
				this.emitClear = value;
			}
		}

		/// <summary>Gets a value indicating whether access to the collection is synchronized.</summary>
		/// <returns>
		///   <see langword="true" /> if access to the <see cref="T:System.Configuration.ConfigurationElementCollection" /> is synchronized; otherwise, <see langword="false" />.</returns>
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object used to synchronize access to the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</summary>
		/// <returns>An object used to synchronize access to the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</returns>
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		/// <summary>Gets a value indicating whether an attempt to add a duplicate <see cref="T:System.Configuration.ConfigurationElement" /> to the <see cref="T:System.Configuration.ConfigurationElementCollection" /> will cause an exception to be thrown.</summary>
		/// <returns>
		///   <see langword="true" /> if an attempt to add a duplicate <see cref="T:System.Configuration.ConfigurationElement" /> to this <see cref="T:System.Configuration.ConfigurationElementCollection" /> will cause an exception to be thrown; otherwise, <see langword="false" />.</returns>
		protected virtual bool ThrowOnDuplicate
		{
			get
			{
				return this.CollectionType == ConfigurationElementCollectionType.AddRemoveClearMap || this.CollectionType == ConfigurationElementCollectionType.AddRemoveClearMapAlternate;
			}
		}

		/// <summary>Gets or sets the name of the <see cref="T:System.Configuration.ConfigurationElement" /> to associate with the add operation in the <see cref="T:System.Configuration.ConfigurationElementCollection" /> when overridden in a derived class.</summary>
		/// <returns>The name of the element.</returns>
		/// <exception cref="T:System.ArgumentException">The selected value starts with the reserved prefix "config" or "lock".</exception>
		protected internal string AddElementName
		{
			get
			{
				return this.addElementName;
			}
			set
			{
				this.addElementName = value;
			}
		}

		/// <summary>Gets or sets the name for the <see cref="T:System.Configuration.ConfigurationElement" /> to associate with the clear operation in the <see cref="T:System.Configuration.ConfigurationElementCollection" /> when overridden in a derived class.</summary>
		/// <returns>The name of the element.</returns>
		/// <exception cref="T:System.ArgumentException">The selected value starts with the reserved prefix "config" or "lock".</exception>
		protected internal string ClearElementName
		{
			get
			{
				return this.clearElementName;
			}
			set
			{
				this.clearElementName = value;
			}
		}

		/// <summary>Gets or sets the name of the <see cref="T:System.Configuration.ConfigurationElement" /> to associate with the remove operation in the <see cref="T:System.Configuration.ConfigurationElementCollection" /> when overridden in a derived class.</summary>
		/// <returns>The name of the element.</returns>
		/// <exception cref="T:System.ArgumentException">The selected value starts with the reserved prefix "config" or "lock".</exception>
		protected internal string RemoveElementName
		{
			get
			{
				return this.removeElementName;
			}
			set
			{
				this.removeElementName = value;
			}
		}

		/// <summary>Adds a configuration element to the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to add.</param>
		protected virtual void BaseAdd(ConfigurationElement element)
		{
			this.BaseAdd(element, this.ThrowOnDuplicate);
		}

		/// <summary>Adds a configuration element to the configuration element collection.</summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to add.</param>
		/// <param name="throwIfExists">
		///   <see langword="true" /> to throw an exception if the <see cref="T:System.Configuration.ConfigurationElement" /> specified is already contained in the <see cref="T:System.Configuration.ConfigurationElementCollection" />; otherwise, <see langword="false" />.</param>
		/// <exception cref="T:System.Exception">The <see cref="T:System.Configuration.ConfigurationElement" /> to add already exists in the <see cref="T:System.Configuration.ConfigurationElementCollection" /> and the <paramref name="throwIfExists" /> parameter is <see langword="true" />.</exception>
		protected void BaseAdd(ConfigurationElement element, bool throwIfExists)
		{
			if (this.IsReadOnly())
			{
				throw new ConfigurationErrorsException("Collection is read only.");
			}
			if (this.IsAlternate)
			{
				this.list.Insert(this.inheritedLimitIndex, element);
				this.inheritedLimitIndex++;
			}
			else
			{
				int num = this.IndexOfKey(this.GetElementKey(element));
				if (num >= 0)
				{
					if (element.Equals(this.list[num]))
					{
						return;
					}
					if (throwIfExists)
					{
						throw new ConfigurationErrorsException("Duplicate element in collection");
					}
					this.list.RemoveAt(num);
				}
				this.list.Add(element);
			}
			this.modified = true;
		}

		/// <summary>Adds a configuration element to the configuration element collection.</summary>
		/// <param name="index">The index location at which to add the specified <see cref="T:System.Configuration.ConfigurationElement" />.</param>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to add.</param>
		protected virtual void BaseAdd(int index, ConfigurationElement element)
		{
			if (this.ThrowOnDuplicate && this.BaseIndexOf(element) != -1)
			{
				throw new ConfigurationErrorsException("Duplicate element in collection");
			}
			if (this.IsReadOnly())
			{
				throw new ConfigurationErrorsException("Collection is read only.");
			}
			if (this.IsAlternate && index > this.inheritedLimitIndex)
			{
				throw new ConfigurationErrorsException("Can't insert new elements below the inherited elements.");
			}
			if (!this.IsAlternate && index <= this.inheritedLimitIndex)
			{
				throw new ConfigurationErrorsException("Can't insert new elements above the inherited elements.");
			}
			this.list.Insert(index, element);
			this.modified = true;
		}

		/// <summary>Removes all configuration element objects from the collection.</summary>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration is read-only.  
		/// -or-
		///  A collection item has been locked in a higher-level configuration.</exception>
		protected internal void BaseClear()
		{
			if (this.IsReadOnly())
			{
				throw new ConfigurationErrorsException("Collection is read only.");
			}
			this.list.Clear();
			this.modified = true;
		}

		/// <summary>Gets the configuration element at the specified index location.</summary>
		/// <param name="index">The index location of the <see cref="T:System.Configuration.ConfigurationElement" /> to return.</param>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationElement" /> at the specified index.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">
		///   <paramref name="index" /> is less than <see langword="0" />.  
		/// -or-
		///  There is no <see cref="T:System.Configuration.ConfigurationElement" /> at the specified <paramref name="index" />.</exception>
		protected internal ConfigurationElement BaseGet(int index)
		{
			return (ConfigurationElement)this.list[index];
		}

		/// <summary>Returns the configuration element with the specified key.</summary>
		/// <param name="key">The key of the element to return.</param>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationElement" /> with the specified key; otherwise, <see langword="null" />.</returns>
		protected internal ConfigurationElement BaseGet(object key)
		{
			int num = this.IndexOfKey(key);
			if (num != -1)
			{
				return (ConfigurationElement)this.list[num];
			}
			return null;
		}

		/// <summary>Returns an array of the keys for all of the configuration elements contained in the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</summary>
		/// <returns>An array that contains the keys for all of the <see cref="T:System.Configuration.ConfigurationElement" /> objects contained in the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</returns>
		protected internal object[] BaseGetAllKeys()
		{
			object[] array = new object[this.list.Count];
			for (int i = 0; i < this.list.Count; i++)
			{
				array[i] = this.BaseGetKey(i);
			}
			return array;
		}

		/// <summary>Gets the key for the <see cref="T:System.Configuration.ConfigurationElement" /> at the specified index location.</summary>
		/// <param name="index">The index location for the <see cref="T:System.Configuration.ConfigurationElement" />.</param>
		/// <returns>The key for the specified <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">
		///   <paramref name="index" /> is less than <see langword="0" />.  
		/// -or-
		///  There is no <see cref="T:System.Configuration.ConfigurationElement" /> at the specified <paramref name="index" />.</exception>
		protected internal object BaseGetKey(int index)
		{
			if (index < 0 || index >= this.list.Count)
			{
				throw new ConfigurationErrorsException(string.Format("Index {0} is out of range", index));
			}
			return this.GetElementKey((ConfigurationElement)this.list[index]).ToString();
		}

		/// <summary>Indicates the index of the specified <see cref="T:System.Configuration.ConfigurationElement" />.</summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> for the specified index location.</param>
		/// <returns>The index of the specified <see cref="T:System.Configuration.ConfigurationElement" />; otherwise, -1.</returns>
		protected int BaseIndexOf(ConfigurationElement element)
		{
			return this.list.IndexOf(element);
		}

		private int IndexOfKey(object key)
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				if (this.CompareKeys(this.GetElementKey((ConfigurationElement)this.list[i]), key))
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>Indicates whether the <see cref="T:System.Configuration.ConfigurationElement" /> with the specified key has been removed from the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</summary>
		/// <param name="key">The key of the element to check.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.ConfigurationElement" /> with the specified key has been removed; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		protected internal bool BaseIsRemoved(object key)
		{
			if (this.removed == null)
			{
				return false;
			}
			foreach (object obj in this.removed)
			{
				ConfigurationElement element = (ConfigurationElement)obj;
				if (this.CompareKeys(this.GetElementKey(element), key))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Removes a <see cref="T:System.Configuration.ConfigurationElement" /> from the collection.</summary>
		/// <param name="key">The key of the <see cref="T:System.Configuration.ConfigurationElement" /> to remove.</param>
		/// <exception cref="T:System.Exception">No <see cref="T:System.Configuration.ConfigurationElement" /> with the specified key exists in the collection, the element has already been removed, or the element cannot be removed because the value of its <see cref="P:System.Configuration.ConfigurationProperty.Type" /> is not <see cref="F:System.Configuration.ConfigurationElementCollectionType.AddRemoveClearMap" />.</exception>
		protected internal void BaseRemove(object key)
		{
			if (this.IsReadOnly())
			{
				throw new ConfigurationErrorsException("Collection is read only.");
			}
			int num = this.IndexOfKey(key);
			if (num != -1)
			{
				this.BaseRemoveAt(num);
				this.modified = true;
			}
		}

		/// <summary>Removes the <see cref="T:System.Configuration.ConfigurationElement" /> at the specified index location.</summary>
		/// <param name="index">The index location of the <see cref="T:System.Configuration.ConfigurationElement" /> to remove.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration is read-only.  
		/// -or-
		///  <paramref name="index" /> is less than <see langword="0" /> or greater than the number of <see cref="T:System.Configuration.ConfigurationElement" /> objects in the collection.  
		/// -or-
		///  The <see cref="T:System.Configuration.ConfigurationElement" /> object has already been removed.  
		/// -or-
		///  The value of the <see cref="T:System.Configuration.ConfigurationElement" /> object has been locked at a higher level.  
		/// -or-
		///  The <see cref="T:System.Configuration.ConfigurationElement" /> object was inherited.  
		/// -or-
		///  The value of the <see cref="T:System.Configuration.ConfigurationElement" /> object's <see cref="P:System.Configuration.ConfigurationProperty.Type" /> is not <see cref="F:System.Configuration.ConfigurationElementCollectionType.AddRemoveClearMap" /> or <see cref="F:System.Configuration.ConfigurationElementCollectionType.AddRemoveClearMapAlternate" />.</exception>
		protected internal void BaseRemoveAt(int index)
		{
			if (this.IsReadOnly())
			{
				throw new ConfigurationErrorsException("Collection is read only.");
			}
			ConfigurationElement configurationElement = (ConfigurationElement)this.list[index];
			if (!this.IsElementRemovable(configurationElement))
			{
				throw new ConfigurationErrorsException("Element can't be removed from element collection.");
			}
			if (this.inherited != null && this.inherited.Contains(configurationElement))
			{
				throw new ConfigurationErrorsException("Inherited items can't be removed.");
			}
			this.list.RemoveAt(index);
			if (this.IsAlternate && this.inheritedLimitIndex > 0)
			{
				this.inheritedLimitIndex--;
			}
			this.modified = true;
		}

		private bool CompareKeys(object key1, object key2)
		{
			if (this.comparer != null)
			{
				return this.comparer.Compare(key1, key2) == 0;
			}
			return object.Equals(key1, key2);
		}

		/// <summary>Copies the contents of the <see cref="T:System.Configuration.ConfigurationElementCollection" /> to an array.</summary>
		/// <param name="array">Array to which to copy the contents of the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</param>
		/// <param name="index">Index location at which to begin copying.</param>
		public void CopyTo(ConfigurationElement[] array, int index)
		{
			this.list.CopyTo(array, index);
		}

		/// <summary>When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.</summary>
		/// <returns>A newly created <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
		protected abstract ConfigurationElement CreateNewElement();

		/// <summary>Creates a new <see cref="T:System.Configuration.ConfigurationElement" /> when overridden in a derived class.</summary>
		/// <param name="elementName">The name of the <see cref="T:System.Configuration.ConfigurationElement" /> to create.</param>
		/// <returns>A new <see cref="T:System.Configuration.ConfigurationElement" /> with a specified name.</returns>
		protected virtual ConfigurationElement CreateNewElement(string elementName)
		{
			return this.CreateNewElement();
		}

		private ConfigurationElement CreateNewElementInternal(string elementName)
		{
			ConfigurationElement configurationElement;
			if (elementName == null)
			{
				configurationElement = this.CreateNewElement();
			}
			else
			{
				configurationElement = this.CreateNewElement(elementName);
			}
			configurationElement.Init();
			return configurationElement;
		}

		/// <summary>Compares the <see cref="T:System.Configuration.ConfigurationElementCollection" /> to the specified object.</summary>
		/// <param name="compareTo">The object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the object to compare with is equal to the current <see cref="T:System.Configuration.ConfigurationElementCollection" /> instance; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public override bool Equals(object compareTo)
		{
			ConfigurationElementCollection configurationElementCollection = compareTo as ConfigurationElementCollection;
			if (configurationElementCollection == null)
			{
				return false;
			}
			if (base.GetType() != configurationElementCollection.GetType())
			{
				return false;
			}
			if (this.Count != configurationElementCollection.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Count; i++)
			{
				if (!this.BaseGet(i).Equals(configurationElementCollection.BaseGet(i)))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Gets the element key for a specified configuration element when overridden in a derived class.</summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for.</param>
		/// <returns>An <see cref="T:System.Object" /> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
		protected abstract object GetElementKey(ConfigurationElement element);

		/// <summary>Gets a unique value representing the <see cref="T:System.Configuration.ConfigurationElementCollection" /> instance.</summary>
		/// <returns>A unique value representing the <see cref="T:System.Configuration.ConfigurationElementCollection" /> current instance.</returns>
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.Count; i++)
			{
				num += this.BaseGet(i).GetHashCode();
			}
			return num;
		}

		/// <summary>Copies the <see cref="T:System.Configuration.ConfigurationElementCollection" /> to an array.</summary>
		/// <param name="arr">Array to which to copy this <see cref="T:System.Configuration.ConfigurationElementCollection" />.</param>
		/// <param name="index">Index location at which to begin copying.</param>
		void ICollection.CopyTo(Array arr, int index)
		{
			this.list.CopyTo(arr, index);
		}

		/// <summary>Gets an <see cref="T:System.Collections.IEnumerator" /> which is used to iterate through the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> which is used to iterate through the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</returns>
		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		/// <summary>Indicates whether the specified <see cref="T:System.Configuration.ConfigurationElement" /> exists in the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</summary>
		/// <param name="elementName">The name of the element to verify.</param>
		/// <returns>
		///   <see langword="true" /> if the element exists in the collection; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		protected virtual bool IsElementName(string elementName)
		{
			return false;
		}

		/// <summary>Indicates whether the specified <see cref="T:System.Configuration.ConfigurationElement" /> can be removed from the <see cref="T:System.Configuration.ConfigurationElementCollection" />.</summary>
		/// <param name="element">The element to check.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Configuration.ConfigurationElement" /> can be removed from this <see cref="T:System.Configuration.ConfigurationElementCollection" />; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		protected virtual bool IsElementRemovable(ConfigurationElement element)
		{
			return !this.IsReadOnly();
		}

		/// <summary>Indicates whether this <see cref="T:System.Configuration.ConfigurationElementCollection" /> has been modified since it was last saved or loaded when overridden in a derived class.</summary>
		/// <returns>
		///   <see langword="true" /> if any contained element has been modified; otherwise, <see langword="false" /></returns>
		protected internal override bool IsModified()
		{
			if (this.modified)
			{
				return true;
			}
			for (int i = 0; i < this.list.Count; i++)
			{
				if (((ConfigurationElement)this.list[i]).IsModified())
				{
					this.modified = true;
					break;
				}
			}
			return this.modified;
		}

		/// <summary>Indicates whether the <see cref="T:System.Configuration.ConfigurationElementCollection" /> object is read only.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.ConfigurationElementCollection" /> object is read only; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public override bool IsReadOnly()
		{
			return base.IsReadOnly();
		}

		internal override void PrepareSave(ConfigurationElement parentElement, ConfigurationSaveMode mode)
		{
			ConfigurationElementCollection configurationElementCollection = (ConfigurationElementCollection)parentElement;
			base.PrepareSave(parentElement, mode);
			for (int i = 0; i < this.list.Count; i++)
			{
				ConfigurationElement configurationElement = (ConfigurationElement)this.list[i];
				object elementKey = this.GetElementKey(configurationElement);
				ConfigurationElement parent = (configurationElementCollection != null) ? configurationElementCollection.BaseGet(elementKey) : null;
				configurationElement.PrepareSave(parent, mode);
			}
		}

		internal override bool HasValues(ConfigurationElement parentElement, ConfigurationSaveMode mode)
		{
			ConfigurationElementCollection configurationElementCollection = (ConfigurationElementCollection)parentElement;
			if (mode == ConfigurationSaveMode.Full)
			{
				return this.list.Count > 0;
			}
			for (int i = 0; i < this.list.Count; i++)
			{
				ConfigurationElement configurationElement = (ConfigurationElement)this.list[i];
				object elementKey = this.GetElementKey(configurationElement);
				ConfigurationElement parent = (configurationElementCollection != null) ? configurationElementCollection.BaseGet(elementKey) : null;
				if (configurationElement.HasValues(parent, mode))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Resets the <see cref="T:System.Configuration.ConfigurationElementCollection" /> to its unmodified state when overridden in a derived class.</summary>
		/// <param name="parentElement">The <see cref="T:System.Configuration.ConfigurationElement" /> representing the collection parent element, if any; otherwise, <see langword="null" />.</param>
		protected internal override void Reset(ConfigurationElement parentElement)
		{
			bool isBasic = this.IsBasic;
			ConfigurationElementCollection configurationElementCollection = (ConfigurationElementCollection)parentElement;
			for (int i = 0; i < configurationElementCollection.Count; i++)
			{
				ConfigurationElement parentElement2 = configurationElementCollection.BaseGet(i);
				ConfigurationElement configurationElement = this.CreateNewElementInternal(null);
				configurationElement.Reset(parentElement2);
				this.BaseAdd(configurationElement);
				if (isBasic)
				{
					if (this.inherited == null)
					{
						this.inherited = new ArrayList();
					}
					this.inherited.Add(configurationElement);
				}
			}
			if (this.IsAlternate)
			{
				this.inheritedLimitIndex = 0;
			}
			else
			{
				this.inheritedLimitIndex = this.Count - 1;
			}
			this.modified = false;
		}

		/// <summary>Resets the value of the <see cref="M:System.Configuration.ConfigurationElementCollection.IsModified" /> property to <see langword="false" /> when overridden in a derived class.</summary>
		protected internal override void ResetModified()
		{
			this.modified = false;
			for (int i = 0; i < this.list.Count; i++)
			{
				((ConfigurationElement)this.list[i]).ResetModified();
			}
		}

		/// <summary>Sets the <see cref="M:System.Configuration.ConfigurationElementCollection.IsReadOnly" /> property for the <see cref="T:System.Configuration.ConfigurationElementCollection" /> object and for all sub-elements.</summary>
		[MonoTODO]
		protected internal override void SetReadOnly()
		{
			base.SetReadOnly();
		}

		/// <summary>Writes the configuration data to an XML element in the configuration file when overridden in a derived class.</summary>
		/// <param name="writer">Output stream that writes XML to the configuration file.</param>
		/// <param name="serializeCollectionKey">
		///   <see langword="true" /> to serialize the collection key; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.ConfigurationElementCollection" /> was written to the configuration file successfully.</returns>
		/// <exception cref="T:System.ArgumentException">One of the elements in the collection was added or replaced and starts with the reserved prefix "config" or "lock".</exception>
		protected internal override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
		{
			if (serializeCollectionKey)
			{
				return base.SerializeElement(writer, serializeCollectionKey);
			}
			bool flag = false;
			if (this.IsBasic)
			{
				for (int i = 0; i < this.list.Count; i++)
				{
					ConfigurationElement configurationElement = (ConfigurationElement)this.list[i];
					if (this.ElementName != string.Empty)
					{
						flag = (configurationElement.SerializeToXmlElement(writer, this.ElementName) || flag);
					}
					else
					{
						flag = (configurationElement.SerializeElement(writer, false) || flag);
					}
				}
			}
			else
			{
				if (this.emitClear)
				{
					writer.WriteElementString(this.clearElementName, "");
					flag = true;
				}
				if (this.removed != null)
				{
					for (int j = 0; j < this.removed.Count; j++)
					{
						writer.WriteStartElement(this.removeElementName);
						((ConfigurationElement)this.removed[j]).SerializeElement(writer, true);
						writer.WriteEndElement();
					}
					flag = (flag || this.removed.Count > 0);
				}
				for (int k = 0; k < this.list.Count; k++)
				{
					((ConfigurationElement)this.list[k]).SerializeToXmlElement(writer, this.addElementName);
				}
				flag = (flag || this.list.Count > 0);
			}
			return flag;
		}

		/// <summary>Causes the configuration system to throw an exception.</summary>
		/// <param name="elementName">The name of the unrecognized element.</param>
		/// <param name="reader">An input stream that reads XML from the configuration file.</param>
		/// <returns>
		///   <see langword="true" /> if the unrecognized element was deserialized successfully; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The element specified in <paramref name="elementName" /> is the <see langword="&lt;clear&gt;" /> element.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="elementName" /> starts with the reserved prefix "config" or "lock".</exception>
		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
		{
			if (this.IsBasic)
			{
				ConfigurationElement configurationElement = null;
				if (elementName == this.ElementName)
				{
					configurationElement = this.CreateNewElementInternal(null);
				}
				if (this.IsElementName(elementName))
				{
					configurationElement = this.CreateNewElementInternal(elementName);
				}
				if (configurationElement != null)
				{
					configurationElement.DeserializeElement(reader, false);
					this.BaseAdd(configurationElement);
					this.modified = false;
					return true;
				}
			}
			else if (elementName == this.clearElementName)
			{
				reader.MoveToContent();
				if (reader.MoveToNextAttribute())
				{
					throw new ConfigurationErrorsException("Unrecognized attribute '" + reader.LocalName + "'.");
				}
				reader.MoveToElement();
				reader.Skip();
				this.BaseClear();
				this.emitClear = true;
				this.modified = false;
				return true;
			}
			else
			{
				if (elementName == this.removeElementName)
				{
					ConfigurationElementCollection.ConfigurationRemoveElement configurationRemoveElement = new ConfigurationElementCollection.ConfigurationRemoveElement(this.CreateNewElementInternal(null), this);
					configurationRemoveElement.DeserializeElement(reader, true);
					this.BaseRemove(configurationRemoveElement.KeyValue);
					this.modified = false;
					return true;
				}
				if (elementName == this.addElementName)
				{
					ConfigurationElement configurationElement2 = this.CreateNewElementInternal(null);
					configurationElement2.DeserializeElement(reader, false);
					this.BaseAdd(configurationElement2);
					this.modified = false;
					return true;
				}
			}
			return false;
		}

		/// <summary>Reverses the effect of merging configuration information from different levels of the configuration hierarchy.</summary>
		/// <param name="sourceElement">A <see cref="T:System.Configuration.ConfigurationElement" /> object at the current level containing a merged view of the properties.</param>
		/// <param name="parentElement">The parent <see cref="T:System.Configuration.ConfigurationElement" /> object of the current element, or <see langword="null" /> if this is the top level.</param>
		/// <param name="saveMode">One of the enumeration value that determines which property values to include.</param>
		protected internal override void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
		{
			ConfigurationElementCollection configurationElementCollection = (ConfigurationElementCollection)sourceElement;
			ConfigurationElementCollection configurationElementCollection2 = (ConfigurationElementCollection)parentElement;
			for (int i = 0; i < configurationElementCollection.Count; i++)
			{
				ConfigurationElement configurationElement = configurationElementCollection.BaseGet(i);
				object elementKey = configurationElementCollection.GetElementKey(configurationElement);
				ConfigurationElement configurationElement2 = (configurationElementCollection2 != null) ? configurationElementCollection2.BaseGet(elementKey) : null;
				ConfigurationElement configurationElement3 = this.CreateNewElementInternal(null);
				if (configurationElement2 != null && saveMode != ConfigurationSaveMode.Full)
				{
					configurationElement3.Unmerge(configurationElement, configurationElement2, saveMode);
					if (configurationElement3.HasValues(configurationElement2, saveMode))
					{
						this.BaseAdd(configurationElement3);
					}
				}
				else
				{
					configurationElement3.Unmerge(configurationElement, null, ConfigurationSaveMode.Full);
					this.BaseAdd(configurationElement3);
				}
			}
			if (saveMode == ConfigurationSaveMode.Full)
			{
				this.EmitClear = true;
				return;
			}
			if (configurationElementCollection2 != null)
			{
				for (int j = 0; j < configurationElementCollection2.Count; j++)
				{
					ConfigurationElement configurationElement4 = configurationElementCollection2.BaseGet(j);
					object elementKey2 = configurationElementCollection2.GetElementKey(configurationElement4);
					if (configurationElementCollection.IndexOfKey(elementKey2) == -1)
					{
						if (this.removed == null)
						{
							this.removed = new ArrayList();
						}
						this.removed.Add(configurationElement4);
					}
				}
			}
		}

		private ArrayList list = new ArrayList();

		private ArrayList removed;

		private ArrayList inherited;

		private bool emitClear;

		private bool modified;

		private IComparer comparer;

		private int inheritedLimitIndex;

		private string addElementName = "add";

		private string clearElementName = "clear";

		private string removeElementName = "remove";

		private sealed class ConfigurationRemoveElement : ConfigurationElement
		{
			internal ConfigurationRemoveElement(ConfigurationElement origElement, ConfigurationElementCollection origCollection)
			{
				this._origElement = origElement;
				this._origCollection = origCollection;
				foreach (object obj in origElement.Properties)
				{
					ConfigurationProperty configurationProperty = (ConfigurationProperty)obj;
					if (configurationProperty.IsKey)
					{
						this.properties.Add(configurationProperty);
					}
				}
			}

			internal object KeyValue
			{
				get
				{
					foreach (object obj in this.Properties)
					{
						ConfigurationProperty prop = (ConfigurationProperty)obj;
						this._origElement[prop] = base[prop];
					}
					return this._origCollection.GetElementKey(this._origElement);
				}
			}

			protected internal override ConfigurationPropertyCollection Properties
			{
				get
				{
					return this.properties;
				}
			}

			private readonly ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

			private readonly ConfigurationElement _origElement;

			private readonly ConfigurationElementCollection _origCollection;
		}
	}
}
