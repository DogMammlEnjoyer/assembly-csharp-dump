using System;

namespace WebSocketSharp.Net
{
	internal class Chunk
	{
		public Chunk(byte[] data)
		{
			this._data = data;
		}

		public int ReadLeft
		{
			get
			{
				return this._data.Length - this._offset;
			}
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			int num = this._data.Length - this._offset;
			bool flag = num == 0;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = count > num;
				if (flag2)
				{
					count = num;
				}
				Buffer.BlockCopy(this._data, this._offset, buffer, offset, count);
				this._offset += count;
				result = count;
			}
			return result;
		}

		private byte[] _data;

		private int _offset;
	}
}
