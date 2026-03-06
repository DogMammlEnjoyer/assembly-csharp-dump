using System;

namespace System.Diagnostics.Tracing
{
	/// <summary>Specifies how to format the value of a user-defined type and can be used to override the default formatting for a field.</summary>
	public enum EventFieldFormat
	{
		/// <summary>Boolean</summary>
		Boolean = 3,
		/// <summary>Default.</summary>
		Default = 0,
		/// <summary>Hexadecimal.</summary>
		Hexadecimal = 4,
		/// <summary>HResult.</summary>
		HResult = 15,
		/// <summary>JSON.</summary>
		Json = 12,
		/// <summary>String.</summary>
		String = 2,
		/// <summary>XML.</summary>
		Xml = 11
	}
}
