using System;
using System.IO;
using System.Text;

namespace SouthPointe.Serialization.MessagePack
{
	public class FormatWriter
	{
		public FormatWriter(Stream stream)
		{
			this.stream = stream;
		}

		public void WriteFormat(byte formatValue)
		{
			this.stream.WriteByte(formatValue);
		}

		public void WriteNil()
		{
			this.stream.WriteByte(192);
		}

		public void Write(bool value)
		{
			this.stream.WriteByte(value ? 195 : 194);
		}

		public void Write(byte value)
		{
			if (value <= 127)
			{
				this.WritePositiveFixInt(value);
				return;
			}
			this.WriteFormat(204);
			this.WriteUInt8(value);
		}

		public void Write(ushort value)
		{
			if (value <= 255)
			{
				this.Write((byte)value);
				return;
			}
			this.WriteFormat(205);
			this.WriteUInt16(value);
		}

		public void Write(uint value)
		{
			if (value <= 65535U)
			{
				this.Write((ushort)value);
				return;
			}
			this.WriteFormat(206);
			this.WriteUInt32(value);
		}

		public void Write(ulong value)
		{
			if (value <= (ulong)-1)
			{
				this.Write((uint)value);
				return;
			}
			this.WriteFormat(207);
			this.WriteUInt64(value);
		}

		public void Write(sbyte value)
		{
			if (value >= 0)
			{
				this.Write((byte)value);
				return;
			}
			if (value >= -32)
			{
				this.WriteNegativeFixInt(value);
				return;
			}
			this.WriteFormat(208);
			this.WriteInt8(value);
		}

		public void Write(short value)
		{
			if (value >= 0)
			{
				this.Write((ushort)value);
				return;
			}
			if (value >= -128)
			{
				this.Write((sbyte)value);
				return;
			}
			this.WriteFormat(209);
			this.WriteInt16(value);
		}

		public void Write(int value)
		{
			if (value >= 0)
			{
				this.Write((uint)value);
				return;
			}
			if (value >= -32768)
			{
				this.Write((short)value);
				return;
			}
			this.WriteFormat(210);
			this.WriteInt32(value);
		}

		public void Write(long value)
		{
			if (value >= 0L)
			{
				this.Write((ulong)value);
				return;
			}
			if (value >= -2147483648L)
			{
				this.Write((int)value);
				return;
			}
			this.WriteFormat(211);
			this.WriteInt64(value);
		}

		public void Write(float value)
		{
			this.WriteFormat(202);
			Float32Bits.GetBytes(value, this.buffer);
			this.stream.Write(this.buffer, 0, 4);
		}

		public void Write(double value)
		{
			this.WriteFormat(203);
			Float64Bits.GetBytes(value, this.buffer);
			this.stream.Write(this.buffer, 0, 8);
		}

		public void Write(string value)
		{
			if (value == null)
			{
				this.WriteNil();
				return;
			}
			int byteCount = Encoding.UTF8.GetByteCount(value);
			if (byteCount <= 31)
			{
				this.WriteFormat(160 | (byte)byteCount);
			}
			else if (byteCount <= 255)
			{
				this.WriteFormat(217);
				this.WriteUInt8((byte)byteCount);
			}
			else if (byteCount <= 65535)
			{
				this.WriteFormat(218);
				this.WriteUInt16((ushort)byteCount);
			}
			else
			{
				this.WriteFormat(219);
				this.WriteUInt32((uint)byteCount);
			}
			ArrayHelper.AdjustSize(ref this.buffer, byteCount);
			Encoding.UTF8.GetBytes(value, 0, value.Length, this.buffer, 0);
			this.stream.Write(this.buffer, 0, byteCount);
		}

		public void Write(byte[] bytes)
		{
			if (bytes == null)
			{
				this.WriteNil();
				return;
			}
			if (bytes.Length <= 255)
			{
				this.WriteFormat(196);
				this.WriteUInt8((byte)bytes.Length);
			}
			else if (bytes.Length <= 65535)
			{
				this.WriteFormat(197);
				this.WriteUInt16((ushort)bytes.Length);
			}
			else
			{
				this.WriteFormat(198);
				this.WriteUInt32((uint)bytes.Length);
			}
			this.stream.Write(bytes, 0, bytes.Length);
		}

		public void WriteArrayHeader(int length)
		{
			if (length <= 15)
			{
				this.WriteFormat((byte)(length | 144));
				return;
			}
			if (length <= 65535)
			{
				this.WriteFormat(220);
				this.WriteUInt16((ushort)length);
				return;
			}
			this.WriteFormat(221);
			this.WriteUInt32((uint)length);
		}

		public void WriteBinHeader(int length)
		{
			if (length <= 255)
			{
				this.WriteFormat(196);
				this.WriteUInt8((byte)length);
				return;
			}
			if (length <= 65535)
			{
				this.WriteFormat(197);
				this.WriteUInt16((ushort)length);
				return;
			}
			this.WriteFormat(198);
			this.WriteUInt32((uint)length);
		}

		public void WriteMapHeader(int length)
		{
			if (length <= 15)
			{
				this.WriteFormat((byte)(length | 128));
				return;
			}
			if (length <= 65535)
			{
				this.WriteFormat(222);
				this.WriteUInt16((ushort)length);
				return;
			}
			this.WriteFormat(223);
			this.WriteUInt32((uint)length);
		}

		public void WriteExtHeader(uint length, sbyte extType)
		{
			if (length == 1U)
			{
				this.WriteFormat(212);
			}
			else if (length == 2U)
			{
				this.WriteFormat(213);
			}
			else if (length == 4U)
			{
				this.WriteFormat(214);
			}
			else if (length == 8U)
			{
				this.WriteFormat(215);
			}
			else if (length == 16U)
			{
				this.WriteFormat(216);
			}
			else if (length <= 255U)
			{
				this.WriteFormat(199);
				this.WriteUInt8((byte)length);
			}
			else if (length <= 65535U)
			{
				this.WriteFormat(200);
				this.WriteUInt16((ushort)length);
			}
			else
			{
				if (length > 4294967295U)
				{
					throw new FormatException();
				}
				this.WriteFormat(201);
				this.WriteUInt32(length);
			}
			this.stream.WriteByte((byte)extType);
		}

		public void WritePositiveFixInt(byte value)
		{
			if (value >= 0 || value <= 127)
			{
				this.stream.WriteByte(value | 0);
				return;
			}
			throw new FormatException(value.ToString() + " is out of range for PositiveFixInt");
		}

		public void WriteUInt8(byte value)
		{
			this.stream.WriteByte(value);
		}

		public void WriteUInt16(ushort value)
		{
			this.buffer[0] = (byte)(value >> 8);
			this.buffer[1] = (byte)value;
			this.stream.Write(this.buffer, 0, 2);
		}

		public void WriteUInt32(uint value)
		{
			this.buffer[0] = (byte)(value >> 24);
			this.buffer[1] = (byte)(value >> 16);
			this.buffer[2] = (byte)(value >> 8);
			this.buffer[3] = (byte)value;
			this.stream.Write(this.buffer, 0, 4);
		}

		public void WriteUInt64(ulong value)
		{
			this.buffer[0] = (byte)(value >> 56);
			this.buffer[1] = (byte)(value >> 48);
			this.buffer[2] = (byte)(value >> 40);
			this.buffer[3] = (byte)(value >> 32);
			this.buffer[4] = (byte)(value >> 24);
			this.buffer[5] = (byte)(value >> 16);
			this.buffer[6] = (byte)(value >> 8);
			this.buffer[7] = (byte)value;
			this.stream.Write(this.buffer, 0, 8);
		}

		public void WriteNegativeFixInt(sbyte value)
		{
			if (value >= -32 && value <= -1)
			{
				this.stream.WriteByte((byte)value | 224);
				return;
			}
			throw new FormatException(value.ToString() + " is out of range for NegativeFixInt");
		}

		public void WriteInt8(sbyte value)
		{
			this.stream.WriteByte((byte)value);
		}

		public void WriteInt16(short value)
		{
			this.buffer[0] = (byte)(value >> 8);
			this.buffer[1] = (byte)value;
			this.stream.Write(this.buffer, 0, 2);
		}

		public void WriteInt32(int value)
		{
			this.buffer[0] = (byte)(value >> 24);
			this.buffer[1] = (byte)(value >> 16);
			this.buffer[2] = (byte)(value >> 8);
			this.buffer[3] = (byte)value;
			this.stream.Write(this.buffer, 0, 4);
		}

		public void WriteInt64(long value)
		{
			this.buffer[0] = (byte)(value >> 56);
			this.buffer[1] = (byte)(value >> 48);
			this.buffer[2] = (byte)(value >> 40);
			this.buffer[3] = (byte)(value >> 32);
			this.buffer[4] = (byte)(value >> 24);
			this.buffer[5] = (byte)(value >> 16);
			this.buffer[6] = (byte)(value >> 8);
			this.buffer[7] = (byte)value;
			this.stream.Write(this.buffer, 0, 8);
		}

		public void WriteRawByte(byte value)
		{
			this.stream.WriteByte(value);
		}

		private readonly Stream stream;

		private byte[] buffer = new byte[64];
	}
}
