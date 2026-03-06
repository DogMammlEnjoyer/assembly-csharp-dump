using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;

namespace UnityEngine.Experimental.Playables
{
	[NativeHeader("Runtime/Export/Director/TexturePlayableGraphExtensions.bindings.h")]
	[StaticAccessor("TexturePlayableGraphExtensionsBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
	internal static class TexturePlayableGraphExtensions
	{
		[NativeThrows]
		internal unsafe static bool InternalCreateTextureOutput(ref PlayableGraph graph, string name, out PlayableOutputHandle handle)
		{
			bool result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = TexturePlayableGraphExtensions.InternalCreateTextureOutput_Injected(ref graph, ref managedSpanWrapper, out handle);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalCreateTextureOutput_Injected(ref PlayableGraph graph, ref ManagedSpanWrapper name, out PlayableOutputHandle handle);
	}
}
