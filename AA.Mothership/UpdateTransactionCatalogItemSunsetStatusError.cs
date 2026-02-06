using System;
using System.Runtime.InteropServices;

public class UpdateTransactionCatalogItemSunsetStatusError : MothershipError
{
	internal UpdateTransactionCatalogItemSunsetStatusError(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.UpdateTransactionCatalogItemSunsetStatusError_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(UpdateTransactionCatalogItemSunsetStatusError obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(UpdateTransactionCatalogItemSunsetStatusError obj)
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
					MothershipApiPINVOKE.delete_UpdateTransactionCatalogItemSunsetStatusError(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public UpdateTransactionCatalogItemSunsetStatusError(string message, int statusCode, string traceId, string mothershipErrorCode) : this(MothershipApiPINVOKE.new_UpdateTransactionCatalogItemSunsetStatusError__SWIG_0(message, statusCode, traceId, mothershipErrorCode), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UpdateTransactionCatalogItemSunsetStatusError(string message, int statusCode, string traceId) : this(MothershipApiPINVOKE.new_UpdateTransactionCatalogItemSunsetStatusError__SWIG_1(message, statusCode, traceId), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UpdateTransactionCatalogItemSunsetStatusError(string message, int statusCode) : this(MothershipApiPINVOKE.new_UpdateTransactionCatalogItemSunsetStatusError__SWIG_2(message, statusCode), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
