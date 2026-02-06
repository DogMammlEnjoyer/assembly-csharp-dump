using System;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;

public class GetProgressionTreeBindingsCompleteDelegateWrapper : MothershipRequestCompleteDelegateWrapper
{
	internal GetProgressionTreeBindingsCompleteDelegateWrapper(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetProgressionTreeBindingsCompleteDelegateWrapper_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetProgressionTreeBindingsCompleteDelegateWrapper obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetProgressionTreeBindingsCompleteDelegateWrapper obj)
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
					MothershipApiPINVOKE.delete_GetProgressionTreeBindingsCompleteDelegateWrapper(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public GetProgressionTreeBindingsCompleteDelegateWrapper() : this(MothershipApiPINVOKE.new_GetProgressionTreeBindingsCompleteDelegateWrapper(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		this.SwigDirectorConnect();
	}

	private void SwigDirectorConnect()
	{
		GetProgressionTreeBindingsCompleteDelegateWrapper.selfInstance = this;
		if (this.SwigDerivedClassHasMethod("OnCompleteCallback", GetProgressionTreeBindingsCompleteDelegateWrapper.swigMethodTypes0))
		{
			this.swigDelegate0 = new GetProgressionTreeBindingsCompleteDelegateWrapper.SwigDelegateGetProgressionTreeBindingsCompleteDelegateWrapper_0(GetProgressionTreeBindingsCompleteDelegateWrapper.SwigDirectorMethodOnCompleteCallback);
		}
		MothershipApiPINVOKE.GetProgressionTreeBindingsCompleteDelegateWrapper_director_connect(this.swigCPtr, this.swigDelegate0);
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
					if (flag && methodInfo.IsVirtual && methodInfo.DeclaringType.IsSubclassOf(typeof(GetProgressionTreeBindingsCompleteDelegateWrapper)) && methodInfo.DeclaringType != methodInfo.GetBaseDefinition().DeclaringType)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	[MonoPInvokeCallback(typeof(GetProgressionTreeBindingsCompleteDelegateWrapper.SwigDelegateGetProgressionTreeBindingsCompleteDelegateWrapper_0))]
	private static void SwigDirectorMethodOnCompleteCallback(IntPtr response, bool wasSuccess, IntPtr error, IntPtr userData)
	{
		GetProgressionTreeBindingsCompleteDelegateWrapper.selfInstance.OnCompleteCallback(new MothershipResponse(response, false), wasSuccess, new MothershipError(error, false), userData);
	}

	private HandleRef swigCPtr;

	protected static GetProgressionTreeBindingsCompleteDelegateWrapper selfInstance;

	private GetProgressionTreeBindingsCompleteDelegateWrapper.SwigDelegateGetProgressionTreeBindingsCompleteDelegateWrapper_0 swigDelegate0;

	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(MothershipResponse),
		typeof(bool),
		typeof(MothershipError),
		typeof(IntPtr)
	};

	public delegate void SwigDelegateGetProgressionTreeBindingsCompleteDelegateWrapper_0(IntPtr response, bool wasSuccess, IntPtr error, IntPtr userData);
}
