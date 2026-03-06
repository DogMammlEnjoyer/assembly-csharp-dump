using System;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	/// <summary>Represents a user or group account.</summary>
	[ComVisible(false)]
	public sealed class NTAccount : IdentityReference
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Principal.NTAccount" /> class by using the specified name.</summary>
		/// <param name="name">The name used to create the <see cref="T:System.Security.Principal.NTAccount" /> object. This parameter cannot be <see langword="null" /> or an empty string.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is an empty string.  
		/// -or-  
		/// <paramref name="name" /> is too long.</exception>
		public NTAccount(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException(Locale.GetText("Empty"), "name");
			}
			this._value = name;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Principal.NTAccount" /> class by using the specified domain name and account name.</summary>
		/// <param name="domainName">The name of the domain. This parameter can be <see langword="null" /> or an empty string. Domain names that are null values are treated like an empty string.</param>
		/// <param name="accountName">The name of the account. This parameter cannot be <see langword="null" /> or an empty string.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="accountName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="accountName" /> is an empty string.  
		/// -or-  
		/// <paramref name="accountName" /> is too long.  
		/// -or-  
		/// <paramref name="domainName" /> is too long.</exception>
		public NTAccount(string domainName, string accountName)
		{
			if (accountName == null)
			{
				throw new ArgumentNullException("accountName");
			}
			if (accountName.Length == 0)
			{
				throw new ArgumentException(Locale.GetText("Empty"), "accountName");
			}
			if (domainName == null)
			{
				this._value = accountName;
				return;
			}
			this._value = domainName + "\\" + accountName;
		}

		/// <summary>Returns an uppercase string representation of this <see cref="T:System.Security.Principal.NTAccount" /> object.</summary>
		/// <returns>The uppercase string representation of this <see cref="T:System.Security.Principal.NTAccount" /> object.</returns>
		public override string Value
		{
			get
			{
				return this._value;
			}
		}

		/// <summary>Returns a value that indicates whether this <see cref="T:System.Security.Principal.NTAccount" /> object is equal to a specified object.</summary>
		/// <param name="o">An object to compare with this <see cref="T:System.Security.Principal.NTAccount" /> object, or <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="o" /> is an object with the same underlying type and value as this <see cref="T:System.Security.Principal.NTAccount" /> object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object o)
		{
			NTAccount ntaccount = o as NTAccount;
			return !(ntaccount == null) && ntaccount.Value == this.Value;
		}

		/// <summary>Serves as a hash function for the current <see cref="T:System.Security.Principal.NTAccount" /> object. The <see cref="M:System.Security.Principal.NTAccount.GetHashCode" /> method is suitable for hashing algorithms and data structures like a hash table.</summary>
		/// <returns>A hash value for the current <see cref="T:System.Security.Principal.NTAccount" /> object.</returns>
		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		/// <summary>Returns a value that indicates whether the specified type is a valid translation type for the <see cref="T:System.Security.Principal.NTAccount" /> class.</summary>
		/// <param name="targetType">The type being queried for validity to serve as a conversion from <see cref="T:System.Security.Principal.NTAccount" />. The following target types are valid:  
		///  - <see cref="T:System.Security.Principal.NTAccount" />  
		///  - <see cref="T:System.Security.Principal.SecurityIdentifier" /></param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="targetType" /> is a valid translation type for the <see cref="T:System.Security.Principal.NTAccount" /> class; otherwise <see langword="false" />.</returns>
		public override bool IsValidTargetType(Type targetType)
		{
			return targetType == typeof(NTAccount) || targetType == typeof(SecurityIdentifier);
		}

		/// <summary>Returns the account name, in Domain \ Account format, for the account represented by the <see cref="T:System.Security.Principal.NTAccount" /> object.</summary>
		/// <returns>The account name, in Domain \ Account format.</returns>
		public override string ToString()
		{
			return this.Value;
		}

		/// <summary>Translates the account name represented by the <see cref="T:System.Security.Principal.NTAccount" /> object into another <see cref="T:System.Security.Principal.IdentityReference" />-derived type.</summary>
		/// <param name="targetType">The target type for the conversion from <see cref="T:System.Security.Principal.NTAccount" />. The target type must be a type that is considered valid by the <see cref="M:System.Security.Principal.NTAccount.IsValidTargetType(System.Type)" /> method.</param>
		/// <returns>The converted identity.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="targetType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="targetType" /> is not an <see cref="T:System.Security.Principal.IdentityReference" /> type.</exception>
		/// <exception cref="T:System.Security.Principal.IdentityNotMappedException">Some or all identity references could not be translated.</exception>
		/// <exception cref="T:System.SystemException">The source account name is too long.  
		///  -or-  
		///  A Win32 error code was returned.</exception>
		public override IdentityReference Translate(Type targetType)
		{
			if (targetType == typeof(NTAccount))
			{
				return this;
			}
			if (!(targetType == typeof(SecurityIdentifier)))
			{
				throw new ArgumentException("Unknown type", "targetType");
			}
			WellKnownAccount wellKnownAccount = WellKnownAccount.LookupByName(this.Value);
			if (wellKnownAccount == null || wellKnownAccount.Sid == null)
			{
				throw new IdentityNotMappedException("Cannot map account name: " + this.Value);
			}
			return new SecurityIdentifier(wellKnownAccount.Sid);
		}

		/// <summary>Compares two <see cref="T:System.Security.Principal.NTAccount" /> objects to determine whether they are equal. They are considered equal if they have the same canonical name representation as the one returned by the <see cref="P:System.Security.Principal.NTAccount.Value" /> property or if they are both <see langword="null" />.</summary>
		/// <param name="left">The left operand to use for the equality comparison. This parameter can be <see langword="null" />.</param>
		/// <param name="right">The right operand to use for the equality comparison. This parameter can be <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise <see langword="false" />.</returns>
		public static bool operator ==(NTAccount left, NTAccount right)
		{
			if (left == null)
			{
				return right == null;
			}
			return right != null && left.Value == right.Value;
		}

		/// <summary>Compares two <see cref="T:System.Security.Principal.NTAccount" /> objects to determine whether they are not equal. They are considered not equal if they have different canonical name representations than the one returned by the <see cref="P:System.Security.Principal.NTAccount.Value" /> property or if one of the objects is <see langword="null" /> and the other is not.</summary>
		/// <param name="left">The left operand to use for the inequality comparison. This parameter can be <see langword="null" />.</param>
		/// <param name="right">The right operand to use for the inequality comparison. This parameter can be <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise <see langword="false" />.</returns>
		public static bool operator !=(NTAccount left, NTAccount right)
		{
			if (left == null)
			{
				return right != null;
			}
			return right == null || left.Value != right.Value;
		}

		private string _value;
	}
}
