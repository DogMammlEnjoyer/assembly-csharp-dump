using System;
using System.Runtime.InteropServices;
using Photon.Voice;
using POpusCodec.Enums;

namespace POpusCodec
{
	public class OpusDecoder<T> : IDisposable
	{
		public Bandwidth? PreviousPacketBandwidth
		{
			get
			{
				return this._previousPacketBandwidth;
			}
		}

		public OpusDecoder(SamplingRate outputSamplingRateHz, Channels numChannels)
		{
			this.TisFloat = (default(T) is float);
			this.sizeofT = Marshal.SizeOf<T>(default(T));
			if (outputSamplingRateHz != SamplingRate.Sampling08000 && outputSamplingRateHz != SamplingRate.Sampling12000 && outputSamplingRateHz != SamplingRate.Sampling16000 && outputSamplingRateHz != SamplingRate.Sampling24000 && outputSamplingRateHz != SamplingRate.Sampling48000)
			{
				throw new ArgumentOutOfRangeException("outputSamplingRateHz", "Must use one of the pre-defined sampling rates (" + outputSamplingRateHz.ToString() + ")");
			}
			if (numChannels != Channels.Mono && numChannels != Channels.Stereo)
			{
				throw new ArgumentOutOfRangeException("numChannels", "Must be Mono or Stereo");
			}
			this._channelCount = (int)numChannels;
			this._handle = Wrapper.opus_decoder_create(outputSamplingRateHz, numChannels);
			if (this._handle == IntPtr.Zero)
			{
				throw new OpusException(OpusStatusCode.AllocFail, "Memory was not allocated for the encoder");
			}
		}

		public T[] DecodePacket(ref FrameBuffer packetData)
		{
			if (this.buffer == null && packetData.Array == null)
			{
				return OpusDecoder<T>.EmptyBuffer;
			}
			int num = 0;
			if (this.buffer == null)
			{
				this.buffer = new T[5760 * this._channelCount];
			}
			bool flag = packetData.Array == null || Wrapper.opus_packet_get_bandwidth(packetData.Ptr) == -4;
			bool flag2 = false;
			if (this.prevPacketInvalid)
			{
				if (flag)
				{
					num = (this.TisFloat ? Wrapper.opus_decode(this._handle, default(FrameBuffer), this.buffer as float[], 0, this._channelCount) : Wrapper.opus_decode(this._handle, default(FrameBuffer), this.buffer as short[], 0, this._channelCount));
				}
				else
				{
					num = (this.TisFloat ? Wrapper.opus_decode(this._handle, packetData, this.buffer as float[], 1, this._channelCount) : Wrapper.opus_decode(this._handle, packetData, this.buffer as short[], 1, this._channelCount));
				}
			}
			else if (this.prevPacketData.Array != null)
			{
				num = (this.TisFloat ? Wrapper.opus_decode(this._handle, this.prevPacketData, this.buffer as float[], 0, this._channelCount) : Wrapper.opus_decode(this._handle, this.prevPacketData, this.buffer as short[], 0, this._channelCount));
				flag2 = true;
			}
			this.prevPacketData.Release();
			this.prevPacketData = packetData;
			packetData.Retain();
			this.prevPacketInvalid = flag;
			if (num == 0)
			{
				return OpusDecoder<T>.EmptyBuffer;
			}
			if (this.buffer.Length != num * this._channelCount)
			{
				if (!flag2)
				{
					return OpusDecoder<T>.EmptyBuffer;
				}
				Array src = this.buffer;
				this.buffer = new T[num * this._channelCount];
				Buffer.BlockCopy(src, 0, this.buffer, 0, num * this.sizeofT);
			}
			return this.buffer;
		}

		public T[] DecodeEndOfStream()
		{
			int num = 0;
			if (this.prevPacketInvalid)
			{
				this.prevPacketData.Release();
				this.prevPacketData = default(FrameBuffer);
				this.prevPacketInvalid = false;
				return OpusDecoder<T>.EmptyBuffer;
			}
			if (this.buffer == null)
			{
				this.buffer = new T[5760 * this._channelCount];
			}
			if (this.prevPacketData.Array != null)
			{
				num = (this.TisFloat ? Wrapper.opus_decode(this._handle, this.prevPacketData, this.buffer as float[], 1, this._channelCount) : Wrapper.opus_decode(this._handle, this.prevPacketData, this.buffer as short[], 1, this._channelCount));
			}
			this.prevPacketData.Release();
			this.prevPacketData = default(FrameBuffer);
			this.prevPacketInvalid = false;
			if (num == 0)
			{
				return OpusDecoder<T>.EmptyBuffer;
			}
			if (this.buffer.Length != num * this._channelCount)
			{
				Array src = this.buffer;
				this.buffer = new T[num * this._channelCount];
				Buffer.BlockCopy(src, 0, this.buffer, 0, num * this.sizeofT);
			}
			return this.buffer;
		}

		public void Dispose()
		{
			this.prevPacketData.Release();
			if (this._handle != IntPtr.Zero)
			{
				Wrapper.opus_decoder_destroy(this._handle);
				this._handle = IntPtr.Zero;
			}
		}

		private const bool UseInbandFEC = true;

		private bool TisFloat;

		private int sizeofT;

		private IntPtr _handle = IntPtr.Zero;

		private const int MaxFrameSize = 5760;

		private int _channelCount;

		private static readonly T[] EmptyBuffer = new T[0];

		private Bandwidth? _previousPacketBandwidth;

		private T[] buffer;

		private FrameBuffer prevPacketData;

		private bool prevPacketInvalid;
	}
}
