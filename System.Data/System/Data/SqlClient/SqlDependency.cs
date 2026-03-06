using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.ProviderBase;
using System.Data.Sql;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml;

namespace System.Data.SqlClient
{
	/// <summary>The <see cref="T:System.Data.SqlClient.SqlDependency" /> object represents a query notification dependency between an application and an instance of SQL Server. An application can create a <see cref="T:System.Data.SqlClient.SqlDependency" /> object and register to receive notifications via the <see cref="T:System.Data.SqlClient.OnChangeEventHandler" /> event handler.</summary>
	public sealed class SqlDependency
	{
		/// <summary>Creates a new instance of the <see cref="T:System.Data.SqlClient.SqlDependency" /> class with the default settings.</summary>
		public SqlDependency() : this(null, null, 0)
		{
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Data.SqlClient.SqlDependency" /> class and associates it with the <see cref="T:System.Data.SqlClient.SqlCommand" /> parameter.</summary>
		/// <param name="command">The <see cref="T:System.Data.SqlClient.SqlCommand" /> object to associate with this <see cref="T:System.Data.SqlClient.SqlDependency" /> object. The constructor will set up a <see cref="T:System.Data.Sql.SqlNotificationRequest" /> object and bind it to the command.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="command" /> parameter is NULL.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlCommand" /> object already has a <see cref="T:System.Data.Sql.SqlNotificationRequest" /> object assigned to its <see cref="P:System.Data.SqlClient.SqlCommand.Notification" /> property, and that <see cref="T:System.Data.Sql.SqlNotificationRequest" /> is not associated with this dependency.</exception>
		public SqlDependency(SqlCommand command) : this(command, null, 0)
		{
		}

		/// <summary>Creates a new instance of the <see cref="T:System.Data.SqlClient.SqlDependency" /> class, associates it with the <see cref="T:System.Data.SqlClient.SqlCommand" /> parameter, and specifies notification options and a time-out value.</summary>
		/// <param name="command">The <see cref="T:System.Data.SqlClient.SqlCommand" /> object to associate with this <see cref="T:System.Data.SqlClient.SqlDependency" /> object. The constructor sets up a <see cref="T:System.Data.Sql.SqlNotificationRequest" /> object and bind it to the command.</param>
		/// <param name="options">The notification request options to be used by this dependency. <see langword="null" /> to use the default service.</param>
		/// <param name="timeout">The time-out for this notification in seconds. The default is 0, indicating that the server's time-out should be used.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="command" /> parameter is NULL.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The time-out value is less than zero.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlCommand" /> object already has a <see cref="T:System.Data.Sql.SqlNotificationRequest" /> object assigned to its <see cref="P:System.Data.SqlClient.SqlCommand.Notification" /> property and that <see cref="T:System.Data.Sql.SqlNotificationRequest" /> is not associated with this dependency.  
		///  An attempt was made to create a SqlDependency instance from within SQLCLR.</exception>
		public SqlDependency(SqlCommand command, string options, int timeout)
		{
			if (timeout < 0)
			{
				throw SQL.InvalidSqlDependencyTimeout("timeout");
			}
			this._timeout = timeout;
			if (options != null)
			{
				this._options = options;
			}
			this.AddCommandInternal(command);
			SqlDependencyPerAppDomainDispatcher.SingletonInstance.AddDependencyEntry(this);
		}

		/// <summary>Gets a value that indicates whether one of the result sets associated with the dependency has changed.</summary>
		/// <returns>A Boolean value indicating whether one of the result sets has changed.</returns>
		public bool HasChanges
		{
			get
			{
				return this._dependencyFired;
			}
		}

		/// <summary>Gets a value that uniquely identifies this instance of the <see cref="T:System.Data.SqlClient.SqlDependency" /> class.</summary>
		/// <returns>A string representation of a GUID that is generated for each instance of the <see cref="T:System.Data.SqlClient.SqlDependency" /> class.</returns>
		public string Id
		{
			get
			{
				return this._id;
			}
		}

		internal static string AppDomainKey
		{
			get
			{
				return SqlDependency.s_appDomainKey;
			}
		}

		internal DateTime ExpirationTime
		{
			get
			{
				return this._expirationTime;
			}
		}

		internal string Options
		{
			get
			{
				return this._options;
			}
		}

		internal static SqlDependencyProcessDispatcher ProcessDispatcher
		{
			get
			{
				return SqlDependency.s_processDispatcher;
			}
		}

		internal int Timeout
		{
			get
			{
				return this._timeout;
			}
		}

		/// <summary>Occurs when a notification is received for any of the commands associated with this <see cref="T:System.Data.SqlClient.SqlDependency" /> object.</summary>
		public event OnChangeEventHandler OnChange
		{
			add
			{
				if (value != null)
				{
					SqlNotificationEventArgs sqlNotificationEventArgs = null;
					object eventHandlerLock = this._eventHandlerLock;
					lock (eventHandlerLock)
					{
						if (this._dependencyFired)
						{
							sqlNotificationEventArgs = new SqlNotificationEventArgs(SqlNotificationType.Subscribe, SqlNotificationInfo.AlreadyChanged, SqlNotificationSource.Client);
						}
						else
						{
							SqlDependency.EventContextPair item = new SqlDependency.EventContextPair(value, this);
							if (this._eventList.Contains(item))
							{
								throw SQL.SqlDependencyEventNoDuplicate();
							}
							this._eventList.Add(item);
						}
					}
					if (sqlNotificationEventArgs != null)
					{
						value(this, sqlNotificationEventArgs);
					}
				}
			}
			remove
			{
				if (value != null)
				{
					SqlDependency.EventContextPair item = new SqlDependency.EventContextPair(value, this);
					object eventHandlerLock = this._eventHandlerLock;
					lock (eventHandlerLock)
					{
						int num = this._eventList.IndexOf(item);
						if (0 <= num)
						{
							this._eventList.RemoveAt(num);
						}
					}
				}
			}
		}

		/// <summary>Associates a <see cref="T:System.Data.SqlClient.SqlCommand" /> object with this <see cref="T:System.Data.SqlClient.SqlDependency" /> instance.</summary>
		/// <param name="command">A <see cref="T:System.Data.SqlClient.SqlCommand" /> object containing a statement that is valid for notifications.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="command" /> parameter is null.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <see cref="T:System.Data.SqlClient.SqlCommand" /> object already has a <see cref="T:System.Data.Sql.SqlNotificationRequest" /> object assigned to its <see cref="P:System.Data.SqlClient.SqlCommand.Notification" /> property, and that <see cref="T:System.Data.Sql.SqlNotificationRequest" /> is not associated with this dependency.</exception>
		public void AddCommandDependency(SqlCommand command)
		{
			if (command == null)
			{
				throw ADP.ArgumentNull("command");
			}
			this.AddCommandInternal(command);
		}

		/// <summary>Starts the listener for receiving dependency change notifications from the instance of SQL Server specified by the connection string.</summary>
		/// <param name="connectionString">The connection string for the instance of SQL Server from which to obtain change notifications.</param>
		/// <returns>
		///   <see langword="true" /> if the listener initialized successfully; <see langword="false" /> if a compatible listener already exists.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="connectionString" /> parameter is NULL.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="connectionString" /> parameter is the same as a previous call to this method, but the parameters are different.  
		///  The method was called from within the CLR.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required <see cref="T:System.Data.SqlClient.SqlClientPermission" /> code access security (CAS) permission.</exception>
		/// <exception cref="T:System.Data.SqlClient.SqlException">A subsequent call to the method has been made with an equivalent <paramref name="connectionString" /> parameter with a different user, or a user that does not default to the same schema.  
		///  Also, any underlying SqlClient exceptions.</exception>
		public static bool Start(string connectionString)
		{
			return SqlDependency.Start(connectionString, null, true);
		}

		/// <summary>Starts the listener for receiving dependency change notifications from the instance of SQL Server specified by the connection string using the specified SQL Server Service Broker queue.</summary>
		/// <param name="connectionString">The connection string for the instance of SQL Server from which to obtain change notifications.</param>
		/// <param name="queue">An existing SQL Server Service Broker queue to be used. If <see langword="null" />, the default queue is used.</param>
		/// <returns>
		///   <see langword="true" /> if the listener initialized successfully; <see langword="false" /> if a compatible listener already exists.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="connectionString" /> parameter is NULL.</exception>
		/// <exception cref="T:System.InvalidOperationException">The <paramref name="connectionString" /> parameter is the same as a previous call to this method, but the parameters are different.  
		///  The method was called from within the CLR.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required <see cref="T:System.Data.SqlClient.SqlClientPermission" /> code access security (CAS) permission.</exception>
		/// <exception cref="T:System.Data.SqlClient.SqlException">A subsequent call to the method has been made with an equivalent <paramref name="connectionString" /> parameter but a different user, or a user that does not default to the same schema.  
		///  Also, any underlying SqlClient exceptions.</exception>
		public static bool Start(string connectionString, string queue)
		{
			return SqlDependency.Start(connectionString, queue, false);
		}

		internal static bool Start(string connectionString, string queue, bool useDefaults)
		{
			if (!string.IsNullOrEmpty(connectionString))
			{
				if (!useDefaults && string.IsNullOrEmpty(queue))
				{
					useDefaults = true;
					queue = null;
				}
				bool flag = false;
				bool result = false;
				object obj = SqlDependency.s_startStopLock;
				lock (obj)
				{
					try
					{
						if (SqlDependency.s_processDispatcher == null)
						{
							SqlDependency.s_processDispatcher = SqlDependencyProcessDispatcher.SingletonProcessDispatcher;
						}
						if (useDefaults)
						{
							string server = null;
							DbConnectionPoolIdentity identity = null;
							string userName = null;
							string database = null;
							string service = null;
							bool flag3 = false;
							RuntimeHelpers.PrepareConstrainedRegions();
							try
							{
								result = SqlDependency.s_processDispatcher.StartWithDefault(connectionString, out server, out identity, out userName, out database, ref service, SqlDependency.s_appDomainKey, SqlDependencyPerAppDomainDispatcher.SingletonInstance, out flag, out flag3);
								goto IL_FF;
							}
							finally
							{
								if (flag3 && !flag)
								{
									SqlDependency.IdentityUserNamePair identityUser = new SqlDependency.IdentityUserNamePair(identity, userName);
									SqlDependency.DatabaseServicePair databaseService = new SqlDependency.DatabaseServicePair(database, service);
									if (!SqlDependency.AddToServerUserHash(server, identityUser, databaseService))
									{
										try
										{
											SqlDependency.Stop(connectionString, queue, useDefaults, true);
										}
										catch (Exception e)
										{
											if (!ADP.IsCatchableExceptionType(e))
											{
												throw;
											}
											ADP.TraceExceptionWithoutRethrow(e);
										}
										throw SQL.SqlDependencyDuplicateStart();
									}
								}
							}
						}
						result = SqlDependency.s_processDispatcher.Start(connectionString, queue, SqlDependency.s_appDomainKey, SqlDependencyPerAppDomainDispatcher.SingletonInstance);
						IL_FF:;
					}
					catch (Exception e2)
					{
						if (!ADP.IsCatchableExceptionType(e2))
						{
							throw;
						}
						ADP.TraceExceptionWithoutRethrow(e2);
						throw;
					}
				}
				return result;
			}
			if (connectionString == null)
			{
				throw ADP.ArgumentNull("connectionString");
			}
			throw ADP.Argument("connectionString");
		}

		/// <summary>Stops a listener for a connection specified in a previous <see cref="Overload:System.Data.SqlClient.SqlDependency.Start" /> call.</summary>
		/// <param name="connectionString">Connection string for the instance of SQL Server that was used in a previous <see cref="M:System.Data.SqlClient.SqlDependency.Start(System.String)" /> call.</param>
		/// <returns>
		///   <see langword="true" /> if the listener was completely stopped; <see langword="false" /> if the <see cref="T:System.AppDomain" /> was unbound from the listener, but there are is at least one other <see cref="T:System.AppDomain" /> using the same listener.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="connectionString" /> parameter is NULL.</exception>
		/// <exception cref="T:System.InvalidOperationException">The method was called from within SQLCLR.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required <see cref="T:System.Data.SqlClient.SqlClientPermission" /> code access security (CAS) permission.</exception>
		/// <exception cref="T:System.Data.SqlClient.SqlException">An underlying SqlClient exception occurred.</exception>
		public static bool Stop(string connectionString)
		{
			return SqlDependency.Stop(connectionString, null, true, false);
		}

		/// <summary>Stops a listener for a connection specified in a previous <see cref="Overload:System.Data.SqlClient.SqlDependency.Start" /> call.</summary>
		/// <param name="connectionString">Connection string for the instance of SQL Server that was used in a previous <see cref="M:System.Data.SqlClient.SqlDependency.Start(System.String,System.String)" /> call.</param>
		/// <param name="queue">The SQL Server Service Broker queue that was used in a previous <see cref="M:System.Data.SqlClient.SqlDependency.Start(System.String,System.String)" /> call.</param>
		/// <returns>
		///   <see langword="true" /> if the listener was completely stopped; <see langword="false" /> if the <see cref="T:System.AppDomain" /> was unbound from the listener, but there is at least one other <see cref="T:System.AppDomain" /> using the same listener.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="connectionString" /> parameter is NULL.</exception>
		/// <exception cref="T:System.InvalidOperationException">The method was called from within SQLCLR.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required <see cref="T:System.Data.SqlClient.SqlClientPermission" /> code access security (CAS) permission.</exception>
		/// <exception cref="T:System.Data.SqlClient.SqlException">And underlying SqlClient exception occurred.</exception>
		public static bool Stop(string connectionString, string queue)
		{
			return SqlDependency.Stop(connectionString, queue, false, false);
		}

		internal static bool Stop(string connectionString, string queue, bool useDefaults, bool startFailed)
		{
			if (!string.IsNullOrEmpty(connectionString))
			{
				if (!useDefaults && string.IsNullOrEmpty(queue))
				{
					useDefaults = true;
					queue = null;
				}
				bool result = false;
				object obj = SqlDependency.s_startStopLock;
				lock (obj)
				{
					if (SqlDependency.s_processDispatcher != null)
					{
						try
						{
							string server = null;
							DbConnectionPoolIdentity identity = null;
							string userName = null;
							string database = null;
							string service = null;
							if (useDefaults)
							{
								bool flag2 = false;
								RuntimeHelpers.PrepareConstrainedRegions();
								try
								{
									result = SqlDependency.s_processDispatcher.Stop(connectionString, out server, out identity, out userName, out database, ref service, SqlDependency.s_appDomainKey, out flag2);
									goto IL_CB;
								}
								finally
								{
									if (flag2 && !startFailed)
									{
										SqlDependency.IdentityUserNamePair identityUser = new SqlDependency.IdentityUserNamePair(identity, userName);
										SqlDependency.DatabaseServicePair databaseService = new SqlDependency.DatabaseServicePair(database, service);
										SqlDependency.RemoveFromServerUserHash(server, identityUser, databaseService);
									}
								}
							}
							bool flag3;
							result = SqlDependency.s_processDispatcher.Stop(connectionString, out server, out identity, out userName, out database, ref queue, SqlDependency.s_appDomainKey, out flag3);
							IL_CB:;
						}
						catch (Exception e)
						{
							if (!ADP.IsCatchableExceptionType(e))
							{
								throw;
							}
							ADP.TraceExceptionWithoutRethrow(e);
						}
					}
				}
				return result;
			}
			if (connectionString == null)
			{
				throw ADP.ArgumentNull("connectionString");
			}
			throw ADP.Argument("connectionString");
		}

		private static bool AddToServerUserHash(string server, SqlDependency.IdentityUserNamePair identityUser, SqlDependency.DatabaseServicePair databaseService)
		{
			bool result = false;
			Dictionary<string, Dictionary<SqlDependency.IdentityUserNamePair, List<SqlDependency.DatabaseServicePair>>> obj = SqlDependency.s_serverUserHash;
			lock (obj)
			{
				Dictionary<SqlDependency.IdentityUserNamePair, List<SqlDependency.DatabaseServicePair>> dictionary;
				if (!SqlDependency.s_serverUserHash.ContainsKey(server))
				{
					dictionary = new Dictionary<SqlDependency.IdentityUserNamePair, List<SqlDependency.DatabaseServicePair>>();
					SqlDependency.s_serverUserHash.Add(server, dictionary);
				}
				else
				{
					dictionary = SqlDependency.s_serverUserHash[server];
				}
				List<SqlDependency.DatabaseServicePair> list;
				if (!dictionary.ContainsKey(identityUser))
				{
					list = new List<SqlDependency.DatabaseServicePair>();
					dictionary.Add(identityUser, list);
				}
				else
				{
					list = dictionary[identityUser];
				}
				if (!list.Contains(databaseService))
				{
					list.Add(databaseService);
					result = true;
				}
			}
			return result;
		}

		private static void RemoveFromServerUserHash(string server, SqlDependency.IdentityUserNamePair identityUser, SqlDependency.DatabaseServicePair databaseService)
		{
			Dictionary<string, Dictionary<SqlDependency.IdentityUserNamePair, List<SqlDependency.DatabaseServicePair>>> obj = SqlDependency.s_serverUserHash;
			lock (obj)
			{
				if (SqlDependency.s_serverUserHash.ContainsKey(server))
				{
					Dictionary<SqlDependency.IdentityUserNamePair, List<SqlDependency.DatabaseServicePair>> dictionary = SqlDependency.s_serverUserHash[server];
					if (dictionary.ContainsKey(identityUser))
					{
						List<SqlDependency.DatabaseServicePair> list = dictionary[identityUser];
						int num = list.IndexOf(databaseService);
						if (num >= 0)
						{
							list.RemoveAt(num);
							if (list.Count == 0)
							{
								dictionary.Remove(identityUser);
								if (dictionary.Count == 0)
								{
									SqlDependency.s_serverUserHash.Remove(server);
								}
							}
						}
					}
				}
			}
		}

		internal static string GetDefaultComposedOptions(string server, string failoverServer, SqlDependency.IdentityUserNamePair identityUser, string database)
		{
			Dictionary<string, Dictionary<SqlDependency.IdentityUserNamePair, List<SqlDependency.DatabaseServicePair>>> obj = SqlDependency.s_serverUserHash;
			string result;
			lock (obj)
			{
				if (!SqlDependency.s_serverUserHash.ContainsKey(server))
				{
					if (SqlDependency.s_serverUserHash.Count == 0)
					{
						throw SQL.SqlDepDefaultOptionsButNoStart();
					}
					if (string.IsNullOrEmpty(failoverServer) || !SqlDependency.s_serverUserHash.ContainsKey(failoverServer))
					{
						throw SQL.SqlDependencyNoMatchingServerStart();
					}
					server = failoverServer;
				}
				Dictionary<SqlDependency.IdentityUserNamePair, List<SqlDependency.DatabaseServicePair>> dictionary = SqlDependency.s_serverUserHash[server];
				List<SqlDependency.DatabaseServicePair> list = null;
				if (!dictionary.ContainsKey(identityUser))
				{
					if (dictionary.Count > 1)
					{
						throw SQL.SqlDependencyNoMatchingServerStart();
					}
					using (Dictionary<SqlDependency.IdentityUserNamePair, List<SqlDependency.DatabaseServicePair>>.Enumerator enumerator = dictionary.GetEnumerator())
					{
						if (!enumerator.MoveNext())
						{
							goto IL_B6;
						}
						KeyValuePair<SqlDependency.IdentityUserNamePair, List<SqlDependency.DatabaseServicePair>> keyValuePair = enumerator.Current;
						list = keyValuePair.Value;
						goto IL_B6;
					}
				}
				list = dictionary[identityUser];
				IL_B6:
				SqlDependency.DatabaseServicePair item = new SqlDependency.DatabaseServicePair(database, null);
				SqlDependency.DatabaseServicePair databaseServicePair = null;
				int num = list.IndexOf(item);
				if (num != -1)
				{
					databaseServicePair = list[num];
				}
				if (databaseServicePair != null)
				{
					database = SqlDependency.FixupServiceOrDatabaseName(databaseServicePair.Database);
					string str = SqlDependency.FixupServiceOrDatabaseName(databaseServicePair.Service);
					result = "Service=" + str + ";Local Database=" + database;
				}
				else
				{
					if (list.Count != 1)
					{
						throw SQL.SqlDependencyNoMatchingServerDatabaseStart();
					}
					object[] array = list.ToArray();
					databaseServicePair = (SqlDependency.DatabaseServicePair)array[0];
					string str2 = SqlDependency.FixupServiceOrDatabaseName(databaseServicePair.Database);
					string str3 = SqlDependency.FixupServiceOrDatabaseName(databaseServicePair.Service);
					result = "Service=" + str3 + ";Local Database=" + str2;
				}
			}
			return result;
		}

		internal void AddToServerList(string server)
		{
			List<string> serverList = this._serverList;
			lock (serverList)
			{
				int num = this._serverList.BinarySearch(server, StringComparer.OrdinalIgnoreCase);
				if (0 > num)
				{
					num = ~num;
					this._serverList.Insert(num, server);
				}
			}
		}

		internal bool ContainsServer(string server)
		{
			List<string> serverList = this._serverList;
			bool result;
			lock (serverList)
			{
				result = this._serverList.Contains(server);
			}
			return result;
		}

		internal string ComputeHashAndAddToDispatcher(SqlCommand command)
		{
			string commandHash = this.ComputeCommandHash(command.Connection.ConnectionString, command);
			return SqlDependencyPerAppDomainDispatcher.SingletonInstance.AddCommandEntry(commandHash, this);
		}

		internal void Invalidate(SqlNotificationType type, SqlNotificationInfo info, SqlNotificationSource source)
		{
			List<SqlDependency.EventContextPair> list = null;
			object eventHandlerLock = this._eventHandlerLock;
			lock (eventHandlerLock)
			{
				if (this._dependencyFired && SqlNotificationInfo.AlreadyChanged != info && SqlNotificationSource.Client != source)
				{
					if (this.ExpirationTime >= DateTime.UtcNow)
					{
					}
				}
				else
				{
					this._dependencyFired = true;
					list = this._eventList;
					this._eventList = new List<SqlDependency.EventContextPair>();
				}
			}
			if (list != null)
			{
				foreach (SqlDependency.EventContextPair eventContextPair in list)
				{
					eventContextPair.Invoke(new SqlNotificationEventArgs(type, info, source));
				}
			}
		}

		internal void StartTimer(SqlNotificationRequest notificationRequest)
		{
			if (this._expirationTime == DateTime.MaxValue)
			{
				int num = 432000;
				if (this._timeout != 0)
				{
					num = this._timeout;
				}
				if (notificationRequest != null && notificationRequest.Timeout < num && notificationRequest.Timeout != 0)
				{
					num = notificationRequest.Timeout;
				}
				this._expirationTime = DateTime.UtcNow.AddSeconds((double)num);
				SqlDependencyPerAppDomainDispatcher.SingletonInstance.StartTimer(this);
			}
		}

		private void AddCommandInternal(SqlCommand cmd)
		{
			if (cmd != null)
			{
				SqlConnection connection = cmd.Connection;
				if (cmd.Notification != null)
				{
					if (cmd._sqlDep == null || cmd._sqlDep != this)
					{
						throw SQL.SqlCommandHasExistingSqlNotificationRequest();
					}
				}
				else
				{
					bool flag = false;
					object eventHandlerLock = this._eventHandlerLock;
					lock (eventHandlerLock)
					{
						if (!this._dependencyFired)
						{
							cmd.Notification = new SqlNotificationRequest
							{
								Timeout = this._timeout
							};
							if (this._options != null)
							{
								cmd.Notification.Options = this._options;
							}
							cmd._sqlDep = this;
						}
						else if (this._eventList.Count == 0)
						{
							flag = true;
						}
					}
					if (flag)
					{
						this.Invalidate(SqlNotificationType.Subscribe, SqlNotificationInfo.AlreadyChanged, SqlNotificationSource.Client);
					}
				}
			}
		}

		private string ComputeCommandHash(string connectionString, SqlCommand command)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0};{1}", connectionString, command.CommandText);
			for (int i = 0; i < command.Parameters.Count; i++)
			{
				object value = command.Parameters[i].Value;
				if (value == null || value == DBNull.Value)
				{
					stringBuilder.Append("; NULL");
				}
				else
				{
					Type type = value.GetType();
					if (type == typeof(byte[]))
					{
						stringBuilder.Append(";");
						byte[] array = (byte[])value;
						for (int j = 0; j < array.Length; j++)
						{
							stringBuilder.Append(array[j].ToString("x2", CultureInfo.InvariantCulture));
						}
					}
					else if (type == typeof(char[]))
					{
						stringBuilder.Append((char[])value);
					}
					else if (type == typeof(XmlReader))
					{
						stringBuilder.Append(";");
						stringBuilder.Append(Guid.NewGuid().ToString());
					}
					else
					{
						stringBuilder.Append(";");
						stringBuilder.Append(value.ToString());
					}
				}
			}
			return stringBuilder.ToString();
		}

		internal static string FixupServiceOrDatabaseName(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				return "\"" + name.Replace("\"", "\"\"") + "\"";
			}
			return name;
		}

		private readonly string _id = Guid.NewGuid().ToString() + ";" + SqlDependency.s_appDomainKey;

		private string _options;

		private int _timeout;

		private bool _dependencyFired;

		private List<SqlDependency.EventContextPair> _eventList = new List<SqlDependency.EventContextPair>();

		private object _eventHandlerLock = new object();

		private DateTime _expirationTime = DateTime.MaxValue;

		private List<string> _serverList = new List<string>();

		private static object s_startStopLock = new object();

		private static readonly string s_appDomainKey = Guid.NewGuid().ToString();

		private static Dictionary<string, Dictionary<SqlDependency.IdentityUserNamePair, List<SqlDependency.DatabaseServicePair>>> s_serverUserHash = new Dictionary<string, Dictionary<SqlDependency.IdentityUserNamePair, List<SqlDependency.DatabaseServicePair>>>(StringComparer.OrdinalIgnoreCase);

		private static SqlDependencyProcessDispatcher s_processDispatcher = null;

		private static readonly string s_assemblyName = typeof(SqlDependencyProcessDispatcher).Assembly.FullName;

		private static readonly string s_typeName = typeof(SqlDependencyProcessDispatcher).FullName;

		internal class IdentityUserNamePair
		{
			internal IdentityUserNamePair(DbConnectionPoolIdentity identity, string userName)
			{
				this._identity = identity;
				this._userName = userName;
			}

			internal DbConnectionPoolIdentity Identity
			{
				get
				{
					return this._identity;
				}
			}

			internal string UserName
			{
				get
				{
					return this._userName;
				}
			}

			public override bool Equals(object value)
			{
				SqlDependency.IdentityUserNamePair identityUserNamePair = (SqlDependency.IdentityUserNamePair)value;
				bool result = false;
				if (identityUserNamePair == null)
				{
					result = false;
				}
				else if (this == identityUserNamePair)
				{
					result = true;
				}
				else if (this._identity != null)
				{
					if (this._identity.Equals(identityUserNamePair._identity))
					{
						result = true;
					}
				}
				else if (this._userName == identityUserNamePair._userName)
				{
					result = true;
				}
				return result;
			}

			public override int GetHashCode()
			{
				int hashCode;
				if (this._identity != null)
				{
					hashCode = this._identity.GetHashCode();
				}
				else
				{
					hashCode = this._userName.GetHashCode();
				}
				return hashCode;
			}

			private DbConnectionPoolIdentity _identity;

			private string _userName;
		}

		private class DatabaseServicePair
		{
			internal DatabaseServicePair(string database, string service)
			{
				this._database = database;
				this._service = service;
			}

			internal string Database
			{
				get
				{
					return this._database;
				}
			}

			internal string Service
			{
				get
				{
					return this._service;
				}
			}

			public override bool Equals(object value)
			{
				SqlDependency.DatabaseServicePair databaseServicePair = (SqlDependency.DatabaseServicePair)value;
				bool result = false;
				if (databaseServicePair == null)
				{
					result = false;
				}
				else if (this == databaseServicePair)
				{
					result = true;
				}
				else if (this._database == databaseServicePair._database)
				{
					result = true;
				}
				return result;
			}

			public override int GetHashCode()
			{
				return this._database.GetHashCode();
			}

			private string _database;

			private string _service;
		}

		internal class EventContextPair
		{
			internal EventContextPair(OnChangeEventHandler eventHandler, SqlDependency dependency)
			{
				this._eventHandler = eventHandler;
				this._context = ExecutionContext.Capture();
				this._dependency = dependency;
			}

			public override bool Equals(object value)
			{
				SqlDependency.EventContextPair eventContextPair = (SqlDependency.EventContextPair)value;
				bool result = false;
				if (eventContextPair == null)
				{
					result = false;
				}
				else if (this == eventContextPair)
				{
					result = true;
				}
				else if (this._eventHandler == eventContextPair._eventHandler)
				{
					result = true;
				}
				return result;
			}

			public override int GetHashCode()
			{
				return this._eventHandler.GetHashCode();
			}

			internal void Invoke(SqlNotificationEventArgs args)
			{
				this._args = args;
				ExecutionContext.Run(this._context, SqlDependency.EventContextPair.s_contextCallback, this);
			}

			private static void InvokeCallback(object eventContextPair)
			{
				SqlDependency.EventContextPair eventContextPair2 = (SqlDependency.EventContextPair)eventContextPair;
				eventContextPair2._eventHandler(eventContextPair2._dependency, eventContextPair2._args);
			}

			private OnChangeEventHandler _eventHandler;

			private ExecutionContext _context;

			private SqlDependency _dependency;

			private SqlNotificationEventArgs _args;

			private static ContextCallback s_contextCallback = new ContextCallback(SqlDependency.EventContextPair.InvokeCallback);
		}
	}
}
