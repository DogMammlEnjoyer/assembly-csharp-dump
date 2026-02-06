using System;
using System.Runtime.InteropServices;

public class ListClientMothershipTitleDataResponse : MothershipResponse
{
	internal ListClientMothershipTitleDataResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.ListClientMothershipTitleDataResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(ListClientMothershipTitleDataResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(ListClientMothershipTitleDataResponse obj)
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
					MothershipApiPINVOKE.delete_ListClientMothershipTitleDataResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public TitleDataShortVector Results
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.ListClientMothershipTitleDataResponse_Results_get(this.swigCPtr);
			TitleDataShortVector result = (intPtr == IntPtr.Zero) ? null : new TitleDataShortVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.ListClientMothershipTitleDataResponse_Results_set(this.swigCPtr, TitleDataShortVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.ListClientMothershipTitleDataResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static ListClientMothershipTitleDataResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ListClientMothershipTitleDataResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		ListClientMothershipTitleDataResponse result = (intPtr == IntPtr.Zero) ? null : new ListClientMothershipTitleDataResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public ListClientMothershipTitleDataResponse() : this(MothershipApiPINVOKE.new_ListClientMothershipTitleDataResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
