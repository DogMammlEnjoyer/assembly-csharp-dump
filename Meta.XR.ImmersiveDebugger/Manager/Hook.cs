using System;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal abstract class Hook
	{
		public DebugMember Attribute
		{
			get
			{
				return this._attribute;
			}
		}

		public MemberInfo MemberInfo
		{
			get
			{
				return this._memberInfo;
			}
		}

		public bool Valid
		{
			get
			{
				return this._instanceHandle.Valid;
			}
		}

		protected Hook(MemberInfo memberInfo, InstanceHandle instanceHandle, DebugMember attribute)
		{
			this._memberInfo = memberInfo;
			this._instanceHandle = instanceHandle;
			this._instance = this._instanceHandle.Instance;
			this._attribute = attribute;
		}

		public bool Matches(MemberInfo memberInfo, InstanceHandle instance)
		{
			return this._memberInfo == memberInfo && this._instanceHandle.Equals(instance);
		}

		private readonly InstanceHandle _instanceHandle;

		private readonly DebugMember _attribute;

		protected readonly MemberInfo _memberInfo;

		protected readonly object _instance;
	}
}
