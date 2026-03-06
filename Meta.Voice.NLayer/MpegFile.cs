using System;
using System.IO;
using Meta.Voice.NLayer.Decoder;

namespace Meta.Voice.NLayer
{
	public class MpegFile : IDisposable
	{
		public MpegFile(string fileName)
		{
			this.Init(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read), true);
		}

		public MpegFile(Stream stream)
		{
			this.Init(stream, false);
		}

		private void Init(Stream stream, bool closeStream)
		{
			this._stream = stream;
			this._closeStream = closeStream;
			this._reader = new MpegStreamReader(this._stream);
			this._decoder = new MpegFrameDecoder();
		}

		public void Dispose()
		{
			if (this._closeStream)
			{
				this._stream.Dispose();
				this._closeStream = false;
			}
		}

		public int SampleRate
		{
			get
			{
				return this._reader.SampleRate;
			}
		}

		public int Channels
		{
			get
			{
				return this._reader.Channels;
			}
		}

		public bool CanSeek
		{
			get
			{
				return this._reader.CanSeek;
			}
		}

		public long Length
		{
			get
			{
				return this._reader.SampleCount * (long)this._reader.Channels * 4L;
			}
		}

		public TimeSpan Duration
		{
			get
			{
				long sampleCount = this._reader.SampleCount;
				if (sampleCount == -1L)
				{
					return TimeSpan.Zero;
				}
				return TimeSpan.FromSeconds((double)sampleCount / (double)this._reader.SampleRate);
			}
		}

		public long Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (!this._reader.CanSeek)
				{
					throw new InvalidOperationException("Cannot Seek!");
				}
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				long num = value / 4L / (long)this._reader.Channels;
				int num2 = 0;
				if (num >= (long)this._reader.FirstFrameSampleCount)
				{
					num2 = this._reader.FirstFrameSampleCount;
					num -= (long)num2;
				}
				object seekLock = this._seekLock;
				lock (seekLock)
				{
					long num3 = this._reader.SeekTo(num);
					if (num3 == -1L)
					{
						throw new ArgumentOutOfRangeException("value");
					}
					this._decoder.Reset();
					if (num2 != 0)
					{
						this._decoder.DecodeFrame(this._reader.NextFrame(), this._readBuf, 0);
						num3 += (long)num2;
					}
					this._position = num3 * 4L * (long)this._reader.Channels;
					this._eofFound = false;
					this._readBufOfs = (this._readBufLen = 0);
				}
			}
		}

		public TimeSpan Time
		{
			get
			{
				return TimeSpan.FromSeconds((double)this._position / 4.0 / (double)this._reader.Channels / (double)this._reader.SampleRate);
			}
			set
			{
				this.Position = (long)(value.TotalSeconds * (double)this._reader.SampleRate * (double)this._reader.Channels * 4.0);
			}
		}

		public void SetEQ(float[] eq)
		{
			this._decoder.SetEQ(eq);
		}

		public StereoMode StereoMode
		{
			get
			{
				return this._decoder.StereoMode;
			}
			set
			{
				this._decoder.StereoMode = value;
			}
		}

		public int ReadSamples(byte[] buffer, int index, int count)
		{
			if (index < 0 || index + count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			count -= count % 4;
			return this.ReadSamplesImpl(buffer, index, count, 32);
		}

		public int ReadSamples(float[] buffer, int index, int count)
		{
			if (index < 0 || index + count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return this.ReadSamplesImpl(buffer, index * 4, count * 4, 32) / 4;
		}

		public int ReadSamplesInt16(byte[] buffer, int index, int count)
		{
			if (index < 0 || index + count > buffer.Length * 2)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return this.ReadSamplesImpl(buffer, index, count, 16) * 2 / 4;
		}

		public int ReadSamplesInt8(byte[] buffer, int index, int count)
		{
			if (index < 0 || index + count > buffer.Length * 4)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return this.ReadSamplesImpl(buffer, index, count, 8) / 4;
		}

		private int ReadSamplesImpl(Array buffer, int index, int count, int bitDepth)
		{
			int num = 0;
			object seekLock = this._seekLock;
			lock (seekLock)
			{
				while (count > 0)
				{
					if (this._readBufLen > this._readBufOfs)
					{
						int num2 = this._readBufLen - this._readBufOfs;
						if (num2 > count)
						{
							num2 = count;
						}
						if (bitDepth == 32)
						{
							Buffer.BlockCopy(this._readBuf, this._readBufOfs, buffer, index, num2);
						}
						else
						{
							for (int i = 0; i < num2 / 4; i++)
							{
								if (bitDepth != 8)
								{
									if (bitDepth == 16)
									{
										int num3 = (int)Math.Round((double)(32767.5f * this._readBuf[this._readBufOfs / 4 + i] - 0.5f));
										if (num3 < 0)
										{
											num3 += 65536;
										}
										buffer.SetValue((byte)(num3 % 256), 2 * (index / 4 + i));
										buffer.SetValue((byte)(num3 / 256), 2 * (index / 4 + i) + 1);
									}
								}
								else
								{
									buffer.SetValue((byte)Math.Round((double)(127.5f * this._readBuf[this._readBufOfs / 4 + i] + 127.5f)), index / 4 + i);
								}
							}
						}
						num += num2;
						count -= num2;
						index += num2;
						this._position += (long)num2;
						this._readBufOfs += num2;
						if (this._readBufOfs == this._readBufLen)
						{
							this._readBufLen = 0;
						}
					}
					if (this._readBufLen == 0)
					{
						if (this._eofFound)
						{
							break;
						}
						MpegFrame mpegFrame = this._reader.NextFrame();
						if (mpegFrame == null)
						{
							this._eofFound = true;
							break;
						}
						try
						{
							this._readBufLen = this._decoder.DecodeFrame(mpegFrame, this._readBuf, 0) * 4;
							this._readBufOfs = 0;
						}
						catch (InvalidDataException)
						{
							this._decoder.Reset();
							this._readBufOfs = (this._readBufLen = 0);
						}
						catch (EndOfStreamException)
						{
							this._eofFound = true;
							break;
						}
						finally
						{
							mpegFrame.ClearBuffer();
						}
					}
				}
			}
			return num;
		}

		private Stream _stream;

		private bool _closeStream;

		private bool _eofFound;

		private MpegStreamReader _reader;

		private MpegFrameDecoder _decoder;

		private object _seekLock = new object();

		private long _position;

		private float[] _readBuf = new float[2304];

		private int _readBufLen;

		private int _readBufOfs;
	}
}
