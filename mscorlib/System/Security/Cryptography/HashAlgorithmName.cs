using System;

namespace System.Security.Cryptography
{
	/// <summary>Specifies the name of a cryptographic hash algorithm.</summary>
	public readonly struct HashAlgorithmName : IEquatable<HashAlgorithmName>
	{
		/// <summary>Gets a hash algorithm name that represents "MD5".</summary>
		/// <returns>A hash algorithm name that represents "MD5".</returns>
		public static HashAlgorithmName MD5
		{
			get
			{
				return new HashAlgorithmName("MD5");
			}
		}

		/// <summary>Gets a hash algorithm name that represents "SHA1".</summary>
		/// <returns>A hash algorithm name that represents "SHA1".</returns>
		public static HashAlgorithmName SHA1
		{
			get
			{
				return new HashAlgorithmName("SHA1");
			}
		}

		/// <summary>Gets a hash algorithm name that represents "SHA256".</summary>
		/// <returns>A hash algorithm name that represents "SHA256".</returns>
		public static HashAlgorithmName SHA256
		{
			get
			{
				return new HashAlgorithmName("SHA256");
			}
		}

		/// <summary>Gets a hash algorithm name that represents "SHA384".</summary>
		/// <returns>A hash algorithm name that represents "SHA384".</returns>
		public static HashAlgorithmName SHA384
		{
			get
			{
				return new HashAlgorithmName("SHA384");
			}
		}

		/// <summary>Gets a hash algorithm name that represents "SHA512".</summary>
		/// <returns>A hash algorithm name that represents "SHA512".</returns>
		public static HashAlgorithmName SHA512
		{
			get
			{
				return new HashAlgorithmName("SHA512");
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.HashAlgorithmName" /> structure with a custom name.</summary>
		/// <param name="name">The custom hash algorithm name.</param>
		public HashAlgorithmName(string name)
		{
			this._name = name;
		}

		/// <summary>Gets the underlying string representation of the algorithm name.</summary>
		/// <returns>The string representation of the algorithm name, or <see langword="null" /> or <see cref="F:System.String.Empty" /> if no hash algorithm is available.</returns>
		public string Name
		{
			get
			{
				return this._name;
			}
		}

		/// <summary>Returns the string representation of the current <see cref="T:System.Security.Cryptography.HashAlgorithmName" /> instance.</summary>
		/// <returns>The string representation of the current <see cref="T:System.Security.Cryptography.HashAlgorithmName" /> instance.</returns>
		public override string ToString()
		{
			return this._name ?? string.Empty;
		}

		/// <summary>Returns a value that indicates whether the current instance and a specified object are equal.</summary>
		/// <param name="obj">The object to compare with the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is a <see cref="T:System.Security.Cryptography.HashAlgorithmName" /> object and its <see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> property is equal to that of the current instance. The comparison is ordinal and case-sensitive.</returns>
		public override bool Equals(object obj)
		{
			return obj is HashAlgorithmName && this.Equals((HashAlgorithmName)obj);
		}

		/// <summary>Returns a value that indicates whether two <see cref="T:System.Security.Cryptography.HashAlgorithmName" /> instances are equal.</summary>
		/// <param name="other">The object to compare with the current instance.</param>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> property of <paramref name="other" /> is equal to that of the current instance. The comparison is ordinal and case-sensitive.</returns>
		public bool Equals(HashAlgorithmName other)
		{
			return this._name == other._name;
		}

		/// <summary>Returns the hash code for the current instance.</summary>
		/// <returns>The hash code for the current instance, or 0 if no <paramref name="name" /> value was supplied to the <see cref="T:System.Security.Cryptography.HashAlgorithmName" /> constructor.</returns>
		public override int GetHashCode()
		{
			if (this._name != null)
			{
				return this._name.GetHashCode();
			}
			return 0;
		}

		/// <summary>Determines whether two specified <see cref="T:System.Security.Cryptography.HashAlgorithmName" /> objects are equal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if both <paramref name="left" /> and <paramref name="right" /> have the same <see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> value; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(HashAlgorithmName left, HashAlgorithmName right)
		{
			return left.Equals(right);
		}

		/// <summary>Determines whether two specified <see cref="T:System.Security.Cryptography.HashAlgorithmName" /> objects are not equal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if both <paramref name="left" /> and <paramref name="right" /> do not have the same <see cref="P:System.Security.Cryptography.HashAlgorithmName.Name" /> value; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(HashAlgorithmName left, HashAlgorithmName right)
		{
			return !(left == right);
		}

		private readonly string _name;
	}
}
