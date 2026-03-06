using System;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal abstract class Tweak : Hook
	{
		public abstract float Tween { get; set; }

		protected Tweak(MemberInfo memberInfo, InstanceHandle instanceHandle, DebugMember attribute) : base(memberInfo, instanceHandle, attribute)
		{
		}
	}
}
