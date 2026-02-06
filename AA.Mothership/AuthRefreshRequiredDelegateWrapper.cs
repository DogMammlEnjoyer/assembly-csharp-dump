using System;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;

public class AuthRefreshRequiredDelegateWrapper : IDisposable
{
	internal AuthRefreshRequiredDelegateWrapper(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(AuthRefreshRequiredDelegateWrapper obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(AuthRefreshRequiredDelegateWrapper obj)
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

	~AuthRefreshRequiredDelegateWrapper()
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
					MothershipApiPINVOKE.delete_AuthRefreshRequiredDelegateWrapper(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public virtual void AuthRefreshRequired(string arg0)
	{
		MothershipApiPINVOKE.AuthRefreshRequiredDelegateWrapper_AuthRefreshRequired(this.swigCPtr, arg0);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public AuthRefreshRequiredDelegateWrapper() : this(MothershipApiPINVOKE.new_AuthRefreshRequiredDelegateWrapper(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		this.SwigDirectorConnect();
	}

	private void SwigDirectorConnect()
	{
		AuthRefreshRequiredDelegateWrapper.selfInstance = this;
		if (this.SwigDerivedClassHasMethod("AuthRefreshRequired", AuthRefreshRequiredDelegateWrapper.swigMethodTypes0))
		{
			this.swigDelegate0 = new AuthRefreshRequiredDelegateWrapper.SwigDelegateAuthRefreshRequiredDelegateWrapper_0(AuthRefreshRequiredDelegateWrapper.SwigDirectorMethodAuthRefreshRequired);
		}
		MothershipApiPINVOKE.AuthRefreshRequiredDelegateWrapper_director_connect(this.swigCPtr, this.swigDelegate0);
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
					if (flag && methodInfo.IsVirtual && methodInfo.DeclaringType.IsSubclassOf(typeof(AuthRefreshRequiredDelegateWrapper)) && methodInfo.DeclaringType != methodInfo.GetBaseDefinition().DeclaringType)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	[MonoPInvokeCallback(typeof(AuthRefreshRequiredDelegateWrapper.SwigDelegateAuthRefreshRequiredDelegateWrapper_0))]
	private static void SwigDirectorMethodAuthRefreshRequired(string arg0)
	{
		AuthRefreshRequiredDelegateWrapper.selfInstance.AuthRefreshRequired(arg0);
	}

	private HandleRef swigCPtr;

	protected static AuthRefreshRequiredDelegateWrapper selfInstance;

	protected bool swigCMemOwn;

	private AuthRefreshRequiredDelegateWrapper.SwigDelegateAuthRefreshRequiredDelegateWrapper_0 swigDelegate0;

	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(string)
	};

	public delegate void SwigDelegateAuthRefreshRequiredDelegateWrapper_0(string arg0);
}
