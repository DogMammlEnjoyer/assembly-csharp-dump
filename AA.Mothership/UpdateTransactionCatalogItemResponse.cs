using System;
using System.Runtime.InteropServices;

public class UpdateTransactionCatalogItemResponse : MothershipResponse
{
	internal UpdateTransactionCatalogItemResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.UpdateTransactionCatalogItemResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(UpdateTransactionCatalogItemResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(UpdateTransactionCatalogItemResponse obj)
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
					MothershipApiPINVOKE.delete_UpdateTransactionCatalogItemResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.UpdateTransactionCatalogItemResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static UpdateTransactionCatalogItemResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UpdateTransactionCatalogItemResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		UpdateTransactionCatalogItemResponse result = (intPtr == IntPtr.Zero) ? null : new UpdateTransactionCatalogItemResponse(intPtr, false);
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
			IntPtr intPtr = MothershipApiPINVOKE.UpdateTransactionCatalogItemResponse_item_get(this.swigCPtr);
			MothershipTransactionCatalogItem result = (intPtr == IntPtr.Zero) ? null : new MothershipTransactionCatalogItem(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.UpdateTransactionCatalogItemResponse_item_set(this.swigCPtr, MothershipTransactionCatalogItem.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public UpdateTransactionCatalogItemResponse() : this(MothershipApiPINVOKE.new_UpdateTransactionCatalogItemResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
