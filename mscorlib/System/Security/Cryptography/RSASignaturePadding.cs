using System;
using Unity;

namespace System.Security.Cryptography
{
	/// <summary>Specifies the padding mode and parameters to use with RSA signature creation or verification operations.</summary>
	public sealed class RSASignaturePadding : IEquatable<RSASignaturePadding>
	{
		private RSASignaturePadding(RSASignaturePaddingMode mode)
		{
			this._mode = mode;
		}

		/// <summary>Gets an object that uses the PKCS #1 v1.5 padding mode.</summary>
		/// <returns>An object that uses the <see cref="F:System.Security.Cryptography.RSASignaturePaddingMode.Pkcs1" /> padding mode.</returns>
		public static RSASignaturePadding Pkcs1
		{
			get
			{
				return RSASignaturePadding.s_pkcs1;
			}
		}

		/// <summary>Gets an object that uses PSS padding mode.</summary>
		/// <returns>An object that uses the <see cref="F:System.Security.Cryptography.RSASignaturePaddingMode.Pss" /> padding mode with the number of salt bytes equal to the size of the hash.</returns>
		public static RSASignaturePadding Pss
		{
			get
			{
				return RSASignaturePadding.s_pss;
			}
		}

		/// <summary>Gets the padding mode of this <see cref="T:System.Security.Cryptography.RSASignaturePadding" /> instance.</summary>
		/// <returns>The padding mode (either <see cref="F:System.Security.Cryptography.RSASignaturePaddingMode.Pkcs1" /> or <see cref="F:System.Security.Cryptography.RSASignaturePaddingMode.Pss" />) of this instance.</returns>
		public RSASignaturePaddingMode Mode
		{
			get
			{
				return this._mode;
			}
		}

		/// <summary>Returns the hash code for this <see cref="T:System.Security.Cryptography.RSASignaturePadding" /> instance.</summary>
		/// <returns>The hash code for this <see cref="T:System.Security.Cryptography.RSASignaturePadding" /> instance.</returns>
		public override int GetHashCode()
		{
			return this._mode.GetHashCode();
		}

		/// <summary>Returns a value that indicates whether this instance is equal to a specified object.</summary>
		/// <param name="obj">The object to compare with the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the specified object is equal to the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return this.Equals(obj as RSASignaturePadding);
		}

		/// <summary>Returns a value that indicates whether this instance is equal to a specified <see cref="T:System.Security.Cryptography.RSASignaturePadding" /> object.</summary>
		/// <param name="other">The object to compare with the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the specified object is equal to the current object; otherwise, <see langword="false" />.</returns>
		public bool Equals(RSASignaturePadding other)
		{
			return other != null && this._mode == other._mode;
		}

		/// <summary>Indicates whether two specified <see cref="T:System.Security.Cryptography.RSASignaturePadding" /> objects are equal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <see langword="left" /> and <see langword="right" /> are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(RSASignaturePadding left, RSASignaturePadding right)
		{
			if (left == null)
			{
				return right == null;
			}
			return left.Equals(right);
		}

		/// <summary>Indicates whether two specified <see cref="T:System.Security.Cryptography.RSASignaturePadding" /> objects are unequal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <see langword="left" /> and <see langword="right" /> are unequal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(RSASignaturePadding left, RSASignaturePadding right)
		{
			return !(left == right);
		}

		/// <summary>Returns the string representation of the current <see cref="T:System.Security.Cryptography.RSASignaturePadding" /> instance.</summary>
		/// <returns>The string representation of the current object.</returns>
		public override string ToString()
		{
			return this._mode.ToString();
		}

		internal RSASignaturePadding()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private static readonly RSASignaturePadding s_pkcs1 = new RSASignaturePadding(RSASignaturePaddingMode.Pkcs1);

		private static readonly RSASignaturePadding s_pss = new RSASignaturePadding(RSASignaturePaddingMode.Pss);

		private readonly RSASignaturePaddingMode _mode;
	}
}
