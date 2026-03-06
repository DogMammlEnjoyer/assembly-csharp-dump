using System;
using System.Text;

namespace Fusion.Protocol
{
	public class BitStream
	{
		public int Size
		{
			get
			{
				return this._size;
			}
			set
			{
				this._size = Maths.Clamp(value, 0, this._data.Length << 3);
			}
		}

		public int Position
		{
			get
			{
				return this._ptr;
			}
			set
			{
				this._ptr = Maths.Clamp(value, 0, this._size);
			}
		}

		public int BytesRequired
		{
			get
			{
				return Maths.BytesRequiredForBits(this.Position);
			}
		}

		public bool IsEvenBytes
		{
			get
			{
				return this._ptr % 8 == 0;
			}
		}

		public int Capacity
		{
			get
			{
				return this._data.Length;
			}
		}

		public bool Done
		{
			get
			{
				return this._ptr == this._size;
			}
		}

		public bool Overflowing
		{
			get
			{
				return this._ptr > this._size;
			}
		}

		public bool Writing
		{
			get
			{
				return this._write;
			}
			set
			{
				this._write = value;
			}
		}

		public bool Reading
		{
			get
			{
				return !this._write;
			}
			set
			{
				this._write = !value;
			}
		}

		public byte[] Data
		{
			get
			{
				return this._data;
			}
		}

		public BitStream() : this(new byte[0])
		{
		}

		public BitStream(int size) : this(new byte[size])
		{
		}

		public BitStream(byte[] arr) : this(arr, arr.Length)
		{
		}

		public BitStream(byte[] arr, int size)
		{
			this._ptr = 0;
			this._data = arr;
			this._size = size << 3;
		}

		public void SetBuffer(byte[] arr)
		{
			this.SetBuffer(arr, arr.Length);
		}

		public void SetBuffer(byte[] arr, int size)
		{
			this._ptr = 0;
			this._data = arr;
			this._size = size << 3;
		}

		public int RoundToByte()
		{
			int num = this._ptr % 8;
			bool flag = num > 0;
			if (flag)
			{
				int num2 = 8 - num;
				bool write = this._write;
				if (write)
				{
					this.WriteByte(0, num2);
				}
				else
				{
					this._ptr += num2;
				}
			}
			Assert.Check(this._ptr % 8 == 0);
			return this._ptr / 8;
		}

		public void Expand()
		{
			byte[] array = new byte[this._data.Length * 2];
			Buffer.BlockCopy(this._data, 0, array, 0, this._data.Length);
			this._data = array;
			this._size = array.Length << 3;
		}

		public bool CanWrite()
		{
			return this.CanWrite(1);
		}

		public bool CanRead()
		{
			return this.CanRead(1);
		}

		public bool CanWrite(int bits)
		{
			return this._ptr + bits <= this._size;
		}

		public bool CanRead(int bits)
		{
			return this._ptr + bits <= this._size;
		}

		public void CopyFromArray(byte[] array)
		{
			Assert.Check(array.Length <= this._data.Length);
			Array.Copy(array, 0, this._data, 0, array.Length);
			this._ptr = 0;
			this._size = array.Length << 3;
		}

		public void Reset()
		{
			this.Reset(this._data.Length);
		}

		public void Reset(int byteSize)
		{
			Assert.Check(byteSize <= this._data.Length);
			Array.Clear(this._data, 0, this._data.Length);
			this._ptr = 0;
			this._size = byteSize << 3;
		}

		public void ResetFast(int byteSize)
		{
			Assert.Check(byteSize <= this._data.Length);
			this._ptr = 0;
			this._size = byteSize << 3;
		}

		public byte[] ToArray()
		{
			byte[] array = new byte[Maths.BytesRequiredForBits(this._ptr)];
			Buffer.BlockCopy(this._data, 0, array, 0, array.Length);
			return array;
		}

		public bool WriteBool(bool value)
		{
			this.InternalWriteByte(value ? 1 : 0, 1);
			return value;
		}

		public bool WriteBoolean(bool value)
		{
			this.InternalWriteByte(value ? 1 : 0, 1);
			return value;
		}

		public bool ReadBool()
		{
			return this.InternalReadByte(1) == 1;
		}

		public bool ReadBoolean()
		{
			return this.InternalReadByte(1) == 1;
		}

		public void WriteByte(byte value, int bits)
		{
			this.InternalWriteByte(value, bits);
		}

		public byte ReadByte(int bits)
		{
			return this.InternalReadByte(bits);
		}

		public void WriteByte(byte value)
		{
			this.WriteByte(value, 8);
		}

		public byte ReadByte()
		{
			return this.ReadByte(8);
		}

		public sbyte ReadSByte()
		{
			return (sbyte)this.ReadByte();
		}

		public void WriteSByte(sbyte value)
		{
			this.WriteByte((byte)value);
		}

		public void WriteUShort(ushort value, int bits)
		{
			bool flag = bits <= 8;
			if (flag)
			{
				this.InternalWriteByte((byte)(value & 255), bits);
			}
			else
			{
				this.InternalWriteByte((byte)(value & 255), 8);
				this.InternalWriteByte((byte)(value >> 8), bits - 8);
			}
		}

		public ushort ReadUShort(int bits)
		{
			bool flag = bits <= 8;
			ushort result;
			if (flag)
			{
				result = (ushort)this.InternalReadByte(bits);
			}
			else
			{
				result = (ushort)((int)this.InternalReadByte(8) | (int)this.InternalReadByte(bits - 8) << 8);
			}
			return result;
		}

		public void WriteUShort(ushort value)
		{
			this.WriteUShort(value, 16);
		}

		public ushort ReadUShort()
		{
			return this.ReadUShort(16);
		}

		public void WriteShort(short value, int bits)
		{
			this.WriteUShort((ushort)value, bits);
		}

		public short ReadShort(int bits)
		{
			return (short)this.ReadUShort(bits);
		}

		public void WriteShort(short value)
		{
			this.WriteShort(value, 16);
		}

		public short ReadShort()
		{
			return this.ReadShort(16);
		}

		public void WriteChar(char value)
		{
			this.WriteUShort((ushort)value, 16);
		}

		public char ReadChar()
		{
			return (char)this.ReadUShort(16);
		}

		public void WriteUInt(uint value, int bits)
		{
			byte value2 = (byte)value;
			byte value3 = (byte)(value >> 8);
			byte value4 = (byte)(value >> 16);
			byte value5 = (byte)(value >> 24);
			switch ((bits + 7) / 8)
			{
			case 1:
				this.InternalWriteByte(value2, bits);
				break;
			case 2:
				this.InternalWriteByte(value2, 8);
				this.InternalWriteByte(value3, bits - 8);
				break;
			case 3:
				this.InternalWriteByte(value2, 8);
				this.InternalWriteByte(value3, 8);
				this.InternalWriteByte(value4, bits - 16);
				break;
			case 4:
				this.InternalWriteByte(value2, 8);
				this.InternalWriteByte(value3, 8);
				this.InternalWriteByte(value4, 8);
				this.InternalWriteByte(value5, bits - 24);
				break;
			}
		}

		public uint ReadUInt(int bits)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			switch ((bits + 7) / 8)
			{
			case 1:
				num = (int)this.InternalReadByte(bits);
				break;
			case 2:
				num = (int)this.InternalReadByte(8);
				num2 = (int)this.InternalReadByte(bits - 8);
				break;
			case 3:
				num = (int)this.InternalReadByte(8);
				num2 = (int)this.InternalReadByte(8);
				num3 = (int)this.InternalReadByte(bits - 16);
				break;
			case 4:
				num = (int)this.InternalReadByte(8);
				num2 = (int)this.InternalReadByte(8);
				num3 = (int)this.InternalReadByte(8);
				num4 = (int)this.InternalReadByte(bits - 24);
				break;
			}
			return (uint)(num | num2 << 8 | num3 << 16 | num4 << 24);
		}

		public void WriteUInt(uint value)
		{
			this.WriteUInt(value, 32);
		}

		public uint ReadUInt()
		{
			return this.ReadUInt(32);
		}

		public void WriteInt_Shifted(int value, int bits, int shift)
		{
			this.WriteInt(value, 32);
		}

		public int ReadInt_Shifted(int bits, int shift)
		{
			return this.ReadInt(32);
		}

		public void WriteInt(int value, int bits)
		{
			this.WriteUInt((uint)value, bits);
		}

		public int ReadInt(int bits)
		{
			return (int)this.ReadUInt(bits);
		}

		public void WriteInt(int value)
		{
			this.WriteInt(value, 32);
		}

		public int ReadInt()
		{
			return this.ReadInt(32);
		}

		public void WriteULong(ulong value, int bits)
		{
			bool flag = bits <= 32;
			if (flag)
			{
				this.WriteUInt((uint)(value & (ulong)-1), bits);
			}
			else
			{
				this.WriteUInt((uint)value, 32);
				this.WriteUInt((uint)(value >> 32), bits - 32);
			}
		}

		public ulong ReadULong(int bits)
		{
			bool flag = bits <= 32;
			ulong result;
			if (flag)
			{
				result = (ulong)this.ReadUInt(bits);
			}
			else
			{
				ulong num = (ulong)this.ReadUInt(32);
				ulong num2 = (ulong)this.ReadUInt(bits - 32);
				result = (num | num2 << 32);
			}
			return result;
		}

		public void WriteULong(ulong value)
		{
			this.WriteULong(value, 64);
		}

		public ulong ReadULong()
		{
			return this.ReadULong(64);
		}

		public void WriteLong(long value, int bits)
		{
			this.WriteULong((ulong)value, bits);
		}

		public long ReadLong(int bits)
		{
			return (long)this.ReadULong(bits);
		}

		public void WriteLong(long value)
		{
			this.WriteLong(value, 64);
		}

		public long ReadLong()
		{
			return this.ReadLong(64);
		}

		public void WriteFloat(float value)
		{
			UdpByteConverter udpByteConverter = value;
			this.InternalWriteByte(udpByteConverter.Byte0, 8);
			this.InternalWriteByte(udpByteConverter.Byte1, 8);
			this.InternalWriteByte(udpByteConverter.Byte2, 8);
			this.InternalWriteByte(udpByteConverter.Byte3, 8);
		}

		public float ReadFloat()
		{
			return new UdpByteConverter
			{
				Byte0 = this.InternalReadByte(8),
				Byte1 = this.InternalReadByte(8),
				Byte2 = this.InternalReadByte(8),
				Byte3 = this.InternalReadByte(8)
			}.Float32;
		}

		public void WriteDouble(double value)
		{
			UdpByteConverter udpByteConverter = value;
			this.InternalWriteByte(udpByteConverter.Byte0, 8);
			this.InternalWriteByte(udpByteConverter.Byte1, 8);
			this.InternalWriteByte(udpByteConverter.Byte2, 8);
			this.InternalWriteByte(udpByteConverter.Byte3, 8);
			this.InternalWriteByte(udpByteConverter.Byte4, 8);
			this.InternalWriteByte(udpByteConverter.Byte5, 8);
			this.InternalWriteByte(udpByteConverter.Byte6, 8);
			this.InternalWriteByte(udpByteConverter.Byte7, 8);
		}

		public double ReadDouble()
		{
			return new UdpByteConverter
			{
				Byte0 = this.InternalReadByte(8),
				Byte1 = this.InternalReadByte(8),
				Byte2 = this.InternalReadByte(8),
				Byte3 = this.InternalReadByte(8),
				Byte4 = this.InternalReadByte(8),
				Byte5 = this.InternalReadByte(8),
				Byte6 = this.InternalReadByte(8),
				Byte7 = this.InternalReadByte(8)
			}.Float64;
		}

		public void WriteByteArray(byte[] from)
		{
			this.WriteByteArray(from, 0, from.Length);
		}

		public void WriteByteArray(byte[] from, int count)
		{
			this.WriteByteArray(from, 0, count);
		}

		public void WriteByteArray(byte[] from, int offset, int count)
		{
			int num = this._ptr >> 3;
			int num2 = this._ptr % 8;
			int num3 = 8 - num2;
			bool flag = num2 == 0;
			if (flag)
			{
				Buffer.BlockCopy(from, offset, this._data, num, count);
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					byte b = from[offset + i];
					byte[] data = this._data;
					int num4 = num;
					data[num4] &= (byte)(255 >> num3);
					byte[] data2 = this._data;
					int num5 = num;
					data2[num5] |= (byte)(b << num2);
					num++;
					byte[] data3 = this._data;
					int num6 = num;
					data3[num6] &= (byte)(255 << num2);
					byte[] data4 = this._data;
					int num7 = num;
					data4[num7] |= (byte)(b >> num3);
				}
			}
			this._ptr += count * 8;
		}

		public byte[] ReadByteArray(int size)
		{
			byte[] array = new byte[size];
			this.ReadByteArray(array);
			return array;
		}

		public void ReadByteArray(byte[] to)
		{
			this.ReadByteArray(to, 0, to.Length);
		}

		public void ReadByteArray(byte[] to, int count)
		{
			this.ReadByteArray(to, 0, count);
		}

		public void ReadByteArray(byte[] to, int offset, int count)
		{
			int num = this._ptr >> 3;
			int num2 = this._ptr % 8;
			bool flag = num2 == 0;
			if (flag)
			{
				Buffer.BlockCopy(this._data, num, to, offset, count);
			}
			else
			{
				int num3 = 8 - num2;
				for (int i = 0; i < count; i++)
				{
					int num4 = this._data[num] >> num2;
					num++;
					int num5 = (int)this._data[num] & 255 >> num3;
					to[offset + i] = (byte)(num4 | num5 << num3);
				}
			}
			this._ptr += count * 8;
		}

		public void WriteByteArrayLengthPrefixed(byte[] array)
		{
			this.WriteByteArrayLengthPrefixed(array, (array == null) ? 0 : array.Length);
		}

		public void WriteByteArrayLengthPrefixed(byte[] array, int maxLength)
		{
			bool flag = this.WriteBool(array != null);
			if (flag)
			{
				int num = Math.Min(array.Length, maxLength);
				bool flag2 = num < array.Length;
				if (flag2)
				{
					LogStream logWarn = InternalLogStreams.LogWarn;
					if (logWarn != null)
					{
						logWarn.Log(string.Format("Only sendig {0}/{1} bytes from byte array", num, array.Length));
					}
				}
				this.WriteUShort((ushort)num);
				this.WriteByteArray(array, 0, num);
			}
		}

		public byte[] ReadByteArrayLengthPrefixed()
		{
			bool flag = this.ReadBool();
			byte[] result;
			if (flag)
			{
				byte[] array = new byte[(int)this.ReadUShort()];
				this.ReadByteArray(array, 0, array.Length);
				result = array;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void WriteString(string value, Encoding encoding)
		{
			bool flag = this.WriteBool(value == null);
			if (!flag)
			{
				byte[] bytes = encoding.GetBytes(value);
				this.WriteUShort((ushort)bytes.Length);
				this.WriteByteArray(bytes);
			}
		}

		public void WriteString(string value)
		{
			this.WriteString(value, Encoding.UTF8);
		}

		public string ReadString(Encoding encoding)
		{
			bool flag = this.ReadBool();
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				int num = (int)this.ReadUShort();
				bool flag2 = num == 0;
				if (flag2)
				{
					result = "";
				}
				else
				{
					byte[] array = new byte[num];
					this.ReadByteArray(array);
					result = encoding.GetString(array, 0, array.Length);
				}
			}
			return result;
		}

		public string ReadString()
		{
			return this.ReadString(Encoding.UTF8);
		}

		public void WriteGuid(Guid guid)
		{
			this.WriteByteArray(guid.ToByteArray());
		}

		public Guid ReadGuid()
		{
			byte[] array = new byte[16];
			this.ReadByteArray(array);
			return new Guid(array);
		}

		private void InternalWriteByte(byte value, int bits)
		{
			BitStream.WriteByteAt(this._data, this._ptr, bits, value);
			this._ptr += bits;
		}

		public static void WriteByteAt(byte[] data, int ptr, int bits, byte value)
		{
			bool flag = bits <= 0;
			if (!flag)
			{
				value = (byte)((int)value & 255 >> 8 - bits);
				int num = ptr >> 3;
				int num2 = ptr & 7;
				int num3 = 8 - num2;
				int num4 = num3 - bits;
				bool flag2 = num4 >= 0;
				if (flag2)
				{
					int num5 = 255 >> num3 | 255 << 8 - num4;
					data[num] = (byte)(((int)data[num] & num5) | (int)value << num2);
				}
				else
				{
					data[num] = (byte)(((int)data[num] & 255 >> num3) | (int)value << num2);
					data[num + 1] = (byte)(((int)data[num + 1] & 255 << bits - num3) | value >> num3);
				}
			}
		}

		private byte InternalReadByte(int bits)
		{
			bool flag = bits <= 0;
			byte result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				int num = this._ptr >> 3;
				int num2 = this._ptr % 8;
				bool flag2 = num2 == 0 && bits == 8;
				byte b;
				if (flag2)
				{
					b = this._data[num];
				}
				else
				{
					int num3 = this._data[num] >> num2;
					int num4 = bits - (8 - num2);
					bool flag3 = num4 < 1;
					if (flag3)
					{
						b = (byte)(num3 & 255 >> 8 - bits);
					}
					else
					{
						int num5 = (int)this._data[num + 1] & 255 >> 8 - num4;
						b = (byte)(num3 | num5 << bits - num4);
					}
				}
				this._ptr += bits;
				result = b;
			}
			return result;
		}

		public bool Condition(bool condition)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteBool(condition);
			}
			else
			{
				condition = this.ReadBool();
			}
			return condition;
		}

		public void Serialize(ref string value)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteString(value, Encoding.UTF8);
			}
			else
			{
				value = this.ReadString(Encoding.UTF8);
			}
		}

		public void Serialize(ref bool value)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteBool(value);
			}
			else
			{
				value = this.ReadBool();
			}
		}

		public void Serialize(ref float value)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteFloat(value);
			}
			else
			{
				value = this.ReadFloat();
			}
		}

		public void Serialize(ref double value)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteDouble(value);
			}
			else
			{
				value = this.ReadDouble();
			}
		}

		public void Serialize(ref long value)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteLong(value);
			}
			else
			{
				value = this.ReadLong();
			}
		}

		public void Serialize(ref ulong value)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteULong(value);
			}
			else
			{
				value = this.ReadULong();
			}
		}

		public void Serialize(ref byte value)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteByte(value);
			}
			else
			{
				value = this.ReadByte();
			}
		}

		public void Serialize(ref uint value)
		{
			this.Serialize(ref value, 32);
		}

		public void Serialize(ref uint value, int bits)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteUInt(value, bits);
			}
			else
			{
				value = this.ReadUInt(bits);
			}
		}

		public void Serialize(ref ulong value, int bits)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteULong(value, bits);
			}
			else
			{
				value = this.ReadULong(bits);
			}
		}

		public void Serialize(ref int value)
		{
			this.Serialize(ref value, 32);
		}

		public void Serialize(ref int value, int bits)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteInt(value, bits);
			}
			else
			{
				value = this.ReadInt(bits);
			}
		}

		public void Serialize(ref int[] value)
		{
			bool write = this._write;
			if (write)
			{
				bool flag = this.WriteBool(value != null);
				if (flag)
				{
					this.WriteUShort((ushort)value.Length);
					for (int i = 0; i < value.Length; i++)
					{
						this.WriteInt(value[i]);
					}
				}
			}
			else
			{
				bool flag2 = this.ReadBool();
				if (flag2)
				{
					value = new int[(int)this.ReadUShort()];
					for (int j = 0; j < value.Length; j++)
					{
						value[j] = this.ReadInt();
					}
				}
				else
				{
					value = null;
				}
			}
		}

		public void Serialize(ref byte[] value)
		{
			bool write = this._write;
			if (write)
			{
				byte[] array = value;
				byte[] array2 = value;
				this.WriteByteArrayLengthPrefixed(array, ((array2 != null) ? new int?(array2.Length) : null).GetValueOrDefault(0));
			}
			else
			{
				value = this.ReadByteArrayLengthPrefixed();
			}
		}

		public void Serialize(ref byte[] array, ref int length)
		{
			bool write = this._write;
			if (write)
			{
				bool flag = this.WriteBool(array != null);
				if (flag)
				{
					this.WriteUShort((ushort)length);
					this.WriteByteArray(array, 0, length);
				}
			}
			else
			{
				bool flag2 = this.ReadBool();
				if (flag2)
				{
					length = (int)this.ReadUShort();
					bool flag3 = array == null || array.Length < length;
					if (flag3)
					{
						array = new byte[length];
					}
					this.ReadByteArray(array, 0, length);
				}
			}
		}

		public void Serialize(ref byte[] value, int fixedSize)
		{
			bool write = this._write;
			if (write)
			{
				bool flag = this.WriteBoolean(value != null && value.Length != 0);
				if (flag)
				{
					Assert.Check(value.Length == fixedSize);
					this.WriteByteArray(value, fixedSize);
				}
			}
			else
			{
				bool flag2 = this.ReadBoolean();
				if (flag2)
				{
					value = this.ReadByteArray(fixedSize);
				}
				else
				{
					value = null;
				}
			}
		}

		public void Serialize(ref byte[] array, ref int length, int fixedSize)
		{
			length = fixedSize;
			bool write = this._write;
			if (write)
			{
				bool flag = this.WriteBoolean(array != null && array.Length != 0);
				if (flag)
				{
					Assert.Check(length == fixedSize);
					Assert.Check(array.Length <= fixedSize);
					this.WriteByteArray(array, fixedSize);
				}
			}
			else
			{
				bool flag2 = this.ReadBoolean();
				if (flag2)
				{
					bool flag3 = array == null || array.Length < fixedSize;
					if (flag3)
					{
						array = new byte[fixedSize];
					}
					this.ReadByteArray(array, fixedSize);
				}
				else
				{
					array = null;
				}
			}
		}

		public void SerializeArrayLength<T>(ref T[] array)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteInt((array != null) ? array.Length : 0);
			}
			else
			{
				array = new T[this.ReadInt()];
			}
		}

		public void SerializeArray<T>(ref T[] array, BitStream.ArrayElementSerializer<T> serializer)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteInt((array != null) ? array.Length : 0);
			}
			else
			{
				array = new T[this.ReadInt()];
			}
			bool flag = array != null;
			if (flag)
			{
				for (int i = 0; i < array.Length; i++)
				{
					serializer(ref array[i]);
				}
			}
		}

		public unsafe void Serialize(byte* v)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteByte(*v);
			}
			else
			{
				*v = this.ReadByte();
			}
		}

		public unsafe void Serialize(sbyte* v)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteSByte(*v);
			}
			else
			{
				*v = this.ReadSByte();
			}
		}

		public unsafe void Serialize(short* v)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteShort(*v);
			}
			else
			{
				*v = this.ReadShort();
			}
		}

		public unsafe void Serialize(ushort* v)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteUShort(*v);
			}
			else
			{
				*v = this.ReadUShort();
			}
		}

		public unsafe void Serialize(int* v)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteInt(*v);
			}
			else
			{
				*v = this.ReadInt();
			}
		}

		public unsafe void Serialize(uint* v)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteUInt(*v);
			}
			else
			{
				*v = this.ReadUInt();
			}
		}

		public unsafe void Serialize(long* v)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteLong(*v);
			}
			else
			{
				*v = this.ReadLong();
			}
		}

		public unsafe void Serialize(ulong* v)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteULong(*v);
			}
			else
			{
				*v = this.ReadULong();
			}
		}

		public unsafe void Serialize(uint* v, int bits)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteUInt(*v, bits);
			}
			else
			{
				*v = this.ReadUInt(bits);
			}
		}

		public unsafe void Serialize(int* v, int bits)
		{
			bool write = this._write;
			if (write)
			{
				this.WriteInt(*v, bits);
			}
			else
			{
				*v = this.ReadInt(bits);
			}
		}

		public unsafe void SerializeBuffer(byte* buffer, int length)
		{
			bool write = this._write;
			if (write)
			{
				for (int i = 0; i < length; i++)
				{
					this.WriteByte(buffer[i]);
				}
			}
			else
			{
				for (int j = 0; j < length; j++)
				{
					buffer[j] = this.ReadByte();
				}
			}
		}

		public unsafe void SerializeBuffer(sbyte* buffer, int length)
		{
			bool write = this._write;
			if (write)
			{
				for (int i = 0; i < length; i++)
				{
					this.WriteSByte(buffer[i]);
				}
			}
			else
			{
				for (int j = 0; j < length; j++)
				{
					buffer[j] = this.ReadSByte();
				}
			}
		}

		public unsafe void SerializeBuffer(short* buffer, int length)
		{
			bool write = this._write;
			if (write)
			{
				for (int i = 0; i < length; i++)
				{
					this.WriteShort(buffer[i]);
				}
			}
			else
			{
				for (int j = 0; j < length; j++)
				{
					buffer[j] = this.ReadShort();
				}
			}
		}

		public unsafe void SerializeBuffer(ushort* buffer, int length)
		{
			bool write = this._write;
			if (write)
			{
				for (int i = 0; i < length; i++)
				{
					this.WriteUShort(buffer[i]);
				}
			}
			else
			{
				for (int j = 0; j < length; j++)
				{
					buffer[j] = this.ReadUShort();
				}
			}
		}

		public unsafe void SerializeBuffer(int* buffer, int length)
		{
			bool write = this._write;
			if (write)
			{
				for (int i = 0; i < length; i++)
				{
					this.WriteInt(buffer[i]);
				}
			}
			else
			{
				for (int j = 0; j < length; j++)
				{
					buffer[j] = this.ReadInt();
				}
			}
		}

		public unsafe void SerializeBuffer(uint* buffer, int length)
		{
			bool write = this._write;
			if (write)
			{
				for (int i = 0; i < length; i++)
				{
					this.WriteUInt(buffer[i]);
				}
			}
			else
			{
				for (int j = 0; j < length; j++)
				{
					buffer[j] = this.ReadUInt();
				}
			}
		}

		public unsafe void SerializeBuffer(long* buffer, int length)
		{
			bool write = this._write;
			if (write)
			{
				for (int i = 0; i < length; i++)
				{
					this.WriteLong(buffer[i]);
				}
			}
			else
			{
				for (int j = 0; j < length; j++)
				{
					buffer[j] = this.ReadLong();
				}
			}
		}

		public unsafe void SerializeBuffer(ulong* buffer, int length)
		{
			bool write = this._write;
			if (write)
			{
				for (int i = 0; i < length; i++)
				{
					this.WriteULong(buffer[i]);
				}
			}
			else
			{
				for (int j = 0; j < length; j++)
				{
					buffer[j] = this.ReadULong();
				}
			}
		}

		private int _ptr;

		private int _size;

		private byte[] _data;

		private bool _write;

		public delegate void ArrayElementSerializer<T>(ref T element);
	}
}
