using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Use <see cref="T:System.Runtime.InteropServices.ComTypes.CONNECTDATA" /> instead.</summary>
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.CONNECTDATA instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct CONNECTDATA
	{
		/// <summary>Represents a pointer to the <see langword="IUnknown" /> interface on a connected advisory sink. The caller must call <see langword="IUnknown::Release" /> on this pointer when the <see langword="CONNECTDATA" /> structure is no longer needed.</summary>
		[MarshalAs(UnmanagedType.Interface)]
		public object pUnk;

		/// <summary>Represents a connection token that is returned from a call to <see cref="M:System.Runtime.InteropServices.UCOMIConnectionPoint.Advise(System.Object,System.Int32@)" />.</summary>
		public int dwCookie;
	}
}
