using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	/// <summary>Configures a role for an application or component. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true)]
	[ComVisible(false)]
	public sealed class SecurityRoleAttribute : Attribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.SecurityRoleAttribute" /> class and sets the <see cref="P:System.EnterpriseServices.SecurityRoleAttribute.Role" /> property.</summary>
		/// <param name="role">A security role for the application, component, interface, or method.</param>
		public SecurityRoleAttribute(string role) : this(role, false)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.SecurityRoleAttribute" /> class and sets the <see cref="P:System.EnterpriseServices.SecurityRoleAttribute.Role" /> and <see cref="P:System.EnterpriseServices.SecurityRoleAttribute.SetEveryoneAccess" /> properties.</summary>
		/// <param name="role">A security role for the application, component, interface, or method.</param>
		/// <param name="everyone">
		///   <see langword="true" /> to require that the newly created role have the Everyone user group added as a user; otherwise, <see langword="false" />.</param>
		public SecurityRoleAttribute(string role, bool everyone)
		{
			this.description = string.Empty;
			this.everyone = everyone;
			this.role = role;
		}

		/// <summary>Gets or sets the role description.</summary>
		/// <returns>The description for the role.</returns>
		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		/// <summary>Gets or sets the security role.</summary>
		/// <returns>The security role applied to an application, component, interface, or method.</returns>
		public string Role
		{
			get
			{
				return this.role;
			}
			set
			{
				this.role = value;
			}
		}

		/// <summary>Sets a value indicating whether to add the Everyone user group as a user.</summary>
		/// <returns>
		///   <see langword="true" /> to require that a newly created role have the Everyone user group added as a user (roles that already exist on the application are not modified); otherwise, <see langword="false" /> to suppress adding the Everyone user group as a user.</returns>
		public bool SetEveryoneAccess
		{
			get
			{
				return this.everyone;
			}
			set
			{
				this.everyone = value;
			}
		}

		private string description;

		private bool everyone;

		private string role;
	}
}
