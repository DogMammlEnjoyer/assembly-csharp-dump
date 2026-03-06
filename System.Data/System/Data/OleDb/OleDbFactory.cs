using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.OleDb
{
	/// <summary>Represents a set of methods for creating instances of the OLEDB provider's implementation of the data source classes.</summary>
	[MonoTODO("OleDb is not implemented.")]
	public sealed class OleDbFactory : DbProviderFactory
	{
		internal OleDbFactory()
		{
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Data.Common.DbCommand" /> instance.</summary>
		/// <returns>A new strongly-typed instance of <see cref="T:System.Data.Common.DbCommand" />.</returns>
		public override DbCommand CreateCommand()
		{
			throw ADP.OleDb();
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Data.Common.DbCommandBuilder" /> instance.</summary>
		/// <returns>A new strongly-typed instance of <see cref="T:System.Data.Common.DbCommandBuilder" />.</returns>
		public override DbCommandBuilder CreateCommandBuilder()
		{
			throw ADP.OleDb();
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Data.Common.DbConnection" /> instance.</summary>
		/// <returns>A new strongly-typed instance of <see cref="T:System.Data.Common.DbConnection" />.</returns>
		public override DbConnection CreateConnection()
		{
			throw ADP.OleDb();
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> instance.</summary>
		/// <returns>A new strongly-typed instance of <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</returns>
		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			throw ADP.OleDb();
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Data.Common.DbDataAdapter" /> instance.</summary>
		/// <returns>A new strongly-typed instance of <see cref="T:System.Data.Common.DbDataAdapter" />.</returns>
		public override DbDataAdapter CreateDataAdapter()
		{
			throw ADP.OleDb();
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Data.Common.DbParameter" /> instance.</summary>
		/// <returns>A new strongly-typed instance of <see cref="T:System.Data.Common.DbParameter" />.</returns>
		public override DbParameter CreateParameter()
		{
			throw ADP.OleDb();
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Security.CodeAccessPermission" /> instance.</summary>
		/// <param name="state">A member of the <see cref="T:System.Security.Permissions.PermissionState" /> enumeration.</param>
		/// <returns>A strongly-typed instance of <see cref="T:System.Security.CodeAccessPermission" />.</returns>
		public override CodeAccessPermission CreatePermission(PermissionState state)
		{
			throw ADP.OleDb();
		}

		/// <summary>Gets an instance of the <see cref="T:System.Data.OleDb.OleDbFactory" />. This can be used to retrieve strongly-typed data objects.</summary>
		public static readonly OleDbFactory Instance;
	}
}
