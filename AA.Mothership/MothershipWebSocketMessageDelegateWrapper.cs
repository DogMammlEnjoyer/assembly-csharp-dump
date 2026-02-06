using System;
using System.Runtime.InteropServices;

public class MothershipWebSocketMessageDelegateWrapper : IDisposable
{
	internal MothershipWebSocketMessageDelegateWrapper(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(MothershipWebSocketMessageDelegateWrapper obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(MothershipWebSocketMessageDelegateWrapper obj)
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

	~MothershipWebSocketMessageDelegateWrapper()
	{
		this.Dispose(false);
	}

	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero)
			{
				if (this.swigCMemOwn)
				{
					this.swigCMemOwn = false;
					MothershipApiPINVOKE.delete_MothershipWebSocketMessageDelegateWrapper(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public virtual void OnOpenCallback(IntPtr userData)
	{
		MothershipApiPINVOKE.MothershipWebSocketMessageDelegateWrapper_OnOpenCallback(this.swigCPtr, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public virtual void OnMessageCallback(MothershipWebSocketMessage message, IntPtr userData)
	{
		MothershipApiPINVOKE.MothershipWebSocketMessageDelegateWrapper_OnMessageCallback(this.swigCPtr, MothershipWebSocketMessage.getCPtr(message), userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public virtual void OnCloseCallback(IntPtr userData)
	{
		MothershipApiPINVOKE.MothershipWebSocketMessageDelegateWrapper_OnCloseCallback(this.swigCPtr, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public virtual void OnErrorCallback(IntPtr userData)
	{
		MothershipApiPINVOKE.MothershipWebSocketMessageDelegateWrapper_OnErrorCallback(this.swigCPtr, userData);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;
}
