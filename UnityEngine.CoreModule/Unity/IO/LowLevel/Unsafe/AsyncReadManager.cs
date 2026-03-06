using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.IO.LowLevel.Unsafe
{
	[NativeHeader("Runtime/File/AsyncReadManagerManagedApi.h")]
	public static class AsyncReadManager
	{
		[FreeFunction("AsyncReadManagerManaged::Read", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private unsafe static ReadHandle ReadInternal(string filename, void* cmds, uint cmdCount, string assetName, ulong typeID, AssetLoadingSubsystem subsystem)
		{
			ReadHandle result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filename, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filename.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(assetName, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = assetName.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ReadHandle readHandle;
				AsyncReadManager.ReadInternal_Injected(ref managedSpanWrapper, cmds, cmdCount, ref managedSpanWrapper2, typeID, subsystem, out readHandle);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
				ReadHandle readHandle;
				result = readHandle;
			}
			return result;
		}

		public unsafe static ReadHandle Read(string filename, ReadCommand* readCmds, uint readCmdCount, string assetName = "", ulong typeID = 0UL, AssetLoadingSubsystem subsystem = AssetLoadingSubsystem.Scripts)
		{
			return AsyncReadManager.ReadInternal(filename, (void*)readCmds, readCmdCount, assetName, typeID, subsystem);
		}

		[ThreadAndSerializationSafe]
		[FreeFunction("AsyncReadManagerManaged::GetFileInfo", IsThreadSafe = true)]
		private unsafe static ReadHandle GetFileInfoInternal(string filename, void* cmd)
		{
			ReadHandle result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filename, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filename.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ReadHandle readHandle;
				AsyncReadManager.GetFileInfoInternal_Injected(ref managedSpanWrapper, cmd, out readHandle);
			}
			finally
			{
				char* ptr = null;
				ReadHandle readHandle;
				result = readHandle;
			}
			return result;
		}

		public unsafe static ReadHandle GetFileInfo(string filename, FileInfoResult* result)
		{
			bool flag = result == null;
			if (flag)
			{
				throw new NullReferenceException("GetFileInfo must have a valid FileInfoResult to write into.");
			}
			return AsyncReadManager.GetFileInfoInternal(filename, (void*)result);
		}

		[ThreadAndSerializationSafe]
		[FreeFunction("AsyncReadManagerManaged::ReadWithHandles_NativePtr", IsThreadSafe = true)]
		private unsafe static ReadHandle ReadWithHandlesInternal_NativePtr(in FileHandle fileHandle, void* readCmdArray, JobHandle dependency)
		{
			ReadHandle result;
			AsyncReadManager.ReadWithHandlesInternal_NativePtr_Injected(fileHandle, readCmdArray, ref dependency, out result);
			return result;
		}

		[FreeFunction("AsyncReadManagerManaged::ReadWithHandles_NativeCopy", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private unsafe static ReadHandle ReadWithHandlesInternal_NativeCopy(in FileHandle fileHandle, void* readCmdArray)
		{
			ReadHandle result;
			AsyncReadManager.ReadWithHandlesInternal_NativeCopy_Injected(fileHandle, readCmdArray, out result);
			return result;
		}

		public unsafe static ReadHandle ReadDeferred(in FileHandle fileHandle, ReadCommandArray* readCmdArray, JobHandle dependency)
		{
			bool flag = !fileHandle.IsValid();
			if (flag)
			{
				throw new InvalidOperationException("FileHandle is invalid and may not be read from.");
			}
			return AsyncReadManager.ReadWithHandlesInternal_NativePtr(fileHandle, (void*)readCmdArray, dependency);
		}

		public static ReadHandle Read(in FileHandle fileHandle, ReadCommandArray readCmdArray)
		{
			bool flag = !fileHandle.IsValid();
			if (flag)
			{
				throw new InvalidOperationException("FileHandle is invalid and may not be read from.");
			}
			return AsyncReadManager.ReadWithHandlesInternal_NativeCopy(fileHandle, UnsafeUtility.AddressOf<ReadCommandArray>(ref readCmdArray));
		}

		[FreeFunction("AsyncReadManagerManaged::ScheduleOpenRequest", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		private unsafe static FileHandle OpenFileAsync_Internal(string fileName)
		{
			FileHandle result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(fileName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = fileName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				FileHandle fileHandle;
				AsyncReadManager.OpenFileAsync_Internal_Injected(ref managedSpanWrapper, out fileHandle);
			}
			finally
			{
				char* ptr = null;
				FileHandle fileHandle;
				result = fileHandle;
			}
			return result;
		}

		public static FileHandle OpenFileAsync(string fileName)
		{
			bool flag = fileName.Length == 0;
			if (flag)
			{
				throw new InvalidOperationException("FileName is empty");
			}
			return AsyncReadManager.OpenFileAsync_Internal(fileName);
		}

		[FreeFunction("AsyncReadManagerManaged::ScheduleCloseRequest", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		internal static JobHandle CloseFileAsync(in FileHandle fileHandle, JobHandle dependency)
		{
			JobHandle result;
			AsyncReadManager.CloseFileAsync_Injected(fileHandle, ref dependency, out result);
			return result;
		}

		[FreeFunction("AsyncReadManagerManaged::ScheduleCloseCachedFileRequest", IsThreadSafe = true)]
		[ThreadAndSerializationSafe]
		public unsafe static JobHandle CloseCachedFileAsync(string fileName, JobHandle dependency = default(JobHandle))
		{
			JobHandle result;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(fileName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = fileName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				JobHandle jobHandle;
				AsyncReadManager.CloseCachedFileAsync_Injected(ref managedSpanWrapper, ref dependency, out jobHandle);
			}
			finally
			{
				char* ptr = null;
				JobHandle jobHandle;
				result = jobHandle;
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ReadInternal_Injected(ref ManagedSpanWrapper filename, void* cmds, uint cmdCount, ref ManagedSpanWrapper assetName, ulong typeID, AssetLoadingSubsystem subsystem, out ReadHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void GetFileInfoInternal_Injected(ref ManagedSpanWrapper filename, void* cmd, out ReadHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ReadWithHandlesInternal_NativePtr_Injected(in FileHandle fileHandle, void* readCmdArray, [In] ref JobHandle dependency, out ReadHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ReadWithHandlesInternal_NativeCopy_Injected(in FileHandle fileHandle, void* readCmdArray, out ReadHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void OpenFileAsync_Internal_Injected(ref ManagedSpanWrapper fileName, out FileHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CloseFileAsync_Injected(in FileHandle fileHandle, [In] ref JobHandle dependency, out JobHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CloseCachedFileAsync_Injected(ref ManagedSpanWrapper fileName, [In] ref JobHandle dependency, out JobHandle ret);
	}
}
