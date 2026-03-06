using System;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal abstract class Watch : Hook
	{
		public abstract string Value { get; }

		public abstract string[] Values { get; }

		public abstract int NumberOfValues { get; }

		protected Watch(MemberInfo memberInfo, InstanceHandle instanceHandle, DebugMember attribute) : base(memberInfo, instanceHandle, attribute)
		{
		}
	}
}
