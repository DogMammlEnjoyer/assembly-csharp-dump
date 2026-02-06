using System;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;

public class GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper : MothershipRequestCompleteDelegateWrapper
{
	internal GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper obj)
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
					MothershipApiPINVOKE.delete_GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper() : this(MothershipApiPINVOKE.new_GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		this.SwigDirectorConnect();
	}

	private void SwigDirectorConnect()
	{
		GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper.selfInstance = this;
		if (this.SwigDerivedClassHasMethod("OnCompleteCallback", GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper.swigMethodTypes0))
		{
			this.swigDelegate0 = new GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper.SwigDelegateGetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper_0(GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper.SwigDirectorMethodOnCompleteCallback);
		}
		MothershipApiPINVOKE.GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper_director_connect(this.swigCPtr, this.swigDelegate0);
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
					if (flag && methodInfo.IsVirtual && methodInfo.DeclaringType.IsSubclassOf(typeof(GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper)) && methodInfo.DeclaringType != methodInfo.GetBaseDefinition().DeclaringType)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	[MonoPInvokeCallback(typeof(GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper.SwigDelegateGetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper_0))]
	private static void SwigDirectorMethodOnCompleteCallback(IntPtr response, bool wasSuccess, IntPtr error, IntPtr userData)
	{
		GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper.selfInstance.OnCompleteCallback(new MothershipResponse(response, false), wasSuccess, new MothershipError(error, false), userData);
	}

	private HandleRef swigCPtr;

	protected static GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper selfInstance;

	private GetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper.SwigDelegateGetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper_0 swigDelegate0;

	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(MothershipResponse),
		typeof(bool),
		typeof(MothershipError),
		typeof(IntPtr)
	};

	public delegate void SwigDelegateGetProgressionTrackValuesForPlayerCompleteServerDelegateWrapper_0(IntPtr response, bool wasSuccess, IntPtr error, IntPtr userData);
}
