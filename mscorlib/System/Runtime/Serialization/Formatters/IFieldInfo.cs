using System;

namespace System.Runtime.Serialization.Formatters
{
	/// <summary>Allows access to field names and field types of objects that support the <see cref="T:System.Runtime.Serialization.ISerializable" /> interface.</summary>
	public interface IFieldInfo
	{
		/// <summary>Gets or sets the field names of serialized objects.</summary>
		/// <returns>The field names of serialized objects.</returns>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		string[] FieldNames { get; set; }

		/// <summary>Gets or sets the field types of the serialized objects.</summary>
		/// <returns>The field types of the serialized objects.</returns>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		Type[] FieldTypes { get; set; }
	}
}
