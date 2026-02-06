using System;
using System.Runtime.InteropServices;

public class GetEnvironmentResponse : MothershipResponse
{
	internal GetEnvironmentResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetEnvironmentResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetEnvironmentResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetEnvironmentResponse obj)
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
					MothershipApiPINVOKE.delete_GetEnvironmentResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool environmentResponse_ParseFromResponseString = MothershipApiPINVOKE.GetEnvironmentResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return environmentResponse_ParseFromResponseString;
	}

	public static GetEnvironmentResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr environmentResponse_FromMothershipResponse = MothershipApiPINVOKE.GetEnvironmentResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		GetEnvironmentResponse result = (environmentResponse_FromMothershipResponse == IntPtr.Zero) ? null : new GetEnvironmentResponse(environmentResponse_FromMothershipResponse, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public MothershipTitleEnvironment env
	{
		get
		{
			IntPtr environmentResponse_env_get = MothershipApiPINVOKE.GetEnvironmentResponse_env_get(this.swigCPtr);
			MothershipTitleEnvironment result = (environmentResponse_env_get == IntPtr.Zero) ? null : new MothershipTitleEnvironment(environmentResponse_env_get, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.GetEnvironmentResponse_env_set(this.swigCPtr, MothershipTitleEnvironment.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public GetEnvironmentResponse() : this(MothershipApiPINVOKE.new_GetEnvironmentResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
