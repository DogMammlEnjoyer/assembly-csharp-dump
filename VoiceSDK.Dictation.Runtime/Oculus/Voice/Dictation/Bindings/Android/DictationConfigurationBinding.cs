using System;
using Meta.WitAi;
using Meta.WitAi.Configuration;
using Oculus.Voice.Dictation.Configuration;
using UnityEngine;

namespace Oculus.Voice.Dictation.Bindings.Android
{
	public class DictationConfigurationBinding
	{
		public DictationConfigurationBinding(WitDictationRuntimeConfiguration runtimeConfiguration)
		{
			if (runtimeConfiguration == null)
			{
				VLog.W("No dictation config has been defined. Using the default configuration.", null);
				this._dictationConfiguration = new DictationConfiguration();
				return;
			}
			this._dictationConfiguration = runtimeConfiguration.dictationConfiguration;
			this._runtimeConfiguration = runtimeConfiguration;
		}

		public AndroidJavaObject ToJavaObject()
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.oculus.assistant.api.voicesdk.dictation.PlatformDictationConfiguration", Array.Empty<object>());
			androidJavaObject.Set<bool>("multiPhrase", this._dictationConfiguration.multiPhrase);
			androidJavaObject.Set<string>("scenario", this._dictationConfiguration.scenario);
			androidJavaObject.Set<string>("inputType", this._dictationConfiguration.inputType);
			if (this._runtimeConfiguration != null)
			{
				int num = (int)this._runtimeConfiguration.maxRecordingTime;
				if (num < 0)
				{
					num = this.MAX_PLATFORM_SUPPORTED_RECORDING_TIME_SECONDS;
				}
				androidJavaObject.Set<int>("interactionTimeoutSeconds", num);
			}
			return androidJavaObject;
		}

		private readonly WitDictationRuntimeConfiguration _runtimeConfiguration;

		private readonly DictationConfiguration _dictationConfiguration;

		private readonly int MAX_PLATFORM_SUPPORTED_RECORDING_TIME_SECONDS = 300;
	}
}
