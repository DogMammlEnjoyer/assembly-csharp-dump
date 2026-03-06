using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.Xml.Serialization
{
	/// <summary>Describes the context in which a set of schema is bound to .NET Framework code entities.</summary>
	public class ImportContext
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Xml.Serialization.ImportContext" /> class for the given code identifiers, with the given type-sharing option.</summary>
		/// <param name="identifiers">The code entities to which the context applies.</param>
		/// <param name="shareTypes">A <see cref="T:System.Boolean" /> value that determines whether custom types are shared among schema.</param>
		public ImportContext(CodeIdentifiers identifiers, bool shareTypes)
		{
			this.typeIdentifiers = identifiers;
			this.shareTypes = shareTypes;
		}

		internal ImportContext() : this(null, false)
		{
		}

		internal SchemaObjectCache Cache
		{
			get
			{
				if (this.cache == null)
				{
					this.cache = new SchemaObjectCache();
				}
				return this.cache;
			}
		}

		internal Hashtable Elements
		{
			get
			{
				if (this.elements == null)
				{
					this.elements = new Hashtable();
				}
				return this.elements;
			}
		}

		internal Hashtable Mappings
		{
			get
			{
				if (this.mappings == null)
				{
					this.mappings = new Hashtable();
				}
				return this.mappings;
			}
		}

		/// <summary>Gets a set of code entities to which the context applies.</summary>
		/// <returns>A <see cref="T:System.Xml.Serialization.CodeIdentifiers" /> that specifies the code entities to which the context applies.</returns>
		public CodeIdentifiers TypeIdentifiers
		{
			get
			{
				if (this.typeIdentifiers == null)
				{
					this.typeIdentifiers = new CodeIdentifiers();
				}
				return this.typeIdentifiers;
			}
		}

		/// <summary>Gets a value that determines whether custom types are shared.</summary>
		/// <returns>
		///     <see langword="true" />, if custom types are shared among schema; otherwise, <see langword="false" />.</returns>
		public bool ShareTypes
		{
			get
			{
				return this.shareTypes;
			}
		}

		/// <summary>Gets a collection of warnings that are generated when importing the code entity descriptions.</summary>
		/// <returns>A <see cref="T:System.Collections.Specialized.StringCollection" /> that contains warnings that were generated when importing the code entity descriptions.</returns>
		public StringCollection Warnings
		{
			get
			{
				return this.Cache.Warnings;
			}
		}

		private bool shareTypes;

		private SchemaObjectCache cache;

		private Hashtable mappings;

		private Hashtable elements;

		private CodeIdentifiers typeIdentifiers;
	}
}
