using System;
using System.Runtime.InteropServices;

public class CreateTransactionCatalogItemError : MothershipError
{
	internal CreateTransactionCatalogItemError(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.CreateTransactionCatalogItemError_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(CreateTransactionCatalogItemError obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(CreateTransactionCatalogItemError obj)
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
					MothershipApiPINVOKE.delete_CreateTransactionCatalogItemError(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public CreateTransactionCatalogItemError(string message, int statusCode, string traceId, string mothershipErrorCode) : this(MothershipApiPINVOKE.new_CreateTransactionCatalogItemError__SWIG_0(message, statusCode, traceId, mothershipErrorCode), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public CreateTransactionCatalogItemError(string message, int statusCode, string traceId) : this(MothershipApiPINVOKE.new_CreateTransactionCatalogItemError__SWIG_1(message, statusCode, traceId), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public CreateTransactionCatalogItemError(string message, int statusCode) : this(MothershipApiPINVOKE.new_CreateTransactionCatalogItemError__SWIG_2(message, statusCode), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
