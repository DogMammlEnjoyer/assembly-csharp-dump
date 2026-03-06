using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	/// <summary>The <see langword="MethodToken" /> struct is an object representation of a token that represents a method.</summary>
	[ComVisible(true)]
	[Serializable]
	public readonly struct MethodToken : IEquatable<MethodToken>
	{
		internal MethodToken(int val)
		{
			this.tokValue = val;
		}

		/// <summary>Tests whether the given object is equal to this <see langword="MethodToken" /> object.</summary>
		/// <param name="obj">The object to compare to this object.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is an instance of <see langword="MethodToken" /> and is equal to this object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			bool flag = obj is MethodToken;
			if (flag)
			{
				MethodToken methodToken = (MethodToken)obj;
				flag = (this.tokValue == methodToken.tokValue);
			}
			return flag;
		}

		/// <summary>Indicates whether the current instance is equal to the specified <see cref="T:System.Reflection.Emit.MethodToken" />.</summary>
		/// <param name="obj">The <see cref="T:System.Reflection.Emit.MethodToken" /> to compare to the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the value of <paramref name="obj" /> is equal to the value of the current instance; otherwise, <see langword="false" />.</returns>
		public bool Equals(MethodToken obj)
		{
			return this.tokValue == obj.tokValue;
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Emit.MethodToken" /> structures are equal.</summary>
		/// <param name="a">The <see cref="T:System.Reflection.Emit.MethodToken" /> to compare to <paramref name="b" />.</param>
		/// <param name="b">The <see cref="T:System.Reflection.Emit.MethodToken" /> to compare to <paramref name="a" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(MethodToken a, MethodToken b)
		{
			return object.Equals(a, b);
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Emit.MethodToken" /> structures are not equal.</summary>
		/// <param name="a">The <see cref="T:System.Reflection.Emit.MethodToken" /> to compare to <paramref name="b" />.</param>
		/// <param name="b">The <see cref="T:System.Reflection.Emit.MethodToken" /> to compare to <paramref name="a" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is not equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(MethodToken a, MethodToken b)
		{
			return !object.Equals(a, b);
		}

		/// <summary>Returns the generated hash code for this method.</summary>
		/// <returns>The hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return this.tokValue;
		}

		/// <summary>Returns the metadata token for this method.</summary>
		/// <returns>Read-only. Returns the metadata token for this method.</returns>
		public int Token
		{
			get
			{
				return this.tokValue;
			}
		}

		internal readonly int tokValue;

		/// <summary>The default <see langword="MethodToken" /> with <see cref="P:System.Reflection.Emit.MethodToken.Token" /> value 0.</summary>
		public static readonly MethodToken Empty;
	}
}
