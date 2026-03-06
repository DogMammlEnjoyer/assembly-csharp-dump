using System;
using System.Collections.Generic;
using System.Text;
using Meta.Voice.Audio.Decoding;
using Meta.WitAi.Configuration;
using UnityEngine;
using UnityEngine.Networking;

namespace Meta.WitAi
{
	public static class WitRequestSettings
	{
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			if (WitRequestSettings._operatingSystem == null)
			{
				WitRequestSettings._operatingSystem = SystemInfo.operatingSystem;
			}
			if (WitRequestSettings._deviceModel == null)
			{
				WitRequestSettings._deviceModel = SystemInfo.deviceModel;
			}
			if (WitRequestSettings._appIdentifier == null)
			{
				WitRequestSettings._appIdentifier = Application.identifier;
			}
			if (WitRequestSettings._unityVersion == null)
			{
				WitRequestSettings._unityVersion = Application.unityVersion;
			}
			if (string.IsNullOrEmpty(WitRequestSettings._localClientUserId))
			{
				WitRequestSettings.LocalClientUserId = PlayerPrefs.GetString("client-user-id");
			}
		}

		public static string LocalClientUserId
		{
			get
			{
				return WitRequestSettings._localClientUserId;
			}
			set
			{
				string text = value;
				if (string.IsNullOrEmpty(text))
				{
					text = Guid.NewGuid().ToString();
				}
				else if (string.Equals(text, WitRequestSettings._localClientUserId))
				{
					return;
				}
				WitRequestSettings._localClientUserId = text;
				ThreadUtility.CallOnMainThread(delegate()
				{
					PlayerPrefs.SetString("client-user-id", WitRequestSettings._localClientUserId);
					PlayerPrefs.Save();
				});
			}
		}

		internal static string GetByteString(byte[] bytes)
		{
			return WitRequestSettings.GetByteString(bytes, 0, bytes.Length);
		}

		internal static string GetByteString(byte[] bytes, int start, int length)
		{
			return BitConverter.ToString(bytes, start, length);
		}

		public static Uri GetUri(IWitRequestConfiguration configuration, string path, Dictionary<string, string> queryParams = null)
		{
			UriBuilder uriBuilder = new UriBuilder();
			IWitRequestEndpointInfo endpointInfo = configuration.GetEndpointInfo();
			uriBuilder.Scheme = endpointInfo.UriScheme;
			uriBuilder.Host = endpointInfo.Authority;
			uriBuilder.Port = endpointInfo.Port;
			uriBuilder.Path = path;
			string witApiVersion = endpointInfo.WitApiVersion;
			uriBuilder.Query = "v=" + witApiVersion;
			if (queryParams != null)
			{
				foreach (string text in queryParams.Keys)
				{
					string text2 = queryParams[text];
					if (!string.IsNullOrEmpty(text2))
					{
						text2 = UnityWebRequest.EscapeURL(text2).Replace("+", "%20");
						UriBuilder uriBuilder2 = uriBuilder;
						uriBuilder2.Query = string.Concat(new string[]
						{
							uriBuilder2.Query,
							"&",
							text,
							"=",
							text2
						});
					}
				}
			}
			if (WitRequestSettings.OnProvideCustomUri != null)
			{
				Delegate[] invocationList = WitRequestSettings.OnProvideCustomUri.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					uriBuilder = ((Func<UriBuilder, UriBuilder>)invocationList[i])(uriBuilder);
				}
			}
			return uriBuilder.Uri;
		}

		public static Dictionary<string, string> GetHeaders(IWitRequestConfiguration configuration, string requestId, bool useServerToken, string clientUserId = null)
		{
			return WitRequestSettings.GetHeaders(configuration, new WitRequestOptions(requestId, clientUserId, null, null), useServerToken);
		}

		public static Dictionary<string, string> GetHeaders(IWitRequestConfiguration configuration, WitRequestOptions options, bool useServerToken)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["Authorization"] = WitRequestSettings.GetAuthorizationHeader(configuration, useServerToken);
			dictionary["X-Wit-Client-Request-Id"] = ((!string.IsNullOrEmpty(options.RequestId)) ? options.RequestId : WitConstants.GetUniqueId());
			dictionary["X-Wit-Client-Operation-Id"] = ((!string.IsNullOrEmpty(options.OperationId)) ? options.OperationId : WitConstants.GetUniqueId());
			dictionary["client-user-id"] = ((!string.IsNullOrEmpty(options.ClientUserId)) ? options.ClientUserId : WitRequestSettings.LocalClientUserId);
			dictionary["User-Agent"] = WitRequestSettings.GetUserAgentHeader(configuration);
			if (WitRequestSettings.OnProvideCustomHeaders != null)
			{
				Delegate[] invocationList = WitRequestSettings.OnProvideCustomHeaders.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					((Action<Dictionary<string, string>>)invocationList[i])(dictionary);
				}
			}
			return dictionary;
		}

		private static string GetAuthorizationHeader(IWitRequestConfiguration configuration, bool useServerToken)
		{
			string text = configuration.GetClientAccessToken();
			if (useServerToken)
			{
				text = string.Empty;
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Trim();
			}
			else
			{
				text = "XXX";
			}
			return "Bearer " + text;
		}

		private static string GetUserAgentHeader(IWitRequestConfiguration configuration)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("wit-unity-78.0.0");
			stringBuilder.Append(",\"" + WitRequestSettings._operatingSystem + "\"");
			stringBuilder.Append(",\"" + WitRequestSettings._deviceModel + "\"");
			string text = configuration.GetConfigurationId();
			if (string.IsNullOrEmpty(text))
			{
				text = "not-yet-configured";
			}
			stringBuilder.Append("," + text);
			stringBuilder.Append("," + WitRequestSettings._appIdentifier);
			stringBuilder.Append(",Runtime");
			stringBuilder.Append("," + WitRequestSettings._unityVersion);
			if (WitRequestSettings.OnProvideCustomUserAgent != null)
			{
				Delegate[] invocationList = WitRequestSettings.OnProvideCustomUserAgent.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					((Action<StringBuilder>)invocationList[i])(stringBuilder);
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetTtsErrors(string textToSpeak, IWitRequestConfiguration configuration)
		{
			if (string.IsNullOrEmpty(textToSpeak))
			{
				return "No text provided";
			}
			if (configuration == null)
			{
				return "No WitConfiguration Set";
			}
			if (string.IsNullOrEmpty(configuration.GetClientAccessToken()))
			{
				return "No WitConfiguration Client Token";
			}
			return string.Empty;
		}

		public static bool CanStreamAudio(TTSWitAudioType witAudioType)
		{
			return witAudioType != TTSWitAudioType.WAV;
		}

		public static string GetAudioMimeType(TTSWitAudioType witAudioType)
		{
			switch (witAudioType)
			{
			case TTSWitAudioType.PCM:
				return "audio/raw";
			case TTSWitAudioType.OPUS:
				return "audio/opus-demo";
			}
			return "audio/" + witAudioType.ToString().ToLower();
		}

		public static string GetAudioExtension(TTSWitAudioType witAudioType, bool includeEvents)
		{
			string text;
			switch (witAudioType)
			{
			case TTSWitAudioType.PCM:
				text = ".raw";
				goto IL_4D;
			case TTSWitAudioType.MPEG:
				text = ".mp3";
				goto IL_4D;
			case TTSWitAudioType.OPUS:
				text = ".opusd";
				goto IL_4D;
			}
			text = "." + witAudioType.ToString().ToLower();
			IL_4D:
			if (includeEvents)
			{
				text += "v";
			}
			return text;
		}

		public static IAudioDecoder GetTtsAudioDecoder(TTSWitAudioType witAudioType)
		{
			switch (witAudioType)
			{
			case TTSWitAudioType.PCM:
				return new AudioDecoderPcm(AudioDecoderPcmType.Int16, 720);
			case TTSWitAudioType.MPEG:
				return new AudioDecoderMp3();
			case TTSWitAudioType.WAV:
				return new AudioDecoderWav(720);
			case TTSWitAudioType.OPUS:
				return new AudioDecoderOpus(1, 24000);
			default:
				throw new ArgumentException(string.Format("{0} audio decoder not supported", witAudioType));
			}
		}

		public static IAudioDecoder GetTtsAudioDecoder(TTSWitAudioType witAudioType, AudioJsonDecodeDelegate onEventsDecoded)
		{
			IAudioDecoder ttsAudioDecoder = WitRequestSettings.GetTtsAudioDecoder(witAudioType);
			if (ttsAudioDecoder != null && onEventsDecoded != null)
			{
				return new AudioDecoderJson(ttsAudioDecoder, onEventsDecoded);
			}
			return ttsAudioDecoder;
		}

		private static string _operatingSystem;

		private static string _deviceModel;

		private static string _appIdentifier;

		private static string _unityVersion;

		public static Func<UriBuilder, UriBuilder> OnProvideCustomUri;

		public static Action<Dictionary<string, string>> OnProvideCustomHeaders;

		public static Action<StringBuilder> OnProvideCustomUserAgent;

		private static string _localClientUserId;

		private const string PREF_CLIENT_USER_ID = "client-user-id";
	}
}
