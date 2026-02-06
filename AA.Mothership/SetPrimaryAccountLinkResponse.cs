using System;
using System.Runtime.InteropServices;

public class SetPrimaryAccountLinkResponse : MothershipResponse
{
	internal SetPrimaryAccountLinkResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.SetPrimaryAccountLinkResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(SetPrimaryAccountLinkResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(SetPrimaryAccountLinkResponse obj)
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
					MothershipApiPINVOKE.delete_SetPrimaryAccountLinkResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.SetPrimaryAccountLinkResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static SetPrimaryAccountLinkResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.SetPrimaryAccountLinkResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		SetPrimaryAccountLinkResponse result = (intPtr == IntPtr.Zero) ? null : new SetPrimaryAccountLinkResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public AccountLinksVector Links
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.SetPrimaryAccountLinkResponse_Links_get(this.swigCPtr);
			AccountLinksVector result = (intPtr == IntPtr.Zero) ? null : new AccountLinksVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.SetPrimaryAccountLinkResponse_Links_set(this.swigCPtr, AccountLinksVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public SetPrimaryAccountLinkResponse() : this(MothershipApiPINVOKE.new_SetPrimaryAccountLinkResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	public static readonly string links_name = MothershipApiPINVOKE.SetPrimaryAccountLinkResponse_links_name_get();
}
