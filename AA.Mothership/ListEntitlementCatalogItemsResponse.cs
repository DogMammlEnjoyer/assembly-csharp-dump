using System;
using System.Runtime.InteropServices;

public class ListEntitlementCatalogItemsResponse : MothershipResponse
{
	internal ListEntitlementCatalogItemsResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.ListEntitlementCatalogItemsResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(ListEntitlementCatalogItemsResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(ListEntitlementCatalogItemsResponse obj)
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
					MothershipApiPINVOKE.delete_ListEntitlementCatalogItemsResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.ListEntitlementCatalogItemsResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static ListEntitlementCatalogItemsResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ListEntitlementCatalogItemsResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		ListEntitlementCatalogItemsResponse result = (intPtr == IntPtr.Zero) ? null : new ListEntitlementCatalogItemsResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public ListEntitlementResultsVector Results
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.ListEntitlementCatalogItemsResponse_Results_get(this.swigCPtr);
			ListEntitlementResultsVector result = (intPtr == IntPtr.Zero) ? null : new ListEntitlementResultsVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.ListEntitlementCatalogItemsResponse_Results_set(this.swigCPtr, ListEntitlementResultsVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public ListEntitlementCatalogItemsResponse() : this(MothershipApiPINVOKE.new_ListEntitlementCatalogItemsResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
