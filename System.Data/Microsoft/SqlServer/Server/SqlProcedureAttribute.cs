using System;

namespace Microsoft.SqlServer.Server
{
	/// <summary>Used to mark a method definition in an assembly as a stored procedure. The properties on the attribute reflect the physical characteristics used when the type is registered with SQL Server. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[Serializable]
	public sealed class SqlProcedureAttribute : Attribute
	{
		/// <summary>An attribute on a method definition in an assembly, used to indicate that the given method should be registered as a stored procedure in SQL Server.</summary>
		public SqlProcedureAttribute()
		{
			this.m_fName = null;
		}

		/// <summary>The name of the stored procedure.</summary>
		/// <returns>A <see cref="T:System.String" /> representing the name of the stored procedure.</returns>
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

		private string m_fName;
	}
}
