using System;
using System.Security.Permissions;

namespace System.Diagnostics
{
	/// <summary>Defines the smallest unit of a code access security permission that is set for an <see cref="T:System.Diagnostics.EventLog" />.</summary>
	[Serializable]
	public class EventLogPermissionEntry
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.EventLogPermissionEntry" /> class.</summary>
		/// <param name="permissionAccess">A bitwise combination of the <see cref="T:System.Diagnostics.EventLogPermissionAccess" /> values. The <see cref="P:System.Diagnostics.EventLogPermissionEntry.PermissionAccess" /> property is set to this value.</param>
		/// <param name="machineName">The name of the computer on which to read or write events. The <see cref="P:System.Diagnostics.EventLogPermissionEntry.MachineName" /> property is set to this value.</param>
		/// <exception cref="T:System.ArgumentException">The computer name is invalid.</exception>
		public EventLogPermissionEntry(EventLogPermissionAccess permissionAccess, string machineName)
		{
			ResourcePermissionBase.ValidateMachineName(machineName);
			this.permissionAccess = permissionAccess;
			this.machineName = machineName;
		}

		/// <summary>Gets the name of the computer on which to read or write events.</summary>
		/// <returns>The name of the computer on which to read or write events.</returns>
		public string MachineName
		{
			get
			{
				return this.machineName;
			}
		}

		/// <summary>Gets the permission access levels used in the permissions request.</summary>
		/// <returns>A bitwise combination of the <see cref="T:System.Diagnostics.EventLogPermissionAccess" /> values.</returns>
		public EventLogPermissionAccess PermissionAccess
		{
			get
			{
				return this.permissionAccess;
			}
		}

		internal ResourcePermissionBaseEntry CreateResourcePermissionBaseEntry()
		{
			return new ResourcePermissionBaseEntry((int)this.permissionAccess, new string[]
			{
				this.machineName
			});
		}

		private EventLogPermissionAccess permissionAccess;

		private string machineName;
	}
}
