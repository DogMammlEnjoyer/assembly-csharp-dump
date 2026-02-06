using System;
using System.Runtime.InteropServices;

public class GetUserDataMetadataResponse : MothershipUserDataMetadata
{
	internal GetUserDataMetadataResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetUserDataMetadataResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetUserDataMetadataResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetUserDataMetadataResponse obj)
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
					MothershipApiPINVOKE.delete_GetUserDataMetadataResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool userDataMetadataResponse_ParseFromResponseString = MothershipApiPINVOKE.GetUserDataMetadataResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return userDataMetadataResponse_ParseFromResponseString;
	}

	public static GetUserDataMetadataResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr userDataMetadataResponse_FromMothershipResponse = MothershipApiPINVOKE.GetUserDataMetadataResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		GetUserDataMetadataResponse result = (userDataMetadataResponse_FromMothershipResponse == IntPtr.Zero) ? null : new GetUserDataMetadataResponse(userDataMetadataResponse_FromMothershipResponse, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public GetUserDataMetadataResponse() : this(MothershipApiPINVOKE.new_GetUserDataMetadataResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
