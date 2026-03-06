using System;
using UnityEngine;

namespace Oculus.Voice.Core.Bindings.Android
{
	public class BaseServiceBinding
	{
		protected BaseServiceBinding(AndroidJavaObject sdkInstance)
		{
			this.binding = sdkInstance;
		}

		public void Shutdown()
		{
			this.binding.Call("shutdown", Array.Empty<object>());
		}

		protected AndroidJavaObject binding;
	}
}
