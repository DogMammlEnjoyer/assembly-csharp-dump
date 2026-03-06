using System;

namespace Microsoft.SqlServer.Server
{
	/// <summary>Used to mark a method definition of a user-defined aggregate as a function in SQL Server. The properties on the attribute reflect the physical characteristics used when the type is registered with SQL Server.</summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[Serializable]
	public class SqlFunctionAttribute : Attribute
	{
		/// <summary>An optional attribute on a user-defined aggregate, used to indicate that the method should be registered in SQL Server as a function. Also used to set the <see cref="P:Microsoft.SqlServer.Server.SqlFunctionAttribute.DataAccess" />, <see cref="P:Microsoft.SqlServer.Server.SqlFunctionAttribute.FillRowMethodName" />, <see cref="P:Microsoft.SqlServer.Server.SqlFunctionAttribute.IsDeterministic" />, <see cref="P:Microsoft.SqlServer.Server.SqlFunctionAttribute.IsPrecise" />, <see cref="P:Microsoft.SqlServer.Server.SqlFunctionAttribute.Name" />, <see cref="P:Microsoft.SqlServer.Server.SqlFunctionAttribute.SystemDataAccess" />, and <see cref="P:Microsoft.SqlServer.Server.SqlFunctionAttribute.TableDefinition" /> properties of the function attribute.</summary>
		public SqlFunctionAttribute()
		{
			this.m_fDeterministic = false;
			this.m_eDataAccess = DataAccessKind.None;
			this.m_eSystemDataAccess = SystemDataAccessKind.None;
			this.m_fPrecise = false;
			this.m_fName = null;
			this.m_fTableDefinition = null;
			this.m_FillRowMethodName = null;
		}

		/// <summary>Indicates whether the user-defined function is deterministic.</summary>
		/// <returns>
		///   <see langword="true" /> if the function is deterministic; otherwise <see langword="false" />.</returns>
		public bool IsDeterministic
		{
			get
			{
				return this.m_fDeterministic;
			}
			set
			{
				this.m_fDeterministic = value;
			}
		}

		/// <summary>Indicates whether the function involves access to user data stored in the local instance of SQL Server.</summary>
		/// <returns>
		///   <see cref="T:Microsoft.SqlServer.Server.DataAccessKind" />.<see langword="None" />: Does not access data. <see cref="T:Microsoft.SqlServer.Server.DataAccessKind" />.<see langword="Read" />: Only reads user data.</returns>
		public DataAccessKind DataAccess
		{
			get
			{
				return this.m_eDataAccess;
			}
			set
			{
				this.m_eDataAccess = value;
			}
		}

		/// <summary>Indicates whether the function requires access to data stored in the system catalogs or virtual system tables of SQL Server.</summary>
		/// <returns>
		///   <see cref="T:Microsoft.SqlServer.Server.DataAccessKind" />.<see langword="None" />: Does not access system data. <see cref="T:Microsoft.SqlServer.Server.DataAccessKind" />.<see langword="Read" />: Only reads system data.</returns>
		public SystemDataAccessKind SystemDataAccess
		{
			get
			{
				return this.m_eSystemDataAccess;
			}
			set
			{
				this.m_eSystemDataAccess = value;
			}
		}

		/// <summary>Indicates whether the function involves imprecise computations, such as floating point operations.</summary>
		/// <returns>
		///   <see langword="true" /> if the function involves precise computations; otherwise <see langword="false" />.</returns>
		public bool IsPrecise
		{
			get
			{
				return this.m_fPrecise;
			}
			set
			{
				this.m_fPrecise = value;
			}
		}

		/// <summary>The name under which the function should be registered in SQL Server.</summary>
		/// <returns>A <see cref="T:System.String" /> value representing the name under which the function should be registered.</returns>
		public string Name
		{
			get
			{
				return this.m_fName;
			}
			set
			{
				this.m_fName = value;
			}
		}

		/// <summary>A string that represents the table definition of the results, if the method is used as a table-valued function (TVF).</summary>
		/// <returns>A <see cref="T:System.String" /> value representing the table definition of the results.</returns>
		public string TableDefinition
		{
			get
			{
				return this.m_fTableDefinition;
			}
			set
			{
				this.m_fTableDefinition = value;
			}
		}

		/// <summary>The name of a method in the same class which is used to fill a row of data in the table returned by the table-valued function.</summary>
		/// <returns>A <see cref="T:System.String" /> value representing the name of a method in the same class which is used to fill a row of data in the table returned by the table-valued function.</returns>
		public string FillRowMethodName
		{
			get
			{
				return this.m_FillRowMethodName;
			}
			set
			{
				this.m_FillRowMethodName = value;
			}
		}

		private bool m_fDeterministic;

		private DataAccessKind m_eDataAccess;

		private SystemDataAccessKind m_eSystemDataAccess;

		private bool m_fPrecise;

		private string m_fName;

		private string m_fTableDefinition;

		private string m_FillRowMethodName;
	}
}
