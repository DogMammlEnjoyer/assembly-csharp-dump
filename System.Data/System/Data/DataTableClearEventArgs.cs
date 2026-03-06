using System;

namespace System.Data
{
	/// <summary>Provides data for the <see cref="M:System.Data.DataTable.Clear" /> method.</summary>
	public sealed class DataTableClearEventArgs : EventArgs
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.DataTableClearEventArgs" /> class.</summary>
		/// <param name="dataTable">The <see cref="T:System.Data.DataTable" /> whose rows are being cleared.</param>
		public DataTableClearEventArgs(DataTable dataTable)
		{
			this.Table = dataTable;
		}

		/// <summary>Gets the table whose rows are being cleared.</summary>
		/// <returns>The <see cref="T:System.Data.DataTable" /> whose rows are being cleared.</returns>
		public DataTable Table { get; }

		/// <summary>Gets the table name whose rows are being cleared.</summary>
		/// <returns>A <see cref="T:System.String" /> indicating the table name.</returns>
		public string TableName
		{
			get
			{
				return this.Table.TableName;
			}
		}

		/// <summary>Gets the namespace of the table whose rows are being cleared.</summary>
		/// <returns>A <see cref="T:System.String" /> indicating the namespace name.</returns>
		public string TableNamespace
		{
			get
			{
				return this.Table.Namespace;
			}
		}
	}
}
