using System;
using System.Collections.Generic;
using System.IO;
using Meta.Voice.Audio.Decoding;
using Meta.Voice.Net.Encoding.Wit;
using Meta.Voice.TelemetryUtilities;
using Meta.WitAi;
using Meta.WitAi.Json;

namespace Meta.Voice.Net.WebSockets.Requests
{
	public class WitWebSocketTtsRequest : WitWebSocketJsonRequest
	{
		public string TextToSpeak { get; }

		public Dictionary<string, string> VoiceSettings { get; }

		public TTSWitAudioType AudioType { get; }

		public bool UseEvents { get; }

		public string DownloadPath { get; }

		public WitWebSocketTtsRequest(string requestId, string textToSpeak, Dictionary<string, string> voiceSettings, TTSWitAudioType audioType, bool useEvents, string downloadPath = null, string opId = null) : base(WitWebSocketTtsRequest.GetTtsNode(textToSpeak, voiceSettings, audioType, useEvents), requestId, null, opId)
		{
			this.TextToSpeak = textToSpeak;
			this.VoiceSettings = voiceSettings;
			this.AudioType = audioType;
			this.UseEvents = useEvents;
			this.DownloadPath = downloadPath;
			this._audioDecoder = WitRequestSettings.GetTtsAudioDecoder(audioType);
		}

		private static WitResponseNode GetTtsNode(string textToSpeak, Dictionary<string, string> voiceSettings, TTSWitAudioType audioType, bool useEvents)
		{
			WitResponseClass witResponseClass = new WitResponseClass();
			WitResponseClass witResponseClass2 = new WitResponseClass();
			WitResponseClass witResponseClass3 = new WitResponseClass();
			witResponseClass3["q"] = textToSpeak;
			foreach (string text in voiceSettings.Keys)
			{
				witResponseClass3[text] = voiceSettings[text];
			}
			witResponseClass3["accept_header"] = WitRequestSettings.GetAudioMimeType(audioType);
			if (useEvents)
			{
				witResponseClass3["viseme"] = new WitResponseData(true);
			}
			witResponseClass2["synthesize"] = witResponseClass3;
			witResponseClass["data"] = witResponseClass2;
			return witResponseClass;
		}

		public override void HandleDownload(string jsonString, WitResponseNode jsonData, byte[] binaryData)
		{
			if (base.IsComplete || jsonData == null)
			{
				return;
			}
			if (!base.IsDownloading)
			{
				this.HandleDownloadBegin();
			}
			this.ReturnRawResponse(jsonString);
			this.SetResponseData(jsonData);
			if (!string.IsNullOrEmpty(base.Error))
			{
				this.HandleComplete();
				return;
			}
			try
			{
				WitResponseNode witResponseNode = jsonData["viseme"];
				WitResponseArray witResponseArray = (witResponseNode != null) ? witResponseNode.AsArray : null;
				if (witResponseArray != null && witResponseArray.Count > 0 && this.OnEventsReceived != null)
				{
					this._jsonDecoded.AddRange(witResponseArray.Childs);
					if (this._jsonDecoded.Count > 0)
					{
						this._eventCount += this._jsonDecoded.Count;
						this.OnEventsReceived(this._jsonDecoded);
						this._jsonDecoded.Clear();
					}
				}
				if (binaryData != null && binaryData.Length != 0 && this.OnSamplesReceived != null && this._audioDecoder != null)
				{
					this._sampleCount += binaryData.Length;
					this._audioDecoder.Decode(binaryData, 0, binaryData.Length, this.OnSamplesReceived);
				}
				if (this._fileStream != null)
				{
					if (witResponseArray != null)
					{
						byte[] array = WitChunkConverter.Encode(new WitChunk
						{
							jsonData = witResponseArray,
							binaryData = binaryData
						});
						this._fileStream.Write(array, 0, array.Length);
					}
					else if (binaryData != null)
					{
						this._fileStream.Write(binaryData, 0, binaryData.Length);
					}
				}
			}
			catch (Exception ex)
			{
				base.Logger.Error("Decode Response Failed\n{0}\n\n{1}", new object[]
				{
					this,
					ex
				});
			}
			WitResponseNode witResponseNode2 = jsonData["end_stream"];
			if (witResponseNode2 != null && witResponseNode2.AsBool)
			{
				this.HandleComplete();
			}
		}

		protected override void HandleDownloadBegin()
		{
			base.HandleDownloadBegin();
			if (string.IsNullOrEmpty(this.DownloadPath))
			{
				return;
			}
			string directoryName = Path.GetDirectoryName(this.DownloadPath);
			if (!Directory.Exists(directoryName))
			{
				base.Logger.Error("Tts download file directory does not exist\nPath: {0}\n{1}", new object[]
				{
					directoryName,
					this
				});
				return;
			}
			try
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this.DownloadPath);
				string audioExtension = WitRequestSettings.GetAudioExtension(this.AudioType, this.UseEvents);
				string path = Path.Join(directoryName, fileNameWithoutExtension + audioExtension);
				this._fileStream = new FileStream(path, FileMode.Create);
			}
			catch (Exception ex)
			{
				base.Logger.Error("Tts download file stream generation failed\n{0}\n{1}", new object[]
				{
					this,
					ex
				});
			}
		}

		protected override void HandleComplete()
		{
			if (string.IsNullOrEmpty(base.Error))
			{
				if (this._sampleCount == 0)
				{
					base.Error = "No audio samples returned";
					RuntimeTelemetry.Instance.LogPoint(base.OperationId, RuntimeTelemetryPoint.FinalAudioSamplesEmpty);
				}
				else if (this._eventCount == 0 && this.UseEvents)
				{
					base.Error = "No audio events returned";
					RuntimeTelemetry.Instance.LogPoint(base.OperationId, RuntimeTelemetryPoint.FinalAudioEventsEmpty);
				}
			}
			if (this._fileStream != null)
			{
				this._fileStream.Close();
				this._fileStream = null;
			}
			base.HandleComplete();
		}

		public override string ToString()
		{
			return string.Format("Type: {0}\nRequest Id: {1}\nTopic Id: {2}\nText: {3}\nAudio Type: {4}\nUse Events: {5}\nDownload Path: {6}\nError: {7}", new object[]
			{
				base.GetType().Name,
				base.RequestId,
				base.TopicId ?? "Null",
				this.TextToSpeak ?? "Null",
				this.AudioType,
				this.UseEvents,
				this.DownloadPath ?? "Null",
				base.Error ?? "Null"
			});
		}

		public AudioSampleDecodeDelegate OnSamplesReceived;

		public AudioJsonDecodeDelegate OnEventsReceived;

		private IAudioDecoder _audioDecoder;

		private FileStream _fileStream;

		private readonly List<WitResponseNode> _jsonDecoded = new List<WitResponseNode>();

		private int _sampleCount;

		private int _eventCount;
	}
}
