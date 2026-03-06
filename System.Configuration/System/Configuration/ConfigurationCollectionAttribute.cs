using System;

namespace System.Configuration
{
	/// <summary>Declaratively instructs the .NET Framework to create an instance of a configuration element collection. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public sealed class ConfigurationCollectionAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.ConfigurationCollectionAttribute" /> class.</summary>
		/// <param name="itemType">The type of the property collection to create.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="itemType" /> is <see langword="null" />.</exception>
		public ConfigurationCollectionAttribute(Type itemType)
		{
			this.itemType = itemType;
		}

		/// <summary>Gets or sets the name of the <see langword="&lt;add&gt;" /> configuration element.</summary>
		/// <returns>The name that substitutes the standard name "add" for the configuration item.</returns>
		public string AddItemName
		{
			get
			{
				return this.addItemName;
			}
			set
			{
				this.addItemName = value;
			}
		}

		/// <summary>Gets or sets the name for the <see langword="&lt;clear&gt;" /> configuration element.</summary>
		/// <returns>The name that replaces the standard name "clear" for the configuration item.</returns>
		public string ClearItemsName
		{
			get
			{
				return this.clearItemsName;
			}
			set
			{
				this.clearItemsName = value;
			}
		}

		/// <summary>Gets or sets the name for the <see langword="&lt;remove&gt;" /> configuration element.</summary>
		/// <returns>The name that replaces the standard name "remove" for the configuration element.</returns>
		public string RemoveItemName
		{
			get
			{
				return this.removeItemName;
			}
			set
			{
				this.removeItemName = value;
			}
		}

		/// <summary>Gets or sets the type of the <see cref="T:System.Configuration.ConfigurationCollectionAttribute" /> attribute.</summary>
		/// <returns>The type of the <see cref="T:System.Configuration.ConfigurationCollectionAttribute" />.</returns>
		public ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return this.collectionType;
			}
			set
			{
				this.collectionType = value;
			}
		}

		/// <summary>Gets the type of the collection element.</summary>
		/// <returns>The type of the collection element.</returns>
		[MonoInternalNote("Do something with this in ConfigurationElementCollection")]
		public Type ItemType
		{
			get
			{
				return this.itemType;
			}
		}

		private string addItemName = "add";

		private string clearItemsName = "clear";

		private string removeItemName = "remove";

		private ConfigurationElementCollectionType collectionType;

		private Type itemType;
	}
}
