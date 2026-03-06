using System;
using System.IO;
using System.Text;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Logging;
using Meta.Voice.NLayer;
using UnityEngine;
using UnityEngine.Scripting;

namespace Meta.Voice.Audio.Decoding
{
	[Preserve]
	internal class AudioDecoderMp3Frame : IMpegFrame, ILogSource
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Audio, null);

		public bool IsHeaderDecoded
		{
			get
			{
				return this._dataOffset >= 4;
			}
		}

		private void Clear()
		{
			this._dataOffset = 0;
			this.Reset();
		}

		public void Reset()
		{
			this._readOffset = 4 + (this.HasCrc ? 2 : 0);
			this._bitBucket = 0UL;
			this._bitsRead = 0;
		}

		public int Decode(byte[] buffer, int bufferOffset, int bufferLength, AudioSampleDecodeDelegate onSamplesDecoded)
		{
			int num = 0;
			if (!this.IsHeaderDecoded)
			{
				num = Mathf.Min(4 - this._dataOffset, bufferLength);
				Array.Copy(buffer, bufferOffset, this._dataBuffer, this._dataOffset, num);
				this._dataOffset += num;
				if (!this.IsHeaderDecoded)
				{
					return num;
				}
				try
				{
					this.DecodeHeader();
				}
				catch (Exception ex)
				{
					this.Logger.Error("MP3 Frame {0} - Header Decode Failed\n\n{1}\n{2}", new object[]
					{
						this._frameIndex,
						ex,
						this
					});
					this._frameIndex += 1U;
					this.Clear();
					return num;
				}
				if (this._dataBuffer.Length < this.FrameLength)
				{
					this.Logger.Error("MP3 Frame {0} - Data Buffer Needs Increase\nNew Frame Length: {1}\nOld Frame Length: {2}\n{3}", new object[]
					{
						this._frameIndex,
						this.FrameLength,
						this._dataBuffer.Length,
						this
					});
				}
				if (this._sampleBuffer.Length < this.SampleCount)
				{
					this.Logger.Error("MP3 Frame {0} - Sample Buffer Needs Increase\nNew Sample Count: {1}\nOld Sample Count: {2}\n{3}", new object[]
					{
						this._frameIndex,
						this.SampleCount,
						this._sampleBuffer.Length,
						this
					});
				}
			}
			int num2 = Mathf.Min(this.FrameLength - this._dataOffset, bufferLength - num);
			Array.Copy(buffer, bufferOffset + num, this._dataBuffer, this._dataOffset, num2);
			this._dataOffset += num2;
			num += num2;
			if (this._dataOffset < this.FrameLength)
			{
				return num;
			}
			int length = this._decoder.DecodeFrame(this, this._sampleBuffer, 0);
			if (onSamplesDecoded != null)
			{
				onSamplesDecoded(this._sampleBuffer, 0, length);
			}
			this._frameIndex += 1U;
			this.Clear();
			return num;
		}

		public MpegVersion Version { get; private set; }

		public MpegLayer Layer { get; private set; }

		public MpegChannelMode ChannelMode { get; private set; }

		public int ChannelModeExtension { get; private set; }

		public int BitRateIndex { get; private set; }

		public int BitRate { get; private set; }

		public int SampleRateIndex { get; private set; }

		public int SampleRate { get; private set; }

		public bool IsCopyrighted { get; private set; }

		public bool HasCrc { get; private set; }

		public bool IsCorrupted { get; private set; }

		public int FrameLength { get; private set; }

		public int SampleCount { get; private set; }

		public static void Reverse<T>(T[] array, int start, int length)
		{
			for (int i = 0; i < length / 2; i++)
			{
				int num = start + i;
				int num2 = start + length - i - 1;
				int num3 = num;
				int num4 = num2;
				T t = array[num2];
				T t2 = array[num];
				array[num3] = t;
				array[num4] = t2;
			}
		}

		private void DecodeHeader()
		{
			AudioDecoderMp3Frame.Reverse<byte>(this._dataBuffer, 0, 4);
			int num = BitConverter.ToInt32(this._dataBuffer, 0);
			if ((AudioDecoderMp3Frame.BitRShift(num, 21) & 2047) != 2047)
			{
				throw new Exception(string.Format("Invalid frame {0} sync\nBits: {1}", this._frameIndex, AudioDecoderMp3Frame.GetBitString(num)));
			}
			this.Version = AudioDecoderMp3Frame.GetMpegVersion(num);
			this.Layer = AudioDecoderMp3Frame.GetMpegLayer(num);
			this.HasCrc = ((AudioDecoderMp3Frame.BitRShift(num, 16) & 1) == 0);
			this.BitRateIndex = (AudioDecoderMp3Frame.BitRShift(num, 12) & 15);
			if (this.BitRateIndex > 0)
			{
				this.BitRate = AudioDecoderMp3Frame._bitRateTable[this.Version / MpegVersion.Version1 - (MpegVersion)1][this.Layer - MpegLayer.LayerI][this.BitRateIndex] * 1000;
				this.SampleRateIndex = (AudioDecoderMp3Frame.BitRShift(num, 10) & 3);
				switch (this.SampleRateIndex)
				{
				case 0:
					this.SampleRate = 44100;
					break;
				case 1:
					this.SampleRate = 48000;
					break;
				case 2:
					this.SampleRate = 32000;
					break;
				default:
					this.SampleRate = 0;
					throw new Exception(string.Format("Invalid frame {0} Mpeg sample rate index\nBits: {1}", this._frameIndex, AudioDecoderMp3Frame.GetBitString(num)));
				}
				if (this.Version == MpegVersion.Version2)
				{
					this.SampleRate /= 2;
				}
				else if (this.Version == MpegVersion.Version25)
				{
					this.SampleRate /= 4;
				}
				if (this.Layer == MpegLayer.LayerI)
				{
					this.SampleCount = 384;
				}
				else if (this.Layer == MpegLayer.LayerIII && this.Version > MpegVersion.Version1)
				{
					this.SampleCount = 576;
				}
				else
				{
					this.SampleCount = 1152;
				}
				int num2 = AudioDecoderMp3Frame.BitRShift(num, 9) & 1;
				this.ChannelMode = (MpegChannelMode)(AudioDecoderMp3Frame.BitRShift(num, 6) & 3);
				this.ChannelModeExtension = (AudioDecoderMp3Frame.BitRShift(num, 4) & 3);
				this.IsCopyrighted = ((AudioDecoderMp3Frame.BitRShift(num, 3) & 1) != 0);
				if (this.BitRateIndex > 0)
				{
					if (this.Layer == MpegLayer.LayerI)
					{
						this.FrameLength = 12 * this.BitRate / this.SampleRate + num2;
						this.FrameLength <<= 2;
					}
					else
					{
						this.FrameLength = 144 * this.BitRate / this.SampleRate;
						if (this.Version == MpegVersion.Version2 || this.Version == MpegVersion.Version25)
						{
							this.FrameLength >>= 1;
						}
						this.FrameLength += num2;
					}
				}
				else
				{
					this.FrameLength = this._bitsRead + this.GetSideDataSize() + num2;
					this.BitRate = (this.FrameLength * 8 * this.SampleRate / this.SampleCount + 499 + 500) / 1000 * 1000;
				}
				this.IsCorrupted = false;
				return;
			}
			throw new Exception(string.Format("Invalid frame {0} bitrate index\nBits: {1}", this._frameIndex, AudioDecoderMp3Frame.GetBitString(num)));
		}

		private static MpegVersion GetMpegVersion(int header)
		{
			switch (AudioDecoderMp3Frame.BitRShift(header, 19) & 3)
			{
			case 0:
				return MpegVersion.Version25;
			case 1:
				return MpegVersion.Version1;
			case 2:
				return MpegVersion.Version2;
			default:
				throw new Exception("Invalid Mpeg Version\nBits: " + AudioDecoderMp3Frame.GetBitString(header));
			}
		}

		private static MpegLayer GetMpegLayer(int header)
		{
			int num = 4 - AudioDecoderMp3Frame.BitRShift(header, 17) & 3;
			if (num == 0)
			{
				throw new Exception("Invalid frame Mpeg Layer\nBits: " + AudioDecoderMp3Frame.GetBitString(header));
			}
			return (MpegLayer)num;
		}

		internal static int BitRShift(int number, int bits)
		{
			if (number >= 0)
			{
				return number >> bits;
			}
			return (number >> bits) + (2 << ~bits);
		}

		internal int GetSideDataSize()
		{
			switch (this.Layer)
			{
			case MpegLayer.LayerI:
				if (this.ChannelMode == MpegChannelMode.Mono)
				{
					return 16;
				}
				if (this.ChannelMode == MpegChannelMode.Stereo || this.ChannelMode == MpegChannelMode.DualChannel)
				{
					return 32;
				}
				switch (this.ChannelModeExtension)
				{
				case 0:
					return 18;
				case 1:
					return 20;
				case 2:
					return 22;
				case 3:
					return 24;
				}
				break;
			case MpegLayer.LayerII:
				return 0;
			case MpegLayer.LayerIII:
				if (this.ChannelMode == MpegChannelMode.Mono && this.Version >= MpegVersion.Version2)
				{
					return 9;
				}
				if (this.ChannelMode != MpegChannelMode.Mono && this.Version < MpegVersion.Version2)
				{
					return 32;
				}
				return 17;
			}
			return 0;
		}

		public int ReadBits(int bitCount)
		{
			if (bitCount < 1 || bitCount > 32)
			{
				throw new ArgumentOutOfRangeException("bitCount");
			}
			if (this.IsCorrupted)
			{
				return 0;
			}
			while (this._bitsRead < bitCount)
			{
				int num = this.ReadByte(this._readOffset);
				if (num == -1)
				{
					throw new EndOfStreamException();
				}
				this._readOffset++;
				this._bitBucket <<= 8;
				this._bitBucket |= (ulong)((byte)(num & 255));
				this._bitsRead += 8;
			}
			int result = (int)(this._bitBucket >> this._bitsRead - bitCount & (1UL << bitCount) - 1UL);
			this._bitsRead -= bitCount;
			return result;
		}

		private int ReadByte(int offset)
		{
			if (this._dataBuffer == null || offset < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			return (int)this._dataBuffer[offset];
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("MP3 Frame Data");
			if (!this.IsHeaderDecoded)
			{
				stringBuilder.AppendLine("\tNot yet decoded");
			}
			else
			{
				int headerData = BitConverter.ToInt32(this._dataBuffer, 0);
				stringBuilder.AppendLine("\tBits: " + AudioDecoderMp3Frame.GetBitString(headerData));
				stringBuilder.AppendLine("\tRaw: " + BitConverter.ToString(this._dataBuffer));
				stringBuilder.AppendLine("\tVersion: " + this.Version.ToString());
				stringBuilder.AppendLine("\tLayer: " + this.Layer.ToString());
				stringBuilder.AppendLine("\tChannel Mode: " + this.ChannelMode.ToString());
				stringBuilder.AppendLine(string.Format("\tCrc: {0}", this.HasCrc));
				stringBuilder.AppendLine(string.Format("\tCopyright: {0}", this.IsCopyrighted));
				stringBuilder.AppendLine(string.Format("\tBit Rate[{0}]: {1}", this.BitRateIndex, this.BitRate));
				stringBuilder.AppendLine(string.Format("\tSample Rate[{0}]: {1}", this.SampleRateIndex, this.SampleRate));
				stringBuilder.AppendLine(string.Format("\tSample Count: {0}", this.SampleCount));
				stringBuilder.AppendLine(string.Format("\tFrame Length: {0}", this.FrameLength));
			}
			return stringBuilder.ToString();
		}

		internal static string GetBitString(int headerData)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 31; i >= 0; i--)
			{
				stringBuilder.Append(AudioDecoderMp3Frame.BitRShift(headerData, i) & 1);
				if (i % 8 == 0 && i > 0)
				{
					stringBuilder.Append(" ");
				}
			}
			return stringBuilder.ToString();
		}

		private readonly byte[] _dataBuffer = new byte[192];

		private int _dataOffset;

		private const int HeaderLength = 4;

		private readonly float[] _sampleBuffer = new float[576];

		private readonly MpegFrameDecoder _decoder = new MpegFrameDecoder();

		private int _readOffset;

		private ulong _bitBucket;

		private int _bitsRead;

		private uint _frameIndex;

		private static readonly int[][][] _bitRateTable = new int[][][]
		{
			new int[][]
			{
				new int[]
				{
					0,
					32,
					64,
					96,
					128,
					160,
					192,
					224,
					256,
					288,
					320,
					352,
					384,
					416,
					448
				},
				new int[]
				{
					0,
					32,
					48,
					56,
					64,
					80,
					96,
					112,
					128,
					160,
					192,
					224,
					256,
					320,
					384
				},
				new int[]
				{
					0,
					32,
					40,
					48,
					56,
					64,
					80,
					96,
					112,
					128,
					160,
					192,
					224,
					256,
					320
				}
			},
			new int[][]
			{
				new int[]
				{
					0,
					32,
					48,
					56,
					64,
					80,
					96,
					112,
					128,
					144,
					160,
					176,
					192,
					224,
					256
				},
				new int[]
				{
					0,
					8,
					16,
					24,
					32,
					40,
					48,
					56,
					64,
					80,
					96,
					112,
					128,
					144,
					160
				},
				new int[]
				{
					0,
					8,
					16,
					24,
					32,
					40,
					48,
					56,
					64,
					80,
					96,
					112,
					128,
					144,
					160
				}
			}
		};

		private const int frameSyncMask = 2047;
	}
}
