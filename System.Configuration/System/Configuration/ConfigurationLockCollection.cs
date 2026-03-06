using System;
using System.Collections;
using Unity;

namespace System.Configuration
{
	/// <summary>Contains a collection of locked configuration objects. This class cannot be inherited.</summary>
	public sealed class ConfigurationLockCollection : ICollection, IEnumerable
	{
		internal ConfigurationLockCollection(ConfigurationElement element, ConfigurationLockType lockType)
		{
			this.names = new ArrayList();
			this.element = element;
			this.lockType = lockType;
		}

		private void CheckName(string name)
		{
			bool flag = (this.lockType & ConfigurationLockType.Attribute) == ConfigurationLockType.Attribute;
			if (this.valid_name_hash == null)
			{
				this.valid_name_hash = new Hashtable();
				foreach (object obj in this.element.Properties)
				{
					ConfigurationProperty configurationProperty = (ConfigurationProperty)obj;
					if (flag != configurationProperty.IsElement)
					{
						this.valid_name_hash.Add(configurationProperty.Name, true);
					}
				}
				if (!flag)
				{
					ConfigurationElementCollection defaultCollection = this.element.GetDefaultCollection();
					this.valid_name_hash.Add(defaultCollection.AddElementName, true);
					this.valid_name_hash.Add(defaultCollection.ClearElementName, true);
					this.valid_name_hash.Add(defaultCollection.RemoveElementName, true);
				}
				string[] array = new string[this.valid_name_hash.Keys.Count];
				this.valid_name_hash.Keys.CopyTo(array, 0);
				this.valid_names = string.Join(",", array);
			}
			if (this.valid_name_hash[name] == null)
			{
				throw new ConfigurationErrorsException(string.Format("The {2} '{0}' is not valid in the locked list for this section.  The following {3} can be locked: '{1}'", new object[]
				{
					name,
					this.valid_names,
					flag ? "attribute" : "element",
					flag ? "attributes" : "elements"
				}));
			}
		}

		/// <summary>Locks a configuration object by adding it to the collection.</summary>
		/// <param name="name">The name of the configuration object.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">Occurs when the <paramref name="name" /> does not match an existing configuration object within the collection.</exception>
		public void Add(string name)
		{
			this.CheckName(name);
			if (!this.names.Contains(name))
			{
				this.names.Add(name);
				this.is_modified = true;
			}
		}

		/// <summary>Clears all configuration objects from the collection.</summary>
		public void Clear()
		{
			this.names.Clear();
			this.is_modified = true;
		}

		/// <summary>Verifies whether a specific configuration object is locked.</summary>
		/// <param name="name">The name of the configuration object to verify.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.ConfigurationLockCollection" /> contains the specified configuration object; otherwise, <see langword="false" />.</returns>
		public bool Contains(string name)
		{
			return this.names.Contains(name);
		}

		/// <summary>Copies the entire <see cref="T:System.Configuration.ConfigurationLockCollection" /> collection to a compatible one-dimensional <see cref="T:System.Array" />, starting at the specified index of the target array.</summary>
		/// <param name="array">A one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from the <see cref="T:System.Configuration.ConfigurationLockCollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		public void CopyTo(string[] array, int index)
		{
			this.names.CopyTo(array, index);
		}

		/// <summary>Gets an <see cref="T:System.Collections.IEnumerator" /> object, which is used to iterate through this <see cref="T:System.Configuration.ConfigurationLockCollection" /> collection.</summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator" /> object.</returns>
		public IEnumerator GetEnumerator()
		{
			return this.names.GetEnumerator();
		}

		/// <summary>Verifies whether a specific configuration object is read-only.</summary>
		/// <param name="name">The name of the configuration object to verify.</param>
		/// <returns>
		///   <see langword="true" /> if the specified configuration object in the <see cref="T:System.Configuration.ConfigurationLockCollection" /> collection is read-only; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The specified configuration object is not in the collection.</exception>
		[MonoInternalNote("we can't possibly *always* return false here...")]
		public bool IsReadOnly(string name)
		{
			for (int i = 0; i < this.names.Count; i++)
			{
				if ((string)this.names[i] == name)
				{
					return false;
				}
			}
			throw new ConfigurationErrorsException(string.Format("The entry '{0}' is not in the collection.", name));
		}

		/// <summary>Removes a configuration object from the collection.</summary>
		/// <param name="name">The name of the configuration object.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">Occurs when the <paramref name="name" /> does not match an existing configuration object within the collection.</exception>
		public void Remove(string name)
		{
			this.names.Remove(name);
			this.is_modified = true;
		}

		/// <summary>Locks a set of configuration objects based on the supplied list.</summary>
		/// <param name="attributeList">A comma-delimited string.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">Occurs when an item in the <paramref name="attributeList" /> parameter is not a valid lockable configuration attribute.</exception>
		public void SetFromList(string attributeList)
		{
			this.Clear();
			char[] separator = new char[]
			{
				','
			};
			foreach (string text in attributeList.Split(separator))
			{
				this.Add(text.Trim());
			}
		}

		/// <summary>Copies the entire <see cref="T:System.Configuration.ConfigurationLockCollection" /> collection to a compatible one-dimensional <see cref="T:System.Array" />, starting at the specified index of the target array.</summary>
		/// <param name="array">A one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from the <see cref="T:System.Configuration.ConfigurationLockCollection" /> collection. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			this.names.CopyTo(array, index);
		}

		/// <summary>Gets a list of configuration objects contained in the collection.</summary>
		/// <returns>A comma-delimited string that lists the lock configuration objects in the collection.</returns>
		public string AttributeList
		{
			get
			{
				string[] array = new string[this.names.Count];
				this.names.CopyTo(array, 0);
				return string.Join(",", array);
			}
		}

		/// <summary>Gets the number of locked configuration objects contained in the collection.</summary>
		/// <returns>The number of locked configuration objects contained in the collection.</returns>
		public int Count
		{
			get
			{
				return this.names.Count;
			}
		}

		/// <summary>Gets a value specifying whether the collection of locked objects has parent elements.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.ConfigurationLockCollection" /> collection has parent elements; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public bool HasParentElements
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value specifying whether the collection has been modified.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.ConfigurationLockCollection" /> collection has been modified; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public bool IsModified
		{
			get
			{
				return this.is_modified;
			}
			internal set
			{
				this.is_modified = value;
			}
		}

		/// <summary>Gets a value specifying whether the collection is synchronized.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Configuration.ConfigurationLockCollection" /> collection is synchronized; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets an object used to synchronize access to this <see cref="T:System.Configuration.ConfigurationLockCollection" /> collection.</summary>
		/// <returns>An object used to synchronize access to this <see cref="T:System.Configuration.ConfigurationLockCollection" /> collection.</returns>
		[MonoTODO]
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		internal ConfigurationLockCollection()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private ArrayList names;

		private ConfigurationElement element;

		private ConfigurationLockType lockType;

		private bool is_modified;

		private Hashtable valid_name_hash;

		private string valid_names;
	}
}
