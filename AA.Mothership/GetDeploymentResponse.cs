using System;
using System.Runtime.InteropServices;

public class GetDeploymentResponse : MothershipResponse
{
	internal GetDeploymentResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetDeploymentResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetDeploymentResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetDeploymentResponse obj)
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
					MothershipApiPINVOKE.delete_GetDeploymentResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool deploymentResponse_ParseFromResponseString = MothershipApiPINVOKE.GetDeploymentResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return deploymentResponse_ParseFromResponseString;
	}

	public static GetDeploymentResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr deploymentResponse_FromMothershipResponse = MothershipApiPINVOKE.GetDeploymentResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		GetDeploymentResponse result = (deploymentResponse_FromMothershipResponse == IntPtr.Zero) ? null : new GetDeploymentResponse(deploymentResponse_FromMothershipResponse, false);
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
			IntPtr deploymentResponse_deployment_get = MothershipApiPINVOKE.GetDeploymentResponse_deployment_get(this.swigCPtr);
			MothershipTitleEnvDeployment result = (deploymentResponse_deployment_get == IntPtr.Zero) ? null : new MothershipTitleEnvDeployment(deploymentResponse_deployment_get, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.GetDeploymentResponse_deployment_set(this.swigCPtr, MothershipTitleEnvDeployment.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public GetDeploymentResponse() : this(MothershipApiPINVOKE.new_GetDeploymentResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
