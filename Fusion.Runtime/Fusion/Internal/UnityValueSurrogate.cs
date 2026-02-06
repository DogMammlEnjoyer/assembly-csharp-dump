using System;
using System.Runtime.CompilerServices;

namespace Fusion.Internal
{
	[Serializable]
	public abstract class UnityValueSurrogate<T, [IsUnmanaged] TReaderWriter> : UnitySurrogateBase, IUnityValueSurrogate<T>, IUnitySurrogate where TReaderWriter : struct, ValueType, IElementReaderWriter<T>
	{
		public abstract T DataProperty { get; set; }

		public unsafe override void Read(int* data, int capacity)
		{
			TReaderWriter treaderWriter = default(TReaderWriter);
			this.DataProperty = treaderWriter.Read((byte*)data, 0);
		}

		public unsafe override void Write(int* data, int capacity)
		{
			TReaderWriter treaderWriter = default(TReaderWriter);
			treaderWriter.Write((byte*)data, 0, this.DataProperty);
		}

		public override void Init(int capacity)
		{
		}
	}
}
