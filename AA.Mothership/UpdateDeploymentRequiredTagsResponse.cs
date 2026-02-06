using System;
using System.Runtime.InteropServices;

public class UpdateDeploymentRequiredTagsResponse : MothershipResponse
{
	internal UpdateDeploymentRequiredTagsResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.UpdateDeploymentRequiredTagsResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(UpdateDeploymentRequiredTagsResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(UpdateDeploymentRequiredTagsResponse obj)
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
					MothershipApiPINVOKE.delete_UpdateDeploymentRequiredTagsResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.UpdateDeploymentRequiredTagsResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static UpdateDeploymentRequiredTagsResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UpdateDeploymentRequiredTagsResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		UpdateDeploymentRequiredTagsResponse result = (intPtr == IntPtr.Zero) ? null : new UpdateDeploymentRequiredTagsResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public MothershipTitleEnvDeployment deployment
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.UpdateDeploymentRequiredTagsResponse_deployment_get(this.swigCPtr);
			MothershipTitleEnvDeployment result = (intPtr == IntPtr.Zero) ? null : new MothershipTitleEnvDeployment(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UpdateDeploymentRequiredTagsResponse_deployment_set(this.swigCPtr, MothershipTitleEnvDeployment.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public UpdateDeploymentRequiredTagsResponse() : this(MothershipApiPINVOKE.new_UpdateDeploymentRequiredTagsResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
