using System;
using UnityEngine;

namespace Meta.Voice.UnityOpus
{
	public class Encoder : IDisposable
	{
		public int Bitrate
		{
			get
			{
				return this.bitrate;
			}
			set
			{
				Library.OpusEncoderSetBitrate(this.encoder, value);
				this.bitrate = value;
			}
		}

		public int Complexity
		{
			get
			{
				return this.complexity;
			}
			set
			{
				Library.OpusEncoderSetComplexity(this.encoder, value);
				this.complexity = value;
			}
		}

		public OpusSignal Signal
		{
			get
			{
				return this.signal;
			}
			set
			{
				Library.OpusEncoderSetSignal(this.encoder, value);
				this.signal = value;
			}
		}

		public Encoder(SamplingFrequency samplingFrequency, NumChannels channels, OpusApplication application)
		{
			this.channels = channels;
			ErrorCode errorCode;
			this.encoder = Library.OpusEncoderCreate(samplingFrequency, channels, application, out errorCode);
			if (errorCode != ErrorCode.OK)
			{
				Debug.LogError("[UnityOpus] Failed to init encoder. Error code: " + errorCode.ToString());
				this.encoder = IntPtr.Zero;
			}
		}

		public int Encode(float[] pcm, byte[] output)
		{
			if (this.encoder == IntPtr.Zero)
			{
				return 0;
			}
			return Library.OpusEncodeFloat(this.encoder, pcm, (int)((NumChannels)pcm.Length / this.channels), output, output.Length);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposedValue)
			{
				if (this.encoder == IntPtr.Zero)
				{
					return;
				}
				Library.OpusEncoderDestroy(this.encoder);
				this.encoder = IntPtr.Zero;
				this.disposedValue = true;
			}
		}

		~Encoder()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private int bitrate;

		private int complexity;

		private OpusSignal signal;

		private IntPtr encoder;

		private NumChannels channels;

		private bool disposedValue;
	}
}
