using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Use <see cref="T:System.Runtime.InteropServices.ComTypes.TYPEDESC" /> instead.</summary>
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.TYPEDESC instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct TYPEDESC
	{
		/// <summary>If the variable is <see langword="VT_SAFEARRAY" /> or <see langword="VT_PTR" />, the <see cref="F:System.Runtime.InteropServices.TYPEDESC.lpValue" /> field contains a pointer to a <see langword="TYPEDESC" /> that specifies the element type.</summary>
		public IntPtr lpValue;

		/// <summary>Indicates the variant type for the item described by this <see langword="TYPEDESC" />.</summary>
		public short vt;
	}
}
