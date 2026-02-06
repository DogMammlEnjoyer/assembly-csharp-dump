using System;
using POpusCodec.Enums;

namespace POpusCodec
{
	public class OpusEncoder : IDisposable
	{
		public SamplingRate InputSamplingRate
		{
			get
			{
				return this._inputSamplingRate;
			}
		}

		public Channels InputChannels
		{
			get
			{
				return this._inputChannels;
			}
		}

		public Delay EncoderDelay
		{
			get
			{
				return this._encoderDelay;
			}
			set
			{
				this._encoderDelay = value;
				this._frameSizePerChannel = (int)((int)(this._inputSamplingRate / (SamplingRate)1000) * (int)this._encoderDelay / 2m);
			}
		}

		public int FrameSizePerChannel
		{
			get
			{
				return this._frameSizePerChannel;
			}
		}

		public int Bitrate
		{
			get
			{
				return Wrapper.get_opus_encoder_ctl(this._handle, OpusCtlGetRequest.Bitrate);
			}
			set
			{
				Wrapper.set_opus_encoder_ctl(this._handle, OpusCtlSetRequest.Bitrate, value);
			}
		}

		public Bandwidth MaxBandwidth
		{
			get
			{
				return (Bandwidth)Wrapper.get_opus_encoder_ctl(this._handle, OpusCtlGetRequest.MaxBandwidth);
			}
			set
			{
				Wrapper.set_opus_encoder_ctl(this._handle, OpusCtlSetRequest.MaxBandwidth, (int)value);
			}
		}

		public Complexity Complexity
		{
			get
			{
				return (Complexity)Wrapper.get_opus_encoder_ctl(this._handle, OpusCtlGetRequest.Complexity);
			}
			set
			{
				Wrapper.set_opus_encoder_ctl(this._handle, OpusCtlSetRequest.Complexity, (int)value);
			}
		}

		public int ExpectedPacketLossPercentage
		{
			get
			{
				return Wrapper.get_opus_encoder_ctl(this._handle, OpusCtlGetRequest.PacketLossPercentage);
			}
			set
			{
				Wrapper.set_opus_encoder_ctl(this._handle, OpusCtlSetRequest.PacketLossPercentage, value);
			}
		}

		public SignalHint SignalHint
		{
			get
			{
				return (SignalHint)Wrapper.get_opus_encoder_ctl(this._handle, OpusCtlGetRequest.Signal);
			}
			set
			{
				Wrapper.set_opus_encoder_ctl(this._handle, OpusCtlSetRequest.Signal, (int)value);
			}
		}

		public ForceChannels ForceChannels
		{
			get
			{
				return (ForceChannels)Wrapper.get_opus_encoder_ctl(this._handle, OpusCtlGetRequest.ForceChannels);
			}
			set
			{
				Wrapper.set_opus_encoder_ctl(this._handle, OpusCtlSetRequest.ForceChannels, (int)value);
			}
		}

		public bool UseInbandFEC
		{
			get
			{
				return Wrapper.get_opus_encoder_ctl(this._handle, OpusCtlGetRequest.InbandFec) == 1;
			}
			set
			{
				Wrapper.set_opus_encoder_ctl(this._handle, OpusCtlSetRequest.InbandFec, value ? 1 : 0);
			}
		}

		public int PacketLossPercentage
		{
			get
			{
				return Wrapper.get_opus_encoder_ctl(this._handle, OpusCtlGetRequest.PacketLossPercentage);
			}
			set
			{
				Wrapper.set_opus_encoder_ctl(this._handle, OpusCtlSetRequest.PacketLossPercentage, value);
			}
		}

		public bool UseUnconstrainedVBR
		{
			get
			{
				return Wrapper.get_opus_encoder_ctl(this._handle, OpusCtlGetRequest.VBRConstraint) == 0;
			}
			set
			{
				Wrapper.set_opus_encoder_ctl(this._handle, OpusCtlSetRequest.VBRConstraint, value ? 0 : 1);
			}
		}

		public bool DtxEnabled
		{
			get
			{
				return Wrapper.get_opus_encoder_ctl(this._handle, OpusCtlGetRequest.Dtx) == 1;
			}
			set
			{
				Wrapper.set_opus_encoder_ctl(this._handle, OpusCtlSetRequest.Dtx, value ? 1 : 0);
			}
		}

		public OpusEncoder(SamplingRate inputSamplingRateHz, Channels numChannels, int bitrate, OpusApplicationType applicationType, Delay encoderDelay)
		{
			if (inputSamplingRateHz != SamplingRate.Sampling08000 && inputSamplingRateHz != SamplingRate.Sampling12000 && inputSamplingRateHz != SamplingRate.Sampling16000 && inputSamplingRateHz != SamplingRate.Sampling24000 && inputSamplingRateHz != SamplingRate.Sampling48000)
			{
				throw new ArgumentOutOfRangeException("inputSamplingRateHz", "Must use one of the pre-defined sampling rates(" + inputSamplingRateHz.ToString() + ")");
			}
			if (numChannels != Channels.Mono && numChannels != Channels.Stereo)
			{
				throw new ArgumentOutOfRangeException("numChannels", "Must be Mono or Stereo");
			}
			if (applicationType != OpusApplicationType.Audio && applicationType != OpusApplicationType.RestrictedLowDelay && applicationType != OpusApplicationType.Voip)
			{
				throw new ArgumentOutOfRangeException("applicationType", "Must use one of the pre-defined application types (" + applicationType.ToString() + ")");
			}
			if (encoderDelay != Delay.Delay10ms && encoderDelay != Delay.Delay20ms && encoderDelay != Delay.Delay2dot5ms && encoderDelay != Delay.Delay40ms && encoderDelay != Delay.Delay5ms && encoderDelay != Delay.Delay60ms)
			{
				throw new ArgumentOutOfRangeException("encoderDelay", "Must use one of the pre-defined delay values (" + encoderDelay.ToString() + ")");
			}
			this._inputSamplingRate = inputSamplingRateHz;
			this._inputChannels = numChannels;
			this._handle = Wrapper.opus_encoder_create(inputSamplingRateHz, numChannels, applicationType);
			if (this._handle == IntPtr.Zero)
			{
				throw new OpusException(OpusStatusCode.AllocFail, "Memory was not allocated for the encoder");
			}
			this.EncoderDelay = encoderDelay;
			this.Bitrate = bitrate;
			this.UseInbandFEC = true;
			this.PacketLossPercentage = 30;
		}

		public ArraySegment<byte> Encode(float[] pcmSamples)
		{
			int num = Wrapper.opus_encode(this._handle, pcmSamples, this._frameSizePerChannel, this.writePacket);
			if (num <= 1)
			{
				return OpusEncoder.EmptyBuffer;
			}
			return new ArraySegment<byte>(this.writePacket, 0, num);
		}

		public ArraySegment<byte> Encode(short[] pcmSamples)
		{
			int num = Wrapper.opus_encode(this._handle, pcmSamples, this._frameSizePerChannel, this.writePacket);
			if (num <= 1)
			{
				return OpusEncoder.EmptyBuffer;
			}
			return new ArraySegment<byte>(this.writePacket, 0, num);
		}

		public void Dispose()
		{
			if (this._handle != IntPtr.Zero)
			{
				Wrapper.opus_encoder_destroy(this._handle);
				this._handle = IntPtr.Zero;
			}
		}

		public const int BitrateMax = -1;

		private IntPtr _handle = IntPtr.Zero;

		private const int RecommendedMaxPacketSize = 4000;

		private int _frameSizePerChannel = 960;

		private SamplingRate _inputSamplingRate = SamplingRate.Sampling48000;

		private Channels _inputChannels = Channels.Stereo;

		private readonly byte[] writePacket = new byte[4000];

		private static readonly ArraySegment<byte> EmptyBuffer = new ArraySegment<byte>(new byte[0]);

		private Delay _encoderDelay = Delay.Delay20ms;
	}
}
