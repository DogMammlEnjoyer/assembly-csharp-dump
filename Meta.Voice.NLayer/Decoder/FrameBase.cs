using System;
using System.Threading;

namespace Meta.Voice.NLayer.Decoder
{
	internal abstract class FrameBase
	{
		internal static int TotalAllocation
		{
			get
			{
				return Interlocked.CompareExchange(ref FrameBase._totalAllocation, 0, 0);
			}
		}

		internal long Offset { get; private set; }

		internal int Length { get; set; }

		internal bool Validate(long offset, MpegStreamReader reader)
		{
			this.Offset = offset;
			this._reader = reader;
			int num = this.Validate();
			if (num > 0)
			{
				this.Length = num;
				return true;
			}
			return false;
		}

		protected int Read(int offset, byte[] buffer)
		{
			return this.Read(offset, buffer, 0, buffer.Length);
		}

		protected int Read(int offset, byte[] buffer, int index, int count)
		{
			if (this._savedBuffer == null)
			{
				return this._reader.Read(this.Offset + (long)offset, buffer, index, count);
			}
			if (index < 0 || index + count > buffer.Length)
			{
				return 0;
			}
			if (offset < 0 || offset >= this._savedBuffer.Length)
			{
				return 0;
			}
			if (offset + count > this._savedBuffer.Length)
			{
				count = this._savedBuffer.Length - index;
			}
			Array.Copy(this._savedBuffer, offset, buffer, index, count);
			return count;
		}

		protected int ReadByte(int offset)
		{
			if (this._savedBuffer == null)
			{
				return this._reader.ReadByte(this.Offset + (long)offset);
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (offset >= this._savedBuffer.Length)
			{
				return -1;
			}
			return (int)this._savedBuffer[offset];
		}

		protected abstract int Validate();

		internal void SaveBuffer()
		{
			this._savedBuffer = new byte[this.Length];
			this._reader.Read(this.Offset, this._savedBuffer, 0, this.Length);
			Interlocked.Add(ref FrameBase._totalAllocation, this.Length);
		}

		internal void ClearBuffer()
		{
			Interlocked.Add(ref FrameBase._totalAllocation, -this.Length);
			this._savedBuffer = null;
		}

		internal virtual void Parse()
		{
		}

		private static int _totalAllocation;

		private MpegStreamReader _reader;

		private byte[] _savedBuffer;
	}
}
