using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.SqlClient
{
	/// <summary>Associates a security action with a custom security attribute.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class SqlClientPermissionAttribute : DBDataPermissionAttribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.SqlClient.SqlClientPermissionAttribute" /> class.</summary>
		/// <param name="action">One of the <see cref="T:System.Security.Permissions.SecurityAction" /> values representing an action that can be performed by using declarative security.</param>
		public SqlClientPermissionAttribute(SecurityAction action) : base(action)
		{
		}

		/// <summary>Returns a <see cref="T:System.Data.SqlClient.SqlClientPermission" /> object that is configured according to the attribute properties.</summary>
		/// <returns>A <see cref="T:System.Data.SqlClient.SqlClientPermission" /> object.</returns>
		public override IPermission CreatePermission()
		{
			return new SqlClientPermission(this);
		}
	}
}
