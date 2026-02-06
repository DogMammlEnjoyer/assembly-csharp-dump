using System;
using System.Runtime.InteropServices;

public class GetSingleProgressionTrackBindingResponse : MothershipResponse
{
	internal GetSingleProgressionTrackBindingResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetSingleProgressionTrackBindingResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetSingleProgressionTrackBindingResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetSingleProgressionTrackBindingResponse obj)
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
					MothershipApiPINVOKE.delete_GetSingleProgressionTrackBindingResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool singleProgressionTrackBindingResponse_ParseFromResponseString = MothershipApiPINVOKE.GetSingleProgressionTrackBindingResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return singleProgressionTrackBindingResponse_ParseFromResponseString;
	}

	public static GetSingleProgressionTrackBindingResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr singleProgressionTrackBindingResponse_FromMothershipResponse = MothershipApiPINVOKE.GetSingleProgressionTrackBindingResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		GetSingleProgressionTrackBindingResponse result = (singleProgressionTrackBindingResponse_FromMothershipResponse == IntPtr.Zero) ? null : new GetSingleProgressionTrackBindingResponse(singleProgressionTrackBindingResponse_FromMothershipResponse, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public ProgressionTrackBindingResponse Results
	{
		get
		{
			IntPtr singleProgressionTrackBindingResponse_Results_get = MothershipApiPINVOKE.GetSingleProgressionTrackBindingResponse_Results_get(this.swigCPtr);
			ProgressionTrackBindingResponse result = (singleProgressionTrackBindingResponse_Results_get == IntPtr.Zero) ? null : new ProgressionTrackBindingResponse(singleProgressionTrackBindingResponse_Results_get, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.GetSingleProgressionTrackBindingResponse_Results_set(this.swigCPtr, ProgressionTrackBindingResponse.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public GetSingleProgressionTrackBindingResponse() : this(MothershipApiPINVOKE.new_GetSingleProgressionTrackBindingResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	public static readonly string Results_name = MothershipApiPINVOKE.GetSingleProgressionTrackBindingResponse_Results_name_get();
}
