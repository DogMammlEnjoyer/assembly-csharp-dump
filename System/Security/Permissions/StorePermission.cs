using System;

namespace System.Security.Permissions
{
	/// <summary>Controls access to stores containing X.509 certificates. This class cannot be inherited.</summary>
	[Serializable]
	public sealed class StorePermission : CodeAccessPermission, IUnrestrictedPermission
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Permissions.StorePermission" /> class with either fully restricted or unrestricted permission state.</summary>
		/// <param name="state">One of the <see cref="T:System.Security.Permissions.PermissionState" /> values.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="state" /> is not a valid <see cref="T:System.Security.Permissions.PermissionState" /> value.</exception>
		public StorePermission(PermissionState state)
		{
			if (PermissionHelper.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				this._flags = StorePermissionFlags.AllFlags;
				return;
			}
			this._flags = StorePermissionFlags.NoFlags;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Permissions.StorePermission" /> class with the specified access.</summary>
		/// <param name="flag">A bitwise combination of the <see cref="T:System.Security.Permissions.StorePermissionFlags" /> values.</param>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="flag" /> is not a valid combination of <see cref="T:System.Security.Permissions.StorePermissionFlags" /> values.</exception>
		public StorePermission(StorePermissionFlags flag)
		{
			this.Flags = flag;
		}

		/// <summary>Gets or sets the type of <see cref="T:System.Security.Cryptography.X509Certificates.X509Store" /> access allowed by the current permission.</summary>
		/// <returns>A bitwise combination of the <see cref="T:System.Security.Permissions.StorePermissionFlags" /> values.</returns>
		/// <exception cref="T:System.ArgumentException">An attempt is made to set this property to an invalid value. See <see cref="T:System.Security.Permissions.StorePermissionFlags" /> for the valid values.</exception>
		public StorePermissionFlags Flags
		{
			get
			{
				return this._flags;
			}
			set
			{
				if (value != StorePermissionFlags.NoFlags && (value & StorePermissionFlags.AllFlags) == StorePermissionFlags.NoFlags)
				{
					throw new ArgumentException(string.Format(Locale.GetText("Invalid enum {0}"), value), "StorePermissionFlags");
				}
				this._flags = value;
			}
		}

		/// <summary>Returns a value indicating whether the current permission is unrestricted.</summary>
		/// <returns>
		///   <see langword="true" /> if the current permission is unrestricted; otherwise, <see langword="false" />.</returns>
		public bool IsUnrestricted()
		{
			return this._flags == StorePermissionFlags.AllFlags;
		}

		/// <summary>Creates and returns an identical copy of the current permission.</summary>
		/// <returns>A copy of the current permission.</returns>
		public override IPermission Copy()
		{
			if (this._flags == StorePermissionFlags.NoFlags)
			{
				return null;
			}
			return new StorePermission(this._flags);
		}

		/// <summary>Creates and returns a permission that is the intersection of the current permission and the specified permission.</summary>
		/// <param name="target">A permission to intersect with the current permission. It must be of the same type as the current permission.</param>
		/// <returns>A new permission that represents the intersection of the current permission and the specified permission. This new permission is <see langword="null" /> if the intersection is empty.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="target" /> s not <see langword="null" /> and is not of the same type as the current permission.</exception>
		public override IPermission Intersect(IPermission target)
		{
			StorePermission storePermission = this.Cast(target);
			if (storePermission == null)
			{
				return null;
			}
			if (this.IsUnrestricted() && storePermission.IsUnrestricted())
			{
				return new StorePermission(PermissionState.Unrestricted);
			}
			if (this.IsUnrestricted())
			{
				return storePermission.Copy();
			}
			if (storePermission.IsUnrestricted())
			{
				return this.Copy();
			}
			StorePermissionFlags storePermissionFlags = this._flags & storePermission._flags;
			if (storePermissionFlags == StorePermissionFlags.NoFlags)
			{
				return null;
			}
			return new StorePermission(storePermissionFlags);
		}

		/// <summary>Creates a permission that is the union of the current permission and the specified permission.</summary>
		/// <param name="target">A permission to combine with the current permission. It must be of the same type as the current permission.</param>
		/// <returns>A new permission that represents the union of the current permission and the specified permission.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="target" /> is not <see langword="null" /> and is not of the same type as the current permission.</exception>
		public override IPermission Union(IPermission target)
		{
			StorePermission storePermission = this.Cast(target);
			if (storePermission == null)
			{
				return this.Copy();
			}
			if (this.IsUnrestricted() || storePermission.IsUnrestricted())
			{
				return new StorePermission(PermissionState.Unrestricted);
			}
			StorePermissionFlags storePermissionFlags = this._flags | storePermission._flags;
			if (storePermissionFlags == StorePermissionFlags.NoFlags)
			{
				return null;
			}
			return new StorePermission(storePermissionFlags);
		}

		/// <summary>Determines whether the current permission is a subset of the specified permission.</summary>
		/// <param name="target">A permission to test for the subset relationship. This permission must be of the same type as the current permission.</param>
		/// <returns>
		///   <see langword="true" /> if the current permission is a subset of the specified permission; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="target" /> is not <see langword="null" /> and is not of the same type as the current permission.</exception>
		public override bool IsSubsetOf(IPermission target)
		{
			StorePermission storePermission = this.Cast(target);
			if (storePermission == null)
			{
				return this._flags == StorePermissionFlags.NoFlags;
			}
			return storePermission.IsUnrestricted() || (!this.IsUnrestricted() && (this._flags & ~storePermission._flags) == StorePermissionFlags.NoFlags);
		}

		/// <summary>Reconstructs a permission with a specified state from an XML encoding.</summary>
		/// <param name="securityElement">A <see cref="T:System.Security.SecurityElement" /> that contains the XML encoding to use to reconstruct the permission.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="securityElement" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="securityElement" /> is not a valid permission element.  
		/// -or-  
		/// The version number in <paramref name="securityElement" /> is not valid.</exception>
		public override void FromXml(SecurityElement securityElement)
		{
			PermissionHelper.CheckSecurityElement(securityElement, "securityElement", 1, 1);
			string text = securityElement.Attribute("Flags");
			if (text == null)
			{
				this._flags = StorePermissionFlags.NoFlags;
				return;
			}
			this._flags = (StorePermissionFlags)Enum.Parse(typeof(StorePermissionFlags), text);
		}

		/// <summary>Creates an XML encoding of the permission and its current state.</summary>
		/// <returns>A <see cref="T:System.Security.SecurityElement" /> that contains an XML encoding of the permission, including any state information.</returns>
		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = PermissionHelper.Element(typeof(StorePermission), 1);
			if (this.IsUnrestricted())
			{
				securityElement.AddAttribute("Unrestricted", bool.TrueString);
			}
			else
			{
				securityElement.AddAttribute("Flags", this._flags.ToString());
			}
			return securityElement;
		}

		private StorePermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			StorePermission storePermission = target as StorePermission;
			if (storePermission == null)
			{
				PermissionHelper.ThrowInvalidPermission(target, typeof(StorePermission));
			}
			return storePermission;
		}

		private const int version = 1;

		private StorePermissionFlags _flags;
	}
}
