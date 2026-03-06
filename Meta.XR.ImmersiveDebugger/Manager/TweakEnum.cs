using System;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal class TweakEnum : Tweak
	{
		public MemberInfo Member
		{
			get
			{
				return this._memberInfo;
			}
		}

		public string Value
		{
			get
			{
				return this._memberInfo.GetValue(this._instance).ToString();
			}
			set
			{
				object value2 = Enum.Parse(this._enumType, value);
				this._memberInfo.SetValue(this._instance, value2);
			}
		}

		public TweakEnum(MemberInfo memberInfo, InstanceHandle instanceHandle, DebugMember attribute, Type enumType) : base(memberInfo, instanceHandle, attribute)
		{
			this._enumType = enumType;
		}

		public override float Tween { get; set; }

		private readonly Type _enumType;
	}
}
