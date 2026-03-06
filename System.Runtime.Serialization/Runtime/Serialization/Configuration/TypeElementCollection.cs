using System;
using System.Configuration;

namespace System.Runtime.Serialization.Configuration
{
	/// <summary>Handles the XML elements used to configure the known types used for serialization by the <see cref="T:System.Runtime.Serialization.DataContractSerializer" />.</summary>
	[ConfigurationCollection(typeof(TypeElement), CollectionType = ConfigurationElementCollectionType.BasicMap)]
	public sealed class TypeElementCollection : ConfigurationElementCollection
	{
		/// <summary>Returns a specific member of the collection by its position.</summary>
		/// <param name="index">The position of the item to return.</param>
		/// <returns>The element at the specified position.</returns>
		public TypeElement this[int index]
		{
			get
			{
				return (TypeElement)base.BaseGet(index);
			}
			set
			{
				if (!this.IsReadOnly())
				{
					if (value == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
					}
					if (base.BaseGet(index) != null)
					{
						base.BaseRemoveAt(index);
					}
				}
				this.BaseAdd(index, value);
			}
		}

		/// <summary>Adds the specified element to the collection.</summary>
		/// <param name="element">A <see cref="T:System.Runtime.Serialization.Configuration.TypeElement" /> that represents the known type to add.</param>
		public void Add(TypeElement element)
		{
			if (!this.IsReadOnly() && element == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
			}
			this.BaseAdd(element);
		}

		/// <summary>Removes all members of the collection.</summary>
		public void Clear()
		{
			base.BaseClear();
		}

		/// <summary>Gets the collection of elements that represents the types using known types.</summary>
		/// <returns>A <see cref="T:System.Configuration.ConfigurationElementCollectionType" /> that contains the element objects.</returns>
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.BasicMap;
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new TypeElement();
		}

		protected override string ElementName
		{
			get
			{
				return "knownType";
			}
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			if (element == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
			}
			return ((TypeElement)element).Key;
		}

		/// <summary>Returns the position of the specified element.</summary>
		/// <param name="element">The <see cref="T:System.Runtime.Serialization.Configuration.TypeElement" /> to find in the collection.</param>
		/// <returns>The position of the specified element.</returns>
		public int IndexOf(TypeElement element)
		{
			if (element == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
			}
			return base.BaseIndexOf(element);
		}

		/// <summary>Removes the specified element from the collection.</summary>
		/// <param name="element">The <see cref="T:System.Runtime.Serialization.Configuration.TypeElement" /> to remove.</param>
		public void Remove(TypeElement element)
		{
			if (!this.IsReadOnly() && element == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
			}
			base.BaseRemove(this.GetElementKey(element));
		}

		/// <summary>Removes the element at the specified position.</summary>
		/// <param name="index">The position in the collection from which to remove the element.</param>
		public void RemoveAt(int index)
		{
			base.BaseRemoveAt(index);
		}

		private const string KnownTypeConfig = "knownType";
	}
}
