using System;
using System.Runtime.InteropServices;

public class UpdateEnvRequiredTagsResponse : MothershipResponse
{
	internal UpdateEnvRequiredTagsResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.UpdateEnvRequiredTagsResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(UpdateEnvRequiredTagsResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(UpdateEnvRequiredTagsResponse obj)
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
					MothershipApiPINVOKE.delete_UpdateEnvRequiredTagsResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.UpdateEnvRequiredTagsResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static UpdateEnvRequiredTagsResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UpdateEnvRequiredTagsResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		UpdateEnvRequiredTagsResponse result = (intPtr == IntPtr.Zero) ? null : new UpdateEnvRequiredTagsResponse(intPtr, false);
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
			IntPtr intPtr = MothershipApiPINVOKE.UpdateEnvRequiredTagsResponse_env_get(this.swigCPtr);
			MothershipTitleEnvironment result = (intPtr == IntPtr.Zero) ? null : new MothershipTitleEnvironment(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UpdateEnvRequiredTagsResponse_env_set(this.swigCPtr, MothershipTitleEnvironment.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public UpdateEnvRequiredTagsResponse() : this(MothershipApiPINVOKE.new_UpdateEnvRequiredTagsResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
