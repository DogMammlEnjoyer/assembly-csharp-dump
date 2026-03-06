using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	[NativeHeader("Modules/Terrain/Public/SpeedTreeWind.h")]
	internal struct SpeedTreeWindParamsBufferIterator
	{
		public IntPtr bufferPtr;

		[FixedBuffer(typeof(int), 16)]
		public SpeedTreeWindParamsBufferIterator.<uintParamOffsets>e__FixedBuffer uintParamOffsets;

		public int uintStride;

		public int elementOffset;

		public int elementsCount;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 64)]
		public struct <uintParamOffsets>e__FixedBuffer
		{
			public int FixedElementField;
		}
	}
}
