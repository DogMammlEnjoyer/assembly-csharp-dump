using System;

namespace System.Data.Common
{
	/// <summary>Identifies which provider-specific property in the strongly typed parameter classes is to be used when setting a provider-specific type.</summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	[Serializable]
	public sealed class DbProviderSpecificTypePropertyAttribute : Attribute
	{
		/// <summary>Initializes a new instance of a <see cref="T:System.Data.Common.DbProviderSpecificTypePropertyAttribute" /> class.</summary>
		/// <param name="isProviderSpecificTypeProperty">Specifies whether this property is a provider-specific property.</param>
		public DbProviderSpecificTypePropertyAttribute(bool isProviderSpecificTypeProperty)
		{
			this.IsProviderSpecificTypeProperty = isProviderSpecificTypeProperty;
		}

		/// <summary>Indicates whether the attributed property is a provider-specific type.</summary>
		/// <returns>
		///   <see langword="true" /> if the property that this attribute is applied to is a provider-specific type property; otherwise <see langword="false" />.</returns>
		public bool IsProviderSpecificTypeProperty { get; }
	}
}
