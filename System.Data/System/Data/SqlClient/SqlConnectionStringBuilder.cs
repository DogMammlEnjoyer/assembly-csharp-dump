using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using Unity;

namespace System.Data.SqlClient
{
	/// <summary>Provides a simple way to create and manage the contents of connection strings used by the <see cref="T:System.Data.SqlClient.SqlConnection" /> class.</summary>
	public sealed class SqlConnectionStringBuilder : DbConnectionStringBuilder
	{
		private static string[] CreateValidKeywords()
		{
			string[] array = new string[29];
			array[25] = "ApplicationIntent";
			array[20] = "Application Name";
			array[2] = "AttachDbFilename";
			array[14] = "Connect Timeout";
			array[21] = "Current Language";
			array[0] = "Data Source";
			array[15] = "Encrypt";
			array[8] = "Enlist";
			array[1] = "Failover Partner";
			array[3] = "Initial Catalog";
			array[4] = "Integrated Security";
			array[17] = "Load Balance Timeout";
			array[11] = "Max Pool Size";
			array[10] = "Min Pool Size";
			array[12] = "MultipleActiveResultSets";
			array[26] = "MultiSubnetFailover";
			array[18] = "Packet Size";
			array[7] = "Password";
			array[5] = "Persist Security Info";
			array[9] = "Pooling";
			array[13] = "Replication";
			array[24] = "Transaction Binding";
			array[16] = "TrustServerCertificate";
			array[19] = "Type System Version";
			array[6] = "User ID";
			array[23] = "User Instance";
			array[22] = "Workstation ID";
			array[27] = "ConnectRetryCount";
			array[28] = "ConnectRetryInterval";
			return array;
		}

		private static Dictionary<string, SqlConnectionStringBuilder.Keywords> CreateKeywordsDictionary()
		{
			return new Dictionary<string, SqlConnectionStringBuilder.Keywords>(47, StringComparer.OrdinalIgnoreCase)
			{
				{
					"ApplicationIntent",
					SqlConnectionStringBuilder.Keywords.ApplicationIntent
				},
				{
					"Application Name",
					SqlConnectionStringBuilder.Keywords.ApplicationName
				},
				{
					"AttachDbFilename",
					SqlConnectionStringBuilder.Keywords.AttachDBFilename
				},
				{
					"Connect Timeout",
					SqlConnectionStringBuilder.Keywords.ConnectTimeout
				},
				{
					"Current Language",
					SqlConnectionStringBuilder.Keywords.CurrentLanguage
				},
				{
					"Data Source",
					SqlConnectionStringBuilder.Keywords.DataSource
				},
				{
					"Encrypt",
					SqlConnectionStringBuilder.Keywords.Encrypt
				},
				{
					"Enlist",
					SqlConnectionStringBuilder.Keywords.Enlist
				},
				{
					"Failover Partner",
					SqlConnectionStringBuilder.Keywords.FailoverPartner
				},
				{
					"Initial Catalog",
					SqlConnectionStringBuilder.Keywords.InitialCatalog
				},
				{
					"Integrated Security",
					SqlConnectionStringBuilder.Keywords.IntegratedSecurity
				},
				{
					"Load Balance Timeout",
					SqlConnectionStringBuilder.Keywords.LoadBalanceTimeout
				},
				{
					"MultipleActiveResultSets",
					SqlConnectionStringBuilder.Keywords.MultipleActiveResultSets
				},
				{
					"Max Pool Size",
					SqlConnectionStringBuilder.Keywords.MaxPoolSize
				},
				{
					"Min Pool Size",
					SqlConnectionStringBuilder.Keywords.MinPoolSize
				},
				{
					"MultiSubnetFailover",
					SqlConnectionStringBuilder.Keywords.MultiSubnetFailover
				},
				{
					"Packet Size",
					SqlConnectionStringBuilder.Keywords.PacketSize
				},
				{
					"Password",
					SqlConnectionStringBuilder.Keywords.Password
				},
				{
					"Persist Security Info",
					SqlConnectionStringBuilder.Keywords.PersistSecurityInfo
				},
				{
					"Pooling",
					SqlConnectionStringBuilder.Keywords.Pooling
				},
				{
					"Replication",
					SqlConnectionStringBuilder.Keywords.Replication
				},
				{
					"Transaction Binding",
					SqlConnectionStringBuilder.Keywords.TransactionBinding
				},
				{
					"TrustServerCertificate",
					SqlConnectionStringBuilder.Keywords.TrustServerCertificate
				},
				{
					"Type System Version",
					SqlConnectionStringBuilder.Keywords.TypeSystemVersion
				},
				{
					"User ID",
					SqlConnectionStringBuilder.Keywords.UserID
				},
				{
					"User Instance",
					SqlConnectionStringBuilder.Keywords.UserInstance
				},
				{
					"Workstation ID",
					SqlConnectionStringBuilder.Keywords.WorkstationID
				},
				{
					"ConnectRetryCount",
					SqlConnectionStringBuilder.Keywords.ConnectRetryCount
				},
				{
					"ConnectRetryInterval",
					SqlConnectionStringBuilder.Keywords.ConnectRetryInterval
				},
				{
					"app",
					SqlConnectionStringBuilder.Keywords.ApplicationName
				},
				{
					"extended properties",
					SqlConnectionStringBuilder.Keywords.AttachDBFilename
				},
				{
					"initial file name",
					SqlConnectionStringBuilder.Keywords.AttachDBFilename
				},
				{
					"connection timeout",
					SqlConnectionStringBuilder.Keywords.ConnectTimeout
				},
				{
					"timeout",
					SqlConnectionStringBuilder.Keywords.ConnectTimeout
				},
				{
					"language",
					SqlConnectionStringBuilder.Keywords.CurrentLanguage
				},
				{
					"addr",
					SqlConnectionStringBuilder.Keywords.DataSource
				},
				{
					"address",
					SqlConnectionStringBuilder.Keywords.DataSource
				},
				{
					"network address",
					SqlConnectionStringBuilder.Keywords.DataSource
				},
				{
					"server",
					SqlConnectionStringBuilder.Keywords.DataSource
				},
				{
					"database",
					SqlConnectionStringBuilder.Keywords.InitialCatalog
				},
				{
					"trusted_connection",
					SqlConnectionStringBuilder.Keywords.IntegratedSecurity
				},
				{
					"connection lifetime",
					SqlConnectionStringBuilder.Keywords.LoadBalanceTimeout
				},
				{
					"pwd",
					SqlConnectionStringBuilder.Keywords.Password
				},
				{
					"persistsecurityinfo",
					SqlConnectionStringBuilder.Keywords.PersistSecurityInfo
				},
				{
					"uid",
					SqlConnectionStringBuilder.Keywords.UserID
				},
				{
					"user",
					SqlConnectionStringBuilder.Keywords.UserID
				},
				{
					"wsid",
					SqlConnectionStringBuilder.Keywords.WorkstationID
				}
			};
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" /> class.</summary>
		public SqlConnectionStringBuilder() : this(null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" /> class. The provided connection string provides the data for the instance's internal connection information.</summary>
		/// <param name="connectionString">The basis for the object's internal connection information. Parsed into name/value pairs. Invalid key names raise <see cref="T:System.Collections.Generic.KeyNotFoundException" />.</param>
		/// <exception cref="T:System.Collections.Generic.KeyNotFoundException">Invalid key name within the connection string.</exception>
		/// <exception cref="T:System.FormatException">Invalid value within the connection string (specifically, when a Boolean or numeric value was expected but not supplied).</exception>
		/// <exception cref="T:System.ArgumentException">The supplied <paramref name="connectionString" /> is not valid.</exception>
		public SqlConnectionStringBuilder(string connectionString)
		{
			if (!string.IsNullOrEmpty(connectionString))
			{
				base.ConnectionString = connectionString;
			}
		}

		/// <summary>Gets or sets the value associated with the specified key. In C#, this property is the indexer.</summary>
		/// <param name="keyword">The key of the item to get or set.</param>
		/// <returns>The value associated with the specified key.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> is a null reference (<see langword="Nothing" /> in Visual Basic).</exception>
		/// <exception cref="T:System.Collections.Generic.KeyNotFoundException">Tried to add a key that does not exist within the available keys.</exception>
		/// <exception cref="T:System.FormatException">Invalid value within the connection string (specifically, a Boolean or numeric value was expected but not supplied).</exception>
		public override object this[string keyword]
		{
			get
			{
				SqlConnectionStringBuilder.Keywords index = this.GetIndex(keyword);
				return this.GetAt(index);
			}
			set
			{
				if (value == null)
				{
					this.Remove(keyword);
					return;
				}
				switch (this.GetIndex(keyword))
				{
				case SqlConnectionStringBuilder.Keywords.DataSource:
					this.DataSource = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.FailoverPartner:
					this.FailoverPartner = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.AttachDBFilename:
					this.AttachDBFilename = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.InitialCatalog:
					this.InitialCatalog = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.IntegratedSecurity:
					this.IntegratedSecurity = SqlConnectionStringBuilder.ConvertToIntegratedSecurity(value);
					return;
				case SqlConnectionStringBuilder.Keywords.PersistSecurityInfo:
					this.PersistSecurityInfo = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.UserID:
					this.UserID = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.Password:
					this.Password = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.Enlist:
					this.Enlist = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.Pooling:
					this.Pooling = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.MinPoolSize:
					this.MinPoolSize = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				case SqlConnectionStringBuilder.Keywords.MaxPoolSize:
					this.MaxPoolSize = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				case SqlConnectionStringBuilder.Keywords.MultipleActiveResultSets:
					this.MultipleActiveResultSets = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.Replication:
					this.Replication = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.ConnectTimeout:
					this.ConnectTimeout = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				case SqlConnectionStringBuilder.Keywords.Encrypt:
					this.Encrypt = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.TrustServerCertificate:
					this.TrustServerCertificate = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.LoadBalanceTimeout:
					this.LoadBalanceTimeout = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				case SqlConnectionStringBuilder.Keywords.PacketSize:
					this.PacketSize = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				case SqlConnectionStringBuilder.Keywords.TypeSystemVersion:
					this.TypeSystemVersion = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.ApplicationName:
					this.ApplicationName = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.CurrentLanguage:
					this.CurrentLanguage = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.WorkstationID:
					this.WorkstationID = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.UserInstance:
					this.UserInstance = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.TransactionBinding:
					this.TransactionBinding = SqlConnectionStringBuilder.ConvertToString(value);
					return;
				case SqlConnectionStringBuilder.Keywords.ApplicationIntent:
					this.ApplicationIntent = SqlConnectionStringBuilder.ConvertToApplicationIntent(keyword, value);
					return;
				case SqlConnectionStringBuilder.Keywords.MultiSubnetFailover:
					this.MultiSubnetFailover = SqlConnectionStringBuilder.ConvertToBoolean(value);
					return;
				case SqlConnectionStringBuilder.Keywords.ConnectRetryCount:
					this.ConnectRetryCount = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				case SqlConnectionStringBuilder.Keywords.ConnectRetryInterval:
					this.ConnectRetryInterval = SqlConnectionStringBuilder.ConvertToInt32(value);
					return;
				default:
					throw this.UnsupportedKeyword(keyword);
				}
			}
		}

		/// <summary>Declares the application workload type when connecting to a database in an SQL Server Availability Group. You can set the value of this property with <see cref="T:System.Data.SqlClient.ApplicationIntent" />. For more information about SqlClient support for Always On Availability Groups, see SqlClient Support for High Availability, Disaster Recovery.</summary>
		/// <returns>Returns the current value of the property (a value of type <see cref="T:System.Data.SqlClient.ApplicationIntent" />).</returns>
		public ApplicationIntent ApplicationIntent
		{
			get
			{
				return this._applicationIntent;
			}
			set
			{
				if (!DbConnectionStringBuilderUtil.IsValidApplicationIntentValue(value))
				{
					throw ADP.InvalidEnumerationValue(typeof(ApplicationIntent), (int)value);
				}
				this.SetApplicationIntentValue(value);
				this._applicationIntent = value;
			}
		}

		/// <summary>Gets or sets the name of the application associated with the connection string.</summary>
		/// <returns>The name of the application, or ".NET SqlClient Data Provider" if no name has been supplied.</returns>
		/// <exception cref="T:System.ArgumentNullException">To set the value to null, use <see cref="F:System.DBNull.Value" />.</exception>
		public string ApplicationName
		{
			get
			{
				return this._applicationName;
			}
			set
			{
				this.SetValue("Application Name", value);
				this._applicationName = value;
			}
		}

		/// <summary>Gets or sets a string that contains the name of the primary data file. This includes the full path name of an attachable database.</summary>
		/// <returns>The value of the <see langword="AttachDBFilename" /> property, or <see langword="String.Empty" /> if no value has been supplied.</returns>
		/// <exception cref="T:System.ArgumentNullException">To set the value to null, use <see cref="F:System.DBNull.Value" />.</exception>
		public string AttachDBFilename
		{
			get
			{
				return this._attachDBFilename;
			}
			set
			{
				this.SetValue("AttachDbFilename", value);
				this._attachDBFilename = value;
			}
		}

		/// <summary>Gets or sets the length of time (in seconds) to wait for a connection to the server before terminating the attempt and generating an error.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.ConnectTimeout" /> property, or 15 seconds if no value has been supplied.</returns>
		public int ConnectTimeout
		{
			get
			{
				return this._connectTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw ADP.InvalidConnectionOptionValue("Connect Timeout");
				}
				this.SetValue("Connect Timeout", value);
				this._connectTimeout = value;
			}
		}

		/// <summary>Gets or sets the SQL Server Language record name.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.CurrentLanguage" /> property, or <see langword="String.Empty" /> if no value has been supplied.</returns>
		/// <exception cref="T:System.ArgumentNullException">To set the value to null, use <see cref="F:System.DBNull.Value" />.</exception>
		public string CurrentLanguage
		{
			get
			{
				return this._currentLanguage;
			}
			set
			{
				this.SetValue("Current Language", value);
				this._currentLanguage = value;
			}
		}

		/// <summary>Gets or sets the name or network address of the instance of SQL Server to connect to.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.DataSource" /> property, or <see langword="String.Empty" /> if none has been supplied.</returns>
		/// <exception cref="T:System.ArgumentNullException">To set the value to null, use <see cref="F:System.DBNull.Value" />.</exception>
		public string DataSource
		{
			get
			{
				return this._dataSource;
			}
			set
			{
				this.SetValue("Data Source", value);
				this._dataSource = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that indicates whether SQL Server uses SSL encryption for all data sent between the client and server if the server has a certificate installed.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.Encrypt" /> property, or <see langword="false" /> if none has been supplied.</returns>
		public bool Encrypt
		{
			get
			{
				return this._encrypt;
			}
			set
			{
				this.SetValue("Encrypt", value);
				this._encrypt = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether the channel will be encrypted while bypassing walking the certificate chain to validate trust.</summary>
		/// <returns>A <see langword="Boolean" />. Recognized values are <see langword="true" />, <see langword="false" />, <see langword="yes" />, and <see langword="no" />.</returns>
		public bool TrustServerCertificate
		{
			get
			{
				return this._trustServerCertificate;
			}
			set
			{
				this.SetValue("TrustServerCertificate", value);
				this._trustServerCertificate = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that indicates whether the SQL Server connection pooler automatically enlists the connection in the creation thread's current transaction context.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.Enlist" /> property, or <see langword="true" /> if none has been supplied.</returns>
		public bool Enlist
		{
			get
			{
				return this._enlist;
			}
			set
			{
				this.SetValue("Enlist", value);
				this._enlist = value;
			}
		}

		/// <summary>Gets or sets the name or address of the partner server to connect to if the primary server is down.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.FailoverPartner" /> property, or <see langword="String.Empty" /> if none has been supplied.</returns>
		/// <exception cref="T:System.ArgumentNullException">To set the value to null, use <see cref="F:System.DBNull.Value" />.</exception>
		public string FailoverPartner
		{
			get
			{
				return this._failoverPartner;
			}
			set
			{
				this.SetValue("Failover Partner", value);
				this._failoverPartner = value;
			}
		}

		/// <summary>Gets or sets the name of the database associated with the connection.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.InitialCatalog" /> property, or <see langword="String.Empty" /> if none has been supplied.</returns>
		/// <exception cref="T:System.ArgumentNullException">To set the value to null, use <see cref="F:System.DBNull.Value" />.</exception>
		[TypeConverter(typeof(SqlConnectionStringBuilder.SqlInitialCatalogConverter))]
		public string InitialCatalog
		{
			get
			{
				return this._initialCatalog;
			}
			set
			{
				this.SetValue("Initial Catalog", value);
				this._initialCatalog = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that indicates whether User ID and Password are specified in the connection (when <see langword="false" />) or whether the current Windows account credentials are used for authentication (when <see langword="true" />).</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.IntegratedSecurity" /> property, or <see langword="false" /> if none has been supplied.</returns>
		public bool IntegratedSecurity
		{
			get
			{
				return this._integratedSecurity;
			}
			set
			{
				this.SetValue("Integrated Security", value);
				this._integratedSecurity = value;
			}
		}

		/// <summary>Gets or sets the minimum time, in seconds, for the connection to live in the connection pool before being destroyed.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.LoadBalanceTimeout" /> property, or 0 if none has been supplied.</returns>
		public int LoadBalanceTimeout
		{
			get
			{
				return this._loadBalanceTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw ADP.InvalidConnectionOptionValue("Load Balance Timeout");
				}
				this.SetValue("Load Balance Timeout", value);
				this._loadBalanceTimeout = value;
			}
		}

		/// <summary>Gets or sets the maximum number of connections allowed in the connection pool for this specific connection string.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.MaxPoolSize" /> property, or 100 if none has been supplied.</returns>
		public int MaxPoolSize
		{
			get
			{
				return this._maxPoolSize;
			}
			set
			{
				if (value < 1)
				{
					throw ADP.InvalidConnectionOptionValue("Max Pool Size");
				}
				this.SetValue("Max Pool Size", value);
				this._maxPoolSize = value;
			}
		}

		/// <summary>The number of reconnections attempted after identifying that there was an idle connection failure. This must be an integer between 0 and 255. Default is 1. Set to 0 to disable reconnecting on idle connection failures. An <see cref="T:System.ArgumentException" /> will be thrown if set to a value outside of the allowed range.</summary>
		/// <returns>The number of reconnections attempted after identifying that there was an idle connection failure.</returns>
		public int ConnectRetryCount
		{
			get
			{
				return this._connectRetryCount;
			}
			set
			{
				if (value < 0 || value > 255)
				{
					throw ADP.InvalidConnectionOptionValue("ConnectRetryCount");
				}
				this.SetValue("ConnectRetryCount", value);
				this._connectRetryCount = value;
			}
		}

		/// <summary>Amount of time (in seconds) between each reconnection attempt after identifying that there was an idle connection failure. This must be an integer between 1 and 60. The default is 10 seconds. An <see cref="T:System.ArgumentException" /> will be thrown if set to a value outside of the allowed range.</summary>
		/// <returns>Amount of time (in seconds) between each reconnection attempt after identifying that there was an idle connection failure.</returns>
		public int ConnectRetryInterval
		{
			get
			{
				return this._connectRetryInterval;
			}
			set
			{
				if (value < 1 || value > 60)
				{
					throw ADP.InvalidConnectionOptionValue("ConnectRetryInterval");
				}
				this.SetValue("ConnectRetryInterval", value);
				this._connectRetryInterval = value;
			}
		}

		/// <summary>Gets or sets the minimum number of connections allowed in the connection pool for this specific connection string.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.MinPoolSize" /> property, or 0 if none has been supplied.</returns>
		public int MinPoolSize
		{
			get
			{
				return this._minPoolSize;
			}
			set
			{
				if (value < 0)
				{
					throw ADP.InvalidConnectionOptionValue("Min Pool Size");
				}
				this.SetValue("Min Pool Size", value);
				this._minPoolSize = value;
			}
		}

		/// <summary>When true, an application can maintain multiple active result sets (MARS). When false, an application must process or cancel all result sets from one batch before it can execute any other batch on that connection.  
		///  For more information, see Multiple Active Result Sets (MARS).</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.MultipleActiveResultSets" /> property, or <see langword="false" /> if none has been supplied.</returns>
		public bool MultipleActiveResultSets
		{
			get
			{
				return this._multipleActiveResultSets;
			}
			set
			{
				this.SetValue("MultipleActiveResultSets", value);
				this._multipleActiveResultSets = value;
			}
		}

		/// <summary>If your application is connecting to an AlwaysOn availability group (AG) on different subnets, setting MultiSubnetFailover=true provides faster detection of and connection to the (currently) active server. For more information about SqlClient support for Always On Availability Groups, see SqlClient Support for High Availability, Disaster Recovery.</summary>
		/// <returns>Returns <see cref="T:System.Boolean" /> indicating the current value of the property.</returns>
		public bool MultiSubnetFailover
		{
			get
			{
				return this._multiSubnetFailover;
			}
			set
			{
				this.SetValue("MultiSubnetFailover", value);
				this._multiSubnetFailover = value;
			}
		}

		/// <summary>Gets or sets the size in bytes of the network packets used to communicate with an instance of SQL Server.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.PacketSize" /> property, or 8000 if none has been supplied.</returns>
		public int PacketSize
		{
			get
			{
				return this._packetSize;
			}
			set
			{
				if (value < 512 || 32768 < value)
				{
					throw SQL.InvalidPacketSizeValue();
				}
				this.SetValue("Packet Size", value);
				this._packetSize = value;
			}
		}

		/// <summary>Gets or sets the password for the SQL Server account.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.Password" /> property, or <see langword="String.Empty" /> if none has been supplied.</returns>
		/// <exception cref="T:System.ArgumentNullException">The password was incorrectly set to null.  See code sample below.</exception>
		public string Password
		{
			get
			{
				return this._password;
			}
			set
			{
				this.SetValue("Password", value);
				this._password = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that indicates if security-sensitive information, such as the password, is not returned as part of the connection if the connection is open or has ever been in an open state.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.PersistSecurityInfo" /> property, or <see langword="false" /> if none has been supplied.</returns>
		public bool PersistSecurityInfo
		{
			get
			{
				return this._persistSecurityInfo;
			}
			set
			{
				this.SetValue("Persist Security Info", value);
				this._persistSecurityInfo = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that indicates whether the connection will be pooled or explicitly opened every time that the connection is requested.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.Pooling" /> property, or <see langword="true" /> if none has been supplied.</returns>
		public bool Pooling
		{
			get
			{
				return this._pooling;
			}
			set
			{
				this.SetValue("Pooling", value);
				this._pooling = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that indicates whether replication is supported using the connection.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.Replication" /> property, or false if none has been supplied.</returns>
		public bool Replication
		{
			get
			{
				return this._replication;
			}
			set
			{
				this.SetValue("Replication", value);
				this._replication = value;
			}
		}

		/// <summary>Gets or sets a string value that indicates how the connection maintains its association with an enlisted <see langword="System.Transactions" /> transaction.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.TransactionBinding" /> property, or <see langword="String.Empty" /> if none has been supplied.</returns>
		public string TransactionBinding
		{
			get
			{
				return this._transactionBinding;
			}
			set
			{
				this.SetValue("Transaction Binding", value);
				this._transactionBinding = value;
			}
		}

		/// <summary>Gets or sets a string value that indicates the type system the application expects.</summary>
		/// <returns>The following table shows the possible values for the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.TypeSystemVersion" /> property:  
		///   Value  
		///
		///   Description  
		///
		///   SQL Server 2005  
		///
		///   Uses the SQL Server 2005 type system. No conversions are made for the current version of ADO.NET.  
		///
		///   SQL Server 2008  
		///
		///   Uses the SQL Server 2008 type system.  
		///
		///   Latest  
		///
		///   Use the latest version than this client-server pair can handle. This will automatically move forward as the client and server components are upgraded.</returns>
		public string TypeSystemVersion
		{
			get
			{
				return this._typeSystemVersion;
			}
			set
			{
				this.SetValue("Type System Version", value);
				this._typeSystemVersion = value;
			}
		}

		/// <summary>Gets or sets the user ID to be used when connecting to SQL Server.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.UserID" /> property, or <see langword="String.Empty" /> if none has been supplied.</returns>
		/// <exception cref="T:System.ArgumentNullException">To set the value to null, use <see cref="F:System.DBNull.Value" />.</exception>
		public string UserID
		{
			get
			{
				return this._userID;
			}
			set
			{
				this.SetValue("User ID", value);
				this._userID = value;
			}
		}

		/// <summary>Gets or sets a value that indicates whether to redirect the connection from the default SQL Server Express instance to a runtime-initiated instance running under the account of the caller.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.UserInstance" /> property, or <see langword="False" /> if none has been supplied.</returns>
		/// <exception cref="T:System.ArgumentNullException">To set the value to null, use <see cref="F:System.DBNull.Value" />.</exception>
		public bool UserInstance
		{
			get
			{
				return this._userInstance;
			}
			set
			{
				this.SetValue("User Instance", value);
				this._userInstance = value;
			}
		}

		/// <summary>Gets or sets the name of the workstation connecting to SQL Server.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.WorkstationID" /> property, or <see langword="String.Empty" /> if none has been supplied.</returns>
		/// <exception cref="T:System.ArgumentNullException">To set the value to null, use <see cref="F:System.DBNull.Value" />.</exception>
		public string WorkstationID
		{
			get
			{
				return this._workstationID;
			}
			set
			{
				this.SetValue("Workstation ID", value);
				this._workstationID = value;
			}
		}

		/// <summary>Gets an <see cref="T:System.Collections.ICollection" /> that contains the keys in the <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> that contains the keys in the <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" />.</returns>
		public override ICollection Keys
		{
			get
			{
				return new ReadOnlyCollection<string>(SqlConnectionStringBuilder.s_validKeywords);
			}
		}

		/// <summary>Gets an <see cref="T:System.Collections.ICollection" /> that contains the values in the <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" />.</summary>
		/// <returns>An <see cref="T:System.Collections.ICollection" /> that contains the values in the <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" />.</returns>
		public override ICollection Values
		{
			get
			{
				object[] array = new object[SqlConnectionStringBuilder.s_validKeywords.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this.GetAt((SqlConnectionStringBuilder.Keywords)i);
				}
				return new ReadOnlyCollection<object>(array);
			}
		}

		/// <summary>Clears the contents of the <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" /> instance.</summary>
		public override void Clear()
		{
			base.Clear();
			for (int i = 0; i < SqlConnectionStringBuilder.s_validKeywords.Length; i++)
			{
				this.Reset((SqlConnectionStringBuilder.Keywords)i);
			}
		}

		/// <summary>Determines whether the <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" /> contains a specific key.</summary>
		/// <param name="keyword">The key to locate in the <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" />.</param>
		/// <returns>true if the <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" /> contains an element that has the specified key; otherwise, false.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> is null (<see langword="Nothing" /> in Visual Basic)</exception>
		public override bool ContainsKey(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			return SqlConnectionStringBuilder.s_keywords.ContainsKey(keyword);
		}

		private static bool ConvertToBoolean(object value)
		{
			return DbConnectionStringBuilderUtil.ConvertToBoolean(value);
		}

		private static int ConvertToInt32(object value)
		{
			return DbConnectionStringBuilderUtil.ConvertToInt32(value);
		}

		private static bool ConvertToIntegratedSecurity(object value)
		{
			return DbConnectionStringBuilderUtil.ConvertToIntegratedSecurity(value);
		}

		private static string ConvertToString(object value)
		{
			return DbConnectionStringBuilderUtil.ConvertToString(value);
		}

		private static ApplicationIntent ConvertToApplicationIntent(string keyword, object value)
		{
			return DbConnectionStringBuilderUtil.ConvertToApplicationIntent(keyword, value);
		}

		private object GetAt(SqlConnectionStringBuilder.Keywords index)
		{
			switch (index)
			{
			case SqlConnectionStringBuilder.Keywords.DataSource:
				return this.DataSource;
			case SqlConnectionStringBuilder.Keywords.FailoverPartner:
				return this.FailoverPartner;
			case SqlConnectionStringBuilder.Keywords.AttachDBFilename:
				return this.AttachDBFilename;
			case SqlConnectionStringBuilder.Keywords.InitialCatalog:
				return this.InitialCatalog;
			case SqlConnectionStringBuilder.Keywords.IntegratedSecurity:
				return this.IntegratedSecurity;
			case SqlConnectionStringBuilder.Keywords.PersistSecurityInfo:
				return this.PersistSecurityInfo;
			case SqlConnectionStringBuilder.Keywords.UserID:
				return this.UserID;
			case SqlConnectionStringBuilder.Keywords.Password:
				return this.Password;
			case SqlConnectionStringBuilder.Keywords.Enlist:
				return this.Enlist;
			case SqlConnectionStringBuilder.Keywords.Pooling:
				return this.Pooling;
			case SqlConnectionStringBuilder.Keywords.MinPoolSize:
				return this.MinPoolSize;
			case SqlConnectionStringBuilder.Keywords.MaxPoolSize:
				return this.MaxPoolSize;
			case SqlConnectionStringBuilder.Keywords.MultipleActiveResultSets:
				return this.MultipleActiveResultSets;
			case SqlConnectionStringBuilder.Keywords.Replication:
				return this.Replication;
			case SqlConnectionStringBuilder.Keywords.ConnectTimeout:
				return this.ConnectTimeout;
			case SqlConnectionStringBuilder.Keywords.Encrypt:
				return this.Encrypt;
			case SqlConnectionStringBuilder.Keywords.TrustServerCertificate:
				return this.TrustServerCertificate;
			case SqlConnectionStringBuilder.Keywords.LoadBalanceTimeout:
				return this.LoadBalanceTimeout;
			case SqlConnectionStringBuilder.Keywords.PacketSize:
				return this.PacketSize;
			case SqlConnectionStringBuilder.Keywords.TypeSystemVersion:
				return this.TypeSystemVersion;
			case SqlConnectionStringBuilder.Keywords.ApplicationName:
				return this.ApplicationName;
			case SqlConnectionStringBuilder.Keywords.CurrentLanguage:
				return this.CurrentLanguage;
			case SqlConnectionStringBuilder.Keywords.WorkstationID:
				return this.WorkstationID;
			case SqlConnectionStringBuilder.Keywords.UserInstance:
				return this.UserInstance;
			case SqlConnectionStringBuilder.Keywords.TransactionBinding:
				return this.TransactionBinding;
			case SqlConnectionStringBuilder.Keywords.ApplicationIntent:
				return this.ApplicationIntent;
			case SqlConnectionStringBuilder.Keywords.MultiSubnetFailover:
				return this.MultiSubnetFailover;
			case SqlConnectionStringBuilder.Keywords.ConnectRetryCount:
				return this.ConnectRetryCount;
			case SqlConnectionStringBuilder.Keywords.ConnectRetryInterval:
				return this.ConnectRetryInterval;
			default:
				throw this.UnsupportedKeyword(SqlConnectionStringBuilder.s_validKeywords[(int)index]);
			}
		}

		private SqlConnectionStringBuilder.Keywords GetIndex(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			SqlConnectionStringBuilder.Keywords result;
			if (SqlConnectionStringBuilder.s_keywords.TryGetValue(keyword, out result))
			{
				return result;
			}
			throw this.UnsupportedKeyword(keyword);
		}

		/// <summary>Removes the entry with the specified key from the <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" /> instance.</summary>
		/// <param name="keyword">The key of the key/value pair to be removed from the connection string in this <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" />.</param>
		/// <returns>
		///   <see langword="true" /> if the key existed within the connection string and was removed; <see langword="false" /> if the key did not exist.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> is null (<see langword="Nothing" /> in Visual Basic)</exception>
		public override bool Remove(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			SqlConnectionStringBuilder.Keywords keywords;
			if (SqlConnectionStringBuilder.s_keywords.TryGetValue(keyword, out keywords) && base.Remove(SqlConnectionStringBuilder.s_validKeywords[(int)keywords]))
			{
				this.Reset(keywords);
				return true;
			}
			return false;
		}

		private void Reset(SqlConnectionStringBuilder.Keywords index)
		{
			switch (index)
			{
			case SqlConnectionStringBuilder.Keywords.DataSource:
				this._dataSource = "";
				return;
			case SqlConnectionStringBuilder.Keywords.FailoverPartner:
				this._failoverPartner = "";
				return;
			case SqlConnectionStringBuilder.Keywords.AttachDBFilename:
				this._attachDBFilename = "";
				return;
			case SqlConnectionStringBuilder.Keywords.InitialCatalog:
				this._initialCatalog = "";
				return;
			case SqlConnectionStringBuilder.Keywords.IntegratedSecurity:
				this._integratedSecurity = false;
				return;
			case SqlConnectionStringBuilder.Keywords.PersistSecurityInfo:
				this._persistSecurityInfo = false;
				return;
			case SqlConnectionStringBuilder.Keywords.UserID:
				this._userID = "";
				return;
			case SqlConnectionStringBuilder.Keywords.Password:
				this._password = "";
				return;
			case SqlConnectionStringBuilder.Keywords.Enlist:
				this._enlist = true;
				return;
			case SqlConnectionStringBuilder.Keywords.Pooling:
				this._pooling = true;
				return;
			case SqlConnectionStringBuilder.Keywords.MinPoolSize:
				this._minPoolSize = 0;
				return;
			case SqlConnectionStringBuilder.Keywords.MaxPoolSize:
				this._maxPoolSize = 100;
				return;
			case SqlConnectionStringBuilder.Keywords.MultipleActiveResultSets:
				this._multipleActiveResultSets = false;
				return;
			case SqlConnectionStringBuilder.Keywords.Replication:
				this._replication = false;
				return;
			case SqlConnectionStringBuilder.Keywords.ConnectTimeout:
				this._connectTimeout = 15;
				return;
			case SqlConnectionStringBuilder.Keywords.Encrypt:
				this._encrypt = false;
				return;
			case SqlConnectionStringBuilder.Keywords.TrustServerCertificate:
				this._trustServerCertificate = false;
				return;
			case SqlConnectionStringBuilder.Keywords.LoadBalanceTimeout:
				this._loadBalanceTimeout = 0;
				return;
			case SqlConnectionStringBuilder.Keywords.PacketSize:
				this._packetSize = 8000;
				return;
			case SqlConnectionStringBuilder.Keywords.TypeSystemVersion:
				this._typeSystemVersion = "Latest";
				return;
			case SqlConnectionStringBuilder.Keywords.ApplicationName:
				this._applicationName = "Core .Net SqlClient Data Provider";
				return;
			case SqlConnectionStringBuilder.Keywords.CurrentLanguage:
				this._currentLanguage = "";
				return;
			case SqlConnectionStringBuilder.Keywords.WorkstationID:
				this._workstationID = "";
				return;
			case SqlConnectionStringBuilder.Keywords.UserInstance:
				this._userInstance = false;
				return;
			case SqlConnectionStringBuilder.Keywords.TransactionBinding:
				this._transactionBinding = "Implicit Unbind";
				return;
			case SqlConnectionStringBuilder.Keywords.ApplicationIntent:
				this._applicationIntent = ApplicationIntent.ReadWrite;
				return;
			case SqlConnectionStringBuilder.Keywords.MultiSubnetFailover:
				this._multiSubnetFailover = false;
				return;
			case SqlConnectionStringBuilder.Keywords.ConnectRetryCount:
				this._connectRetryCount = 1;
				return;
			case SqlConnectionStringBuilder.Keywords.ConnectRetryInterval:
				this._connectRetryInterval = 10;
				return;
			default:
				throw this.UnsupportedKeyword(SqlConnectionStringBuilder.s_validKeywords[(int)index]);
			}
		}

		private void SetValue(string keyword, bool value)
		{
			base[keyword] = value.ToString();
		}

		private void SetValue(string keyword, int value)
		{
			base[keyword] = value.ToString(null);
		}

		private void SetValue(string keyword, string value)
		{
			ADP.CheckArgumentNull(value, keyword);
			base[keyword] = value;
		}

		private void SetApplicationIntentValue(ApplicationIntent value)
		{
			base["ApplicationIntent"] = DbConnectionStringBuilderUtil.ApplicationIntentToString(value);
		}

		/// <summary>Indicates whether the specified key exists in this <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" /> instance.</summary>
		/// <param name="keyword">The key to locate in the <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" />.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" /> contains an entry with the specified key; otherwise, <see langword="false" />.</returns>
		public override bool ShouldSerialize(string keyword)
		{
			ADP.CheckArgumentNull(keyword, "keyword");
			SqlConnectionStringBuilder.Keywords keywords;
			return SqlConnectionStringBuilder.s_keywords.TryGetValue(keyword, out keywords) && base.ShouldSerialize(SqlConnectionStringBuilder.s_validKeywords[(int)keywords]);
		}

		/// <summary>Retrieves a value corresponding to the supplied key from this <see cref="T:System.Data.SqlClient.SqlConnectionStringBuilder" />.</summary>
		/// <param name="keyword">The key of the item to retrieve.</param>
		/// <param name="value">The value corresponding to <paramref name="keyword" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="keyword" /> was found within the connection string; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="keyword" /> contains a null value (<see langword="Nothing" /> in Visual Basic).</exception>
		public override bool TryGetValue(string keyword, out object value)
		{
			SqlConnectionStringBuilder.Keywords index;
			if (SqlConnectionStringBuilder.s_keywords.TryGetValue(keyword, out index))
			{
				value = this.GetAt(index);
				return true;
			}
			value = null;
			return false;
		}

		private Exception UnsupportedKeyword(string keyword)
		{
			if (SqlConnectionStringBuilder.s_notSupportedKeywords.Contains(keyword, StringComparer.OrdinalIgnoreCase))
			{
				return SQL.UnsupportedKeyword(keyword);
			}
			if (SqlConnectionStringBuilder.s_notSupportedNetworkLibraryKeywords.Contains(keyword, StringComparer.OrdinalIgnoreCase))
			{
				return SQL.NetworkLibraryKeywordNotSupported();
			}
			return ADP.KeywordNotSupported(keyword);
		}

		/// <summary>Gets or sets a Boolean value that indicates whether asynchronous processing is allowed by the connection created by using this connection string.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.AsynchronousProcessing" /> property, or <see langword="false" /> if no value has been supplied.</returns>
		[Obsolete("This property is ignored beginning in .NET Framework 4.5.For more information about SqlClient support for asynchronous programming, seehttps://docs.microsoft.com/en-us/dotnet/framework/data/adonet/asynchronous-programming")]
		public bool AsynchronousProcessing { get; set; }

		/// <summary>Obsolete. Gets or sets a Boolean value that indicates whether the connection is reset when drawn from the connection pool.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.ConnectionReset" /> property, or true if no value has been supplied.</returns>
		[Obsolete("ConnectionReset has been deprecated.  SqlConnection will ignore the 'connection reset'keyword and always reset the connection")]
		public bool ConnectionReset { get; set; }

		/// <summary>Gets the authentication of the connection string.</summary>
		/// <returns>The authentication of the connection string.</returns>
		[MonoTODO("Not implemented in corefx: https://github.com/dotnet/corefx/issues/22474")]
		public SqlAuthenticationMethod Authentication
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

		/// <summary>Gets or sets a value that indicates whether a client/server or in-process connection to SQL Server should be made.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.ContextConnection" /> property, or <see langword="False" /> if none has been supplied.</returns>
		[MonoTODO("Not implemented in corefx: https://github.com/dotnet/corefx/issues/22474")]
		public bool ContextConnection
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

		/// <summary>Gets or sets a string that contains the name of the network library used to establish a connection to the SQL Server.</summary>
		/// <returns>The value of the <see cref="P:System.Data.SqlClient.SqlConnectionStringBuilder.NetworkLibrary" /> property, or <see langword="String.Empty" /> if none has been supplied.</returns>
		/// <exception cref="T:System.ArgumentNullException">To set the value to null, use <see cref="F:System.DBNull.Value" />.</exception>
		[MonoTODO("Not implemented in corefx: https://github.com/dotnet/corefx/issues/22474")]
		public string NetworkLibrary
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

		/// <summary>The blocking period behavior for a connection pool.</summary>
		/// <returns>The available blocking period settings.</returns>
		[MonoTODO("Not implemented in corefx: https://github.com/dotnet/corefx/issues/22474")]
		public PoolBlockingPeriod PoolBlockingPeriod
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

		/// <summary>When the value of this key is set to <see langword="true" />, the application is required to retrieve all IP addresses for a particular DNS entry and attempt to connect with the first one in the list. If the connection is not established within 0.5 seconds, the application will try to connect to all others in parallel. When the first answers, the application will establish the connection with the respondent IP address.</summary>
		/// <returns>A boolean value.</returns>
		[MonoTODO("Not implemented in corefx: https://github.com/dotnet/corefx/issues/22474")]
		public bool TransparentNetworkIPResolution
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

		/// <summary>Gets or sets the column encryption settings for the connection string builder.</summary>
		/// <returns>The column encryption settings for the connection string builder.</returns>
		[MonoTODO("Not implemented in corefx: https://github.com/dotnet/corefx/issues/22474")]
		public SqlConnectionColumnEncryptionSetting ColumnEncryptionSetting
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

		/// <summary>Gets or sets the enclave attestation Url to be used with enclave based Always Encrypted.</summary>
		/// <returns>The enclave attestation Url.</returns>
		public string EnclaveAttestationUrl
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		internal const int KeywordsCount = 29;

		internal const int DeprecatedKeywordsCount = 4;

		private static readonly string[] s_validKeywords = SqlConnectionStringBuilder.CreateValidKeywords();

		private static readonly Dictionary<string, SqlConnectionStringBuilder.Keywords> s_keywords = SqlConnectionStringBuilder.CreateKeywordsDictionary();

		private ApplicationIntent _applicationIntent;

		private string _applicationName = "Core .Net SqlClient Data Provider";

		private string _attachDBFilename = "";

		private string _currentLanguage = "";

		private string _dataSource = "";

		private string _failoverPartner = "";

		private string _initialCatalog = "";

		private string _password = "";

		private string _transactionBinding = "Implicit Unbind";

		private string _typeSystemVersion = "Latest";

		private string _userID = "";

		private string _workstationID = "";

		private int _connectTimeout = 15;

		private int _loadBalanceTimeout;

		private int _maxPoolSize = 100;

		private int _minPoolSize;

		private int _packetSize = 8000;

		private int _connectRetryCount = 1;

		private int _connectRetryInterval = 10;

		private bool _encrypt;

		private bool _trustServerCertificate;

		private bool _enlist = true;

		private bool _integratedSecurity;

		private bool _multipleActiveResultSets;

		private bool _multiSubnetFailover;

		private bool _persistSecurityInfo;

		private bool _pooling = true;

		private bool _replication;

		private bool _userInstance;

		private static readonly string[] s_notSupportedKeywords = new string[]
		{
			"Asynchronous Processing",
			"Connection Reset",
			"Context Connection",
			"Transaction Binding",
			"async"
		};

		private static readonly string[] s_notSupportedNetworkLibraryKeywords = new string[]
		{
			"Network Library",
			"net",
			"network"
		};

		private enum Keywords
		{
			DataSource,
			FailoverPartner,
			AttachDBFilename,
			InitialCatalog,
			IntegratedSecurity,
			PersistSecurityInfo,
			UserID,
			Password,
			Enlist,
			Pooling,
			MinPoolSize,
			MaxPoolSize,
			MultipleActiveResultSets,
			Replication,
			ConnectTimeout,
			Encrypt,
			TrustServerCertificate,
			LoadBalanceTimeout,
			PacketSize,
			TypeSystemVersion,
			ApplicationName,
			CurrentLanguage,
			WorkstationID,
			UserInstance,
			TransactionBinding,
			ApplicationIntent,
			MultiSubnetFailover,
			ConnectRetryCount,
			ConnectRetryInterval,
			KeywordsCount
		}

		private sealed class SqlInitialCatalogConverter : StringConverter
		{
			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				return this.GetStandardValuesSupportedInternal(context);
			}

			private bool GetStandardValuesSupportedInternal(ITypeDescriptorContext context)
			{
				bool result = false;
				if (context != null)
				{
					SqlConnectionStringBuilder sqlConnectionStringBuilder = context.Instance as SqlConnectionStringBuilder;
					if (sqlConnectionStringBuilder != null && 0 < sqlConnectionStringBuilder.DataSource.Length && (sqlConnectionStringBuilder.IntegratedSecurity || 0 < sqlConnectionStringBuilder.UserID.Length))
					{
						result = true;
					}
				}
				return result;
			}

			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
			{
				return false;
			}

			public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			{
				if (this.GetStandardValuesSupportedInternal(context))
				{
					List<string> list = new List<string>();
					try
					{
						SqlConnectionStringBuilder sqlConnectionStringBuilder = (SqlConnectionStringBuilder)context.Instance;
						using (SqlConnection sqlConnection = new SqlConnection())
						{
							sqlConnection.ConnectionString = sqlConnectionStringBuilder.ConnectionString;
							sqlConnection.Open();
							foreach (object obj in sqlConnection.GetSchema("DATABASES").Rows)
							{
								string item = (string)((DataRow)obj)["database_name"];
								list.Add(item);
							}
						}
					}
					catch (SqlException e)
					{
						ADP.TraceExceptionWithoutRethrow(e);
					}
					return new TypeConverter.StandardValuesCollection(list);
				}
				return null;
			}
		}
	}
}
