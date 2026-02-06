using System;
using System.Runtime.InteropServices;

public class MothershipGetMergedInventoryResponse : MothershipResponse
{
	internal MothershipGetMergedInventoryResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.MothershipGetMergedInventoryResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(MothershipGetMergedInventoryResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(MothershipGetMergedInventoryResponse obj)
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
					MothershipApiPINVOKE.delete_MothershipGetMergedInventoryResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public InventoryItemSummaryVector Results
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.MothershipGetMergedInventoryResponse_Results_get(this.swigCPtr);
			InventoryItemSummaryVector result = (intPtr == IntPtr.Zero) ? null : new InventoryItemSummaryVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.MothershipGetMergedInventoryResponse_Results_set(this.swigCPtr, InventoryItemSummaryVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.MothershipGetMergedInventoryResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static MothershipGetMergedInventoryResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.MothershipGetMergedInventoryResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		MothershipGetMergedInventoryResponse result = (intPtr == IntPtr.Zero) ? null : new MothershipGetMergedInventoryResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public MothershipGetMergedInventoryResponse() : this(MothershipApiPINVOKE.new_MothershipGetMergedInventoryResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
