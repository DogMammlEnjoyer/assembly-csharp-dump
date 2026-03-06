using System;

namespace System.Data
{
	/// <summary>Provides data for the <see cref="M:System.Data.DataTable.NewRow" /> method.</summary>
	public sealed class DataTableNewRowEventArgs : EventArgs
	{
		/// <summary>Initializes a new instance of <see cref="T:System.Data.DataTableNewRowEventArgs" />.</summary>
		/// <param name="dataRow">The <see cref="T:System.Data.DataRow" /> being added.</param>
		public DataTableNewRowEventArgs(DataRow dataRow)
		{
			this.Row = dataRow;
		}

		/// <summary>Gets the row that is being added.</summary>
		/// <returns>The <see cref="T:System.Data.DataRow" /> that is being added.</returns>
		public DataRow Row { get; }
	}
}
