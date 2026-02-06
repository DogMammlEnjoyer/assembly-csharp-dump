using System;
using System.Runtime.InteropServices;

public class ListProgressionTreeResponse : MothershipResponse
{
	internal ListProgressionTreeResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.ListProgressionTreeResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(ListProgressionTreeResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(ListProgressionTreeResponse obj)
	{
		if (obj == null)
		{
			return new HandleRef(null, IntPtr.Zero);
		}
		if (!obj.swigCMemOwn)
		{
			throw new ApplicationException("Cannot release ownership as memory is not owned");
		}
		HandleRef result = obj.swigCPtr;
		obj.swigCMemOwn = false;
		obj.Dispose();
		return result;
	}

	protected override void Dispose(bool disposing)
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero)
			{
				if (this.swigCMemOwn)
				{
					this.swigCMemOwn = false;
					MothershipApiPINVOKE.delete_ListProgressionTreeResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.ListProgressionTreeResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static ListProgressionTreeResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ListProgressionTreeResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		ListProgressionTreeResponse result = (intPtr == IntPtr.Zero) ? null : new ListProgressionTreeResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public SWIGTYPE_p_std__vectorT_MothershipApiShared__HydratedProgressionTreeResponse_t Results
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.ListProgressionTreeResponse_Results_get(this.swigCPtr);
			SWIGTYPE_p_std__vectorT_MothershipApiShared__HydratedProgressionTreeResponse_t result = (intPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_std__vectorT_MothershipApiShared__HydratedProgressionTreeResponse_t(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.ListProgressionTreeResponse_Results_set(this.swigCPtr, SWIGTYPE_p_std__vectorT_MothershipApiShared__HydratedProgressionTreeResponse_t.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public ListProgressionTreeResponse() : this(MothershipApiPINVOKE.new_ListProgressionTreeResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	public static readonly string Results_name = MothershipApiPINVOKE.ListProgressionTreeResponse_Results_name_get();
}
