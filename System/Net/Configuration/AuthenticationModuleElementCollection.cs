using System;
using System.Configuration;

namespace System.Net.Configuration
{
	/// <summary>Represents a container for authentication module configuration elements. This class cannot be inherited.</summary>
	[ConfigurationCollection(typeof(AuthenticationModuleElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public sealed class AuthenticationModuleElementCollection : ConfigurationElementCollection
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.AuthenticationModuleElementCollection" /> class.</summary>
		[MonoTODO]
		public AuthenticationModuleElementCollection()
		{
		}

		/// <summary>Gets or sets the element at the specified position in the collection.</summary>
		/// <param name="index">The zero-based index of the element.</param>
		/// <returns>The <see cref="T:System.Net.Configuration.AuthenticationModuleElement" /> at the specified location.</returns>
		[MonoTODO]
		public AuthenticationModuleElement this[int index]
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
		/// <returns>The <see cref="T:System.Net.Configuration.AuthenticationModuleElement" /> with the specified key or <see langword="null" /> if there is no element with the specified key.</returns>
		[MonoTODO]
		public AuthenticationModuleElement this[string name]
		{
			get
			{
				return (AuthenticationModuleElement)base[name];
			}
			set
			{
				base[name] = value;
			}
		}

		/// <summary>Adds an element to the collection.</summary>
		/// <param name="element">The <see cref="T:System.Net.Configuration.AuthenticationModuleElement" /> to add to the collection.</param>
		public void Add(AuthenticationModuleElement element)
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
			return new AuthenticationModuleElement();
		}

		[MonoTODO("argument exception?")]
		protected override object GetElementKey(ConfigurationElement element)
		{
			if (!(element is AuthenticationModuleElement))
			{
				throw new ArgumentException("element");
			}
			return ((AuthenticationModuleElement)element).Type;
		}

		/// <summary>Returns the index of the specified configuration element.</summary>
		/// <param name="element">A <see cref="T:System.Net.Configuration.AuthenticationModuleElement" />.</param>
		/// <returns>The zero-based index of <paramref name="element" />.</returns>
		public int IndexOf(AuthenticationModuleElement element)
		{
			return base.BaseIndexOf(element);
		}

		/// <summary>Removes the specified configuration element from the collection.</summary>
		/// <param name="element">The <see cref="T:System.Net.Configuration.AuthenticationModuleElement" /> to remove.</param>
		public void Remove(AuthenticationModuleElement element)
		{
			base.BaseRemove(element);
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
