using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	/// <summary>Represents the <see langword="Token" /> returned by the metadata to represent an event.</summary>
	[ComVisible(true)]
	[Serializable]
	public readonly struct EventToken : IEquatable<EventToken>
	{
		internal EventToken(int val)
		{
			this.tokValue = val;
		}

		/// <summary>Checks if the given object is an instance of <see langword="EventToken" /> and is equal to this instance.</summary>
		/// <param name="obj">The object to be compared with this instance.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is an instance of <see langword="EventToken" /> and equals the current instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			bool flag = obj is EventToken;
			if (flag)
			{
				EventToken eventToken = (EventToken)obj;
				flag = (this.tokValue == eventToken.tokValue);
			}
			return flag;
		}

		/// <summary>Indicates whether the current instance is equal to the specified <see cref="T:System.Reflection.Emit.EventToken" />.</summary>
		/// <param name="obj">The <see cref="T:System.Reflection.Emit.EventToken" /> to compare to the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the value of <paramref name="obj" /> is equal to the value of the current instance; otherwise, <see langword="false" />.</returns>
		public bool Equals(EventToken obj)
		{
			return this.tokValue == obj.tokValue;
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Emit.EventToken" /> structures are equal.</summary>
		/// <param name="a">The <see cref="T:System.Reflection.Emit.EventToken" /> to compare to <paramref name="b" />.</param>
		/// <param name="b">The <see cref="T:System.Reflection.Emit.EventToken" /> to compare to <paramref name="a" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(EventToken a, EventToken b)
		{
			return object.Equals(a, b);
		}

		/// <summary>Indicates whether two <see cref="T:System.Reflection.Emit.EventToken" /> structures are not equal.</summary>
		/// <param name="a">The <see cref="T:System.Reflection.Emit.EventToken" /> to compare to <paramref name="b" />.</param>
		/// <param name="b">The <see cref="T:System.Reflection.Emit.EventToken" /> to compare to <paramref name="a" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="a" /> is not equal to <paramref name="b" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(EventToken a, EventToken b)
		{
			return !object.Equals(a, b);
		}

		/// <summary>Generates the hash code for this event.</summary>
		/// <returns>The hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return this.tokValue;
		}

		/// <summary>Retrieves the metadata token for this event.</summary>
		/// <returns>Read-only. Retrieves the metadata token for this event.</returns>
		public int Token
		{
			get
			{
				return this.tokValue;
			}
		}

		internal readonly int tokValue;

		/// <summary>The default <see langword="EventToken" /> with <see cref="P:System.Reflection.Emit.EventToken.Token" /> value 0.</summary>
		public static readonly EventToken Empty;
	}
}
