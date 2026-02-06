using System;
using System.Runtime.InteropServices;

public class UpdateTransactionCatalogItemSunsetStatusResponse : MothershipResponse
{
	internal UpdateTransactionCatalogItemSunsetStatusResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.UpdateTransactionCatalogItemSunsetStatusResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(UpdateTransactionCatalogItemSunsetStatusResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(UpdateTransactionCatalogItemSunsetStatusResponse obj)
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
					MothershipApiPINVOKE.delete_UpdateTransactionCatalogItemSunsetStatusResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.UpdateTransactionCatalogItemSunsetStatusResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static UpdateTransactionCatalogItemSunsetStatusResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UpdateTransactionCatalogItemSunsetStatusResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		UpdateTransactionCatalogItemSunsetStatusResponse result = (intPtr == IntPtr.Zero) ? null : new UpdateTransactionCatalogItemSunsetStatusResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public MothershipTransactionCatalogItem item
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.UpdateTransactionCatalogItemSunsetStatusResponse_item_get(this.swigCPtr);
			MothershipTransactionCatalogItem result = (intPtr == IntPtr.Zero) ? null : new MothershipTransactionCatalogItem(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UpdateTransactionCatalogItemSunsetStatusResponse_item_set(this.swigCPtr, MothershipTransactionCatalogItem.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public UpdateTransactionCatalogItemSunsetStatusResponse() : this(MothershipApiPINVOKE.new_UpdateTransactionCatalogItemSunsetStatusResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
