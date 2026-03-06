using System;
using System.Configuration;

namespace System.Runtime.Serialization.Configuration
{
	/// <summary>Handles the XML elements used to configure XML serialization using the <see cref="T:System.Runtime.Serialization.DataContractSerializer" />.</summary>
	[ConfigurationCollection(typeof(DeclaredTypeElement))]
	public sealed class DeclaredTypeElementCollection : ConfigurationElementCollection
	{
		/// <summary>Gets or sets the configuration element at the specified index location.</summary>
		/// <param name="index">The index location of the configuration element to return.</param>
		/// <returns>The <see cref="T:System.Runtime.Serialization.Configuration.DeclaredTypeElement" /> at the specified index.</returns>
		public DeclaredTypeElement this[int index]
		{
			get
			{
				return (DeclaredTypeElement)base.BaseGet(index);
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

		/// <summary>Gets or sets the element in the collection of types by its key.</summary>
		/// <param name="typeName">The name (that functions as a key) of the type to get or set.</param>
		/// <returns>The specified element (when used to get the element).</returns>
		public DeclaredTypeElement this[string typeName]
		{
			get
			{
				if (string.IsNullOrEmpty(typeName))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
				}
				return (DeclaredTypeElement)base.BaseGet(typeName);
			}
			set
			{
				if (!this.IsReadOnly())
				{
					if (string.IsNullOrEmpty(typeName))
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
					}
					if (value == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
					}
					if (base.BaseGet(typeName) == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new IndexOutOfRangeException(SR.GetString("For type '{0}', configuration index is out of range.", new object[]
						{
							typeName
						})));
					}
					base.BaseRemove(typeName);
				}
				this.Add(value);
			}
		}

		/// <summary>Adds a specified configuration element to the collection.</summary>
		/// <param name="element">The configuration element to add.</param>
		public void Add(DeclaredTypeElement element)
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

		/// <summary>Returns a value that specifies whether the element is in the collection.</summary>
		/// <param name="typeName">The name of the type to check for.</param>
		/// <returns>
		///   <see langword="true" /> if the element is in the collection; otherwise, <see langword="false" />.</returns>
		public bool Contains(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
			}
			return base.BaseGet(typeName) != null;
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new DeclaredTypeElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			if (element == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
			}
			return ((DeclaredTypeElement)element).Type;
		}

		/// <summary>Returns the position of the specified configuration element.</summary>
		/// <param name="element">The element to find in the collection.</param>
		/// <returns>The index of the specified configuration element; otherwise, -1.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="element" /> argument is <see langword="null" />.</exception>
		public int IndexOf(DeclaredTypeElement element)
		{
			if (element == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
			}
			return base.BaseIndexOf(element);
		}

		/// <summary>Removes the specified configuration element from the collection.</summary>
		/// <param name="element">The <see cref="T:System.Runtime.Serialization.Configuration.DeclaredTypeElement" /> to remove.</param>
		public void Remove(DeclaredTypeElement element)
		{
			if (!this.IsReadOnly() && element == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
			}
			base.BaseRemove(this.GetElementKey(element));
		}

		/// <summary>Removes the element specified by its key from the collection.</summary>
		/// <param name="typeName">The name of the type (which functions as a key) to remove from the collection.</param>
		public void Remove(string typeName)
		{
			if (!this.IsReadOnly() && string.IsNullOrEmpty(typeName))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("typeName");
			}
			base.BaseRemove(typeName);
		}

		/// <summary>Removes the configuration element found at the specified position.</summary>
		/// <param name="index">The position of the configuration element to remove.</param>
		public void RemoveAt(int index)
		{
			base.BaseRemoveAt(index);
		}
	}
}
