using System;

namespace System.Security.Permissions
{
	/// <summary>Allows security actions for <see cref="T:System.Security.Permissions.DataProtectionPermission" /> to be applied to code using declarative security. This class cannot be inherited.</summary>
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class DataProtectionPermissionAttribute : CodeAccessSecurityAttribute
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Permissions.DataProtectionPermissionAttribute" /> class with the specified <see cref="T:System.Security.Permissions.SecurityAction" />.</summary>
		/// <param name="action">One of the <see cref="T:System.Security.Permissions.SecurityAction" /> values.</param>
		public DataProtectionPermissionAttribute(SecurityAction action) : base(action)
		{
		}

		/// <summary>Gets or sets the data protection permissions.</summary>
		/// <returns>A bitwise combination of the <see cref="T:System.Security.Permissions.DataProtectionPermissionFlags" /> values. The default is <see cref="F:System.Security.Permissions.DataProtectionPermissionFlags.NoFlags" />.</returns>
		public DataProtectionPermissionFlags Flags
		{
			get
			{
				return this._flags;
			}
			set
			{
				if ((value & DataProtectionPermissionFlags.AllFlags) != value)
				{
					throw new ArgumentException(string.Format(Locale.GetText("Invalid flags {0}"), value), "DataProtectionPermissionFlags");
				}
				this._flags = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether data can be encrypted using the <see cref="T:System.Security.Cryptography.ProtectedData" /> class.</summary>
		/// <returns>
		///   <see langword="true" /> if data can be encrypted; otherwise, <see langword="false" />.</returns>
		public bool ProtectData
		{
			get
			{
				return (this._flags & DataProtectionPermissionFlags.ProtectData) > DataProtectionPermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= DataProtectionPermissionFlags.ProtectData;
					return;
				}
				this._flags &= ~DataProtectionPermissionFlags.ProtectData;
			}
		}

		/// <summary>Gets or sets a value indicating whether data can be unencrypted using the <see cref="T:System.Security.Cryptography.ProtectedData" /> class.</summary>
		/// <returns>
		///   <see langword="true" /> if data can be unencrypted; otherwise, <see langword="false" />.</returns>
		public bool UnprotectData
		{
			get
			{
				return (this._flags & DataProtectionPermissionFlags.UnprotectData) > DataProtectionPermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= DataProtectionPermissionFlags.UnprotectData;
					return;
				}
				this._flags &= ~DataProtectionPermissionFlags.UnprotectData;
			}
		}

		/// <summary>Gets or sets a value indicating whether memory can be encrypted using the <see cref="T:System.Security.Cryptography.ProtectedMemory" /> class.</summary>
		/// <returns>
		///   <see langword="true" /> if memory can be encrypted; otherwise, <see langword="false" />.</returns>
		public bool ProtectMemory
		{
			get
			{
				return (this._flags & DataProtectionPermissionFlags.ProtectMemory) > DataProtectionPermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= DataProtectionPermissionFlags.ProtectMemory;
					return;
				}
				this._flags &= ~DataProtectionPermissionFlags.ProtectMemory;
			}
		}

		/// <summary>Gets or sets a value indicating whether memory can be unencrypted using the <see cref="T:System.Security.Cryptography.ProtectedMemory" /> class.</summary>
		/// <returns>
		///   <see langword="true" /> if memory can be unencrypted; otherwise, <see langword="false" />.</returns>
		public bool UnprotectMemory
		{
			get
			{
				return (this._flags & DataProtectionPermissionFlags.UnprotectMemory) > DataProtectionPermissionFlags.NoFlags;
			}
			set
			{
				if (value)
				{
					this._flags |= DataProtectionPermissionFlags.UnprotectMemory;
					return;
				}
				this._flags &= ~DataProtectionPermissionFlags.UnprotectMemory;
			}
		}

		/// <summary>Creates and returns a new <see cref="T:System.Security.Permissions.DataProtectionPermission" />.</summary>
		/// <returns>A <see cref="T:System.Security.Permissions.DataProtectionPermission" /> that corresponds to the attribute.</returns>
		public override IPermission CreatePermission()
		{
			DataProtectionPermission result;
			if (base.Unrestricted)
			{
				result = new DataProtectionPermission(PermissionState.Unrestricted);
			}
			else
			{
				result = new DataProtectionPermission(this._flags);
			}
			return result;
		}

		private DataProtectionPermissionFlags _flags;
	}
}
