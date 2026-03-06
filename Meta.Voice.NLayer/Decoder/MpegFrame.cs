using System;
using System.IO;

namespace Meta.Voice.NLayer.Decoder
{
	internal class MpegFrame : FrameBase, IMpegFrame
	{
		internal static MpegFrame TrySync(uint syncMark)
		{
			if ((syncMark & 4292870144U) == 4292870144U && (syncMark & 1572864U) != 524288U && (syncMark & 393216U) != 0U && (syncMark & 61440U) != 61440U && (syncMark & 3072U) != 3072U)
			{
				switch (syncMark >> 4 & 15U)
				{
				case 0U:
				case 4U:
				case 5U:
				case 6U:
				case 7U:
				case 8U:
				case 12U:
					return new MpegFrame
					{
						_syncBits = (int)syncMark
					};
				}
			}
			return null;
		}

		private MpegFrame()
		{
		}

		protected override int Validate()
		{
			if (this.Layer == MpegLayer.LayerII)
			{
				int bitRate = this.BitRate;
				if (bitRate <= 80000)
				{
					if (bitRate <= 48000)
					{
						if (bitRate != 32000 && bitRate != 48000)
						{
							goto IL_86;
						}
					}
					else if (bitRate != 56000 && bitRate != 80000)
					{
						goto IL_86;
					}
					if (this.ChannelMode != MpegChannelMode.Mono)
					{
						return -1;
					}
				}
				else
				{
					if (bitRate <= 256000)
					{
						if (bitRate != 224000 && bitRate != 256000)
						{
							goto IL_86;
						}
					}
					else if (bitRate != 320000 && bitRate != 384000)
					{
						goto IL_86;
					}
					if (this.ChannelMode == MpegChannelMode.Mono)
					{
						return -1;
					}
				}
			}
			IL_86:
			int result;
			if (this.BitRateIndex > 0)
			{
				if (this.Layer == MpegLayer.LayerI)
				{
					result = (12 * this.BitRate / this.SampleRate + this.Padding) * 4;
				}
				else
				{
					result = 144 * this.BitRate / this.SampleRate + this.Padding;
				}
			}
			else
			{
				result = this._readOffset + this.GetSideDataSize() + this.Padding;
			}
			if (this.HasCrc)
			{
				this._readOffset = 4 + (this.HasCrc ? 2 : 0);
				if (!this.ValidateCRC())
				{
					this._isMuted = true;
					return 6;
				}
			}
			this.Reset();
			return result;
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

		private bool ValidateCRC()
		{
			uint num = 65535U;
			MpegFrame.UpdateCRC(this._syncBits, 16, ref num);
			bool flag = false;
			switch (this.Layer)
			{
			case MpegLayer.LayerI:
				flag = LayerIDecoder.GetCRC(this, ref num);
				break;
			case MpegLayer.LayerII:
				flag = LayerIIDecoder.GetCRC(this, ref num);
				break;
			case MpegLayer.LayerIII:
				flag = LayerIIIDecoder.GetCRC(this, ref num);
				break;
			}
			return !flag || (long)(base.ReadByte(4) << 8 | base.ReadByte(5)) == (long)((ulong)num);
		}

		internal static void UpdateCRC(int data, int length, ref uint crc)
		{
			uint num = 1U << length;
			while ((num >>= 1) != 0U)
			{
				int num2 = (int)(crc & 32768U);
				crc <<= 1;
				if (num2 == 0 ^ ((long)data & (long)((ulong)num)) == 0L)
				{
					crc ^= 32773U;
				}
			}
			crc &= 65535U;
		}

		internal VBRInfo ParseVBR()
		{
			byte[] array = new byte[4];
			int num;
			if (this.Version == MpegVersion.Version1 && this.ChannelMode != MpegChannelMode.Mono)
			{
				num = 36;
			}
			else if (this.Version > MpegVersion.Version1 && this.ChannelMode == MpegChannelMode.Mono)
			{
				num = 13;
			}
			else
			{
				num = 21;
			}
			if (base.Read(num, array) != 4)
			{
				return null;
			}
			if ((array[0] == 88 && array[1] == 105 && array[2] == 110 && array[3] == 103) || (array[0] == 73 && array[1] == 110 && array[2] == 102 && array[3] == 111))
			{
				return this.ParseXing(num + 4);
			}
			if (base.Read(36, array) != 4)
			{
				return null;
			}
			if (array[0] == 86 && array[1] == 66 && array[2] == 82 && array[3] == 73)
			{
				return this.ParseVBRI();
			}
			return null;
		}

		private VBRInfo ParseXing(int offset)
		{
			VBRInfo vbrinfo = new VBRInfo();
			vbrinfo.Channels = this.Channels;
			vbrinfo.SampleRate = this.SampleRate;
			vbrinfo.SampleCount = this.SampleCount;
			byte[] array = new byte[100];
			if (base.Read(offset, array, 0, 4) != 4)
			{
				return null;
			}
			offset += 4;
			int num = (int)array[0] << 24 | (int)array[1] << 16 | (int)array[2] << 8 | (int)array[3];
			if ((num & 1) != 0)
			{
				if (base.Read(offset, array, 0, 4) != 4)
				{
					return null;
				}
				offset += 4;
				vbrinfo.VBRFrames = ((int)array[0] << 24 | (int)array[1] << 16 | (int)array[2] << 8 | (int)array[3]);
			}
			if ((num & 2) != 0)
			{
				if (base.Read(offset, array, 0, 4) != 4)
				{
					return null;
				}
				offset += 4;
				vbrinfo.VBRBytes = ((int)array[0] << 24 | (int)array[1] << 16 | (int)array[2] << 8 | (int)array[3]);
			}
			if ((num & 4) != 0)
			{
				if (base.Read(offset, array) != 100)
				{
					return null;
				}
				offset += 100;
			}
			if ((num & 8) != 0)
			{
				if (base.Read(offset, array, 0, 4) != 4)
				{
					return null;
				}
				offset += 4;
				vbrinfo.VBRQuality = ((int)array[0] << 24 | (int)array[1] << 16 | (int)array[2] << 8 | (int)array[3]);
			}
			return vbrinfo;
		}

		private VBRInfo ParseVBRI()
		{
			VBRInfo vbrinfo = new VBRInfo();
			vbrinfo.Channels = this.Channels;
			vbrinfo.SampleRate = this.SampleRate;
			vbrinfo.SampleCount = this.SampleCount;
			byte[] array = new byte[26];
			if (base.Read(36, array) != 26)
			{
				return null;
			}
			byte b = array[4];
			byte b2 = array[5];
			vbrinfo.VBRDelay = ((int)array[6] << 8 | (int)array[7]);
			vbrinfo.VBRQuality = ((int)array[8] << 8 | (int)array[9]);
			vbrinfo.VBRBytes = ((int)array[10] << 24 | (int)array[11] << 16 | (int)array[12] << 8 | (int)array[13]);
			vbrinfo.VBRFrames = ((int)array[14] << 24 | (int)array[15] << 16 | (int)array[16] << 8 | (int)array[17]);
			int num = (int)array[18] << 8 | (int)array[19];
			byte b3 = array[20];
			byte b4 = array[21];
			int num2 = (int)array[22] << 8 | (int)array[23];
			byte b5 = array[24];
			byte b6 = array[25];
			int num3 = num * num2;
			byte[] buffer = new byte[num3];
			if (base.Read(62, buffer) != num3)
			{
				return null;
			}
			return vbrinfo;
		}

		public int FrameLength
		{
			get
			{
				return base.Length;
			}
		}

		public MpegVersion Version
		{
			get
			{
				switch (this._syncBits >> 19 & 3)
				{
				case 0:
					return MpegVersion.Version25;
				case 2:
					return MpegVersion.Version2;
				case 3:
					return MpegVersion.Version1;
				}
				return MpegVersion.Unknown;
			}
		}

		public MpegLayer Layer
		{
			get
			{
				return ((MpegLayer)4 - (this._syncBits >> 17 & 3)) % (MpegLayer)4;
			}
		}

		public bool HasCrc
		{
			get
			{
				return (this._syncBits & 65536) == 0;
			}
		}

		public int BitRate
		{
			get
			{
				if (this.BitRateIndex > 0)
				{
					return MpegFrame._bitRateTable[this.Version / MpegVersion.Version1 - (MpegVersion)1][this.Layer - MpegLayer.LayerI][this.BitRateIndex] * 1000;
				}
				return (this.FrameLength * 8 * this.SampleRate / this.SampleCount + 499 + 500) / 1000 * 1000;
			}
		}

		public int BitRateIndex
		{
			get
			{
				return this._syncBits >> 12 & 15;
			}
		}

		public int SampleRate
		{
			get
			{
				int num;
				switch (this.SampleRateIndex)
				{
				case 0:
					num = 44100;
					break;
				case 1:
					num = 48000;
					break;
				case 2:
					num = 32000;
					break;
				default:
					num = 0;
					break;
				}
				if (this.Version > MpegVersion.Version1)
				{
					if (this.Version == MpegVersion.Version25)
					{
						num /= 4;
					}
					else
					{
						num /= 2;
					}
				}
				return num;
			}
		}

		public int SampleRateIndex
		{
			get
			{
				return this._syncBits >> 10 & 3;
			}
		}

		private int Padding
		{
			get
			{
				return this._syncBits >> 9 & 1;
			}
		}

		public MpegChannelMode ChannelMode
		{
			get
			{
				return (MpegChannelMode)(this._syncBits >> 6 & 3);
			}
		}

		public int ChannelModeExtension
		{
			get
			{
				return this._syncBits >> 4 & 3;
			}
		}

		internal int Channels
		{
			get
			{
				if (this.ChannelMode != MpegChannelMode.Mono)
				{
					return 2;
				}
				return 1;
			}
		}

		public bool IsCopyrighted
		{
			get
			{
				return (this._syncBits & 8) == 8;
			}
		}

		internal bool IsOriginal
		{
			get
			{
				return (this._syncBits & 4) == 4;
			}
		}

		internal int EmphasisMode
		{
			get
			{
				return this._syncBits & 3;
			}
		}

		public bool IsCorrupted
		{
			get
			{
				return this._isMuted;
			}
		}

		public int SampleCount
		{
			get
			{
				if (this.Layer == MpegLayer.LayerI)
				{
					return 384;
				}
				if (this.Layer == MpegLayer.LayerIII && this.Version > MpegVersion.Version1)
				{
					return 576;
				}
				return 1152;
			}
		}

		internal long SampleOffset
		{
			get
			{
				return this._offset;
			}
			set
			{
				this._offset = value;
			}
		}

		public void Reset()
		{
			this._readOffset = 4 + (this.HasCrc ? 2 : 0);
			this._bitBucket = 0UL;
			this._bitsRead = 0;
		}

		public int ReadBits(int bitCount)
		{
			if (bitCount < 1 || bitCount > 32)
			{
				throw new ArgumentOutOfRangeException("bitCount");
			}
			if (this._isMuted)
			{
				return 0;
			}
			while (this._bitsRead < bitCount)
			{
				int num = base.ReadByte(this._readOffset);
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

		internal MpegFrame Next;

		internal int Number;

		private int _syncBits;

		private int _readOffset;

		private int _bitsRead;

		private ulong _bitBucket;

		private long _offset;

		private bool _isMuted;
	}
}
