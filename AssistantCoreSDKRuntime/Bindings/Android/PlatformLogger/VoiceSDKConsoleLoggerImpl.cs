using System;
using Oculus.Voice.Core.Bindings.Interfaces;
using Oculus.Voice.Core.Utilities;
using UnityEngine;
using UnityEngine.Device;

namespace Oculus.Voice.Core.Bindings.Android.PlatformLogger
{
	public class VoiceSDKConsoleLoggerImpl : IVoiceSDKLogger
	{
		public bool IsUsingPlatformIntegration { get; set; }

		public string WitApplication { get; set; }

		public string PackageName { get; }

		public bool ShouldLogToConsole { get; set; }

		public VoiceSDKConsoleLoggerImpl()
		{
			this.PackageName = UnityEngine.Device.Application.identifier;
		}

		public void LogInteractionStart(string requestId, string witApi)
		{
			if (!this.ShouldLogToConsole)
			{
				return;
			}
			this.loggedFirstTranscriptionTime = false;
			Debug.Log(VoiceSDKConsoleLoggerImpl.TAG + ": Interaction started with request ID: " + requestId);
			Debug.Log(VoiceSDKConsoleLoggerImpl.TAG + ": WitApi: " + witApi);
			Debug.Log(VoiceSDKConsoleLoggerImpl.TAG + ": request_start_time: " + DateTimeUtility.ElapsedMilliseconds.ToString());
			Debug.Log(VoiceSDKConsoleLoggerImpl.TAG + ": WitAppID: " + this.WitApplication);
			Debug.Log(VoiceSDKConsoleLoggerImpl.TAG + ": PackageName: " + this.PackageName);
		}

		public void LogInteractionEndSuccess()
		{
			if (!this.ShouldLogToConsole)
			{
				return;
			}
			Debug.Log(VoiceSDKConsoleLoggerImpl.TAG + ": Interaction finished successfully");
			Debug.Log(VoiceSDKConsoleLoggerImpl.TAG + ": request_end_time: " + DateTimeUtility.ElapsedMilliseconds.ToString());
		}

		public void LogInteractionEndFailure(string errorMessage)
		{
			if (!this.ShouldLogToConsole)
			{
				return;
			}
			Debug.Log(VoiceSDKConsoleLoggerImpl.TAG + ": Interaction finished with error: " + errorMessage);
			Debug.Log(VoiceSDKConsoleLoggerImpl.TAG + ": request_end_time: " + DateTimeUtility.ElapsedMilliseconds.ToString());
		}

		public void LogInteractionPoint(string interactionPoint)
		{
			if (!this.ShouldLogToConsole)
			{
				return;
			}
			Debug.Log(VoiceSDKConsoleLoggerImpl.TAG + ": Interaction point: " + interactionPoint);
			Debug.Log(string.Concat(new string[]
			{
				VoiceSDKConsoleLoggerImpl.TAG,
				": ",
				interactionPoint,
				"_start_time: ",
				DateTimeUtility.ElapsedMilliseconds.ToString()
			}));
		}

		public void LogAnnotation(string annotationKey, string annotationValue)
		{
			if (!this.ShouldLogToConsole)
			{
				return;
			}
			Debug.Log(string.Concat(new string[]
			{
				VoiceSDKConsoleLoggerImpl.TAG,
				": Logging key-value pair: ",
				annotationKey,
				"::",
				annotationValue
			}));
		}

		public void LogFirstTranscriptionTime()
		{
			if (!this.loggedFirstTranscriptionTime)
			{
				this.loggedFirstTranscriptionTime = true;
				this.LogInteractionPoint("firstPartialTranscriptionTime");
			}
		}

		private static readonly string TAG = "VoiceSDKConsoleLogger";

		private bool loggedFirstTranscriptionTime;
	}
}
