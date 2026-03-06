using System;
using System.Reflection;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal class WatchTexture : Watch
	{
		public WatchTexture(MemberInfo memberInfo, InstanceHandle instanceHandle, DebugMember attribute) : base(memberInfo, instanceHandle, attribute)
		{
			WatchTexture <>4__this = this;
			this._getter = (() => (Texture2D)memberInfo.GetValue(<>4__this._instance));
		}

		public Texture2D Texture
		{
			get
			{
				return this._getter();
			}
		}

		public override string Value
		{
			get
			{
				return string.Empty;
			}
		}

		public override string[] Values
		{
			get
			{
				return Array.Empty<string>();
			}
		}

		public override int NumberOfValues
		{
			get
			{
				return 0;
			}
		}

		private readonly Func<Texture2D> _getter;
	}
}
