using System;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using Unity;

namespace System.Security.AccessControl
{
	/// <summary>Encapsulates all Access Control Entry (ACE) types currently defined by Microsoft Corporation. All <see cref="T:System.Security.AccessControl.KnownAce" /> objects contain a 32-bit access mask and a <see cref="T:System.Security.Principal.SecurityIdentifier" /> object.</summary>
	public abstract class KnownAce : GenericAce
	{
		internal KnownAce(AceType type, AceFlags flags) : base(type, flags)
		{
		}

		internal KnownAce(byte[] binaryForm, int offset) : base(binaryForm, offset)
		{
		}

		/// <summary>Gets or sets the access mask for this <see cref="T:System.Security.AccessControl.KnownAce" /> object.</summary>
		/// <returns>The access mask for this <see cref="T:System.Security.AccessControl.KnownAce" /> object.</returns>
		public int AccessMask
		{
			get
			{
				return this.access_mask;
			}
			set
			{
				this.access_mask = value;
			}
		}

		/// <summary>Gets or sets the <see cref="T:System.Security.Principal.SecurityIdentifier" /> object associated with this <see cref="T:System.Security.AccessControl.KnownAce" /> object.</summary>
		/// <returns>The <see cref="T:System.Security.Principal.SecurityIdentifier" /> object associated with this <see cref="T:System.Security.AccessControl.KnownAce" /> object.</returns>
		public SecurityIdentifier SecurityIdentifier
		{
			get
			{
				return this.identifier;
			}
			set
			{
				this.identifier = value;
			}
		}

		internal static string GetSddlAccessRights(int accessMask)
		{
			string sddlAliasRights = KnownAce.GetSddlAliasRights(accessMask);
			if (!string.IsNullOrEmpty(sddlAliasRights))
			{
				return sddlAliasRights;
			}
			return string.Format(CultureInfo.InvariantCulture, "0x{0:x}", accessMask);
		}

		private static string GetSddlAliasRights(int accessMask)
		{
			SddlAccessRight[] array = SddlAccessRight.Decompose(accessMask);
			if (array == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SddlAccessRight sddlAccessRight in array)
			{
				stringBuilder.Append(sddlAccessRight.Name);
			}
			return stringBuilder.ToString();
		}

		internal KnownAce()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private int access_mask;

		private SecurityIdentifier identifier;
	}
}
