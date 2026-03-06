using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace Unity.IO.LowLevel.Unsafe
{
	[StaticAccessor("GetFileSystem()", StaticAccessorType.Dot)]
	[NativeHeader("Runtime/VirtualFileSystem/VirtualFileSystem.h")]
	public static class VirtualFileSystem
	{
		[FreeFunction(IsThreadSafe = true)]
		public unsafe static bool GetLocalFileSystemName(string vfsFileName, out string localFileName, out ulong localFileOffset, out ulong localFileSize)
		{
			bool localFileSystemName_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(vfsFileName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = vfsFileName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpan;
				localFileSystemName_Injected = VirtualFileSystem.GetLocalFileSystemName_Injected(ref managedSpanWrapper, out managedSpan, out localFileOffset, out localFileSize);
			}
			finally
			{
				char* ptr = null;
				ManagedSpanWrapper managedSpan;
				localFileName = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return localFileSystemName_Injected;
		}

		internal unsafe static string ToLogicalPath(string physicalPath)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(physicalPath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = physicalPath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpan;
				VirtualFileSystem.ToLogicalPath_Injected(ref managedSpanWrapper, out managedSpan);
			}
			finally
			{
				char* ptr = null;
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetLocalFileSystemName_Injected(ref ManagedSpanWrapper vfsFileName, out ManagedSpanWrapper localFileName, out ulong localFileOffset, out ulong localFileSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ToLogicalPath_Injected(ref ManagedSpanWrapper physicalPath, out ManagedSpanWrapper ret);
	}
}
