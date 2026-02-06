using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CSCore
{
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	public class WaveFormatExtensible : WaveFormat
	{
		public static Guid SubTypeFromWaveFormat(WaveFormat waveFormat)
		{
			if (waveFormat == null)
			{
				throw new ArgumentNullException("waveFormat");
			}
			if (waveFormat is WaveFormatExtensible)
			{
				return ((WaveFormatExtensible)waveFormat).SubFormat;
			}
			return AudioSubTypes.SubTypeFromEncoding(waveFormat.WaveFormatTag);
		}

		public int ValidBitsPerSample
		{
			get
			{
				return (int)this._samplesUnion;
			}
			protected internal set
			{
				this._samplesUnion = (short)value;
			}
		}

		public int SamplesPerBlock
		{
			get
			{
				return (int)this._samplesUnion;
			}
			protected internal set
			{
				this._samplesUnion = (short)value;
			}
		}

		public ChannelMask ChannelMask
		{
			get
			{
				return this._channelMask;
			}
			protected internal set
			{
				this._channelMask = value;
			}
		}

		public Guid SubFormat
		{
			get
			{
				return this._subFormat;
			}
			protected internal set
			{
				this._subFormat = value;
			}
		}

		internal WaveFormatExtensible()
		{
		}

		public WaveFormatExtensible(int sampleRate, int bits, int channels, Guid subFormat) : base(sampleRate, bits, channels, AudioEncoding.Extensible, 22)
		{
			this._samplesUnion = (short)bits;
			this._subFormat = WaveFormatExtensible.SubTypeFromWaveFormat(this);
			int num = 0;
			for (int i = 0; i < channels; i++)
			{
				num |= 1 << i;
			}
			this._channelMask = (ChannelMask)num;
			this._subFormat = subFormat;
		}

		public WaveFormatExtensible(int sampleRate, int bits, int channels, Guid subFormat, ChannelMask channelMask) : this(sampleRate, bits, channels, subFormat)
		{
			Array values = Enum.GetValues(typeof(ChannelMask));
			int num = 0;
			for (int i = 0; i < values.Length; i++)
			{
				if ((channelMask & (ChannelMask)values.GetValue(i)) == (ChannelMask)values.GetValue(i))
				{
					num++;
				}
			}
			if (channels != num)
			{
				throw new ArgumentException("Channels has to equal the set flags in the channelmask.");
			}
			this._channelMask = channelMask;
		}

		public WaveFormat ToWaveFormat()
		{
			return new WaveFormat(this.SampleRate, this.BitsPerSample, this.Channels, AudioSubTypes.EncodingFromSubType(this.SubFormat));
		}

		public override object Clone()
		{
			return base.MemberwiseClone();
		}

		internal override void SetWaveFormatTagInternal(AudioEncoding waveFormatTag)
		{
			this.SubFormat = AudioSubTypes.SubTypeFromEncoding(waveFormatTag);
		}

		[DebuggerStepThrough]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.ToString());
			stringBuilder.Append("|SubFormat: " + this.SubFormat.ToString());
			stringBuilder.Append("|ChannelMask: " + this.ChannelMask.ToString());
			return stringBuilder.ToString();
		}

		internal const int WaveFormatExtensibleExtraSize = 22;

		private short _samplesUnion;

		private ChannelMask _channelMask;

		private Guid _subFormat;
	}
}
