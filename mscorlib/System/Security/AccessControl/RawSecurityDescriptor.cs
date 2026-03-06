using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	/// <summary>Represents a security descriptor. A security descriptor includes an owner, a primary group, a Discretionary Access Control List (DACL), and a System Access Control List (SACL).</summary>
	public sealed class RawSecurityDescriptor : GenericSecurityDescriptor
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> class from the specified Security Descriptor Definition Language (SDDL) string.</summary>
		/// <param name="sddlForm">The SDDL string from which to create the new <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</param>
		public RawSecurityDescriptor(string sddlForm)
		{
			if (sddlForm == null)
			{
				throw new ArgumentNullException("sddlForm");
			}
			this.ParseSddl(sddlForm.Replace(" ", ""));
			this.control_flags |= ControlFlags.SelfRelative;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> class from the specified array of byte values.</summary>
		/// <param name="binaryForm">The array of byte values from which to create the new <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</param>
		/// <param name="offset">The offset in the  <paramref name="binaryForm" /> array at which to begin copying.</param>
		public RawSecurityDescriptor(byte[] binaryForm, int offset)
		{
			if (binaryForm == null)
			{
				throw new ArgumentNullException("binaryForm");
			}
			if (offset < 0 || offset > binaryForm.Length - 20)
			{
				throw new ArgumentOutOfRangeException("offset", offset, "Offset out of range");
			}
			if (binaryForm[offset] != 1)
			{
				throw new ArgumentException("Unrecognized Security Descriptor revision.", "binaryForm");
			}
			this.resourcemgr_control = binaryForm[offset + 1];
			this.control_flags = (ControlFlags)this.ReadUShort(binaryForm, offset + 2);
			int num = this.ReadInt(binaryForm, offset + 4);
			int num2 = this.ReadInt(binaryForm, offset + 8);
			int num3 = this.ReadInt(binaryForm, offset + 12);
			int num4 = this.ReadInt(binaryForm, offset + 16);
			if (num != 0)
			{
				this.owner_sid = new SecurityIdentifier(binaryForm, num);
			}
			if (num2 != 0)
			{
				this.group_sid = new SecurityIdentifier(binaryForm, num2);
			}
			if (num3 != 0)
			{
				this.system_acl = new RawAcl(binaryForm, num3);
			}
			if (num4 != 0)
			{
				this.discretionary_acl = new RawAcl(binaryForm, num4);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> class with the specified values.</summary>
		/// <param name="flags">Flags that specify behavior of the new <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</param>
		/// <param name="owner">The owner for the new <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</param>
		/// <param name="group">The primary group for the new <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</param>
		/// <param name="systemAcl">The System Access Control List (SACL) for the new <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</param>
		/// <param name="discretionaryAcl">The Discretionary Access Control List (DACL) for the new <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</param>
		public RawSecurityDescriptor(ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, RawAcl systemAcl, RawAcl discretionaryAcl)
		{
			this.control_flags = flags;
			this.owner_sid = owner;
			this.group_sid = group;
			this.system_acl = systemAcl;
			this.discretionary_acl = discretionaryAcl;
		}

		/// <summary>Gets values that specify behavior of the <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</summary>
		/// <returns>One or more values of the <see cref="T:System.Security.AccessControl.ControlFlags" /> enumeration combined with a logical OR operation.</returns>
		public override ControlFlags ControlFlags
		{
			get
			{
				return this.control_flags;
			}
		}

		/// <summary>Gets or sets the Discretionary Access Control List (DACL) for this <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object. The DACL contains access rules.</summary>
		/// <returns>The DACL for this <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</returns>
		public RawAcl DiscretionaryAcl
		{
			get
			{
				return this.discretionary_acl;
			}
			set
			{
				this.discretionary_acl = value;
			}
		}

		/// <summary>Gets or sets the primary group for this <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</summary>
		/// <returns>The primary group for this <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</returns>
		public override SecurityIdentifier Group
		{
			get
			{
				return this.group_sid;
			}
			set
			{
				this.group_sid = value;
			}
		}

		/// <summary>Gets or sets the owner of the object associated with this <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</summary>
		/// <returns>The owner of the object associated with this <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</returns>
		public override SecurityIdentifier Owner
		{
			get
			{
				return this.owner_sid;
			}
			set
			{
				this.owner_sid = value;
			}
		}

		/// <summary>Gets or sets a byte value that represents the resource manager control bits associated with this <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</summary>
		/// <returns>A byte value that represents the resource manager control bits associated with this <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</returns>
		public byte ResourceManagerControl
		{
			get
			{
				return this.resourcemgr_control;
			}
			set
			{
				this.resourcemgr_control = value;
			}
		}

		/// <summary>Gets or sets the System Access Control List (SACL) for this <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object. The SACL contains audit rules.</summary>
		/// <returns>The SACL for this <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</returns>
		public RawAcl SystemAcl
		{
			get
			{
				return this.system_acl;
			}
			set
			{
				this.system_acl = value;
			}
		}

		/// <summary>Sets the <see cref="P:System.Security.AccessControl.RawSecurityDescriptor.ControlFlags" /> property of this <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object to the specified value.</summary>
		/// <param name="flags">One or more values of the <see cref="T:System.Security.AccessControl.ControlFlags" /> enumeration combined with a logical OR operation.</param>
		public void SetFlags(ControlFlags flags)
		{
			this.control_flags = (flags | ControlFlags.SelfRelative);
		}

		internal override GenericAcl InternalDacl
		{
			get
			{
				return this.DiscretionaryAcl;
			}
		}

		internal override GenericAcl InternalSacl
		{
			get
			{
				return this.SystemAcl;
			}
		}

		internal override byte InternalReservedField
		{
			get
			{
				return this.ResourceManagerControl;
			}
		}

		private void ParseSddl(string sddlForm)
		{
			ControlFlags controlFlags = ControlFlags.None;
			int i = 0;
			while (i < sddlForm.Length - 2)
			{
				string a = sddlForm.Substring(i, 2);
				if (!(a == "O:"))
				{
					if (!(a == "G:"))
					{
						if (!(a == "D:"))
						{
							if (!(a == "S:"))
							{
								throw new ArgumentException("Invalid SDDL.", "sddlForm");
							}
							i += 2;
							this.SystemAcl = RawAcl.ParseSddlForm(sddlForm, false, ref controlFlags, ref i);
							controlFlags |= ControlFlags.SystemAclPresent;
						}
						else
						{
							i += 2;
							this.DiscretionaryAcl = RawAcl.ParseSddlForm(sddlForm, true, ref controlFlags, ref i);
							controlFlags |= ControlFlags.DiscretionaryAclPresent;
						}
					}
					else
					{
						i += 2;
						this.Group = SecurityIdentifier.ParseSddlForm(sddlForm, ref i);
					}
				}
				else
				{
					i += 2;
					this.Owner = SecurityIdentifier.ParseSddlForm(sddlForm, ref i);
				}
			}
			if (i != sddlForm.Length)
			{
				throw new ArgumentException("Invalid SDDL.", "sddlForm");
			}
			this.SetFlags(controlFlags);
		}

		private ushort ReadUShort(byte[] buffer, int offset)
		{
			return (ushort)((int)buffer[offset] | (int)buffer[offset + 1] << 8);
		}

		private int ReadInt(byte[] buffer, int offset)
		{
			return (int)buffer[offset] | (int)buffer[offset + 1] << 8 | (int)buffer[offset + 2] << 16 | (int)buffer[offset + 3] << 24;
		}

		private ControlFlags control_flags;

		private SecurityIdentifier owner_sid;

		private SecurityIdentifier group_sid;

		private RawAcl system_acl;

		private RawAcl discretionary_acl;

		private byte resourcemgr_control;
	}
}
