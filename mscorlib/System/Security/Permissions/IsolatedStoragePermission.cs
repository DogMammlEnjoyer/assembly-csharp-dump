using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	/// <summary>Represents access to generic isolated storage capabilities.</summary>
	[ComVisible(true)]
	[SecurityPermission(SecurityAction.InheritanceDemand, ControlEvidence = true, ControlPolicy = true)]
	[Serializable]
	public abstract class IsolatedStoragePermission : CodeAccessPermission, IUnrestrictedPermission
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Permissions.IsolatedStoragePermission" /> class with either restricted or unrestricted permission as specified.</summary>
		/// <param name="state">One of the <see cref="T:System.Security.Permissions.PermissionState" /> values.</param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="state" /> parameter is not a valid value of <see cref="T:System.Security.Permissions.PermissionState" />.</exception>
		protected IsolatedStoragePermission(PermissionState state)
		{
			if (CodeAccessPermission.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				this.UsageAllowed = IsolatedStorageContainment.UnrestrictedIsolatedStorage;
			}
		}

		/// <summary>Gets or sets the quota on the overall size of each user's total store.</summary>
		/// <returns>The size, in bytes, of the resource allocated to the user.</returns>
		public long UserQuota
		{
			get
			{
				return this.m_userQuota;
			}
			set
			{
				this.m_userQuota = value;
			}
		}

		/// <summary>Gets or sets the type of isolated storage containment allowed.</summary>
		/// <returns>One of the <see cref="T:System.Security.Permissions.IsolatedStorageContainment" /> values.</returns>
		public IsolatedStorageContainment UsageAllowed
		{
			get
			{
				return this.m_allowed;
			}
			set
			{
				if (!Enum.IsDefined(typeof(IsolatedStorageContainment), value))
				{
					throw new ArgumentException(string.Format(Locale.GetText("Invalid enum {0}"), value), "IsolatedStorageContainment");
				}
				this.m_allowed = value;
				if (this.m_allowed == IsolatedStorageContainment.UnrestrictedIsolatedStorage)
				{
					this.m_userQuota = long.MaxValue;
					this.m_machineQuota = long.MaxValue;
					this.m_expirationDays = long.MaxValue;
					this.m_permanentData = true;
				}
			}
		}

		/// <summary>Returns a value indicating whether the current permission is unrestricted.</summary>
		/// <returns>
		///   <see langword="true" /> if the current permission is unrestricted; otherwise, <see langword="false" />.</returns>
		public bool IsUnrestricted()
		{
			return IsolatedStorageContainment.UnrestrictedIsolatedStorage == this.m_allowed;
		}

		/// <summary>Creates an XML encoding of the permission and its current state.</summary>
		/// <returns>An XML encoding of the permission, including any state information.</returns>
		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = base.Element(1);
			if (this.m_allowed == IsolatedStorageContainment.UnrestrictedIsolatedStorage)
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			else
			{
				securityElement.AddAttribute("Allowed", this.m_allowed.ToString());
				if (this.m_userQuota > 0L)
				{
					securityElement.AddAttribute("UserQuota", this.m_userQuota.ToString());
				}
			}
			return securityElement;
		}

		/// <summary>Reconstructs a permission with a specified state from an XML encoding.</summary>
		/// <param name="esd">The XML encoding to use to reconstruct the permission.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="esd" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="esd" /> parameter is not a valid permission element.  
		///  -or-  
		///  The <paramref name="esd" /> parameter's version number is not valid.</exception>
		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			this.m_userQuota = 0L;
			this.m_machineQuota = 0L;
			this.m_expirationDays = 0L;
			this.m_permanentData = false;
			this.m_allowed = IsolatedStorageContainment.None;
			if (CodeAccessPermission.IsUnrestricted(esd))
			{
				this.UsageAllowed = IsolatedStorageContainment.UnrestrictedIsolatedStorage;
				return;
			}
			string text = esd.Attribute("Allowed");
			if (text != null)
			{
				this.UsageAllowed = (IsolatedStorageContainment)Enum.Parse(typeof(IsolatedStorageContainment), text);
			}
			text = esd.Attribute("UserQuota");
			if (text != null)
			{
				this.m_userQuota = long.Parse(text, CultureInfo.InvariantCulture);
			}
		}

		internal bool IsEmpty()
		{
			return this.m_userQuota == 0L && this.m_allowed == IsolatedStorageContainment.None;
		}

		private const int version = 1;

		internal long m_userQuota;

		internal long m_machineQuota;

		internal long m_expirationDays;

		internal bool m_permanentData;

		internal IsolatedStorageContainment m_allowed;
	}
}
