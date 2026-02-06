using System;
using POpusCodec;
using POpusCodec.Enums;

namespace Photon.Voice
{
	public class OpusCodec
	{
		public static string Version
		{
			get
			{
				return OpusLib.Version;
			}
		}

		public enum FrameDuration
		{
			Frame2dot5ms = 2500,
			Frame5ms = 5000,
			Frame10ms = 10000,
			Frame20ms = 20000,
			Frame40ms = 40000,
			Frame60ms = 60000
		}

		public static class Factory
		{
			public static IEncoder CreateEncoder<B>(VoiceInfo i, ILogger logger)
			{
				if (typeof(B) == typeof(float[]))
				{
					return new OpusCodec.EncoderFloat(i, logger);
				}
				if (typeof(B) == typeof(short[]))
				{
					return new OpusCodec.EncoderShort(i, logger);
				}
				string str = "Factory.CreateEncoder<";
				Type typeFromHandle = typeof(B);
				throw new UnsupportedCodecException(str + ((typeFromHandle != null) ? typeFromHandle.ToString() : null) + ">", i.Codec);
			}
		}

		public static class DecoderFactory
		{
			public static IEncoder Create<T>(VoiceInfo i, ILogger logger)
			{
				T[] array = new T[1];
				if (array[0].GetType() == typeof(float))
				{
					return new OpusCodec.EncoderFloat(i, logger);
				}
				if (array[0].GetType() == typeof(short))
				{
					return new OpusCodec.EncoderShort(i, logger);
				}
				string str = "EncoderFactory.Create<";
				Type type = array[0].GetType();
				throw new UnsupportedCodecException(str + ((type != null) ? type.ToString() : null) + ">", i.Codec);
			}
		}

		public abstract class Encoder<T> : IEncoderDirect<T[]>, IEncoder, IDisposable
		{
			protected Encoder(VoiceInfo i, ILogger logger)
			{
				try
				{
					this.encoder = new OpusEncoder((SamplingRate)i.SamplingRate, (Channels)i.Channels, i.Bitrate, OpusApplicationType.Voip, (Delay)(i.FrameDurationUs * 2 / 1000));
					logger.LogInfo(string.Concat(new string[]
					{
						"[PV] OpusCodec.Encoder created. Opus version ",
						OpusCodec.Version,
						". Bitrate ",
						this.encoder.Bitrate.ToString(),
						". EncoderDelay ",
						this.encoder.EncoderDelay.ToString()
					}), Array.Empty<object>());
				}
				catch (Exception ex)
				{
					this.Error = ex.ToString();
					if (this.Error == null)
					{
						this.Error = "Exception in OpusCodec.Encoder constructor";
					}
					logger.LogError("[PV] OpusCodec.Encoder: " + this.Error, Array.Empty<object>());
				}
			}

			public string Error { get; private set; }

			public Action<ArraySegment<byte>, FrameFlags> Output { get; set; }

			public void Input(T[] buf)
			{
				if (this.Error != null)
				{
					return;
				}
				if (this.Output == null)
				{
					this.Error = "OpusCodec.Encoder: Output action is not set";
					return;
				}
				lock (this)
				{
					if (!this.disposed)
					{
						if (this.Error == null)
						{
							ArraySegment<byte> arg = this.encodeTyped(buf);
							if (arg.Count != 0)
							{
								this.Output(arg, (FrameFlags)0);
							}
						}
					}
				}
			}

			public void EndOfStream()
			{
				lock (this)
				{
					if (!this.disposed)
					{
						if (this.Error == null)
						{
							this.Output(OpusCodec.Encoder<T>.EmptyBuffer, FrameFlags.EndOfStream);
						}
					}
				}
			}

			public ArraySegment<byte> DequeueOutput(out FrameFlags flags)
			{
				flags = (FrameFlags)0;
				return OpusCodec.Encoder<T>.EmptyBuffer;
			}

			protected abstract ArraySegment<byte> encodeTyped(T[] buf);

			public I GetPlatformAPI<I>() where I : class
			{
				return default(I);
			}

			public void Dispose()
			{
				lock (this)
				{
					if (this.encoder != null)
					{
						this.encoder.Dispose();
					}
					this.disposed = true;
				}
			}

			protected OpusEncoder encoder;

			protected bool disposed;

			private static readonly ArraySegment<byte> EmptyBuffer = new ArraySegment<byte>(new byte[0]);
		}

		public class EncoderFloat : OpusCodec.Encoder<float>
		{
			internal EncoderFloat(VoiceInfo i, ILogger logger) : base(i, logger)
			{
			}

			protected override ArraySegment<byte> encodeTyped(float[] buf)
			{
				return this.encoder.Encode(buf);
			}
		}

		public class EncoderShort : OpusCodec.Encoder<short>
		{
			internal EncoderShort(VoiceInfo i, ILogger logger) : base(i, logger)
			{
			}

			protected override ArraySegment<byte> encodeTyped(short[] buf)
			{
				return this.encoder.Encode(buf);
			}
		}

		public class Decoder<T> : IDecoder, IDisposable
		{
			public Decoder(Action<FrameOut<T>> output, ILogger logger)
			{
				this.output = output;
				this.logger = logger;
			}

			public void Open(VoiceInfo i)
			{
				try
				{
					this.decoder = new OpusDecoder<T>((SamplingRate)i.SamplingRate, (Channels)i.Channels);
					this.logger.LogInfo("[PV] OpusCodec.Decoder created. Opus version " + OpusCodec.Version, Array.Empty<object>());
				}
				catch (Exception ex)
				{
					this.Error = ex.ToString();
					if (this.Error == null)
					{
						this.Error = "Exception in OpusCodec.Decoder constructor";
					}
					this.logger.LogError("[PV] OpusCodec.Decoder: " + this.Error, Array.Empty<object>());
				}
			}

			public string Error { get; private set; }

			public void Dispose()
			{
				if (this.decoder != null)
				{
					this.decoder.Dispose();
				}
			}

			public void Input(ref FrameBuffer buf)
			{
				if (this.Error == null)
				{
					if ((buf.Flags & FrameFlags.EndOfStream) > (FrameFlags)0)
					{
						T[] array = null;
						if (buf.Array == null && buf.Length > 0)
						{
							array = this.decoder.DecodePacket(ref buf);
						}
						T[] array2 = this.decoder.DecodeEndOfStream();
						if (array != null && array.Length == 0)
						{
							if (array2 != null && array2.Length != 0)
							{
								this.output(this.frameOut.Set(array, false));
							}
							else
							{
								array2 = array;
							}
						}
						this.output(this.frameOut.Set(array2, true));
						return;
					}
					T[] array3 = this.decoder.DecodePacket(ref buf);
					if (array3.Length != 0)
					{
						this.output(this.frameOut.Set(array3, false));
					}
				}
			}

			protected OpusDecoder<T> decoder;

			private ILogger logger;

			private Action<FrameOut<T>> output;

			private FrameOut<T> frameOut = new FrameOut<T>(null, false);
		}

		public class Util
		{
			internal static int bestEncoderSampleRate(int f)
			{
				int num = int.MaxValue;
				int result = 48000;
				foreach (object obj in Enum.GetValues(typeof(SamplingRate)))
				{
					int num2 = Math.Abs((int)obj - f);
					if (num2 < num)
					{
						num = num2;
						result = (int)obj;
					}
				}
				return result;
			}
		}
	}
}
