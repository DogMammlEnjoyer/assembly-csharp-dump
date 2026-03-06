using System;
using System.Globalization;

namespace System.Configuration
{
	/// <summary>Contains a collection of <see cref="T:System.Configuration.ConnectionStringSettings" /> objects.</summary>
	[ConfigurationCollection(typeof(ConnectionStringSettings), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public sealed class ConnectionStringSettingsCollection : ConfigurationElementCollection
	{
		/// <summary>Gets or sets the <see cref="T:System.Configuration.ConnectionStringSettings" /> object with the specified name in the collection.</summary>
		/// <param name="name">The name of a <see cref="T:System.Configuration.ConnectionStringSettings" /> object in the collection.</param>
		/// <returns>The <see cref="T:System.Configuration.ConnectionStringSettings" /> object with the specified name; otherwise, <see langword="null" />.</returns>
		public ConnectionStringSettings this[string name]
		{
			get
			{
				foreach (object obj in this)
				{
					ConfigurationElement configurationElement = (ConfigurationElement)obj;
					if (configurationElement is ConnectionStringSettings && string.Compare(((ConnectionStringSettings)configurationElement).Name, name, true, CultureInfo.InvariantCulture) == 0)
					{
						return configurationElement as ConnectionStringSettings;
					}
				}
				return null;
			}
		}

		/// <summary>Gets or sets the connection string at the specified index in the collection.</summary>
		/// <param name="index">The index of a <see cref="T:System.Configuration.ConnectionStringSettings" /> object in the collection.</param>
		/// <returns>The <see cref="T:System.Configuration.ConnectionStringSettings" /> object at the specified index.</returns>
		public ConnectionStringSettings this[int index]
		{
			get
			{
				return (ConnectionStringSettings)base.BaseGet(index);
			}
			set
			{
				if (base.BaseGet(index) != null)
				{
					base.BaseRemoveAt(index);
				}
				this.BaseAdd(index, value);
			}
		}

		[MonoTODO]
		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return base.Properties;
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new ConnectionStringSettings();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ConnectionStringSettings)element).Name;
		}

		/// <summary>Adds a <see cref="T:System.Configuration.ConnectionStringSettings" /> object to the collection.</summary>
		/// <param name="settings">A <see cref="T:System.Configuration.ConnectionStringSettings" /> object to add to the collection.</param>
		public void Add(ConnectionStringSettings settings)
		{
			this.BaseAdd(settings);
		}

		/// <summary>Removes all the <see cref="T:System.Configuration.ConnectionStringSettings" /> objects from the collection.</summary>
		public void Clear()
		{
			base.BaseClear();
		}

		/// <summary>Returns the collection index of the passed <see cref="T:System.Configuration.ConnectionStringSettings" /> object.</summary>
		/// <param name="settings">A <see cref="T:System.Configuration.ConnectionStringSettings" /> object in the collection.</param>
		/// <returns>The collection index of the specified <see cref="T:System.Configuration.ConnectionStringSettingsCollection" /> object.</returns>
		public int IndexOf(ConnectionStringSettings settings)
		{
			return base.BaseIndexOf(settings);
		}

		/// <summary>Removes the specified <see cref="T:System.Configuration.ConnectionStringSettings" /> object from the collection.</summary>
		/// <param name="settings">A <see cref="T:System.Configuration.ConnectionStringSettings" /> object in the collection.</param>
		public void Remove(ConnectionStringSettings settings)
		{
			base.BaseRemove(settings.Name);
		}

		/// <summary>Removes the specified <see cref="T:System.Configuration.ConnectionStringSettings" /> object from the collection.</summary>
		/// <param name="name">The name of a <see cref="T:System.Configuration.ConnectionStringSettings" /> object in the collection.</param>
		public void Remove(string name)
		{
			base.BaseRemove(name);
		}

		/// <summary>Removes the <see cref="T:System.Configuration.ConnectionStringSettings" /> object at the specified index in the collection.</summary>
		/// <param name="index">The index of a <see cref="T:System.Configuration.ConnectionStringSettings" /> object in the collection.</param>
		public void RemoveAt(int index)
		{
			base.BaseRemoveAt(index);
		}

		protected override void BaseAdd(int index, ConfigurationElement element)
		{
			if (!(element is ConnectionStringSettings))
			{
				base.BaseAdd(element);
			}
			if (this.IndexOf((ConnectionStringSettings)element) >= 0)
			{
				throw new ConfigurationErrorsException(string.Format("The element {0} already exist!", ((ConnectionStringSettings)element).Name));
			}
			this[index] = (ConnectionStringSettings)element;
		}
	}
}
