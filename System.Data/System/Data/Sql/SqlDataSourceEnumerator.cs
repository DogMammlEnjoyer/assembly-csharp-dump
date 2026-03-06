using System;
using System.Data.Common;
using System.Globalization;

namespace System.Data.Sql
{
	/// <summary>Provides a mechanism for enumerating all available instances of SQL Server within the local network.</summary>
	public sealed class SqlDataSourceEnumerator : DbDataSourceEnumerator
	{
		private SqlDataSourceEnumerator()
		{
		}

		/// <summary>Gets an instance of the <see cref="T:System.Data.Sql.SqlDataSourceEnumerator" />, which can be used to retrieve information about available SQL Server instances.</summary>
		/// <returns>An instance of the <see cref="T:System.Data.Sql.SqlDataSourceEnumerator" /> used to retrieve information about available SQL Server instances.</returns>
		public static SqlDataSourceEnumerator Instance
		{
			get
			{
				return SqlDataSourceEnumerator.SingletonInstance;
			}
		}

		/// <summary>Retrieves a <see cref="T:System.Data.DataTable" /> containing information about all visible SQL Server 2000 or SQL Server 2005 instances.</summary>
		/// <returns>A <see cref="T:System.Data.DataTable" /> containing information about the visible SQL Server instances.</returns>
		public override DataTable GetDataSources()
		{
			this.timeoutTime = 0L;
			throw new NotImplementedException();
		}

		private static DataTable ParseServerEnumString(string serverInstances)
		{
			DataTable dataTable = new DataTable("SqlDataSources");
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("ServerName", typeof(string));
			dataTable.Columns.Add("InstanceName", typeof(string));
			dataTable.Columns.Add("IsClustered", typeof(string));
			dataTable.Columns.Add("Version", typeof(string));
			string text = null;
			string text2 = null;
			string text3 = null;
			string value = null;
			string[] array = serverInstances.Split('\0', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text4 = array[i].Trim('\0');
				if (text4.Length != 0)
				{
					foreach (string text5 in text4.Split(';', StringSplitOptions.None))
					{
						if (text == null)
						{
							foreach (string text6 in text5.Split('\\', StringSplitOptions.None))
							{
								if (text == null)
								{
									text = text6;
								}
								else
								{
									text2 = text6;
								}
							}
						}
						else if (text3 == null)
						{
							text3 = text5.Substring(SqlDataSourceEnumerator._clusterLength);
						}
						else
						{
							value = text5.Substring(SqlDataSourceEnumerator._versionLength);
						}
					}
					string text7 = "ServerName='" + text + "'";
					if (!ADP.IsEmpty(text2))
					{
						text7 = text7 + " AND InstanceName='" + text2 + "'";
					}
					if (dataTable.Select(text7).Length == 0)
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow[0] = text;
						dataRow[1] = text2;
						dataRow[2] = text3;
						dataRow[3] = value;
						dataTable.Rows.Add(dataRow);
					}
					text = null;
					text2 = null;
					text3 = null;
					value = null;
				}
			}
			foreach (object obj in dataTable.Columns)
			{
				((DataColumn)obj).ReadOnly = true;
			}
			return dataTable;
		}

		private static readonly SqlDataSourceEnumerator SingletonInstance = new SqlDataSourceEnumerator();

		internal const string ServerName = "ServerName";

		internal const string InstanceName = "InstanceName";

		internal const string IsClustered = "IsClustered";

		internal const string Version = "Version";

		private long timeoutTime;

		private static string _Version = "Version:";

		private static string _Cluster = "Clustered:";

		private static int _clusterLength = SqlDataSourceEnumerator._Cluster.Length;

		private static int _versionLength = SqlDataSourceEnumerator._Version.Length;
	}
}
