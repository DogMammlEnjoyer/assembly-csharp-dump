using System;
using System.Runtime.CompilerServices;

namespace Fusion.Internal
{
	[Serializable]
	public abstract class UnityLinkedListSurrogate<[IsUnmanaged] T, [IsUnmanaged] ReaderWriter> : UnitySurrogateBase where T : struct, ValueType where ReaderWriter : struct, ValueType, IElementReaderWriter<T>
	{
		public abstract T[] DataProperty { get; set; }

		public unsafe override void Read(int* data, int capacity)
		{
			NetworkLinkedList<T> networkLinkedList = new NetworkLinkedList<T>((byte*)data, capacity, UnityLinkedListSurrogate<T, ReaderWriter>._readerWriter);
			T[] dataProperty = this.DataProperty;
			Array.Resize<T>(ref dataProperty, networkLinkedList.Count);
			int num = 0;
			foreach (T t in networkLinkedList)
			{
				dataProperty[num++] = t;
			}
			this.DataProperty = dataProperty;
		}

		public unsafe override void Write(int* data, int capacity)
		{
			NetworkLinkedList<T> networkLinkedList = new NetworkLinkedList<T>((byte*)data, capacity, UnityLinkedListSurrogate<T, ReaderWriter>._readerWriter);
			networkLinkedList.Clear();
			foreach (T value in this.DataProperty)
			{
				networkLinkedList.Add(value);
			}
		}

		public override void Init(int capacity)
		{
			this.DataProperty = Array.Empty<T>();
		}

		private static IElementReaderWriter<T> _readerWriter = Activator.CreateInstance<ReaderWriter>();
	}
}
