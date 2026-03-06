using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	/// <summary>Represents a label in the instruction stream. <see langword="Label" /> is used in conjunction with the <see cref="T:System.Reflection.Emit.ILGenerator" /> class.</summary>
	[ComVisible(true)]
	[Serializable]
	public readonly struct Label : IEquatable<Label>
	{
		internal Label(int val)
		{
			this.label = val;
		}

		/// <summary>Checks if the given object is an instance of <see langword="Label" /> and is equal to this instance.</summary>
		/// <param name="obj">The object to compare with this <see langword="Label" /> instance.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is an instance of <see langword="Label" /> and is equal to this object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			bool flag = obj is Label;
			if (flag)
			{
				Label label = (Label)obj;
				flag = (this.label == label.label);
			}
			return flag;
		}

		/// <summary>Indicates whether the current instance is equal to the specified <see cref="T:System.Reflection.Emit.Label" />.</summary>
		/// <param name="obj">The <see cref="T:System.Reflection.Emit.Label" /> to compare to the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the value of <paramref name="obj" /> is equal to the value of the current instance; otherwise, <see langword="false" />.</returns>
		public bool Equals(Label obj)
		{
			return this.label == obj.label;
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Emit.Label" /> structures are equal.</summary>
		/// <param name="a">The <see cref="T:System.Reflection.Emit.Label" /> to compare to <paramref name="b" />.</param>
		/// <param name="b">The <see cref="T:System.Reflection.Emit.Label" /> to compare to <paramref name="a" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(Label a, Label b)
		{
			return a.Equals(b);
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Emit.Label" /> structures are not equal.</summary>
		/// <param name="a">The <see cref="T:System.Reflection.Emit.Label" /> to compare to <paramref name="b" />.</param>
		/// <param name="b">The <see cref="T:System.Reflection.Emit.Label" /> to compare to <paramref name="a" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is not equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(Label a, Label b)
		{
			return !(a == b);
		}

		/// <summary>Generates a hash code for this instance.</summary>
		/// <returns>A hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return this.label.GetHashCode();
		}

		internal readonly int label;
	}
}
