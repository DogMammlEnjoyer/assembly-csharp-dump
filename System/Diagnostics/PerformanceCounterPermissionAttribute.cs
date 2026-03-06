using System;
using System.Security;
using System.Security.Permissions;

namespace System.Diagnostics
{
	/// <summary>Allows declaritive performance counter permission checks.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public class PerformanceCounterPermissionAttribute : CodeAccessSecurityAttribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Diagnostics.PerformanceCounterPermissionAttribute" /> class.</summary>
		/// <param name="action">One of the <see cref="T:System.Security.Permissions.SecurityAction" /> values.</param>
		public PerformanceCounterPermissionAttribute(SecurityAction action) : base(action)
		{
			this.categoryName = "*";
			this.machineName = ".";
			this.permissionAccess = PerformanceCounterPermissionAccess.Write;
		}

		/// <summary>Gets or sets the name of the performance counter category.</summary>
		/// <returns>The name of the performance counter category (performance object).</returns>
		/// <exception cref="T:System.ArgumentNullException">The value is <see langword="null" />.</exception>
		public string CategoryName
		{
			get
			{
				return this.categoryName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("CategoryName");
				}
				this.categoryName = value;
			}
		}

		/// <summary>Gets or sets the computer name for the performance counter.</summary>
		/// <returns>The server on which the category of the performance counter resides.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.Diagnostics.PerformanceCounterPermissionAttribute.MachineName" /> format is invalid.</exception>
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
		/// <returns>A bitwise combination of the <see cref="T:System.Diagnostics.PerformanceCounterPermissionAccess" /> values. The default is <see cref="F:System.Diagnostics.EventLogPermissionAccess.Write" />.</returns>
		public PerformanceCounterPermissionAccess PermissionAccess
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

		/// <summary>Creates the permission based on the requested access levels that are set through the <see cref="P:System.Diagnostics.PerformanceCounterPermissionAttribute.PermissionAccess" /> property on the attribute.</summary>
		/// <returns>An <see cref="T:System.Security.IPermission" /> that represents the created permission.</returns>
		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new PerformanceCounterPermission(PermissionState.Unrestricted);
			}
			return new PerformanceCounterPermission(this.permissionAccess, this.machineName, this.categoryName);
		}

		private string categoryName;

		private string machineName;

		private PerformanceCounterPermissionAccess permissionAccess;
	}
}
