using System;
using Internal.Cryptography;

namespace System.Security.Cryptography
{
	/// <summary>Represents a cryptographic object identifier. This class cannot be inherited.</summary>
	public sealed class Oid
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Oid" /> class.</summary>
		public Oid()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Oid" /> class using a string value of an <see cref="T:System.Security.Cryptography.Oid" /> object.</summary>
		/// <param name="oid">An object identifier.</param>
		public Oid(string oid)
		{
			string text = OidLookup.ToOid(oid, OidGroup.All, false);
			if (text == null)
			{
				text = oid;
			}
			this.Value = text;
			this._group = OidGroup.All;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Oid" /> class using the specified value and friendly name.</summary>
		/// <param name="value">The dotted number of the identifier.</param>
		/// <param name="friendlyName">The friendly name of the identifier.</param>
		public Oid(string value, string friendlyName)
		{
			this._value = value;
			this._friendlyName = friendlyName;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.Oid" /> class using the specified <see cref="T:System.Security.Cryptography.Oid" /> object.</summary>
		/// <param name="oid">The object identifier information to use to create the new object identifier.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="oid" /> is <see langword="null" />.</exception>
		public Oid(Oid oid)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid");
			}
			this._value = oid._value;
			this._friendlyName = oid._friendlyName;
			this._group = oid._group;
		}

		/// <summary>Creates an <see cref="T:System.Security.Cryptography.Oid" /> object from an OID friendly name by searching the specified group.</summary>
		/// <param name="friendlyName">The friendly name of the identifier.</param>
		/// <param name="group">The group to search in.</param>
		/// <returns>An object that represents the specified OID.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="friendlyName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The OID was not found.</exception>
		public static Oid FromFriendlyName(string friendlyName, OidGroup group)
		{
			if (friendlyName == null)
			{
				throw new ArgumentNullException("friendlyName");
			}
			string text = OidLookup.ToOid(friendlyName, group, false);
			if (text == null)
			{
				throw new CryptographicException("No OID value matches this name.");
			}
			return new Oid(text, friendlyName, group);
		}

		/// <summary>Creates an <see cref="T:System.Security.Cryptography.Oid" /> object by using the specified OID value and group.</summary>
		/// <param name="oidValue">The OID value.</param>
		/// <param name="group">The group to search in.</param>
		/// <returns>A new instance of an <see cref="T:System.Security.Cryptography.Oid" /> object.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="oidValue" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Security.Cryptography.CryptographicException">The friendly name for the OID value was not found.</exception>
		public static Oid FromOidValue(string oidValue, OidGroup group)
		{
			if (oidValue == null)
			{
				throw new ArgumentNullException("oidValue");
			}
			string text = OidLookup.ToFriendlyName(oidValue, group, false);
			if (text == null)
			{
				throw new CryptographicException("The OID value is invalid.");
			}
			return new Oid(oidValue, text, group);
		}

		/// <summary>Gets or sets the dotted number of the identifier.</summary>
		/// <returns>The dotted number of the identifier.</returns>
		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		/// <summary>Gets or sets the friendly name of the identifier.</summary>
		/// <returns>The friendly name of the identifier.</returns>
		public string FriendlyName
		{
			get
			{
				if (this._friendlyName == null && this._value != null)
				{
					this._friendlyName = OidLookup.ToFriendlyName(this._value, this._group, true);
				}
				return this._friendlyName;
			}
			set
			{
				this._friendlyName = value;
				if (this._friendlyName != null)
				{
					string text = OidLookup.ToOid(this._friendlyName, this._group, true);
					if (text != null)
					{
						this._value = text;
					}
				}
			}
		}

		private Oid(string value, string friendlyName, OidGroup group)
		{
			this._value = value;
			this._friendlyName = friendlyName;
			this._group = group;
		}

		private string _value;

		private string _friendlyName;

		private OidGroup _group;
	}
}
