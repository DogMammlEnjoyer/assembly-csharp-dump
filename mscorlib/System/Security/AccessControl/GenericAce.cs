using System;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using Unity;

namespace System.Security.AccessControl
{
	/// <summary>Represents an Access Control Entry (ACE), and is the base class for all other ACE classes.</summary>
	public abstract class GenericAce
	{
		internal GenericAce(AceType type, AceFlags flags)
		{
			if (type > AceType.SystemAlarmCallbackObject)
			{
				throw new ArgumentOutOfRangeException("type");
			}
			this.ace_type = type;
			this.ace_flags = flags;
		}

		internal GenericAce(byte[] binaryForm, int offset)
		{
			if (binaryForm == null)
			{
				throw new ArgumentNullException("binaryForm");
			}
			if (offset < 0 || offset > binaryForm.Length - 2)
			{
				throw new ArgumentOutOfRangeException("offset", offset, "Offset out of range");
			}
			this.ace_type = (AceType)binaryForm[offset];
			this.ace_flags = (AceFlags)binaryForm[offset + 1];
		}

		/// <summary>Gets or sets the <see cref="T:System.Security.AccessControl.AceFlags" /> associated with this <see cref="T:System.Security.AccessControl.GenericAce" /> object.</summary>
		/// <returns>The <see cref="T:System.Security.AccessControl.AceFlags" /> associated with this <see cref="T:System.Security.AccessControl.GenericAce" /> object.</returns>
		public AceFlags AceFlags
		{
			get
			{
				return this.ace_flags;
			}
			set
			{
				this.ace_flags = value;
			}
		}

		/// <summary>Gets the type of this Access Control Entry (ACE).</summary>
		/// <returns>The type of this ACE.</returns>
		public AceType AceType
		{
			get
			{
				return this.ace_type;
			}
		}

		/// <summary>Gets the audit information associated with this Access Control Entry (ACE).</summary>
		/// <returns>The audit information associated with this Access Control Entry (ACE).</returns>
		public AuditFlags AuditFlags
		{
			get
			{
				AuditFlags auditFlags = AuditFlags.None;
				if ((this.ace_flags & AceFlags.SuccessfulAccess) != AceFlags.None)
				{
					auditFlags |= AuditFlags.Success;
				}
				if ((this.ace_flags & AceFlags.FailedAccess) != AceFlags.None)
				{
					auditFlags |= AuditFlags.Failure;
				}
				return auditFlags;
			}
		}

		/// <summary>Gets the length, in bytes, of the binary representation of the current <see cref="T:System.Security.AccessControl.GenericAce" /> object. This length should be used before marshaling the ACL into a binary array with the <see cref="M:System.Security.AccessControl.GenericAce.GetBinaryForm(System.Byte[],System.Int32)" /> method.</summary>
		/// <returns>The length, in bytes, of the binary representation of the current <see cref="T:System.Security.AccessControl.GenericAce" /> object.</returns>
		public abstract int BinaryLength { get; }

		/// <summary>Gets flags that specify the inheritance properties of this Access Control Entry (ACE).</summary>
		/// <returns>Flags that specify the inheritance properties of this ACE.</returns>
		public InheritanceFlags InheritanceFlags
		{
			get
			{
				InheritanceFlags inheritanceFlags = InheritanceFlags.None;
				if ((this.ace_flags & AceFlags.ObjectInherit) != AceFlags.None)
				{
					inheritanceFlags |= InheritanceFlags.ObjectInherit;
				}
				if ((this.ace_flags & AceFlags.ContainerInherit) != AceFlags.None)
				{
					inheritanceFlags |= InheritanceFlags.ContainerInherit;
				}
				return inheritanceFlags;
			}
		}

		/// <summary>Gets a Boolean value that specifies whether this Access Control Entry (ACE) is inherited or is set explicitly.</summary>
		/// <returns>
		///   <see langword="true" /> if this ACE is inherited; otherwise, <see langword="false" />.</returns>
		public bool IsInherited
		{
			get
			{
				return (this.ace_flags & AceFlags.Inherited) > AceFlags.None;
			}
		}

		/// <summary>Gets flags that specify the inheritance propagation properties of this Access Control Entry (ACE).</summary>
		/// <returns>Flags that specify the inheritance propagation properties of this ACE.</returns>
		public PropagationFlags PropagationFlags
		{
			get
			{
				PropagationFlags propagationFlags = PropagationFlags.None;
				if ((this.ace_flags & AceFlags.InheritOnly) != AceFlags.None)
				{
					propagationFlags |= PropagationFlags.InheritOnly;
				}
				if ((this.ace_flags & AceFlags.NoPropagateInherit) != AceFlags.None)
				{
					propagationFlags |= PropagationFlags.NoPropagateInherit;
				}
				return propagationFlags;
			}
		}

		/// <summary>Creates a deep copy of this Access Control Entry (ACE).</summary>
		/// <returns>The <see cref="T:System.Security.AccessControl.GenericAce" /> object that this method creates.</returns>
		public GenericAce Copy()
		{
			byte[] binaryForm = new byte[this.BinaryLength];
			this.GetBinaryForm(binaryForm, 0);
			return GenericAce.CreateFromBinaryForm(binaryForm, 0);
		}

		/// <summary>Creates a <see cref="T:System.Security.AccessControl.GenericAce" /> object from the specified binary data.</summary>
		/// <param name="binaryForm">The binary data from which to create the new <see cref="T:System.Security.AccessControl.GenericAce" /> object.</param>
		/// <param name="offset">The offset at which to begin unmarshaling.</param>
		/// <returns>The <see cref="T:System.Security.AccessControl.GenericAce" /> object this method creates.</returns>
		public static GenericAce CreateFromBinaryForm(byte[] binaryForm, int offset)
		{
			if (binaryForm == null)
			{
				throw new ArgumentNullException("binaryForm");
			}
			if (offset < 0 || offset > binaryForm.Length - 1)
			{
				throw new ArgumentOutOfRangeException("offset", offset, "Offset out of range");
			}
			if (GenericAce.IsObjectType((AceType)binaryForm[offset]))
			{
				return new ObjectAce(binaryForm, offset);
			}
			return new CommonAce(binaryForm, offset);
		}

		/// <summary>Determines whether the specified <see cref="T:System.Security.AccessControl.GenericAce" /> object is equal to the current <see cref="T:System.Security.AccessControl.GenericAce" /> object.</summary>
		/// <param name="o">The <see cref="T:System.Security.AccessControl.GenericAce" /> object to compare to the current <see cref="T:System.Security.AccessControl.GenericAce" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the specified <see cref="T:System.Security.AccessControl.GenericAce" /> object is equal to the current <see cref="T:System.Security.AccessControl.GenericAce" /> object; otherwise, <see langword="false" />.</returns>
		public sealed override bool Equals(object o)
		{
			return this == o as GenericAce;
		}

		/// <summary>Marshals the contents of the <see cref="T:System.Security.AccessControl.GenericAce" /> object into the specified byte array beginning at the specified offset.</summary>
		/// <param name="binaryForm">The byte array into which the contents of the <see cref="T:System.Security.AccessControl.GenericAce" /> is marshaled.</param>
		/// <param name="offset">The offset at which to start marshaling.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is negative or too high to allow the entire <see cref="T:System.Security.AccessControl.GenericAcl" /> to be copied into <paramref name="array" />.</exception>
		public abstract void GetBinaryForm(byte[] binaryForm, int offset);

		/// <summary>Serves as a hash function for the <see cref="T:System.Security.AccessControl.GenericAce" /> class. The  <see cref="M:System.Security.AccessControl.GenericAce.GetHashCode" /> method is suitable for use in hashing algorithms and data structures like a hash table.</summary>
		/// <returns>A hash code for the current <see cref="T:System.Security.AccessControl.GenericAce" /> object.</returns>
		public sealed override int GetHashCode()
		{
			byte[] array = new byte[this.BinaryLength];
			this.GetBinaryForm(array, 0);
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				num = (num << 3 | (num >> 29 & 7));
				num ^= (int)(array[i] & byte.MaxValue);
			}
			return num;
		}

		/// <summary>Determines whether the specified <see cref="T:System.Security.AccessControl.GenericAce" /> objects are considered equal.</summary>
		/// <param name="left">The first <see cref="T:System.Security.AccessControl.GenericAce" /> object to compare.</param>
		/// <param name="right">The second <see cref="T:System.Security.AccessControl.GenericAce" /> to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the two <see cref="T:System.Security.AccessControl.GenericAce" /> objects are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(GenericAce left, GenericAce right)
		{
			if (left == null)
			{
				return right == null;
			}
			if (right == null)
			{
				return false;
			}
			int binaryLength = left.BinaryLength;
			int binaryLength2 = right.BinaryLength;
			if (binaryLength != binaryLength2)
			{
				return false;
			}
			byte[] array = new byte[binaryLength];
			byte[] array2 = new byte[binaryLength2];
			left.GetBinaryForm(array, 0);
			right.GetBinaryForm(array2, 0);
			for (int i = 0; i < binaryLength; i++)
			{
				if (array[i] != array2[i])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Determines whether the specified <see cref="T:System.Security.AccessControl.GenericAce" /> objects are considered unequal.</summary>
		/// <param name="left">The first <see cref="T:System.Security.AccessControl.GenericAce" /> object to compare.</param>
		/// <param name="right">The second <see cref="T:System.Security.AccessControl.GenericAce" /> to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the two <see cref="T:System.Security.AccessControl.GenericAce" /> objects are unequal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(GenericAce left, GenericAce right)
		{
			if (left == null)
			{
				return right != null;
			}
			if (right == null)
			{
				return true;
			}
			int binaryLength = left.BinaryLength;
			int binaryLength2 = right.BinaryLength;
			if (binaryLength != binaryLength2)
			{
				return true;
			}
			byte[] array = new byte[binaryLength];
			byte[] array2 = new byte[binaryLength2];
			left.GetBinaryForm(array, 0);
			right.GetBinaryForm(array2, 0);
			for (int i = 0; i < binaryLength; i++)
			{
				if (array[i] != array2[i])
				{
					return true;
				}
			}
			return false;
		}

		internal abstract string GetSddlForm();

		internal static GenericAce CreateFromSddlForm(string sddlForm, ref int pos)
		{
			if (sddlForm[pos] != '(')
			{
				throw new ArgumentException("Invalid SDDL string.", "sddlForm");
			}
			int num = sddlForm.IndexOf(')', pos);
			if (num < 0)
			{
				throw new ArgumentException("Invalid SDDL string.", "sddlForm");
			}
			int length = num - (pos + 1);
			string[] array = sddlForm.Substring(pos + 1, length).ToUpperInvariant().Split(';', StringSplitOptions.None);
			if (array.Length != 6)
			{
				throw new ArgumentException("Invalid SDDL string.", "sddlForm");
			}
			ObjectAceFlags objectAceFlags = ObjectAceFlags.None;
			AceType aceType = GenericAce.ParseSddlAceType(array[0]);
			AceFlags flags = GenericAce.ParseSddlAceFlags(array[1]);
			int accessMask = GenericAce.ParseSddlAccessRights(array[2]);
			Guid empty = Guid.Empty;
			if (!string.IsNullOrEmpty(array[3]))
			{
				empty = new Guid(array[3]);
				objectAceFlags |= ObjectAceFlags.ObjectAceTypePresent;
			}
			Guid empty2 = Guid.Empty;
			if (!string.IsNullOrEmpty(array[4]))
			{
				empty2 = new Guid(array[4]);
				objectAceFlags |= ObjectAceFlags.InheritedObjectAceTypePresent;
			}
			SecurityIdentifier sid = new SecurityIdentifier(array[5]);
			if (aceType == AceType.AccessAllowedCallback || aceType == AceType.AccessDeniedCallback)
			{
				throw new NotImplementedException("Conditional ACEs not supported");
			}
			pos = num + 1;
			if (GenericAce.IsObjectType(aceType))
			{
				return new ObjectAce(aceType, flags, accessMask, sid, objectAceFlags, empty, empty2, null);
			}
			if (objectAceFlags != ObjectAceFlags.None)
			{
				throw new ArgumentException("Invalid SDDL string.", "sddlForm");
			}
			return new CommonAce(aceType, flags, accessMask, sid, null);
		}

		private static bool IsObjectType(AceType type)
		{
			return type == AceType.AccessAllowedCallbackObject || type == AceType.AccessAllowedObject || type == AceType.AccessDeniedCallbackObject || type == AceType.AccessDeniedObject || type == AceType.SystemAlarmCallbackObject || type == AceType.SystemAlarmObject || type == AceType.SystemAuditCallbackObject || type == AceType.SystemAuditObject;
		}

		internal static string GetSddlAceType(AceType type)
		{
			switch (type)
			{
			case AceType.AccessAllowed:
				return "A";
			case AceType.AccessDenied:
				return "D";
			case AceType.SystemAudit:
				return "AU";
			case AceType.SystemAlarm:
				return "AL";
			case AceType.AccessAllowedObject:
				return "OA";
			case AceType.AccessDeniedObject:
				return "OD";
			case AceType.SystemAuditObject:
				return "OU";
			case AceType.SystemAlarmObject:
				return "OL";
			case AceType.AccessAllowedCallback:
				return "XA";
			case AceType.AccessDeniedCallback:
				return "XD";
			}
			throw new ArgumentException("Unable to convert to SDDL ACE type: " + type.ToString(), "type");
		}

		private static AceType ParseSddlAceType(string type)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(type);
			if (num <= 2078582897U)
			{
				if (num <= 936719067U)
				{
					if (num != 517278592U)
					{
						if (num == 936719067U)
						{
							if (type == "AU")
							{
								return AceType.SystemAudit;
							}
						}
					}
					else if (type == "AL")
					{
						return AceType.SystemAlarm;
					}
				}
				else if (num != 1561581017U)
				{
					if (num != 1611913874U)
					{
						if (num == 2078582897U)
						{
							if (type == "OU")
							{
								return AceType.SystemAuditObject;
							}
						}
					}
					else if (type == "XA")
					{
						return AceType.AccessAllowedCallback;
					}
				}
				else if (type == "XD")
				{
					return AceType.AccessDeniedCallback;
				}
			}
			else if (num <= 2330247182U)
			{
				if (num != 2196026230U)
				{
					if (num == 2330247182U)
					{
						if (type == "OD")
						{
							return AceType.AccessDeniedObject;
						}
					}
				}
				else if (type == "OL")
				{
					return AceType.SystemAlarmObject;
				}
			}
			else if (num != 2414135277U)
			{
				if (num != 3238785555U)
				{
					if (num == 3289118412U)
					{
						if (type == "A")
						{
							return AceType.AccessAllowed;
						}
					}
				}
				else if (type == "D")
				{
					return AceType.AccessDenied;
				}
			}
			else if (type == "OA")
			{
				return AceType.AccessAllowedObject;
			}
			throw new ArgumentException("Unable to convert SDDL to ACE type: " + type, "type");
		}

		internal static string GetSddlAceFlags(AceFlags flags)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if ((flags & AceFlags.ObjectInherit) != AceFlags.None)
			{
				stringBuilder.Append("OI");
			}
			if ((flags & AceFlags.ContainerInherit) != AceFlags.None)
			{
				stringBuilder.Append("CI");
			}
			if ((flags & AceFlags.NoPropagateInherit) != AceFlags.None)
			{
				stringBuilder.Append("NP");
			}
			if ((flags & AceFlags.InheritOnly) != AceFlags.None)
			{
				stringBuilder.Append("IO");
			}
			if ((flags & AceFlags.Inherited) != AceFlags.None)
			{
				stringBuilder.Append("ID");
			}
			if ((flags & AceFlags.SuccessfulAccess) != AceFlags.None)
			{
				stringBuilder.Append("SA");
			}
			if ((flags & AceFlags.FailedAccess) != AceFlags.None)
			{
				stringBuilder.Append("FA");
			}
			return stringBuilder.ToString();
		}

		private static AceFlags ParseSddlAceFlags(string flags)
		{
			AceFlags aceFlags = AceFlags.None;
			int i = 0;
			while (i < flags.Length - 1)
			{
				string text = flags.Substring(i, 2);
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 1476560089U)
				{
					if (num != 619077139U)
					{
						if (num != 1458105184U)
						{
							if (num != 1476560089U)
							{
								goto IL_112;
							}
							if (!(text == "SA"))
							{
								goto IL_112;
							}
							aceFlags |= AceFlags.SuccessfulAccess;
						}
						else
						{
							if (!(text == "ID"))
							{
								goto IL_112;
							}
							aceFlags |= AceFlags.Inherited;
						}
					}
					else
					{
						if (!(text == "NP"))
						{
							goto IL_112;
						}
						aceFlags |= AceFlags.NoPropagateInherit;
					}
				}
				else if (num <= 2145001825U)
				{
					if (num != 1642658993U)
					{
						if (num != 2145001825U)
						{
							goto IL_112;
						}
						if (!(text == "CI"))
						{
							goto IL_112;
						}
						aceFlags |= AceFlags.ContainerInherit;
					}
					else
					{
						if (!(text == "IO"))
						{
							goto IL_112;
						}
						aceFlags |= AceFlags.InheritOnly;
					}
				}
				else if (num != 2211671016U)
				{
					if (num != 2279914325U)
					{
						goto IL_112;
					}
					if (!(text == "OI"))
					{
						goto IL_112;
					}
					aceFlags |= AceFlags.ObjectInherit;
				}
				else
				{
					if (!(text == "FA"))
					{
						goto IL_112;
					}
					aceFlags |= AceFlags.FailedAccess;
				}
				i += 2;
				continue;
				IL_112:
				throw new ArgumentException("Invalid SDDL string.", "flags");
			}
			if (i != flags.Length)
			{
				throw new ArgumentException("Invalid SDDL string.", "flags");
			}
			return aceFlags;
		}

		private static int ParseSddlAccessRights(string accessMask)
		{
			if (accessMask.StartsWith("0X"))
			{
				return int.Parse(accessMask.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			}
			if (char.IsDigit(accessMask, 0))
			{
				return int.Parse(accessMask, NumberStyles.Integer, CultureInfo.InvariantCulture);
			}
			return GenericAce.ParseSddlAliasRights(accessMask);
		}

		private static int ParseSddlAliasRights(string accessMask)
		{
			int num = 0;
			int i;
			for (i = 0; i < accessMask.Length - 1; i += 2)
			{
				SddlAccessRight sddlAccessRight = SddlAccessRight.LookupByName(accessMask.Substring(i, 2));
				if (sddlAccessRight == null)
				{
					throw new ArgumentException("Invalid SDDL string.", "accessMask");
				}
				num |= sddlAccessRight.Value;
			}
			if (i != accessMask.Length)
			{
				throw new ArgumentException("Invalid SDDL string.", "accessMask");
			}
			return num;
		}

		internal static ushort ReadUShort(byte[] buffer, int offset)
		{
			return (ushort)((int)buffer[offset] | (int)buffer[offset + 1] << 8);
		}

		internal static int ReadInt(byte[] buffer, int offset)
		{
			return (int)buffer[offset] | (int)buffer[offset + 1] << 8 | (int)buffer[offset + 2] << 16 | (int)buffer[offset + 3] << 24;
		}

		internal static void WriteInt(int val, byte[] buffer, int offset)
		{
			buffer[offset] = (byte)val;
			buffer[offset + 1] = (byte)(val >> 8);
			buffer[offset + 2] = (byte)(val >> 16);
			buffer[offset + 3] = (byte)(val >> 24);
		}

		internal static void WriteUShort(ushort val, byte[] buffer, int offset)
		{
			buffer[offset] = (byte)val;
			buffer[offset + 1] = (byte)(val >> 8);
		}

		internal GenericAce()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private AceFlags ace_flags;

		private AceType ace_type;
	}
}
