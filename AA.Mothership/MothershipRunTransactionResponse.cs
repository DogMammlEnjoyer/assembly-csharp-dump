using System;
using System.Runtime.InteropServices;

public class MothershipRunTransactionResponse : MothershipResponse
{
	internal MothershipRunTransactionResponse(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.MothershipRunTransactionResponse_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(MothershipRunTransactionResponse obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(MothershipRunTransactionResponse obj)
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
					MothershipApiPINVOKE.delete_MothershipRunTransactionResponse(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public override bool ParseFromResponseString(string response)
	{
		bool result = MothershipApiPINVOKE.MothershipRunTransactionResponse_ParseFromResponseString(this.swigCPtr, response);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public static MothershipRunTransactionResponse FromMothershipResponse(MothershipResponse response)
	{
		IntPtr intPtr = MothershipApiPINVOKE.MothershipRunTransactionResponse_FromMothershipResponse(MothershipResponse.getCPtr(response));
		MothershipRunTransactionResponse result = (intPtr == IntPtr.Zero) ? null : new MothershipRunTransactionResponse(intPtr, false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public PurchaseResultsVector Results
	{
		get
		{
			IntPtr intPtr = MothershipApiPINVOKE.MothershipRunTransactionResponse_Results_get(this.swigCPtr);
			PurchaseResultsVector result = (intPtr == IntPtr.Zero) ? null : new PurchaseResultsVector(intPtr, false);
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
			return result;
		}
		set
		{
			MothershipApiPINVOKE.MothershipRunTransactionResponse_Results_set(this.swigCPtr, PurchaseResultsVector.getCPtr(value));
			if (MothershipApiPINVOKE.SWIGPendingException.Pending)
			{
				throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
			}
		}
	}

	public MothershipRunTransactionResponse() : this(MothershipApiPINVOKE.new_MothershipRunTransactionResponse(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
