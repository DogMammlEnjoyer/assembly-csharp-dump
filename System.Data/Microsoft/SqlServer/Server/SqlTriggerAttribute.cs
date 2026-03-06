using System;

namespace Microsoft.SqlServer.Server
{
	/// <summary>Used to mark a method definition in an assembly as a trigger in SQL Server. The properties on the attribute reflect the physical attributes used when the type is registered with SQL Server. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	[Serializable]
	public sealed class SqlTriggerAttribute : Attribute
	{
		/// <summary>An attribute on a method definition in an assembly, used to mark the method as a trigger in SQL Server.</summary>
		public SqlTriggerAttribute()
		{
			this.m_fName = null;
			this.m_fTarget = null;
			this.m_fEvent = null;
		}

		/// <summary>The name of the trigger.</summary>
		/// <returns>A <see cref="T:System.String" /> value representing the name of the trigger.</returns>
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

		/// <summary>The table to which the trigger applies.</summary>
		/// <returns>A <see cref="T:System.String" /> value representing the table name.</returns>
		public string Target
		{
			get
			{
				return this.m_fTarget;
			}
			set
			{
				this.m_fTarget = value;
			}
		}

		/// <summary>The type of trigger and what data manipulation language (DML) action activates the trigger.</summary>
		/// <returns>A <see cref="T:System.String" /> value representing the type of trigger and what data manipulation language (DML) action activates the trigger.</returns>
		public string Event
		{
			get
			{
				return this.m_fEvent;
			}
			set
			{
				this.m_fEvent = value;
			}
		}

		private string m_fName;

		private string m_fTarget;

		private string m_fEvent;
	}
}
