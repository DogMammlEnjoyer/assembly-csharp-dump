using System;
using System.Runtime.InteropServices;

public class GetProgressionTrackValuesForPlayerResponse : MothershipResponse
{
	internal GetProgressionTrackValuesForPlayerResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetProgressionTrackValuesForPlayerResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetProgressionTrackValuesForPlayerResponse obj)
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
					MothershipApiPINVOKE.delete_GetProgressionTrackValuesForPlayerResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool progressionTrackValuesForPlayerResponse_ParseFromResponseString = MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return progressionTrackValuesForPlayerResponse_ParseFromResponseString;
	}

	public static GetProgressionTrackValuesForPlayerResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr progressionTrackValuesForPlayerResponse_FromMothershipResponse = MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		GetProgressionTrackValuesForPlayerResponse result = (progressionTrackValuesForPlayerResponse_FromMothershipResponse == IntPtr.Zero) ? null : new GetProgressionTrackValuesForPlayerResponse(progressionTrackValuesForPlayerResponse_FromMothershipResponse, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public UserHydratedProgressionTrackVector Results
	{
		get
		{
			IntPtr progressionTrackValuesForPlayerResponse_Results_get = MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerResponse_Results_get(this.swigCPtr);
			UserHydratedProgressionTrackVector result = (progressionTrackValuesForPlayerResponse_Results_get == IntPtr.Zero) ? null : new UserHydratedProgressionTrackVector(progressionTrackValuesForPlayerResponse_Results_get, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerResponse_Results_set(this.swigCPtr, UserHydratedProgressionTrackVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public GetProgressionTrackValuesForPlayerResponse() : this(MothershipApiPINVOKE.new_GetProgressionTrackValuesForPlayerResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	public static readonly string Results_name = MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerResponse_Results_name_get();
}
