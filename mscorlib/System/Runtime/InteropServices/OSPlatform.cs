using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Represents an operating system platform.</summary>
	public readonly struct OSPlatform : IEquatable<OSPlatform>
	{
		/// <summary>Gets an object that represents the Linux operating system.</summary>
		/// <returns>An object that represents the Linux operating system.</returns>
		public static OSPlatform Linux { get; } = new OSPlatform("LINUX");

		/// <summary>Gets an object that represents the OSX operating system.</summary>
		/// <returns>An object that represents the OSX operating system.</returns>
		public static OSPlatform OSX { get; } = new OSPlatform("OSX");

		/// <summary>Gets an object that represents the Windows operating system.</summary>
		/// <returns>An object that represents the Windows operating system.</returns>
		public static OSPlatform Windows { get; } = new OSPlatform("WINDOWS");

		private OSPlatform(string osPlatform)
		{
			if (osPlatform == null)
			{
				throw new ArgumentNullException("osPlatform");
			}
			if (osPlatform.Length == 0)
			{
				throw new ArgumentException("Value cannot be empty.", "osPlatform");
			}
			this._osPlatform = osPlatform;
		}

		/// <summary>Creates a new <see cref="T:System.Runtime.InteropServices.OSPlatform" /> instance.</summary>
		/// <param name="osPlatform">The name of the platform that this instance represents.</param>
		/// <returns>An object that represents the <paramref name="osPlatform" /> operating system.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="osPlatform" /> is an empty string.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="osPlatform" /> is <see langword="null" />.</exception>
		public static OSPlatform Create(string osPlatform)
		{
			return new OSPlatform(osPlatform);
		}

		/// <summary>Determines whether the current instance and the specified <see cref="T:System.Runtime.InteropServices.OSPlatform" /> instance are equal.</summary>
		/// <param name="other">The object to compare with the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the current instance and <paramref name="other" /> are equal; otherwise, <see langword="false" />.</returns>
		public bool Equals(OSPlatform other)
		{
			return this.Equals(other._osPlatform);
		}

		internal bool Equals(string other)
		{
			return string.Equals(this._osPlatform, other, StringComparison.Ordinal);
		}

		/// <summary>Determines whether the current <see cref="T:System.Runtime.InteropServices.OSPlatform" /> instance is equal to the specified object.</summary>
		/// <param name="obj">
		///   <see langword="true" /> if <paramref name="obj" /> is a <see cref="T:System.Runtime.InteropServices.OSPlatform" /> instance and its name is the same as the current object; otherwise, false.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is a <see cref="T:System.Runtime.InteropServices.OSPlatform" /> instance and its name is the same as the current object.</returns>
		public override bool Equals(object obj)
		{
			return obj is OSPlatform && this.Equals((OSPlatform)obj);
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>The hash code for this instance.</returns>
		public override int GetHashCode()
		{
			if (this._osPlatform != null)
			{
				return this._osPlatform.GetHashCode();
			}
			return 0;
		}

		/// <summary>Returns the string representation of this <see cref="T:System.Runtime.InteropServices.OSPlatform" /> instance.</summary>
		/// <returns>A string that represents this <see cref="T:System.Runtime.InteropServices.OSPlatform" /> instance.</returns>
		public override string ToString()
		{
			return this._osPlatform ?? string.Empty;
		}

		/// <summary>Determines whether two <see cref="T:System.Runtime.InteropServices.OSPlatform" /> objects are equal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(OSPlatform left, OSPlatform right)
		{
			return left.Equals(right);
		}

		/// <summary>Determines whether two <see cref="T:System.Runtime.InteropServices.OSPlatform" /> instances are unequal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are unequal; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(OSPlatform left, OSPlatform right)
		{
			return !(left == right);
		}

		private readonly string _osPlatform;
	}
}
