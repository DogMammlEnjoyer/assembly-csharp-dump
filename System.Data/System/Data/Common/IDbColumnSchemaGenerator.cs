using System;
using System.Collections.ObjectModel;

namespace System.Data.Common
{
	/// <summary>Generates a column schema.</summary>
	public interface IDbColumnSchemaGenerator
	{
		/// <summary>Gets the column schema (<see cref="T:System.Data.Common.DbColumn" /> collection).</summary>
		/// <returns>The column schema (<see cref="T:System.Data.Common.DbColumn" /> collection).</returns>
		ReadOnlyCollection<DbColumn> GetColumnSchema();
	}
}
