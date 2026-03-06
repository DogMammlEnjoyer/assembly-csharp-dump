using System;

namespace System.Data
{
	/// <summary>Used by the Visual Basic .NET Data Designers to represent a parameter to a Command object, and optionally, its mapping to <see cref="T:System.Data.DataSet" /> columns.</summary>
	public interface IDbDataParameter : IDataParameter
	{
		/// <summary>Indicates the precision of numeric parameters.</summary>
		/// <returns>The maximum number of digits used to represent the Value property of a data provider Parameter object. The default value is 0, which indicates that a data provider sets the precision for Value.</returns>
		byte Precision { get; set; }

		/// <summary>Indicates the scale of numeric parameters.</summary>
		/// <returns>The number of decimal places to which <see cref="P:System.Data.OleDb.OleDbParameter.Value" /> is resolved. The default is 0.</returns>
		byte Scale { get; set; }

		/// <summary>The size of the parameter.</summary>
		/// <returns>The maximum size, in bytes, of the data within the column. The default value is inferred from the parameter value.</returns>
		int Size { get; set; }
	}
}
