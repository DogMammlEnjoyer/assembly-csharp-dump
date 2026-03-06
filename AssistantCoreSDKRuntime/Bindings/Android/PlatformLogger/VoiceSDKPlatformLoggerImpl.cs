using System;
using Oculus.Voice.Core.Bindings.Interfaces;
using Oculus.Voice.Core.Utilities;
using UnityEngine;

namespace Oculus.Voice.Core.Bindings.Android.PlatformLogger
{
	public class VoiceSDKPlatformLoggerImpl : BaseAndroidConnectionImpl<VoiceSDKLoggerBinding>, IVoiceSDKLogger
	{
		public bool IsUsingPlatformIntegration { get; set; }

		public string WitApplication { get; set; }

		public string PackageName { get; }

		public bool ShouldLogToConsole
		{
			get
			{
				return this.consoleLoggerImpl.ShouldLogToConsole;
			}
			set
			{
				this.consoleLoggerImpl.ShouldLogToConsole = value;
			}
		}

		public VoiceSDKPlatformLoggerImpl() : base("com.oculus.assistant.api.unity.logging.UnityPlatformLoggerServiceFragment")
		{
			this.PackageName = Application.identifier;
		}

		public override void Connect(string version)
		{
			base.Connect(version);
			if (this.service == null)
			{
				return;
			}
			this.service.Connect();
			Debug.Log("Logging Platform integration initialization complete.");
		}

		public override void Disconnect()
		{
			Debug.Log("Logging Platform integration shutdown");
			base.Disconnect();
		}

		public void LogInteractionStart(string requestId, string witApi)
		{
			this.loggedFirstTranscriptionTime = false;
			this.consoleLoggerImpl.LogInteractionStart(requestId, witApi);
			VoiceSDKLoggerBinding service = this.service;
			if (service != null)
			{
				service.LogInteractionStart(requestId, DateTimeUtility.ElapsedMilliseconds.ToString());
			}
			this.LogAnnotation("isUsingPlatform", this.IsUsingPlatformIntegration.ToString());
			this.LogAnnotation("witApi", witApi);
			this.LogAnnotation("witAppId", this.WitApplication);
			this.LogAnnotation("package", this.PackageName);
		}

		public void LogInteractionEndSuccess()
		{
			this.consoleLoggerImpl.LogInteractionEndSuccess();
			VoiceSDKLoggerBinding service = this.service;
			if (service == null)
			{
				return;
			}
			service.LogInteractionEndSuccess(DateTimeUtility.ElapsedMilliseconds.ToString());
		}

		public void LogInteractionEndFailure(string errorMessage)
		{
			this.consoleLoggerImpl.LogInteractionEndFailure(errorMessage);
			VoiceSDKLoggerBinding service = this.service;
			if (service == null)
			{
				return;
			}
			service.LogInteractionEndFailure(DateTimeUtility.ElapsedMilliseconds.ToString(), errorMessage);
		}

		public void LogInteractionPoint(string interactionPoint)
		{
			this.consoleLoggerImpl.LogInteractionPoint(interactionPoint);
			VoiceSDKLoggerBinding service = this.service;
			if (service == null)
			{
				return;
			}
			service.LogInteractionPoint(interactionPoint, DateTimeUtility.ElapsedMilliseconds.ToString());
		}

		public void LogAnnotation(string annotationKey, string annotationValue)
		{
			this.consoleLoggerImpl.LogAnnotation(annotationKey, annotationValue);
			VoiceSDKLoggerBinding service = this.service;
			if (service == null)
			{
				return;
			}
			service.LogAnnotation(annotationKey, annotationValue);
		}

		public void LogFirstTranscriptionTime()
		{
			if (!this.loggedFirstTranscriptionTime)
			{
				this.loggedFirstTranscriptionTime = true;
				this.LogInteractionPoint("firstPartialTranscriptionTime");
			}
		}

		private VoiceSDKConsoleLoggerImpl consoleLoggerImpl = new VoiceSDKConsoleLoggerImpl();

		private bool loggedFirstTranscriptionTime;
	}
}
