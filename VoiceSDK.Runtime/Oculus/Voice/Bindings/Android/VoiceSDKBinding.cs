using System;
using Meta.WitAi.Configuration;
using Oculus.Voice.Core.Bindings.Android;
using UnityEngine;
using UnityEngine.Scripting;

namespace Oculus.Voice.Bindings.Android
{
	public class VoiceSDKBinding : BaseServiceBinding
	{
		[Preserve]
		public VoiceSDKBinding(AndroidJavaObject sdkInstance) : base(sdkInstance)
		{
		}

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

		public bool MicActive
		{
			get
			{
				return this.binding.Call<bool>("isMicActive", Array.Empty<object>());
			}
		}

		public bool PlatformSupportsWit
		{
			get
			{
				return this.binding.Call<bool>("isSupported", Array.Empty<object>());
			}
		}

		public void Activate(string text, WitRequestOptions options)
		{
			this.binding.Call("activate", new object[]
			{
				text,
				options.ToJsonString()
			});
		}

		public void Activate(WitRequestOptions options)
		{
			this.binding.Call("activate", new object[]
			{
				options.ToJsonString()
			});
		}

		public void ActivateImmediately(WitRequestOptions options)
		{
			this.binding.Call("activateImmediately", new object[]
			{
				options.ToJsonString()
			});
		}

		public void Deactivate()
		{
			this.binding.Call("deactivate", Array.Empty<object>());
		}

		public void DeactivateAndAbortRequest()
		{
			this.binding.Call("deactivateAndAbortRequest", Array.Empty<object>());
		}

		public void Deactivate(string requestID)
		{
			this.binding.Call("deactivate", new object[]
			{
				requestID
			});
		}

		public void DeactivateAndAbortRequest(string requestID)
		{
			this.binding.Call("deactivateAndAbortRequest", new object[]
			{
				requestID
			});
		}

		public void SetRuntimeConfiguration(WitRuntimeConfiguration configuration)
		{
			this.binding.Call("setRuntimeConfig", new object[]
			{
				new VoiceSDKConfigBinding(configuration).ToJavaObject()
			});
		}

		public void SetListener(VoiceSDKListenerBinding listener)
		{
			this.binding.Call("setListener", new object[]
			{
				listener
			});
		}

		public void Connect()
		{
			this.binding.Call<bool>("connect", Array.Empty<object>());
		}
	}
}
