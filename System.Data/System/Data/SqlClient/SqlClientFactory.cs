using System;
using System.Data.Common;
using System.Data.Sql;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Data.SqlClient
{
	/// <summary>Represents a set of methods for creating instances of the <see cref="N:System.Data.SqlClient" /> provider's implementation of the data source classes.</summary>
	public sealed class SqlClientFactory : DbProviderFactory, IServiceProvider
	{
		private SqlClientFactory()
		{
		}

		/// <summary>Returns a strongly typed <see cref="T:System.Data.Common.DbCommand" /> instance.</summary>
		/// <returns>A new strongly typed instance of <see cref="T:System.Data.Common.DbCommand" />.</returns>
		public override DbCommand CreateCommand()
		{
			return new SqlCommand();
		}

		/// <summary>Returns a strongly typed <see cref="T:System.Data.Common.DbCommandBuilder" /> instance.</summary>
		/// <returns>A new strongly typed instance of <see cref="T:System.Data.Common.DbCommandBuilder" />.</returns>
		public override DbCommandBuilder CreateCommandBuilder()
		{
			return new SqlCommandBuilder();
		}

		/// <summary>Returns a strongly typed <see cref="T:System.Data.Common.DbConnection" /> instance.</summary>
		/// <returns>A new strongly typed instance of <see cref="T:System.Data.Common.DbConnection" />.</returns>
		public override DbConnection CreateConnection()
		{
			return new SqlConnection();
		}

		/// <summary>Returns a strongly typed <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> instance.</summary>
		/// <returns>A new strongly typed instance of <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</returns>
		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			return new SqlConnectionStringBuilder();
		}

		/// <summary>Returns a strongly typed <see cref="T:System.Data.Common.DbDataAdapter" /> instance.</summary>
		/// <returns>A new strongly typed instance of <see cref="T:System.Data.Common.DbDataAdapter" />.</returns>
		public override DbDataAdapter CreateDataAdapter()
		{
			return new SqlDataAdapter();
		}

		/// <summary>Returns a strongly typed <see cref="T:System.Data.Common.DbParameter" /> instance.</summary>
		/// <returns>A new strongly typed instance of <see cref="T:System.Data.Common.DbParameter" />.</returns>
		public override DbParameter CreateParameter()
		{
			return new SqlParameter();
		}

		/// <summary>Gets a value that indicates whether a <see cref="T:System.Data.Sql.SqlDataSourceEnumerator" /> can be created.</summary>
		/// <returns>
		///   <see langword="true" /> if a <see cref="T:System.Data.Sql.SqlDataSourceEnumerator" /> can be created; otherwise, <see langword="false" />.</returns>
		public override bool CanCreateDataSourceEnumerator
		{
			get
			{
				return true;
			}
		}

		/// <summary>Returns a new <see cref="T:System.Data.Sql.SqlDataSourceEnumerator" />.</summary>
		/// <returns>A new data source enumerator.</returns>
		public override DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			return SqlDataSourceEnumerator.Instance;
		}

		/// <summary>Returns a new <see cref="T:System.Security.CodeAccessPermission" />.</summary>
		/// <param name="state">A member of the <see cref="T:System.Security.Permissions.PermissionState" /> enumeration.</param>
		/// <returns>A strongly typed instance of <see cref="T:System.Security.CodeAccessPermission" />.</returns>
		public override CodeAccessPermission CreatePermission(PermissionState state)
		{
			return new SqlClientPermission(state);
		}

		/// <summary>For a description of this member, see <see cref="M:System.IServiceProvider.GetService(System.Type)" />.</summary>
		/// <param name="serviceType">An object that specifies the type of service object to get.</param>
		/// <returns>A service object.</returns>
		object IServiceProvider.GetService(Type serviceType)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		/// <summary>Gets an instance of the <see cref="T:System.Data.SqlClient.SqlClientFactory" />. This can be used to retrieve strongly typed data objects.</summary>
		public static readonly SqlClientFactory Instance = new SqlClientFactory();
	}
}
