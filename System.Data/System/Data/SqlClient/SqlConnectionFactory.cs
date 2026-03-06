using System;
using System.Data.Common;
using System.Data.ProviderBase;
using System.IO;
using System.Reflection;

namespace System.Data.SqlClient
{
	internal sealed class SqlConnectionFactory : DbConnectionFactory
	{
		private SqlConnectionFactory()
		{
		}

		public override DbProviderFactory ProviderFactory
		{
			get
			{
				return SqlClientFactory.Instance;
			}
		}

		protected override DbConnectionInternal CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningConnection)
		{
			return this.CreateConnection(options, poolKey, poolGroupProviderInfo, pool, owningConnection, null);
		}

		protected override DbConnectionInternal CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningConnection, DbConnectionOptions userOptions)
		{
			SqlConnectionString sqlConnectionString = (SqlConnectionString)options;
			SqlConnectionPoolKey sqlConnectionPoolKey = (SqlConnectionPoolKey)poolKey;
			SessionData reconnectSessionData = null;
			SqlConnection sqlConnection = (SqlConnection)owningConnection;
			bool applyTransientFaultHandling = sqlConnection != null && sqlConnection._applyTransientFaultHandling;
			SqlConnectionString userConnectionOptions = null;
			if (userOptions != null)
			{
				userConnectionOptions = (SqlConnectionString)userOptions;
			}
			else if (sqlConnection != null)
			{
				userConnectionOptions = (SqlConnectionString)sqlConnection.UserConnectionOptions;
			}
			if (sqlConnection != null)
			{
				reconnectSessionData = sqlConnection._recoverySessionData;
			}
			bool redirectedUserInstance = false;
			DbConnectionPoolIdentity identity = null;
			if (sqlConnectionString.IntegratedSecurity)
			{
				if (pool != null)
				{
					identity = pool.Identity;
				}
				else
				{
					identity = DbConnectionPoolIdentity.GetCurrent();
				}
			}
			if (sqlConnectionString.UserInstance)
			{
				redirectedUserInstance = true;
				string instanceName;
				if (pool == null || (pool != null && pool.Count <= 0))
				{
					SqlInternalConnectionTds sqlInternalConnectionTds = null;
					try
					{
						SqlConnectionString connectionOptions = new SqlConnectionString(sqlConnectionString, sqlConnectionString.DataSource, true, new bool?(false));
						sqlInternalConnectionTds = new SqlInternalConnectionTds(identity, connectionOptions, sqlConnectionPoolKey.Credential, null, "", null, false, null, null, applyTransientFaultHandling, null);
						instanceName = sqlInternalConnectionTds.InstanceName;
						if (!instanceName.StartsWith("\\\\.\\", StringComparison.Ordinal))
						{
							throw SQL.NonLocalSSEInstance();
						}
						if (pool != null)
						{
							((SqlConnectionPoolProviderInfo)pool.ProviderInfo).InstanceName = instanceName;
						}
						goto IL_125;
					}
					finally
					{
						if (sqlInternalConnectionTds != null)
						{
							sqlInternalConnectionTds.Dispose();
						}
					}
				}
				instanceName = ((SqlConnectionPoolProviderInfo)pool.ProviderInfo).InstanceName;
				IL_125:
				sqlConnectionString = new SqlConnectionString(sqlConnectionString, instanceName, false, null);
				poolGroupProviderInfo = null;
			}
			return new SqlInternalConnectionTds(identity, sqlConnectionString, sqlConnectionPoolKey.Credential, poolGroupProviderInfo, "", null, redirectedUserInstance, userConnectionOptions, reconnectSessionData, applyTransientFaultHandling, sqlConnectionPoolKey.AccessToken);
		}

		protected override DbConnectionOptions CreateConnectionOptions(string connectionString, DbConnectionOptions previous)
		{
			return new SqlConnectionString(connectionString);
		}

		internal override DbConnectionPoolProviderInfo CreateConnectionPoolProviderInfo(DbConnectionOptions connectionOptions)
		{
			DbConnectionPoolProviderInfo result = null;
			if (((SqlConnectionString)connectionOptions).UserInstance)
			{
				result = new SqlConnectionPoolProviderInfo();
			}
			return result;
		}

		protected override DbConnectionPoolGroupOptions CreateConnectionPoolGroupOptions(DbConnectionOptions connectionOptions)
		{
			SqlConnectionString sqlConnectionString = (SqlConnectionString)connectionOptions;
			DbConnectionPoolGroupOptions result = null;
			if (sqlConnectionString.Pooling)
			{
				int num = sqlConnectionString.ConnectTimeout;
				if (0 < num && num < 2147483)
				{
					num *= 1000;
				}
				else if (num >= 2147483)
				{
					num = int.MaxValue;
				}
				result = new DbConnectionPoolGroupOptions(sqlConnectionString.IntegratedSecurity, sqlConnectionString.MinPoolSize, sqlConnectionString.MaxPoolSize, num, sqlConnectionString.LoadBalanceTimeout, sqlConnectionString.Enlist);
			}
			return result;
		}

		internal override DbConnectionPoolGroupProviderInfo CreateConnectionPoolGroupProviderInfo(DbConnectionOptions connectionOptions)
		{
			return new SqlConnectionPoolGroupProviderInfo((SqlConnectionString)connectionOptions);
		}

		internal static SqlConnectionString FindSqlConnectionOptions(SqlConnectionPoolKey key)
		{
			SqlConnectionString sqlConnectionString = (SqlConnectionString)SqlConnectionFactory.SingletonInstance.FindConnectionOptions(key);
			if (sqlConnectionString == null)
			{
				sqlConnectionString = new SqlConnectionString(key.ConnectionString);
			}
			if (sqlConnectionString.IsEmpty)
			{
				throw ADP.NoConnectionString();
			}
			return sqlConnectionString;
		}

		internal override DbConnectionPoolGroup GetConnectionPoolGroup(DbConnection connection)
		{
			SqlConnection sqlConnection = connection as SqlConnection;
			if (sqlConnection != null)
			{
				return sqlConnection.PoolGroup;
			}
			return null;
		}

		internal override DbConnectionInternal GetInnerConnection(DbConnection connection)
		{
			SqlConnection sqlConnection = connection as SqlConnection;
			if (sqlConnection != null)
			{
				return sqlConnection.InnerConnection;
			}
			return null;
		}

		internal override void PermissionDemand(DbConnection outerConnection)
		{
			SqlConnection sqlConnection = outerConnection as SqlConnection;
			if (sqlConnection != null)
			{
				sqlConnection.PermissionDemand();
			}
		}

		internal override void SetConnectionPoolGroup(DbConnection outerConnection, DbConnectionPoolGroup poolGroup)
		{
			SqlConnection sqlConnection = outerConnection as SqlConnection;
			if (sqlConnection != null)
			{
				sqlConnection.PoolGroup = poolGroup;
			}
		}

		internal override void SetInnerConnectionEvent(DbConnection owningObject, DbConnectionInternal to)
		{
			SqlConnection sqlConnection = owningObject as SqlConnection;
			if (sqlConnection != null)
			{
				sqlConnection.SetInnerConnectionEvent(to);
			}
		}

		internal override bool SetInnerConnectionFrom(DbConnection owningObject, DbConnectionInternal to, DbConnectionInternal from)
		{
			SqlConnection sqlConnection = owningObject as SqlConnection;
			return sqlConnection != null && sqlConnection.SetInnerConnectionFrom(to, from);
		}

		internal override void SetInnerConnectionTo(DbConnection owningObject, DbConnectionInternal to)
		{
			SqlConnection sqlConnection = owningObject as SqlConnection;
			if (sqlConnection != null)
			{
				sqlConnection.SetInnerConnectionTo(to);
			}
		}

		protected override DbMetaDataFactory CreateMetaDataFactory(DbConnectionInternal internalConnection, out bool cacheMetaDataFactory)
		{
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("System.Data.SqlClient.SqlMetaData.xml");
			cacheMetaDataFactory = true;
			return new SqlMetaDataFactory(manifestResourceStream, internalConnection.ServerVersion, internalConnection.ServerVersion);
		}

		private const string _metaDataXml = "MetaDataXml";

		public static readonly SqlConnectionFactory SingletonInstance = new SqlConnectionFactory();
	}
}
