using System;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;

public class VerifyTokenCompleteDelegateWrapper : MothershipRequestCompleteDelegateWrapper
{
	internal VerifyTokenCompleteDelegateWrapper(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.VerifyTokenCompleteDelegateWrapper_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(VerifyTokenCompleteDelegateWrapper obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(VerifyTokenCompleteDelegateWrapper obj)
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
					MothershipApiPINVOKE.delete_VerifyTokenCompleteDelegateWrapper(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public VerifyTokenCompleteDelegateWrapper() : this(MothershipApiPINVOKE.new_VerifyTokenCompleteDelegateWrapper(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		this.SwigDirectorConnect();
	}

	private void SwigDirectorConnect()
	{
		VerifyTokenCompleteDelegateWrapper.selfInstance = this;
		if (this.SwigDerivedClassHasMethod("OnCompleteCallback", VerifyTokenCompleteDelegateWrapper.swigMethodTypes0))
		{
			this.swigDelegate0 = new VerifyTokenCompleteDelegateWrapper.SwigDelegateVerifyTokenCompleteDelegateWrapper_0(VerifyTokenCompleteDelegateWrapper.SwigDirectorMethodOnCompleteCallback);
		}
		MothershipApiPINVOKE.VerifyTokenCompleteDelegateWrapper_director_connect(this.swigCPtr, this.swigDelegate0);
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
					if (flag && methodInfo.IsVirtual && methodInfo.DeclaringType.IsSubclassOf(typeof(VerifyTokenCompleteDelegateWrapper)) && methodInfo.DeclaringType != methodInfo.GetBaseDefinition().DeclaringType)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	[MonoPInvokeCallback(typeof(VerifyTokenCompleteDelegateWrapper.SwigDelegateVerifyTokenCompleteDelegateWrapper_0))]
	private static void SwigDirectorMethodOnCompleteCallback(IntPtr response, bool wasSuccess, IntPtr error, IntPtr userData)
	{
		VerifyTokenCompleteDelegateWrapper.selfInstance.OnCompleteCallback(new MothershipResponse(response, false), wasSuccess, new MothershipError(error, false), userData);
	}

	private HandleRef swigCPtr;

	protected static VerifyTokenCompleteDelegateWrapper selfInstance;

	private VerifyTokenCompleteDelegateWrapper.SwigDelegateVerifyTokenCompleteDelegateWrapper_0 swigDelegate0;

	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(MothershipResponse),
		typeof(bool),
		typeof(MothershipError),
		typeof(IntPtr)
	};

	public delegate void SwigDelegateVerifyTokenCompleteDelegateWrapper_0(IntPtr response, bool wasSuccess, IntPtr error, IntPtr userData);
}
