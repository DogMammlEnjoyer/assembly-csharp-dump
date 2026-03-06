using System;

namespace System.Security.Permissions
{
	/// <summary>Defines the smallest unit of a code access security permission set.</summary>
	[Serializable]
	public class ResourcePermissionBaseEntry
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Permissions.ResourcePermissionBaseEntry" /> class.</summary>
		public ResourcePermissionBaseEntry()
		{
			this.permissionAccessPath = new string[0];
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Permissions.ResourcePermissionBaseEntry" /> class with the specified permission access and permission access path.</summary>
		/// <param name="permissionAccess">The integer representation of the permission access level enumeration value. The <see cref="P:System.Security.Permissions.ResourcePermissionBaseEntry.PermissionAccess" /> property is set to this value.</param>
		/// <param name="permissionAccessPath">The array of strings that identify the resource you are protecting. The <see cref="P:System.Security.Permissions.ResourcePermissionBaseEntry.PermissionAccessPath" /> property is set to this value.</param>
		/// <exception cref="T:System.ArgumentNullException">The specified <paramref name="permissionAccessPath" /> is <see langword="null" />.</exception>
		public ResourcePermissionBaseEntry(int permissionAccess, string[] permissionAccessPath)
		{
			if (permissionAccessPath == null)
			{
				throw new ArgumentNullException("permissionAccessPath");
			}
			this.permissionAccess = permissionAccess;
			this.permissionAccessPath = permissionAccessPath;
		}

		/// <summary>Gets an integer representation of the access level enumeration value.</summary>
		/// <returns>The access level enumeration value.</returns>
		public int PermissionAccess
		{
			get
			{
				return this.permissionAccess;
			}
		}

		/// <summary>Gets an array of strings that identify the resource you are protecting.</summary>
		/// <returns>An array of strings that identify the resource you are protecting.</returns>
		public string[] PermissionAccessPath
		{
			get
			{
				return this.permissionAccessPath;
			}
		}

		private int permissionAccess;

		private string[] permissionAccessPath;
	}
}
