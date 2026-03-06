using System;

namespace System.Runtime.InteropServices.ComTypes
{
	/// <summary>Describes the type of a variable, return type of a function, or the type of a function parameter.</summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct TYPEDESC
	{
		/// <summary>If the variable is <see langword="VT_SAFEARRAY" /> or <see langword="VT_PTR" />, the <see langword="lpValue" /> field contains a pointer to a <see langword="TYPEDESC" /> that specifies the element type.</summary>
		public IntPtr lpValue;

		/// <summary>Indicates the variant type for the item described by this <see langword="TYPEDESC" />.</summary>
		public short vt;
	}
}
