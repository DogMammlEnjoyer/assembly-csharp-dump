using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[ExcludeFromDocs]
	[NativeHeader("Modules/Marshalling/ReturnArrayMarshallingTests.h")]
	internal static class ReturnArrayMarshallingTests
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: Unmarshalled]
		public static extern float[] ReturnArrayOfPrimitiveTypeWorks_Float1D();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: Unmarshalled]
		public static extern float[,] ReturnArrayOfPrimitiveTypeWorks_Float2D();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: Unmarshalled]
		public static extern float[,,] ReturnArrayOfPrimitiveTypeWorks_Float3D();
	}
}
