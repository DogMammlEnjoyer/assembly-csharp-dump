using System;
using System.Runtime.InteropServices;

public class UpdateEntitlementCatalogItemResponse : MothershipResponse
{
	internal UpdateEntitlementCatalogItemResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.UpdateEntitlementCatalogItemResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(UpdateEntitlementCatalogItemResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(UpdateEntitlementCatalogItemResponse obj)
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
					MothershipApiPINVOKE.delete_UpdateEntitlementCatalogItemResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.UpdateEntitlementCatalogItemResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static UpdateEntitlementCatalogItemResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UpdateEntitlementCatalogItemResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		UpdateEntitlementCatalogItemResponse result = (intPtr == IntPtr.Zero) ? null : new UpdateEntitlementCatalogItemResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public MothershipEntitlementCatalogItem result
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.UpdateEntitlementCatalogItemResponse_result_get(this.swigCPtr);
			MothershipEntitlementCatalogItem result = (intPtr == IntPtr.Zero) ? null : new MothershipEntitlementCatalogItem(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UpdateEntitlementCatalogItemResponse_result_set(this.swigCPtr, MothershipEntitlementCatalogItem.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public UpdateEntitlementCatalogItemResponse() : this(MothershipApiPINVOKE.new_UpdateEntitlementCatalogItemResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
