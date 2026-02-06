using System;
using System.Runtime.InteropServices;

public class ListOfferCatalogItemsResponse : MothershipResponse
{
	internal ListOfferCatalogItemsResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.ListOfferCatalogItemsResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(ListOfferCatalogItemsResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(ListOfferCatalogItemsResponse obj)
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
					MothershipApiPINVOKE.delete_ListOfferCatalogItemsResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.ListOfferCatalogItemsResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static ListOfferCatalogItemsResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ListOfferCatalogItemsResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		ListOfferCatalogItemsResponse result = (intPtr == IntPtr.Zero) ? null : new ListOfferCatalogItemsResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public OfferCatalogItemVector Results
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.ListOfferCatalogItemsResponse_Results_get(this.swigCPtr);
			OfferCatalogItemVector result = (intPtr == IntPtr.Zero) ? null : new OfferCatalogItemVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.ListOfferCatalogItemsResponse_Results_set(this.swigCPtr, OfferCatalogItemVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public ListOfferCatalogItemsResponse() : this(MothershipApiPINVOKE.new_ListOfferCatalogItemsResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	public static readonly string Results_name = MothershipApiPINVOKE.ListOfferCatalogItemsResponse_Results_name_get();
}
