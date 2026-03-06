using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.VirtualTexturing
{
	[StaticAccessor("VirtualTexturing::Editor", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/VirtualTexturing/ScriptBindings/VirtualTexturing.bindings.h")]
	[NativeConditional("UNITY_EDITOR")]
	public static class EditorHelpers
	{
		[NativeThrows]
		internal static extern int tileSize { [MethodImpl(MethodImplOptions.InternalCall)] get; }

		[NativeThrows]
		public static bool ValidateTextureStack([Unmarshalled] [NotNull] Texture[] textures, out string errorMessage)
		{
			if (textures == null)
			{
				ThrowHelper.ThrowArgumentNullException(textures, "textures");
			}
			bool result;
			try
			{
				ManagedSpanWrapper managedSpan;
				result = EditorHelpers.ValidateTextureStack_Injected(textures, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				errorMessage = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return result;
		}

		[NativeThrows]
		internal static EditorHelpers.StackValidationResult[] ValidateMaterialTextureStacks([NotNull] Material mat)
		{
			if (mat == null)
			{
				ThrowHelper.ThrowArgumentNullException(mat, "mat");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Material>(mat);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(mat, "mat");
			}
			return EditorHelpers.ValidateMaterialTextureStacks_Injected(intPtr);
		}

		[NativeConditional("UNITY_EDITOR")]
		[NativeThrows]
		public static GraphicsFormat[] QuerySupportedFormats()
		{
			GraphicsFormat[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				EditorHelpers.QuerySupportedFormats_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				GraphicsFormat[] array;
				blittableArrayWrapper.Unmarshal<GraphicsFormat>(ref array);
				result = array;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ValidateTextureStack_Injected(Texture[] textures, out ManagedSpanWrapper errorMessage);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern EditorHelpers.StackValidationResult[] ValidateMaterialTextureStacks_Injected(IntPtr mat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void QuerySupportedFormats_Injected(out BlittableArrayWrapper ret);

		[NativeHeader("Runtime/Shaders/SharedMaterialData.h")]
		internal struct StackValidationResult
		{
			public string stackName;

			public string errorMessage;
		}
	}
}
