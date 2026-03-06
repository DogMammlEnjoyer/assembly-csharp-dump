using System;
using System.Collections.Generic;
using Meta.Voice.Net.Encoding.Wit;
using Meta.WitAi.Json;
using UnityEngine.Scripting;

namespace Meta.Voice.Audio.Decoding
{
	[Preserve]
	public class AudioDecoderJson : IAudioDecoder
	{
		public AudioDecoderJson(IAudioDecoder audioDecoder, AudioJsonDecodeDelegate onJsonDecoded)
		{
			this._audioDecoder = audioDecoder;
			this._onJsonDecoded = onJsonDecoded;
		}

		public bool WillDecodeInBackground
		{
			get
			{
				return true;
			}
		}

		public void Decode(byte[] buffer, int bufferOffset, int bufferLength, AudioSampleDecodeDelegate onSamplesDecoded)
		{
			this._onSamplesDecoded = onSamplesDecoded;
			this._chunkDecoder.Decode(buffer, bufferOffset, bufferLength, new Action<WitChunk>(this.DecodeJson), new Action<byte[], int, int>(this.DecodeAudio));
			this._onSamplesDecoded = null;
			if (this._decodedJson.Count == 0)
			{
				return;
			}
			AudioJsonDecodeDelegate onJsonDecoded = this._onJsonDecoded;
			if (onJsonDecoded != null)
			{
				onJsonDecoded(this._decodedJson);
			}
			this._decodedJson.Clear();
		}

		private void DecodeJson(WitChunk chunk)
		{
			WitResponseArray witResponseArray = chunk.jsonData as WitResponseArray;
			if (witResponseArray != null)
			{
				this._decodedJson.AddRange(witResponseArray.Childs);
				return;
			}
			if (chunk.jsonData != null)
			{
				this._decodedJson.Add(chunk.jsonData);
			}
		}

		private void DecodeAudio(byte[] buffer, int bufferOffset, int bufferLength)
		{
			this._audioDecoder.Decode(buffer, bufferOffset, bufferLength, this._onSamplesDecoded);
		}

		private readonly WitChunkConverter _chunkDecoder = new WitChunkConverter();

		private readonly List<WitResponseNode> _decodedJson = new List<WitResponseNode>();

		private readonly AudioJsonDecodeDelegate _onJsonDecoded;

		private readonly IAudioDecoder _audioDecoder;

		private AudioSampleDecodeDelegate _onSamplesDecoded;
	}
}
