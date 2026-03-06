using System;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal class Watch<T> : Watch
	{
		public static Watch<T>.ToDisplayStringSignature ToDisplayStringsDelegate { get; private set; } = null;

		public static int NumberOfDisplayStrings { get; private set; } = 1;

		public override int NumberOfValues
		{
			get
			{
				return Watch<T>.NumberOfDisplayStrings;
			}
		}

		internal static void ResetBuffer()
		{
			Watch<T>._buffer = new string[Watch<T>.NumberOfDisplayStrings];
		}

		public static void Setup(Watch<T>.ToDisplayStringSignature del, int numberOfValues)
		{
			Watch<T>.ToDisplayStringsDelegate = del;
			Watch<T>.NumberOfDisplayStrings = numberOfValues;
			Watch<T>.ResetBuffer();
		}

		public static string[] ToDisplayStrings(T value)
		{
			if (Watch<T>.ToDisplayStringsDelegate != null)
			{
				Watch<T>.ToDisplayStringsDelegate(value, ref Watch<T>._buffer);
			}
			else
			{
				Watch<T>._buffer[0] = ((value != null) ? value.ToString() : "");
			}
			return Watch<T>._buffer;
		}

		public override string[] Values
		{
			get
			{
				return Watch<T>.ToDisplayStrings(this._getter());
			}
		}

		public override string Value
		{
			get
			{
				return this.Values[0];
			}
		}

		public Watch(MemberInfo memberInfo, InstanceHandle instanceHandle, DebugMember attribute) : base(memberInfo, instanceHandle, attribute)
		{
			Watch<T> <>4__this = this;
			this._getter = (() => (T)((object)memberInfo.GetValue(<>4__this._instance)));
		}

		private static string[] _buffer = new string[1];

		private readonly Func<T> _getter;

		public delegate void ToDisplayStringSignature(T value, ref string[] valuesContainer);
	}
}
