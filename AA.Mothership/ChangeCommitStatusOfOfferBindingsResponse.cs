using System;
using System.Runtime.InteropServices;

public class ChangeCommitStatusOfOfferBindingsResponse : MothershipResponse
{
	internal ChangeCommitStatusOfOfferBindingsResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.ChangeCommitStatusOfOfferBindingsResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(ChangeCommitStatusOfOfferBindingsResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(ChangeCommitStatusOfOfferBindingsResponse obj)
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
					MothershipApiPINVOKE.delete_ChangeCommitStatusOfOfferBindingsResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.ChangeCommitStatusOfOfferBindingsResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static ChangeCommitStatusOfOfferBindingsResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ChangeCommitStatusOfOfferBindingsResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		ChangeCommitStatusOfOfferBindingsResponse result = (intPtr == IntPtr.Zero) ? null : new ChangeCommitStatusOfOfferBindingsResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public OfferBindingVector Results
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.ChangeCommitStatusOfOfferBindingsResponse_Results_get(this.swigCPtr);
			OfferBindingVector result = (intPtr == IntPtr.Zero) ? null : new OfferBindingVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.ChangeCommitStatusOfOfferBindingsResponse_Results_set(this.swigCPtr, OfferBindingVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public ChangeCommitStatusOfOfferBindingsResponse() : this(MothershipApiPINVOKE.new_ChangeCommitStatusOfOfferBindingsResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	public static readonly string Results_name = MothershipApiPINVOKE.ChangeCommitStatusOfOfferBindingsResponse_Results_name_get();
}
