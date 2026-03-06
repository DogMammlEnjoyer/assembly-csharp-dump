using System;
using System.Collections.Generic;
using System.Globalization;
using Meta.WitAi.Data.Configuration;
using Meta.WitAi.Interfaces;
using Meta.WitAi.TTS;
using Meta.WitAi.TTS.Data;
using Oculus.Voice.Core.Bindings.Android.PlatformLogger;
using Oculus.Voice.Core.Bindings.Interfaces;
using Oculus.Voice.Core.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Voice.Logging
{
	internal class TTSServiceLogging : MonoBehaviour
	{
		public TTSService Service { get; private set; }

		private void Awake()
		{
			this.Service = base.gameObject.GetComponent<TTSService>();
			this.InitLogger();
		}

		private void InitLogger()
		{
			this._voiceSDKLoggerImpl = new VoiceSDKConsoleLoggerImpl();
			IWitConfigurationProvider component = this.Service.GetComponent<IWitConfigurationProvider>();
			WitConfiguration witConfiguration = (component != null) ? component.Configuration : null;
			if (witConfiguration != null)
			{
				this._voiceSDKLoggerImpl.WitApplication = witConfiguration.GetLoggerAppId();
			}
		}

		private void OnEnable()
		{
			if (this._voiceSDKLoggerImpl != null)
			{
				this._voiceSDKLoggerImpl.ShouldLogToConsole = this.EnableConsoleLogging;
			}
			if (this.Service)
			{
				this.Service.Events.WebRequest.OnRequestBegin.AddListener(new UnityAction<TTSClipData>(this.OnRequestBegin));
				this.Service.Events.WebRequest.OnRequestCancel.AddListener(new UnityAction<TTSClipData>(this.OnRequestCancel));
				this.Service.Events.WebRequest.OnRequestError.AddListener(new UnityAction<TTSClipData, string>(this.OnRequestError));
				this.Service.Events.WebRequest.OnRequestFirstResponse.AddListener(new UnityAction<TTSClipData>(this.OnRequestFirstResponse));
				this.Service.Events.WebRequest.OnRequestReady.AddListener(new UnityAction<TTSClipData>(this.OnRequestReady));
				this.Service.Events.WebRequest.OnRequestComplete.AddListener(new UnityAction<TTSClipData>(this.OnRequestComplete));
			}
		}

		private void OnDisable()
		{
			if (this.Service)
			{
				this.Service.Events.WebRequest.OnRequestBegin.RemoveListener(new UnityAction<TTSClipData>(this.OnRequestBegin));
				this.Service.Events.WebRequest.OnRequestCancel.RemoveListener(new UnityAction<TTSClipData>(this.OnRequestCancel));
				this.Service.Events.WebRequest.OnRequestError.RemoveListener(new UnityAction<TTSClipData, string>(this.OnRequestError));
				this.Service.Events.WebRequest.OnRequestFirstResponse.RemoveListener(new UnityAction<TTSClipData>(this.OnRequestFirstResponse));
				this.Service.Events.WebRequest.OnRequestReady.RemoveListener(new UnityAction<TTSClipData>(this.OnRequestReady));
				this.Service.Events.WebRequest.OnRequestComplete.RemoveListener(new UnityAction<TTSClipData>(this.OnRequestComplete));
			}
		}

		private void OnRequestBegin(TTSClipData clipData)
		{
			this.LogStart(clipData);
		}

		private void OnRequestCancel(TTSClipData clipData)
		{
			this.LogComplete(clipData, "aborted");
		}

		private void OnRequestError(TTSClipData clipData, string error)
		{
			this.LogComplete(clipData, error);
		}

		private void OnRequestFirstResponse(TTSClipData clipData)
		{
			this.LogTimestamp(clipData, "ttsFirstResponseTime");
		}

		private void OnRequestReady(TTSClipData clipData)
		{
			this.LogTimestamp(clipData, "ttsReadyTime");
		}

		private void OnRequestComplete(TTSClipData clipData)
		{
			this.LogComplete(clipData, null);
		}

		private void LogStart(TTSClipData clipData)
		{
			TTSServiceLogging.TTSServiceRequestLog requestData = this.GetRequestData(clipData);
			requestData.startTime = DateTime.UtcNow;
			requestData.annotations = new Dictionary<string, string>();
			this.LogTimestamp(requestData, "ttsStartTime");
			this.LogAnnotate(requestData, "ttsFileType", clipData.extension);
			this.LogAnnotate(requestData, "ttsFileStream", clipData.queryStream.ToString(CultureInfo.InvariantCulture));
			this._requests[clipData.queryRequestId] = requestData;
		}

		private TTSServiceLogging.TTSServiceRequestLog GetRequestData(TTSClipData clipData)
		{
			if (this._requests.ContainsKey(clipData.queryRequestId))
			{
				return this._requests[clipData.queryRequestId];
			}
			return default(TTSServiceLogging.TTSServiceRequestLog);
		}

		private void LogTimestamp(TTSClipData clipData, string key)
		{
			this.LogTimestamp(this.GetRequestData(clipData), key);
		}

		private void LogTimestamp(TTSServiceLogging.TTSServiceRequestLog requestData, string key)
		{
			this.LogAnnotate(requestData, key, DateTimeUtility.ElapsedMilliseconds.ToString());
		}

		private void LogAnnotate(TTSServiceLogging.TTSServiceRequestLog requestData, string key, string value)
		{
			if (requestData.annotations != null)
			{
				requestData.annotations[key] = value;
			}
		}

		private void LogComplete(TTSClipData clipData, string error = null)
		{
			TTSServiceLogging.TTSServiceRequestLog requestData = this.GetRequestData(clipData);
			if (requestData.annotations == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(error))
			{
				this.LogAnnotate(requestData, "ttsError", error);
			}
			this.LogTimestamp(requestData, "ttsFinishedTime");
			if (this._voiceSDKLoggerImpl != null)
			{
				this._voiceSDKLoggerImpl.LogInteractionStart(clipData.queryRequestId, "synthesize");
				foreach (string text in requestData.annotations.Keys)
				{
					this._voiceSDKLoggerImpl.LogAnnotation(text, requestData.annotations[text]);
				}
				if (string.IsNullOrEmpty(error))
				{
					this._voiceSDKLoggerImpl.LogInteractionEndSuccess();
				}
				else
				{
					this._voiceSDKLoggerImpl.LogInteractionEndFailure(error);
				}
			}
			this._requests.Remove(clipData.queryRequestId);
		}

		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			if (TTSServiceLogging._initialized)
			{
				return;
			}
			TTSServiceLogging._initialized = true;
			TTSService.OnServiceStart += TTSServiceLogging.OnServiceStart;
		}

		private static void OnServiceStart(TTSService service)
		{
			if (service != null && service.GetComponent<TTSServiceLogging>() == null)
			{
				service.gameObject.AddComponent<TTSServiceLogging>();
			}
		}

		public bool EnableConsoleLogging;

		private IVoiceSDKLogger _voiceSDKLoggerImpl;

		private Dictionary<string, TTSServiceLogging.TTSServiceRequestLog> _requests = new Dictionary<string, TTSServiceLogging.TTSServiceRequestLog>();

		private const string TTS_FILETYPE_ANNOTATION = "ttsFileType";

		private const string TTS_FILESTREAM_ANNOTATION = "ttsFileStream";

		private const string TTS_START_TIME_ANNOTATION = "ttsStartTime";

		private const string TTS_FIRST_TIME_ANNOTATION = "ttsFirstResponseTime";

		private const string TTS_READY_TIME_ANNOTATION = "ttsReadyTime";

		private const string TTS_FINISH_TIME_ANNOTATION = "ttsFinishedTime";

		private const string TTS_ERROR_ANNOTATION = "ttsError";

		private static bool _initialized;

		private struct TTSServiceRequestLog
		{
			public DateTime startTime;

			public Dictionary<string, string> annotations;
		}
	}
}
