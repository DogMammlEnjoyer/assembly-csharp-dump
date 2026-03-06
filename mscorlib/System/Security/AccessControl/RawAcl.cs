using System;
using System.Collections.Generic;
using System.Text;

namespace System.Security.AccessControl
{
	/// <summary>Represents an Access Control List (ACL).</summary>
	public sealed class RawAcl : GenericAcl
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.RawAcl" /> class with the specified revision level.</summary>
		/// <param name="revision">The revision level of the new Access Control List (ACL).</param>
		/// <param name="capacity">The number of Access Control Entries (ACEs) this <see cref="T:System.Security.AccessControl.RawAcl" /> object can contain. This number is to be used only as a hint.</param>
		public RawAcl(byte revision, int capacity)
		{
			this.revision = revision;
			this.list = new List<GenericAce>(capacity);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.RawAcl" /> class from the specified binary form.</summary>
		/// <param name="binaryForm">An array of byte values that represent an Access Control List (ACL).</param>
		/// <param name="offset">The offset in the <paramref name="binaryForm" /> parameter at which to begin unmarshaling data.</param>
		public RawAcl(byte[] binaryForm, int offset)
		{
			if (binaryForm == null)
			{
				throw new ArgumentNullException("binaryForm");
			}
			if (offset < 0 || offset > binaryForm.Length - 8)
			{
				throw new ArgumentOutOfRangeException("offset", offset, "Offset out of range");
			}
			this.revision = binaryForm[offset];
			if (this.revision != GenericAcl.AclRevision && this.revision != GenericAcl.AclRevisionDS)
			{
				throw new ArgumentException("Invalid ACL - unknown revision", "binaryForm");
			}
			int num = (int)this.ReadUShort(binaryForm, offset + 2);
			if (offset > binaryForm.Length - num)
			{
				throw new ArgumentException("Invalid ACL - truncated", "binaryForm");
			}
			int num2 = offset + 8;
			int num3 = (int)this.ReadUShort(binaryForm, offset + 4);
			this.list = new List<GenericAce>(num3);
			for (int i = 0; i < num3; i++)
			{
				GenericAce genericAce = GenericAce.CreateFromBinaryForm(binaryForm, num2);
				this.list.Add(genericAce);
				num2 += genericAce.BinaryLength;
			}
		}

		internal RawAcl(byte revision, List<GenericAce> aces)
		{
			this.revision = revision;
			this.list = aces;
		}

		/// <summary>Gets the length, in bytes, of the binary representation of the current <see cref="T:System.Security.AccessControl.RawAcl" /> object. This length should be used before marshaling the ACL into a binary array with the <see cref="M:System.Security.AccessControl.RawAcl.GetBinaryForm(System.Byte[],System.Int32)" /> method.</summary>
		/// <returns>The length, in bytes, of the binary representation of the current <see cref="T:System.Security.AccessControl.RawAcl" /> object.</returns>
		public override int BinaryLength
		{
			get
			{
				int num = 8;
				foreach (GenericAce genericAce in this.list)
				{
					num += genericAce.BinaryLength;
				}
				return num;
			}
		}

		/// <summary>Gets the number of access control entries (ACEs) in the current <see cref="T:System.Security.AccessControl.RawAcl" /> object.</summary>
		/// <returns>The number of ACEs in the current <see cref="T:System.Security.AccessControl.RawAcl" /> object.</returns>
		public override int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		/// <summary>Gets or sets the Access Control Entry (ACE) at the specified index.</summary>
		/// <param name="index">The zero-based index of the ACE to get or set.</param>
		/// <returns>The ACE at the specified index.</returns>
		public override GenericAce this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		/// <summary>Gets the revision level of the <see cref="T:System.Security.AccessControl.RawAcl" />.</summary>
		/// <returns>A byte value that specifies the revision level of the <see cref="T:System.Security.AccessControl.RawAcl" />.</returns>
		public override byte Revision
		{
			get
			{
				return this.revision;
			}
		}

		/// <summary>Marshals the contents of the <see cref="T:System.Security.AccessControl.RawAcl" /> object into the specified byte array beginning at the specified offset.</summary>
		/// <param name="binaryForm">The byte array into which the contents of the <see cref="T:System.Security.AccessControl.RawAcl" /> is marshaled.</param>
		/// <param name="offset">The offset at which to start marshaling.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is negative or too high to allow the entire <see cref="T:System.Security.AccessControl.RawAcl" /> to be copied into <paramref name="array" />.</exception>
		public override void GetBinaryForm(byte[] binaryForm, int offset)
		{
			if (binaryForm == null)
			{
				throw new ArgumentNullException("binaryForm");
			}
			if (offset < 0 || offset > binaryForm.Length - this.BinaryLength)
			{
				throw new ArgumentException("Offset out of range", "offset");
			}
			binaryForm[offset] = this.Revision;
			binaryForm[offset + 1] = 0;
			this.WriteUShort((ushort)this.BinaryLength, binaryForm, offset + 2);
			this.WriteUShort((ushort)this.list.Count, binaryForm, offset + 4);
			this.WriteUShort(0, binaryForm, offset + 6);
			int num = offset + 8;
			foreach (GenericAce genericAce in this.list)
			{
				genericAce.GetBinaryForm(binaryForm, num);
				num += genericAce.BinaryLength;
			}
		}

		/// <summary>Inserts the specified Access Control Entry (ACE) at the specified index.</summary>
		/// <param name="index">The position at which to add the new ACE. Specify the value of the <see cref="P:System.Security.AccessControl.RawAcl.Count" /> property to insert an ACE at the end of the <see cref="T:System.Security.AccessControl.RawAcl" /> object.</param>
		/// <param name="ace">The ACE to insert.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset" /> is negative or too high to allow the entire <see cref="T:System.Security.AccessControl.GenericAcl" /> to be copied into <paramref name="array" />.</exception>
		public void InsertAce(int index, GenericAce ace)
		{
			if (ace == null)
			{
				throw new ArgumentNullException("ace");
			}
			this.list.Insert(index, ace);
		}

		/// <summary>Removes the Access Control Entry (ACE) at the specified location.</summary>
		/// <param name="index">The zero-based index of the ACE to remove.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value of the <paramref name="index" /> parameter is higher than the value of the <see cref="P:System.Security.AccessControl.RawAcl.Count" /> property minus one or is negative.</exception>
		public void RemoveAce(int index)
		{
			this.list.RemoveAt(index);
		}

		internal override string GetSddlForm(ControlFlags sdFlags, bool isDacl)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (isDacl)
			{
				if ((sdFlags & ControlFlags.DiscretionaryAclProtected) != ControlFlags.None)
				{
					stringBuilder.Append("P");
				}
				if ((sdFlags & ControlFlags.DiscretionaryAclAutoInheritRequired) != ControlFlags.None)
				{
					stringBuilder.Append("AR");
				}
				if ((sdFlags & ControlFlags.DiscretionaryAclAutoInherited) != ControlFlags.None)
				{
					stringBuilder.Append("AI");
				}
			}
			else
			{
				if ((sdFlags & ControlFlags.SystemAclProtected) != ControlFlags.None)
				{
					stringBuilder.Append("P");
				}
				if ((sdFlags & ControlFlags.SystemAclAutoInheritRequired) != ControlFlags.None)
				{
					stringBuilder.Append("AR");
				}
				if ((sdFlags & ControlFlags.SystemAclAutoInherited) != ControlFlags.None)
				{
					stringBuilder.Append("AI");
				}
			}
			foreach (GenericAce genericAce in this.list)
			{
				stringBuilder.Append(genericAce.GetSddlForm());
			}
			return stringBuilder.ToString();
		}

		internal static RawAcl ParseSddlForm(string sddlForm, bool isDacl, ref ControlFlags sdFlags, ref int pos)
		{
			RawAcl.ParseFlags(sddlForm, isDacl, ref sdFlags, ref pos);
			byte b = GenericAcl.AclRevision;
			List<GenericAce> list = new List<GenericAce>();
			while (pos < sddlForm.Length && sddlForm[pos] == '(')
			{
				GenericAce genericAce = GenericAce.CreateFromSddlForm(sddlForm, ref pos);
				if (genericAce as ObjectAce != null)
				{
					b = GenericAcl.AclRevisionDS;
				}
				list.Add(genericAce);
			}
			return new RawAcl(b, list);
		}

		private static void ParseFlags(string sddlForm, bool isDacl, ref ControlFlags sdFlags, ref int pos)
		{
			char c = char.ToUpperInvariant(sddlForm[pos]);
			while (c == 'P' || c == 'A')
			{
				if (c == 'P')
				{
					if (isDacl)
					{
						sdFlags |= ControlFlags.DiscretionaryAclProtected;
					}
					else
					{
						sdFlags |= ControlFlags.SystemAclProtected;
					}
					pos++;
				}
				else
				{
					if (sddlForm.Length <= pos + 1)
					{
						throw new ArgumentException("Invalid SDDL string.", "sddlForm");
					}
					c = char.ToUpperInvariant(sddlForm[pos + 1]);
					if (c == 'R')
					{
						if (isDacl)
						{
							sdFlags |= ControlFlags.DiscretionaryAclAutoInheritRequired;
						}
						else
						{
							sdFlags |= ControlFlags.SystemAclAutoInheritRequired;
						}
						pos += 2;
					}
					else
					{
						if (c != 'I')
						{
							throw new ArgumentException("Invalid SDDL string.", "sddlForm");
						}
						if (isDacl)
						{
							sdFlags |= ControlFlags.DiscretionaryAclAutoInherited;
						}
						else
						{
							sdFlags |= ControlFlags.SystemAclAutoInherited;
						}
						pos += 2;
					}
				}
				c = char.ToUpperInvariant(sddlForm[pos]);
			}
		}

		private void WriteUShort(ushort val, byte[] buffer, int offset)
		{
			buffer[offset] = (byte)val;
			buffer[offset + 1] = (byte)(val >> 8);
		}

		private ushort ReadUShort(byte[] buffer, int offset)
		{
			return (ushort)((int)buffer[offset] | (int)buffer[offset + 1] << 8);
		}

		private byte revision;

		private List<GenericAce> list;
	}
}
