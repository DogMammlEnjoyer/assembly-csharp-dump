using System;
using System.Text;

namespace Fusion.Sockets
{
	public struct NetBitBufferSerializer
	{
		public bool Writing
		{
			get
			{
				return this._write;
			}
		}

		public bool Reading
		{
			get
			{
				return !this._write;
			}
		}

		public unsafe NetBitBuffer* Buffer
		{
			get
			{
				return this._buffer;
			}
		}

		private unsafe NetBitBufferSerializer(NetBitBuffer* buffer, bool write)
		{
			this._write = write;
			this._buffer = buffer;
		}

		public unsafe static NetBitBufferSerializer Writer(NetBitBuffer* buffer)
		{
			return new NetBitBufferSerializer(buffer, true);
		}

		public unsafe static NetBitBufferSerializer Reader(NetBitBuffer* buffer)
		{
			return new NetBitBufferSerializer(buffer, false);
		}

		public unsafe bool Check(bool value)
		{
			bool write = this._write;
			bool result;
			if (write)
			{
				result = this._buffer->WriteBoolean(value);
			}
			else
			{
				result = this._buffer->ReadBoolean();
			}
			return result;
		}

		public unsafe void Serialize(ref float value)
		{
			bool write = this._write;
			if (write)
			{
				this._buffer->WriteSingle(value);
			}
			else
			{
				value = this._buffer->ReadSingle();
			}
		}

		public unsafe void Serialize(ref byte value)
		{
			bool write = this._write;
			if (write)
			{
				this._buffer->WriteByte(value, 8);
			}
			else
			{
				value = this._buffer->ReadByte(8);
			}
		}

		public unsafe void Serialize(ref bool value)
		{
			bool write = this._write;
			if (write)
			{
				this._buffer->WriteBoolean(value);
			}
			else
			{
				value = this._buffer->ReadBoolean();
			}
		}

		public unsafe void Serialize(ref int value)
		{
			bool write = this._write;
			if (write)
			{
				this._buffer->WriteInt32(value, 32);
			}
			else
			{
				value = this._buffer->ReadInt32(32);
			}
		}

		public unsafe void Serialize(ref uint value)
		{
			bool write = this._write;
			if (write)
			{
				this._buffer->WriteUInt32(value, 32);
			}
			else
			{
				value = this._buffer->ReadUInt32(32);
			}
		}

		public unsafe void Serialize(ref ulong value)
		{
			bool write = this._write;
			if (write)
			{
				this._buffer->WriteUInt64(value, 64);
			}
			else
			{
				value = this._buffer->ReadUInt64(64);
			}
		}

		public unsafe void Serialize(ref string value)
		{
			bool write = this._write;
			if (write)
			{
				this._buffer->WriteString(value, Encoding.UTF8);
			}
			else
			{
				value = this._buffer->ReadString(Encoding.UTF8);
			}
		}

		private bool _write;

		private unsafe NetBitBuffer* _buffer;
	}
}
