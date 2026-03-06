using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	/// <summary>Specifies the allowed usage of a private virtual file system. This class cannot be inherited.</summary>
	[ComVisible(true)]
	[Serializable]
	public sealed class IsolatedStorageFilePermission : IsolatedStoragePermission, IBuiltInPermission
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Permissions.IsolatedStorageFilePermission" /> class with either fully restricted or unrestricted permission as specified.</summary>
		/// <param name="state">One of the <see cref="T:System.Security.Permissions.PermissionState" /> values.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="state" /> parameter is not a valid value of <see cref="T:System.Security.Permissions.PermissionState" />.</exception>
		public IsolatedStorageFilePermission(PermissionState state) : base(state)
		{
		}

		/// <summary>Creates and returns an identical copy of the current permission.</summary>
		/// <returns>A copy of the current permission.</returns>
		public override IPermission Copy()
		{
			return new IsolatedStorageFilePermission(PermissionState.None)
			{
				m_userQuota = this.m_userQuota,
				m_machineQuota = this.m_machineQuota,
				m_expirationDays = this.m_expirationDays,
				m_permanentData = this.m_permanentData,
				m_allowed = this.m_allowed
			};
		}

		/// <summary>Creates and returns a permission that is the intersection of the current permission and the specified permission.</summary>
		/// <param name="target">A permission to intersect with the current permission object. It must be of the same type as the current permission.</param>
		/// <returns>A new permission that represents the intersection of the current permission and the specified permission. This new permission is <see langword="null" /> if the intersection is empty.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="target" /> parameter is not <see langword="null" /> and is not of the same type as the current permission.</exception>
		public override IPermission Intersect(IPermission target)
		{
			IsolatedStorageFilePermission isolatedStorageFilePermission = this.Cast(target);
			if (isolatedStorageFilePermission == null)
			{
				return null;
			}
			if (base.IsEmpty() && isolatedStorageFilePermission.IsEmpty())
			{
				return null;
			}
			return new IsolatedStorageFilePermission(PermissionState.None)
			{
				m_userQuota = ((this.m_userQuota < isolatedStorageFilePermission.m_userQuota) ? this.m_userQuota : isolatedStorageFilePermission.m_userQuota),
				m_machineQuota = ((this.m_machineQuota < isolatedStorageFilePermission.m_machineQuota) ? this.m_machineQuota : isolatedStorageFilePermission.m_machineQuota),
				m_expirationDays = ((this.m_expirationDays < isolatedStorageFilePermission.m_expirationDays) ? this.m_expirationDays : isolatedStorageFilePermission.m_expirationDays),
				m_permanentData = (this.m_permanentData && isolatedStorageFilePermission.m_permanentData),
				UsageAllowed = ((this.m_allowed < isolatedStorageFilePermission.m_allowed) ? this.m_allowed : isolatedStorageFilePermission.m_allowed)
			};
		}

		/// <summary>Determines whether the current permission is a subset of the specified permission.</summary>
		/// <param name="target">A permission that is to be tested for the subset relationship. This permission must be of the same type as the current permission.</param>
		/// <returns>
		///   <see langword="true" /> if the current permission is a subset of the specified permission; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="target" /> parameter is not <see langword="null" /> and is not of the same type as the current permission.</exception>
		public override bool IsSubsetOf(IPermission target)
		{
			IsolatedStorageFilePermission isolatedStorageFilePermission = this.Cast(target);
			if (isolatedStorageFilePermission == null)
			{
				return base.IsEmpty();
			}
			return isolatedStorageFilePermission.IsUnrestricted() || (this.m_userQuota <= isolatedStorageFilePermission.m_userQuota && this.m_machineQuota <= isolatedStorageFilePermission.m_machineQuota && this.m_expirationDays <= isolatedStorageFilePermission.m_expirationDays && this.m_permanentData == isolatedStorageFilePermission.m_permanentData && this.m_allowed <= isolatedStorageFilePermission.m_allowed);
		}

		/// <summary>Creates a permission that is the union of the current permission and the specified permission.</summary>
		/// <param name="target">A permission to combine with the current permission. It must be of the same type as the current permission.</param>
		/// <returns>A new permission that represents the union of the current permission and the specified permission.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="target" /> parameter is not <see langword="null" /> and is not of the same type as the current permission.</exception>
		public override IPermission Union(IPermission target)
		{
			IsolatedStorageFilePermission isolatedStorageFilePermission = this.Cast(target);
			if (isolatedStorageFilePermission == null)
			{
				return this.Copy();
			}
			return new IsolatedStorageFilePermission(PermissionState.None)
			{
				m_userQuota = ((this.m_userQuota > isolatedStorageFilePermission.m_userQuota) ? this.m_userQuota : isolatedStorageFilePermission.m_userQuota),
				m_machineQuota = ((this.m_machineQuota > isolatedStorageFilePermission.m_machineQuota) ? this.m_machineQuota : isolatedStorageFilePermission.m_machineQuota),
				m_expirationDays = ((this.m_expirationDays > isolatedStorageFilePermission.m_expirationDays) ? this.m_expirationDays : isolatedStorageFilePermission.m_expirationDays),
				m_permanentData = (this.m_permanentData || isolatedStorageFilePermission.m_permanentData),
				UsageAllowed = ((this.m_allowed > isolatedStorageFilePermission.m_allowed) ? this.m_allowed : isolatedStorageFilePermission.m_allowed)
			};
		}

		/// <summary>Creates an XML encoding of the permission and its current state.</summary>
		/// <returns>An XML encoding of the permission, including any state information.</returns>
		[MonoTODO("(2.0) new override - something must have been added ???")]
		[ComVisible(false)]
		public override SecurityElement ToXml()
		{
			return base.ToXml();
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 3;
		}

		private IsolatedStorageFilePermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			IsolatedStorageFilePermission isolatedStorageFilePermission = target as IsolatedStorageFilePermission;
			if (isolatedStorageFilePermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(IsolatedStorageFilePermission));
			}
			return isolatedStorageFilePermission;
		}
	}
}
