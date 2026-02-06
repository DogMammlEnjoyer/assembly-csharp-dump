using System;
using System.Runtime.InteropServices;

public class GetSingleProgressionTreeBindingResponse : MothershipResponse
{
	internal GetSingleProgressionTreeBindingResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetSingleProgressionTreeBindingResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetSingleProgressionTreeBindingResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetSingleProgressionTreeBindingResponse obj)
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
					MothershipApiPINVOKE.delete_GetSingleProgressionTreeBindingResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool singleProgressionTreeBindingResponse_ParseFromResponseString = MothershipApiPINVOKE.GetSingleProgressionTreeBindingResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return singleProgressionTreeBindingResponse_ParseFromResponseString;
	}

	public static GetSingleProgressionTreeBindingResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr singleProgressionTreeBindingResponse_FromMothershipResponse = MothershipApiPINVOKE.GetSingleProgressionTreeBindingResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		GetSingleProgressionTreeBindingResponse result = (singleProgressionTreeBindingResponse_FromMothershipResponse == IntPtr.Zero) ? null : new GetSingleProgressionTreeBindingResponse(singleProgressionTreeBindingResponse_FromMothershipResponse, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public ProgressionTreeBindingResponse Results
	{
		get
		{
			IntPtr singleProgressionTreeBindingResponse_Results_get = MothershipApiPINVOKE.GetSingleProgressionTreeBindingResponse_Results_get(this.swigCPtr);
			ProgressionTreeBindingResponse result = (singleProgressionTreeBindingResponse_Results_get == IntPtr.Zero) ? null : new ProgressionTreeBindingResponse(singleProgressionTreeBindingResponse_Results_get, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.GetSingleProgressionTreeBindingResponse_Results_set(this.swigCPtr, ProgressionTreeBindingResponse.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public GetSingleProgressionTreeBindingResponse() : this(MothershipApiPINVOKE.new_GetSingleProgressionTreeBindingResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	public static readonly string Results_name = MothershipApiPINVOKE.GetSingleProgressionTreeBindingResponse_Results_name_get();
}
