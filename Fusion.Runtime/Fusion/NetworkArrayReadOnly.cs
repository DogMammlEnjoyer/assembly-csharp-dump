using System;

namespace Fusion
{
	public readonly ref struct NetworkArrayReadOnly<T>
	{
		public int Length
		{
			get
			{
				return this._length;
			}
		}

		public T this[int index]
		{
			get
			{
				bool flag = index >= this._length;
				if (flag)
				{
					throw new IndexOutOfRangeException();
				}
				return this._readerWriter.Read(this._array, index);
			}
		}

		internal unsafe NetworkArrayReadOnly(byte* array, int length, IElementReaderWriter<T> readerWriter)
		{
			this._array = array;
			this._length = length;
			this._readerWriter = readerWriter;
		}

		private unsafe readonly byte* _array;

		private readonly int _length;

		private readonly IElementReaderWriter<T> _readerWriter;
	}
}
