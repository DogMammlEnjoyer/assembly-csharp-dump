using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Networking
{
	[NativeHeader("Modules/UnityWebRequestAudio/Public/DownloadHandlerAudioClip.h")]
	internal static class WebRequestWWW
	{
		[FreeFunction("UnityWebRequestCreateAudioClip")]
		internal unsafe static AudioClip InternalCreateAudioClipUsingDH(DownloadHandler dh, string url, bool stream, bool compressed, AudioType audioType)
		{
			AudioClip result;
			try
			{
				IntPtr dh2 = (dh == null) ? ((IntPtr)0) : DownloadHandler.BindingsMarshaller.ConvertToNative(dh);
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(url, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = url.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr gcHandlePtr = WebRequestWWW.InternalCreateAudioClipUsingDH_Injected(dh2, ref managedSpanWrapper, stream, compressed, audioType);
			}
			finally
			{
				IntPtr gcHandlePtr;
				result = Unmarshal.UnmarshalUnityObject<AudioClip>(gcHandlePtr);
				char* ptr = null;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalCreateAudioClipUsingDH_Injected(IntPtr dh, ref ManagedSpanWrapper url, bool stream, bool compressed, AudioType audioType);
	}
}
