using System;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Transactions;

namespace System.Data.Odbc
{
	internal sealed class OdbcConnectionHandle : OdbcHandle
	{
		internal OdbcConnectionHandle(OdbcConnection connection, OdbcConnectionString constr, OdbcEnvironmentHandle environmentHandle) : base(ODBC32.SQL_HANDLE.DBC, environmentHandle)
		{
			if (connection == null)
			{
				throw ADP.ArgumentNull("connection");
			}
			if (constr == null)
			{
				throw ADP.ArgumentNull("constr");
			}
			int connectionTimeout = connection.ConnectionTimeout;
			ODBC32.RetCode retcode = this.SetConnectionAttribute2(ODBC32.SQL_ATTR.LOGIN_TIMEOUT, (IntPtr)connectionTimeout, -5);
			string connectionString = constr.UsersConnectionString(false);
			retcode = this.Connect(connectionString);
			connection.HandleError(this, retcode);
		}

		private ODBC32.RetCode AutoCommitOff()
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			ODBC32.RetCode retCode;
			try
			{
			}
			finally
			{
				retCode = Interop.Odbc.SQLSetConnectAttrW(this, ODBC32.SQL_ATTR.AUTOCOMMIT, ODBC32.SQL_AUTOCOMMIT_OFF, -5);
				if (retCode <= ODBC32.RetCode.SUCCESS_WITH_INFO)
				{
					this._handleState = OdbcConnectionHandle.HandleState.Transacted;
				}
			}
			ODBC.TraceODBC(3, "SQLSetConnectAttrW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode BeginTransaction(ref IsolationLevel isolevel)
		{
			ODBC32.RetCode retCode = ODBC32.RetCode.SUCCESS;
			if (IsolationLevel.Unspecified != isolevel)
			{
				IsolationLevel isolationLevel = isolevel;
				ODBC32.SQL_TRANSACTION value;
				ODBC32.SQL_ATTR attribute;
				if (isolationLevel <= IsolationLevel.ReadCommitted)
				{
					if (isolationLevel == IsolationLevel.Chaos)
					{
						throw ODBC.NotSupportedIsolationLevel(isolevel);
					}
					if (isolationLevel == IsolationLevel.ReadUncommitted)
					{
						value = ODBC32.SQL_TRANSACTION.READ_UNCOMMITTED;
						attribute = ODBC32.SQL_ATTR.TXN_ISOLATION;
						goto IL_7D;
					}
					if (isolationLevel == IsolationLevel.ReadCommitted)
					{
						value = ODBC32.SQL_TRANSACTION.READ_COMMITTED;
						attribute = ODBC32.SQL_ATTR.TXN_ISOLATION;
						goto IL_7D;
					}
				}
				else
				{
					if (isolationLevel == IsolationLevel.RepeatableRead)
					{
						value = ODBC32.SQL_TRANSACTION.REPEATABLE_READ;
						attribute = ODBC32.SQL_ATTR.TXN_ISOLATION;
						goto IL_7D;
					}
					if (isolationLevel == IsolationLevel.Serializable)
					{
						value = ODBC32.SQL_TRANSACTION.SERIALIZABLE;
						attribute = ODBC32.SQL_ATTR.TXN_ISOLATION;
						goto IL_7D;
					}
					if (isolationLevel == IsolationLevel.Snapshot)
					{
						value = ODBC32.SQL_TRANSACTION.SNAPSHOT;
						attribute = ODBC32.SQL_ATTR.SQL_COPT_SS_TXN_ISOLATION;
						goto IL_7D;
					}
				}
				throw ADP.InvalidIsolationLevel(isolevel);
				IL_7D:
				retCode = this.SetConnectionAttribute2(attribute, (IntPtr)((int)value), -6);
				if (ODBC32.RetCode.SUCCESS_WITH_INFO == retCode)
				{
					isolevel = IsolationLevel.Unspecified;
				}
			}
			if (retCode <= ODBC32.RetCode.SUCCESS_WITH_INFO)
			{
				retCode = this.AutoCommitOff();
				this._handleState = OdbcConnectionHandle.HandleState.TransactionInProgress;
			}
			return retCode;
		}

		internal ODBC32.RetCode CompleteTransaction(short transactionOperation)
		{
			bool flag = false;
			RuntimeHelpers.PrepareConstrainedRegions();
			ODBC32.RetCode result;
			try
			{
				base.DangerousAddRef(ref flag);
				result = this.CompleteTransaction(transactionOperation, this.handle);
			}
			finally
			{
				if (flag)
				{
					base.DangerousRelease();
				}
			}
			return result;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		private ODBC32.RetCode CompleteTransaction(short transactionOperation, IntPtr handle)
		{
			ODBC32.RetCode retCode = ODBC32.RetCode.SUCCESS;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				if (OdbcConnectionHandle.HandleState.TransactionInProgress == this._handleState)
				{
					retCode = Interop.Odbc.SQLEndTran(base.HandleType, handle, transactionOperation);
					if (retCode == ODBC32.RetCode.SUCCESS || ODBC32.RetCode.SUCCESS_WITH_INFO == retCode)
					{
						this._handleState = OdbcConnectionHandle.HandleState.Transacted;
					}
				}
				if (OdbcConnectionHandle.HandleState.Transacted == this._handleState)
				{
					retCode = Interop.Odbc.SQLSetConnectAttrW(handle, ODBC32.SQL_ATTR.AUTOCOMMIT, ODBC32.SQL_AUTOCOMMIT_ON, -5);
					this._handleState = OdbcConnectionHandle.HandleState.Connected;
				}
			}
			return retCode;
		}

		private ODBC32.RetCode Connect(string connectionString)
		{
			RuntimeHelpers.PrepareConstrainedRegions();
			ODBC32.RetCode retCode;
			try
			{
			}
			finally
			{
				short num;
				retCode = Interop.Odbc.SQLDriverConnectW(this, ADP.PtrZero, connectionString, -3, ADP.PtrZero, 0, out num, 0);
				if (retCode <= ODBC32.RetCode.SUCCESS_WITH_INFO)
				{
					this._handleState = OdbcConnectionHandle.HandleState.Connected;
				}
			}
			ODBC.TraceODBC(3, "SQLDriverConnectW", retCode);
			return retCode;
		}

		protected override bool ReleaseHandle()
		{
			this.CompleteTransaction(1, this.handle);
			if (OdbcConnectionHandle.HandleState.Connected == this._handleState || OdbcConnectionHandle.HandleState.TransactionInProgress == this._handleState)
			{
				Interop.Odbc.SQLDisconnect(this.handle);
				this._handleState = OdbcConnectionHandle.HandleState.Allocated;
			}
			return base.ReleaseHandle();
		}

		internal ODBC32.RetCode GetConnectionAttribute(ODBC32.SQL_ATTR attribute, byte[] buffer, out int cbActual)
		{
			return Interop.Odbc.SQLGetConnectAttrW(this, attribute, buffer, buffer.Length, out cbActual);
		}

		internal ODBC32.RetCode GetFunctions(ODBC32.SQL_API fFunction, out short fExists)
		{
			ODBC32.RetCode retCode = Interop.Odbc.SQLGetFunctions(this, fFunction, out fExists);
			ODBC.TraceODBC(3, "SQLGetFunctions", retCode);
			return retCode;
		}

		internal ODBC32.RetCode GetInfo2(ODBC32.SQL_INFO info, byte[] buffer, out short cbActual)
		{
			return Interop.Odbc.SQLGetInfoW(this, info, buffer, checked((short)buffer.Length), out cbActual);
		}

		internal ODBC32.RetCode GetInfo1(ODBC32.SQL_INFO info, byte[] buffer)
		{
			return Interop.Odbc.SQLGetInfoW(this, info, buffer, checked((short)buffer.Length), ADP.PtrZero);
		}

		internal ODBC32.RetCode SetConnectionAttribute2(ODBC32.SQL_ATTR attribute, IntPtr value, int length)
		{
			ODBC32.RetCode retCode = Interop.Odbc.SQLSetConnectAttrW(this, attribute, value, length);
			ODBC.TraceODBC(3, "SQLSetConnectAttrW", retCode);
			return retCode;
		}

		internal ODBC32.RetCode SetConnectionAttribute3(ODBC32.SQL_ATTR attribute, string buffer, int length)
		{
			return Interop.Odbc.SQLSetConnectAttrW(this, attribute, buffer, length);
		}

		internal ODBC32.RetCode SetConnectionAttribute4(ODBC32.SQL_ATTR attribute, IDtcTransaction transaction, int length)
		{
			ODBC32.RetCode retCode = Interop.Odbc.SQLSetConnectAttrW(this, attribute, transaction, length);
			ODBC.TraceODBC(3, "SQLSetConnectAttrW", retCode);
			return retCode;
		}

		private OdbcConnectionHandle.HandleState _handleState;

		private enum HandleState
		{
			Allocated,
			Connected,
			Transacted,
			TransactionInProgress
		}
	}
}
