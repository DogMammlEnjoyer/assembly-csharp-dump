using System;
using System.Runtime.InteropServices;

public class GetProgressionTreesForPlayerResponse : MothershipResponse
{
	internal GetProgressionTreesForPlayerResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetProgressionTreesForPlayerResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetProgressionTreesForPlayerResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetProgressionTreesForPlayerResponse obj)
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
					MothershipApiPINVOKE.delete_GetProgressionTreesForPlayerResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool progressionTreesForPlayerResponse_ParseFromResponseString = MothershipApiPINVOKE.GetProgressionTreesForPlayerResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return progressionTreesForPlayerResponse_ParseFromResponseString;
	}

	public static GetProgressionTreesForPlayerResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr progressionTreesForPlayerResponse_FromMothershipResponse = MothershipApiPINVOKE.GetProgressionTreesForPlayerResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		GetProgressionTreesForPlayerResponse result = (progressionTreesForPlayerResponse_FromMothershipResponse == IntPtr.Zero) ? null : new GetProgressionTreesForPlayerResponse(progressionTreesForPlayerResponse_FromMothershipResponse, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public UserHydratedProgressionTreeVector Results
	{
		get
		{
			IntPtr progressionTreesForPlayerResponse_Results_get = MothershipApiPINVOKE.GetProgressionTreesForPlayerResponse_Results_get(this.swigCPtr);
			UserHydratedProgressionTreeVector result = (progressionTreesForPlayerResponse_Results_get == IntPtr.Zero) ? null : new UserHydratedProgressionTreeVector(progressionTreesForPlayerResponse_Results_get, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.GetProgressionTreesForPlayerResponse_Results_set(this.swigCPtr, UserHydratedProgressionTreeVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public GetProgressionTreesForPlayerResponse() : this(MothershipApiPINVOKE.new_GetProgressionTreesForPlayerResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	public static readonly string Results_name = MothershipApiPINVOKE.GetProgressionTreesForPlayerResponse_Results_name_get();
}
