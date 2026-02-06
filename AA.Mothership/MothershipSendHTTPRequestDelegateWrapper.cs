using System;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;

public class MothershipSendHTTPRequestDelegateWrapper : IDisposable
{
	internal MothershipSendHTTPRequestDelegateWrapper(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(MothershipSendHTTPRequestDelegateWrapper obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(MothershipSendHTTPRequestDelegateWrapper obj)
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

	~MothershipSendHTTPRequestDelegateWrapper()
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
					MothershipApiPINVOKE.delete_MothershipSendHTTPRequestDelegateWrapper(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public virtual bool SendRequest(MothershipHTTPRequest request)
	{
		bool result = MothershipApiPINVOKE.MothershipSendHTTPRequestDelegateWrapper_SendRequest(this.swigCPtr, MothershipHTTPRequest.getCPtr(request));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public MothershipSendHTTPRequestDelegateWrapper() : this(MothershipApiPINVOKE.new_MothershipSendHTTPRequestDelegateWrapper(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		this.SwigDirectorConnect();
	}

	private void SwigDirectorConnect()
	{
		MothershipSendHTTPRequestDelegateWrapper.selfInstance = this;
		if (this.SwigDerivedClassHasMethod("SendRequest", MothershipSendHTTPRequestDelegateWrapper.swigMethodTypes0))
		{
			this.swigDelegate0 = new MothershipSendHTTPRequestDelegateWrapper.SwigDelegateMothershipSendHTTPRequestDelegateWrapper_0(MothershipSendHTTPRequestDelegateWrapper.SwigDirectorMethodSendRequest);
		}
		MothershipApiPINVOKE.MothershipSendHTTPRequestDelegateWrapper_director_connect(this.swigCPtr, this.swigDelegate0);
	}

	private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
	{
		foreach (MethodInfo methodInfo in base.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
		{
			if (!(methodInfo.DeclaringType == null) && !(methodInfo.Name != methodName))
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters.Length == methodTypes.Length)
				{
					bool flag = true;
					for (int j = 0; j < parameters.Length; j++)
					{
						if (parameters[j].ParameterType != methodTypes[j])
						{
							flag = false;
							break;
						}
					}
					if (flag && methodInfo.IsVirtual && methodInfo.DeclaringType.IsSubclassOf(typeof(MothershipSendHTTPRequestDelegateWrapper)) && methodInfo.DeclaringType != methodInfo.GetBaseDefinition().DeclaringType)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	[MonoPInvokeCallback(typeof(MothershipSendHTTPRequestDelegateWrapper.SwigDelegateMothershipSendHTTPRequestDelegateWrapper_0))]
	private static bool SwigDirectorMethodSendRequest(IntPtr request)
	{
		return MothershipSendHTTPRequestDelegateWrapper.selfInstance.SendRequest(new MothershipHTTPRequest(request, false));
	}

	private HandleRef swigCPtr;

	protected static MothershipSendHTTPRequestDelegateWrapper selfInstance;

	protected bool swigCMemOwn;

	private MothershipSendHTTPRequestDelegateWrapper.SwigDelegateMothershipSendHTTPRequestDelegateWrapper_0 swigDelegate0;

	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(MothershipHTTPRequest)
	};

	public delegate bool SwigDelegateMothershipSendHTTPRequestDelegateWrapper_0(IntPtr request);
}
