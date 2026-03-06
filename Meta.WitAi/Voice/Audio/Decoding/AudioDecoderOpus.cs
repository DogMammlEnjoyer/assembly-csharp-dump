using System;
using Meta.Voice.UnityOpus;
using UnityEngine;
using UnityEngine.Scripting;

namespace Meta.Voice.Audio.Decoding
{
	[Preserve]
	public class AudioDecoderOpus : IAudioDecoder
	{
		public AudioDecoderOpus(int channels, int samplerate)
		{
			this._decoder = new Decoder((SamplingFrequency)samplerate, (NumChannels)channels);
			this._frameBuffer = new byte[240];
			this._opusBuffer = new float[5760 * channels];
		}

		public bool WillDecodeInBackground { get; set; } = true;

		public void Decode(byte[] buffer, int bufferOffset, int bufferLength, AudioSampleDecodeDelegate onSamplesDecoded)
		{
			while (bufferLength > 0)
			{
				if (!this._validHeader)
				{
					int num = this.DecodeFrameHeader(buffer, bufferOffset, bufferLength);
					bufferOffset += num;
					bufferLength -= num;
				}
				else
				{
					int num2 = Mathf.Min(this._frameLength - this._frameOffset, bufferLength);
					if (num2 == 0)
					{
						this._validHeader = false;
						this._frameOffset = 0;
					}
					else
					{
						Array.Copy(buffer, bufferOffset, this._frameBuffer, this._frameOffset, num2);
						this._frameOffset += num2;
						bufferOffset += num2;
						bufferLength -= num2;
						if (this._frameOffset == this._frameLength)
						{
							int length = this._decoder.Decode(this._frameBuffer, this._frameLength, this._opusBuffer, 0);
							if (onSamplesDecoded != null)
							{
								onSamplesDecoded(this._opusBuffer, 0, length);
							}
							this._validHeader = false;
							this._frameOffset = 0;
						}
					}
				}
			}
		}

		private int DecodeFrameHeader(byte[] buffer, int bufferOffset, int bufferLength)
		{
			int num = Mathf.Min(8 - this._frameOffset, bufferLength);
			Array.Copy(buffer, bufferOffset, this._frameBuffer, this._frameOffset, num);
			this._frameOffset += num;
			if (this._frameOffset < 8)
			{
				return num;
			}
			this._frameOffset = 0;
			Array.Reverse<byte>(this._frameBuffer, 0, 4);
			this._frameLength = BitConverter.ToInt32(this._frameBuffer, 0);
			if (this._frameLength == 0)
			{
				throw new Exception("Invalid zero-length opus frame");
			}
			if (this._frameLength > 240)
			{
				throw new Exception(string.Format("Frame size ({0}) exceeded max frame size ({1})", this._frameLength, 240));
			}
			this._validHeader = true;
			return num;
		}

		private readonly Decoder _decoder;

		private readonly byte[] _frameBuffer;

		private readonly float[] _opusBuffer;

		private const int _headerLength = 8;

		private const int _frameMax = 240;

		private int _frameLength;

		private bool _validHeader;

		private int _frameOffset;
	}
}
