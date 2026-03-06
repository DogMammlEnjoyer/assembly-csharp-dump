using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	/// <summary>Wraps an XML <see langword="language" /> type.</summary>
	[ComVisible(true)]
	[Serializable]
	public sealed class SoapLanguage : ISoapXsd
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Remoting.Metadata.W3cXsd2001.SoapLanguage" /> class.</summary>
		public SoapLanguage()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Remoting.Metadata.W3cXsd2001.SoapLanguage" /> class with the language identifier value of <see langword="language" /> attribute.</summary>
		/// <param name="value">A <see cref="T:System.String" /> that contains the language identifier value of a <see langword="language" /> attribute.</param>
		public SoapLanguage(string value)
		{
			this._value = SoapHelper.Normalize(value);
		}

		/// <summary>Gets or sets the language identifier of a <see langword="language" /> attribute.</summary>
		/// <returns>A <see cref="T:System.String" /> that contains the language identifier of a <see langword="language" /> attribute.</returns>
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

		/// <summary>Gets the XML Schema definition language (XSD) of the current SOAP type.</summary>
		/// <returns>A <see cref="T:System.String" /> that indicates the XSD of the current SOAP type.</returns>
		public static string XsdType
		{
			get
			{
				return "language";
			}
		}

		/// <summary>Returns the XML Schema definition language (XSD) of the current SOAP type.</summary>
		/// <returns>A <see cref="T:System.String" /> that indicates the XSD of the current SOAP type.</returns>
		public string GetXsdType()
		{
			return SoapLanguage.XsdType;
		}

		/// <summary>Converts the specified <see cref="T:System.String" /> into a <see cref="T:System.Runtime.Remoting.Metadata.W3cXsd2001.SoapLanguage" /> object.</summary>
		/// <param name="value">The <see langword="String" /> to convert.</param>
		/// <returns>A <see cref="T:System.Runtime.Remoting.Metadata.W3cXsd2001.SoapLanguage" /> object that is obtained from <paramref name="value" />.</returns>
		public static SoapLanguage Parse(string value)
		{
			return new SoapLanguage(value);
		}

		/// <summary>Returns <see cref="P:System.Runtime.Remoting.Metadata.W3cXsd2001.SoapLanguage.Value" /> as a <see cref="T:System.String" />.</summary>
		/// <returns>A <see cref="T:System.Runtime.Remoting.Metadata.W3cXsd2001.SoapLanguage" /> object that is obtained from <see cref="P:System.Runtime.Remoting.Metadata.W3cXsd2001.SoapLanguage.Value" />.</returns>
		public override string ToString()
		{
			return this._value;
		}

		private string _value;
	}
}
