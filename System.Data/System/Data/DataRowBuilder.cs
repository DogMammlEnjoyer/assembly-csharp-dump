using System;
using Unity;

namespace System.Data
{
	/// <summary>The <see langword="DataRowBuilder" /> type supports the .NET Framework infrastructure and is not intended to be used directly from your code.</summary>
	public sealed class DataRowBuilder
	{
		internal DataRowBuilder(DataTable table, int record)
		{
			this._table = table;
			this._record = record;
		}

		internal DataRowBuilder()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		internal readonly DataTable _table;

		internal int _record;
	}
}
