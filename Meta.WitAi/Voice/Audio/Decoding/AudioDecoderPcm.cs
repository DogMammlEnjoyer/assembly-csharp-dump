using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Meta.Voice.Audio.Decoding
{
	[Preserve]
	public class AudioDecoderPcm : IAudioDecoder
	{
		[Preserve]
		public AudioDecoderPcm(AudioDecoderPcmType pcmType, int sampleBufferLength = 720)
		{
			this.PcmType = pcmType;
			this._byteCount = AudioDecoderPcm.GetByteCount(this.PcmType);
			this._overflow = new byte[this._byteCount];
			this._samples = new float[sampleBufferLength];
			this._decoder = AudioDecoderPcm.GetPcmDecoder(this.PcmType);
		}

		public bool WillDecodeInBackground
		{
			get
			{
				return true;
			}
		}

		public virtual void Decode(byte[] buffer, int bufferOffset, int bufferLength, AudioSampleDecodeDelegate onSamplesDecoded)
		{
			if (this._overflowOffset > 0)
			{
				int num = Mathf.Min(this._byteCount - this._overflowOffset, bufferLength);
				Array.Copy(buffer, bufferOffset, this._overflow, this._overflowOffset, num);
				this._samples[0] = this._decoder(this._overflow, 0);
				if (onSamplesDecoded != null)
				{
					onSamplesDecoded(this._samples, 0, 1);
				}
				bufferOffset += num;
				bufferLength -= num;
				this._overflowOffset = 0;
			}
			while (bufferLength >= this._byteCount)
			{
				int num2 = Mathf.Min(Mathf.FloorToInt((float)bufferLength / (float)this._byteCount), this._samples.Length);
				for (int i = 0; i < num2; i++)
				{
					this._samples[i] = this._decoder(buffer, bufferOffset + i * this._byteCount);
				}
				if (onSamplesDecoded != null)
				{
					onSamplesDecoded(this._samples, 0, num2);
				}
				num2 *= this._byteCount;
				bufferOffset += num2;
				bufferLength -= num2;
			}
			if (bufferLength > 0)
			{
				Array.Copy(buffer, bufferOffset, this._overflow, this._overflowOffset, bufferLength);
				this._overflowOffset += bufferLength;
			}
		}

		public static int GetByteCount(AudioDecoderPcmType pcmType)
		{
			switch (pcmType)
			{
			case AudioDecoderPcmType.Int16:
			case AudioDecoderPcmType.UInt16:
				return 2;
			case AudioDecoderPcmType.Int32:
			case AudioDecoderPcmType.UInt32:
				return 4;
			case AudioDecoderPcmType.Int64:
			case AudioDecoderPcmType.UInt64:
				return 8;
			default:
				return 0;
			}
		}

		public static long GetTotalSamplesPcm(long contentLength, AudioDecoderPcmType pcmType = AudioDecoderPcmType.Int16)
		{
			return contentLength / (long)AudioDecoderPcm.GetByteCount(pcmType);
		}

		public static float[] DecodePcm(byte[] rawData, AudioDecoderPcmType pcmType = AudioDecoderPcmType.Int16)
		{
			AudioDecoderPcm.PcmDecodeDelegate pcmDecoder = AudioDecoderPcm.GetPcmDecoder(pcmType);
			float[] array = new float[(int)AudioDecoderPcm.GetTotalSamplesPcm((long)rawData.Length, pcmType)];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = pcmDecoder(rawData, i * 2);
			}
			return array;
		}

		internal static AudioDecoderPcm.PcmDecodeDelegate GetPcmDecoder(AudioDecoderPcmType pcmType)
		{
			switch (pcmType)
			{
			case AudioDecoderPcmType.Int16:
				return new AudioDecoderPcm.PcmDecodeDelegate(AudioDecoderPcm.DecodeSample_Pcm16);
			case AudioDecoderPcmType.Int32:
				return new AudioDecoderPcm.PcmDecodeDelegate(AudioDecoderPcm.DecodeSample_Pcm32);
			case AudioDecoderPcmType.Int64:
				return new AudioDecoderPcm.PcmDecodeDelegate(AudioDecoderPcm.DecodeSample_Pcm64);
			case AudioDecoderPcmType.UInt16:
				return new AudioDecoderPcm.PcmDecodeDelegate(AudioDecoderPcm.DecodeSample_PcmU16);
			case AudioDecoderPcmType.UInt32:
				return new AudioDecoderPcm.PcmDecodeDelegate(AudioDecoderPcm.DecodeSample_PcmU32);
			case AudioDecoderPcmType.UInt64:
				return new AudioDecoderPcm.PcmDecodeDelegate(AudioDecoderPcm.DecodeSample_PcmU64);
			default:
				return new AudioDecoderPcm.PcmDecodeDelegate(AudioDecoderPcm.DecodeSample_Pcm16);
			}
		}

		public static float DecodeSample_Pcm16(byte[] rawData, int index)
		{
			return (float)BitConverter.ToInt16(rawData, index) / 32767f;
		}

		public static float DecodeSample_Pcm32(byte[] rawData, int index)
		{
			return (float)BitConverter.ToInt32(rawData, index) / 2.1474836E+09f;
		}

		public static float DecodeSample_Pcm64(byte[] rawData, int index)
		{
			return (float)((double)BitConverter.ToInt64(rawData, index) / 9.223372036854776E+18);
		}

		public static float DecodeSample_PcmU16(byte[] rawData, int index)
		{
			return (float)BitConverter.ToUInt16(rawData, index) / 65535f;
		}

		public static float DecodeSample_PcmU32(byte[] rawData, int index)
		{
			return BitConverter.ToUInt32(rawData, index) / 4.2949673E+09f;
		}

		public static float DecodeSample_PcmU64(byte[] rawData, int index)
		{
			return (float)(BitConverter.ToUInt64(rawData, index) / 1.8446744073709552E+19);
		}

		public readonly AudioDecoderPcmType PcmType;

		private readonly int _byteCount;

		private readonly AudioDecoderPcm.PcmDecodeDelegate _decoder;

		private int _overflowOffset;

		private readonly byte[] _overflow;

		private readonly float[] _samples;

		internal delegate float PcmDecodeDelegate(byte[] buffer, int bufferOffset);
	}
}
