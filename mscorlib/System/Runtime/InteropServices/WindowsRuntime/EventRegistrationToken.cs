using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	/// <summary>A token that is returned when an event handler is added to a Windows Runtime event. The token is used to remove the event handler from the event at a later time.</summary>
	public struct EventRegistrationToken
	{
		internal EventRegistrationToken(ulong value)
		{
			this.m_value = value;
		}

		internal ulong Value
		{
			get
			{
				return this.m_value;
			}
		}

		/// <summary>Indicates whether two <see cref="T:System.Runtime.InteropServices.WindowsRuntime.EventRegistrationToken" /> instances are equal.</summary>
		/// <param name="left">The first instance to compare.</param>
		/// <param name="right">The second instance to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the two objects are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(EventRegistrationToken left, EventRegistrationToken right)
		{
			return left.Equals(right);
		}

		/// <summary>Indicates whether two <see cref="T:System.Runtime.InteropServices.WindowsRuntime.EventRegistrationToken" /> instances are not equal.</summary>
		/// <param name="left">The first instance to compare.</param>
		/// <param name="right">The second instance to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the two instances are not equal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(EventRegistrationToken left, EventRegistrationToken right)
		{
			return !left.Equals(right);
		}

		/// <summary>Returns a value that indicates whether the current object is equal to the specified object.</summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if the current object is equal to <paramref name="obj" />; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return obj is EventRegistrationToken && ((EventRegistrationToken)obj).Value == this.Value;
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>The hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return this.m_value.GetHashCode();
		}

		internal ulong m_value;
	}
}
