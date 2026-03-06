using System;

namespace System.ComponentModel
{
	/// <summary>Provides metadata for a property representing a data field. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class DataObjectFieldAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DataObjectFieldAttribute" /> class and indicates whether the field is the primary key for the data row.</summary>
		/// <param name="primaryKey">
		///   <see langword="true" /> to indicate that the field is in the primary key of the data row; otherwise, <see langword="false" />.</param>
		public DataObjectFieldAttribute(bool primaryKey) : this(primaryKey, false, false, -1)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DataObjectFieldAttribute" /> class and indicates whether the field is the primary key for the data row, and whether the field is a database identity field.</summary>
		/// <param name="primaryKey">
		///   <see langword="true" /> to indicate that the field is in the primary key of the data row; otherwise, <see langword="false" />.</param>
		/// <param name="isIdentity">
		///   <see langword="true" /> to indicate that the field is an identity field that uniquely identifies the data row; otherwise, <see langword="false" />.</param>
		public DataObjectFieldAttribute(bool primaryKey, bool isIdentity) : this(primaryKey, isIdentity, false, -1)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DataObjectFieldAttribute" /> class and indicates whether the field is the primary key for the data row, whether the field is a database identity field, and whether the field can be null.</summary>
		/// <param name="primaryKey">
		///   <see langword="true" /> to indicate that the field is in the primary key of the data row; otherwise, <see langword="false" />.</param>
		/// <param name="isIdentity">
		///   <see langword="true" /> to indicate that the field is an identity field that uniquely identifies the data row; otherwise, <see langword="false" />.</param>
		/// <param name="isNullable">
		///   <see langword="true" /> to indicate that the field can be null in the data store; otherwise, <see langword="false" />.</param>
		public DataObjectFieldAttribute(bool primaryKey, bool isIdentity, bool isNullable) : this(primaryKey, isIdentity, isNullable, -1)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.DataObjectFieldAttribute" /> class and indicates whether the field is the primary key for the data row, whether it is a database identity field, and whether it can be null and sets the length of the field.</summary>
		/// <param name="primaryKey">
		///   <see langword="true" /> to indicate that the field is in the primary key of the data row; otherwise, <see langword="false" />.</param>
		/// <param name="isIdentity">
		///   <see langword="true" /> to indicate that the field is an identity field that uniquely identifies the data row; otherwise, <see langword="false" />.</param>
		/// <param name="isNullable">
		///   <see langword="true" /> to indicate that the field can be null in the data store; otherwise, <see langword="false" />.</param>
		/// <param name="length">The length of the field in bytes.</param>
		public DataObjectFieldAttribute(bool primaryKey, bool isIdentity, bool isNullable, int length)
		{
			this.PrimaryKey = primaryKey;
			this.IsIdentity = isIdentity;
			this.IsNullable = isNullable;
			this.Length = length;
		}

		/// <summary>Gets a value indicating whether a property represents an identity field in the underlying data.</summary>
		/// <returns>
		///   <see langword="true" /> if the property represents an identity field in the underlying data; otherwise, <see langword="false" />. The default value is <see langword="false" />.</returns>
		public bool IsIdentity { get; }

		/// <summary>Gets a value indicating whether a property represents a field that can be null in the underlying data store.</summary>
		/// <returns>
		///   <see langword="true" /> if the property represents a field that can be null in the underlying data store; otherwise, <see langword="false" />.</returns>
		public bool IsNullable { get; }

		/// <summary>Gets the length of the property in bytes.</summary>
		/// <returns>The length of the property in bytes, or -1 if not set.</returns>
		public int Length { get; }

		/// <summary>Gets a value indicating whether a property is in the primary key in the underlying data.</summary>
		/// <returns>
		///   <see langword="true" /> if the property is in the primary key of the data store; otherwise, <see langword="false" />.</returns>
		public bool PrimaryKey { get; }

		/// <summary>Returns a value indicating whether this instance is equal to a specified object.</summary>
		/// <param name="obj">An object to compare with this instance of <see cref="T:System.ComponentModel.DataObjectFieldAttribute" />.</param>
		/// <returns>
		///   <see langword="true" /> if this instance is the same as the instance specified by the <paramref name="obj" /> parameter; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DataObjectFieldAttribute dataObjectFieldAttribute = obj as DataObjectFieldAttribute;
			return dataObjectFieldAttribute != null && dataObjectFieldAttribute.IsIdentity == this.IsIdentity && dataObjectFieldAttribute.IsNullable == this.IsNullable && dataObjectFieldAttribute.Length == this.Length && dataObjectFieldAttribute.PrimaryKey == this.PrimaryKey;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
