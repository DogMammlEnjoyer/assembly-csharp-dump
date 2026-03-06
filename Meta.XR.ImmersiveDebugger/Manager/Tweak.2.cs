using System;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal class Tweak<T> : Tweak
	{
		public override float Tween
		{
			get
			{
				return Tweak<T>.InverseLerp(this._min, this._max, this._getter());
			}
			set
			{
				this._setter(Tweak<T>.Lerp(this._min, this._max, value));
			}
		}

		public Tweak(MemberInfo memberInfo, InstanceHandle instanceHandle, DebugMember attribute) : base(memberInfo, instanceHandle, attribute)
		{
			Tweak<T> <>4__this = this;
			this._min = Tweak<T>.FromFloat(attribute.Min);
			this._max = Tweak<T>.FromFloat(attribute.Max);
			this._getter = (() => (T)((object)memberInfo.GetValue(<>4__this._instance)));
			this._setter = delegate(T value)
			{
				memberInfo.SetValue(<>4__this._instance, value);
			};
		}

		public static Func<T, T, T, float> InverseLerp;

		public static Func<T, T, float, T> Lerp;

		public static Func<float, T> FromFloat;

		private readonly Func<T> _getter;

		private readonly Action<T> _setter;

		private readonly T _min;

		private readonly T _max;
	}
}
