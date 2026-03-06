using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.VirtualTexturing
{
	[StaticAccessor("VirtualTexturing::Debugging", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/VirtualTexturing/ScriptBindings/VirtualTexturing.bindings.h")]
	public static class Debugging
	{
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetNumHandles();

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void GrabHandleInfo(out Debugging.Handle debugHandle, int index);

		[NativeThrows]
		public static string GetInfoDump()
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				Debugging.GetInfoDump_Injected(out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[NativeThrows]
		public static extern bool debugTilesEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		[NativeThrows]
		public static extern bool resolvingEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		[NativeThrows]
		public static extern bool flushEveryTickEnabled { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

		[NativeThrows]
		public static extern int mipPreloadedTextureCount { [MethodImpl(MethodImplOptions.InternalCall)] get; }

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetInfoDump_Injected(out ManagedSpanWrapper ret);

		[NativeHeader("Modules/VirtualTexturing/Public/VirtualTexturingDebugHandle.h")]
		[UsedByNativeCode]
		public struct Handle
		{
			public long handle;

			public string group;

			public string name;

			public int numLayers;

			public Material material;
		}
	}
}
