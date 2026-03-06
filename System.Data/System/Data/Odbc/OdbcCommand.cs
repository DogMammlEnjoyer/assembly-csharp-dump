using System;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Data.Odbc
{
	/// <summary>Represents an SQL statement or stored procedure to execute against a data source. This class cannot be inherited.</summary>
	public sealed class OdbcCommand : DbCommand, ICloneable
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Odbc.OdbcCommand" /> class.</summary>
		public OdbcCommand()
		{
			GC.SuppressFinalize(this);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Odbc.OdbcCommand" /> class with the text of the query.</summary>
		/// <param name="cmdText">The text of the query.</param>
		public OdbcCommand(string cmdText) : this()
		{
			this.CommandText = cmdText;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Odbc.OdbcCommand" /> class with the text of the query and an <see cref="T:System.Data.Odbc.OdbcConnection" /> object.</summary>
		/// <param name="cmdText">The text of the query.</param>
		/// <param name="connection">An <see cref="T:System.Data.Odbc.OdbcConnection" /> object that represents the connection to a data source.</param>
		public OdbcCommand(string cmdText, OdbcConnection connection) : this()
		{
			this.CommandText = cmdText;
			this.Connection = connection;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Odbc.OdbcCommand" /> class with the text of the query, an <see cref="T:System.Data.Odbc.OdbcConnection" /> object, and the <see cref="P:System.Data.Odbc.OdbcCommand.Transaction" />.</summary>
		/// <param name="cmdText">The text of the query.</param>
		/// <param name="connection">An <see cref="T:System.Data.Odbc.OdbcConnection" /> object that represents the connection to a data source.</param>
		/// <param name="transaction">The transaction in which the <see cref="T:System.Data.Odbc.OdbcCommand" /> executes.</param>
		public OdbcCommand(string cmdText, OdbcConnection connection, OdbcTransaction transaction) : this()
		{
			this.CommandText = cmdText;
			this.Connection = connection;
			this.Transaction = transaction;
		}

		private void DisposeDeadDataReader()
		{
			if (ConnectionState.Fetching == this._cmdState && this._weakDataReaderReference != null && !this._weakDataReaderReference.IsAlive)
			{
				if (this._cmdWrapper != null)
				{
					this._cmdWrapper.FreeKeyInfoStatementHandle(ODBC32.STMT.CLOSE);
					this._cmdWrapper.FreeStatementHandle(ODBC32.STMT.CLOSE);
				}
				this.CloseFromDataReader();
			}
		}

		private void DisposeDataReader()
		{
			if (this._weakDataReaderReference != null)
			{
				IDisposable disposable = (IDisposable)this._weakDataReaderReference.Target;
				if (disposable != null && this._weakDataReaderReference.IsAlive)
				{
					disposable.Dispose();
				}
				this.CloseFromDataReader();
			}
		}

		internal void DisconnectFromDataReaderAndConnection()
		{
			OdbcDataReader odbcDataReader = null;
			if (this._weakDataReaderReference != null)
			{
				OdbcDataReader odbcDataReader2 = (OdbcDataReader)this._weakDataReaderReference.Target;
				if (this._weakDataReaderReference.IsAlive)
				{
					odbcDataReader = odbcDataReader2;
				}
			}
			if (odbcDataReader != null)
			{
				odbcDataReader.Command = null;
			}
			this._transaction = null;
			if (this._connection != null)
			{
				this._connection.RemoveWeakReference(this);
				this._connection = null;
			}
			if (odbcDataReader == null)
			{
				this.CloseCommandWrapper();
			}
			this._cmdWrapper = null;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DisconnectFromDataReaderAndConnection();
				this._parameterCollection = null;
				this.CommandText = null;
			}
			this._cmdWrapper = null;
			this._isPrepared = false;
			base.Dispose(disposing);
		}

		internal bool Canceling
		{
			get
			{
				return this._cmdWrapper.Canceling;
			}
		}

		/// <summary>Gets or sets the SQL statement or stored procedure to execute against the data source.</summary>
		/// <returns>The SQL statement or stored procedure to execute. The default value is an empty string ("").</returns>
		public override string CommandText
		{
			get
			{
				string commandText = this._commandText;
				if (commandText == null)
				{
					return ADP.StrEmpty;
				}
				return commandText;
			}
			set
			{
				if (this._commandText != value)
				{
					this.PropertyChanging();
					this._commandText = value;
				}
			}
		}

		/// <summary>Gets or sets the wait time before terminating an attempt to execute a command and generating an error.</summary>
		/// <returns>The time in seconds to wait for the command to execute. The default is 30 seconds.</returns>
		public override int CommandTimeout
		{
			get
			{
				return this._commandTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw ADP.InvalidCommandTimeout(value, "CommandTimeout");
				}
				if (value != this._commandTimeout)
				{
					this.PropertyChanging();
					this._commandTimeout = value;
				}
			}
		}

		/// <summary>Resets the <see cref="P:System.Data.Odbc.OdbcCommand.CommandTimeout" /> property to the default value.</summary>
		public void ResetCommandTimeout()
		{
			if (30 != this._commandTimeout)
			{
				this.PropertyChanging();
				this._commandTimeout = 30;
			}
		}

		private bool ShouldSerializeCommandTimeout()
		{
			return 30 != this._commandTimeout;
		}

		/// <summary>Gets or sets a value that indicates how the <see cref="P:System.Data.Odbc.OdbcCommand.CommandText" /> property is interpreted.</summary>
		/// <returns>One of the <see cref="T:System.Data.CommandType" /> values. The default is <see langword="Text" />.</returns>
		/// <exception cref="T:System.ArgumentException">The value was not a valid <see cref="T:System.Data.CommandType" />.</exception>
		[DefaultValue(CommandType.Text)]
		public override CommandType CommandType
		{
			get
			{
				CommandType commandType = this._commandType;
				if (commandType == (CommandType)0)
				{
					return CommandType.Text;
				}
				return commandType;
			}
			set
			{
				if (value == CommandType.Text || value == CommandType.StoredProcedure)
				{
					this.PropertyChanging();
					this._commandType = value;
					return;
				}
				if (value != CommandType.TableDirect)
				{
					throw ADP.InvalidCommandType(value);
				}
				throw ODBC.NotSupportedCommandType(value);
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Data.Odbc.OdbcConnection" /> used by this instance of the <see cref="T:System.Data.Odbc.OdbcCommand" />.</summary>
		/// <returns>The connection to a data source. The default is a null value.</returns>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Data.Odbc.OdbcCommand.Connection" /> property was changed while a transaction was in progress.</exception>
		public new OdbcConnection Connection
		{
			get
			{
				return this._connection;
			}
			set
			{
				if (value != this._connection)
				{
					this.PropertyChanging();
					this.DisconnectFromDataReaderAndConnection();
					this._connection = value;
				}
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return this.Connection;
			}
			set
			{
				this.Connection = (OdbcConnection)value;
			}
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get
			{
				return this.Parameters;
			}
		}

		protected override DbTransaction DbTransaction
		{
			get
			{
				return this.Transaction;
			}
			set
			{
				this.Transaction = (OdbcTransaction)value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether the command object should be visible in a customized interface control.</summary>
		/// <returns>
		///   <see langword="true" /> if the command object should be visible in a control; otherwise, <see langword="false" />. The default is <see langword="true" />.</returns>
		[DefaultValue(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignOnly(true)]
		public override bool DesignTimeVisible
		{
			get
			{
				return !this._designTimeInvisible;
			}
			set
			{
				this._designTimeInvisible = !value;
				TypeDescriptor.Refresh(this);
			}
		}

		internal bool HasParameters
		{
			get
			{
				return this._parameterCollection != null;
			}
		}

		/// <summary>Gets the <see cref="T:System.Data.Odbc.OdbcParameterCollection" />.</summary>
		/// <returns>The parameters of the SQL statement or stored procedure. The default is an empty collection.</returns>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new OdbcParameterCollection Parameters
		{
			get
			{
				if (this._parameterCollection == null)
				{
					this._parameterCollection = new OdbcParameterCollection();
				}
				return this._parameterCollection;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Data.Odbc.OdbcTransaction" /> within which the <see cref="T:System.Data.Odbc.OdbcCommand" /> executes.</summary>
		/// <returns>An <see cref="T:System.Data.Odbc.OdbcTransaction" />. The default is a null value.</returns>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new OdbcTransaction Transaction
		{
			get
			{
				if (this._transaction != null && this._transaction.Connection == null)
				{
					this._transaction = null;
				}
				return this._transaction;
			}
			set
			{
				if (this._transaction != value)
				{
					this.PropertyChanging();
					this._transaction = value;
				}
			}
		}

		/// <summary>Gets or sets a value that specifies how the Update method should apply command results to the DataRow.</summary>
		/// <returns>One of the <see cref="T:System.Data.UpdateRowSource" /> values.</returns>
		[DefaultValue(UpdateRowSource.Both)]
		public override UpdateRowSource UpdatedRowSource
		{
			get
			{
				return this._updatedRowSource;
			}
			set
			{
				if (value <= UpdateRowSource.Both)
				{
					this._updatedRowSource = value;
					return;
				}
				throw ADP.InvalidUpdateRowSource(value);
			}
		}

		internal OdbcDescriptorHandle GetDescriptorHandle(ODBC32.SQL_ATTR attribute)
		{
			return this._cmdWrapper.GetDescriptorHandle(attribute);
		}

		internal CMDWrapper GetStatementHandle()
		{
			if (this._cmdWrapper == null)
			{
				this._cmdWrapper = new CMDWrapper(this._connection);
				this._connection.AddWeakReference(this, 1);
			}
			if (this._cmdWrapper._dataReaderBuf == null)
			{
				this._cmdWrapper._dataReaderBuf = new CNativeBuffer(4096);
			}
			if (this._cmdWrapper.StatementHandle == null)
			{
				this._isPrepared = false;
				this._cmdWrapper.CreateStatementHandle();
			}
			else if (this._parameterCollection != null && this._parameterCollection.RebindCollection)
			{
				this._cmdWrapper.FreeStatementHandle(ODBC32.STMT.RESET_PARAMS);
			}
			return this._cmdWrapper;
		}

		/// <summary>Tries to cancel the execution of an <see cref="T:System.Data.Odbc.OdbcCommand" />.</summary>
		public override void Cancel()
		{
			CMDWrapper cmdWrapper = this._cmdWrapper;
			if (cmdWrapper != null)
			{
				cmdWrapper.Canceling = true;
				OdbcStatementHandle statementHandle = cmdWrapper.StatementHandle;
				if (statementHandle != null)
				{
					OdbcStatementHandle obj = statementHandle;
					lock (obj)
					{
						ODBC32.RetCode retCode = statementHandle.Cancel();
						if (retCode > ODBC32.RetCode.SUCCESS_WITH_INFO)
						{
							throw cmdWrapper.Connection.HandleErrorNoThrow(statementHandle, retCode);
						}
					}
				}
			}
		}

		/// <summary>For a description of this member, see <see cref="M:System.ICloneable.Clone" />.</summary>
		/// <returns>A new <see cref="T:System.Object" /> that is a copy of this instance.</returns>
		object ICloneable.Clone()
		{
			OdbcCommand odbcCommand = new OdbcCommand();
			odbcCommand.CommandText = this.CommandText;
			odbcCommand.CommandTimeout = this.CommandTimeout;
			odbcCommand.CommandType = this.CommandType;
			odbcCommand.Connection = this.Connection;
			odbcCommand.Transaction = this.Transaction;
			odbcCommand.UpdatedRowSource = this.UpdatedRowSource;
			if (this._parameterCollection != null && 0 < this.Parameters.Count)
			{
				OdbcParameterCollection parameters = odbcCommand.Parameters;
				foreach (object obj in this.Parameters)
				{
					ICloneable cloneable = (ICloneable)obj;
					parameters.Add(cloneable.Clone());
				}
			}
			return odbcCommand;
		}

		internal bool RecoverFromConnection()
		{
			this.DisposeDeadDataReader();
			return this._cmdState == ConnectionState.Closed;
		}

		private void CloseCommandWrapper()
		{
			CMDWrapper cmdWrapper = this._cmdWrapper;
			if (cmdWrapper != null)
			{
				try
				{
					cmdWrapper.Dispose();
					if (this._connection != null)
					{
						this._connection.RemoveWeakReference(this);
					}
				}
				finally
				{
					this._cmdWrapper = null;
				}
			}
		}

		internal void CloseFromConnection()
		{
			if (this._parameterCollection != null)
			{
				this._parameterCollection.RebindCollection = true;
			}
			this.DisposeDataReader();
			this.CloseCommandWrapper();
			this._isPrepared = false;
			this._transaction = null;
		}

		internal void CloseFromDataReader()
		{
			this._weakDataReaderReference = null;
			this._cmdState = ConnectionState.Closed;
		}

		/// <summary>Creates a new instance of an <see cref="T:System.Data.Odbc.OdbcParameter" /> object.</summary>
		/// <returns>An <see cref="T:System.Data.Odbc.OdbcParameter" /> object.</returns>
		public new OdbcParameter CreateParameter()
		{
			return new OdbcParameter();
		}

		protected override DbParameter CreateDbParameter()
		{
			return this.CreateParameter();
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return this.ExecuteReader(behavior);
		}

		/// <summary>Executes an SQL statement against the <see cref="P:System.Data.Odbc.OdbcCommand.Connection" /> and returns the number of rows affected.</summary>
		/// <returns>For UPDATE, INSERT, and DELETE statements, the return value is the number of rows affected by the command. For all other types of statements, the return value is -1.</returns>
		/// <exception cref="T:System.InvalidOperationException">The connection does not exist.  
		///  -or-  
		///  The connection is not open.</exception>
		public override int ExecuteNonQuery()
		{
			int recordsAffected;
			using (OdbcDataReader odbcDataReader = this.ExecuteReaderObject(CommandBehavior.Default, "ExecuteNonQuery", false))
			{
				odbcDataReader.Close();
				recordsAffected = odbcDataReader.RecordsAffected;
			}
			return recordsAffected;
		}

		/// <summary>Sends the <see cref="P:System.Data.Odbc.OdbcCommand.CommandText" /> to the <see cref="P:System.Data.Odbc.OdbcCommand.Connection" /> and builds an <see cref="T:System.Data.Odbc.OdbcDataReader" />.</summary>
		/// <returns>An <see cref="T:System.Data.Odbc.OdbcDataReader" /> object.</returns>
		public new OdbcDataReader ExecuteReader()
		{
			return this.ExecuteReader(CommandBehavior.Default);
		}

		/// <summary>Sends the <see cref="P:System.Data.Odbc.OdbcCommand.CommandText" /> to the <see cref="P:System.Data.Odbc.OdbcCommand.Connection" />, and builds an <see cref="T:System.Data.Odbc.OdbcDataReader" /> using one of the <see langword="CommandBehavior" /> values.</summary>
		/// <param name="behavior">One of the <see langword="System.Data.CommandBehavior" /> values.</param>
		/// <returns>An <see cref="T:System.Data.Odbc.OdbcDataReader" /> object.</returns>
		public new OdbcDataReader ExecuteReader(CommandBehavior behavior)
		{
			return this.ExecuteReaderObject(behavior, "ExecuteReader", true);
		}

		internal OdbcDataReader ExecuteReaderFromSQLMethod(object[] methodArguments, ODBC32.SQL_API method)
		{
			return this.ExecuteReaderObject(CommandBehavior.Default, method.ToString(), true, methodArguments, method);
		}

		private OdbcDataReader ExecuteReaderObject(CommandBehavior behavior, string method, bool needReader)
		{
			if (this.CommandText == null || this.CommandText.Length == 0)
			{
				throw ADP.CommandTextRequired(method);
			}
			return this.ExecuteReaderObject(behavior, method, needReader, null, ODBC32.SQL_API.SQLEXECDIRECT);
		}

		private OdbcDataReader ExecuteReaderObject(CommandBehavior behavior, string method, bool needReader, object[] methodArguments, ODBC32.SQL_API odbcApiMethod)
		{
			OdbcDataReader odbcDataReader = null;
			try
			{
				this.DisposeDeadDataReader();
				this.ValidateConnectionAndTransaction(method);
				if ((CommandBehavior.SingleRow & behavior) != CommandBehavior.Default)
				{
					behavior |= CommandBehavior.SingleResult;
				}
				OdbcStatementHandle statementHandle = this.GetStatementHandle().StatementHandle;
				this._cmdWrapper.Canceling = false;
				if (this._weakDataReaderReference != null && this._weakDataReaderReference.IsAlive)
				{
					object target = this._weakDataReaderReference.Target;
					if (target != null && this._weakDataReaderReference.IsAlive && !((OdbcDataReader)target).IsClosed)
					{
						throw ADP.OpenReaderExists();
					}
				}
				odbcDataReader = new OdbcDataReader(this, this._cmdWrapper, behavior);
				if (!this.Connection.ProviderInfo.NoQueryTimeout)
				{
					this.TrySetStatementAttribute(statementHandle, ODBC32.SQL_ATTR.QUERY_TIMEOUT, (IntPtr)this.CommandTimeout);
				}
				if (needReader && this.Connection.IsV3Driver && !this.Connection.ProviderInfo.NoSqlSoptSSNoBrowseTable && !this.Connection.ProviderInfo.NoSqlSoptSSHiddenColumns)
				{
					if (odbcDataReader.IsBehavior(CommandBehavior.KeyInfo))
					{
						if (!this._cmdWrapper._ssKeyInfoModeOn)
						{
							this.TrySetStatementAttribute(statementHandle, (ODBC32.SQL_ATTR)1228, (IntPtr)1);
							this.TrySetStatementAttribute(statementHandle, ODBC32.SQL_ATTR.SQL_COPT_SS_TXN_ISOLATION, (IntPtr)1);
							this._cmdWrapper._ssKeyInfoModeOff = false;
							this._cmdWrapper._ssKeyInfoModeOn = true;
						}
					}
					else if (!this._cmdWrapper._ssKeyInfoModeOff)
					{
						this.TrySetStatementAttribute(statementHandle, (ODBC32.SQL_ATTR)1228, (IntPtr)0);
						this.TrySetStatementAttribute(statementHandle, ODBC32.SQL_ATTR.SQL_COPT_SS_TXN_ISOLATION, (IntPtr)0);
						this._cmdWrapper._ssKeyInfoModeOff = true;
						this._cmdWrapper._ssKeyInfoModeOn = false;
					}
				}
				if (odbcDataReader.IsBehavior(CommandBehavior.KeyInfo) || odbcDataReader.IsBehavior(CommandBehavior.SchemaOnly))
				{
					ODBC32.RetCode retCode = statementHandle.Prepare(this.CommandText);
					if (retCode != ODBC32.RetCode.SUCCESS)
					{
						this._connection.HandleError(statementHandle, retCode);
					}
				}
				bool flag = false;
				CNativeBuffer cnativeBuffer = this._cmdWrapper._nativeParameterBuffer;
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					if (this._parameterCollection != null && 0 < this._parameterCollection.Count)
					{
						int num = this._parameterCollection.CalcParameterBufferSize(this);
						if (cnativeBuffer == null || cnativeBuffer.Length < num)
						{
							if (cnativeBuffer != null)
							{
								cnativeBuffer.Dispose();
							}
							cnativeBuffer = new CNativeBuffer(num);
							this._cmdWrapper._nativeParameterBuffer = cnativeBuffer;
						}
						else
						{
							cnativeBuffer.ZeroMemory();
						}
						cnativeBuffer.DangerousAddRef(ref flag);
						this._parameterCollection.Bind(this, this._cmdWrapper, cnativeBuffer);
					}
					if (!odbcDataReader.IsBehavior(CommandBehavior.SchemaOnly))
					{
						ODBC32.RetCode retCode;
						if ((odbcDataReader.IsBehavior(CommandBehavior.KeyInfo) || odbcDataReader.IsBehavior(CommandBehavior.SchemaOnly)) && this.CommandType != CommandType.StoredProcedure)
						{
							short num2;
							retCode = statementHandle.NumberOfResultColumns(out num2);
							if (retCode == ODBC32.RetCode.SUCCESS || retCode == ODBC32.RetCode.SUCCESS_WITH_INFO)
							{
								if (num2 > 0)
								{
									odbcDataReader.GetSchemaTable();
								}
							}
							else if (retCode != ODBC32.RetCode.NO_DATA)
							{
								this._connection.HandleError(statementHandle, retCode);
							}
						}
						if (odbcApiMethod <= ODBC32.SQL_API.SQLGETTYPEINFO)
						{
							if (odbcApiMethod != ODBC32.SQL_API.SQLEXECDIRECT)
							{
								if (odbcApiMethod == ODBC32.SQL_API.SQLCOLUMNS)
								{
									retCode = statementHandle.Columns((string)methodArguments[0], (string)methodArguments[1], (string)methodArguments[2], (string)methodArguments[3]);
									goto IL_42A;
								}
								if (odbcApiMethod == ODBC32.SQL_API.SQLGETTYPEINFO)
								{
									retCode = statementHandle.GetTypeInfo((short)methodArguments[0]);
									goto IL_42A;
								}
							}
							else
							{
								if (odbcDataReader.IsBehavior(CommandBehavior.KeyInfo) || this._isPrepared)
								{
									retCode = statementHandle.Execute();
									goto IL_42A;
								}
								retCode = statementHandle.ExecuteDirect(this.CommandText);
								goto IL_42A;
							}
						}
						else if (odbcApiMethod <= ODBC32.SQL_API.SQLTABLES)
						{
							if (odbcApiMethod == ODBC32.SQL_API.SQLSTATISTICS)
							{
								retCode = statementHandle.Statistics((string)methodArguments[0], (string)methodArguments[1], (string)methodArguments[2], (short)methodArguments[3], (short)methodArguments[4]);
								goto IL_42A;
							}
							if (odbcApiMethod == ODBC32.SQL_API.SQLTABLES)
							{
								retCode = statementHandle.Tables((string)methodArguments[0], (string)methodArguments[1], (string)methodArguments[2], (string)methodArguments[3]);
								goto IL_42A;
							}
						}
						else
						{
							if (odbcApiMethod == ODBC32.SQL_API.SQLPROCEDURECOLUMNS)
							{
								retCode = statementHandle.ProcedureColumns((string)methodArguments[0], (string)methodArguments[1], (string)methodArguments[2], (string)methodArguments[3]);
								goto IL_42A;
							}
							if (odbcApiMethod == ODBC32.SQL_API.SQLPROCEDURES)
							{
								retCode = statementHandle.Procedures((string)methodArguments[0], (string)methodArguments[1], (string)methodArguments[2]);
								goto IL_42A;
							}
						}
						throw ADP.InvalidOperation(method.ToString());
						IL_42A:
						if (retCode != ODBC32.RetCode.SUCCESS && ODBC32.RetCode.NO_DATA != retCode)
						{
							this._connection.HandleError(statementHandle, retCode);
						}
					}
				}
				finally
				{
					if (flag)
					{
						cnativeBuffer.DangerousRelease();
					}
				}
				this._weakDataReaderReference = new WeakReference(odbcDataReader);
				if (!odbcDataReader.IsBehavior(CommandBehavior.SchemaOnly))
				{
					odbcDataReader.FirstResult();
				}
				this._cmdState = ConnectionState.Fetching;
			}
			finally
			{
				if (ConnectionState.Fetching != this._cmdState)
				{
					if (odbcDataReader != null)
					{
						if (this._parameterCollection != null)
						{
							this._parameterCollection.ClearBindings();
						}
						((IDisposable)odbcDataReader).Dispose();
					}
					if (this._cmdState != ConnectionState.Closed)
					{
						this._cmdState = ConnectionState.Closed;
					}
				}
			}
			return odbcDataReader;
		}

		/// <summary>Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.</summary>
		/// <returns>The first column of the first row in the result set, or a null reference if the result set is empty.</returns>
		public override object ExecuteScalar()
		{
			object result = null;
			using (IDataReader dataReader = this.ExecuteReaderObject(CommandBehavior.Default, "ExecuteScalar", false))
			{
				if (dataReader.Read() && 0 < dataReader.FieldCount)
				{
					result = dataReader.GetValue(0);
				}
				dataReader.Close();
			}
			return result;
		}

		internal string GetDiagSqlState()
		{
			return this._cmdWrapper.GetDiagSqlState();
		}

		private void PropertyChanging()
		{
			this._isPrepared = false;
		}

		/// <summary>Creates a prepared or compiled version of the command at the data source.</summary>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.Data.Odbc.OdbcCommand.Connection" /> is not set.  
		///  -or-  
		///  The <see cref="P:System.Data.Odbc.OdbcCommand.Connection" /> is not <see cref="M:System.Data.Odbc.OdbcConnection.Open" />.</exception>
		public override void Prepare()
		{
			this.ValidateOpenConnection("Prepare");
			if ((ConnectionState.Fetching & this._connection.InternalState) != ConnectionState.Closed)
			{
				throw ADP.OpenReaderExists();
			}
			if (this.CommandType == CommandType.TableDirect)
			{
				return;
			}
			this.DisposeDeadDataReader();
			this.GetStatementHandle();
			OdbcStatementHandle statementHandle = this._cmdWrapper.StatementHandle;
			ODBC32.RetCode retCode = statementHandle.Prepare(this.CommandText);
			if (retCode != ODBC32.RetCode.SUCCESS)
			{
				this._connection.HandleError(statementHandle, retCode);
			}
			this._isPrepared = true;
		}

		private void TrySetStatementAttribute(OdbcStatementHandle stmt, ODBC32.SQL_ATTR stmtAttribute, IntPtr value)
		{
			if (stmt.SetStatementAttribute(stmtAttribute, value, ODBC32.SQL_IS.UINTEGER) == ODBC32.RetCode.ERROR)
			{
				string a;
				stmt.GetDiagnosticField(out a);
				if (a == "HYC00" || a == "HY092")
				{
					this.Connection.FlagUnsupportedStmtAttr(stmtAttribute);
				}
			}
		}

		private void ValidateOpenConnection(string methodName)
		{
			OdbcConnection connection = this.Connection;
			if (connection == null)
			{
				throw ADP.ConnectionRequired(methodName);
			}
			ConnectionState state = connection.State;
			if (ConnectionState.Open != state)
			{
				throw ADP.OpenConnectionRequired(methodName, state);
			}
		}

		private void ValidateConnectionAndTransaction(string method)
		{
			if (this._connection == null)
			{
				throw ADP.ConnectionRequired(method);
			}
			this._transaction = this._connection.SetStateExecuting(method, this.Transaction);
			this._cmdState = ConnectionState.Executing;
		}

		private static int s_objectTypeCount;

		internal readonly int ObjectID = Interlocked.Increment(ref OdbcCommand.s_objectTypeCount);

		private string _commandText;

		private CommandType _commandType;

		private int _commandTimeout = 30;

		private UpdateRowSource _updatedRowSource = UpdateRowSource.Both;

		private bool _designTimeInvisible;

		private bool _isPrepared;

		private OdbcConnection _connection;

		private OdbcTransaction _transaction;

		private WeakReference _weakDataReaderReference;

		private CMDWrapper _cmdWrapper;

		private OdbcParameterCollection _parameterCollection;

		private ConnectionState _cmdState;
	}
}
