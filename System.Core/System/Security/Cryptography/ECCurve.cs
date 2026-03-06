using System;
using System.Diagnostics;

namespace System.Security.Cryptography
{
	/// <summary>Represents an elliptic curve.</summary>
	[DebuggerDisplay("ECCurve: {Oid}")]
	public struct ECCurve
	{
		/// <summary>Gets the identifier of a named curve.</summary>
		/// <returns>The identifier of a named curve.</returns>
		public Oid Oid
		{
			get
			{
				return new Oid(this._oid.Value, this._oid.FriendlyName);
			}
			private set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Oid");
				}
				if (string.IsNullOrEmpty(value.Value) && string.IsNullOrEmpty(value.FriendlyName))
				{
					throw new ArgumentException(string.Format("The specified Oid is not valid. The Oid.FriendlyName or Oid.Value property must be set.", Array.Empty<object>()));
				}
				this._oid = value;
			}
		}

		private static ECCurve Create(Oid oid)
		{
			return new ECCurve
			{
				CurveType = ECCurve.ECCurveType.Named,
				Oid = oid
			};
		}

		/// <summary>Creates a named curve using the specified <see cref="T:System.Security.Cryptography.Oid" /> object.</summary>
		/// <param name="curveOid">The object identifier to use.</param>
		/// <returns>An object representing the named curve.</returns>
		public static ECCurve CreateFromOid(Oid curveOid)
		{
			return ECCurve.Create(new Oid(curveOid.Value, curveOid.FriendlyName));
		}

		/// <summary>Creates a named curve using the specified friendly name of the identifier.</summary>
		/// <param name="oidFriendlyName">The friendly name of the identifier.</param>
		/// <returns>An object representing the named curve.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="oidFriendlyName" /> is <see langword="null" />.</exception>
		public static ECCurve CreateFromFriendlyName(string oidFriendlyName)
		{
			if (oidFriendlyName == null)
			{
				throw new ArgumentNullException("oidFriendlyName");
			}
			return ECCurve.CreateFromValueAndName(null, oidFriendlyName);
		}

		/// <summary>Creates a named curve using the specified dotted-decimal representation of the identifier.</summary>
		/// <param name="oidValue">The dotted number of the identifier.</param>
		/// <returns>An object representing the named curve.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="oidValue" /> is <see langword="null" />.</exception>
		public static ECCurve CreateFromValue(string oidValue)
		{
			if (oidValue == null)
			{
				throw new ArgumentNullException("oidValue");
			}
			return ECCurve.CreateFromValueAndName(oidValue, null);
		}

		private static ECCurve CreateFromValueAndName(string oidValue, string oidFriendlyName)
		{
			return ECCurve.Create(new Oid(oidValue, oidFriendlyName));
		}

		/// <summary>Gets a value that indicates whether the curve type indicates an explicit prime curve.</summary>
		/// <returns>
		///     <see langword="true" /> if the curve is an explicit prime curve; <see langword="false" /> if the curve is a named prime, characteristic 2 or implicit curves.</returns>
		public bool IsPrime
		{
			get
			{
				return this.CurveType == ECCurve.ECCurveType.PrimeShortWeierstrass || this.CurveType == ECCurve.ECCurveType.PrimeMontgomery || this.CurveType == ECCurve.ECCurveType.PrimeTwistedEdwards;
			}
		}

		/// <summary>Gets a value that indicates whether the curve type indicates an explicit characteristic 2 curve.</summary>
		/// <returns>
		///     <see langword="true" /> if the curve is an explicit characteristic 2 curve; <see langword="false" /> if the curve is a named characteristic 2, prime, or implicit curve.</returns>
		public bool IsCharacteristic2
		{
			get
			{
				return this.CurveType == ECCurve.ECCurveType.Characteristic2;
			}
		}

		/// <summary>Gets a value that indicates whether the curve type indicates an explicit curve (either prime or characteristic 2).</summary>
		/// <returns>
		///     <see langword="true" /> if the curve is an explicit curve (either prime or characteristic 2); <see langword="false" /> if the curve is a named or implicit curve.</returns>
		public bool IsExplicit
		{
			get
			{
				return this.IsPrime || this.IsCharacteristic2;
			}
		}

		/// <summary>Gets a value that indicates whether the curve type indicates a named curve.</summary>
		/// <returns>
		///     <see langword="true" /> if the curve is a named curve; <see langword="false" /> if the curve is an implict or an  explicit curve (either prime or characteristic 2).</returns>
		public bool IsNamed
		{
			get
			{
				return this.CurveType == ECCurve.ECCurveType.Named;
			}
		}

		/// <summary>Validates the integrity of the current curve. Throws a <see cref="T:System.Security.Cryptography.CryptographicException" /> exception if the structure is not valid.</summary>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The curve parameters are not valid for the current curve type.</exception>
		public void Validate()
		{
			if (this.IsNamed)
			{
				if (this.HasAnyExplicitParameters())
				{
					throw new CryptographicException("The specified named curve parameters are not valid. Only the Oid parameter must be set.");
				}
				if (this.Oid == null || (string.IsNullOrEmpty(this.Oid.FriendlyName) && string.IsNullOrEmpty(this.Oid.Value)))
				{
					throw new CryptographicException("The specified Oid is not valid. The Oid.FriendlyName or Oid.Value property must be set.");
				}
			}
			else if (this.IsExplicit)
			{
				bool flag = false;
				if (this.A == null || this.B == null || this.B.Length != this.A.Length || this.G.X == null || this.G.X.Length != this.A.Length || this.G.Y == null || this.G.Y.Length != this.A.Length || this.Order == null || this.Order.Length == 0 || this.Cofactor == null || this.Cofactor.Length == 0)
				{
					flag = true;
				}
				if (this.IsPrime)
				{
					if (!flag && (this.Prime == null || this.Prime.Length != this.A.Length))
					{
						flag = true;
					}
					if (flag)
					{
						throw new CryptographicException("The specified prime curve parameters are not valid. Prime, A, B, G.X, G.Y and Order are required and must be the same length, and the same length as Q.X, Q.Y and D if those are specified. Seed, Cofactor and Hash are optional. Other parameters are not allowed.");
					}
				}
				else if (this.IsCharacteristic2)
				{
					if (!flag && (this.Polynomial == null || this.Polynomial.Length == 0))
					{
						flag = true;
					}
					if (flag)
					{
						throw new CryptographicException("The specified Characteristic2 curve parameters are not valid. Polynomial, A, B, G.X, G.Y, and Order are required. A, B, G.X, G.Y must be the same length, and the same length as Q.X, Q.Y and D if those are specified. Seed, Cofactor and Hash are optional. Other parameters are not allowed.");
					}
				}
			}
			else if (this.HasAnyExplicitParameters() || this.Oid != null)
			{
				throw new CryptographicException(string.Format("The specified curve '{0}' or its parameters are not valid for this platform.", this.CurveType.ToString()));
			}
		}

		private bool HasAnyExplicitParameters()
		{
			return this.A != null || this.B != null || this.G.X != null || this.G.Y != null || this.Order != null || this.Cofactor != null || this.Prime != null || this.Polynomial != null || this.Seed != null || this.Hash != null;
		}

		/// <summary>The first coefficient for an explicit curve. A for short Weierstrass, Montgomery, and Twisted Edwards curves.</summary>
		/// <returns>Coefficient A.</returns>
		public byte[] A;

		/// <summary>The second coefficient for an explicit curve. B for short Weierstrass and d for Twisted Edwards curves.</summary>
		/// <returns>Coefficient B.</returns>
		public byte[] B;

		/// <summary>The generator, or base point, for operations on the curve.</summary>
		/// <returns>The base point.</returns>
		public ECPoint G;

		/// <summary>The order of the curve. Applies only to explicit curves.</summary>
		/// <returns>The order of the curve. </returns>
		public byte[] Order;

		/// <summary>The cofactor of the curve.</summary>
		/// <returns>The cofactor of the curve.</returns>
		public byte[] Cofactor;

		/// <summary>The seed value for coefficient generation under the ANSI X9.62 generation algorithm. Applies only to explicit curves.</summary>
		/// <returns>The seed value.</returns>
		public byte[] Seed;

		/// <summary>Identifies the composition of the <see cref="T:System.Security.Cryptography.ECCurve" /> object.</summary>
		/// <returns>The curve type.</returns>
		public ECCurve.ECCurveType CurveType;

		/// <summary>The name of the hash algorithm which was used to generate the curve coefficients (<see cref="F:System.Security.Cryptography.ECCurve.A" /> and <see cref="F:System.Security.Cryptography.ECCurve.B" />) from the <see cref="F:System.Security.Cryptography.ECCurve.Seed" /> under the ANSI X9.62 generation algorithm. Applies only to explicit curves.</summary>
		/// <returns>The name of the hash algorithm used to generate the curve coefficients.</returns>
		public HashAlgorithmName? Hash;

		/// <summary>The curve polynomial. Applies only to characteristic 2 curves.</summary>
		/// <returns>The curve polynomial.</returns>
		public byte[] Polynomial;

		/// <summary>The prime specifying the base field. Applies only to prime curves.</summary>
		/// <returns>The prime P.</returns>
		public byte[] Prime;

		private Oid _oid;

		/// <summary>Indicates how to interpret the data contained in an <see cref="T:System.Security.Cryptography.ECCurve" /> object.</summary>
		public enum ECCurveType
		{
			/// <summary>No curve data is interpreted. The caller is assumed to know what the curve is.</summary>
			Implicit,
			/// <summary>The curve parameters represent a prime curve with the formula y^2 = x^3 + A*x + B in the prime field P.</summary>
			PrimeShortWeierstrass,
			/// <summary>The curve parameters represent a prime curve with the formula A*x^2 + y^2 = 1 + B*x^2*y^2 in the prime field P.</summary>
			PrimeTwistedEdwards,
			/// <summary>The curve parameters represent a prime curve with the formula B*y^2 = x^3 + A*x^2 + x.</summary>
			PrimeMontgomery,
			/// <summary>The curve parameters represent a characteristic 2 curve.</summary>
			Characteristic2,
			/// <summary>The curve parameters represent a named curve.</summary>
			Named
		}

		/// <summary>Represents a factory class for creating named curves.</summary>
		public static class NamedCurves
		{
			/// <summary>Gets a brainpoolP160r1 named curve.</summary>
			/// <returns>A brainpoolP160r1 named curve.</returns>
			public static ECCurve brainpoolP160r1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP160r1");
				}
			}

			/// <summary>Gets a brainpoolP160t1 named curve.</summary>
			/// <returns>A brainpoolP160t1 named curve.</returns>
			public static ECCurve brainpoolP160t1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP160t1");
				}
			}

			/// <summary>Gets a brainpoolP192r1 named curve.</summary>
			/// <returns>A brainpoolP192r1 named curve.</returns>
			public static ECCurve brainpoolP192r1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP192r1");
				}
			}

			/// <summary>Gets a brainpoolP192t1 named curve.</summary>
			/// <returns>A brainpoolP192t1 named curve.</returns>
			public static ECCurve brainpoolP192t1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP192t1");
				}
			}

			/// <summary>Gets a brainpoolP224r1 named curve.</summary>
			/// <returns>A brainpoolP224r1 named curve.</returns>
			public static ECCurve brainpoolP224r1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP224r1");
				}
			}

			/// <summary>Gets a brainpoolP224t1 named curve.</summary>
			/// <returns>A brainpoolP224t1 named curve.</returns>
			public static ECCurve brainpoolP224t1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP224t1");
				}
			}

			/// <summary>Gets a brainpoolP256r1 named curve.</summary>
			/// <returns>A brainpoolP256r1 named curve.</returns>
			public static ECCurve brainpoolP256r1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP256r1");
				}
			}

			/// <summary>Gets a brainpoolP256t1 named curve.</summary>
			/// <returns>A brainpoolP256t1 named curve.</returns>
			public static ECCurve brainpoolP256t1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP256t1");
				}
			}

			/// <summary>Gets a brainpoolP320r1 named curve.</summary>
			/// <returns>A brainpoolP320r1 named curve.</returns>
			public static ECCurve brainpoolP320r1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP320r1");
				}
			}

			/// <summary>Gets a brainpoolP320t1 named curve.</summary>
			/// <returns>A brainpoolP320t1 named curve.</returns>
			public static ECCurve brainpoolP320t1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP320t1");
				}
			}

			/// <summary>Gets a brainpoolP384r1 named curve.</summary>
			/// <returns>A brainpoolP384r1 named curve.</returns>
			public static ECCurve brainpoolP384r1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP384r1");
				}
			}

			/// <summary>Gets a brainpoolP384t1 named curve.</summary>
			/// <returns>A brainpoolP384t1 named curve.</returns>
			public static ECCurve brainpoolP384t1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP384t1");
				}
			}

			/// <summary>Gets a brainpoolP512r1 named curve.</summary>
			/// <returns>A brainpoolP512r1 named curve.</returns>
			public static ECCurve brainpoolP512r1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP512r1");
				}
			}

			/// <summary>Gets a brainpoolP512t1 named curve.</summary>
			/// <returns>A brainpoolP512t1 named curve.</returns>
			public static ECCurve brainpoolP512t1
			{
				get
				{
					return ECCurve.CreateFromFriendlyName("brainpoolP512t1");
				}
			}

			/// <summary>Gets a nistP256 named curve.</summary>
			/// <returns>A nistP256 named curve.</returns>
			public static ECCurve nistP256
			{
				get
				{
					return ECCurve.CreateFromValueAndName("1.2.840.10045.3.1.7", "nistP256");
				}
			}

			/// <summary>Gets a nistP384 named curve.</summary>
			/// <returns>A nistP384 named curve.</returns>
			public static ECCurve nistP384
			{
				get
				{
					return ECCurve.CreateFromValueAndName("1.3.132.0.34", "nistP384");
				}
			}

			/// <summary>Gets a nistP521 named curve.</summary>
			/// <returns>A nistP521 named curve.</returns>
			public static ECCurve nistP521
			{
				get
				{
					return ECCurve.CreateFromValueAndName("1.3.132.0.35", "nistP521");
				}
			}

			private const string ECDSA_P256_OID_VALUE = "1.2.840.10045.3.1.7";

			private const string ECDSA_P384_OID_VALUE = "1.3.132.0.34";

			private const string ECDSA_P521_OID_VALUE = "1.3.132.0.35";
		}
	}
}
