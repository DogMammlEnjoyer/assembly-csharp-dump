using System;
using System.Runtime.InteropServices;

public class GetProgressionTrackBindingsResponse : MothershipResponse
{
	internal GetProgressionTrackBindingsResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetProgressionTrackBindingsResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetProgressionTrackBindingsResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetProgressionTrackBindingsResponse obj)
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
					MothershipApiPINVOKE.delete_GetProgressionTrackBindingsResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool progressionTrackBindingsResponse_ParseFromResponseString = MothershipApiPINVOKE.GetProgressionTrackBindingsResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return progressionTrackBindingsResponse_ParseFromResponseString;
	}

	public static GetProgressionTrackBindingsResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr progressionTrackBindingsResponse_FromMothershipResponse = MothershipApiPINVOKE.GetProgressionTrackBindingsResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		GetProgressionTrackBindingsResponse result = (progressionTrackBindingsResponse_FromMothershipResponse == IntPtr.Zero) ? null : new GetProgressionTrackBindingsResponse(progressionTrackBindingsResponse_FromMothershipResponse, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public ProgressionTrackBindingVector Results
	{
		get
		{
			IntPtr progressionTrackBindingsResponse_Results_get = MothershipApiPINVOKE.GetProgressionTrackBindingsResponse_Results_get(this.swigCPtr);
			ProgressionTrackBindingVector result = (progressionTrackBindingsResponse_Results_get == IntPtr.Zero) ? null : new ProgressionTrackBindingVector(progressionTrackBindingsResponse_Results_get, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.GetProgressionTrackBindingsResponse_Results_set(this.swigCPtr, ProgressionTrackBindingVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public GetProgressionTrackBindingsResponse() : this(MothershipApiPINVOKE.new_GetProgressionTrackBindingsResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	public static readonly string Results_name = MothershipApiPINVOKE.GetProgressionTrackBindingsResponse_Results_name_get();
}
