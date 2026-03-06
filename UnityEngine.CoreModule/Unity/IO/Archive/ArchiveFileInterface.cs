using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Content;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.IO.Archive
{
	[RequiredByNativeCode]
	[NativeHeader("Runtime/VirtualFileSystem/ArchiveFileSystem/ArchiveFileHandle.h")]
	[StaticAccessor("GetManagedArchiveSystem()", StaticAccessorType.Dot)]
	public static class ArchiveFileInterface
	{
		public unsafe static ArchiveHandle MountAsync(ContentNamespace namespaceId, string filePath, string prefix)
		{
			ArchiveHandle result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(prefix, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = prefix.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ArchiveHandle archiveHandle;
				ArchiveFileInterface.MountAsync_Injected(ref namespaceId, ref managedSpanWrapper, ref managedSpanWrapper2, out archiveHandle);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
				ArchiveHandle archiveHandle;
				result = archiveHandle;
			}
			return result;
		}

		public static ArchiveHandle[] GetMountedArchives(ContentNamespace namespaceId)
		{
			ArchiveHandle[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ArchiveFileInterface.GetMountedArchives_Injected(ref namespaceId, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ArchiveHandle[] array;
				blittableArrayWrapper.Unmarshal<ArchiveHandle>(ref array);
				result = array;
			}
			return result;
		}

		internal static ArchiveStatus Archive_GetStatus(ArchiveHandle handle)
		{
			return ArchiveFileInterface.Archive_GetStatus_Injected(ref handle);
		}

		internal static JobHandle Archive_GetJobHandle(ArchiveHandle handle)
		{
			JobHandle result;
			ArchiveFileInterface.Archive_GetJobHandle_Injected(ref handle, out result);
			return result;
		}

		internal static bool Archive_IsValid(ArchiveHandle handle)
		{
			return ArchiveFileInterface.Archive_IsValid_Injected(ref handle);
		}

		internal static JobHandle Archive_UnmountAsync(ArchiveHandle handle)
		{
			JobHandle result;
			ArchiveFileInterface.Archive_UnmountAsync_Injected(ref handle, out result);
			return result;
		}

		internal static string Archive_GetMountPath(ArchiveHandle handle)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				ArchiveFileInterface.Archive_GetMountPath_Injected(ref handle, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		internal static CompressionType Archive_GetCompression(ArchiveHandle handle)
		{
			return ArchiveFileInterface.Archive_GetCompression_Injected(ref handle);
		}

		internal static bool Archive_IsStreamed(ArchiveHandle handle)
		{
			return ArchiveFileInterface.Archive_IsStreamed_Injected(ref handle);
		}

		internal static ArchiveFileInfo[] Archive_GetFileInfo(ArchiveHandle handle)
		{
			return ArchiveFileInterface.Archive_GetFileInfo_Injected(ref handle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MountAsync_Injected([In] ref ContentNamespace namespaceId, ref ManagedSpanWrapper filePath, ref ManagedSpanWrapper prefix, out ArchiveHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMountedArchives_Injected([In] ref ContentNamespace namespaceId, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ArchiveStatus Archive_GetStatus_Injected([In] ref ArchiveHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Archive_GetJobHandle_Injected([In] ref ArchiveHandle handle, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Archive_IsValid_Injected([In] ref ArchiveHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Archive_UnmountAsync_Injected([In] ref ArchiveHandle handle, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Archive_GetMountPath_Injected([In] ref ArchiveHandle handle, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern CompressionType Archive_GetCompression_Injected([In] ref ArchiveHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Archive_IsStreamed_Injected([In] ref ArchiveHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ArchiveFileInfo[] Archive_GetFileInfo_Injected([In] ref ArchiveHandle handle);
	}
}
