using System;

namespace System.ComponentModel
{
	/// <summary>Specifies what type to use as a converter for the object this attribute is bound to.</summary>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class TypeConverterAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.TypeConverterAttribute" /> class with the default type converter, which is an empty string ("").</summary>
		public TypeConverterAttribute()
		{
			this.ConverterTypeName = string.Empty;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.TypeConverterAttribute" /> class, using the specified type as the data converter for the object this attribute is bound to.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> that represents the type of the converter class to use for data conversion for the object this attribute is bound to.</param>
		public TypeConverterAttribute(Type type)
		{
			this.ConverterTypeName = type.AssemblyQualifiedName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.TypeConverterAttribute" /> class, using the specified type name as the data converter for the object this attribute is bound to.</summary>
		/// <param name="typeName">The fully qualified name of the class to use for data conversion for the object this attribute is bound to.</param>
		public TypeConverterAttribute(string typeName)
		{
			this.ConverterTypeName = typeName;
		}

		/// <summary>Gets the fully qualified type name of the <see cref="T:System.Type" /> to use as a converter for the object this attribute is bound to.</summary>
		/// <returns>The fully qualified type name of the <see cref="T:System.Type" /> to use as a converter for the object this attribute is bound to, or an empty string ("") if none exists. The default value is an empty string ("").</returns>
		public string ConverterTypeName { get; }

		/// <summary>Returns whether the value of the given object is equal to the current <see cref="T:System.ComponentModel.TypeConverterAttribute" />.</summary>
		/// <param name="obj">The object to test the value equality of.</param>
		/// <returns>
		///   <see langword="true" /> if the value of the given object is equal to that of the current <see cref="T:System.ComponentModel.TypeConverterAttribute" />; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			TypeConverterAttribute typeConverterAttribute = obj as TypeConverterAttribute;
			return typeConverterAttribute != null && typeConverterAttribute.ConverterTypeName == this.ConverterTypeName;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>A hash code for the current <see cref="T:System.ComponentModel.TypeConverterAttribute" />.</returns>
		public override int GetHashCode()
		{
			return this.ConverterTypeName.GetHashCode();
		}

		/// <summary>Specifies the type to use as a converter for the object this attribute is bound to.</summary>
		public static readonly TypeConverterAttribute Default = new TypeConverterAttribute();
	}
}
