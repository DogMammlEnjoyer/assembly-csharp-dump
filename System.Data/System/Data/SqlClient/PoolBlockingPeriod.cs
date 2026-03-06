using System;

namespace System.Data.SqlClient
{
	/// <summary>Specifies a value for the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.PoolBlockingPeriod" /> property.</summary>
	public enum PoolBlockingPeriod
	{
		/// <summary>Blocking period OFF for Azure SQL servers, but ON for all other SQL servers.</summary>
		Auto,
		/// <summary>Blocking period ON for all SQL servers including Azure SQL servers.</summary>
		AlwaysBlock,
		/// <summary>Blocking period OFF for all SQL servers including Azure SQL servers.</summary>
		NeverBlock
	}
}
