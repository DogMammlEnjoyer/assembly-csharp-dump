using System;
using Meta.WitAi.Configuration;
using UnityEngine;

namespace Oculus.Voice.Bindings.Android
{
	public class VoiceSDKConfigBinding
	{
		public VoiceSDKConfigBinding(WitRuntimeConfiguration config)
		{
			this.configuration = config;
		}

		public AndroidJavaObject ToJavaObject()
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.oculus.assistant.api.voicesdk.immersivevoicecommands.WitConfiguration", Array.Empty<object>());
			androidJavaObject.Set<string>("clientAccessToken", this.configuration.witConfiguration.GetClientAccessToken());
			AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("com.oculus.assistant.api.voicesdk.immersivevoicecommands.WitRuntimeConfiguration", Array.Empty<object>());
			androidJavaObject2.Set<AndroidJavaObject>("witConfiguration", androidJavaObject);
			androidJavaObject2.Set<float>("minKeepAliveVolume", this.configuration.minKeepAliveVolume);
			androidJavaObject2.Set<float>("minKeepAliveTimeInSeconds", this.configuration.minKeepAliveTimeInSeconds);
			androidJavaObject2.Set<float>("minTranscriptionKeepAliveTimeInSeconds", this.configuration.minTranscriptionKeepAliveTimeInSeconds);
			androidJavaObject2.Set<float>("maxRecordingTime", this.configuration.maxRecordingTime);
			androidJavaObject2.Set<float>("soundWakeThreshold", this.configuration.soundWakeThreshold);
			androidJavaObject2.Set<int>("sampleLengthInMs", this.configuration.sampleLengthInMs);
			androidJavaObject2.Set<float>("micBufferLengthInSeconds", this.configuration.micBufferLengthInSeconds);
			androidJavaObject2.Set<bool>("sendAudioToWit", this.configuration.sendAudioToWit);
			androidJavaObject2.Set<float>("preferredActivationOffset", this.configuration.preferredActivationOffset);
			androidJavaObject2.Set<string>("clientName", "wit-unity");
			androidJavaObject2.Set<string>("serverVersion", "20250213");
			return androidJavaObject2;
		}

		private WitRuntimeConfiguration configuration;
	}
}
