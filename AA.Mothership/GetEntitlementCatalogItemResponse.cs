using System;
using System.Runtime.InteropServices;

public class GetEntitlementCatalogItemResponse : MothershipResponse
{
	internal GetEntitlementCatalogItemResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetEntitlementCatalogItemResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetEntitlementCatalogItemResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetEntitlementCatalogItemResponse obj)
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
					MothershipApiPINVOKE.delete_GetEntitlementCatalogItemResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool entitlementCatalogItemResponse_ParseFromResponseString = MothershipApiPINVOKE.GetEntitlementCatalogItemResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return entitlementCatalogItemResponse_ParseFromResponseString;
	}

	public static GetEntitlementCatalogItemResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr entitlementCatalogItemResponse_FromMothershipResponse = MothershipApiPINVOKE.GetEntitlementCatalogItemResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		GetEntitlementCatalogItemResponse result = (entitlementCatalogItemResponse_FromMothershipResponse == IntPtr.Zero) ? null : new GetEntitlementCatalogItemResponse(entitlementCatalogItemResponse_FromMothershipResponse, false);
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
			IntPtr entitlementCatalogItemResponse_result_get = MothershipApiPINVOKE.GetEntitlementCatalogItemResponse_result_get(this.swigCPtr);
			MothershipEntitlementCatalogItem result = (entitlementCatalogItemResponse_result_get == IntPtr.Zero) ? null : new MothershipEntitlementCatalogItem(entitlementCatalogItemResponse_result_get, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.GetEntitlementCatalogItemResponse_result_set(this.swigCPtr, MothershipEntitlementCatalogItem.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public GetEntitlementCatalogItemResponse() : this(MothershipApiPINVOKE.new_GetEntitlementCatalogItemResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
