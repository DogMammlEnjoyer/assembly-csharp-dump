using System;
using Oculus.Voice.Core.Bindings.Android;
using UnityEngine;

namespace Oculus.Voice.Dictation.Bindings.Android
{
	public class PlatformDictationSDKBinding : BaseServiceBinding
	{
		public bool Active
		{
			get
			{
				return this.binding.Call<bool>("isActive", Array.Empty<object>());
			}
		}

		public bool IsRequestActive
		{
			get
			{
				return this.binding.Call<bool>("isRequestActive", Array.Empty<object>());
			}
		}

		public bool IsSupported
		{
			get
			{
				return this.binding.Call<bool>("isSupported", Array.Empty<object>());
			}
		}

		public PlatformDictationSDKBinding(AndroidJavaObject sdkInstance) : base(sdkInstance)
		{
		}

		public void StartDictation(DictationConfigurationBinding configuration)
		{
			this.binding.Call("startDictation", new object[]
			{
				configuration.ToJavaObject()
			});
		}

		public void StopDictation()
		{
			this.binding.Call("stopDictation", Array.Empty<object>());
		}

		public void SetListener(DictationListenerBinding listenerBinding)
		{
			this.binding.Call("setListener", new object[]
			{
				listenerBinding
			});
		}
	}
}
