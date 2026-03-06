using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	/// <summary>Represents the <see langword="Token" /> returned by the metadata to represent a signature.</summary>
	[ComVisible(true)]
	public readonly struct SignatureToken : IEquatable<SignatureToken>
	{
		internal SignatureToken(int val)
		{
			this.tokValue = val;
		}

		/// <summary>Checks if the given object is an instance of <see langword="SignatureToken" /> and is equal to this instance.</summary>
		/// <param name="obj">The object to compare with this <see langword="SignatureToken" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is an instance of <see langword="SignatureToken" /> and is equal to this object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			bool flag = obj is SignatureToken;
			if (flag)
			{
				SignatureToken signatureToken = (SignatureToken)obj;
				flag = (this.tokValue == signatureToken.tokValue);
			}
			return flag;
		}

		/// <summary>Indicates whether the current instance is equal to the specified <see cref="T:System.Reflection.Emit.SignatureToken" />.</summary>
		/// <param name="obj">The <see cref="T:System.Reflection.Emit.SignatureToken" /> to compare to the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the value of <paramref name="obj" /> is equal to the value of the current instance; otherwise, <see langword="false" />.</returns>
		public bool Equals(SignatureToken obj)
		{
			return this.tokValue == obj.tokValue;
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Emit.SignatureToken" /> structures are equal.</summary>
		/// <param name="a">The <see cref="T:System.Reflection.Emit.SignatureToken" /> to compare to <paramref name="b" />.</param>
		/// <param name="b">The <see cref="T:System.Reflection.Emit.SignatureToken" /> to compare to <paramref name="a" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(SignatureToken a, SignatureToken b)
		{
			return object.Equals(a, b);
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Emit.SignatureToken" /> structures are not equal.</summary>
		/// <param name="a">The <see cref="T:System.Reflection.Emit.SignatureToken" /> to compare to <paramref name="b" />.</param>
		/// <param name="b">The <see cref="T:System.Reflection.Emit.SignatureToken" /> to compare to <paramref name="a" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is not equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(SignatureToken a, SignatureToken b)
		{
			return !object.Equals(a, b);
		}

		/// <summary>Generates the hash code for this signature.</summary>
		/// <returns>The hash code for this signature.</returns>
		public override int GetHashCode()
		{
			return this.tokValue;
		}

		/// <summary>Retrieves the metadata token for the local variable signature for this method.</summary>
		/// <returns>Read-only. Retrieves the metadata token of this signature.</returns>
		public int Token
		{
			get
			{
				return this.tokValue;
			}
		}

		internal readonly int tokValue;

		/// <summary>The default <see langword="SignatureToken" /> with <see cref="P:System.Reflection.Emit.SignatureToken.Token" /> value 0.</summary>
		public static readonly SignatureToken Empty;
	}
}
