using System;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(10)]
	[StructLayout(LayoutKind.Explicit)]
	public struct NetworkPhysicsInfo : INetworkStruct
	{
		public const int WORD_COUNT = 10;

		public const int SIZE = 40;

		[FieldOffset(0)]
		public float TimeScale;
	}
}
