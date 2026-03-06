using System;
using System.Security;
using System.Security.Permissions;

namespace System.Drawing.Printing
{
	/// <summary>Allows declarative printing permission checks.</summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	[Serializable]
	public sealed class PrintingPermissionAttribute : CodeAccessSecurityAttribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.PrintingPermissionAttribute" /> class.</summary>
		/// <param name="action">One of the <see cref="T:System.Security.Permissions.SecurityAction" /> values.</param>
		public PrintingPermissionAttribute(SecurityAction action) : base(action)
		{
		}

		/// <summary>Gets or sets the type of printing allowed.</summary>
		/// <returns>One of the <see cref="T:System.Drawing.Printing.PrintingPermissionLevel" /> values.</returns>
		/// <exception cref="T:System.ArgumentException">The value is not one of the <see cref="T:System.Drawing.Printing.PrintingPermissionLevel" /> values.</exception>
		public PrintingPermissionLevel Level { get; set; }

		/// <summary>Creates the permission based on the requested access levels, which are set through the <see cref="P:System.Drawing.Printing.PrintingPermissionAttribute.Level" /> property on the attribute.</summary>
		/// <returns>An <see cref="T:System.Security.IPermission" /> that represents the created permission.</returns>
		public override IPermission CreatePermission()
		{
			return null;
		}
	}
}
