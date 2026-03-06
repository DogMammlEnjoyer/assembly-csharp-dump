using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Odbc
{
	/// <summary>Associates a security action with a custom security attribute.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class OdbcPermissionAttribute : DBDataPermissionAttribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Data.Odbc.OdbcPermissionAttribute" /> class with one of the <see cref="T:System.Security.Permissions.SecurityAction" /> values.</summary>
		/// <param name="action">One of the <see cref="T:System.Security.Permissions.SecurityAction" /> values representing an action that can be performed by using declarative security.</param>
		public OdbcPermissionAttribute(SecurityAction action) : base(action)
		{
		}

		/// <summary>Returns an <see cref="T:System.Data.Odbc.OdbcPermission" /> object that is configured according to the attribute properties.</summary>
		/// <returns>An <see cref="T:System.Data.Odbc.OdbcPermission" /> object.</returns>
		public override IPermission CreatePermission()
		{
			return new OdbcPermission(this);
		}
	}
}
