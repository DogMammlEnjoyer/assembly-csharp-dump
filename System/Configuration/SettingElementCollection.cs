using System;

namespace System.Configuration
{
	/// <summary>Contains a collection of <see cref="T:System.Configuration.SettingElement" /> objects. This class cannot be inherited.</summary>
	public sealed class SettingElementCollection : ConfigurationElementCollection
	{
		/// <summary>Adds a <see cref="T:System.Configuration.SettingElement" /> object to the collection.</summary>
		/// <param name="element">The <see cref="T:System.Configuration.SettingElement" /> object to add to the collection.</param>
		public void Add(SettingElement element)
		{
			this.BaseAdd(element);
		}

		/// <summary>Removes all <see cref="T:System.Configuration.SettingElement" /> objects from the collection.</summary>
		public void Clear()
		{
			base.BaseClear();
		}

		/// <summary>Gets a <see cref="T:System.Configuration.SettingElement" /> object from the collection.</summary>
		/// <param name="elementKey">A string value representing the <see cref="T:System.Configuration.SettingElement" /> object in the collection.</param>
		/// <returns>A <see cref="T:System.Configuration.SettingElement" /> object.</returns>
		public SettingElement Get(string elementKey)
		{
			foreach (object obj in this)
			{
				SettingElement settingElement = (SettingElement)obj;
				if (settingElement.Name == elementKey)
				{
					return settingElement;
				}
			}
			return null;
		}

		/// <summary>Removes a <see cref="T:System.Configuration.SettingElement" /> object from the collection.</summary>
		/// <param name="element">A <see cref="T:System.Configuration.SettingElement" /> object.</param>
		public void Remove(SettingElement element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			base.BaseRemove(element.Name);
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new SettingElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SettingElement)element).Name;
		}

		/// <summary>Gets the type of the configuration collection.</summary>
		/// <returns>The <see cref="T:System.Configuration.ConfigurationElementCollectionType" /> object of the collection.</returns>
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.BasicMap;
			}
		}

		protected override string ElementName
		{
			get
			{
				return "setting";
			}
		}
	}
}
