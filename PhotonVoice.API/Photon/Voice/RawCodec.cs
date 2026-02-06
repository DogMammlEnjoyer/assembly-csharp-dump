using System;
using System.Runtime.InteropServices;

namespace Photon.Voice
{
	public class RawCodec
	{
		public class Encoder<T> : IEncoderDirect<T[]>, IEncoder, IDisposable
		{
			public string Error { get; private set; }

			public Action<ArraySegment<byte>, FrameFlags> Output { get; set; }

			public ArraySegment<byte> DequeueOutput(out FrameFlags flags)
			{
				flags = (FrameFlags)0;
				return RawCodec.Encoder<T>.EmptyBuffer;
			}

			public void EndOfStream()
			{
			}

			public I GetPlatformAPI<I>() where I : class
			{
				return default(I);
			}

			public void Dispose()
			{
			}

			public void Input(T[] buf)
			{
				if (this.Error != null)
				{
					return;
				}
				if (this.Output == null)
				{
					this.Error = "RawCodec.Encoder: Output action is not set";
					return;
				}
				if (buf == null)
				{
					return;
				}
				if (buf.Length == 0)
				{
					return;
				}
				int num = buf.Length * this.sizeofT;
				if (this.byteBuf.Length < num)
				{
					this.byteBuf = new byte[num];
				}
				Buffer.BlockCopy(buf, 0, this.byteBuf, 0, num);
				this.Output(new ArraySegment<byte>(this.byteBuf, 0, num), (FrameFlags)0);
			}

			private int sizeofT = Marshal.SizeOf<T>(default(T));

			private byte[] byteBuf = new byte[0];

			private static readonly ArraySegment<byte> EmptyBuffer = new ArraySegment<byte>(new byte[0]);
		}

		public class Decoder<T> : IDecoder, IDisposable
		{
			public string Error { get; private set; }

			public Decoder(Action<FrameOut<T>> output)
			{
				this.output = output;
			}

			public void Open(VoiceInfo info)
			{
			}

			public void Input(ref FrameBuffer byteBuf)
			{
				if (byteBuf.Array == null)
				{
					return;
				}
				if (byteBuf.Length == 0)
				{
					return;
				}
				int num = byteBuf.Length / this.sizeofT;
				if (this.buf.Length < num)
				{
					this.buf = new T[num];
				}
				Buffer.BlockCopy(byteBuf.Array, byteBuf.Offset, this.buf, 0, byteBuf.Length);
				this.output(new FrameOut<T>((T[])this.buf, false));
			}

			public void Dispose()
			{
			}

			private T[] buf = new T[0];

			private int sizeofT = Marshal.SizeOf<T>(default(T));

			private Action<FrameOut<T>> output;
		}

		public class ShortToFloat
		{
			public ShortToFloat(Action<FrameOut<float>> output)
			{
				this.output = output;
			}

			public void Output(FrameOut<short> shortBuf)
			{
				if (this.buf.Length < shortBuf.Buf.Length)
				{
					this.buf = new float[shortBuf.Buf.Length];
				}
				AudioUtil.Convert(shortBuf.Buf, this.buf, shortBuf.Buf.Length);
				this.output(new FrameOut<float>((float[])this.buf, false));
			}

			private Action<FrameOut<float>> output;

			private float[] buf = new float[0];
		}
	}
}
