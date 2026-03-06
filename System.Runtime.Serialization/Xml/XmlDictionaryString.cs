using System;
using System.Runtime.Serialization;
using System.Text;

namespace System.Xml
{
	/// <summary>Represents an entry stored in a <see cref="T:System.Xml.XmlDictionary" />.</summary>
	public class XmlDictionaryString
	{
		/// <summary>Creates an instance of this class.</summary>
		/// <param name="dictionary">The <see cref="T:System.Xml.IXmlDictionary" /> containing this instance.</param>
		/// <param name="value">The string that is the value of the dictionary entry.</param>
		/// <param name="key">The integer that is the key of the dictionary entry.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="dictionary" /> or <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="key" /> is less than 0 or greater than <see cref="F:System.Int32.MaxValue" /> / 4.</exception>
		public XmlDictionaryString(IXmlDictionary dictionary, string value, int key)
		{
			if (dictionary == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("dictionary"));
			}
			if (value == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
			}
			if (key < 0 || key > 536870911)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("key", System.Runtime.Serialization.SR.GetString("The value of this argument must fall within the range {0} to {1}.", new object[]
				{
					0,
					536870911
				})));
			}
			this.dictionary = dictionary;
			this.value = value;
			this.key = key;
		}

		internal static string GetString(XmlDictionaryString s)
		{
			if (s == null)
			{
				return null;
			}
			return s.Value;
		}

		/// <summary>Gets an <see cref="T:System.Xml.XmlDictionaryString" /> representing the empty string.</summary>
		/// <returns>An <see cref="T:System.Xml.XmlDictionaryString" /> representing the empty string.</returns>
		public static XmlDictionaryString Empty
		{
			get
			{
				return XmlDictionaryString.emptyStringDictionary.EmptyString;
			}
		}

		/// <summary>Represents the <see cref="T:System.Xml.IXmlDictionary" /> passed to the constructor of this instance of <see cref="T:System.Xml.XmlDictionaryString" />.</summary>
		/// <returns>The <see cref="T:System.Xml.IXmlDictionary" /> for this dictionary entry.</returns>
		public IXmlDictionary Dictionary
		{
			get
			{
				return this.dictionary;
			}
		}

		/// <summary>Gets the integer key for this instance of the class.</summary>
		/// <returns>The integer key for this instance of the class.</returns>
		public int Key
		{
			get
			{
				return this.key;
			}
		}

		/// <summary>Gets the string value for this instance of the class.</summary>
		/// <returns>The string value for this instance of the class.</returns>
		public string Value
		{
			get
			{
				return this.value;
			}
		}

		internal byte[] ToUTF8()
		{
			if (this.buffer == null)
			{
				this.buffer = Encoding.UTF8.GetBytes(this.value);
			}
			return this.buffer;
		}

		/// <summary>Displays a text representation of this object.</summary>
		/// <returns>The string value for this instance of the class.</returns>
		public override string ToString()
		{
			return this.value;
		}

		internal const int MinKey = 0;

		internal const int MaxKey = 536870911;

		private IXmlDictionary dictionary;

		private string value;

		private int key;

		private byte[] buffer;

		private static XmlDictionaryString.EmptyStringDictionary emptyStringDictionary = new XmlDictionaryString.EmptyStringDictionary();

		private class EmptyStringDictionary : IXmlDictionary
		{
			public EmptyStringDictionary()
			{
				this.empty = new XmlDictionaryString(this, string.Empty, 0);
			}

			public XmlDictionaryString EmptyString
			{
				get
				{
					return this.empty;
				}
			}

			public bool TryLookup(string value, out XmlDictionaryString result)
			{
				if (value == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
				}
				if (value.Length == 0)
				{
					result = this.empty;
					return true;
				}
				result = null;
				return false;
			}

			public bool TryLookup(int key, out XmlDictionaryString result)
			{
				if (key == 0)
				{
					result = this.empty;
					return true;
				}
				result = null;
				return false;
			}

			public bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
			{
				if (value == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
				}
				if (value.Dictionary != this)
				{
					result = null;
					return false;
				}
				result = value;
				return true;
			}

			private XmlDictionaryString empty;
		}
	}
}
