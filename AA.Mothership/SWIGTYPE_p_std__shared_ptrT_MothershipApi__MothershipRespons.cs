using System;
using System.Runtime.InteropServices;

public class SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipResponse_t
{
	internal SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipResponse_t(IntPtr cPtr, bool futureUse)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	protected SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipResponse_t()
	{
		this.swigCPtr = new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef getCPtr(SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipResponse_t obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(SWIGTYPE_p_std__shared_ptrT_MothershipApi__MothershipResponse_t obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	private HandleRef swigCPtr;
}
