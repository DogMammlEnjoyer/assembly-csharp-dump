using System;
using System.Security;
using System.Security.Permissions;

namespace System.Diagnostics
{
	/// <summary>Allows declaritive permission checks for event logging.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public class EventLogPermissionAttribute : CodeAccessSecurityAttribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.EventLogPermissionAttribute" /> class.</summary>
		/// <param name="action">One of the <see cref="T:System.Security.Permissions.SecurityAction" /> values.</param>
		public EventLogPermissionAttribute(SecurityAction action) : base(action)
		{
			this.machineName = ".";
			this.permissionAccess = EventLogPermissionAccess.Write;
		}

		/// <summary>Gets or sets the name of the computer on which events might be read.</summary>
		/// <returns>The name of the computer on which events might be read. The default is ".".</returns>
		/// <exception cref="T:System.ArgumentException">The computer name is invalid.</exception>
		public string MachineName
		{
			get
			{
				return this.machineName;
			}
			set
			{
				ResourcePermissionBase.ValidateMachineName(value);
				this.machineName = value;
			}
		}

		/// <summary>Gets or sets the access levels used in the permissions request.</summary>
		/// <returns>A bitwise combination of the <see cref="T:System.Diagnostics.EventLogPermissionAccess" /> values. The default is <see cref="F:System.Diagnostics.EventLogPermissionAccess.Write" />.</returns>
		public EventLogPermissionAccess PermissionAccess
		{
			get
			{
				return this.permissionAccess;
			}
			set
			{
				this.permissionAccess = value;
			}
		}

		/// <summary>Creates the permission based on the <see cref="P:System.Diagnostics.EventLogPermissionAttribute.MachineName" /> property and the requested access levels that are set through the <see cref="P:System.Diagnostics.EventLogPermissionAttribute.PermissionAccess" /> property on the attribute.</summary>
		/// <returns>An <see cref="T:System.Security.IPermission" /> that represents the created permission.</returns>
		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new EventLogPermission(PermissionState.Unrestricted);
			}
			return new EventLogPermission(this.permissionAccess, this.machineName);
		}

		private string machineName;

		private EventLogPermissionAccess permissionAccess;
	}
}
