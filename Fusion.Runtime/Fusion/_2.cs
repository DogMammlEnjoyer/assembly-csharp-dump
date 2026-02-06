using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(2)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 8)]
	public struct _2 : INetworkStruct, IFixedStorage
	{
		public const int SIZE = 8;

		[FixedBuffer(typeof(uint), 2)]
		[FieldOffset(0)]
		public _2.<Data>e__FixedBuffer Data;

		[NonSerialized]
		[FieldOffset(0)]
		private uint _data0;

		[NonSerialized]
		[FieldOffset(4)]
		private uint _data1;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 8)]
		public struct <Data>e__FixedBuffer
		{
			public uint FixedElementField;
		}
	}
}
