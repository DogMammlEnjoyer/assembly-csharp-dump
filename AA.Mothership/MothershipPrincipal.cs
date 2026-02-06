using System;
using System.Runtime.InteropServices;

public class MothershipPrincipal : IDisposable
{
	internal MothershipPrincipal(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(MothershipPrincipal obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(MothershipPrincipal obj)
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

	~MothershipPrincipal()
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
					MothershipApiPINVOKE.delete_MothershipPrincipal(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public MothershipPrincipal(MothershipPrincipal.Value v) : this(MothershipApiPINVOKE.new_MothershipPrincipal((int)v), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public string toString()
	{
		string result = MothershipApiPINVOKE.MothershipPrincipal_toString(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public MothershipPrincipal.Value getValue()
	{
		MothershipPrincipal.Value result = (MothershipPrincipal.Value)MothershipApiPINVOKE.MothershipPrincipal_getValue(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public enum Value
	{
		CLIENT,
		SERVER,
		AUTOMATION
	}
}
