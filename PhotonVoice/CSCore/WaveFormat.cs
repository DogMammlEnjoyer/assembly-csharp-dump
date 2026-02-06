using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CSCore
{
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	public class WaveFormat : ICloneable, IEquatable<WaveFormat>
	{
		public virtual int Channels
		{
			get
			{
				return (int)this._channels;
			}
			protected internal set
			{
				this._channels = (short)value;
				this.UpdateProperties();
			}
		}

		public virtual int SampleRate
		{
			get
			{
				return this._sampleRate;
			}
			protected internal set
			{
				this._sampleRate = value;
				this.UpdateProperties();
			}
		}

		public virtual int BytesPerSecond
		{
			get
			{
				return this._bytesPerSecond;
			}
			protected internal set
			{
				this._bytesPerSecond = value;
			}
		}

		public virtual int BlockAlign
		{
			get
			{
				return (int)this._blockAlign;
			}
			protected internal set
			{
				this._blockAlign = (short)value;
			}
		}

		public virtual int BitsPerSample
		{
			get
			{
				return (int)this._bitsPerSample;
			}
			protected internal set
			{
				this._bitsPerSample = (short)value;
				this.UpdateProperties();
			}
		}

		public virtual int ExtraSize
		{
			get
			{
				return (int)this._extraSize;
			}
			protected internal set
			{
				this._extraSize = (short)value;
			}
		}

		public virtual int BytesPerSample
		{
			get
			{
				return this.BitsPerSample / 8;
			}
		}

		public virtual int BytesPerBlock
		{
			get
			{
				return this.BytesPerSample * this.Channels;
			}
		}

		public virtual AudioEncoding WaveFormatTag
		{
			get
			{
				return this._encoding;
			}
			protected internal set
			{
				this._encoding = value;
			}
		}

		public WaveFormat() : this(44100, 16, 2)
		{
		}

		public WaveFormat(int sampleRate, int bits, int channels) : this(sampleRate, bits, channels, AudioEncoding.Pcm)
		{
		}

		public WaveFormat(int sampleRate, int bits, int channels, AudioEncoding encoding) : this(sampleRate, bits, channels, encoding, 0)
		{
		}

		public WaveFormat(int sampleRate, int bits, int channels, AudioEncoding encoding, int extraSize)
		{
			if (sampleRate < 1)
			{
				throw new ArgumentOutOfRangeException("sampleRate");
			}
			if (bits < 0)
			{
				throw new ArgumentOutOfRangeException("bits");
			}
			if (channels < 1)
			{
				throw new ArgumentOutOfRangeException("channels", "Number of channels has to be bigger than 0.");
			}
			this._sampleRate = sampleRate;
			this._bitsPerSample = (short)bits;
			this._channels = (short)channels;
			this._encoding = encoding;
			this._extraSize = (short)extraSize;
			this.UpdateProperties();
		}

		public long MillisecondsToBytes(double milliseconds)
		{
			long num = (long)((double)this.BytesPerSecond / 1000.0 * milliseconds);
			return num - num % (long)this.BlockAlign;
		}

		public double BytesToMilliseconds(long bytes)
		{
			bytes -= bytes % (long)this.BlockAlign;
			return (double)bytes / (double)this.BytesPerSecond * 1000.0;
		}

		public virtual bool Equals(WaveFormat other)
		{
			return this.Channels == other.Channels && this.SampleRate == other.SampleRate && this.BytesPerSecond == other.BytesPerSecond && this.BlockAlign == other.BlockAlign && this.BitsPerSample == other.BitsPerSample && this.ExtraSize == other.ExtraSize && this.WaveFormatTag == other.WaveFormatTag;
		}

		public override string ToString()
		{
			return this.GetInformation().ToString();
		}

		public virtual object Clone()
		{
			return base.MemberwiseClone();
		}

		internal virtual void SetWaveFormatTagInternal(AudioEncoding waveFormatTag)
		{
			this.WaveFormatTag = waveFormatTag;
		}

		internal virtual void SetBitsPerSampleAndFormatProperties(int bitsPerSample)
		{
			this.BitsPerSample = bitsPerSample;
			this.UpdateProperties();
		}

		protected internal virtual void UpdateProperties()
		{
			this.BlockAlign = this.BitsPerSample / 8 * this.Channels;
			this.BytesPerSecond = this.BlockAlign * this.SampleRate;
		}

		[DebuggerStepThrough]
		private StringBuilder GetInformation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ChannelsAvailable: " + this.Channels.ToString());
			stringBuilder.Append("|SampleRate: " + this.SampleRate.ToString());
			stringBuilder.Append("|Bps: " + this.BytesPerSecond.ToString());
			stringBuilder.Append("|BlockAlign: " + this.BlockAlign.ToString());
			stringBuilder.Append("|BitsPerSample: " + this.BitsPerSample.ToString());
			stringBuilder.Append("|Encoding: " + this._encoding.ToString());
			return stringBuilder;
		}

		private AudioEncoding _encoding;

		private short _channels;

		private int _sampleRate;

		private int _bytesPerSecond;

		private short _blockAlign;

		private short _bitsPerSample;

		private short _extraSize;
	}
}
