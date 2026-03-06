using System;
using System.Collections;

namespace System.Xml.Serialization.Advanced
{
	/// <summary>Represents a collection of <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtension" /> objects.</summary>
	public class SchemaImporterExtensionCollection : CollectionBase
	{
		internal Hashtable Names
		{
			get
			{
				if (this.exNames == null)
				{
					this.exNames = new Hashtable();
				}
				return this.exNames;
			}
		}

		/// <summary>Adds the specified importer extension to the collection.</summary>
		/// <param name="extension">The <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtensionCollection" /> to add.</param>
		/// <returns>The index of the added extension.</returns>
		public int Add(SchemaImporterExtension extension)
		{
			return this.Add(extension.GetType().FullName, extension);
		}

		/// <summary>Adds the specified importer extension to the collection. The name parameter allows you to supply a custom name for the extension.</summary>
		/// <param name="name">A custom name for the extension.</param>
		/// <param name="type">The <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtensionCollection" /> to add.</param>
		/// <returns>The index of the newly added item.</returns>
		/// <exception cref="T:System.ArgumentException">The value of type does not inherit from <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtensionCollection" />.</exception>
		public int Add(string name, Type type)
		{
			if (type.IsSubclassOf(typeof(SchemaImporterExtension)))
			{
				return this.Add(name, (SchemaImporterExtension)Activator.CreateInstance(type));
			}
			throw new ArgumentException(Res.GetString("'{0}' is not a valid SchemaExtensionType.", new object[]
			{
				type
			}));
		}

		/// <summary>Removes the <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtension" />, specified by name, from the collection.</summary>
		/// <param name="name">The name of the <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtension" /> to remove. The name is set using the <see cref="M:System.Xml.Serialization.Advanced.SchemaImporterExtensionCollection.Add(System.String,System.Type)" /> method.</param>
		public void Remove(string name)
		{
			if (this.Names[name] != null)
			{
				base.List.Remove(this.Names[name]);
				this.Names[name] = null;
			}
		}

		/// <summary>Clears the collection of importer extensions.</summary>
		public new void Clear()
		{
			this.Names.Clear();
			base.List.Clear();
		}

		internal SchemaImporterExtensionCollection Clone()
		{
			SchemaImporterExtensionCollection schemaImporterExtensionCollection = new SchemaImporterExtensionCollection();
			schemaImporterExtensionCollection.exNames = (Hashtable)this.Names.Clone();
			foreach (object value in base.List)
			{
				schemaImporterExtensionCollection.List.Add(value);
			}
			return schemaImporterExtensionCollection;
		}

		/// <summary>Gets the <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtensionCollection" /> at the specified index.</summary>
		/// <param name="index">The index of the item to find.</param>
		/// <returns>The <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtensionCollection" /> at the specified index.</returns>
		public SchemaImporterExtension this[int index]
		{
			get
			{
				return (SchemaImporterExtension)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		internal int Add(string name, SchemaImporterExtension extension)
		{
			if (this.Names[name] == null)
			{
				this.Names[name] = extension;
				return base.List.Add(extension);
			}
			if (this.Names[name].GetType() != extension.GetType())
			{
				throw new InvalidOperationException(Res.GetString("Duplicate extension name.  schemaImporterExtension with name '{0}' already been added.", new object[]
				{
					name
				}));
			}
			return -1;
		}

		/// <summary>Inserts the specified <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtension" /> into the collection at the specified index.</summary>
		/// <param name="index">The zero-base index at which the <paramref name="extension" /> should be inserted.</param>
		/// <param name="extension">The <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtension" /> to insert.</param>
		public void Insert(int index, SchemaImporterExtension extension)
		{
			base.List.Insert(index, extension);
		}

		/// <summary>Searches for the specified item and returns the zero-based index of the first occurrence within the collection.</summary>
		/// <param name="extension">The <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtension" /> to search for.</param>
		/// <returns>The index of the found item.</returns>
		public int IndexOf(SchemaImporterExtension extension)
		{
			return base.List.IndexOf(extension);
		}

		/// <summary>Gets a value that indicates whether the specified importer extension exists in the collection.</summary>
		/// <param name="extension">The <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtensionCollection" /> to search for.</param>
		/// <returns>
		///     <see langword="true" /> if the extension is found; otherwise, <see langword="false" />.</returns>
		public bool Contains(SchemaImporterExtension extension)
		{
			return base.List.Contains(extension);
		}

		/// <summary>Removes the specified <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtension" /> from the collection.</summary>
		/// <param name="extension">The <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtension" /> to remove. </param>
		public void Remove(SchemaImporterExtension extension)
		{
			base.List.Remove(extension);
		}

		/// <summary>Copies all the elements of the current <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtensionCollection" /> to the specified array of <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtension" /> objects at the specified index. </summary>
		/// <param name="array">The <see cref="T:System.Xml.Serialization.Advanced.SchemaImporterExtension" /> to copy the current collection to.</param>
		/// <param name="index">The zero-based index at which the collection is added.</param>
		public void CopyTo(SchemaImporterExtension[] array, int index)
		{
			base.List.CopyTo(array, index);
		}

		private Hashtable exNames;
	}
}
