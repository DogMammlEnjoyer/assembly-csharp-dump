using System;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;

public class GetTransactionCatalogItemCompleteDelegateWrapper : MothershipRequestCompleteDelegateWrapper
{
	internal GetTransactionCatalogItemCompleteDelegateWrapper(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetTransactionCatalogItemCompleteDelegateWrapper_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetTransactionCatalogItemCompleteDelegateWrapper obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetTransactionCatalogItemCompleteDelegateWrapper obj)
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
					MothershipApiPINVOKE.delete_GetTransactionCatalogItemCompleteDelegateWrapper(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public GetTransactionCatalogItemCompleteDelegateWrapper() : this(MothershipApiPINVOKE.new_GetTransactionCatalogItemCompleteDelegateWrapper(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		this.SwigDirectorConnect();
	}

	private void SwigDirectorConnect()
	{
		GetTransactionCatalogItemCompleteDelegateWrapper.selfInstance = this;
		if (this.SwigDerivedClassHasMethod("OnCompleteCallback", GetTransactionCatalogItemCompleteDelegateWrapper.swigMethodTypes0))
		{
			this.swigDelegate0 = new GetTransactionCatalogItemCompleteDelegateWrapper.SwigDelegateGetTransactionCatalogItemCompleteDelegateWrapper_0(GetTransactionCatalogItemCompleteDelegateWrapper.SwigDirectorMethodOnCompleteCallback);
		}
		MothershipApiPINVOKE.GetTransactionCatalogItemCompleteDelegateWrapper_director_connect(this.swigCPtr, this.swigDelegate0);
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
					if (flag && methodInfo.IsVirtual && methodInfo.DeclaringType.IsSubclassOf(typeof(GetTransactionCatalogItemCompleteDelegateWrapper)) && methodInfo.DeclaringType != methodInfo.GetBaseDefinition().DeclaringType)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	[MonoPInvokeCallback(typeof(GetTransactionCatalogItemCompleteDelegateWrapper.SwigDelegateGetTransactionCatalogItemCompleteDelegateWrapper_0))]
	private static void SwigDirectorMethodOnCompleteCallback(IntPtr response, bool wasSuccess, IntPtr error, IntPtr userData)
	{
		GetTransactionCatalogItemCompleteDelegateWrapper.selfInstance.OnCompleteCallback(new MothershipResponse(response, false), wasSuccess, new MothershipError(error, false), userData);
	}

	private HandleRef swigCPtr;

	protected static GetTransactionCatalogItemCompleteDelegateWrapper selfInstance;

	private GetTransactionCatalogItemCompleteDelegateWrapper.SwigDelegateGetTransactionCatalogItemCompleteDelegateWrapper_0 swigDelegate0;

	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(MothershipResponse),
		typeof(bool),
		typeof(MothershipError),
		typeof(IntPtr)
	};

	public delegate void SwigDelegateGetTransactionCatalogItemCompleteDelegateWrapper_0(IntPtr response, bool wasSuccess, IntPtr error, IntPtr userData);
}
