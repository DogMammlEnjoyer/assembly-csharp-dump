using System;

namespace System.Data.Odbc
{
	/// <summary>Provides a list of constants for use with the GetSchema method to retrieve metadata collections.</summary>
	public static class OdbcMetaDataCollectionNames
	{
		/// <summary>A constant for use with the GetSchema method that represents the Columns collection.</summary>
		public static readonly string Columns = "Columns";

		/// <summary>A constant for use with the GetSchema method that represents the Indexes collection.</summary>
		public static readonly string Indexes = "Indexes";

		/// <summary>A constant for use with the GetSchema method that represents the Procedures collection.</summary>
		public static readonly string Procedures = "Procedures";

		/// <summary>A constant for use with the GetSchema method that represents the ProcedureColumns collection.</summary>
		public static readonly string ProcedureColumns = "ProcedureColumns";

		/// <summary>A constant for use with the GetSchema method that represents the ProcedureParameters collection.</summary>
		public static readonly string ProcedureParameters = "ProcedureParameters";

		/// <summary>A constant for use with the GetSchema method that represents the Tables collection.</summary>
		public static readonly string Tables = "Tables";

		/// <summary>A constant for use with the GetSchema method that represents the Views collection.</summary>
		public static readonly string Views = "Views";
	}
}
