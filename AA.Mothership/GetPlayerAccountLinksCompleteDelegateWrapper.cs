using System;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;

public class GetPlayerAccountLinksCompleteDelegateWrapper : MothershipRequestCompleteDelegateWrapper
{
	internal GetPlayerAccountLinksCompleteDelegateWrapper(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetPlayerAccountLinksCompleteDelegateWrapper_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetPlayerAccountLinksCompleteDelegateWrapper obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetPlayerAccountLinksCompleteDelegateWrapper obj)
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
					MothershipApiPINVOKE.delete_GetPlayerAccountLinksCompleteDelegateWrapper(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public GetPlayerAccountLinksCompleteDelegateWrapper() : this(MothershipApiPINVOKE.new_GetPlayerAccountLinksCompleteDelegateWrapper(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		this.SwigDirectorConnect();
	}

	private void SwigDirectorConnect()
	{
		GetPlayerAccountLinksCompleteDelegateWrapper.selfInstance = this;
		if (this.SwigDerivedClassHasMethod("OnCompleteCallback", GetPlayerAccountLinksCompleteDelegateWrapper.swigMethodTypes0))
		{
			this.swigDelegate0 = new GetPlayerAccountLinksCompleteDelegateWrapper.SwigDelegateGetPlayerAccountLinksCompleteDelegateWrapper_0(GetPlayerAccountLinksCompleteDelegateWrapper.SwigDirectorMethodOnCompleteCallback);
		}
		MothershipApiPINVOKE.GetPlayerAccountLinksCompleteDelegateWrapper_director_connect(this.swigCPtr, this.swigDelegate0);
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
					if (flag && methodInfo.IsVirtual && methodInfo.DeclaringType.IsSubclassOf(typeof(GetPlayerAccountLinksCompleteDelegateWrapper)) && methodInfo.DeclaringType != methodInfo.GetBaseDefinition().DeclaringType)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	[MonoPInvokeCallback(typeof(GetPlayerAccountLinksCompleteDelegateWrapper.SwigDelegateGetPlayerAccountLinksCompleteDelegateWrapper_0))]
	private static void SwigDirectorMethodOnCompleteCallback(IntPtr response, bool wasSuccess, IntPtr error, IntPtr userData)
	{
		GetPlayerAccountLinksCompleteDelegateWrapper.selfInstance.OnCompleteCallback(new MothershipResponse(response, false), wasSuccess, new MothershipError(error, false), userData);
	}

	private HandleRef swigCPtr;

	protected static GetPlayerAccountLinksCompleteDelegateWrapper selfInstance;

	private GetPlayerAccountLinksCompleteDelegateWrapper.SwigDelegateGetPlayerAccountLinksCompleteDelegateWrapper_0 swigDelegate0;

	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(MothershipResponse),
		typeof(bool),
		typeof(MothershipError),
		typeof(IntPtr)
	};

	public delegate void SwigDelegateGetPlayerAccountLinksCompleteDelegateWrapper_0(IntPtr response, bool wasSuccess, IntPtr error, IntPtr userData);
}
