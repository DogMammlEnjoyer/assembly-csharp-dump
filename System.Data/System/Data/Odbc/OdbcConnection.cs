using System;
using System.ComponentModel;
using System.Data.Common;
using System.Data.ProviderBase;
using System.EnterpriseServices;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Transactions;
using Unity;

namespace System.Data.Odbc
{
	/// <summary>Represents an open connection to a data source.</summary>
	public sealed class OdbcConnection : DbConnection, ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Odbc.OdbcConnection" /> class with the specified connection string.</summary>
		/// <param name="connectionString">The connection used to open the data source.</param>
		public OdbcConnection(string connectionString) : this()
		{
			this.ConnectionString = connectionString;
		}

		private OdbcConnection(OdbcConnection connection) : this()
		{
			this.CopyFrom(connection);
			this._connectionTimeout = connection._connectionTimeout;
		}

		internal OdbcConnectionHandle ConnectionHandle
		{
			get
			{
				return this._connectionHandle;
			}
			set
			{
				this._connectionHandle = value;
			}
		}

		/// <summary>Gets or sets the string used to open a data source.</summary>
		/// <returns>The ODBC driver connection string that includes settings, such as the data source name, needed to establish the initial connection. The default value is an empty string (""). The maximum length is 1024 characters.</returns>
		public override string ConnectionString
		{
			get
			{
				return this.ConnectionString_Get();
			}
			set
			{
				this.ConnectionString_Set(value);
			}
		}

		/// <summary>Gets or sets the time to wait while trying to establish a connection before terminating the attempt and generating an error.</summary>
		/// <returns>The time in seconds to wait for a connection to open. The default value is 15 seconds.</returns>
		/// <exception cref="T:System.ArgumentException">The value set is less than 0.</exception>
		[DefaultValue(15)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int ConnectionTimeout
		{
			get
			{
				return this._connectionTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw ODBC.NegativeArgument();
				}
				if (this.IsOpen)
				{
					throw ODBC.CantSetPropertyOnOpenConnection();
				}
				this._connectionTimeout = value;
			}
		}

		/// <summary>Gets the name of the current database or the database to be used after a connection is opened.</summary>
		/// <returns>The name of the current database. The default value is an empty string ("") until the connection is opened.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Database
		{
			get
			{
				if (this.IsOpen && !this.ProviderInfo.NoCurrentCatalog)
				{
					return this.GetConnectAttrString(ODBC32.SQL_ATTR.CURRENT_CATALOG);
				}
				return string.Empty;
			}
		}

		/// <summary>Gets the server name or file name of the data source.</summary>
		/// <returns>The server name or file name of the data source. The default value is an empty string ("") until the connection is opened.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override string DataSource
		{
			get
			{
				if (this.IsOpen)
				{
					return this.GetInfoStringUnhandled(ODBC32.SQL_INFO.SERVER_NAME, true);
				}
				return string.Empty;
			}
		}

		/// <summary>Gets a string that contains the version of the server to which the client is connected.</summary>
		/// <returns>The version of the connected server.</returns>
		/// <exception cref="T:System.InvalidOperationException">The connection is closed.</exception>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override string ServerVersion
		{
			get
			{
				return this.InnerConnection.ServerVersion;
			}
		}

		/// <summary>Gets the current state of the connection.</summary>
		/// <returns>A bitwise combination of the <see cref="T:System.Data.ConnectionState" /> values. The default is <see langword="Closed" />.</returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ConnectionState State
		{
			get
			{
				return this.InnerConnection.State;
			}
		}

		internal OdbcConnectionPoolGroupProviderInfo ProviderInfo
		{
			get
			{
				return (OdbcConnectionPoolGroupProviderInfo)this.PoolGroup.ProviderInfo;
			}
		}

		internal ConnectionState InternalState
		{
			get
			{
				return this.State | this._extraState;
			}
		}

		internal bool IsOpen
		{
			get
			{
				return this.InnerConnection is OdbcConnectionOpen;
			}
		}

		internal OdbcTransaction LocalTransaction
		{
			get
			{
				OdbcTransaction result = null;
				if (this._weakTransaction != null)
				{
					result = (OdbcTransaction)this._weakTransaction.Target;
				}
				return result;
			}
			set
			{
				this._weakTransaction = null;
				if (value != null)
				{
					this._weakTransaction = new WeakReference(value);
				}
			}
		}

		/// <summary>Gets the name of the ODBC driver specified for the current connection.</summary>
		/// <returns>The name of the ODBC driver. This typically is the DLL name (for example, Sqlsrv32.dll). The default value is an empty string ("") until the connection is opened.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string Driver
		{
			get
			{
				if (this.IsOpen)
				{
					if (this.ProviderInfo.DriverName == null)
					{
						this.ProviderInfo.DriverName = this.GetInfoStringUnhandled(ODBC32.SQL_INFO.DRIVER_NAME);
					}
					return this.ProviderInfo.DriverName;
				}
				return ADP.StrEmpty;
			}
		}

		internal bool IsV3Driver
		{
			get
			{
				if (this.ProviderInfo.DriverVersion == null)
				{
					this.ProviderInfo.DriverVersion = this.GetInfoStringUnhandled(ODBC32.SQL_INFO.DRIVER_ODBC_VER);
					if (this.ProviderInfo.DriverVersion != null && this.ProviderInfo.DriverVersion.Length >= 2)
					{
						try
						{
							this.ProviderInfo.IsV3Driver = (int.Parse(this.ProviderInfo.DriverVersion.Substring(0, 2), CultureInfo.InvariantCulture) >= 3);
							goto IL_95;
						}
						catch (FormatException e)
						{
							this.ProviderInfo.IsV3Driver = false;
							ADP.TraceExceptionWithoutRethrow(e);
							goto IL_95;
						}
					}
					this.ProviderInfo.DriverVersion = "";
				}
				IL_95:
				return this.ProviderInfo.IsV3Driver;
			}
		}

		/// <summary>Occurs when the ODBC driver sends a warning or an informational message.</summary>
		public event OdbcInfoMessageEventHandler InfoMessage
		{
			add
			{
				this._infoMessageEventHandler = (OdbcInfoMessageEventHandler)Delegate.Combine(this._infoMessageEventHandler, value);
			}
			remove
			{
				this._infoMessageEventHandler = (OdbcInfoMessageEventHandler)Delegate.Remove(this._infoMessageEventHandler, value);
			}
		}

		internal char EscapeChar(string method)
		{
			this.CheckState(method);
			if (!this.ProviderInfo.HasEscapeChar)
			{
				string infoStringUnhandled = this.GetInfoStringUnhandled(ODBC32.SQL_INFO.SEARCH_PATTERN_ESCAPE);
				this.ProviderInfo.EscapeChar = ((infoStringUnhandled.Length == 1) ? infoStringUnhandled[0] : this.QuoteChar(method)[0]);
			}
			return this.ProviderInfo.EscapeChar;
		}

		internal string QuoteChar(string method)
		{
			this.CheckState(method);
			if (!this.ProviderInfo.HasQuoteChar)
			{
				string infoStringUnhandled = this.GetInfoStringUnhandled(ODBC32.SQL_INFO.IDENTIFIER_QUOTE_CHAR);
				this.ProviderInfo.QuoteChar = ((1 == infoStringUnhandled.Length) ? infoStringUnhandled : "\0");
			}
			return this.ProviderInfo.QuoteChar;
		}

		/// <summary>Starts a transaction at the data source.</summary>
		/// <returns>An object representing the new transaction.</returns>
		/// <exception cref="T:System.InvalidOperationException">A transaction is currently active. Parallel transactions are not supported.</exception>
		public new OdbcTransaction BeginTransaction()
		{
			return this.BeginTransaction(IsolationLevel.Unspecified);
		}

		/// <summary>Starts a transaction at the data source with the specified <see cref="T:System.Data.IsolationLevel" /> value.</summary>
		/// <param name="isolevel">The transaction isolation level for this connection. If you do not specify an isolation level, the default isolation level for the driver is used.</param>
		/// <returns>An object representing the new transaction.</returns>
		/// <exception cref="T:System.InvalidOperationException">A transaction is currently active. Parallel transactions are not supported.</exception>
		public new OdbcTransaction BeginTransaction(IsolationLevel isolevel)
		{
			return (OdbcTransaction)this.InnerConnection.BeginTransaction(isolevel);
		}

		private void RollbackDeadTransaction()
		{
			WeakReference weakTransaction = this._weakTransaction;
			if (weakTransaction != null && !weakTransaction.IsAlive)
			{
				this._weakTransaction = null;
				this.ConnectionHandle.CompleteTransaction(1);
			}
		}

		/// <summary>Changes the current database associated with an open <see cref="T:System.Data.Odbc.OdbcConnection" />.</summary>
		/// <param name="value">The database name.</param>
		/// <exception cref="T:System.ArgumentException">The database name is not valid.</exception>
		/// <exception cref="T:System.InvalidOperationException">The connection is not open.</exception>
		/// <exception cref="T:System.Data.Odbc.OdbcException">Cannot change the database.</exception>
		public override void ChangeDatabase(string value)
		{
			this.InnerConnection.ChangeDatabase(value);
		}

		internal void CheckState(string method)
		{
			ConnectionState internalState = this.InternalState;
			if (ConnectionState.Open != internalState)
			{
				throw ADP.OpenConnectionRequired(method, internalState);
			}
		}

		/// <summary>For a description of this member, see <see cref="M:System.ICloneable.Clone" />.</summary>
		/// <returns>A new <see cref="T:System.Object" /> that is a copy of this instance.</returns>
		object ICloneable.Clone()
		{
			return new OdbcConnection(this);
		}

		internal bool ConnectionIsAlive(Exception innerException)
		{
			if (this.IsOpen)
			{
				if (!this.ProviderInfo.NoConnectionDead)
				{
					int connectAttr = this.GetConnectAttr(ODBC32.SQL_ATTR.CONNECTION_DEAD, ODBC32.HANDLER.IGNORE);
					if (1 == connectAttr)
					{
						this.Close();
						throw ADP.ConnectionIsDisabled(innerException);
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>Creates and returns an <see cref="T:System.Data.Odbc.OdbcCommand" /> object associated with the <see cref="T:System.Data.Odbc.OdbcConnection" />.</summary>
		/// <returns>An <see cref="T:System.Data.Odbc.OdbcCommand" /> object.</returns>
		public new OdbcCommand CreateCommand()
		{
			return new OdbcCommand(string.Empty, this);
		}

		internal OdbcStatementHandle CreateStatementHandle()
		{
			return new OdbcStatementHandle(this.ConnectionHandle);
		}

		/// <summary>Closes the connection to the data source.</summary>
		public override void Close()
		{
			this.InnerConnection.CloseConnection(this, this.ConnectionFactory);
			OdbcConnectionHandle connectionHandle = this._connectionHandle;
			if (connectionHandle != null)
			{
				this._connectionHandle = null;
				WeakReference weakTransaction = this._weakTransaction;
				if (weakTransaction != null)
				{
					this._weakTransaction = null;
					IDisposable disposable = weakTransaction.Target as OdbcTransaction;
					if (disposable != null && weakTransaction.IsAlive)
					{
						disposable.Dispose();
					}
				}
				connectionHandle.Dispose();
			}
		}

		private void DisposeMe(bool disposing)
		{
		}

		internal string GetConnectAttrString(ODBC32.SQL_ATTR attribute)
		{
			string result = "";
			int num = 0;
			byte[] array = new byte[100];
			OdbcConnectionHandle connectionHandle = this.ConnectionHandle;
			if (connectionHandle != null)
			{
				ODBC32.RetCode connectionAttribute = connectionHandle.GetConnectionAttribute(attribute, array, out num);
				if (array.Length + 2 <= num)
				{
					array = new byte[num + 2];
					connectionAttribute = connectionHandle.GetConnectionAttribute(attribute, array, out num);
				}
				if (connectionAttribute == ODBC32.RetCode.SUCCESS || ODBC32.RetCode.SUCCESS_WITH_INFO == connectionAttribute)
				{
					result = (BitConverter.IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode).GetString(array, 0, Math.Min(num, array.Length));
				}
				else if (connectionAttribute == ODBC32.RetCode.ERROR)
				{
					string diagSqlState = this.GetDiagSqlState();
					if ("HYC00" == diagSqlState || "HY092" == diagSqlState || "IM001" == diagSqlState)
					{
						this.FlagUnsupportedConnectAttr(attribute);
					}
				}
			}
			return result;
		}

		internal int GetConnectAttr(ODBC32.SQL_ATTR attribute, ODBC32.HANDLER handler)
		{
			int result = -1;
			int num = 0;
			byte[] array = new byte[4];
			OdbcConnectionHandle connectionHandle = this.ConnectionHandle;
			if (connectionHandle != null)
			{
				ODBC32.RetCode connectionAttribute = connectionHandle.GetConnectionAttribute(attribute, array, out num);
				if (connectionAttribute == ODBC32.RetCode.SUCCESS || ODBC32.RetCode.SUCCESS_WITH_INFO == connectionAttribute)
				{
					result = BitConverter.ToInt32(array, 0);
				}
				else
				{
					if (connectionAttribute == ODBC32.RetCode.ERROR)
					{
						string diagSqlState = this.GetDiagSqlState();
						if ("HYC00" == diagSqlState || "HY092" == diagSqlState || "IM001" == diagSqlState)
						{
							this.FlagUnsupportedConnectAttr(attribute);
						}
					}
					if (handler == ODBC32.HANDLER.THROW)
					{
						this.HandleError(connectionHandle, connectionAttribute);
					}
				}
			}
			return result;
		}

		private string GetDiagSqlState()
		{
			string result;
			this.ConnectionHandle.GetDiagnosticField(out result);
			return result;
		}

		internal ODBC32.RetCode GetInfoInt16Unhandled(ODBC32.SQL_INFO info, out short resultValue)
		{
			byte[] array = new byte[2];
			ODBC32.RetCode info2 = this.ConnectionHandle.GetInfo1(info, array);
			resultValue = BitConverter.ToInt16(array, 0);
			return info2;
		}

		internal ODBC32.RetCode GetInfoInt32Unhandled(ODBC32.SQL_INFO info, out int resultValue)
		{
			byte[] array = new byte[4];
			ODBC32.RetCode info2 = this.ConnectionHandle.GetInfo1(info, array);
			resultValue = BitConverter.ToInt32(array, 0);
			return info2;
		}

		private int GetInfoInt32Unhandled(ODBC32.SQL_INFO infotype)
		{
			byte[] array = new byte[4];
			this.ConnectionHandle.GetInfo1(infotype, array);
			return BitConverter.ToInt32(array, 0);
		}

		internal string GetInfoStringUnhandled(ODBC32.SQL_INFO info)
		{
			return this.GetInfoStringUnhandled(info, false);
		}

		private string GetInfoStringUnhandled(ODBC32.SQL_INFO info, bool handleError)
		{
			string result = null;
			short num = 0;
			byte[] array = new byte[100];
			OdbcConnectionHandle connectionHandle = this.ConnectionHandle;
			if (connectionHandle != null)
			{
				ODBC32.RetCode info2 = connectionHandle.GetInfo2(info, array, out num);
				if (array.Length < (int)(num - 2))
				{
					array = new byte[(int)(num + 2)];
					info2 = connectionHandle.GetInfo2(info, array, out num);
				}
				if (info2 == ODBC32.RetCode.SUCCESS || info2 == ODBC32.RetCode.SUCCESS_WITH_INFO)
				{
					result = (BitConverter.IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode).GetString(array, 0, Math.Min((int)num, array.Length));
				}
				else if (handleError)
				{
					this.HandleError(this.ConnectionHandle, info2);
				}
			}
			else if (handleError)
			{
				result = "";
			}
			return result;
		}

		internal Exception HandleErrorNoThrow(OdbcHandle hrHandle, ODBC32.RetCode retcode)
		{
			if (retcode != ODBC32.RetCode.SUCCESS)
			{
				if (retcode != ODBC32.RetCode.SUCCESS_WITH_INFO)
				{
					OdbcException ex = OdbcException.CreateException(ODBC32.GetDiagErrors(null, hrHandle, retcode), retcode);
					if (ex != null)
					{
						ex.Errors.SetSource(this.Driver);
					}
					this.ConnectionIsAlive(ex);
					return ex;
				}
				if (this._infoMessageEventHandler != null)
				{
					OdbcErrorCollection diagErrors = ODBC32.GetDiagErrors(null, hrHandle, retcode);
					diagErrors.SetSource(this.Driver);
					this.OnInfoMessage(new OdbcInfoMessageEventArgs(diagErrors));
				}
			}
			return null;
		}

		internal void HandleError(OdbcHandle hrHandle, ODBC32.RetCode retcode)
		{
			Exception ex = this.HandleErrorNoThrow(hrHandle, retcode);
			if (retcode > ODBC32.RetCode.SUCCESS_WITH_INFO)
			{
				throw ex;
			}
		}

		/// <summary>Opens a connection to a data source with the property settings specified by the <see cref="P:System.Data.Odbc.OdbcConnection.ConnectionString" />.</summary>
		/// <exception cref="T:System.NotSupportedException">The functionality of this method is unsupported in the base class and must be implemented in a derived class instead.</exception>
		public override void Open()
		{
			try
			{
				this.InnerConnection.OpenConnection(this, this.ConnectionFactory);
			}
			catch (DllNotFoundException ex) when (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				throw new DllNotFoundException("Dependency unixODBC with minimum version 2.3.1 is required." + Environment.NewLine + ex.Message);
			}
			if (ADP.NeedManualEnlistment())
			{
				this.EnlistTransaction(Transaction.Current);
			}
		}

		private void OnInfoMessage(OdbcInfoMessageEventArgs args)
		{
			if (this._infoMessageEventHandler != null)
			{
				try
				{
					this._infoMessageEventHandler(this, args);
				}
				catch (Exception e)
				{
					if (!ADP.IsCatchableOrSecurityExceptionType(e))
					{
						throw;
					}
					ADP.TraceExceptionWithoutRethrow(e);
				}
			}
		}

		/// <summary>Indicates that the ODBC Driver Manager environment handle can be released when the last underlying connection is released.</summary>
		public static void ReleaseObjectPool()
		{
			OdbcEnvironment.ReleaseObjectPool();
		}

		internal OdbcTransaction SetStateExecuting(string method, OdbcTransaction transaction)
		{
			if (this._weakTransaction != null)
			{
				OdbcTransaction odbcTransaction = this._weakTransaction.Target as OdbcTransaction;
				if (transaction != odbcTransaction)
				{
					if (transaction == null)
					{
						throw ADP.TransactionRequired(method);
					}
					if (this != transaction.Connection)
					{
						throw ADP.TransactionConnectionMismatch();
					}
					transaction = null;
				}
			}
			else if (transaction != null)
			{
				if (transaction.Connection != null)
				{
					throw ADP.TransactionConnectionMismatch();
				}
				transaction = null;
			}
			ConnectionState internalState = this.InternalState;
			if (ConnectionState.Open != internalState)
			{
				this.NotifyWeakReference(1);
				internalState = this.InternalState;
				if (ConnectionState.Open != internalState)
				{
					if ((ConnectionState.Fetching & internalState) != ConnectionState.Closed)
					{
						throw ADP.OpenReaderExists();
					}
					throw ADP.OpenConnectionRequired(method, internalState);
				}
			}
			return transaction;
		}

		internal void SetSupportedType(ODBC32.SQL_TYPE sqltype)
		{
			ODBC32.SQL_CVT sql_CVT;
			switch (sqltype)
			{
			case ODBC32.SQL_TYPE.WLONGVARCHAR:
				sql_CVT = ODBC32.SQL_CVT.WLONGVARCHAR;
				break;
			case ODBC32.SQL_TYPE.WVARCHAR:
				sql_CVT = ODBC32.SQL_CVT.WVARCHAR;
				break;
			case ODBC32.SQL_TYPE.WCHAR:
				sql_CVT = ODBC32.SQL_CVT.WCHAR;
				break;
			default:
				if (sqltype != ODBC32.SQL_TYPE.NUMERIC)
				{
					return;
				}
				sql_CVT = ODBC32.SQL_CVT.NUMERIC;
				break;
			}
			this.ProviderInfo.TestedSQLTypes |= (int)sql_CVT;
			this.ProviderInfo.SupportedSQLTypes |= (int)sql_CVT;
		}

		internal void FlagRestrictedSqlBindType(ODBC32.SQL_TYPE sqltype)
		{
			ODBC32.SQL_CVT sql_CVT;
			if (sqltype != ODBC32.SQL_TYPE.NUMERIC)
			{
				if (sqltype != ODBC32.SQL_TYPE.DECIMAL)
				{
					return;
				}
				sql_CVT = ODBC32.SQL_CVT.DECIMAL;
			}
			else
			{
				sql_CVT = ODBC32.SQL_CVT.NUMERIC;
			}
			this.ProviderInfo.RestrictedSQLBindTypes |= (int)sql_CVT;
		}

		internal void FlagUnsupportedConnectAttr(ODBC32.SQL_ATTR Attribute)
		{
			if (Attribute == ODBC32.SQL_ATTR.CURRENT_CATALOG)
			{
				this.ProviderInfo.NoCurrentCatalog = true;
				return;
			}
			if (Attribute != ODBC32.SQL_ATTR.CONNECTION_DEAD)
			{
				return;
			}
			this.ProviderInfo.NoConnectionDead = true;
		}

		internal void FlagUnsupportedStmtAttr(ODBC32.SQL_ATTR Attribute)
		{
			if (Attribute == ODBC32.SQL_ATTR.QUERY_TIMEOUT)
			{
				this.ProviderInfo.NoQueryTimeout = true;
				return;
			}
			if (Attribute == ODBC32.SQL_ATTR.SQL_COPT_SS_TXN_ISOLATION)
			{
				this.ProviderInfo.NoSqlSoptSSHiddenColumns = true;
				return;
			}
			if (Attribute != (ODBC32.SQL_ATTR)1228)
			{
				return;
			}
			this.ProviderInfo.NoSqlSoptSSNoBrowseTable = true;
		}

		internal void FlagUnsupportedColAttr(ODBC32.SQL_DESC v3FieldId, ODBC32.SQL_COLUMN v2FieldId)
		{
			if (this.IsV3Driver && v3FieldId == (ODBC32.SQL_DESC)1212)
			{
				this.ProviderInfo.NoSqlCASSColumnKey = true;
			}
		}

		internal bool SQLGetFunctions(ODBC32.SQL_API odbcFunction)
		{
			OdbcConnectionHandle connectionHandle = this.ConnectionHandle;
			if (connectionHandle != null)
			{
				short result;
				ODBC32.RetCode functions = connectionHandle.GetFunctions(odbcFunction, out result);
				if (functions != ODBC32.RetCode.SUCCESS)
				{
					this.HandleError(connectionHandle, functions);
				}
				return result != 0;
			}
			throw ODBC.ConnectionClosed();
		}

		internal bool TestTypeSupport(ODBC32.SQL_TYPE sqltype)
		{
			ODBC32.SQL_CONVERT infotype;
			ODBC32.SQL_CVT sql_CVT;
			switch (sqltype)
			{
			case ODBC32.SQL_TYPE.WLONGVARCHAR:
				infotype = ODBC32.SQL_CONVERT.LONGVARCHAR;
				sql_CVT = ODBC32.SQL_CVT.WLONGVARCHAR;
				break;
			case ODBC32.SQL_TYPE.WVARCHAR:
				infotype = ODBC32.SQL_CONVERT.VARCHAR;
				sql_CVT = ODBC32.SQL_CVT.WVARCHAR;
				break;
			case ODBC32.SQL_TYPE.WCHAR:
				infotype = ODBC32.SQL_CONVERT.CHAR;
				sql_CVT = ODBC32.SQL_CVT.WCHAR;
				break;
			default:
				if (sqltype != ODBC32.SQL_TYPE.NUMERIC)
				{
					return false;
				}
				infotype = ODBC32.SQL_CONVERT.NUMERIC;
				sql_CVT = ODBC32.SQL_CVT.NUMERIC;
				break;
			}
			if ((this.ProviderInfo.TestedSQLTypes & (int)sql_CVT) == 0)
			{
				int num = this.GetInfoInt32Unhandled((ODBC32.SQL_INFO)infotype);
				num &= (int)sql_CVT;
				this.ProviderInfo.TestedSQLTypes |= (int)sql_CVT;
				this.ProviderInfo.SupportedSQLTypes |= num;
			}
			return (this.ProviderInfo.SupportedSQLTypes & (int)sql_CVT) != 0;
		}

		internal bool TestRestrictedSqlBindType(ODBC32.SQL_TYPE sqltype)
		{
			ODBC32.SQL_CVT sql_CVT;
			if (sqltype != ODBC32.SQL_TYPE.NUMERIC)
			{
				if (sqltype != ODBC32.SQL_TYPE.DECIMAL)
				{
					return false;
				}
				sql_CVT = ODBC32.SQL_CVT.DECIMAL;
			}
			else
			{
				sql_CVT = ODBC32.SQL_CVT.NUMERIC;
			}
			return (this.ProviderInfo.RestrictedSQLBindTypes & (int)sql_CVT) != 0;
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			DbTransaction result = this.InnerConnection.BeginTransaction(isolationLevel);
			GC.KeepAlive(this);
			return result;
		}

		internal OdbcTransaction Open_BeginTransaction(IsolationLevel isolevel)
		{
			this.CheckState("BeginTransaction");
			this.RollbackDeadTransaction();
			if (this._weakTransaction != null && this._weakTransaction.IsAlive)
			{
				throw ADP.ParallelTransactionsNotSupported(this);
			}
			if (isolevel <= IsolationLevel.ReadUncommitted)
			{
				if (isolevel == IsolationLevel.Unspecified)
				{
					goto IL_82;
				}
				if (isolevel == IsolationLevel.Chaos)
				{
					throw ODBC.NotSupportedIsolationLevel(isolevel);
				}
				if (isolevel == IsolationLevel.ReadUncommitted)
				{
					goto IL_82;
				}
			}
			else if (isolevel <= IsolationLevel.RepeatableRead)
			{
				if (isolevel == IsolationLevel.ReadCommitted || isolevel == IsolationLevel.RepeatableRead)
				{
					goto IL_82;
				}
			}
			else if (isolevel == IsolationLevel.Serializable || isolevel == IsolationLevel.Snapshot)
			{
				goto IL_82;
			}
			throw ADP.InvalidIsolationLevel(isolevel);
			IL_82:
			OdbcConnectionHandle connectionHandle = this.ConnectionHandle;
			ODBC32.RetCode retCode = connectionHandle.BeginTransaction(ref isolevel);
			if (retCode == ODBC32.RetCode.ERROR)
			{
				this.HandleError(connectionHandle, retCode);
			}
			OdbcTransaction odbcTransaction = new OdbcTransaction(this, isolevel, connectionHandle);
			this._weakTransaction = new WeakReference(odbcTransaction);
			return odbcTransaction;
		}

		internal void Open_ChangeDatabase(string value)
		{
			this.CheckState("ChangeDatabase");
			if (value == null || value.Trim().Length == 0)
			{
				throw ADP.EmptyDatabaseName();
			}
			if (1024 < value.Length * 2 + 2)
			{
				throw ADP.DatabaseNameTooLong();
			}
			this.RollbackDeadTransaction();
			OdbcConnectionHandle connectionHandle = this.ConnectionHandle;
			ODBC32.RetCode retCode = connectionHandle.SetConnectionAttribute3(ODBC32.SQL_ATTR.CURRENT_CATALOG, value, checked(value.Length * 2));
			if (retCode != ODBC32.RetCode.SUCCESS)
			{
				this.HandleError(connectionHandle, retCode);
			}
		}

		internal string Open_GetServerVersion()
		{
			return this.GetInfoStringUnhandled(ODBC32.SQL_INFO.DBMS_VER, true);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Odbc.OdbcConnection" /> class.</summary>
		public OdbcConnection()
		{
			GC.SuppressFinalize(this);
			this._innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
		}

		private void CopyFrom(OdbcConnection connection)
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
				return OdbcConnection.s_connectionFactory;
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

		private void ConnectionString_Set(string value)
		{
			DbConnectionPoolKey key = new DbConnectionPoolKey(value);
			this.ConnectionString_Set(key);
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

		/// <summary>Returns schema information for the data source of this <see cref="T:System.Data.Odbc.OdbcConnection" />.</summary>
		/// <returns>A <see cref="T:System.Data.DataTable" /> that contains schema information.</returns>
		public override DataTable GetSchema()
		{
			return this.GetSchema(DbMetaDataCollectionNames.MetaDataCollections, null);
		}

		/// <summary>Returns schema information for the data source of this <see cref="T:System.Data.Odbc.OdbcConnection" /> using the specified name for the schema name.</summary>
		/// <param name="collectionName">Specifies the name of the schema to return.</param>
		/// <returns>A <see cref="T:System.Data.DataTable" /> that contains schema information.</returns>
		public override DataTable GetSchema(string collectionName)
		{
			return this.GetSchema(collectionName, null);
		}

		/// <summary>Returns schema information for the data source of this <see cref="T:System.Data.Odbc.OdbcConnection" /> using the specified string for the schema name and the specified string array for the restriction values.</summary>
		/// <param name="collectionName">Specifies the name of the schema to return.</param>
		/// <param name="restrictionValues">Specifies a set of restriction values for the requested schema.</param>
		/// <returns>A <see cref="T:System.Data.DataTable" /> that contains schema information.</returns>
		public override DataTable GetSchema(string collectionName, string[] restrictionValues)
		{
			return this.InnerConnection.GetSchema(this.ConnectionFactory, this.PoolGroup, this, collectionName, restrictionValues);
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

		/// <summary>Enlists in the specified transaction as a distributed transaction.</summary>
		/// <param name="transaction">A reference to an existing <see cref="T:System.EnterpriseServices.ITransaction" /> in which to enlist.</param>
		public void EnlistDistributedTransaction(ITransaction transaction)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private int _connectionTimeout = 15;

		private OdbcInfoMessageEventHandler _infoMessageEventHandler;

		private WeakReference _weakTransaction;

		private OdbcConnectionHandle _connectionHandle;

		private ConnectionState _extraState;

		private static readonly DbConnectionFactory s_connectionFactory = OdbcConnectionFactory.SingletonInstance;

		private DbConnectionOptions _userConnectionOptions;

		private DbConnectionPoolGroup _poolGroup;

		private DbConnectionInternal _innerConnection;

		private int _closeCount;
	}
}
