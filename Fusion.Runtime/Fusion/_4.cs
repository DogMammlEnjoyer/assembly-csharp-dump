using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(4)]
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Pack = 4, Size = 16)]
	public struct _4 : INetworkStruct, IFixedStorage
	{
		public const int SIZE = 16;

		[FixedBuffer(typeof(uint), 4)]
		[FieldOffset(0)]
		public _4.<Data>e__FixedBuffer Data;

		[NonSerialized]
		[FieldOffset(0)]
		private uint _data0;

		[NonSerialized]
		[FieldOffset(4)]
		private uint _data1;

		[NonSerialized]
		[FieldOffset(8)]
		private uint _data2;

		[NonSerialized]
		[FieldOffset(12)]
		private uint _data3;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <Data>e__FixedBuffer
		{
			public uint FixedElementField;
		}
	}
}
