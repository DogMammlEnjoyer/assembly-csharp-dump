using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Fusion
{
	public static class Native
	{
		public unsafe static void MemMove(void* destination, void* source, int size)
		{
			bool flag = destination == null || source == null;
			if (!flag)
			{
				UnsafeUtility.MemMove(destination, source, (long)size);
			}
		}

		public unsafe static void MemCpy(void* destination, void* source, int size)
		{
			bool flag = destination == null || source == null;
			if (!flag)
			{
				UnsafeUtility.MemCpy(destination, source, (long)size);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void MemCpy(Span<int> d, Span<int> s)
		{
			Assert.Always<int, int>(s.Length <= d.Length, s.Length, d.Length);
			UnsafeUtility.MemCpy((void*)d.AsPointer<byte>(), (void*)s.AsPointer<byte>(), (long)(4 * d.Length));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void MemCpy(Span<byte> d, Span<byte> s)
		{
			Assert.Always<int, int>(s.Length <= d.Length, s.Length, d.Length);
			UnsafeUtility.MemCpy((void*)d.AsPointer<byte>(), (void*)s.AsPointer<byte>(), (long)d.Length);
		}

		public unsafe static void MemClear(void* ptr, int size)
		{
			bool flag = ptr == null;
			if (!flag)
			{
				UnsafeUtility.MemClear(ptr, (long)size);
			}
		}

		public unsafe static int MemCmp(void* ptr1, void* ptr2, int size)
		{
			bool flag = ptr1 == null || ptr2 == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = UnsafeUtility.MemCmp(ptr1, ptr2, (long)size);
			}
			return result;
		}

		public unsafe static void* Malloc(int size)
		{
			bool flag = size <= 0;
			if (flag)
			{
				throw new Exception(string.Format("Trying to allocate <= bytes: {0}", size));
			}
			bool flag2 = size > 1073741824;
			if (flag2)
			{
				throw new Exception(string.Format("Trying to allocate very large block: {0} bytes", size));
			}
			return UnsafeUtility.Malloc((long)size, 8, Allocator.Persistent);
		}

		private unsafe static void Free(void* memory)
		{
			bool flag = memory == null;
			if (!flag)
			{
				UnsafeUtility.Free(memory, Allocator.Persistent);
			}
		}

		public static int SizeOf(Type t)
		{
			return UnsafeUtility.SizeOf(t);
		}

		public static int GetFieldOffset(FieldInfo fi)
		{
			return UnsafeUtility.GetFieldOffset(fi);
		}

		public unsafe static void Free(ref void* memory)
		{
			void* memory2 = memory;
			memory = (IntPtr)((UIntPtr)0);
			Native.Free(memory2);
		}

		public unsafe static void Free<[IsUnmanaged] T>(ref T* memory) where T : struct, ValueType
		{
			T* memory2 = memory;
			memory = (IntPtr)((UIntPtr)0);
			Native.Free((void*)memory2);
		}

		public unsafe static void Free<[IsUnmanaged] T>(ref T** memory) where T : struct, ValueType
		{
			T** memory2 = memory;
			memory = (IntPtr)((UIntPtr)0);
			Native.Free((void*)memory2);
		}

		public unsafe static void* MallocAndClear(int size)
		{
			void* ptr = Native.Malloc(size);
			Native.MemClear(ptr, size);
			return ptr;
		}

		public unsafe static T* MallocAndClear<[IsUnmanaged] T>() where T : struct, ValueType
		{
			T* ptr = Native.Malloc<T>();
			Native.MemClear((void*)ptr, sizeof(T));
			return ptr;
		}

		public unsafe static T* Malloc<[IsUnmanaged] T>() where T : struct, ValueType
		{
			return (T*)Native.Malloc(sizeof(T));
		}

		public unsafe static void* MallocAndClearArray(int stride, int length)
		{
			void* ptr = Native.Malloc(stride * length);
			Native.MemClear(ptr, stride * length);
			return ptr;
		}

		public unsafe static T* MallocAndClearArray<[IsUnmanaged] T>(int length) where T : struct, ValueType
		{
			return (T*)Native.MallocAndClearArray(sizeof(T), length);
		}

		public unsafe static T* MallocAndClearArrayMin1<[IsUnmanaged] T>(int length) where T : struct, ValueType
		{
			return Native.MallocAndClearArray<T>(Math.Max(1, length));
		}

		public unsafe static T** MallocAndClearPtrArray<[IsUnmanaged] T>(int length) where T : struct, ValueType
		{
			return (T**)Native.MallocAndClearArray(sizeof(T*), length);
		}

		public unsafe static T** MallocAndClearPtrArrayMin1<[IsUnmanaged] T>(int length) where T : struct, ValueType
		{
			return Native.MallocAndClearPtrArray<T>(Math.Max(1, length));
		}

		public unsafe static void ArrayCopy(void* source, int sourceIndex, void* destination, int destinationIndex, int count, int elementStride)
		{
			Native.MemCpy((void*)((byte*)destination + destinationIndex * elementStride), (void*)((byte*)source + sourceIndex * elementStride), count * elementStride);
		}

		public unsafe static void ArrayClear<[IsUnmanaged] T>(T* ptr, int size) where T : struct, ValueType
		{
			Native.MemClear((void*)ptr, sizeof(T) * size);
		}

		public unsafe static int ArrayCompare<[IsUnmanaged] T>(T* ptr1, T* ptr2, int size) where T : struct, ValueType
		{
			return Native.MemCmp((void*)ptr1, (void*)ptr2, sizeof(T) * size);
		}

		public unsafe static T* DoubleArray<[IsUnmanaged] T>(T* array, int currentLength) where T : struct, ValueType
		{
			Assert.Check(currentLength > 0);
			return Native.ExpandArray<T>(array, currentLength, currentLength * 2);
		}

		public unsafe static T* ExpandArray<[IsUnmanaged] T>(T* array, int currentLength, int newLength) where T : struct, ValueType
		{
			Assert.Check(newLength > currentLength);
			T* ptr = Native.MallocAndClearArray<T>(newLength);
			Native.MemCpy((void*)ptr, (void*)array, sizeof(T) * currentLength);
			Native.Free((void*)array);
			return ptr;
		}

		public unsafe static T** DoublePtrArray<[IsUnmanaged] T>(T** array, int currentLength) where T : struct, ValueType
		{
			return Native.ExpandPtrArray<T>(array, currentLength, currentLength * 2);
		}

		public unsafe static T** ExpandPtrArray<[IsUnmanaged] T>(T** array, int currentLength, int newLength) where T : struct, ValueType
		{
			Assert.Check(newLength > currentLength);
			T** ptr = Native.MallocAndClearPtrArray<T>(newLength);
			Native.MemCpy((void*)ptr, (void*)array, sizeof(T*) * currentLength);
			Native.Free((void*)array);
			return ptr;
		}

		public unsafe static void* Expand(void* buffer, int currentSize, int newSize)
		{
			Assert.Check(newSize > currentSize);
			void* ptr = Native.MallocAndClear(newSize);
			Native.MemCpy(ptr, buffer, currentSize);
			Native.Free(buffer);
			return ptr;
		}

		public unsafe static void MemCpyFast(void* d, void* s, int size)
		{
			if (size <= 8)
			{
				if (size == 4)
				{
					*(int*)d = (int)(*(uint*)s);
					return;
				}
				if (size == 8)
				{
					*(long*)d = *(long*)s;
					return;
				}
			}
			else
			{
				if (size == 12)
				{
					*(long*)d = *(long*)s;
					*(int*)((byte*)d + (IntPtr)2 * 4) = (int)(*(uint*)((byte*)s + (IntPtr)2 * 4));
					return;
				}
				if (size == 16)
				{
					*(long*)d = *(long*)s;
					*(long*)((byte*)d + 8) = *(long*)((byte*)s + 8);
					return;
				}
			}
			Native.MemCpy(d, s, size);
		}

		public unsafe static int CopyFromArray<[IsUnmanaged] T>(void* destination, T[] source) where T : struct, ValueType
		{
			T* source2;
			if (source == null || source.Length == 0)
			{
				source2 = null;
			}
			else
			{
				source2 = &source[0];
			}
			Native.MemCpy(destination, (void*)source2, source.Length * sizeof(T));
			return source.Length * sizeof(T);
		}

		public unsafe static int CopyToArray<[IsUnmanaged] T>(T[] destination, void* source) where T : struct, ValueType
		{
			T* destination2;
			if (destination == null || destination.Length == 0)
			{
				destination2 = null;
			}
			else
			{
				destination2 = &destination[0];
			}
			Native.MemCpy((void*)destination2, source, destination.Length * sizeof(T));
			return destination.Length * sizeof(T);
		}

		public static int GetLengthPrefixedUTF8ByteCount(string str)
		{
			return 4 + Encoding.UTF8.GetByteCount(str);
		}

		public unsafe static int WriteLengthPrefixedUTF8(void* destination, string str)
		{
			char* ptr = str;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			int byteCount = Encoding.UTF8.GetByteCount(str);
			int bytes = Encoding.UTF8.GetBytes(ptr, str.Length, (byte*)destination + 4, byteCount);
			Assert.Check<int, int>(byteCount == bytes, "Expected byte count mismatch {0} {1}", byteCount, bytes);
			*(int*)destination = bytes;
			return 4 + bytes;
		}

		public unsafe static int ReadLengthPrefixedUTF8(void* source, out string result)
		{
			int num = *(int*)source;
			result = Encoding.UTF8.GetString((byte*)source + 4, num);
			return num + 4;
		}

		public unsafe static bool IsPointerAligned(void* pointer, int alignment)
		{
			return pointer % alignment == null;
		}

		public unsafe static void* AlignPointer(void* pointer, int alignment)
		{
			long num = pointer;
			bool flag = num % (long)alignment != 0L;
			void* result;
			if (flag)
			{
				result = (void*)((byte*)pointer + ((long)alignment - num % (long)alignment));
			}
			else
			{
				result = pointer;
			}
			return result;
		}

		public static int RoundToMaxAlignment(int stride)
		{
			return Native.RoundToAlignment(stride, 8);
		}

		public static int WordCount(int stride, int wordSize)
		{
			return Native.RoundToAlignment(stride, wordSize) / wordSize;
		}

		public static bool IsAligned(int stride, int alignment)
		{
			return Native.RoundToAlignment(stride, alignment) == stride;
		}

		public static int RoundToAlignment(int stride, int alignment)
		{
			if (alignment <= 8)
			{
				switch (alignment)
				{
				case 1:
					return stride;
				case 2:
					return (stride + 1 >> 1) * 2;
				case 3:
					break;
				case 4:
					return (stride + 3 >> 2) * 4;
				default:
					if (alignment == 8)
					{
						return (stride + 7 >> 3) * 8;
					}
					break;
				}
			}
			else
			{
				if (alignment == 16)
				{
					return (stride + 15 >> 4) * 16;
				}
				if (alignment == 32)
				{
					return (stride + 31 >> 5) * 32;
				}
			}
			throw new InvalidOperationException(string.Format("Invalid Alignment: {0}", alignment));
		}

		public static long RoundToAlignment(long stride, int alignment)
		{
			if (alignment <= 8)
			{
				switch (alignment)
				{
				case 1:
					return stride;
				case 2:
					return (stride + 1L >> 1) * 2L;
				case 3:
					break;
				case 4:
					return (stride + 3L >> 2) * 4L;
				default:
					if (alignment == 8)
					{
						return (stride + 7L >> 3) * 8L;
					}
					break;
				}
			}
			else
			{
				if (alignment == 16)
				{
					return (stride + 15L >> 4) * 16L;
				}
				if (alignment == 32)
				{
					return (stride + 31L >> 5) * 32L;
				}
			}
			throw new InvalidOperationException(string.Format("Invalid Alignment: {0}", alignment));
		}

		public static T Empty<[IsUnmanaged] T>() where T : struct, ValueType
		{
			return Activator.CreateInstance<T>();
		}

		public static int RoundBitsUpTo64(int bits)
		{
			return (bits + 63 >> 6) * 64;
		}

		public static int RoundBitsUpTo32(int bits)
		{
			return (bits + 31 >> 5) * 32;
		}

		public static int GetAlignment<[IsUnmanaged] T>() where T : struct, ValueType
		{
			return Native.GetAlignment(sizeof(T));
		}

		public static int GetAlignment(int stride)
		{
			bool flag = stride % 16 == 0;
			int result;
			if (flag)
			{
				result = 16;
			}
			else
			{
				bool flag2 = stride % 8 == 0;
				if (flag2)
				{
					result = 8;
				}
				else
				{
					bool flag3 = stride % 4 == 0;
					if (flag3)
					{
						result = 4;
					}
					else
					{
						result = ((stride % 2 == 0) ? 2 : 1);
					}
				}
			}
			return result;
		}

		public static int GetMaxAlignment(int a, int b)
		{
			return Math.Max(Native.GetAlignment(a), Native.GetAlignment(b));
		}

		public static int GetMaxAlignment(int a, int b, int c)
		{
			return Math.Max(Native.GetMaxAlignment(a, b), Native.GetAlignment(c));
		}

		public static int GetMaxAlignment(int a, int b, int c, int d)
		{
			return Math.Max(Native.GetMaxAlignment(a, b, c), Native.GetAlignment(d));
		}

		public static int GetMaxAlignment(int a, int b, int c, int d, int e)
		{
			return Math.Max(Native.GetMaxAlignment(a, b, c, e), Native.GetAlignment(e));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static byte* ReferenceToPointer<[IsUnmanaged] T>(ref T obj) where T : struct, ValueType
		{
			fixed (T* ptr = &obj)
			{
				return (byte*)ptr;
			}
		}

		[Conditional("ENABLE_NATIVE_ALLOC_SENTINELS")]
		private unsafe static void InitBlockSentinels(IntPtr memory, int size)
		{
			ulong num = 15705636252112664309UL;
			ulong num2 = 12580085127939517179UL;
			Span<byte> dst = new Span<byte>((void*)memory, 0);
			Span<byte> dst2 = new Span<byte>((void*)((byte*)((void*)memory) + size), 0);
			new ReadOnlySpan<byte>((void*)(&num), 8).RepeatingCopyTo(dst);
			new ReadOnlySpan<byte>((void*)(&num2), 8).RepeatingCopyTo(dst2);
		}

		[Conditional("ENABLE_NATIVE_ALLOC_SENTINELS")]
		public unsafe static void ValidateBlockSentinels(IntPtr memory, int size)
		{
			ulong num = 15705636252112664309UL;
			ulong num2 = 12580085127939517179UL;
			ReadOnlySpan<byte> readOnlySpan = new ReadOnlySpan<byte>((void*)memory, 0);
			ReadOnlySpan<byte> readOnlySpan2 = new ReadOnlySpan<byte>((void*)((byte*)((void*)memory) + size), 0);
			bool flag = !readOnlySpan.RepeatingSequenceEqualTo(new ReadOnlySpan<byte>((void*)(&num), 8));
			if (flag)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log("MSG600 Leading sentinel mismatch: " + BinUtils.BytesToHex(readOnlySpan, 16));
				}
			}
			bool flag2 = !readOnlySpan2.RepeatingSequenceEqualTo(new ReadOnlySpan<byte>((void*)(&num2), 8));
			if (flag2)
			{
				LogStream logError2 = InternalLogStreams.LogError;
				if (logError2 != null)
				{
					logError2.Log("MSG601 Trailing sentinel mismatch: " + BinUtils.BytesToHex(readOnlySpan2, 16));
				}
			}
		}

		public unsafe static int MallocAndClearBlock(int size0, int size1, out void* ptr0, out void* ptr1, int alignment = 8)
		{
			size0 = Native.RoundToAlignment(size0, alignment);
			size1 = Native.RoundToAlignment(size1, alignment);
			int num = size0 + size1;
			byte* ptr2 = (byte*)Native.MallocAndClear(num);
			ptr0 = ptr2;
			ptr2 += size0;
			ptr1 = ptr2;
			Assert.Check(Native.IsPointerAligned(ptr0, alignment));
			Assert.Check(Native.IsPointerAligned(ptr1, alignment));
			return num;
		}

		public unsafe static int MallocAndClearBlock(int size0, int size1, int size2, out void* ptr0, out void* ptr1, out void* ptr2, int alignment = 8)
		{
			size0 = Native.RoundToAlignment(size0, alignment);
			size1 = Native.RoundToAlignment(size1, alignment);
			size2 = Native.RoundToAlignment(size2, alignment);
			int num = size0 + size1 + size2;
			byte* ptr3 = (byte*)Native.MallocAndClear(num);
			ptr0 = ptr3;
			ptr3 += size0;
			ptr1 = ptr3;
			ptr3 += size1;
			ptr2 = ptr3;
			Assert.Check(Native.IsPointerAligned(ptr0, alignment));
			Assert.Check(Native.IsPointerAligned(ptr1, alignment));
			Assert.Check(Native.IsPointerAligned(ptr2, alignment));
			return num;
		}

		public unsafe static int MallocAndClearBlock(int size0, int size1, int size2, int size3, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, int alignment = 8)
		{
			size0 = Native.RoundToAlignment(size0, alignment);
			size1 = Native.RoundToAlignment(size1, alignment);
			size2 = Native.RoundToAlignment(size2, alignment);
			size3 = Native.RoundToAlignment(size3, alignment);
			int num = size0 + size1 + size2 + size3;
			byte* ptr4 = (byte*)Native.MallocAndClear(num);
			ptr0 = ptr4;
			ptr4 += size0;
			ptr1 = ptr4;
			ptr4 += size1;
			ptr2 = ptr4;
			ptr4 += size2;
			ptr3 = ptr4;
			Assert.Check(Native.IsPointerAligned(ptr0, alignment));
			Assert.Check(Native.IsPointerAligned(ptr1, alignment));
			Assert.Check(Native.IsPointerAligned(ptr2, alignment));
			Assert.Check(Native.IsPointerAligned(ptr3, alignment));
			return num;
		}

		public unsafe static int MallocAndClearBlock(int size0, int size1, int size2, int size3, int size4, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, int alignment = 8)
		{
			size0 = Native.RoundToAlignment(size0, alignment);
			size1 = Native.RoundToAlignment(size1, alignment);
			size2 = Native.RoundToAlignment(size2, alignment);
			size3 = Native.RoundToAlignment(size3, alignment);
			size4 = Native.RoundToAlignment(size4, alignment);
			int num = size0 + size1 + size2 + size3 + size4;
			byte* ptr5 = (byte*)Native.MallocAndClear(num);
			ptr0 = ptr5;
			ptr5 += size0;
			ptr1 = ptr5;
			ptr5 += size1;
			ptr2 = ptr5;
			ptr5 += size2;
			ptr3 = ptr5;
			ptr5 += size3;
			ptr4 = ptr5;
			Assert.Check(Native.IsPointerAligned(ptr0, alignment));
			Assert.Check(Native.IsPointerAligned(ptr1, alignment));
			Assert.Check(Native.IsPointerAligned(ptr2, alignment));
			Assert.Check(Native.IsPointerAligned(ptr3, alignment));
			Assert.Check(Native.IsPointerAligned(ptr4, alignment));
			return num;
		}

		public unsafe static int MallocAndClearBlock(int size0, int size1, int size2, int size3, int size4, int size5, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, out void* ptr5, int alignment = 8)
		{
			size0 = Native.RoundToAlignment(size0, alignment);
			size1 = Native.RoundToAlignment(size1, alignment);
			size2 = Native.RoundToAlignment(size2, alignment);
			size3 = Native.RoundToAlignment(size3, alignment);
			size4 = Native.RoundToAlignment(size4, alignment);
			size5 = Native.RoundToAlignment(size5, alignment);
			int num = size0 + size1 + size2 + size3 + size4 + size5;
			byte* ptr6 = (byte*)Native.MallocAndClear(num);
			ptr0 = ptr6;
			ptr6 += size0;
			ptr1 = ptr6;
			ptr6 += size1;
			ptr2 = ptr6;
			ptr6 += size2;
			ptr3 = ptr6;
			ptr6 += size3;
			ptr4 = ptr6;
			ptr6 += size4;
			ptr5 = ptr6;
			Assert.Check(Native.IsPointerAligned(ptr0, alignment));
			Assert.Check(Native.IsPointerAligned(ptr1, alignment));
			Assert.Check(Native.IsPointerAligned(ptr2, alignment));
			Assert.Check(Native.IsPointerAligned(ptr3, alignment));
			Assert.Check(Native.IsPointerAligned(ptr4, alignment));
			Assert.Check(Native.IsPointerAligned(ptr5, alignment));
			return num;
		}

		public unsafe static int MallocAndClearBlock(int size0, int size1, int size2, int size3, int size4, int size5, int size6, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, out void* ptr5, out void* ptr6, int alignment = 8)
		{
			size0 = Native.RoundToAlignment(size0, alignment);
			size1 = Native.RoundToAlignment(size1, alignment);
			size2 = Native.RoundToAlignment(size2, alignment);
			size3 = Native.RoundToAlignment(size3, alignment);
			size4 = Native.RoundToAlignment(size4, alignment);
			size5 = Native.RoundToAlignment(size5, alignment);
			size6 = Native.RoundToAlignment(size6, alignment);
			int num = size0 + size1 + size2 + size3 + size4 + size5 + size6;
			byte* ptr7 = (byte*)Native.MallocAndClear(num);
			ptr0 = ptr7;
			ptr7 += size0;
			ptr1 = ptr7;
			ptr7 += size1;
			ptr2 = ptr7;
			ptr7 += size2;
			ptr3 = ptr7;
			ptr7 += size3;
			ptr4 = ptr7;
			ptr7 += size4;
			ptr5 = ptr7;
			ptr7 += size5;
			ptr6 = ptr7;
			Assert.Check(Native.IsPointerAligned(ptr0, alignment));
			Assert.Check(Native.IsPointerAligned(ptr1, alignment));
			Assert.Check(Native.IsPointerAligned(ptr2, alignment));
			Assert.Check(Native.IsPointerAligned(ptr3, alignment));
			Assert.Check(Native.IsPointerAligned(ptr4, alignment));
			Assert.Check(Native.IsPointerAligned(ptr5, alignment));
			Assert.Check(Native.IsPointerAligned(ptr6, alignment));
			return num;
		}

		public unsafe static int MallocAndClearBlock(int size0, int size1, int size2, int size3, int size4, int size5, int size6, int size7, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, out void* ptr5, out void* ptr6, out void* ptr7, int alignment = 8)
		{
			size0 = Native.RoundToAlignment(size0, alignment);
			size1 = Native.RoundToAlignment(size1, alignment);
			size2 = Native.RoundToAlignment(size2, alignment);
			size3 = Native.RoundToAlignment(size3, alignment);
			size4 = Native.RoundToAlignment(size4, alignment);
			size5 = Native.RoundToAlignment(size5, alignment);
			size6 = Native.RoundToAlignment(size6, alignment);
			size7 = Native.RoundToAlignment(size7, alignment);
			int num = size0 + size1 + size2 + size3 + size4 + size5 + size6 + size7;
			byte* ptr8 = (byte*)Native.MallocAndClear(num);
			ptr0 = ptr8;
			ptr8 += size0;
			ptr1 = ptr8;
			ptr8 += size1;
			ptr2 = ptr8;
			ptr8 += size2;
			ptr3 = ptr8;
			ptr8 += size3;
			ptr4 = ptr8;
			ptr8 += size4;
			ptr5 = ptr8;
			ptr8 += size5;
			ptr6 = ptr8;
			ptr8 += size6;
			ptr7 = ptr8;
			Assert.Check(Native.IsPointerAligned(ptr0, alignment));
			Assert.Check(Native.IsPointerAligned(ptr1, alignment));
			Assert.Check(Native.IsPointerAligned(ptr2, alignment));
			Assert.Check(Native.IsPointerAligned(ptr3, alignment));
			Assert.Check(Native.IsPointerAligned(ptr4, alignment));
			Assert.Check(Native.IsPointerAligned(ptr5, alignment));
			Assert.Check(Native.IsPointerAligned(ptr6, alignment));
			Assert.Check(Native.IsPointerAligned(ptr7, alignment));
			return num;
		}

		public unsafe static int MallocAndClearBlock(int size0, int size1, int size2, int size3, int size4, int size5, int size6, int size7, int size8, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, out void* ptr5, out void* ptr6, out void* ptr7, out void* ptr8, int alignment = 8)
		{
			size0 = Native.RoundToAlignment(size0, alignment);
			size1 = Native.RoundToAlignment(size1, alignment);
			size2 = Native.RoundToAlignment(size2, alignment);
			size3 = Native.RoundToAlignment(size3, alignment);
			size4 = Native.RoundToAlignment(size4, alignment);
			size5 = Native.RoundToAlignment(size5, alignment);
			size6 = Native.RoundToAlignment(size6, alignment);
			size7 = Native.RoundToAlignment(size7, alignment);
			size8 = Native.RoundToAlignment(size8, alignment);
			int num = size0 + size1 + size2 + size3 + size4 + size5 + size6 + size7 + size8;
			byte* ptr9 = (byte*)Native.MallocAndClear(num);
			ptr0 = ptr9;
			ptr9 += size0;
			ptr1 = ptr9;
			ptr9 += size1;
			ptr2 = ptr9;
			ptr9 += size2;
			ptr3 = ptr9;
			ptr9 += size3;
			ptr4 = ptr9;
			ptr9 += size4;
			ptr5 = ptr9;
			ptr9 += size5;
			ptr6 = ptr9;
			ptr9 += size6;
			ptr7 = ptr9;
			ptr9 += size7;
			ptr8 = ptr9;
			Assert.Check(Native.IsPointerAligned(ptr0, alignment));
			Assert.Check(Native.IsPointerAligned(ptr1, alignment));
			Assert.Check(Native.IsPointerAligned(ptr2, alignment));
			Assert.Check(Native.IsPointerAligned(ptr3, alignment));
			Assert.Check(Native.IsPointerAligned(ptr4, alignment));
			Assert.Check(Native.IsPointerAligned(ptr5, alignment));
			Assert.Check(Native.IsPointerAligned(ptr6, alignment));
			Assert.Check(Native.IsPointerAligned(ptr7, alignment));
			Assert.Check(Native.IsPointerAligned(ptr8, alignment));
			return num;
		}

		public unsafe static int MallocAndClearBlock(int size0, int size1, int size2, int size3, int size4, int size5, int size6, int size7, int size8, int size9, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, out void* ptr5, out void* ptr6, out void* ptr7, out void* ptr8, out void* ptr9, int alignment = 8)
		{
			size0 = Native.RoundToAlignment(size0, alignment);
			size1 = Native.RoundToAlignment(size1, alignment);
			size2 = Native.RoundToAlignment(size2, alignment);
			size3 = Native.RoundToAlignment(size3, alignment);
			size4 = Native.RoundToAlignment(size4, alignment);
			size5 = Native.RoundToAlignment(size5, alignment);
			size6 = Native.RoundToAlignment(size6, alignment);
			size7 = Native.RoundToAlignment(size7, alignment);
			size8 = Native.RoundToAlignment(size8, alignment);
			size9 = Native.RoundToAlignment(size9, alignment);
			int num = size0 + size1 + size2 + size3 + size4 + size5 + size6 + size7 + size8 + size9;
			byte* ptr10 = (byte*)Native.MallocAndClear(num);
			ptr0 = ptr10;
			ptr10 += size0;
			ptr1 = ptr10;
			ptr10 += size1;
			ptr2 = ptr10;
			ptr10 += size2;
			ptr3 = ptr10;
			ptr10 += size3;
			ptr4 = ptr10;
			ptr10 += size4;
			ptr5 = ptr10;
			ptr10 += size5;
			ptr6 = ptr10;
			ptr10 += size6;
			ptr7 = ptr10;
			ptr10 += size7;
			ptr8 = ptr10;
			ptr10 += size8;
			ptr9 = ptr10;
			Assert.Check(Native.IsPointerAligned(ptr0, alignment));
			Assert.Check(Native.IsPointerAligned(ptr1, alignment));
			Assert.Check(Native.IsPointerAligned(ptr2, alignment));
			Assert.Check(Native.IsPointerAligned(ptr3, alignment));
			Assert.Check(Native.IsPointerAligned(ptr4, alignment));
			Assert.Check(Native.IsPointerAligned(ptr5, alignment));
			Assert.Check(Native.IsPointerAligned(ptr6, alignment));
			Assert.Check(Native.IsPointerAligned(ptr7, alignment));
			Assert.Check(Native.IsPointerAligned(ptr8, alignment));
			Assert.Check(Native.IsPointerAligned(ptr9, alignment));
			return num;
		}

		public unsafe static int MallocAndClearBlock(int size0, int size1, int size2, int size3, int size4, int size5, int size6, int size7, int size8, int size9, int size10, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, out void* ptr5, out void* ptr6, out void* ptr7, out void* ptr8, out void* ptr9, out void* ptr10, int alignment = 8)
		{
			size0 = Native.RoundToAlignment(size0, alignment);
			size1 = Native.RoundToAlignment(size1, alignment);
			size2 = Native.RoundToAlignment(size2, alignment);
			size3 = Native.RoundToAlignment(size3, alignment);
			size4 = Native.RoundToAlignment(size4, alignment);
			size5 = Native.RoundToAlignment(size5, alignment);
			size6 = Native.RoundToAlignment(size6, alignment);
			size7 = Native.RoundToAlignment(size7, alignment);
			size8 = Native.RoundToAlignment(size8, alignment);
			size9 = Native.RoundToAlignment(size9, alignment);
			size10 = Native.RoundToAlignment(size10, alignment);
			int num = size0 + size1 + size2 + size3 + size4 + size5 + size6 + size7 + size8 + size9 + size10;
			byte* ptr11 = (byte*)Native.MallocAndClear(num);
			ptr0 = ptr11;
			ptr11 += size0;
			ptr1 = ptr11;
			ptr11 += size1;
			ptr2 = ptr11;
			ptr11 += size2;
			ptr3 = ptr11;
			ptr11 += size3;
			ptr4 = ptr11;
			ptr11 += size4;
			ptr5 = ptr11;
			ptr11 += size5;
			ptr6 = ptr11;
			ptr11 += size6;
			ptr7 = ptr11;
			ptr11 += size7;
			ptr8 = ptr11;
			ptr11 += size8;
			ptr9 = ptr11;
			ptr11 += size9;
			ptr10 = ptr11;
			Assert.Check(Native.IsPointerAligned(ptr0, alignment));
			Assert.Check(Native.IsPointerAligned(ptr1, alignment));
			Assert.Check(Native.IsPointerAligned(ptr2, alignment));
			Assert.Check(Native.IsPointerAligned(ptr3, alignment));
			Assert.Check(Native.IsPointerAligned(ptr4, alignment));
			Assert.Check(Native.IsPointerAligned(ptr5, alignment));
			Assert.Check(Native.IsPointerAligned(ptr6, alignment));
			Assert.Check(Native.IsPointerAligned(ptr7, alignment));
			Assert.Check(Native.IsPointerAligned(ptr8, alignment));
			Assert.Check(Native.IsPointerAligned(ptr9, alignment));
			Assert.Check(Native.IsPointerAligned(ptr10, alignment));
			return num;
		}

		public unsafe static int MallocAndClearBlock(int size0, int size1, int size2, int size3, int size4, int size5, int size6, int size7, int size8, int size9, int size10, int size11, out void* ptr0, out void* ptr1, out void* ptr2, out void* ptr3, out void* ptr4, out void* ptr5, out void* ptr6, out void* ptr7, out void* ptr8, out void* ptr9, out void* ptr10, out void* ptr11, int alignment = 8)
		{
			size0 = Native.RoundToAlignment(size0, alignment);
			size1 = Native.RoundToAlignment(size1, alignment);
			size2 = Native.RoundToAlignment(size2, alignment);
			size3 = Native.RoundToAlignment(size3, alignment);
			size4 = Native.RoundToAlignment(size4, alignment);
			size5 = Native.RoundToAlignment(size5, alignment);
			size6 = Native.RoundToAlignment(size6, alignment);
			size7 = Native.RoundToAlignment(size7, alignment);
			size8 = Native.RoundToAlignment(size8, alignment);
			size9 = Native.RoundToAlignment(size9, alignment);
			size10 = Native.RoundToAlignment(size10, alignment);
			size11 = Native.RoundToAlignment(size11, alignment);
			int num = size0 + size1 + size2 + size3 + size4 + size5 + size6 + size7 + size8 + size9 + size10 + size11;
			byte* ptr12 = (byte*)Native.MallocAndClear(num);
			ptr0 = ptr12;
			ptr12 += size0;
			ptr1 = ptr12;
			ptr12 += size1;
			ptr2 = ptr12;
			ptr12 += size2;
			ptr3 = ptr12;
			ptr12 += size3;
			ptr4 = ptr12;
			ptr12 += size4;
			ptr5 = ptr12;
			ptr12 += size5;
			ptr6 = ptr12;
			ptr12 += size6;
			ptr7 = ptr12;
			ptr12 += size7;
			ptr8 = ptr12;
			ptr12 += size8;
			ptr9 = ptr12;
			ptr12 += size9;
			ptr10 = ptr12;
			ptr12 += size10;
			ptr11 = ptr12;
			Assert.Check(Native.IsPointerAligned(ptr0, alignment));
			Assert.Check(Native.IsPointerAligned(ptr1, alignment));
			Assert.Check(Native.IsPointerAligned(ptr2, alignment));
			Assert.Check(Native.IsPointerAligned(ptr3, alignment));
			Assert.Check(Native.IsPointerAligned(ptr4, alignment));
			Assert.Check(Native.IsPointerAligned(ptr5, alignment));
			Assert.Check(Native.IsPointerAligned(ptr6, alignment));
			Assert.Check(Native.IsPointerAligned(ptr7, alignment));
			Assert.Check(Native.IsPointerAligned(ptr8, alignment));
			Assert.Check(Native.IsPointerAligned(ptr9, alignment));
			Assert.Check(Native.IsPointerAligned(ptr10, alignment));
			Assert.Check(Native.IsPointerAligned(ptr11, alignment));
			return num;
		}

		public const int ALIGNMENT = 8;

		public const int CACHE_LINE_SIZE = 64;

		private const int SentinelLeadingSize = 0;

		private const int SentinelTrailingSize = 0;

		private const ulong SentinelLeadingPattern = 15705636252112664309UL;

		private const ulong SentinelTrailingPattern = 12580085127939517179UL;
	}
}
