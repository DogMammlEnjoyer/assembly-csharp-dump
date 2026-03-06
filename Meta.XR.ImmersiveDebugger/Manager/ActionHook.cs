using System;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal class ActionHook : Hook
	{
		internal Action Delegate { get; private set; }

		internal ActionHook(MemberInfo memberInfo, InstanceHandle instanceHandle, DebugMember attribute) : base(memberInfo, instanceHandle, attribute)
		{
			ActionHook <>4__this = this;
			this.Delegate = delegate()
			{
				MethodInfo methodInfo = memberInfo as MethodInfo;
				if (methodInfo == null)
				{
					return;
				}
				methodInfo.Invoke(<>4__this._instance, null);
			};
		}
	}
}
