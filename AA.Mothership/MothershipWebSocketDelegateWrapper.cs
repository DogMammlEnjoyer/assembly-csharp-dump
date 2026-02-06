using System;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;

public class MothershipWebSocketDelegateWrapper : IDisposable
{
	internal MothershipWebSocketDelegateWrapper(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(MothershipWebSocketDelegateWrapper obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(MothershipWebSocketDelegateWrapper obj)
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

	~MothershipWebSocketDelegateWrapper()
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
					MothershipApiPINVOKE.delete_MothershipWebSocketDelegateWrapper(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public virtual bool CreateConnection(MothershipOpenWebSocketEventArgs request)
	{
		bool result = MothershipApiPINVOKE.MothershipWebSocketDelegateWrapper_CreateConnection(this.swigCPtr, MothershipOpenWebSocketEventArgs.getCPtr(request));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public virtual bool CloseConnection(MothershipCloseWebSocketEventArgs request)
	{
		bool result = MothershipApiPINVOKE.MothershipWebSocketDelegateWrapper_CloseConnection(this.swigCPtr, MothershipCloseWebSocketEventArgs.getCPtr(request));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public MothershipWebSocketDelegateWrapper() : this(MothershipApiPINVOKE.new_MothershipWebSocketDelegateWrapper(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		this.SwigDirectorConnect();
	}

	private void SwigDirectorConnect()
	{
		MothershipWebSocketDelegateWrapper.selfInstance = this;
		if (this.SwigDerivedClassHasMethod("CreateConnection", MothershipWebSocketDelegateWrapper.swigMethodTypes0))
		{
			this.swigDelegate0 = new MothershipWebSocketDelegateWrapper.SwigDelegateMothershipWebSocketDelegateWrapper_0(MothershipWebSocketDelegateWrapper.SwigDirectorMethodCreateConnection);
		}
		if (this.SwigDerivedClassHasMethod("CloseConnection", MothershipWebSocketDelegateWrapper.swigMethodTypes1))
		{
			this.swigDelegate1 = new MothershipWebSocketDelegateWrapper.SwigDelegateMothershipWebSocketDelegateWrapper_1(MothershipWebSocketDelegateWrapper.SwigDirectorMethodCloseConnection);
		}
		MothershipApiPINVOKE.MothershipWebSocketDelegateWrapper_director_connect(this.swigCPtr, this.swigDelegate0, this.swigDelegate1);
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
					if (flag && methodInfo.IsVirtual && methodInfo.DeclaringType.IsSubclassOf(typeof(MothershipWebSocketDelegateWrapper)) && methodInfo.DeclaringType != methodInfo.GetBaseDefinition().DeclaringType)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	[MonoPInvokeCallback(typeof(MothershipWebSocketDelegateWrapper.SwigDelegateMothershipWebSocketDelegateWrapper_0))]
	private static bool SwigDirectorMethodCreateConnection(IntPtr request)
	{
		return MothershipWebSocketDelegateWrapper.selfInstance.CreateConnection(new MothershipOpenWebSocketEventArgs(request, false));
	}

	[MonoPInvokeCallback(typeof(MothershipWebSocketDelegateWrapper.SwigDelegateMothershipWebSocketDelegateWrapper_1))]
	private static bool SwigDirectorMethodCloseConnection(IntPtr request)
	{
		return MothershipWebSocketDelegateWrapper.selfInstance.CloseConnection(new MothershipCloseWebSocketEventArgs(request, false));
	}

	private HandleRef swigCPtr;

	protected static MothershipWebSocketDelegateWrapper selfInstance;

	protected bool swigCMemOwn;

	private MothershipWebSocketDelegateWrapper.SwigDelegateMothershipWebSocketDelegateWrapper_0 swigDelegate0;

	private MothershipWebSocketDelegateWrapper.SwigDelegateMothershipWebSocketDelegateWrapper_1 swigDelegate1;

	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(MothershipOpenWebSocketEventArgs)
	};

	private static Type[] swigMethodTypes1 = new Type[]
	{
		typeof(MothershipCloseWebSocketEventArgs)
	};

	public delegate bool SwigDelegateMothershipWebSocketDelegateWrapper_0(IntPtr request);

	public delegate bool SwigDelegateMothershipWebSocketDelegateWrapper_1(IntPtr request);
}
