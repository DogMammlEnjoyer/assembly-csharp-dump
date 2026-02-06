using System;
using System.Runtime.CompilerServices;

namespace Fusion.Internal
{
	[Serializable]
	public abstract class UnityArraySurrogate<[IsUnmanaged] T, [IsUnmanaged] ReaderWriter> : UnitySurrogateBase where T : struct, ValueType where ReaderWriter : struct, ValueType, IElementReaderWriter<T>
	{
		public abstract T[] DataProperty { get; set; }

		public unsafe override void Read(int* data, int capacity)
		{
			ReaderWriter readerWriter = default(ReaderWriter);
			T[] dataProperty = this.DataProperty;
			Array.Resize<T>(ref dataProperty, capacity);
			for (int i = 0; i < capacity; i++)
			{
				dataProperty[i] = readerWriter.Read((byte*)data, i);
			}
			this.DataProperty = dataProperty;
		}

		public unsafe override void Write(int* data, int capacity)
		{
			ReaderWriter readerWriter = default(ReaderWriter);
			T[] dataProperty = this.DataProperty;
			Array.Resize<T>(ref dataProperty, capacity);
			for (int i = 0; i < capacity; i++)
			{
				readerWriter.Write((byte*)data, i, dataProperty[i]);
			}
		}

		public override void Init(int capacity)
		{
			this.DataProperty = new T[capacity];
		}
	}
}
