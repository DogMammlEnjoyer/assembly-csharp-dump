using System;
using System.Runtime.InteropServices;

public class MothershipPurchaseOfferResponse : MothershipResponse
{
	internal MothershipPurchaseOfferResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.MothershipPurchaseOfferResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(MothershipPurchaseOfferResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(MothershipPurchaseOfferResponse obj)
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
					MothershipApiPINVOKE.delete_MothershipPurchaseOfferResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.MothershipPurchaseOfferResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static MothershipPurchaseOfferResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.MothershipPurchaseOfferResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		MothershipPurchaseOfferResponse result = (intPtr == IntPtr.Zero) ? null : new MothershipPurchaseOfferResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public PurchaseChangesMap Changes
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.MothershipPurchaseOfferResponse_Changes_get(this.swigCPtr);
			PurchaseChangesMap result = (intPtr == IntPtr.Zero) ? null : new PurchaseChangesMap(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.MothershipPurchaseOfferResponse_Changes_set(this.swigCPtr, PurchaseChangesMap.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public MothershipPurchaseOfferResponse() : this(MothershipApiPINVOKE.new_MothershipPurchaseOfferResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
