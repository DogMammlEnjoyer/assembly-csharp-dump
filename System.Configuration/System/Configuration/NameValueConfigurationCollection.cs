using System;

namespace System.Configuration
{
	/// <summary>Contains a collection of <see cref="T:System.Configuration.NameValueConfigurationElement" /> objects. This class cannot be inherited.</summary>
	[ConfigurationCollection(typeof(NameValueConfigurationElement), AddItemName = "add", RemoveItemName = "remove", ClearItemsName = "clear", CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public sealed class NameValueConfigurationCollection : ConfigurationElementCollection
	{
		/// <summary>Gets the keys to all items contained in the <see cref="T:System.Configuration.NameValueConfigurationCollection" />.</summary>
		/// <returns>A string array.</returns>
		public string[] AllKeys
		{
			get
			{
				return (string[])base.BaseGetAllKeys();
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Configuration.NameValueConfigurationElement" /> object based on the supplied parameter.</summary>
		/// <param name="name">The name of the <see cref="T:System.Configuration.NameValueConfigurationElement" /> contained in the collection.</param>
		/// <returns>A <see cref="T:System.Configuration.NameValueConfigurationElement" /> object.</returns>
		public NameValueConfigurationElement this[string name]
		{
			get
			{
				return (NameValueConfigurationElement)base.BaseGet(name);
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		protected internal override ConfigurationPropertyCollection Properties
		{
			get
			{
				return NameValueConfigurationCollection.properties;
			}
		}

		/// <summary>Adds a <see cref="T:System.Configuration.NameValueConfigurationElement" /> object to the collection.</summary>
		/// <param name="nameValue">A  <see cref="T:System.Configuration.NameValueConfigurationElement" /> object.</param>
		public void Add(NameValueConfigurationElement nameValue)
		{
			base.BaseAdd(nameValue, false);
		}

		/// <summary>Clears the <see cref="T:System.Configuration.NameValueConfigurationCollection" />.</summary>
		public void Clear()
		{
			base.BaseClear();
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new NameValueConfigurationElement("", "");
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((NameValueConfigurationElement)element).Name;
		}

		/// <summary>Removes a <see cref="T:System.Configuration.NameValueConfigurationElement" /> object from the collection based on the provided parameter.</summary>
		/// <param name="nameValue">A <see cref="T:System.Configuration.NameValueConfigurationElement" /> object.</param>
		public void Remove(NameValueConfigurationElement nameValue)
		{
			throw new NotImplementedException();
		}

		/// <summary>Removes a <see cref="T:System.Configuration.NameValueConfigurationElement" /> object from the collection based on the provided parameter.</summary>
		/// <param name="name">The name of the <see cref="T:System.Configuration.NameValueConfigurationElement" /> object.</param>
		public void Remove(string name)
		{
			base.BaseRemove(name);
		}

		private static ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
	}
}
