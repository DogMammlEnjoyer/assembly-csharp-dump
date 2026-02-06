using System;
using System.Runtime.InteropServices;

public class ClientAuthenticationFailedError : MothershipError
{
	internal ClientAuthenticationFailedError(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.ClientAuthenticationFailedError_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(ClientAuthenticationFailedError obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(ClientAuthenticationFailedError obj)
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
					MothershipApiPINVOKE.delete_ClientAuthenticationFailedError(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public ClientAuthenticationFailedError(string message, int statusCode, string traceId, string mothershipErrorCode) : this(MothershipApiPINVOKE.new_ClientAuthenticationFailedError__SWIG_0(message, statusCode, traceId, mothershipErrorCode), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ClientAuthenticationFailedError(string message, int statusCode, string traceId) : this(MothershipApiPINVOKE.new_ClientAuthenticationFailedError__SWIG_1(message, statusCode, traceId), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ClientAuthenticationFailedError(string message, int statusCode) : this(MothershipApiPINVOKE.new_ClientAuthenticationFailedError__SWIG_2(message, statusCode), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;
}
