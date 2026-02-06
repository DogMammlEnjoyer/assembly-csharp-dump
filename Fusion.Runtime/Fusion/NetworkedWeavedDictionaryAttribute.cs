using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Property)]
	public class NetworkedWeavedDictionaryAttribute : Attribute
	{
		public NetworkedWeavedDictionaryAttribute(int capacity, int keyWordCount, int elementWordCount, Type keyReaderWriterType, Type valueReaderWriterType)
		{
			this.Capacity = capacity;
			this.KeyWordCount = keyWordCount;
			this.ValueWordCount = elementWordCount;
			this.KeyReaderWriterType = keyReaderWriterType;
			this.ValueReaderWriterType = valueReaderWriterType;
		}

		public int Capacity { get; }

		public int KeyWordCount { get; }

		public int ValueWordCount { get; }

		public Type KeyReaderWriterType { get; set; }

		public Type ValueReaderWriterType { get; set; }
	}
}
