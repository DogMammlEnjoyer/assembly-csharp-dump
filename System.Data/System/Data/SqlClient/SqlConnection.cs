using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.ProviderBase;
using System.EnterpriseServices;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.SqlServer.Server;
using Unity;

namespace System.Data.SqlClient
{
	/// <summary>Represents a connection to a SQL Server database. This class cannot be inherited.</summary>
	public sealed class SqlConnection : DbConnection, ICloneable, IDbConnection, IDisposable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlClient.SqlConnection" /> class when given a string that contains the connection string.</summary>
		/// <param name="connectionString">The connection used to open the SQL Server database.</param>
		public SqlConnection(string connectionString) : this()
		{
			this.ConnectionString = connectionString;
			this.CacheConnectionStringProperties();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlClient.SqlConnection" /> class given a connection string, that does not use <see langword="Integrated Security = true" /> and a <see cref="T:System.Data.SqlClient.SqlCredential" /> object that contains the user ID and password.</summary>
		/// <param name="connectionString">A connection string that does not use any of the following connection string keywords: <see langword="Integrated Security = true" />, <see langword="UserId" />, or <see langword="Password" />; or that does not use <see langword="ContextConnection = true" />.</param>
		/// <param name="credential">A <see cref="T:System.Data.SqlClient.SqlCredential" /> object. If <paramref name="credential" /> is null, <see cref="M:System.Data.SqlClient.SqlConnection.#ctor(System.String,System.Data.SqlClient.SqlCredential)" /> is functionally equivalent to <see cref="M:System.Data.SqlClient.SqlConnection.#ctor(System.String)" />.</param>
		public SqlConnection(string connectionString, SqlCredential credential) : this()
		{
			this.ConnectionString = connectionString;
			if (credential != null)
			{
				SqlConnectionString opt = (SqlConnectionString)this.ConnectionOptions;
				if (this.UsesClearUserIdOrPassword(opt))
				{
					throw ADP.InvalidMixedArgumentOfSecureAndClearCredential();
				}
				if (this.UsesIntegratedSecurity(opt))
				{
					throw ADP.InvalidMixedArgumentOfSecureCredentialAndIntegratedSecurity();
				}
				this.Credential = credential;
			}
			this.CacheConnectionStringProperties();
		}

		private SqlConnection(SqlConnection connection)
		{
			this._reconnectLock = new object();
			this._originalConnectionId = Guid.Empty;
			base..ctor();
			GC.SuppressFinalize(this);
			this.CopyFrom(connection);
			this._connectionString = connection._connectionString;
			if (connection._credential != null)
			{
				SecureString secureString = connection._credential.Password.Copy();
				secureString.MakeReadOnly();
				this._credential = new SqlCredential(connection._credential.UserId, secureString);
			}
			this._accessToken = connection._accessToken;
			this.CacheConnectionStringProperties();
		}

		private void CacheConnectionStringProperties()
		{
			SqlConnectionString sqlConnectionString = this.ConnectionOptions as SqlConnectionString;
			if (sqlConnectionString != null)
			{
				this._connectRetryCount = sqlConnectionString.ConnectRetryCount;
			}
		}

		/// <summary>When set to <see langword="true" />, enables statistics gathering for the current connection.</summary>
		/// <returns>Returns <see langword="true" /> if statistics gathering is enabled; otherwise <see langword="false" />. <see langword="false" /> is the default.</returns>
		public bool StatisticsEnabled
		{
			get
			{
				return this._collectstats;
			}
			set
			{
				if (value)
				{
					if (ConnectionState.Open == this.State)
					{
						if (this._statistics == null)
						{
							this._statistics = new SqlStatistics();
							ADP.TimerCurrent(out this._statistics._openTimestamp);
						}
						this.Parser.Statistics = this._statistics;
					}
				}
				else if (this._statistics != null && ConnectionState.Open == this.State)
				{
					this.Parser.Statistics = null;
					ADP.TimerCurrent(out this._statistics._closeTimestamp);
				}
				this._collectstats = value;
			}
		}

		internal bool AsyncCommandInProgress
		{
			get
			{
				return this._AsyncCommandInProgress;
			}
			set
			{
				this._AsyncCommandInProgress = value;
			}
		}

		private bool UsesIntegratedSecurity(SqlConnectionString opt)
		{
			return opt != null && opt.IntegratedSecurity;
		}

		private bool UsesClearUserIdOrPassword(SqlConnectionString opt)
		{
			bool result = false;
			if (opt != null)
			{
				result = (!string.IsNullOrEmpty(opt.UserID) || !string.IsNullOrEmpty(opt.Password));
			}
			return result;
		}

		internal SqlConnectionString.TransactionBindingEnum TransactionBinding
		{
			get
			{
				return ((SqlConnectionString)this.ConnectionOptions).TransactionBinding;
			}
		}

		internal SqlConnectionString.TypeSystem TypeSystem
		{
			get
			{
				return ((SqlConnectionString)this.ConnectionOptions).TypeSystemVersion;
			}
		}

		internal Version TypeSystemAssemblyVersion
		{
			get
			{
				return ((SqlConnectionString)this.ConnectionOptions).TypeSystemAssemblyVersion;
			}
		}

		internal int ConnectRetryInterval
		{
			get
			{
				return ((SqlConnectionString)this.ConnectionOptions).ConnectRetryInterval;
			}
		}

		/// <summary>Gets or sets the string used to open a SQL Server database.</summary>
		/// <returns>The connection string that includes the source database name, and other parameters needed to establish the initial connection. The default value is an empty string.</returns>
		/// <exception cref="T:System.ArgumentException">An invalid connection string argument has been supplied, or a required connection string argument has not been supplied.</exception>
		public override string ConnectionString
		{
			get
			{
				return this.ConnectionString_Get();
			}
			set
			{
				if (this._credential != null || this._accessToken != null)
				{
					SqlConnectionString connectionOptions = new SqlConnectionString(value);
					if (this._credential != null)
					{
						this.CheckAndThrowOnInvalidCombinationOfConnectionStringAndSqlCredential(connectionOptions);
					}
					else
					{
						this.CheckAndThrowOnInvalidCombinationOfConnectionOptionAndAccessToken(connectionOptions);
					}
				}
				this.ConnectionString_Set(new SqlConnectionPoolKey(value, this._credential, this._accessToken));
				this._connectionString = value;
				this.CacheConnectionStringProperties();
			}
		}

		/// <summary>Gets the time to wait while trying to establish a connection before terminating the attempt and generating an error.</summary>
		/// <returns>The time (in seconds) to wait for a connection to open. The default value is 15 seconds.</returns>
		/// <exception cref="T:System.ArgumentException">The value set is less than 0.</exception>
		public override int ConnectionTimeout
		{
			get
			{
				SqlConnectionString sqlConnectionString = (SqlConnectionString)this.ConnectionOptions;
				if (sqlConnectionString == null)
				{
					return 15;
				}
				return sqlConnectionString.ConnectTimeout;
			}
		}

		/// <summary>Gets or sets the access token for the connection.</summary>
		/// <returns>The access token for the connection.</returns>
		public string AccessToken
		{
			get
			{
				string accessToken = this._accessToken;
				SqlConnectionString sqlConnectionString = (SqlConnectionString)this.UserConnectionOptions;
				if (!this.InnerConnection.ShouldHidePassword || sqlConnectionString == null || sqlConnectionString.PersistSecurityInfo)
				{
					return this._accessToken;
				}
				return null;
			}
			set
			{
				if (!this.InnerConnection.AllowSetConnectionString)
				{
					throw ADP.OpenConnectionPropertySet("AccessToken", this.InnerConnection.State);
				}
				if (value != null)
				{
					this.CheckAndThrowOnInvalidCombinationOfConnectionOptionAndAccessToken((SqlConnectionString)this.ConnectionOptions);
				}
				this.ConnectionString_Set(new SqlConnectionPoolKey(this._connectionString, this._credential, value));
				this._accessToken = value;
			}
		}

		/// <summary>Gets the name of the current database or the database to be used after a connection is opened.</summary>
		/// <returns>The name of the current database or the name of the database to be used after a connection is opened. The default value is an empty string.</returns>
		public override string Database
		{
			get
			{
				SqlInternalConnection sqlInternalConnection = this.InnerConnection as SqlInternalConnection;
				string result;
				if (sqlInternalConnection != null)
				{
					result = sqlInternalConnection.CurrentDatabase;
				}
				else
				{
					SqlConnectionString sqlConnectionString = (SqlConnectionString)this.ConnectionOptions;
					result = ((sqlConnectionString != null) ? sqlConnectionString.InitialCatalog : "");
				}
				return result;
			}
		}

		/// <summary>Gets the name of the instance of SQL Server to which to connect.</summary>
		/// <returns>The name of the instance of SQL Server to which to connect. The default value is an empty string.</returns>
		public override string DataSource
		{
			get
			{
				SqlInternalConnection sqlInternalConnection = this.InnerConnection as SqlInternalConnection;
				string result;
				if (sqlInternalConnection != null)
				{
					result = sqlInternalConnection.CurrentDataSource;
				}
				else
				{
					SqlConnectionString sqlConnectionString = (SqlConnectionString)this.ConnectionOptions;
					result = ((sqlConnectionString != null) ? sqlConnectionString.DataSource : "");
				}
				return result;
			}
		}

		/// <summary>Gets the size (in bytes) of network packets used to communicate with an instance of SQL Server.</summary>
		/// <returns>The size (in bytes) of network packets. The default value is 8000.</returns>
		public int PacketSize
		{
			get
			{
				SqlInternalConnectionTds sqlInternalConnectionTds = this.InnerConnection as SqlInternalConnectionTds;
				int result;
				if (sqlInternalConnectionTds != null)
				{
					result = sqlInternalConnectionTds.PacketSize;
				}
				else
				{
					SqlConnectionString sqlConnectionString = (SqlConnectionString)this.ConnectionOptions;
					result = ((sqlConnectionString != null) ? sqlConnectionString.PacketSize : 8000);
				}
				return result;
			}
		}

		/// <summary>The connection ID of the most recent connection attempt, regardless of whether the attempt succeeded or failed.</summary>
		/// <returns>The connection ID of the most recent connection attempt.</returns>
		public Guid ClientConnectionId
		{
			get
			{
				SqlInternalConnectionTds sqlInternalConnectionTds = this.InnerConnection as SqlInternalConnectionTds;
				if (sqlInternalConnectionTds != null)
				{
					return sqlInternalConnectionTds.ClientConnectionId;
				}
				Task currentReconnectionTask = this._currentReconnectionTask;
				if (currentReconnectionTask != null && !currentReconnectionTask.IsCompleted)
				{
					return this._originalConnectionId;
				}
				return Guid.Empty;
			}
		}

		/// <summary>Gets a string that contains the version of the instance of SQL Server to which the client is connected.</summary>
		/// <returns>The version of the instance of SQL Server.</returns>
		/// <exception cref="T:System.InvalidOperationException">The connection is closed.  
		///  <see cref="P:System.Data.SqlClient.SqlConnection.ServerVersion" /> was called while the returned Task was not completed and the connection was not opened after a call to <see cref="M:System.Data.SqlClient.SqlConnection.OpenAsync(System.Threading.CancellationToken)" />.</exception>
		public override string ServerVersion
		{
			get
			{
				return this.GetOpenTdsConnection().ServerVersion;
			}
		}

		/// <summary>Indicates the state of the <see cref="T:System.Data.SqlClient.SqlConnection" /> during the most recent network operation performed on the connection.</summary>
		/// <returns>An <see cref="T:System.Data.ConnectionState" /> enumeration.</returns>
		public override ConnectionState State
		{
			get
			{
				Task currentReconnectionTask = this._currentReconnectionTask;
				if (currentReconnectionTask != null && !currentReconnectionTask.IsCompleted)
				{
					return ConnectionState.Open;
				}
				return this.InnerConnection.State;
			}
		}

		internal SqlStatistics Statistics
		{
			get
			{
				return this._statistics;
			}
		}

		/// <summary>Gets a string that identifies the database client.</summary>
		/// <returns>A string that identifies the database client. If not specified, the name of the client computer. If neither is specified, the value is an empty string.</returns>
		public string WorkstationId
		{
			get
			{
				SqlConnectionString sqlConnectionString = (SqlConnectionString)this.ConnectionOptions;
				return ((sqlConnectionString != null) ? sqlConnectionString.WorkstationId : null) ?? Environment.MachineName;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Data.SqlClient.SqlCredential" /> object for this connection.</summary>
		/// <returns>The <see cref="T:System.Data.SqlClient.SqlCredential" /> object for this connection.</returns>
		public SqlCredential Credential
		{
			get
			{
				SqlCredential result = this._credential;
				SqlConnectionString sqlConnectionString = (SqlConnectionString)this.UserConnectionOptions;
				if (this.InnerConnection.ShouldHidePassword && sqlConnectionString != null && !sqlConnectionString.PersistSecurityInfo)
				{
					result = null;
				}
				return result;
			}
			set
			{
				if (!this.InnerConnection.AllowSetConnectionString)
				{
					throw ADP.OpenConnectionPropertySet("Credential", this.InnerConnection.State);
				}
				if (value != null)
				{
					this.CheckAndThrowOnInvalidCombinationOfConnectionStringAndSqlCredential((SqlConnectionString)this.ConnectionOptions);
					if (this._accessToken != null)
					{
						throw ADP.InvalidMixedUsageOfCredentialAndAccessToken();
					}
				}
				this._credential = value;
				this.ConnectionString_Set(new SqlConnectionPoolKey(this._connectionString, this._credential, this._accessToken));
			}
		}

		private void CheckAndThrowOnInvalidCombinationOfConnectionStringAndSqlCredential(SqlConnectionString connectionOptions)
		{
			if (this.UsesClearUserIdOrPassword(connectionOptions))
			{
				throw ADP.InvalidMixedUsageOfSecureAndClearCredential();
			}
			if (this.UsesIntegratedSecurity(connectionOptions))
			{
				throw ADP.InvalidMixedUsageOfSecureCredentialAndIntegratedSecurity();
			}
		}

		private void CheckAndThrowOnInvalidCombinationOfConnectionOptionAndAccessToken(SqlConnectionString connectionOptions)
		{
			if (this.UsesClearUserIdOrPassword(connectionOptions))
			{
				throw ADP.InvalidMixedUsageOfAccessTokenAndUserIDPassword();
			}
			if (this.UsesIntegratedSecurity(connectionOptions))
			{
				throw ADP.InvalidMixedUsageOfAccessTokenAndIntegratedSecurity();
			}
			if (this._credential != null)
			{
				throw ADP.InvalidMixedUsageOfCredentialAndAccessToken();
			}
		}

		protected override DbProviderFactory DbProviderFactory
		{
			get
			{
				return SqlClientFactory.Instance;
			}
		}

		/// <summary>Occurs when SQL Server returns a warning or informational message.</summary>
		public event SqlInfoMessageEventHandler InfoMessage;

		/// <summary>Gets or sets the <see cref="P:System.Data.SqlClient.SqlConnection.FireInfoMessageEventOnUserErrors" /> property.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Data.SqlClient.SqlConnection.FireInfoMessageEventOnUserErrors" /> property has been set; otherwise <see langword="false" />.</returns>
		public bool FireInfoMessageEventOnUserErrors
		{
			get
			{
				return this._fireInfoMessageEventOnUserErrors;
			}
			set
			{
				this._fireInfoMessageEventOnUserErrors = value;
			}
		}

		internal int ReconnectCount
		{
			get
			{
				return this._reconnectCount;
			}
		}

		internal bool ForceNewConnection { get; set; }

		protected override void OnStateChange(StateChangeEventArgs stateChange)
		{
			if (!this._suppressStateChangeForReconnection)
			{
				base.OnStateChange(stateChange);
			}
		}

		/// <summary>Starts a database transaction.</summary>
		/// <returns>An object representing the new transaction.</returns>
		/// <exception cref="T:System.Data.SqlClient.SqlException">Parallel transactions are not allowed when using Multiple Active Result Sets (MARS).</exception>
		/// <exception cref="T:System.InvalidOperationException">Parallel transactions are not supported.</exception>
		public new SqlTransaction BeginTransaction()
		{
			return this.BeginTransaction(IsolationLevel.Unspecified, null);
		}

		/// <summary>Starts a database transaction with the specified isolation level.</summary>
		/// <param name="iso">The isolation level under which the transaction should run.</param>
		/// <returns>An object representing the new transaction.</returns>
		/// <exception cref="T:System.Data.SqlClient.SqlException">Parallel transactions are not allowed when using Multiple Active Result Sets (MARS).</exception>
		/// <exception cref="T:System.InvalidOperationException">Parallel transactions are not supported.</exception>
		public new SqlTransaction BeginTransaction(IsolationLevel iso)
		{
			return this.BeginTransaction(iso, null);
		}

		/// <summary>Starts a database transaction with the specified transaction name.</summary>
		/// <param name="transactionName">The name of the transaction.</param>
		/// <returns>An object representing the new transaction.</returns>
		/// <exception cref="T:System.Data.SqlClient.SqlException">Parallel transactions are not allowed when using Multiple Active Result Sets (MARS).</exception>
		/// <exception cref="T:System.InvalidOperationException">Parallel transactions are not supported.</exception>
		public SqlTransaction BeginTransaction(string transactionName)
		{
			return this.BeginTransaction(IsolationLevel.Unspecified, transactionName);
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			DbTransaction result = this.BeginTransaction(isolationLevel);
			GC.KeepAlive(this);
			return result;
		}

		/// <summary>Starts a database transaction with the specified isolation level and transaction name.</summary>
		/// <param name="iso">The isolation level under which the transaction should run.</param>
		/// <param name="transactionName">The name of the transaction.</param>
		/// <returns>An object representing the new transaction.</returns>
		/// <exception cref="T:System.Data.SqlClient.SqlException">Parallel transactions are not allowed when using Multiple Active Result Sets (MARS).</exception>
		/// <exception cref="T:System.InvalidOperationException">Parallel transactions are not supported.</exception>
		public SqlTransaction BeginTransaction(IsolationLevel iso, string transactionName)
		{
			this.WaitForPendingReconnection();
			SqlStatistics statistics = null;
			SqlTransaction result;
			try
			{
				statistics = SqlStatistics.StartTimer(this.Statistics);
				bool shouldReconnect = true;
				SqlTransaction sqlTransaction;
				do
				{
					sqlTransaction = this.GetOpenTdsConnection().BeginSqlTransaction(iso, transactionName, shouldReconnect);
					shouldReconnect = false;
				}
				while (sqlTransaction.InternalTransaction.ConnectionHasBeenRestored);
				GC.KeepAlive(this);
				result = sqlTransaction;
			}
			finally
			{
				SqlStatistics.StopTimer(statistics);
			}
			return result;
		}

		/// <summary>Changes the current database for an open <see cref="T:System.Data.SqlClient.SqlConnection" />.</summary>
		/// <param name="database">The name of the database to use instead of the current database.</param>
		/// <exception cref="T:System.ArgumentException">The database name is not valid.</exception>
		/// <exception cref="T:System.InvalidOperationException">The connection is not open.</exception>
		/// <exception cref="T:System.Data.SqlClient.SqlException">Cannot change the database.</exception>
		public override void ChangeDatabase(string database)
		{
			SqlStatistics statistics = null;
			this.RepairInnerConnection();
			try
			{
				statistics = SqlStatistics.StartTimer(this.Statistics);
				this.InnerConnection.ChangeDatabase(database);
			}
			finally
			{
				SqlStatistics.StopTimer(statistics);
			}
		}

		/// <summary>Empties the connection pool.</summary>
		public static void ClearAllPools()
		{
			SqlConnectionFactory.SingletonInstance.ClearAllPools();
		}

		/// <summary>Empties the connection pool associated with the specified connection.</summary>
		/// <param name="connection">The <see cref="T:System.Data.SqlClient.SqlConnection" /> to be cleared from the pool.</param>
		public static void ClearPool(SqlConnection connection)
		{
			ADP.CheckArgumentNull(connection, "connection");
			DbConnectionOptions userConnectionOptions = connection.UserConnectionOptions;
			if (userConnectionOptions != null)
			{
				SqlConnectionFactory.SingletonInstance.ClearPool(connection);
			}
		}

		private void CloseInnerConnection()
		{
			this.InnerConnection.CloseConnection(this, this.ConnectionFactory);
		}

		/// <summary>Closes the connection to the database. This is the preferred method of closing any open connection.</summary>
		/// <exception cref="T:System.Data.SqlClient.SqlException">The connection-level error that occurred while opening the connection.</exception>
		public override void Close()
		{
			ConnectionState state = this.State;
			Guid operationId = default(Guid);
			Guid clientConnectionId = default(Guid);
			if (state != ConnectionState.Closed)
			{
				operationId = SqlConnection.s_diagnosticListener.WriteConnectionCloseBefore(this, "Close");
				clientConnectionId = this.ClientConnectionId;
			}
			SqlStatistics statistics = null;
			Exception ex = null;
			try
			{
				statistics = SqlStatistics.StartTimer(this.Statistics);
				Task currentReconnectionTask = this._currentReconnectionTask;
				if (currentReconnectionTask != null && !currentReconnectionTask.IsCompleted)
				{
					CancellationTokenSource reconnectionCancellationSource = this._reconnectionCancellationSource;
					if (reconnectionCancellationSource != null)
					{
						reconnectionCancellationSource.Cancel();
					}
					AsyncHelper.WaitForCompletion(currentReconnectionTask, 0, null, false);
					if (this.State != ConnectionState.Open)
					{
						this.OnStateChange(DbConnectionInternal.StateChangeClosed);
					}
				}
				this.CancelOpenAndWait();
				this.CloseInnerConnection();
				GC.SuppressFinalize(this);
				if (this.Statistics != null)
				{
					ADP.TimerCurrent(out this._statistics._closeTimestamp);
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				SqlStatistics.StopTimer(statistics);
				if (state != ConnectionState.Closed)
				{
					if (ex != null)
					{
						SqlConnection.s_diagnosticListener.WriteConnectionCloseError(operationId, clientConnectionId, this, ex, "Close");
					}
					else
					{
						SqlConnection.s_diagnosticListener.WriteConnectionCloseAfter(operationId, clientConnectionId, this, "Close");
					}
				}
			}
		}

		/// <summary>Creates and returns a <see cref="T:System.Data.SqlClient.SqlCommand" /> object associated with the <see cref="T:System.Data.SqlClient.SqlConnection" />.</summary>
		/// <returns>A <see cref="T:System.Data.SqlClient.SqlCommand" /> object.</returns>
		public new SqlCommand CreateCommand()
		{
			return new SqlCommand(null, this);
		}

		private void DisposeMe(bool disposing)
		{
			this._credential = null;
			this._accessToken = null;
			if (!disposing)
			{
				SqlInternalConnectionTds sqlInternalConnectionTds = this.InnerConnection as SqlInternalConnectionTds;
				if (sqlInternalConnectionTds != null && !sqlInternalConnectionTds.ConnectionOptions.Pooling)
				{
					TdsParser parser = sqlInternalConnectionTds.Parser;
					if (parser != null && parser._physicalStateObj != null)
					{
						parser._physicalStateObj.DecrementPendingCallbacks(false);
					}
				}
			}
		}

		/// <summary>Opens a database connection with the property settings specified by the <see cref="P:System.Data.SqlClient.SqlConnection.ConnectionString" />.</summary>
		/// <exception cref="T:System.InvalidOperationException">Cannot open a connection without specifying a data source or server.  
		///  or  
		///  The connection is already open.</exception>
		/// <exception cref="T:System.Data.SqlClient.SqlException">A connection-level error occurred while opening the connection. If the <see cref="P:System.Data.SqlClient.SqlException.Number" /> property contains the value 18487 or 18488, this indicates that the specified password has expired or must be reset. See the <see cref="M:System.Data.SqlClient.SqlConnection.ChangePassword(System.String,System.String)" /> method for more information.  
		///  The <see langword="&lt;system.data.localdb&gt;" /> tag in the app.config file has invalid or unknown elements.</exception>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">There are two entries with the same name in the <see langword="&lt;localdbinstances&gt;" /> section.</exception>
		public override void Open()
		{
			Guid operationId = SqlConnection.s_diagnosticListener.WriteConnectionOpenBefore(this, "Open");
			this.PrepareStatisticsForNewConnection();
			SqlStatistics statistics = null;
			Exception ex = null;
			try
			{
				statistics = SqlStatistics.StartTimer(this.Statistics);
				if (!this.TryOpen(null))
				{
					throw ADP.InternalError(ADP.InternalErrorCode.SynchronousConnectReturnedPending);
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				SqlStatistics.StopTimer(statistics);
				if (ex != null)
				{
					SqlConnection.s_diagnosticListener.WriteConnectionOpenError(operationId, this, ex, "Open");
				}
				else
				{
					SqlConnection.s_diagnosticListener.WriteConnectionOpenAfter(operationId, this, "Open");
				}
			}
		}

		internal void RegisterWaitingForReconnect(Task waitingTask)
		{
			if (((SqlConnectionString)this.ConnectionOptions).MARS)
			{
				return;
			}
			Interlocked.CompareExchange<Task>(ref this._asyncWaitingForReconnection, waitingTask, null);
			if (this._asyncWaitingForReconnection != waitingTask)
			{
				throw SQL.MARSUnspportedOnConnection();
			}
		}

		private Task ReconnectAsync(int timeout)
		{
			SqlConnection.<ReconnectAsync>d__97 <ReconnectAsync>d__;
			<ReconnectAsync>d__.<>4__this = this;
			<ReconnectAsync>d__.timeout = timeout;
			<ReconnectAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ReconnectAsync>d__.<>1__state = -1;
			<ReconnectAsync>d__.<>t__builder.Start<SqlConnection.<ReconnectAsync>d__97>(ref <ReconnectAsync>d__);
			return <ReconnectAsync>d__.<>t__builder.Task;
		}

		internal Task ValidateAndReconnect(Action beforeDisconnect, int timeout)
		{
			Task task = this._currentReconnectionTask;
			while (task != null && task.IsCompleted)
			{
				Interlocked.CompareExchange<Task>(ref this._currentReconnectionTask, null, task);
				task = this._currentReconnectionTask;
			}
			if (task == null)
			{
				if (this._connectRetryCount > 0)
				{
					SqlInternalConnectionTds openTdsConnection = this.GetOpenTdsConnection();
					if (openTdsConnection._sessionRecoveryAcknowledged && !openTdsConnection.Parser._physicalStateObj.ValidateSNIConnection())
					{
						if (openTdsConnection.Parser._sessionPool != null && openTdsConnection.Parser._sessionPool.ActiveSessionsCount > 0)
						{
							if (beforeDisconnect != null)
							{
								beforeDisconnect();
							}
							this.OnError(SQL.CR_UnrecoverableClient(this.ClientConnectionId), true, null);
						}
						SessionData currentSessionData = openTdsConnection.CurrentSessionData;
						if (currentSessionData._unrecoverableStatesCount == 0)
						{
							bool flag = false;
							object reconnectLock = this._reconnectLock;
							lock (reconnectLock)
							{
								openTdsConnection.CheckEnlistedTransactionBinding();
								task = this._currentReconnectionTask;
								if (task == null)
								{
									if (currentSessionData._unrecoverableStatesCount == 0)
									{
										this._originalConnectionId = this.ClientConnectionId;
										this._recoverySessionData = currentSessionData;
										if (beforeDisconnect != null)
										{
											beforeDisconnect();
										}
										try
										{
											this._suppressStateChangeForReconnection = true;
											openTdsConnection.DoomThisConnection();
										}
										catch (SqlException)
										{
										}
										task = Task.Run(() => this.ReconnectAsync(timeout));
										this._currentReconnectionTask = task;
									}
								}
								else
								{
									flag = true;
								}
							}
							if (flag && beforeDisconnect != null)
							{
								beforeDisconnect();
							}
						}
						else
						{
							if (beforeDisconnect != null)
							{
								beforeDisconnect();
							}
							this.OnError(SQL.CR_UnrecoverableServer(this.ClientConnectionId), true, null);
						}
					}
				}
			}
			else if (beforeDisconnect != null)
			{
				beforeDisconnect();
			}
			return task;
		}

		private void WaitForPendingReconnection()
		{
			Task currentReconnectionTask = this._currentReconnectionTask;
			if (currentReconnectionTask != null && !currentReconnectionTask.IsCompleted)
			{
				AsyncHelper.WaitForCompletion(currentReconnectionTask, 0, null, false);
			}
		}

		private void CancelOpenAndWait()
		{
			Tuple<TaskCompletionSource<DbConnectionInternal>, Task> currentCompletion = this._currentCompletion;
			if (currentCompletion != null)
			{
				currentCompletion.Item1.TrySetCanceled();
				((IAsyncResult)currentCompletion.Item2).AsyncWaitHandle.WaitOne();
			}
		}

		/// <summary>An asynchronous version of <see cref="M:System.Data.SqlClient.SqlConnection.Open" />, which opens a database connection with the property settings specified by the <see cref="P:System.Data.SqlClient.SqlConnection.ConnectionString" />. The cancellation token can be used to request that the operation be abandoned before the connection timeout elapses.  Exceptions will be propagated via the returned Task. If the connection timeout time elapses without successfully connecting, the returned Task will be marked as faulted with an Exception. The implementation returns a Task without blocking the calling thread for both pooled and non-pooled connections.</summary>
		/// <param name="cancellationToken">The cancellation instruction.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">Calling <see cref="M:System.Data.SqlClient.SqlConnection.OpenAsync(System.Threading.CancellationToken)" /> more than once for the same instance before task completion.  
		///  <see langword="Context Connection=true" /> is specified in the connection string.  
		///  A connection was not available from the connection pool before the connection time out elapsed.</exception>
		/// <exception cref="T:System.Data.SqlClient.SqlException">Any error returned by SQL Server that occurred while opening the connection.</exception>
		public override Task OpenAsync(CancellationToken cancellationToken)
		{
			Guid operationId = SqlConnection.s_diagnosticListener.WriteConnectionOpenBefore(this, "OpenAsync");
			this.PrepareStatisticsForNewConnection();
			SqlStatistics statistics = null;
			Task task;
			try
			{
				statistics = SqlStatistics.StartTimer(this.Statistics);
				TaskCompletionSource<DbConnectionInternal> taskCompletionSource = new TaskCompletionSource<DbConnectionInternal>(ADP.GetCurrentTransaction());
				TaskCompletionSource<object> taskCompletionSource2 = new TaskCompletionSource<object>();
				if (SqlConnection.s_diagnosticListener.IsEnabled("System.Data.SqlClient.WriteConnectionOpenAfter") || SqlConnection.s_diagnosticListener.IsEnabled("System.Data.SqlClient.WriteConnectionOpenError"))
				{
					taskCompletionSource2.Task.ContinueWith(delegate(Task<object> t)
					{
						if (t.Exception != null)
						{
							SqlConnection.s_diagnosticListener.WriteConnectionOpenError(operationId, this, t.Exception, "OpenAsync");
							return;
						}
						SqlConnection.s_diagnosticListener.WriteConnectionOpenAfter(operationId, this, "OpenAsync");
					}, TaskScheduler.Default);
				}
				if (cancellationToken.IsCancellationRequested)
				{
					taskCompletionSource2.SetCanceled();
					task = taskCompletionSource2.Task;
				}
				else
				{
					bool flag;
					try
					{
						flag = this.TryOpen(taskCompletionSource);
					}
					catch (Exception ex)
					{
						SqlConnection.s_diagnosticListener.WriteConnectionOpenError(operationId, this, ex, "OpenAsync");
						taskCompletionSource2.SetException(ex);
						return taskCompletionSource2.Task;
					}
					if (flag)
					{
						taskCompletionSource2.SetResult(null);
						task = taskCompletionSource2.Task;
					}
					else
					{
						CancellationTokenRegistration registration = default(CancellationTokenRegistration);
						if (cancellationToken.CanBeCanceled)
						{
							registration = cancellationToken.Register(delegate(object s)
							{
								((TaskCompletionSource<DbConnectionInternal>)s).TrySetCanceled();
							}, taskCompletionSource);
						}
						SqlConnection.OpenAsyncRetry @object = new SqlConnection.OpenAsyncRetry(this, taskCompletionSource, taskCompletionSource2, registration);
						this._currentCompletion = new Tuple<TaskCompletionSource<DbConnectionInternal>, Task>(taskCompletionSource, taskCompletionSource2.Task);
						taskCompletionSource.Task.ContinueWith(new Action<Task<DbConnectionInternal>>(@object.Retry), TaskScheduler.Default);
						task = taskCompletionSource2.Task;
					}
				}
			}
			catch (Exception ex2)
			{
				SqlConnection.s_diagnosticListener.WriteConnectionOpenError(operationId, this, ex2, "OpenAsync");
				throw;
			}
			finally
			{
				SqlStatistics.StopTimer(statistics);
			}
			return task;
		}

		/// <summary>Returns schema information for the data source of this <see cref="T:System.Data.SqlClient.SqlConnection" />. For more information about scheme, see SQL Server Schema Collections.</summary>
		/// <returns>A <see cref="T:System.Data.DataTable" /> that contains schema information.</returns>
		public override DataTable GetSchema()
		{
			return this.GetSchema(DbMetaDataCollectionNames.MetaDataCollections, null);
		}

		/// <summary>Returns schema information for the data source of this <see cref="T:System.Data.SqlClient.SqlConnection" /> using the specified string for the schema name.</summary>
		/// <param name="collectionName">Specifies the name of the schema to return.</param>
		/// <returns>A <see cref="T:System.Data.DataTable" /> that contains schema information.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="collectionName" /> is specified as null.</exception>
		public override DataTable GetSchema(string collectionName)
		{
			return this.GetSchema(collectionName, null);
		}

		/// <summary>Returns schema information for the data source of this <see cref="T:System.Data.SqlClient.SqlConnection" /> using the specified string for the schema name and the specified string array for the restriction values.</summary>
		/// <param name="collectionName">Specifies the name of the schema to return.</param>
		/// <param name="restrictionValues">A set of restriction values for the requested schema.</param>
		/// <returns>A <see cref="T:System.Data.DataTable" /> that contains schema information.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="collectionName" /> is specified as null.</exception>
		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			return this.InnerConnection.GetSchema(this.ConnectionFactory, this.PoolGroup, this, collectionName, restrictionValues);
		}

		private void PrepareStatisticsForNewConnection()
		{
			if (this.StatisticsEnabled || SqlConnection.s_diagnosticListener.IsEnabled("System.Data.SqlClient.WriteCommandAfter") || SqlConnection.s_diagnosticListener.IsEnabled("System.Data.SqlClient.WriteConnectionOpenAfter"))
			{
				if (this._statistics == null)
				{
					this._statistics = new SqlStatistics();
					return;
				}
				this._statistics.ContinueOnNewConnection();
			}
		}

		private bool TryOpen(TaskCompletionSource<DbConnectionInternal> retry)
		{
			SqlConnectionString sqlConnectionString = (SqlConnectionString)this.ConnectionOptions;
			this._applyTransientFaultHandling = (retry == null && sqlConnectionString != null && sqlConnectionString.ConnectRetryCount > 0);
			if (this.ForceNewConnection)
			{
				if (!this.InnerConnection.TryReplaceConnection(this, this.ConnectionFactory, retry, this.UserConnectionOptions))
				{
					return false;
				}
			}
			else if (!this.InnerConnection.TryOpenConnection(this, this.ConnectionFactory, retry, this.UserConnectionOptions))
			{
				return false;
			}
			SqlInternalConnectionTds sqlInternalConnectionTds = (SqlInternalConnectionTds)this.InnerConnection;
			if (!sqlInternalConnectionTds.ConnectionOptions.Pooling)
			{
				GC.ReRegisterForFinalize(this);
			}
			SqlStatistics statistics = this._statistics;
			if (this.StatisticsEnabled || (SqlConnection.s_diagnosticListener.IsEnabled("System.Data.SqlClient.WriteCommandAfter") && statistics != null))
			{
				ADP.TimerCurrent(out this._statistics._openTimestamp);
				sqlInternalConnectionTds.Parser.Statistics = this._statistics;
			}
			else
			{
				sqlInternalConnectionTds.Parser.Statistics = null;
				this._statistics = null;
			}
			return true;
		}

		internal bool HasLocalTransaction
		{
			get
			{
				return this.GetOpenTdsConnection().HasLocalTransaction;
			}
		}

		internal bool HasLocalTransactionFromAPI
		{
			get
			{
				Task currentReconnectionTask = this._currentReconnectionTask;
				return (currentReconnectionTask == null || currentReconnectionTask.IsCompleted) && this.GetOpenTdsConnection().HasLocalTransactionFromAPI;
			}
		}

		internal bool IsKatmaiOrNewer
		{
			get
			{
				return this._currentReconnectionTask != null || this.GetOpenTdsConnection().IsKatmaiOrNewer;
			}
		}

		internal TdsParser Parser
		{
			get
			{
				return this.GetOpenTdsConnection().Parser;
			}
		}

		internal void ValidateConnectionForExecute(string method, SqlCommand command)
		{
			Task asyncWaitingForReconnection = this._asyncWaitingForReconnection;
			if (asyncWaitingForReconnection != null)
			{
				if (!asyncWaitingForReconnection.IsCompleted)
				{
					throw SQL.MARSUnspportedOnConnection();
				}
				Interlocked.CompareExchange<Task>(ref this._asyncWaitingForReconnection, null, asyncWaitingForReconnection);
			}
			if (this._currentReconnectionTask != null)
			{
				Task currentReconnectionTask = this._currentReconnectionTask;
				if (currentReconnectionTask != null && !currentReconnectionTask.IsCompleted)
				{
					return;
				}
			}
			this.GetOpenTdsConnection(method).ValidateConnectionForExecute(command);
		}

		internal static string FixupDatabaseTransactionName(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				return SqlServerEscapeHelper.EscapeIdentifier(name);
			}
			return name;
		}

		internal void OnError(SqlException exception, bool breakConnection, Action<Action> wrapCloseInAction)
		{
			if (breakConnection && ConnectionState.Open == this.State)
			{
				if (wrapCloseInAction != null)
				{
					int capturedCloseCount = this._closeCount;
					Action obj = delegate()
					{
						if (capturedCloseCount == this._closeCount)
						{
							this.Close();
						}
					};
					wrapCloseInAction(obj);
				}
				else
				{
					this.Close();
				}
			}
			if (exception.Class >= 11)
			{
				throw exception;
			}
			this.OnInfoMessage(new SqlInfoMessageEventArgs(exception));
		}

		internal SqlInternalConnectionTds GetOpenTdsConnection()
		{
			SqlInternalConnectionTds sqlInternalConnectionTds = this.InnerConnection as SqlInternalConnectionTds;
			if (sqlInternalConnectionTds == null)
			{
				throw ADP.ClosedConnectionError();
			}
			return sqlInternalConnectionTds;
		}

		internal SqlInternalConnectionTds GetOpenTdsConnection(string method)
		{
			SqlInternalConnectionTds sqlInternalConnectionTds = this.InnerConnection as SqlInternalConnectionTds;
			if (sqlInternalConnectionTds == null)
			{
				throw ADP.OpenConnectionRequired(method, this.InnerConnection.State);
			}
			return sqlInternalConnectionTds;
		}

		internal void OnInfoMessage(SqlInfoMessageEventArgs imevent)
		{
			bool flag;
			this.OnInfoMessage(imevent, out flag);
		}

		internal void OnInfoMessage(SqlInfoMessageEventArgs imevent, out bool notified)
		{
			SqlInfoMessageEventHandler infoMessage = this.InfoMessage;
			if (infoMessage != null)
			{
				notified = true;
				try
				{
					infoMessage(this, imevent);
					return;
				}
				catch (Exception e)
				{
					if (!ADP.IsCatchableOrSecurityExceptionType(e))
					{
						throw;
					}
					return;
				}
			}
			notified = false;
		}

		/// <summary>Changes the SQL Server password for the user indicated in the connection string to the supplied new password.</summary>
		/// <param name="connectionString">The connection string that contains enough information to connect to the server that you want. The connection string must contain the user ID and the current password.</param>
		/// <param name="newPassword">The new password to set. This password must comply with any password security policy set on the server, including minimum length, requirements for specific characters, and so on.</param>
		/// <exception cref="T:System.ArgumentException">The connection string includes the option to use integrated security.  
		///  Or  
		///  The <paramref name="newPassword" /> exceeds 128 characters.</exception>
		/// <exception cref="T:System.ArgumentNullException">Either the <paramref name="connectionString" /> or the <paramref name="newPassword" /> parameter is null.</exception>
		public static void ChangePassword(string connectionString, string newPassword)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				throw SQL.ChangePasswordArgumentMissing("newPassword");
			}
			if (string.IsNullOrEmpty(newPassword))
			{
				throw SQL.ChangePasswordArgumentMissing("newPassword");
			}
			if (128 < newPassword.Length)
			{
				throw ADP.InvalidArgumentLength("newPassword", 128);
			}
			SqlConnectionString sqlConnectionString = SqlConnectionFactory.FindSqlConnectionOptions(new SqlConnectionPoolKey(connectionString, null, null));
			if (sqlConnectionString.IntegratedSecurity)
			{
				throw SQL.ChangePasswordConflictsWithSSPI();
			}
			if (!string.IsNullOrEmpty(sqlConnectionString.AttachDBFilename))
			{
				throw SQL.ChangePasswordUseOfUnallowedKey("attachdbfilename");
			}
			SqlConnection.ChangePassword(connectionString, sqlConnectionString, null, newPassword, null);
		}

		/// <summary>Changes the SQL Server password for the user indicated in the <see cref="T:System.Data.SqlClient.SqlCredential" /> object.</summary>
		/// <param name="connectionString">The connection string that contains enough information to connect to a server. The connection string should not use any of the following connection string keywords: <see langword="Integrated Security = true" />, <see langword="UserId" />, or <see langword="Password" />; or <see langword="ContextConnection = true" />.</param>
		/// <param name="credential">A <see cref="T:System.Data.SqlClient.SqlCredential" /> object.</param>
		/// <param name="newSecurePassword">The new password. <paramref name="newSecurePassword" /> must be read only. The password must also comply with any password security policy set on the server (for example, minimum length and requirements for specific characters).</param>
		/// <exception cref="T:System.ArgumentException">The connection string contains any combination of <see langword="UserId" />, <see langword="Password" />, or <see langword="Integrated Security=true" />.
		/// -or-
		/// The connection string contains <see langword="Context Connection=true" />.  
		/// -or-
		/// <paramref name="newSecurePassword" /> (or <paramref name="newPassword" />) is greater than 128 characters.
		/// -or-
		/// <paramref name="newSecurePassword" /> (or <paramref name="newPassword" />) is not read only.
		/// -or-
		/// <paramref name="newSecurePassword" /> (or <paramref name="newPassword" />) is an empty string.</exception>
		/// <exception cref="T:System.ArgumentNullException">One of the parameters (<paramref name="connectionString" />, <paramref name="credential" />, or <paramref name="newSecurePassword" />) is null.</exception>
		public static void ChangePassword(string connectionString, SqlCredential credential, SecureString newSecurePassword)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				throw SQL.ChangePasswordArgumentMissing("connectionString");
			}
			if (credential == null)
			{
				throw SQL.ChangePasswordArgumentMissing("credential");
			}
			if (newSecurePassword == null || newSecurePassword.Length == 0)
			{
				throw SQL.ChangePasswordArgumentMissing("newSecurePassword");
			}
			if (!newSecurePassword.IsReadOnly())
			{
				throw ADP.MustBeReadOnly("newSecurePassword");
			}
			if (128 < newSecurePassword.Length)
			{
				throw ADP.InvalidArgumentLength("newSecurePassword", 128);
			}
			SqlConnectionString sqlConnectionString = SqlConnectionFactory.FindSqlConnectionOptions(new SqlConnectionPoolKey(connectionString, null, null));
			if (!string.IsNullOrEmpty(sqlConnectionString.UserID) || !string.IsNullOrEmpty(sqlConnectionString.Password))
			{
				throw ADP.InvalidMixedArgumentOfSecureAndClearCredential();
			}
			if (sqlConnectionString.IntegratedSecurity)
			{
				throw SQL.ChangePasswordConflictsWithSSPI();
			}
			if (!string.IsNullOrEmpty(sqlConnectionString.AttachDBFilename))
			{
				throw SQL.ChangePasswordUseOfUnallowedKey("attachdbfilename");
			}
			SqlConnection.ChangePassword(connectionString, sqlConnectionString, credential, null, newSecurePassword);
		}

		private static void ChangePassword(string connectionString, SqlConnectionString connectionOptions, SqlCredential credential, string newPassword, SecureString newSecurePassword)
		{
			SqlInternalConnectionTds sqlInternalConnectionTds = null;
			try
			{
				sqlInternalConnectionTds = new SqlInternalConnectionTds(null, connectionOptions, credential, null, newPassword, newSecurePassword, false, null, null, false, null);
			}
			finally
			{
				if (sqlInternalConnectionTds != null)
				{
					sqlInternalConnectionTds.Dispose();
				}
			}
			SqlConnectionPoolKey key = new SqlConnectionPoolKey(connectionString, null, null);
			SqlConnectionFactory.SingletonInstance.ClearPool(key);
		}

		internal void RegisterForConnectionCloseNotification<T>(ref Task<T> outerTask, object value, int tag)
		{
			outerTask = outerTask.ContinueWith<Task<T>>(delegate(Task<T> task)
			{
				this.RemoveWeakReference(value);
				return task;
			}, TaskScheduler.Default).Unwrap<T>();
		}

		/// <summary>If statistics gathering is enabled, all values are reset to zero.</summary>
		public void ResetStatistics()
		{
			if (this.Statistics != null)
			{
				this.Statistics.Reset();
				if (ConnectionState.Open == this.State)
				{
					ADP.TimerCurrent(out this._statistics._openTimestamp);
				}
			}
		}

		/// <summary>Returns a name value pair collection of statistics at the point in time the method is called.</summary>
		/// <returns>Returns a reference of type <see cref="T:System.Collections.IDictionary" /> of <see cref="T:System.Collections.DictionaryEntry" /> items.</returns>
		public IDictionary RetrieveStatistics()
		{
			if (this.Statistics != null)
			{
				this.UpdateStatistics();
				return this.Statistics.GetDictionary();
			}
			return new SqlStatistics().GetDictionary();
		}

		private void UpdateStatistics()
		{
			if (ConnectionState.Open == this.State)
			{
				ADP.TimerCurrent(out this._statistics._closeTimestamp);
			}
			this.Statistics.UpdateStatistics();
		}

		/// <summary>Creates a new object that is a copy of the current instance.</summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		object ICloneable.Clone()
		{
			return new SqlConnection(this);
		}

		private void CopyFrom(SqlConnection connection)
		{
			ADP.CheckArgumentNull(connection, "connection");
			this._userConnectionOptions = connection.UserConnectionOptions;
			this._poolGroup = connection.PoolGroup;
			if (DbConnectionClosedNeverOpened.SingletonInstance == connection._innerConnection)
			{
				this._innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
				return;
			}
			this._innerConnection = DbConnectionClosedPreviouslyOpened.SingletonInstance;
		}

		private Assembly ResolveTypeAssembly(AssemblyName asmRef, bool throwOnError)
		{
			if (string.Compare(asmRef.Name, "Microsoft.SqlServer.Types", StringComparison.OrdinalIgnoreCase) == 0)
			{
				asmRef.Version = this.TypeSystemAssemblyVersion;
			}
			Assembly result;
			try
			{
				result = Assembly.Load(asmRef);
			}
			catch (Exception e)
			{
				if (throwOnError || !ADP.IsCatchableExceptionType(e))
				{
					throw;
				}
				result = null;
			}
			return result;
		}

		internal void CheckGetExtendedUDTInfo(SqlMetaDataPriv metaData, bool fThrow)
		{
			if (metaData.udtType == null)
			{
				metaData.udtType = Type.GetType(metaData.udtAssemblyQualifiedName, (AssemblyName asmRef) => this.ResolveTypeAssembly(asmRef, fThrow), null, fThrow);
				if (fThrow && metaData.udtType == null)
				{
					throw SQL.UDTUnexpectedResult(metaData.udtAssemblyQualifiedName);
				}
			}
		}

		internal object GetUdtValue(object value, SqlMetaDataPriv metaData, bool returnDBNull)
		{
			if (returnDBNull && ADP.IsNull(value))
			{
				return DBNull.Value;
			}
			if (ADP.IsNull(value))
			{
				return metaData.udtType.InvokeMember("Null", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty, null, null, new object[0], CultureInfo.InvariantCulture);
			}
			return SerializationHelperSql9.Deserialize(new MemoryStream((byte[])value), metaData.udtType);
		}

		internal byte[] GetBytes(object o)
		{
			Format format = Format.Native;
			int num;
			return this.GetBytes(o, out format, out num);
		}

		internal byte[] GetBytes(object o, out Format format, out int maxSize)
		{
			SqlUdtInfo infoFromType = this.GetInfoFromType(o.GetType());
			maxSize = infoFromType.MaxByteSize;
			format = infoFromType.SerializationFormat;
			if (maxSize < -1 || maxSize >= 65535)
			{
				Type type = o.GetType();
				throw new InvalidOperationException(((type != null) ? type.ToString() : null) + ": invalid Size");
			}
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream((maxSize < 0) ? 0 : maxSize))
			{
				SerializationHelperSql9.Serialize(memoryStream, o);
				result = memoryStream.ToArray();
			}
			return result;
		}

		private SqlUdtInfo GetInfoFromType(Type t)
		{
			Type type = t;
			SqlUdtInfo sqlUdtInfo;
			for (;;)
			{
				sqlUdtInfo = SqlUdtInfo.TryGetFromType(t);
				if (sqlUdtInfo != null)
				{
					break;
				}
				t = t.BaseType;
				if (!(t != null))
				{
					goto Block_2;
				}
			}
			return sqlUdtInfo;
			Block_2:
			throw SQL.UDTInvalidSqlType(type.AssemblyQualifiedName);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlClient.SqlConnection" /> class.</summary>
		public SqlConnection()
		{
			this._reconnectLock = new object();
			this._originalConnectionId = Guid.Empty;
			base..ctor();
			GC.SuppressFinalize(this);
			this._innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
		}

		internal int CloseCount
		{
			get
			{
				return this._closeCount;
			}
		}

		internal DbConnectionFactory ConnectionFactory
		{
			get
			{
				return SqlConnection.s_connectionFactory;
			}
		}

		internal DbConnectionOptions ConnectionOptions
		{
			get
			{
				DbConnectionPoolGroup poolGroup = this.PoolGroup;
				if (poolGroup == null)
				{
					return null;
				}
				return poolGroup.ConnectionOptions;
			}
		}

		private string ConnectionString_Get()
		{
			bool shouldHidePassword = this.InnerConnection.ShouldHidePassword;
			DbConnectionOptions userConnectionOptions = this.UserConnectionOptions;
			if (userConnectionOptions == null)
			{
				return "";
			}
			return userConnectionOptions.UsersConnectionString(shouldHidePassword);
		}

		private void ConnectionString_Set(DbConnectionPoolKey key)
		{
			DbConnectionOptions userConnectionOptions = null;
			DbConnectionPoolGroup connectionPoolGroup = this.ConnectionFactory.GetConnectionPoolGroup(key, null, ref userConnectionOptions);
			DbConnectionInternal innerConnection = this.InnerConnection;
			bool flag = innerConnection.AllowSetConnectionString;
			if (flag)
			{
				flag = this.SetInnerConnectionFrom(DbConnectionClosedBusy.SingletonInstance, innerConnection);
				if (flag)
				{
					this._userConnectionOptions = userConnectionOptions;
					this._poolGroup = connectionPoolGroup;
					this._innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
				}
			}
			if (!flag)
			{
				throw ADP.OpenConnectionPropertySet("ConnectionString", innerConnection.State);
			}
		}

		internal DbConnectionInternal InnerConnection
		{
			get
			{
				return this._innerConnection;
			}
		}

		internal DbConnectionPoolGroup PoolGroup
		{
			get
			{
				return this._poolGroup;
			}
			set
			{
				this._poolGroup = value;
			}
		}

		internal DbConnectionOptions UserConnectionOptions
		{
			get
			{
				return this._userConnectionOptions;
			}
		}

		internal void Abort(Exception e)
		{
			DbConnectionInternal innerConnection = this._innerConnection;
			if (ConnectionState.Open == innerConnection.State)
			{
				Interlocked.CompareExchange<DbConnectionInternal>(ref this._innerConnection, DbConnectionClosedPreviouslyOpened.SingletonInstance, innerConnection);
				innerConnection.DoomThisConnection();
			}
		}

		internal void AddWeakReference(object value, int tag)
		{
			this.InnerConnection.AddWeakReference(value, tag);
		}

		protected override DbCommand CreateDbCommand()
		{
			DbCommand dbCommand = this.ConnectionFactory.ProviderFactory.CreateCommand();
			dbCommand.Connection = this;
			return dbCommand;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._userConnectionOptions = null;
				this._poolGroup = null;
				this.Close();
			}
			this.DisposeMe(disposing);
			base.Dispose(disposing);
		}

		private void RepairInnerConnection()
		{
			this.WaitForPendingReconnection();
			if (this._connectRetryCount == 0)
			{
				return;
			}
			SqlInternalConnectionTds sqlInternalConnectionTds = this.InnerConnection as SqlInternalConnectionTds;
			if (sqlInternalConnectionTds != null)
			{
				sqlInternalConnectionTds.ValidateConnectionForExecute(null);
				sqlInternalConnectionTds.GetSessionAndReconnectIfNeeded(this, 0);
			}
		}

		/// <summary>Enlists in the specified transaction as a distributed transaction.</summary>
		/// <param name="transaction">A reference to an existing <see cref="T:System.Transactions.Transaction" /> in which to enlist.</param>
		public override void EnlistTransaction(Transaction transaction)
		{
			Transaction enlistedTransaction = this.InnerConnection.EnlistedTransaction;
			if (enlistedTransaction != null)
			{
				if (enlistedTransaction.Equals(transaction))
				{
					return;
				}
				if (enlistedTransaction.TransactionInformation.Status == System.Transactions.TransactionStatus.Active)
				{
					throw ADP.TransactionPresent();
				}
			}
			this.RepairInnerConnection();
			this.InnerConnection.EnlistTransaction(transaction);
			GC.KeepAlive(this);
		}

		internal void NotifyWeakReference(int message)
		{
			this.InnerConnection.NotifyWeakReference(message);
		}

		internal void PermissionDemand()
		{
			DbConnectionPoolGroup poolGroup = this.PoolGroup;
			DbConnectionOptions dbConnectionOptions = (poolGroup != null) ? poolGroup.ConnectionOptions : null;
			if (dbConnectionOptions == null || dbConnectionOptions.IsEmpty)
			{
				throw ADP.NoConnectionString();
			}
			DbConnectionOptions userConnectionOptions = this.UserConnectionOptions;
		}

		internal void RemoveWeakReference(object value)
		{
			this.InnerConnection.RemoveWeakReference(value);
		}

		internal void SetInnerConnectionEvent(DbConnectionInternal to)
		{
			ConnectionState connectionState = this._innerConnection.State & ConnectionState.Open;
			ConnectionState connectionState2 = to.State & ConnectionState.Open;
			if (connectionState != connectionState2 && connectionState2 == ConnectionState.Closed)
			{
				this._closeCount++;
			}
			this._innerConnection = to;
			if (connectionState == ConnectionState.Closed && ConnectionState.Open == connectionState2)
			{
				this.OnStateChange(DbConnectionInternal.StateChangeOpen);
				return;
			}
			if (ConnectionState.Open == connectionState && connectionState2 == ConnectionState.Closed)
			{
				this.OnStateChange(DbConnectionInternal.StateChangeClosed);
				return;
			}
			if (connectionState != connectionState2)
			{
				this.OnStateChange(new StateChangeEventArgs(connectionState, connectionState2));
			}
		}

		internal bool SetInnerConnectionFrom(DbConnectionInternal to, DbConnectionInternal from)
		{
			return from == Interlocked.CompareExchange<DbConnectionInternal>(ref this._innerConnection, to, from);
		}

		internal void SetInnerConnectionTo(DbConnectionInternal to)
		{
			this._innerConnection = to;
		}

		[MonoTODO]
		public SqlCredential Credentials
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Enlists in the specified transaction as a distributed transaction.</summary>
		/// <param name="transaction">A reference to an existing <see cref="T:System.EnterpriseServices.ITransaction" /> in which to enlist.</param>
		[MonoTODO]
		public void EnlistDistributedTransaction(ITransaction transaction)
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets or sets the time-to-live for column encryption key entries in the column encryption key cache for the Always Encrypted feature. The default value is 2 hours. 0 means no caching at all.</summary>
		/// <returns>The time interval.</returns>
		public static TimeSpan ColumnEncryptionKeyCacheTtl
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(TimeSpan);
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		/// <summary>Gets or sets a value that indicates whether query metadata caching is enabled (true) or not (false) for parameterized queries running against Always Encrypted enabled databases. The default value is true.</summary>
		/// <returns>Returns true if query metadata caching is enabled; otherwise false. true is the default.</returns>
		public static bool ColumnEncryptionQueryMetadataCacheEnabled
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		/// <summary>Allows you to set a list of trusted key paths for a database server. If while processing an application query the driver receives a key path that is not on the list, the query will fail. This property provides additional protection against security attacks that involve a compromised SQL Server providing fake key paths, which may lead to leaking key store credentials.</summary>
		/// <returns>The list of trusted master key paths for the column encryption.</returns>
		public static IDictionary<string, IList<string>> ColumnEncryptionTrustedMasterKeyPaths
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		/// <summary>Registers the column encryption key store providers.</summary>
		/// <param name="customProviders">The custom providers</param>
		public static void RegisterColumnEncryptionKeyStoreProviders(IDictionary<string, SqlColumnEncryptionKeyStoreProvider> customProviders)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private bool _AsyncCommandInProgress;

		internal SqlStatistics _statistics;

		private bool _collectstats;

		private bool _fireInfoMessageEventOnUserErrors;

		private Tuple<TaskCompletionSource<DbConnectionInternal>, Task> _currentCompletion;

		private SqlCredential _credential;

		private string _connectionString;

		private int _connectRetryCount;

		private string _accessToken;

		private object _reconnectLock;

		internal Task _currentReconnectionTask;

		private Task _asyncWaitingForReconnection;

		private Guid _originalConnectionId;

		private CancellationTokenSource _reconnectionCancellationSource;

		internal SessionData _recoverySessionData;

		internal new bool _suppressStateChangeForReconnection;

		private int _reconnectCount;

		private static readonly DiagnosticListener s_diagnosticListener = new DiagnosticListener("SqlClientDiagnosticListener");

		internal bool _applyTransientFaultHandling;

		private static readonly DbConnectionFactory s_connectionFactory = SqlConnectionFactory.SingletonInstance;

		private DbConnectionOptions _userConnectionOptions;

		private DbConnectionPoolGroup _poolGroup;

		private DbConnectionInternal _innerConnection;

		private int _closeCount;

		private class OpenAsyncRetry
		{
			public OpenAsyncRetry(SqlConnection parent, TaskCompletionSource<DbConnectionInternal> retry, TaskCompletionSource<object> result, CancellationTokenRegistration registration)
			{
				this._parent = parent;
				this._retry = retry;
				this._result = result;
				this._registration = registration;
			}

			internal void Retry(Task<DbConnectionInternal> retryTask)
			{
				this._registration.Dispose();
				try
				{
					SqlStatistics statistics = null;
					try
					{
						statistics = SqlStatistics.StartTimer(this._parent.Statistics);
						if (retryTask.IsFaulted)
						{
							Exception innerException = retryTask.Exception.InnerException;
							this._parent.CloseInnerConnection();
							this._parent._currentCompletion = null;
							this._result.SetException(retryTask.Exception.InnerException);
						}
						else if (retryTask.IsCanceled)
						{
							this._parent.CloseInnerConnection();
							this._parent._currentCompletion = null;
							this._result.SetCanceled();
						}
						else
						{
							DbConnectionInternal innerConnection = this._parent.InnerConnection;
							bool flag2;
							lock (innerConnection)
							{
								flag2 = this._parent.TryOpen(this._retry);
							}
							if (flag2)
							{
								this._parent._currentCompletion = null;
								this._result.SetResult(null);
							}
							else
							{
								this._parent.CloseInnerConnection();
								this._parent._currentCompletion = null;
								this._result.SetException(ADP.ExceptionWithStackTrace(ADP.InternalError(ADP.InternalErrorCode.CompletedConnectReturnedPending)));
							}
						}
					}
					finally
					{
						SqlStatistics.StopTimer(statistics);
					}
				}
				catch (Exception exception)
				{
					this._parent.CloseInnerConnection();
					this._parent._currentCompletion = null;
					this._result.SetException(exception);
				}
			}

			private SqlConnection _parent;

			private TaskCompletionSource<DbConnectionInternal> _retry;

			private TaskCompletionSource<object> _result;

			private CancellationTokenRegistration _registration;
		}
	}
}
