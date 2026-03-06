using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Fusion.Sockets
{
	public struct NetBitBuffer : INetBitWriteStream, ILogDumpable
	{
		internal short Group
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (short)(this._group - 1);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this._group = (int)(value + 1);
			}
		}

		public unsafe ulong* Data
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._data;
			}
			internal set
			{
				this._data = value;
			}
		}

		public int LengthBits
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._lengthBits;
			}
		}

		public int BytesRemaining
		{
			get
			{
				return this._lengthBytes - Maths.BytesRequiredForBits(this._offsetBits);
			}
		}

		public int LengthBytes
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._lengthBytes;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal set
			{
				Assert.Check(value >= 0);
				this._lengthBits = value << 3;
				this._lengthBytes = value;
			}
		}

		public int OffsetBits
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._offsetBits;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal set
			{
				Assert.Check(value >= 0 && value <= this._lengthBits);
				this._offsetBits = value;
			}
		}

		public bool Done
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._offsetBits == this._lengthBits;
			}
		}

		public bool Overflow
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._offsetBits > this._lengthBits;
			}
		}

		internal bool OverflowOrLessThanOneByteRemaining
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._offsetBits + 8 > this._lengthBits;
			}
		}

		public int OffsetBitsUnsafe
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._offsetBits;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				this._offsetBits = value;
			}
		}

		public bool DoneOrOverflow
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._offsetBits >= this._lengthBits;
			}
		}

		public bool MoreToRead
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._offsetBits < this._lengthBits;
			}
		}

		internal unsafe NetPacketType PacketType
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (NetPacketType)(*(byte*)this._data);
			}
			set
			{
				*(byte*)this._data = (byte)value;
			}
		}

		public unsafe void ReplaceDataFromBlockWithTemp(int tempSize)
		{
			EngineProfiler.Begin("ReplaceDataFromBlockWithTemp");
			tempSize = Native.RoundToAlignment(tempSize, 8);
			bool flag = this._dataBlockOriginal == null;
			if (flag)
			{
				this._dataBlockOriginal = this._data;
				this._data = (ulong*)Native.MallocAndClear(tempSize + 1024);
				Native.MemCpy((void*)this._data, (void*)this._dataBlockOriginal, this._lengthBytes);
			}
			else
			{
				ulong* data = this._data;
				this._data = (ulong*)Native.MallocAndClear(tempSize + 1024);
				Native.MemCpy((void*)this._data, (void*)data, this._lengthBytes);
				Native.Free<ulong>(ref data);
			}
			this._lengthBytes = tempSize;
			this._lengthBits = tempSize << 3;
			EngineProfiler.End();
		}

		public unsafe static NetBitBuffer.Offset GetOffset(NetBitBuffer* buffer)
		{
			return new NetBitBuffer.Offset(buffer);
		}

		public unsafe static NetBitBuffer* Allocate(int group, int size)
		{
			bool flag = size <= 0;
			if (flag)
			{
				throw new InvalidOperationException();
			}
			size = Native.RoundToAlignment(size, 8);
			void* ptr;
			void* buffer;
			Native.MallocAndClearBlock(sizeof(NetBitBuffer), size, out ptr, out buffer, 8);
			NetBitBuffer* ptr2 = (NetBitBuffer*)ptr;
			ptr2->_group = group;
			ptr2->SetBufferLengthBytes((ulong*)buffer, size);
			return ptr2;
		}

		public unsafe static void ReleaseRef(ref NetBitBuffer* buffer)
		{
			bool flag = buffer == (IntPtr)((UIntPtr)0);
			if (!flag)
			{
				NetBitBuffer* buffer2 = buffer;
				buffer = (IntPtr)((UIntPtr)0);
				NetBitBuffer.Release(buffer2);
			}
		}

		public unsafe static void Release(NetBitBuffer* buffer)
		{
			bool flag = buffer == null;
			if (!flag)
			{
				bool flag2 = buffer->_dataBlockOriginal != null;
				if (flag2)
				{
					Native.Free<ulong>(ref buffer->_data);
					buffer->_data = buffer->_dataBlockOriginal;
					buffer->_dataBlockOriginal = null;
				}
				bool flag3 = buffer->_block != null;
				if (flag3)
				{
					buffer->_block->Release(buffer);
				}
				else
				{
					DebugLogStream logDebug = InternalLogStreams.LogDebug;
					if (logDebug != null)
					{
						logDebug.Warn("NetBitBuffer trying to release with a null block.");
					}
				}
			}
		}

		public unsafe void SetBufferLengthBytes(ulong* buffer, int lenghtInBytes)
		{
			Assert.Check(buffer % 8L == null);
			Assert.Check(lenghtInBytes % 8 == 0);
			this._data = buffer;
			this._lengthBits = lenghtInBytes << 3;
			this._lengthBytes = lenghtInBytes;
		}

		public unsafe void Clear()
		{
			Assert.Check(this._data != null);
			Assert.Check(this._lengthBytes > 0);
			Native.MemClear((void*)this._data, this._lengthBytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool WriteBoolean(bool value)
		{
			this.Write(value ? 1UL : 0UL, 1);
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ReadBoolean()
		{
			return this.Read(1) == 1UL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool PeekBoolean()
		{
			return this.Peek(1) == 1UL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteByte(byte value, int bits = 8)
		{
			Assert.Check(bits >= 0 && bits <= 8);
			this.Write((ulong)value, bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte ReadByte(int bits = 8)
		{
			Assert.Check<int>(bits >= 0 && bits <= 8, bits);
			return (byte)this.Read(bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteInt16(short value, int bits = 16)
		{
			Assert.Check(bits >= 0 && bits <= 16);
			this.Write((ulong)((long)value), bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public short ReadInt16(int bits = 16)
		{
			Assert.Check<int>(bits >= 0 && bits <= 16, bits);
			return (short)this.Read(bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteUInt16(ushort value, int bits = 16)
		{
			Assert.Check(bits >= 0 && bits <= 16);
			this.Write((ulong)value, bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ushort ReadUInt16(int bits = 16)
		{
			Assert.Check<int>(bits >= 0 && bits <= 16, bits);
			return (ushort)this.Read(bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteInt32(int value, int bits = 32)
		{
			Assert.Check(bits >= 0 && bits <= 32);
			this.Write((ulong)((long)value), bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int ReadInt32(int bits = 32)
		{
			Assert.Check<int>(bits >= 0 && bits <= 32, bits);
			return (int)this.Read(bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteUInt32(uint value, int bits = 32)
		{
			Assert.Check(bits >= 0 && bits <= 32);
			this.Write((ulong)value, bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteString(string value)
		{
			this.WriteString(value, Encoding.UTF8);
		}

		public void WriteString(string value, Encoding encoding)
		{
			bool flag = this.WriteBoolean(value == null);
			if (!flag)
			{
				byte[] bytes = encoding.GetBytes(value);
				this.WriteUInt16((ushort)bytes.Length, 16);
				this.WriteBytesAligned(bytes, bytes.Length);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ReadString()
		{
			return this.ReadString(Encoding.UTF8);
		}

		public unsafe string ReadString(Encoding encoding)
		{
			bool flag = this.ReadBoolean();
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				int num = (int)this.ReadUInt16(16);
				this.SeekToByteBoundary();
				Assert.Check(this.IsOnEvenByte);
				bool flag2 = num == 0;
				if (flag2)
				{
					result = "";
				}
				else
				{
					int num2 = this.Advance(num * 8, false);
					byte* bytes = (byte*)(this._data + num2 / 8 / 8);
					result = encoding.GetString(bytes, num);
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CanWrite(int bits)
		{
			return this.CanRead(bits);
		}

		public bool CanRead(int bits)
		{
			return this._offsetBits + bits <= this._lengthBits;
		}

		public bool IsOnEvenByte
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._offsetBits % 8 == 0;
			}
		}

		public int OffsetBytes
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				Assert.Check(this.IsOnEvenByte);
				Assert.Check(Maths.BytesRequiredForBits(this._offsetBits) == this._offsetBits / 8);
				return this._offsetBits / 8;
			}
		}

		public void PadToByteBoundary()
		{
			bool flag = this._offsetBits % 8 != 0;
			if (flag)
			{
				this.WriteByte(0, 8 - this._offsetBits % 8);
			}
		}

		public unsafe byte* GetDataPointer()
		{
			Assert.Check(this.IsOnEvenByte);
			return (byte*)(this._data + this._offsetBits / 8 / 8);
		}

		public unsafe byte* PadToByteBoundaryAndGetPtr()
		{
			this.PadToByteBoundary();
			Assert.Check(this._offsetBits % 8 == 0);
			return (byte*)(this._data + this._offsetBits / 8 / 8);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CheckBitCount(int count)
		{
			return count >= 0 && this.OffsetBits + count <= this._lengthBits;
		}

		public void SeekToByteBoundary()
		{
			this._offsetBits = (this._offsetBits + 7 & -8);
		}

		public unsafe void WriteBytesAligned(byte[] buffer, int length)
		{
			fixed (byte[] array = buffer)
			{
				byte* buffer2;
				if (buffer == null || array.Length == 0)
				{
					buffer2 = null;
				}
				else
				{
					buffer2 = &array[0];
				}
				this.WriteBytesAligned((void*)buffer2, length);
			}
		}

		public unsafe void WriteBytesAligned(void* buffer, int length)
		{
			this.PadToByteBoundary();
			Assert.Check(this.IsOnEvenByte);
			Assert.Check<int, int, int, int>(this.OffsetBytes + length <= this._lengthBytes, this.OffsetBytes + length, this.OffsetBytes, length, this._lengthBytes);
			int num = this.Advance(length * 8, true);
			int num2 = num >> 6;
			int num3 = num + length * 8 >> 6;
			bool flag = num3 != num2;
			if (flag)
			{
				this._data[num3] = 0UL;
			}
			Native.MemCpy((void*)(this._data + num / 8 / 8), buffer, length);
		}

		public unsafe void WriteBytesAligned(Span<byte> buffer)
		{
			this.PadToByteBoundary();
			Assert.Check(this.IsOnEvenByte);
			Assert.Check<int, int, int, int>(this.OffsetBytes + buffer.Length <= this._lengthBytes, this.OffsetBytes + buffer.Length, this.OffsetBytes, buffer.Length, this._lengthBytes);
			int num = this.Advance(buffer.Length * 8, true);
			int num2 = num >> 6;
			int num3 = num + buffer.Length * 8 >> 6;
			bool flag = num3 != num2;
			if (flag)
			{
				this._data[num3] = 0UL;
			}
			Span<byte> d = new Span<byte>((void*)(this._data + num / 8 / 8), buffer.Length);
			Native.MemCpy(d, buffer);
		}

		public unsafe void ReadBytesAligned(byte[] buffer, int length)
		{
			fixed (byte[] array = buffer)
			{
				byte* buffer2;
				if (buffer == null || array.Length == 0)
				{
					buffer2 = null;
				}
				else
				{
					buffer2 = &array[0];
				}
				this.ReadBytesAligned((void*)buffer2, length);
			}
		}

		public unsafe void ReadBytesAligned(Span<byte> buffer)
		{
			this.SeekToByteBoundary();
			Assert.Check(this.IsOnEvenByte);
			int num = this.Advance(buffer.Length * 8, false);
			Native.MemCpy(buffer, new Span<byte>((void*)(this._data + num / 8 / 8), buffer.Length));
		}

		public unsafe void ReadBytesAligned(void* buffer, int length)
		{
			this.SeekToByteBoundary();
			Assert.Check(this.IsOnEvenByte);
			int num = this.Advance(length * 8, false);
			Native.MemCpy(buffer, (void*)(this._data + num / 8 / 8), length);
		}

		public void WriteInt64VarLength(long value, int blockSize)
		{
			this.WriteUInt64VarLength((ulong)value, blockSize);
		}

		public void WriteInt32VarLength(int value)
		{
			this.WriteUInt32VarLength((uint)value);
		}

		public void WriteInt32VarLength(int value, int blockSize)
		{
			this.WriteUInt32VarLength((uint)value, blockSize);
		}

		public int ReadInt32VarLength()
		{
			return (int)this.ReadUInt32VarLength();
		}

		public long ReadInt64VarLength(int blockSize)
		{
			return (long)this.ReadUInt64VarLength(blockSize);
		}

		public int ReadInt32VarLength(int blockSize)
		{
			return (int)this.ReadUInt32VarLength(blockSize);
		}

		public uint ReadUInt32VarLength(int blockSize)
		{
			blockSize = Maths.Clamp(blockSize, 2, 16);
			int num = 1;
			while (!this.ReadBoolean() && !this.DoneOrOverflow)
			{
				num++;
			}
			bool doneOrOverflow = this.DoneOrOverflow;
			uint result;
			if (doneOrOverflow)
			{
				result = 0U;
			}
			else
			{
				result = this.ReadUInt32(num * blockSize);
			}
			return result;
		}

		public ulong ReadUInt64VarLength(int blockSize)
		{
			blockSize = Maths.Clamp(blockSize, 2, 16);
			int num = 1;
			while (!this.ReadBoolean() && !this.DoneOrOverflow)
			{
				num++;
			}
			bool doneOrOverflow = this.DoneOrOverflow;
			ulong result;
			if (doneOrOverflow)
			{
				result = 0UL;
			}
			else
			{
				result = this.ReadUInt64(num * blockSize);
			}
			return result;
		}

		public void WriteUInt32VarLength(uint value, int blockSize)
		{
			blockSize = Maths.Clamp(blockSize, 2, 16);
			int num = (Maths.BitScanReverse(value) + blockSize) / blockSize;
			this.WriteUInt32(1U << num - 1, num);
			this.WriteUInt32(value, num * blockSize);
		}

		public void WriteUInt64VarLength(ulong value, int blockSize)
		{
			blockSize = Maths.Clamp(blockSize, 2, 16);
			int num = (Maths.BitScanReverse(value) + blockSize) / blockSize;
			this.WriteUInt32(1U << num - 1, num);
			this.WriteUInt64(value, num * blockSize);
		}

		public unsafe void WriteUInt32VarLength(uint value)
		{
			int num = 0;
			ulong value2 = 0UL;
			byte* ptr = (byte*)(&value2);
			for (;;)
			{
				ptr[num] = (byte)(value & 127U);
				value >>= 7;
				bool flag = value > 0U;
				if (!flag)
				{
					break;
				}
				byte* ptr2 = ptr + num++;
				*ptr2 |= 128;
			}
			this.Write(value2, (num + 1) * 8);
		}

		public unsafe uint ReadUInt32VarLength()
		{
			Assert.Check(this._offsetBits < this._lengthBits);
			int num = this._lengthBits - this._offsetBits;
			bool flag = num > 64;
			if (flag)
			{
				num = 64;
			}
			ulong num2 = this.Peek(num);
			int num3 = 0;
			uint num4 = 0U;
			byte* ptr = (byte*)(&num2);
			for (;;)
			{
				Assert.Check(num3 >= 0 && num3 <= 7);
				uint num5 = (uint)ptr[num3];
				num4 |= (num5 & 127U) << 7 * num3;
				bool flag2 = (num5 & 128U) == 128U;
				if (!flag2)
				{
					break;
				}
				num3++;
			}
			this._offsetBits += (num3 + 1) * 8;
			return num4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint ReadUInt32(int bits = 32)
		{
			Assert.Check<int>(bits >= 0 && bits <= 32, bits);
			return (uint)this.Read(bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteInt64(long value, int bits = 64)
		{
			Assert.Check(bits >= 0 && bits <= 64);
			this.Write((ulong)value, bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public long ReadInt64(int bits = 64)
		{
			Assert.Check<int>(bits >= 0 && bits <= 64, bits);
			return (long)this.Read(bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WriteUInt64(ulong value, int bits = 64)
		{
			Assert.Check<int>(bits >= 0 && bits <= 64, bits);
			this.Write(value, bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ulong ReadUInt64(int bits = 64)
		{
			Assert.Check<int>(bits >= 0 && bits <= 64, bits);
			return this.Read(bits);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteSingle(float value)
		{
			this.Write((ulong)(*(uint*)(&value)), 32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe float ReadSingle()
		{
			ulong num = this.Read(32);
			return *(float*)(&num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteDouble(double value)
		{
			this.Write((ulong)(*(long*)(&value)), 64);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe double ReadDouble()
		{
			ulong num = this.Read(64);
			return *(double*)(&num);
		}

		public void WriteInt32AtOffset(int value, int offset, int bits)
		{
			int offsetBits = this._offsetBits;
			try
			{
				this._offsetBits = offset;
				this.WriteSlow((ulong)value, bits);
			}
			finally
			{
				this._offsetBits = offsetBits;
			}
		}

		public void WriteUInt64AtOffset(ulong value, int offset, int bits)
		{
			int offsetBits = this._offsetBits;
			try
			{
				this._offsetBits = offset;
				this.WriteSlow(value, bits);
			}
			finally
			{
				this._offsetBits = offsetBits;
			}
		}

		public unsafe void Write(ulong value, int bits)
		{
			Assert.Check<int>(bits >= 0 && bits <= 64, bits);
			value &= ulong.MaxValue >> 64 - bits;
			Assert.Check(bits >= 0 && bits <= 64);
			int num = this.Advance(bits, true);
			int num2 = num & 63;
			int num3 = 64 - num2;
			Assert.Check(num2 + num3 == 64);
			ulong* ptr = this._data + (num >> 6);
			bool arg = false;
			*ptr = ((*ptr & (1UL << num2) - 1UL) | value << num2);
			bool flag = num3 < bits;
			if (flag)
			{
				arg = true;
				ptr[1] = value >> num3;
			}
			this._offsetBits = num;
			ulong num4 = this.Read(bits);
			Assert.Check<ulong, ulong, bool>(num4 == value, num4, value, arg);
		}

		public unsafe void WriteSlow(ulong value, int bits)
		{
			Assert.Check<int>(bits >= 0 && bits <= 64, bits);
			bool flag = bits <= 0;
			if (!flag)
			{
				value &= ulong.MaxValue >> 64 - bits;
				int num = this.Advance(bits, false);
				int num2 = num >> 6;
				int num3 = num & 63;
				int num4 = 64 - num3;
				int num5 = num4 - bits;
				ulong* data = this._data;
				bool flag2 = num5 >= 0;
				if (flag2)
				{
					ulong num6 = ulong.MaxValue >> num4 | ulong.MaxValue << 64 - num5;
					data[num2] = ((data[num2] & num6) | value << num3);
				}
				else
				{
					data[num2] = ((data[num2] & ulong.MaxValue >> num4) | value << num3);
					data[num2 + 1] = ((data[num2 + 1] & ulong.MaxValue << bits - num4) | value >> num4);
				}
			}
		}

		private unsafe ulong Read(int bits)
		{
			Assert.Check<int>(bits >= 0 && bits <= 64, bits);
			bool flag = bits <= 0;
			ulong result;
			if (flag)
			{
				result = 0UL;
			}
			else
			{
				int num = this.Advance(bits, false);
				int num2 = num >> 6;
				int num3 = num & 63;
				ulong num4 = this._data[num2] >> num3;
				int num5 = bits - (64 - num3);
				bool flag2 = num5 < 1;
				ulong num6;
				if (flag2)
				{
					num6 = (num4 & ulong.MaxValue >> 64 - bits);
				}
				else
				{
					ulong num7 = this._data[num2 + 1] & ulong.MaxValue >> 64 - num5;
					num6 = (num4 | num7 << bits - num5);
				}
				result = num6;
			}
			return result;
		}

		private unsafe ulong Peek(int bits)
		{
			Assert.Check<int>(bits >= 0 && bits <= 64, bits);
			bool flag = bits <= 0;
			ulong result;
			if (flag)
			{
				result = 0UL;
			}
			else
			{
				bool flag2 = !this.CheckBitCount(bits);
				if (flag2)
				{
					throw new InvalidOperationException(string.Format("Out of bounds. Bit position: {0}, length: {1}, capacity: {2}", this._offsetBits, bits, this.LengthBits));
				}
				int offsetBits = this._offsetBits;
				int num = offsetBits >> 6;
				int num2 = offsetBits & 63;
				ulong num3 = this._data[num] >> num2;
				int num4 = bits - (64 - num2);
				bool flag3 = num4 < 1;
				ulong num5;
				if (flag3)
				{
					num5 = (num3 & ulong.MaxValue >> 64 - bits);
				}
				else
				{
					ulong num6 = this._data[num + 1] & ulong.MaxValue >> 64 - num4;
					num5 = (num3 | num6 << bits - num4);
				}
				result = num5;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int Advance(int bits, bool writing)
		{
			int offsetBits = this._offsetBits;
			this._offsetBits += bits;
			bool flag = this._offsetBits > this.LengthBits;
			if (flag)
			{
				if (!writing)
				{
					throw new InvalidOperationException(string.Format("Tried to read out of bounds, position: {0}, reading: {1}, capacity: {2}", offsetBits, bits, this.LengthBits));
				}
				this.ReplaceDataFromBlockWithTemp(this.LengthBytes * 2);
			}
			return offsetBits;
		}

		void ILogDumpable.Dump(StringBuilder builder)
		{
			builder.Append(string.Format("[Offset: {0}]", this.OffsetBits));
		}

		private const int BITCOUNT = 64;

		private const int USEDMASK = 63;

		private const int INDEXSHIFT = 6;

		private const int BYTESHIFT = 3;

		private const ulong MAXVALUE = 18446744073709551615UL;

		public NetAddress Address;

		internal unsafe NetBitBuffer* Prev;

		internal unsafe NetBitBuffer* Next;

		internal unsafe NetBitBufferBlock* _block;

		internal unsafe NetBitBuffer* _allocNext;

		private int _group;

		private unsafe ulong* _data;

		private unsafe ulong* _dataBlockOriginal;

		private int _offsetBits;

		private int _lengthBits;

		private int _lengthBytes;

		public struct Offset
		{
			public unsafe Offset(NetBitBuffer* buffer)
			{
				this._offset = buffer->OffsetBits;
			}

			public unsafe int GetLength(NetBitBuffer* buffer)
			{
				return buffer->OffsetBits - this._offset;
			}

			private int _offset;
		}
	}
}
