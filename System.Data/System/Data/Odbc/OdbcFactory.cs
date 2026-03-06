using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Odbc
{
	/// <summary>Represents a set of methods for creating instances of the ODBC provider's implementation of the data source classes.</summary>
	public sealed class OdbcFactory : DbProviderFactory
	{
		private OdbcFactory()
		{
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Data.Common.DbCommand" /> instance.</summary>
		/// <returns>A new strongly-typed instance of <see cref="T:System.Data.Common.DbCommand" />.</returns>
		public override DbCommand CreateCommand()
		{
			return new OdbcCommand();
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Data.Common.DbCommandBuilder" /> instance.</summary>
		/// <returns>A new strongly-typed instance of <see cref="T:System.Data.Common.DbCommandBuilder" />.</returns>
		public override DbCommandBuilder CreateCommandBuilder()
		{
			return new OdbcCommandBuilder();
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Data.Common.DbConnection" /> instance.</summary>
		/// <returns>A new strongly-typed instance of <see cref="T:System.Data.Common.DbConnection" />.</returns>
		public override DbConnection CreateConnection()
		{
			return new OdbcConnection();
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> instance.</summary>
		/// <returns>A new strongly-typed instance of <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</returns>
		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return new OdbcConnectionStringBuilder();
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Data.Common.DbDataAdapter" /> instance.</summary>
		/// <returns>A new strongly-typed instance of <see cref="T:System.Data.Common.DbDataAdapter" />.</returns>
		public override DbDataAdapter CreateDataAdapter()
		{
			return new OdbcDataAdapter();
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Data.Common.DbParameter" /> instance.</summary>
		/// <returns>A new strongly-typed instance of <see cref="T:System.Data.Common.DbParameter" />.</returns>
		public override DbParameter CreateParameter()
		{
			return new OdbcParameter();
		}

		/// <summary>Returns a strongly-typed <see cref="T:System.Security.CodeAccessPermission" /> instance.</summary>
		/// <param name="state">A member of the <see cref="T:System.Security.Permissions.PermissionState" /> enumeration.</param>
		/// <returns>A new strongly-typed instance of <see cref="T:System.Security.CodeAccessPermission" />.</returns>
		public override CodeAccessPermission CreatePermission(PermissionState state)
		{
			return new OdbcPermission(state);
		}

		/// <summary>Gets an instance of the <see cref="T:System.Data.Odbc.OdbcFactory" />, which can be used to retrieve strongly-typed data objects.</summary>
		public static readonly OdbcFactory Instance = new OdbcFactory();
	}
}
