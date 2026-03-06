using System;
using System.Security;
using System.Security.Permissions;

namespace System.Net.NetworkInformation
{
	/// <summary>Allows security actions for <see cref="T:System.Net.NetworkInformation.NetworkInformationPermission" /> to be applied to code using declarative security.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class NetworkInformationPermissionAttribute : CodeAccessSecurityAttribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Net.NetworkInformation.NetworkInformationPermissionAttribute" /> class.</summary>
		/// <param name="action">A <see cref="T:System.Security.Permissions.SecurityAction" /> value that specifies the permission behavior.</param>
		public NetworkInformationPermissionAttribute(SecurityAction action) : base(action)
		{
		}

		/// <summary>Gets or sets the network information access level.</summary>
		/// <returns>A string that specifies the access level.</returns>
		public string Access
		{
			get
			{
				return this.access;
			}
			set
			{
				this.access = value;
			}
		}

		/// <summary>Creates and returns a new <see cref="T:System.Net.NetworkInformation.NetworkInformationPermission" /> object.</summary>
		/// <returns>A <see cref="T:System.Net.NetworkInformation.NetworkInformationPermission" /> that corresponds to this attribute.</returns>
		public override IPermission CreatePermission()
		{
			NetworkInformationPermission networkInformationPermission;
			if (base.Unrestricted)
			{
				networkInformationPermission = new NetworkInformationPermission(PermissionState.Unrestricted);
			}
			else
			{
				networkInformationPermission = new NetworkInformationPermission(PermissionState.None);
				if (this.access != null)
				{
					if (string.Compare(this.access, "Read", StringComparison.OrdinalIgnoreCase) == 0)
					{
						networkInformationPermission.AddPermission(NetworkInformationAccess.Read);
					}
					else if (string.Compare(this.access, "Ping", StringComparison.OrdinalIgnoreCase) == 0)
					{
						networkInformationPermission.AddPermission(NetworkInformationAccess.Ping);
					}
					else
					{
						if (string.Compare(this.access, "None", StringComparison.OrdinalIgnoreCase) != 0)
						{
							throw new ArgumentException(SR.GetString("The parameter value '{0}={1}' is invalid.", new object[]
							{
								"Access",
								this.access
							}));
						}
						networkInformationPermission.AddPermission(NetworkInformationAccess.None);
					}
				}
			}
			return networkInformationPermission;
		}

		private const string strAccess = "Access";

		private string access;
	}
}
