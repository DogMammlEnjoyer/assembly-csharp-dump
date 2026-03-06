using System;

namespace System.Runtime.Serialization
{
	/// <summary>When applied to a collection type, enables custom specification of the collection item elements. This attribute can be applied only to types that are recognized by the <see cref="T:System.Runtime.Serialization.DataContractSerializer" /> as valid, serializable collections.</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	public sealed class CollectionDataContractAttribute : Attribute
	{
		/// <summary>Gets or sets the namespace for the data contract.</summary>
		/// <returns>The namespace of the data contract.</returns>
		public string Namespace
		{
			get
			{
				return this.ns;
			}
			set
			{
				this.ns = value;
				this.isNamespaceSetExplicitly = true;
			}
		}

		/// <summary>Gets whether <see cref="P:System.Runtime.Serialization.CollectionDataContractAttribute.Namespace" /> has been explicitly set.</summary>
		/// <returns>
		///   <see langword="true" /> if the item namespace has been explicitly set; otherwise, <see langword="false" />.</returns>
		public bool IsNamespaceSetExplicitly
		{
			get
			{
				return this.isNamespaceSetExplicitly;
			}
		}

		/// <summary>Gets or sets the data contract name for the collection type.</summary>
		/// <returns>The data contract name for the collection type.</returns>
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				this.isNameSetExplicitly = true;
			}
		}

		/// <summary>Gets whether <see cref="P:System.Runtime.Serialization.CollectionDataContractAttribute.Name" /> has been explicitly set.</summary>
		/// <returns>
		///   <see langword="true" /> if the name has been explicitly set; otherwise, <see langword="false" />.</returns>
		public bool IsNameSetExplicitly
		{
			get
			{
				return this.isNameSetExplicitly;
			}
		}

		/// <summary>Gets or sets a custom name for a collection element.</summary>
		/// <returns>The name to apply to collection elements.</returns>
		public string ItemName
		{
			get
			{
				return this.itemName;
			}
			set
			{
				this.itemName = value;
				this.isItemNameSetExplicitly = true;
			}
		}

		/// <summary>Gets whether <see cref="P:System.Runtime.Serialization.CollectionDataContractAttribute.ItemName" /> has been explicitly set.</summary>
		/// <returns>
		///   <see langword="true" /> if the item name has been explicitly set; otherwise, <see langword="false" />.</returns>
		public bool IsItemNameSetExplicitly
		{
			get
			{
				return this.isItemNameSetExplicitly;
			}
		}

		/// <summary>Gets or sets the custom name for a dictionary key name.</summary>
		/// <returns>The name to use instead of the default dictionary key name.</returns>
		public string KeyName
		{
			get
			{
				return this.keyName;
			}
			set
			{
				this.keyName = value;
				this.isKeyNameSetExplicitly = true;
			}
		}

		/// <summary>Gets or sets a value that indicates whether to preserve object reference data.</summary>
		/// <returns>
		///   <see langword="true" /> to keep object reference data; otherwise, <see langword="false" />. The default is <see langword="false" />.</returns>
		public bool IsReference
		{
			get
			{
				return this.isReference;
			}
			set
			{
				this.isReference = value;
				this.isReferenceSetExplicitly = true;
			}
		}

		/// <summary>Gets whether reference has been explicitly set.</summary>
		/// <returns>
		///   <see langword="true" /> if the reference has been explicitly set; otherwise, <see langword="false" />.</returns>
		public bool IsReferenceSetExplicitly
		{
			get
			{
				return this.isReferenceSetExplicitly;
			}
		}

		/// <summary>Gets whether <see cref="P:System.Runtime.Serialization.CollectionDataContractAttribute.KeyName" /> has been explicitly set.</summary>
		/// <returns>
		///   <see langword="true" /> if the key name has been explicitly set; otherwise, <see langword="false" />.</returns>
		public bool IsKeyNameSetExplicitly
		{
			get
			{
				return this.isKeyNameSetExplicitly;
			}
		}

		/// <summary>Gets or sets the custom name for a dictionary value name.</summary>
		/// <returns>The name to use instead of the default dictionary value name.</returns>
		public string ValueName
		{
			get
			{
				return this.valueName;
			}
			set
			{
				this.valueName = value;
				this.isValueNameSetExplicitly = true;
			}
		}

		/// <summary>Gets whether <see cref="P:System.Runtime.Serialization.CollectionDataContractAttribute.ValueName" /> has been explicitly set.</summary>
		/// <returns>
		///   <see langword="true" /> if the value name has been explicitly set; otherwise, <see langword="false" />.</returns>
		public bool IsValueNameSetExplicitly
		{
			get
			{
				return this.isValueNameSetExplicitly;
			}
		}

		private string name;

		private string ns;

		private string itemName;

		private string keyName;

		private string valueName;

		private bool isReference;

		private bool isNameSetExplicitly;

		private bool isNamespaceSetExplicitly;

		private bool isReferenceSetExplicitly;

		private bool isItemNameSetExplicitly;

		private bool isKeyNameSetExplicitly;

		private bool isValueNameSetExplicitly;
	}
}
