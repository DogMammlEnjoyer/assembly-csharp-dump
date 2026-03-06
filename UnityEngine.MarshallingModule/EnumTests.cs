using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	internal class EnumTests
	{
		[NativeThrows]
		public unsafe static void ParameterDynamicArrayEnum(SomeEnum[] enumArray)
		{
			Span<SomeEnum> span = new Span<SomeEnum>(enumArray);
			fixed (SomeEnum* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				EnumTests.ParameterDynamicArrayEnum_Injected(ref managedSpanWrapper);
			}
		}

		public unsafe static void ParameterOutDynamicArrayEnum([Out] SomeEnum[] enumArray)
		{
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (enumArray != null)
				{
					fixed (SomeEnum[] array = enumArray)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				EnumTests.ParameterOutDynamicArrayEnum_Injected(out blittableArrayWrapper);
			}
			finally
			{
				SomeEnum[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<SomeEnum>(ref array);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterDynamicArrayEnum_Injected(ref ManagedSpanWrapper enumArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterOutDynamicArrayEnum_Injected(out BlittableArrayWrapper enumArray);
	}
}
