using System;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal class GizmoHook : Hook
	{
		public Action<bool> SetState { get; }

		public Func<bool> GetState { get; }

		public GizmoHook(MemberInfo memberInfo, InstanceHandle instanceHandle, DebugMember attribute, Action<bool> setState, Func<bool> getState) : base(memberInfo, instanceHandle, attribute)
		{
			this.SetState = setState;
			this.GetState = getState;
			Action<bool> setState2 = this.SetState;
			if (setState2 == null)
			{
				return;
			}
			setState2(attribute.ShowGizmoByDefault);
		}
	}
}
