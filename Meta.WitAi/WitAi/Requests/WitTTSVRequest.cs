using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Meta.Voice.Audio.Decoding;
using Meta.WitAi.Json;
using UnityEngine;

namespace Meta.WitAi.Requests
{
	internal class WitTTSVRequest : WitVRequest
	{
		public string TextToSpeak { get; set; }

		public Dictionary<string, string> TtsParameters { get; set; }

		public TTSWitAudioType FileType { get; set; }

		public bool Stream { get; set; }

		public bool UseEvents { get; set; }

		public WitTTSVRequest(IWitRequestConfiguration configuration, string requestId, string operationId) : base(configuration, requestId, operationId, false)
		{
		}

		protected override Dictionary<string, string> GetHeaders()
		{
			Dictionary<string, string> headers = base.GetHeaders();
			headers["Accept"] = WitRequestSettings.GetAudioMimeType(this.FileType);
			return headers;
		}

		public Task<VRequestResponse<bool>> RequestStreamFromDisk(string diskPath, AudioSampleDecodeDelegate onSamplesDecoded, AudioJsonDecodeDelegate onJsonDecoded)
		{
			WitTTSVRequest.<RequestStreamFromDisk>d__23 <RequestStreamFromDisk>d__;
			<RequestStreamFromDisk>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<bool>>.Create();
			<RequestStreamFromDisk>d__.<>4__this = this;
			<RequestStreamFromDisk>d__.diskPath = diskPath;
			<RequestStreamFromDisk>d__.onSamplesDecoded = onSamplesDecoded;
			<RequestStreamFromDisk>d__.onJsonDecoded = onJsonDecoded;
			<RequestStreamFromDisk>d__.<>1__state = -1;
			<RequestStreamFromDisk>d__.<>t__builder.Start<WitTTSVRequest.<RequestStreamFromDisk>d__23>(ref <RequestStreamFromDisk>d__);
			return <RequestStreamFromDisk>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<bool>> RequestStream(AudioSampleDecodeDelegate onSamplesDecoded, AudioJsonDecodeDelegate onJsonDecoded)
		{
			WitTTSVRequest.<RequestStream>d__24 <RequestStream>d__;
			<RequestStream>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<bool>>.Create();
			<RequestStream>d__.<>4__this = this;
			<RequestStream>d__.onSamplesDecoded = onSamplesDecoded;
			<RequestStream>d__.onJsonDecoded = onJsonDecoded;
			<RequestStream>d__.<>1__state = -1;
			<RequestStream>d__.<>t__builder.Start<WitTTSVRequest.<RequestStream>d__24>(ref <RequestStream>d__);
			return <RequestStream>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<bool>> RequestDownload(string downloadPath)
		{
			WitTTSVRequest.<RequestDownload>d__25 <RequestDownload>d__;
			<RequestDownload>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<bool>>.Create();
			<RequestDownload>d__.<>4__this = this;
			<RequestDownload>d__.downloadPath = downloadPath;
			<RequestDownload>d__.<>1__state = -1;
			<RequestDownload>d__.<>t__builder.Start<WitTTSVRequest.<RequestDownload>d__25>(ref <RequestDownload>d__);
			return <RequestDownload>d__.<>t__builder.Task;
		}

		private Task<string> SetupTts(bool download, AudioSampleDecodeDelegate onSamplesDecoded, AudioJsonDecodeDelegate onJsonDecoded)
		{
			WitTTSVRequest.<SetupTts>d__26 <SetupTts>d__;
			<SetupTts>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<SetupTts>d__.<>4__this = this;
			<SetupTts>d__.download = download;
			<SetupTts>d__.onSamplesDecoded = onSamplesDecoded;
			<SetupTts>d__.onJsonDecoded = onJsonDecoded;
			<SetupTts>d__.<>1__state = -1;
			<SetupTts>d__.<>t__builder.Start<WitTTSVRequest.<SetupTts>d__26>(ref <SetupTts>d__);
			return <SetupTts>d__.<>t__builder.Task;
		}

		private string GetWebErrors(bool downloadOnly = false)
		{
			string ttsErrors = WitRequestSettings.GetTtsErrors(this.TextToSpeak, base.Configuration);
			if (!downloadOnly && this.Stream)
			{
				if (Application.platform == RuntimePlatform.WebGLPlayer)
				{
					base.Logger.Warning("Wit cannot currently stream TTS in WebGL", Array.Empty<object>());
					this.Stream = false;
				}
				else if (!WitRequestSettings.CanStreamAudio(this.FileType))
				{
					base.Logger.Warning("Wit cannot stream {0} files please use {1} instead.", new object[]
					{
						this.FileType,
						TTSWitAudioType.MPEG
					});
					this.Stream = false;
				}
			}
			return ttsErrors;
		}

		private byte[] EncodePostData()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["q"] = this.TextToSpeak;
			dictionary["viseme"] = this.UseEvents.ToString().ToLower();
			if (this.TtsParameters != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in this.TtsParameters)
				{
					dictionary[keyValuePair.Key] = keyValuePair.Value;
				}
			}
			string s = JsonConvert.SerializeObject<Dictionary<string, string>>(dictionary, null, false);
			return Encoding.UTF8.GetBytes(s);
		}

		private IAudioDecoder _decoder;
	}
}
