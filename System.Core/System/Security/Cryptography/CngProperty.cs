using System;
using System.Security.Permissions;

namespace System.Security.Cryptography
{
	/// <summary>Encapsulates a property of a Cryptography Next Generation (CNG) key or provider.</summary>
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public struct CngProperty : IEquatable<CngProperty>
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Cryptography.CngProperty" /> class.</summary>
		/// <param name="name">The property name to initialize.</param>
		/// <param name="value">The property value to initialize.</param>
		/// <param name="options">A bitwise combination of the enumeration values that specify how the property is stored.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///         <paramref name="name" /> is <see langword="null" />.</exception>
		public CngProperty(string name, byte[] value, CngPropertyOptions options)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.m_name = name;
			this.m_propertyOptions = options;
			this.m_hashCode = null;
			if (value != null)
			{
				this.m_value = (value.Clone() as byte[]);
				return;
			}
			this.m_value = null;
		}

		/// <summary>Gets the property name that the current <see cref="T:System.Security.Cryptography.CngProperty" /> object specifies.</summary>
		/// <returns>The property name that is set in the current <see cref="T:System.Security.Cryptography.CngProperty" /> object.</returns>
		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		/// <summary>Gets the property options that the current <see cref="T:System.Security.Cryptography.CngProperty" /> object specifies.</summary>
		/// <returns>An object that specifies the options that are set in the current <see cref="T:System.Security.Cryptography.CngProperty" /> object.</returns>
		public CngPropertyOptions Options
		{
			get
			{
				return this.m_propertyOptions;
			}
		}

		internal byte[] Value
		{
			get
			{
				return this.m_value;
			}
		}

		/// <summary>Gets the property value that the current <see cref="T:System.Security.Cryptography.CngProperty" /> object specifies.</summary>
		/// <returns>An array that represents the value stored in the property.</returns>
		public byte[] GetValue()
		{
			byte[] result = null;
			if (this.m_value != null)
			{
				result = (this.m_value.Clone() as byte[]);
			}
			return result;
		}

		/// <summary>Determines whether two <see cref="T:System.Security.Cryptography.CngProperty" /> objects specify the same property name, value, and options.</summary>
		/// <param name="left">An object that specifies a property of a Cryptography Next Generation (CNG) key or provider.</param>
		/// <param name="right">A second object, to be compared to the object that is identified by the <paramref name="left" /> parameter.</param>
		/// <returns>
		///     <see langword="true" /> if the two objects specify the same property; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(CngProperty left, CngProperty right)
		{
			return left.Equals(right);
		}

		/// <summary>Determines whether two <see cref="T:System.Security.Cryptography.CngProperty" /> objects do not specify the same property name, value, and options.</summary>
		/// <param name="left">An object that specifies a property of a Cryptography Next Generation (CNG) key or provider.</param>
		/// <param name="right">A second object, to be compared to the object that is identified by the <paramref name="left" /> parameter.</param>
		/// <returns>
		///     <see langword="true" /> if the two objects do not specify the same property; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(CngProperty left, CngProperty right)
		{
			return !left.Equals(right);
		}

		/// <summary>Compares the specified object to the current <see cref="T:System.Security.Cryptography.CngProperty" /> object.</summary>
		/// <param name="obj">An object to be compared to the current <see cref="T:System.Security.Cryptography.CngProperty" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <paramref name="obj" /> parameter is a <see cref="T:System.Security.Cryptography.CngProperty" /> object that specifies the same property as the current object; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return obj != null && obj is CngProperty && this.Equals((CngProperty)obj);
		}

		/// <summary>Compares the specified <see cref="T:System.Security.Cryptography.CngProperty" /> object to the current <see cref="T:System.Security.Cryptography.CngProperty" /> object.</summary>
		/// <param name="other">An object to be compared to the current <see cref="T:System.Security.Cryptography.CngProperty" /> object.</param>
		/// <returns>
		///     <see langword="true" /> if the <paramref name="other" /> parameter represents the same property as the current object; otherwise, <see langword="false" />.</returns>
		public bool Equals(CngProperty other)
		{
			if (!string.Equals(this.Name, other.Name, StringComparison.Ordinal))
			{
				return false;
			}
			if (this.Options != other.Options)
			{
				return false;
			}
			if (this.m_value == null)
			{
				return other.m_value == null;
			}
			if (other.m_value == null)
			{
				return false;
			}
			if (this.m_value.Length != other.m_value.Length)
			{
				return false;
			}
			for (int i = 0; i < this.m_value.Length; i++)
			{
				if (this.m_value[i] != other.m_value[i])
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Generates a hash value for the current <see cref="T:System.Security.Cryptography.CngProperty" /> object.</summary>
		/// <returns>The hash value of the current <see cref="T:System.Security.Cryptography.CngProperty" /> object.</returns>
		public override int GetHashCode()
		{
			if (this.m_hashCode == null)
			{
				int num = this.Name.GetHashCode() ^ this.Options.GetHashCode();
				if (this.m_value != null)
				{
					for (int i = 0; i < this.m_value.Length; i++)
					{
						int num2 = (int)this.m_value[i] << i % 4 * 8;
						num ^= num2;
					}
				}
				this.m_hashCode = new int?(num);
			}
			return this.m_hashCode.Value;
		}

		private string m_name;

		private CngPropertyOptions m_propertyOptions;

		private byte[] m_value;

		private int? m_hashCode;
	}
}
