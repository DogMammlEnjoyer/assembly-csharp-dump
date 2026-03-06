using System;
using System.Globalization;
using System.Security.Principal;
using System.Text;

namespace System.Security.AccessControl
{
	/// <summary>Represents a security descriptor. A security descriptor includes an owner, a primary group, a Discretionary Access Control List (DACL), and a System Access Control List (SACL).</summary>
	public abstract class GenericSecurityDescriptor
	{
		/// <summary>Gets the length, in bytes, of the binary representation of the current <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> object. This length should be used before marshaling the ACL into a binary array with the <see cref="M:System.Security.AccessControl.GenericSecurityDescriptor.GetBinaryForm(System.Byte[],System.Int32)" /> method.</summary>
		/// <returns>The length, in bytes, of the binary representation of the current <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> object.</returns>
		public int BinaryLength
		{
			get
			{
				int num = 20;
				if (this.Owner != null)
				{
					num += this.Owner.BinaryLength;
				}
				if (this.Group != null)
				{
					num += this.Group.BinaryLength;
				}
				if (this.DaclPresent && !this.DaclIsUnmodifiedAefa)
				{
					num += this.InternalDacl.BinaryLength;
				}
				if (this.SaclPresent)
				{
					num += this.InternalSacl.BinaryLength;
				}
				return num;
			}
		}

		/// <summary>Gets values that specify behavior of the <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> object.</summary>
		/// <returns>One or more values of the <see cref="T:System.Security.AccessControl.ControlFlags" /> enumeration combined with a logical OR operation.</returns>
		public abstract ControlFlags ControlFlags { get; }

		/// <summary>Gets or sets the primary group for this <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> object.</summary>
		/// <returns>The primary group for this <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> object.</returns>
		public abstract SecurityIdentifier Group { get; set; }

		/// <summary>Gets or sets the owner of the object associated with this <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> object.</summary>
		/// <returns>The owner of the object associated with this <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> object.</returns>
		public abstract SecurityIdentifier Owner { get; set; }

		/// <summary>Gets the revision level of the <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> object.</summary>
		/// <returns>A byte value that specifies the revision level of the <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" />.</returns>
		public static byte Revision
		{
			get
			{
				return 1;
			}
		}

		internal virtual GenericAcl InternalDacl
		{
			get
			{
				return null;
			}
		}

		internal virtual GenericAcl InternalSacl
		{
			get
			{
				return null;
			}
		}

		internal virtual byte InternalReservedField
		{
			get
			{
				return 0;
			}
		}

		/// <summary>Returns an array of byte values that represents the information contained in this <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> object.</summary>
		/// <param name="binaryForm">The byte array into which the contents of the <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> is marshaled.</param>
		/// <param name="offset">The offset at which to start marshaling.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is negative or too high to allow the entire <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> to be copied into <paramref name="array" />.</exception>
		public void GetBinaryForm(byte[] binaryForm, int offset)
		{
			if (binaryForm == null)
			{
				throw new ArgumentNullException("binaryForm");
			}
			int binaryLength = this.BinaryLength;
			if (offset < 0 || offset > binaryForm.Length - binaryLength)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			ControlFlags controlFlags = this.ControlFlags;
			if (this.DaclIsUnmodifiedAefa)
			{
				controlFlags &= ~ControlFlags.DiscretionaryAclPresent;
			}
			binaryForm[offset] = GenericSecurityDescriptor.Revision;
			binaryForm[offset + 1] = this.InternalReservedField;
			this.WriteUShort((ushort)controlFlags, binaryForm, offset + 2);
			int num = 20;
			if (this.Owner != null)
			{
				this.WriteInt(num, binaryForm, offset + 4);
				this.Owner.GetBinaryForm(binaryForm, offset + num);
				num += this.Owner.BinaryLength;
			}
			else
			{
				this.WriteInt(0, binaryForm, offset + 4);
			}
			if (this.Group != null)
			{
				this.WriteInt(num, binaryForm, offset + 8);
				this.Group.GetBinaryForm(binaryForm, offset + num);
				num += this.Group.BinaryLength;
			}
			else
			{
				this.WriteInt(0, binaryForm, offset + 8);
			}
			GenericAcl internalSacl = this.InternalSacl;
			if (this.SaclPresent)
			{
				this.WriteInt(num, binaryForm, offset + 12);
				internalSacl.GetBinaryForm(binaryForm, offset + num);
				num += this.InternalSacl.BinaryLength;
			}
			else
			{
				this.WriteInt(0, binaryForm, offset + 12);
			}
			GenericAcl internalDacl = this.InternalDacl;
			if (this.DaclPresent && !this.DaclIsUnmodifiedAefa)
			{
				this.WriteInt(num, binaryForm, offset + 16);
				internalDacl.GetBinaryForm(binaryForm, offset + num);
				num += this.InternalDacl.BinaryLength;
				return;
			}
			this.WriteInt(0, binaryForm, offset + 16);
		}

		/// <summary>Returns the Security Descriptor Definition Language (SDDL) representation of the specified sections of the security descriptor that this <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> object represents.</summary>
		/// <param name="includeSections">Specifies which sections (access rules, audit rules, primary group, owner) of the security descriptor to get.</param>
		/// <returns>The SDDL representation of the specified sections of the security descriptor associated with this <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> object.</returns>
		public string GetSddlForm(AccessControlSections includeSections)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if ((includeSections & AccessControlSections.Owner) != AccessControlSections.None && this.Owner != null)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "O:{0}", this.Owner.GetSddlForm());
			}
			if ((includeSections & AccessControlSections.Group) != AccessControlSections.None && this.Group != null)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "G:{0}", this.Group.GetSddlForm());
			}
			if ((includeSections & AccessControlSections.Access) != AccessControlSections.None && this.DaclPresent && !this.DaclIsUnmodifiedAefa)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "D:{0}", this.InternalDacl.GetSddlForm(this.ControlFlags, true));
			}
			if ((includeSections & AccessControlSections.Audit) != AccessControlSections.None && this.SaclPresent)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "S:{0}", this.InternalSacl.GetSddlForm(this.ControlFlags, false));
			}
			return stringBuilder.ToString();
		}

		/// <summary>Returns a boolean value that specifies whether the security descriptor associated with this  <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> object can be converted to the Security Descriptor Definition Language (SDDL) format.</summary>
		/// <returns>
		///   <see langword="true" /> if the security descriptor associated with this  <see cref="T:System.Security.AccessControl.GenericSecurityDescriptor" /> object can be converted to the Security Descriptor Definition Language (SDDL) format; otherwise, <see langword="false" />.</returns>
		public static bool IsSddlConversionSupported()
		{
			return true;
		}

		internal virtual bool DaclIsUnmodifiedAefa
		{
			get
			{
				return false;
			}
		}

		private bool DaclPresent
		{
			get
			{
				return this.InternalDacl != null && (this.ControlFlags & ControlFlags.DiscretionaryAclPresent) > ControlFlags.None;
			}
		}

		private bool SaclPresent
		{
			get
			{
				return this.InternalSacl != null && (this.ControlFlags & ControlFlags.SystemAclPresent) > ControlFlags.None;
			}
		}

		private void WriteUShort(ushort val, byte[] buffer, int offset)
		{
			buffer[offset] = (byte)val;
			buffer[offset + 1] = (byte)(val >> 8);
		}

		private void WriteInt(int val, byte[] buffer, int offset)
		{
			buffer[offset] = (byte)val;
			buffer[offset + 1] = (byte)(val >> 8);
			buffer[offset + 2] = (byte)(val >> 16);
			buffer[offset + 3] = (byte)(val >> 24);
		}
	}
}
