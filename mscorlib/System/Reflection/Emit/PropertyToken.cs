using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	/// <summary>The <see langword="PropertyToken" /> struct is an opaque representation of the <see langword="Token" /> returned by the metadata to represent a property.</summary>
	[ComVisible(true)]
	[Serializable]
	public readonly struct PropertyToken : IEquatable<PropertyToken>
	{
		internal PropertyToken(int val)
		{
			this.tokValue = val;
		}

		/// <summary>Checks if the given object is an instance of <see langword="PropertyToken" /> and is equal to this instance.</summary>
		/// <param name="obj">The object to this object.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is an instance of <see langword="PropertyToken" /> and equals the current instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			bool flag = obj is PropertyToken;
			if (flag)
			{
				PropertyToken propertyToken = (PropertyToken)obj;
				flag = (this.tokValue == propertyToken.tokValue);
			}
			return flag;
		}

		/// <summary>Indicates whether the current instance is equal to the specified <see cref="T:System.Reflection.Emit.PropertyToken" />.</summary>
		/// <param name="obj">The <see cref="T:System.Reflection.Emit.PropertyToken" /> to compare to the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the value of <paramref name="obj" /> is equal to the value of the current instance; otherwise, <see langword="false" />.</returns>
		public bool Equals(PropertyToken obj)
		{
			return this.tokValue == obj.tokValue;
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Emit.PropertyToken" /> structures are equal.</summary>
		/// <param name="a">The <see cref="T:System.Reflection.Emit.PropertyToken" /> to compare to <paramref name="b" />.</param>
		/// <param name="b">The <see cref="T:System.Reflection.Emit.PropertyToken" /> to compare to <paramref name="a" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(PropertyToken a, PropertyToken b)
		{
			return object.Equals(a, b);
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Emit.PropertyToken" /> structures are not equal.</summary>
		/// <param name="a">The <see cref="T:System.Reflection.Emit.PropertyToken" /> to compare to <paramref name="b" />.</param>
		/// <param name="b">The <see cref="T:System.Reflection.Emit.PropertyToken" /> to compare to <paramref name="a" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is not equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(PropertyToken a, PropertyToken b)
		{
			return !object.Equals(a, b);
		}

		/// <summary>Generates the hash code for this property.</summary>
		/// <returns>The hash code for this property.</returns>
		public override int GetHashCode()
		{
			return this.tokValue;
		}

		/// <summary>Retrieves the metadata token for this property.</summary>
		/// <returns>Read-only. Retrieves the metadata token for this instance.</returns>
		public int Token
		{
			get
			{
				return this.tokValue;
			}
		}

		internal readonly int tokValue;

		/// <summary>The default <see langword="PropertyToken" /> with <see cref="P:System.Reflection.Emit.PropertyToken.Token" /> value 0.</summary>
		public static readonly PropertyToken Empty;
	}
}
