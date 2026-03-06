using System;

namespace ICSharpCode.SharpZipLib.Checksum
{
	public sealed class Adler32 : IChecksum
	{
		public Adler32()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.checkValue = 1U;
		}

		public long Value
		{
			get
			{
				return (long)((ulong)this.checkValue);
			}
		}

		public void Update(int bval)
		{
			uint num = this.checkValue & 65535U;
			uint num2 = this.checkValue >> 16;
			num = (num + (uint)(bval & 255)) % Adler32.BASE;
			num2 = (num + num2) % Adler32.BASE;
			this.checkValue = (num2 << 16) + num;
		}

		public void Update(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			this.Update(new ArraySegment<byte>(buffer, 0, buffer.Length));
		}

		public void Update(ArraySegment<byte> segment)
		{
			uint num = this.checkValue & 65535U;
			uint num2 = this.checkValue >> 16;
			int i = segment.Count;
			int offset = segment.Offset;
			while (i > 0)
			{
				int num3 = 3800;
				if (num3 > i)
				{
					num3 = i;
				}
				i -= num3;
				while (--num3 >= 0)
				{
					num += (uint)(segment.Array[offset++] & byte.MaxValue);
					num2 += num;
				}
				num %= Adler32.BASE;
				num2 %= Adler32.BASE;
			}
			this.checkValue = (num2 << 16 | num);
		}

		private static readonly uint BASE = 65521U;

		private uint checkValue;
	}
}
