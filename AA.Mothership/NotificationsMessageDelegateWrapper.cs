using System;
using System.Reflection;
using System.Runtime.InteropServices;
using AOT;

public class NotificationsMessageDelegateWrapper : MothershipWebSocketMessageDelegateWrapper
{
	internal NotificationsMessageDelegateWrapper(IntPtr cPtr, bool cMemoryOwn) : base(MothershipApiPINVOKE.NotificationsMessageDelegateWrapper_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(NotificationsMessageDelegateWrapper obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(NotificationsMessageDelegateWrapper obj)
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
					MothershipApiPINVOKE.delete_NotificationsMessageDelegateWrapper(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
			base.Dispose(disposing);
		}
	}

	public NotificationsMessageDelegateWrapper() : this(MothershipApiPINVOKE.new_NotificationsMessageDelegateWrapper(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		this.SwigDirectorConnect();
	}

	private void SwigDirectorConnect()
	{
		NotificationsMessageDelegateWrapper.selfInstance = this;
		if (this.SwigDerivedClassHasMethod("OnOpenCallback", NotificationsMessageDelegateWrapper.swigMethodTypes0))
		{
			this.swigDelegate0 = new NotificationsMessageDelegateWrapper.SwigDelegateNotificationsMessageDelegateWrapper_0(NotificationsMessageDelegateWrapper.SwigDirectorMethodOnOpenCallback);
		}
		if (this.SwigDerivedClassHasMethod("OnMessageCallback", NotificationsMessageDelegateWrapper.swigMethodTypes1))
		{
			this.swigDelegate1 = new NotificationsMessageDelegateWrapper.SwigDelegateNotificationsMessageDelegateWrapper_1(NotificationsMessageDelegateWrapper.SwigDirectorMethodOnMessageCallback);
		}
		if (this.SwigDerivedClassHasMethod("OnCloseCallback", NotificationsMessageDelegateWrapper.swigMethodTypes2))
		{
			this.swigDelegate2 = new NotificationsMessageDelegateWrapper.SwigDelegateNotificationsMessageDelegateWrapper_2(NotificationsMessageDelegateWrapper.SwigDirectorMethodOnCloseCallback);
		}
		if (this.SwigDerivedClassHasMethod("OnErrorCallback", NotificationsMessageDelegateWrapper.swigMethodTypes3))
		{
			this.swigDelegate3 = new NotificationsMessageDelegateWrapper.SwigDelegateNotificationsMessageDelegateWrapper_3(NotificationsMessageDelegateWrapper.SwigDirectorMethodOnErrorCallback);
		}
		MothershipApiPINVOKE.NotificationsMessageDelegateWrapper_director_connect(this.swigCPtr, this.swigDelegate0, this.swigDelegate1, this.swigDelegate2, this.swigDelegate3);
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
					if (flag && methodInfo.IsVirtual && methodInfo.DeclaringType.IsSubclassOf(typeof(NotificationsMessageDelegateWrapper)) && methodInfo.DeclaringType != methodInfo.GetBaseDefinition().DeclaringType)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	[MonoPInvokeCallback(typeof(NotificationsMessageDelegateWrapper.SwigDelegateNotificationsMessageDelegateWrapper_0))]
	private static void SwigDirectorMethodOnOpenCallback(IntPtr userData)
	{
		NotificationsMessageDelegateWrapper.selfInstance.OnOpenCallback(userData);
	}

	[MonoPInvokeCallback(typeof(NotificationsMessageDelegateWrapper.SwigDelegateNotificationsMessageDelegateWrapper_1))]
	private static void SwigDirectorMethodOnMessageCallback(IntPtr message, IntPtr userData)
	{
		NotificationsMessageDelegateWrapper.selfInstance.OnMessageCallback(new MothershipWebSocketMessage(message, false), userData);
	}

	[MonoPInvokeCallback(typeof(NotificationsMessageDelegateWrapper.SwigDelegateNotificationsMessageDelegateWrapper_2))]
	private static void SwigDirectorMethodOnCloseCallback(IntPtr userData)
	{
		NotificationsMessageDelegateWrapper.selfInstance.OnCloseCallback(userData);
	}

	[MonoPInvokeCallback(typeof(NotificationsMessageDelegateWrapper.SwigDelegateNotificationsMessageDelegateWrapper_3))]
	private static void SwigDirectorMethodOnErrorCallback(IntPtr userData)
	{
		NotificationsMessageDelegateWrapper.selfInstance.OnErrorCallback(userData);
	}

	private HandleRef swigCPtr;

	protected static NotificationsMessageDelegateWrapper selfInstance;

	private NotificationsMessageDelegateWrapper.SwigDelegateNotificationsMessageDelegateWrapper_0 swigDelegate0;

	private NotificationsMessageDelegateWrapper.SwigDelegateNotificationsMessageDelegateWrapper_1 swigDelegate1;

	private NotificationsMessageDelegateWrapper.SwigDelegateNotificationsMessageDelegateWrapper_2 swigDelegate2;

	private NotificationsMessageDelegateWrapper.SwigDelegateNotificationsMessageDelegateWrapper_3 swigDelegate3;

	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(IntPtr)
	};

	private static Type[] swigMethodTypes1 = new Type[]
	{
		typeof(MothershipWebSocketMessage),
		typeof(IntPtr)
	};

	private static Type[] swigMethodTypes2 = new Type[]
	{
		typeof(IntPtr)
	};

	private static Type[] swigMethodTypes3 = new Type[]
	{
		typeof(IntPtr)
	};

	public delegate void SwigDelegateNotificationsMessageDelegateWrapper_0(IntPtr userData);

	public delegate void SwigDelegateNotificationsMessageDelegateWrapper_1(IntPtr message, IntPtr userData);

	public delegate void SwigDelegateNotificationsMessageDelegateWrapper_2(IntPtr userData);

	public delegate void SwigDelegateNotificationsMessageDelegateWrapper_3(IntPtr userData);
}
