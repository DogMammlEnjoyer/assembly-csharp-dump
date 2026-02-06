using System;

namespace ExitGames.Client.Photon
{
	public class ByteArraySlice : IDisposable
	{
		internal ByteArraySlice(ByteArraySlicePool returnPool, int stackIndex)
		{
			this.Buffer = ((stackIndex == 0) ? null : new byte[1 << stackIndex]);
			this.returnPool = returnPool;
			this.stackIndex = stackIndex;
		}

		public ByteArraySlice(byte[] buffer, int offset = 0, int count = 0)
		{
			this.Buffer = buffer;
			this.Count = count;
			this.Offset = offset;
			this.returnPool = null;
			this.stackIndex = -1;
		}

		public ByteArraySlice()
		{
			this.returnPool = null;
			this.stackIndex = -1;
		}

		public void Dispose()
		{
			this.Release();
		}

		public bool Release()
		{
			bool flag = this.stackIndex < 0;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.Count = 0;
				this.Offset = 0;
				result = this.returnPool.Release(this, this.stackIndex);
			}
			return result;
		}

		public void Reset()
		{
			this.Count = 0;
			this.Offset = 0;
		}

		public byte[] Buffer;

		public int Offset;

		public int Count;

		private readonly ByteArraySlicePool returnPool;

		private readonly int stackIndex;
	}
}
