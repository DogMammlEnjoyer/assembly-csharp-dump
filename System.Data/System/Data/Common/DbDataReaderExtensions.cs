using System;
using System.Collections.ObjectModel;

namespace System.Data.Common
{
	/// <summary>This class contains column schema extension methods for <see cref="T:System.Data.Common.DbDataReader" />.</summary>
	public static class DbDataReaderExtensions
	{
		/// <summary>Gets the column schema (<see cref="T:System.Data.Common.DbColumn" /> collection) for a <see cref="T:System.Data.Common.DbDataReader" />.</summary>
		/// <param name="reader">The <see cref="T:System.Data.Common.DbDataReader" /> to return the column schema.</param>
		/// <returns>The column schema (<see cref="T:System.Data.Common.DbColumn" /> collection) for a <see cref="T:System.Data.Common.DbDataReader" />.</returns>
		public static ReadOnlyCollection<DbColumn> GetColumnSchema(this DbDataReader reader)
		{
			if (reader.CanGetColumnSchema())
			{
				return ((IDbColumnSchemaGenerator)reader).GetColumnSchema();
			}
			throw new NotSupportedException();
		}

		/// <summary>Gets a value that indicates whether a <see cref="T:System.Data.Common.DbDataReader" /> can get a column schema.</summary>
		/// <param name="reader">The <see cref="T:System.Data.Common.DbDataReader" /> to be checked for column schema support.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.Common.DbDataReader" /> can get a column schema; otherwise, <see langword="false" />.</returns>
		public static bool CanGetColumnSchema(this DbDataReader reader)
		{
			return reader is IDbColumnSchemaGenerator;
		}
	}
}
