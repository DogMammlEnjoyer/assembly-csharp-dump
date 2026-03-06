using System;

namespace System.Security.Cryptography
{
	/// <summary>Specifies Cryptography Next Generation (CNG) key property options.</summary>
	[Flags]
	public enum CngPropertyOptions
	{
		/// <summary>The referenced property has no options.</summary>
		None = 0,
		/// <summary>The property is not specified by CNG. Use this option to avoid future name conflicts with CNG properties.</summary>
		CustomProperty = 1073741824,
		/// <summary>The property should be persisted.</summary>
		Persist = -2147483648
	}
}
