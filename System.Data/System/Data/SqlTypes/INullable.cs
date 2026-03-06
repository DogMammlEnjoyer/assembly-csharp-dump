using System;

namespace System.Data.SqlTypes
{
	/// <summary>All the <see cref="N:System.Data.SqlTypes" /> objects and structures implement the <see langword="INullable" /> interface.</summary>
	public interface INullable
	{
		/// <summary>Indicates whether a structure is null. This property is read-only.</summary>
		/// <returns>
		///   <see cref="T:System.Data.SqlTypes.SqlBoolean" />
		///   <see langword="true" /> if the value of this object is null. Otherwise, <see langword="false" />.</returns>
		bool IsNull { get; }
	}
}
