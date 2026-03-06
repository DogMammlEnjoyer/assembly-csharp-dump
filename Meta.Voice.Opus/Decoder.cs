using System;
using UnityEngine;

namespace Meta.Voice.UnityOpus
{
	public class Decoder : IDisposable
	{
		public Decoder(SamplingFrequency samplingFrequency, NumChannels channels)
		{
			this.channels = channels;
			ErrorCode errorCode;
			this.decoder = Library.OpusDecoderCreate(samplingFrequency, channels, out errorCode);
			if (errorCode != ErrorCode.OK)
			{
				Debug.LogError("[UnityOpus] Failed to create Decoder. Error code is " + errorCode.ToString());
				this.decoder = IntPtr.Zero;
			}
			this.softclipMem = new float[(int)channels];
		}

		public int Decode(byte[] data, int dataLength, float[] pcm, int decodeFec = 0)
		{
			if (this.decoder == IntPtr.Zero)
			{
				return 0;
			}
			int num = Library.OpusDecodeFloat(this.decoder, data, dataLength, pcm, (int)((NumChannels)pcm.Length / this.channels), decodeFec);
			Library.OpusPcmSoftClip(pcm, num / (int)this.channels, this.channels, this.softclipMem);
			return num;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (this.decoder == IntPtr.Zero)
				{
					return;
				}
				Library.OpusDecoderDestroy(this.decoder);
				this.decoder = IntPtr.Zero;
				this.disposedValue = true;
			}
		}

		~Decoder()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public const int maximumPacketDuration = 5760;

		private IntPtr decoder;

		private readonly NumChannels channels;

		private readonly float[] softclipMem;

		private bool disposedValue;
	}
}
