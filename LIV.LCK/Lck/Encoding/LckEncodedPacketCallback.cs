using System;

namespace Liv.Lck.Encoding
{
	internal struct LckEncodedPacketCallback
	{
		public IntPtr CallbackObjectPtr { readonly get; set; }

		public IntPtr CallbackFunctionPtr { readonly get; set; }

		public bool IsValid
		{
			get
			{
				return this.CallbackObjectPtr != IntPtr.Zero && this.CallbackFunctionPtr != IntPtr.Zero;
			}
		}

		public LckEncodedPacketCallback(IntPtr callbackObjectPtr, IntPtr callbackFunctionPtr)
		{
			this.CallbackObjectPtr = callbackObjectPtr;
			this.CallbackFunctionPtr = callbackFunctionPtr;
		}
	}
}
