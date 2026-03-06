using System;
using System.IO;
using System.Text;

namespace SouthPointe.Serialization.MessagePack
{
	public class FormatReader
	{
		internal long Position
		{
			get
			{
				return this.stream.Position;
			}
		}

		public FormatReader(Stream stream)
		{
			this.stream = stream;
		}

		public Format ReadFormat()
		{
			int num = this.stream.ReadByte();
			if (num >= 0)
			{
				return new Format((byte)num);
			}
			throw new FormatException("There is nothing more to read");
		}

		public byte ReadPositiveFixInt(Format format)
		{
			return format & 127;
		}

		public byte ReadUInt8()
		{
			return (byte)this.stream.ReadByte();
		}

		public ushort ReadUInt16()
		{
			if (this.stream.Read(this.buffer, 0, 2) == 2)
			{
				return (ushort)((int)this.buffer[0] << 8 | (int)this.buffer[1]);
			}
			throw new FormatException();
		}

		public uint ReadUInt32()
		{
			if (this.stream.Read(this.buffer, 0, 4) == 4)
			{
				return (uint)((int)this.buffer[0] << 24 | (int)this.buffer[1] << 16 | (int)this.buffer[2] << 8 | (int)this.buffer[3]);
			}
			throw new FormatException();
		}

		public ulong ReadUInt64()
		{
			if (this.stream.Read(this.buffer, 0, 8) == 8)
			{
				return (ulong)this.buffer[0] << 56 | (ulong)this.buffer[1] << 48 | (ulong)this.buffer[2] << 40 | (ulong)this.buffer[3] << 32 | (ulong)this.buffer[4] << 24 | (ulong)this.buffer[5] << 16 | (ulong)this.buffer[6] << 8 | (ulong)this.buffer[7];
			}
			throw new FormatException();
		}

		public sbyte ReadNegativeFixInt(Format format)
		{
			return (sbyte)((format & 31) - 32);
		}

		public sbyte ReadInt8()
		{
			return (sbyte)this.stream.ReadByte();
		}

		public short ReadInt16()
		{
			if (this.stream.Read(this.buffer, 0, 2) == 2)
			{
				return (short)((int)this.buffer[0] << 8 | (int)this.buffer[1]);
			}
			throw new FormatException();
		}

		public int ReadInt32()
		{
			if (this.stream.Read(this.buffer, 0, 4) == 4)
			{
				return (int)this.buffer[0] << 24 | (int)this.buffer[1] << 16 | (int)this.buffer[2] << 8 | (int)this.buffer[3];
			}
			throw new FormatException();
		}

		public long ReadInt64()
		{
			if (this.stream.Read(this.buffer, 0, 8) == 8)
			{
				return (long)((ulong)this.buffer[0] << 56 | (ulong)this.buffer[1] << 48 | (ulong)this.buffer[2] << 40 | (ulong)this.buffer[3] << 32 | (ulong)this.buffer[4] << 24 | (ulong)this.buffer[5] << 16 | (ulong)this.buffer[6] << 8 | (ulong)this.buffer[7]);
			}
			throw new FormatException();
		}

		public float ReadFloat32()
		{
			if (this.stream.Read(this.buffer, 0, 4) == 4)
			{
				return Float32Bits.ToSingle(this.buffer);
			}
			throw new FormatException();
		}

		public double ReadFloat64()
		{
			if (this.stream.Read(this.buffer, 0, 8) == 8)
			{
				return Float64Bits.ToDouble(this.buffer);
			}
			throw new FormatException();
		}

		public string ReadFixStr(Format format)
		{
			return this.ReadStringOfLength((int)(format & 31));
		}

		public string ReadStr8()
		{
			return this.ReadStringOfLength((int)this.ReadUInt8());
		}

		public string ReadStr16()
		{
			return this.ReadStringOfLength((int)this.ReadUInt16());
		}

		public string ReadStr32()
		{
			return this.ReadStringOfLength(Convert.ToInt32(this.ReadUInt32()));
		}

		public byte[] ReadBin8()
		{
			return this.ReadBytesOfLength((int)this.ReadUInt8());
		}

		public byte[] ReadBin16()
		{
			return this.ReadBytesOfLength((int)this.ReadUInt16());
		}

		public byte[] ReadBin32()
		{
			return this.ReadBytesOfLength(Convert.ToInt32(this.ReadUInt32()));
		}

		public int ReadArrayLength(Format format)
		{
			if (format.IsNil)
			{
				return 0;
			}
			if (format.IsFixArray)
			{
				return (int)(format & 15);
			}
			if (format.IsArray16)
			{
				return (int)this.ReadUInt16();
			}
			if (format.IsArray32)
			{
				return Convert.ToInt32(this.ReadUInt32());
			}
			throw new FormatException();
		}

		public int ReadMapLength(Format format)
		{
			if (format.IsFixMap)
			{
				return (int)(format & 15);
			}
			if (format.IsMap16)
			{
				return (int)this.ReadUInt16();
			}
			if (format.IsMap32)
			{
				return Convert.ToInt32(this.ReadUInt32());
			}
			throw new FormatException();
		}

		public uint ReadExtLength(Format format)
		{
			if (format.IsFixExt1)
			{
				return 1U;
			}
			if (format.IsFixExt2)
			{
				return 2U;
			}
			if (format.IsFixExt4)
			{
				return 4U;
			}
			if (format.IsFixExt8)
			{
				return 8U;
			}
			if (format.IsFixExt16)
			{
				return 16U;
			}
			if (format.IsExt8)
			{
				return (uint)this.ReadUInt8();
			}
			if (format.IsExt16)
			{
				return (uint)this.ReadUInt16();
			}
			if (format.IsExt32)
			{
				return this.ReadUInt32();
			}
			throw new FormatException();
		}

		public sbyte ReadExtType(Format format)
		{
			if (format.IsPositiveFixInt)
			{
				return (sbyte)this.ReadPositiveFixInt(format);
			}
			if (format.IsUInt8)
			{
				return Convert.ToSByte(this.ReadUInt8());
			}
			if (format.IsNegativeFixInt)
			{
				return this.ReadNegativeFixInt(format);
			}
			if (format.IsInt8)
			{
				return this.ReadInt8();
			}
			throw new FormatException();
		}

		public void Skip()
		{
			Format format = this.ReadFormat();
			if (format.IsNil)
			{
				return;
			}
			if (format.IsFalse)
			{
				return;
			}
			if (format.IsTrue)
			{
				return;
			}
			if (format.IsPositiveFixInt)
			{
				return;
			}
			if (format.IsNegativeFixInt)
			{
				return;
			}
			if (format.IsUInt8 || format.IsInt8)
			{
				this.FastForward(1L);
				return;
			}
			if (format.IsUInt16 || format.IsInt16)
			{
				this.FastForward(2L);
				return;
			}
			if (format.IsUInt32 || format.IsInt32)
			{
				this.FastForward(4L);
				return;
			}
			if (format.IsUInt64 || format.IsInt64)
			{
				this.FastForward(8L);
				return;
			}
			if (format.IsFloat32)
			{
				this.FastForward(4L);
				return;
			}
			if (format.IsFloat64)
			{
				this.FastForward(8L);
				return;
			}
			if (format.IsFixStr)
			{
				this.FastForward((long)((ulong)(format & 31)));
				return;
			}
			if (format.IsStr8)
			{
				this.FastForward((long)((ulong)this.ReadUInt8()));
				return;
			}
			if (format.IsStr16)
			{
				this.FastForward((long)((ulong)this.ReadUInt16()));
				return;
			}
			if (format.IsStr32)
			{
				this.FastForward((long)((ulong)this.ReadUInt32()));
				return;
			}
			if (format.IsBin8)
			{
				this.FastForward((long)((ulong)this.ReadUInt8()));
				return;
			}
			if (format.IsBin16)
			{
				this.FastForward((long)((ulong)this.ReadUInt16()));
				return;
			}
			if (format.IsBin32)
			{
				this.FastForward((long)((ulong)this.ReadUInt32()));
				return;
			}
			if (format.IsArrayFamily)
			{
				for (int i = this.ReadArrayLength(format); i > 0; i--)
				{
					this.Skip();
				}
				return;
			}
			if (format.IsMapFamily)
			{
				for (int j = this.ReadMapLength(format); j > 0; j--)
				{
					this.Skip();
					this.Skip();
				}
				return;
			}
			if (format.IsFixExt1)
			{
				this.FastForward(2L);
				return;
			}
			if (format.IsFixExt2)
			{
				this.FastForward(3L);
				return;
			}
			if (format.IsFixExt4)
			{
				this.FastForward(5L);
				return;
			}
			if (format.IsFixExt8)
			{
				this.FastForward(9L);
				return;
			}
			if (format.IsFixExt16)
			{
				this.FastForward(17L);
				return;
			}
			if (format.IsExt8)
			{
				this.FastForward((long)(this.ReadUInt8() + 1));
				return;
			}
			if (format.IsExt16)
			{
				this.FastForward((long)(this.ReadUInt16() + 1));
				return;
			}
			if (format.IsExt32)
			{
				this.FastForward((long)((ulong)(this.ReadUInt32() + 1U)));
				return;
			}
		}

		private void FastForward(long offset)
		{
			if (this.stream.CanSeek)
			{
				this.stream.Seek(offset, SeekOrigin.Current);
				return;
			}
			while (offset > 0L)
			{
				int num = (offset > 2147483647L) ? int.MaxValue : ((int)offset);
				ArrayHelper.AdjustSize(ref this.buffer, num);
				this.stream.Read(this.buffer, 0, num);
				offset -= 2147483647L;
			}
		}

		private string ReadStringOfLength(int length)
		{
			ArrayHelper.AdjustSize(ref this.buffer, length);
			this.stream.Read(this.buffer, 0, length);
			return Encoding.UTF8.GetString(this.buffer, 0, length);
		}

		internal byte[] ReadBytesOfLength(int length)
		{
			byte[] result = new byte[length];
			this.stream.Read(result, 0, length);
			return result;
		}

		private readonly Stream stream;

		private byte[] buffer = new byte[64];
	}
}
