using System;
using System.Runtime.InteropServices;

public class GetMySubscriptionsResponse : MothershipResponse
{
	internal GetMySubscriptionsResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetMySubscriptionsResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetMySubscriptionsResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetMySubscriptionsResponse obj)
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
					MothershipApiPINVOKE.delete_GetMySubscriptionsResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public SubscriptionsVector Results
	{
		get
		{
			IntPtr mySubscriptionsResponse_Results_get = MothershipApiPINVOKE.GetMySubscriptionsResponse_Results_get(this.swigCPtr);
			SubscriptionsVector result = (mySubscriptionsResponse_Results_get == IntPtr.Zero) ? null : new SubscriptionsVector(mySubscriptionsResponse_Results_get, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.GetMySubscriptionsResponse_Results_set(this.swigCPtr, SubscriptionsVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool mySubscriptionsResponse_ParseFromResponseString = MothershipApiPINVOKE.GetMySubscriptionsResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return mySubscriptionsResponse_ParseFromResponseString;
	}

	public static GetMySubscriptionsResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr mySubscriptionsResponse_FromMothershipResponse = MothershipApiPINVOKE.GetMySubscriptionsResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		GetMySubscriptionsResponse result = (mySubscriptionsResponse_FromMothershipResponse == IntPtr.Zero) ? null : new GetMySubscriptionsResponse(mySubscriptionsResponse_FromMothershipResponse, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public GetMySubscriptionsResponse() : this(MothershipApiPINVOKE.new_GetMySubscriptionsResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
