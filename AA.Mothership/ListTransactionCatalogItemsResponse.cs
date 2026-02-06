using System;
using System.Runtime.InteropServices;

public class ListTransactionCatalogItemsResponse : MothershipResponse
{
	internal ListTransactionCatalogItemsResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.ListTransactionCatalogItemsResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(ListTransactionCatalogItemsResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(ListTransactionCatalogItemsResponse obj)
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
					MothershipApiPINVOKE.delete_ListTransactionCatalogItemsResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.ListTransactionCatalogItemsResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static ListTransactionCatalogItemsResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ListTransactionCatalogItemsResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		ListTransactionCatalogItemsResponse result = (intPtr == IntPtr.Zero) ? null : new ListTransactionCatalogItemsResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public ListTransactionsResultsVector Results
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.ListTransactionCatalogItemsResponse_Results_get(this.swigCPtr);
			ListTransactionsResultsVector result = (intPtr == IntPtr.Zero) ? null : new ListTransactionsResultsVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.ListTransactionCatalogItemsResponse_Results_set(this.swigCPtr, ListTransactionsResultsVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public ListTransactionCatalogItemsResponse() : this(MothershipApiPINVOKE.new_ListTransactionCatalogItemsResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
