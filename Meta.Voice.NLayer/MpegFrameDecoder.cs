using System;
using Meta.Voice.NLayer.Decoder;

namespace Meta.Voice.NLayer
{
	public class MpegFrameDecoder
	{
		public MpegFrameDecoder()
		{
			this._ch0 = new float[1152];
			this._ch1 = new float[1152];
		}

		public void SetEQ(float[] eq)
		{
			if (eq != null)
			{
				float[] array = new float[32];
				for (int i = 0; i < eq.Length; i++)
				{
					array[i] = (float)Math.Pow(2.0, (double)(eq[i] / 6f));
				}
				this._eqFactors = array;
				return;
			}
			this._eqFactors = null;
		}

		public StereoMode StereoMode { get; set; }

		public int DecodeFrame(IMpegFrame frame, byte[] dest, int destOffset)
		{
			if (frame == null)
			{
				throw new ArgumentNullException("frame");
			}
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			if (destOffset % 4 != 0)
			{
				throw new ArgumentException("Must be an even multiple of 4", "destOffset");
			}
			if ((dest.Length - destOffset) / 4 < ((frame.ChannelMode == MpegChannelMode.Mono) ? 1 : 2) * frame.SampleCount)
			{
				throw new ArgumentException("Buffer not large enough!  Must be big enough to hold the frame's entire output.  This is up to 9,216 bytes.", "dest");
			}
			return this.DecodeFrameImpl(frame, dest, destOffset / 4) * 4;
		}

		public int DecodeFrame(IMpegFrame frame, float[] dest, int destOffset)
		{
			if (frame == null)
			{
				throw new ArgumentNullException("frame");
			}
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			if (dest.Length - destOffset < ((frame.ChannelMode == MpegChannelMode.Mono) ? 1 : 2) * frame.SampleCount)
			{
				throw new ArgumentException("Buffer not large enough!  Must be big enough to hold the frame's entire output.  This is up to 2,304 elements.", "dest");
			}
			return this.DecodeFrameImpl(frame, dest, destOffset);
		}

		private int DecodeFrameImpl(IMpegFrame frame, Array dest, int destOffset)
		{
			frame.Reset();
			LayerDecoderBase layerDecoderBase = null;
			switch (frame.Layer)
			{
			case MpegLayer.LayerI:
				if (this._layerIDecoder == null)
				{
					this._layerIDecoder = new LayerIDecoder();
				}
				layerDecoderBase = this._layerIDecoder;
				break;
			case MpegLayer.LayerII:
				if (this._layerIIDecoder == null)
				{
					this._layerIIDecoder = new LayerIIDecoder();
				}
				layerDecoderBase = this._layerIIDecoder;
				break;
			case MpegLayer.LayerIII:
				if (this._layerIIIDecoder == null)
				{
					this._layerIIIDecoder = new LayerIIIDecoder();
				}
				layerDecoderBase = this._layerIIIDecoder;
				break;
			}
			if (layerDecoderBase != null)
			{
				layerDecoderBase.SetEQ(this._eqFactors);
				layerDecoderBase.StereoMode = this.StereoMode;
				int num = layerDecoderBase.DecodeFrame(frame, this._ch0, this._ch1);
				if (frame.ChannelMode == MpegChannelMode.Mono)
				{
					Buffer.BlockCopy(this._ch0, 0, dest, destOffset * 4, num * 4);
				}
				else
				{
					for (int i = 0; i < num; i++)
					{
						Buffer.BlockCopy(this._ch0, i * 4, dest, destOffset * 4, 4);
						destOffset++;
						Buffer.BlockCopy(this._ch1, i * 4, dest, destOffset * 4, 4);
						destOffset++;
					}
					num *= 2;
				}
				return num;
			}
			return 0;
		}

		public void Reset()
		{
			if (this._layerIDecoder != null)
			{
				this._layerIDecoder.ResetForSeek();
			}
			if (this._layerIIDecoder != null)
			{
				this._layerIIDecoder.ResetForSeek();
			}
			if (this._layerIIIDecoder != null)
			{
				this._layerIIIDecoder.ResetForSeek();
			}
		}

		private LayerIDecoder _layerIDecoder;

		private LayerIIDecoder _layerIIDecoder;

		private LayerIIIDecoder _layerIIIDecoder = new LayerIIIDecoder();

		private float[] _eqFactors;

		private float[] _ch0;

		private float[] _ch1;
	}
}
