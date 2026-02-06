using System;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;

public class MothershipLogDelegateWrapper : IDisposable
{
	internal MothershipLogDelegateWrapper(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(MothershipLogDelegateWrapper obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(MothershipLogDelegateWrapper obj)
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

	~MothershipLogDelegateWrapper()
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
					MothershipApiPINVOKE.delete_MothershipLogDelegateWrapper(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public virtual void OnLogCallback(MothershipLogLevel level, string message)
	{
		MothershipApiPINVOKE.MothershipLogDelegateWrapper_OnLogCallback(this.swigCPtr, (int)level, message);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public MothershipLogDelegateWrapper() : this(MothershipApiPINVOKE.new_MothershipLogDelegateWrapper(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		this.SwigDirectorConnect();
	}

	private void SwigDirectorConnect()
	{
		MothershipLogDelegateWrapper.selfInstance = this;
		if (this.SwigDerivedClassHasMethod("OnLogCallback", MothershipLogDelegateWrapper.swigMethodTypes0))
		{
			this.swigDelegate0 = new MothershipLogDelegateWrapper.SwigDelegateMothershipLogDelegateWrapper_0(MothershipLogDelegateWrapper.SwigDirectorMethodOnLogCallback);
		}
		MothershipApiPINVOKE.MothershipLogDelegateWrapper_director_connect(this.swigCPtr, this.swigDelegate0);
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
					if (flag && methodInfo.IsVirtual && methodInfo.DeclaringType.IsSubclassOf(typeof(MothershipLogDelegateWrapper)) && methodInfo.DeclaringType != methodInfo.GetBaseDefinition().DeclaringType)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	[MonoPInvokeCallback(typeof(MothershipLogDelegateWrapper.SwigDelegateMothershipLogDelegateWrapper_0))]
	private static void SwigDirectorMethodOnLogCallback(int level, string message)
	{
		MothershipLogDelegateWrapper.selfInstance.OnLogCallback((MothershipLogLevel)level, message);
	}

	private HandleRef swigCPtr;

	protected static MothershipLogDelegateWrapper selfInstance;

	protected bool swigCMemOwn;

	private MothershipLogDelegateWrapper.SwigDelegateMothershipLogDelegateWrapper_0 swigDelegate0;

	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(MothershipLogLevel),
		typeof(string)
	};

	public delegate void SwigDelegateMothershipLogDelegateWrapper_0(int level, string message);
}
