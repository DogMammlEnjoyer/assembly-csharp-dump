using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Profiling.Memory
{
	[NativeHeader("Runtime/Profiler/Runtime/MemorySnapshotManager.h")]
	public static class MemoryProfiler
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static event Action<string, bool> m_SnapshotFinished;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static event Action<string, bool, DebugScreenCapture> m_SaveScreenshotToDisk;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<MemorySnapshotMetadata> CreatingMetadata;

		[NativeConditional("ENABLE_PROFILER")]
		[NativeMethod("StartOperation")]
		[StaticAccessor("profiling::memory::GetMemorySnapshotManager()", StaticAccessorType.Dot)]
		private unsafe static void StartOperation(uint captureFlag, bool requestScreenshot, string path, bool isRemote)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				MemoryProfiler.StartOperation_Injected(captureFlag, requestScreenshot, ref managedSpanWrapper, isRemote);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public static void TakeSnapshot(string path, Action<string, bool> finishCallback, CaptureFlags captureFlags = CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects)
		{
			MemoryProfiler.TakeSnapshot(path, finishCallback, null, captureFlags);
		}

		public static void TakeSnapshot(string path, Action<string, bool> finishCallback, Action<string, bool, DebugScreenCapture> screenshotCallback, CaptureFlags captureFlags = CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects)
		{
			bool flag = MemoryProfiler.m_SnapshotFinished != null;
			if (flag)
			{
				Debug.LogWarning("Canceling snapshot, there is another snapshot in progress.");
				finishCallback(path, false);
			}
			else
			{
				MemoryProfiler.m_SnapshotFinished += finishCallback;
				MemoryProfiler.m_SaveScreenshotToDisk += screenshotCallback;
				MemoryProfiler.StartOperation((uint)captureFlags, MemoryProfiler.m_SaveScreenshotToDisk != null, path, false);
			}
		}

		public static void TakeTempSnapshot(Action<string, bool> finishCallback, CaptureFlags captureFlags = CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects)
		{
			string[] array = Application.dataPath.Split('/', StringSplitOptions.None);
			string str = array[array.Length - 2];
			string path = Application.temporaryCachePath + "/" + str + ".snap";
			MemoryProfiler.TakeSnapshot(path, finishCallback, captureFlags);
		}

		[RequiredByNativeCode]
		private unsafe static byte[] PrepareMetadata()
		{
			bool flag = MemoryProfiler.CreatingMetadata == null;
			byte[] result;
			if (flag)
			{
				result = new byte[0];
			}
			else
			{
				MemorySnapshotMetadata memorySnapshotMetadata = new MemorySnapshotMetadata();
				memorySnapshotMetadata.Description = string.Empty;
				MemoryProfiler.CreatingMetadata(memorySnapshotMetadata);
				bool flag2 = memorySnapshotMetadata.Description == null;
				if (flag2)
				{
					memorySnapshotMetadata.Description = "";
				}
				int num = 2 * memorySnapshotMetadata.Description.Length;
				int num2 = (memorySnapshotMetadata.Data == null) ? 0 : memorySnapshotMetadata.Data.Length;
				int num3 = num + num2 + 12;
				byte[] array = new byte[num3];
				int num4 = 0;
				num4 = MemoryProfiler.WriteIntToByteArray(array, num4, memorySnapshotMetadata.Description.Length);
				num4 = MemoryProfiler.WriteStringToByteArray(array, num4, memorySnapshotMetadata.Description);
				num4 = MemoryProfiler.WriteIntToByteArray(array, num4, num2);
				byte[] array2;
				byte* source;
				if ((array2 = memorySnapshotMetadata.Data) == null || array2.Length == 0)
				{
					source = null;
				}
				else
				{
					source = &array2[0];
				}
				byte[] array3;
				byte* ptr;
				if ((array3 = array) == null || array3.Length == 0)
				{
					ptr = null;
				}
				else
				{
					ptr = &array3[0];
				}
				byte* destination = ptr + num4;
				UnsafeUtility.MemCpy((void*)destination, (void*)source, (long)num2);
				array2 = null;
				array3 = null;
				result = array;
			}
			return result;
		}

		internal unsafe static int WriteIntToByteArray(byte[] array, int offset, int value)
		{
			byte* ptr = (byte*)(&value);
			array[offset++] = *ptr;
			array[offset++] = ptr[1];
			array[offset++] = ptr[2];
			array[offset++] = ptr[3];
			return offset;
		}

		internal unsafe static int WriteStringToByteArray(byte[] array, int offset, string value)
		{
			bool flag = value.Length != 0;
			if (flag)
			{
				fixed (string text = value)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					char* ptr2 = ptr;
					char* ptr3 = ptr + value.Length;
					while (ptr2 != ptr3)
					{
						for (int i = 0; i < 2; i++)
						{
							array[offset++] = *(byte*)(ptr2 + i / 2);
						}
						ptr2++;
					}
				}
			}
			return offset;
		}

		[RequiredByNativeCode]
		private static void FinalizeSnapshot(string path, bool result)
		{
			bool flag = MemoryProfiler.m_SnapshotFinished != null;
			if (flag)
			{
				Action<string, bool> snapshotFinished = MemoryProfiler.m_SnapshotFinished;
				MemoryProfiler.m_SnapshotFinished = null;
				snapshotFinished(path, result);
			}
		}

		[RequiredByNativeCode]
		private static void SaveScreenshotToDisk(string path, bool result, IntPtr pixelsPtr, int pixelsCount, TextureFormat format, int width, int height)
		{
			bool flag = MemoryProfiler.m_SaveScreenshotToDisk != null;
			if (flag)
			{
				Action<string, bool, DebugScreenCapture> saveScreenshotToDisk = MemoryProfiler.m_SaveScreenshotToDisk;
				MemoryProfiler.m_SaveScreenshotToDisk = null;
				DebugScreenCapture arg = default(DebugScreenCapture);
				if (result)
				{
					NativeArray<byte> rawImageDataReference = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(pixelsPtr.ToPointer(), pixelsCount, Allocator.Persistent);
					arg.RawImageDataReference = rawImageDataReference;
					arg.Height = height;
					arg.Width = width;
					arg.ImageFormat = format;
				}
				saveScreenshotToDisk(path, result, arg);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StartOperation_Injected(uint captureFlag, bool requestScreenshot, ref ManagedSpanWrapper path, bool isRemote);
	}
}
