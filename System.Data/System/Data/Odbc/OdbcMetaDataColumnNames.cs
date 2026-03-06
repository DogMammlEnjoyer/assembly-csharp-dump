using System;

namespace System.Data.Odbc
{
	/// <summary>Provides static values that are used for the column names in the <see cref="T:System.Data.Odbc.OdbcMetaDataCollectionNames" /> objects contained in the <see cref="T:System.Data.DataTable" />. The <see cref="T:System.Data.DataTable" /> is created by the GetSchema method.</summary>
	public static class OdbcMetaDataColumnNames
	{
		/// <summary>Used by the GetSchema method to create the BooleanFalseLiteral column.</summary>
		public static readonly string BooleanFalseLiteral = "BooleanFalseLiteral";

		/// <summary>Used by the GetSchema method to create the BooleanTrueLiteral column.</summary>
		public static readonly string BooleanTrueLiteral = "BooleanTrueLiteral";

		/// <summary>Used by the GetSchema method to create the SQLType column.</summary>
		public static readonly string SQLType = "SQLType";
	}
}
