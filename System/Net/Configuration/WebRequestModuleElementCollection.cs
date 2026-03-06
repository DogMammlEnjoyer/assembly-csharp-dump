using System;
using System.Configuration;

namespace System.Net.Configuration
{
	/// <summary>Represents a container for Web request module configuration elements. This class cannot be inherited.</summary>
	[ConfigurationCollection(typeof(WebRequestModuleElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public sealed class WebRequestModuleElementCollection : ConfigurationElementCollection
	{
		/// <summary>Gets or sets the element at the specified position in the collection.</summary>
		/// <param name="index">The zero-based index of the element.</param>
		/// <returns>The <see cref="T:System.Net.Configuration.WebRequestModuleElement" /> at the specified location.</returns>
		[MonoTODO]
		public WebRequestModuleElement this[int index]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets or sets the element with the specified key.</summary>
		/// <param name="name">The key for an element in the collection.</param>
		/// <returns>The <see cref="T:System.Net.Configuration.WebRequestModuleElement" /> with the specified key or <see langword="null" /> if there is no element with the specified key.</returns>
		[MonoTODO]
		public WebRequestModuleElement this[string name]
		{
			get
			{
				return (WebRequestModuleElement)base[name];
			}
			set
			{
				base[name] = value;
			}
		}

		/// <summary>Adds an element to the collection.</summary>
		/// <param name="element">The <see cref="T:System.Net.Configuration.WebRequestModuleElement" /> to add to the collection.</param>
		public void Add(WebRequestModuleElement element)
		{
			this.BaseAdd(element);
		}

		/// <summary>Removes all elements from the collection.</summary>
		public void Clear()
		{
			base.BaseClear();
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new WebRequestModuleElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			if (!(element is WebRequestModuleElement))
			{
				throw new ArgumentException("element");
			}
			return ((WebRequestModuleElement)element).Prefix;
		}

		/// <summary>Returns the index of the specified configuration element.</summary>
		/// <param name="element">A <see cref="T:System.Net.Configuration.WebRequestModuleElement" />.</param>
		/// <returns>The zero-based index of <paramref name="element" />.</returns>
		public int IndexOf(WebRequestModuleElement element)
		{
			return base.BaseIndexOf(element);
		}

		/// <summary>Removes the specified configuration element from the collection.</summary>
		/// <param name="element">The <see cref="T:System.Net.Configuration.WebRequestModuleElement" /> to remove.</param>
		public void Remove(WebRequestModuleElement element)
		{
			base.BaseRemove(element.Prefix);
		}

		/// <summary>Removes the element with the specified key.</summary>
		/// <param name="name">The key of the element to remove.</param>
		public void Remove(string name)
		{
			base.BaseRemove(name);
		}

		/// <summary>Removes the element at the specified index.</summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		public void RemoveAt(int index)
		{
			base.BaseRemoveAt(index);
		}
	}
}
