using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	public static class CoreUnsafeUtils
	{
		public unsafe static void CopyTo<T>(this List<T> list, void* dest, int count) where T : struct
		{
			int num = Mathf.Min(count, list.Count);
			for (int i = 0; i < num; i++)
			{
				UnsafeUtility.WriteArrayElement<T>(dest, i, list[i]);
			}
		}

		public unsafe static void CopyTo<T>(this T[] list, void* dest, int count) where T : struct
		{
			int num = Mathf.Min(count, list.Length);
			for (int i = 0; i < num; i++)
			{
				UnsafeUtility.WriteArrayElement<T>(dest, i, list[i]);
			}
		}

		private static void CalculateRadixParams(int radixBits, out int bitStates)
		{
			bitStates = 1 << radixBits;
		}

		private static int CalculateRadixSupportSize(int bitStates, int arrayLength)
		{
			return bitStates * 3 + arrayLength;
		}

		private unsafe static void CalculateRadixSortSupportArrays(int bitStates, int arrayLength, uint* supportArray, out uint* bucketIndices, out uint* bucketSizes, out uint* bucketPrefix, out uint* arrayOutput)
		{
			bucketIndices = supportArray;
			bucketSizes = bucketIndices + (IntPtr)bitStates * 4;
			bucketPrefix = bucketSizes + (IntPtr)bitStates * 4;
			arrayOutput = bucketPrefix + (IntPtr)bitStates * 4;
		}

		private unsafe static void MergeSort(uint* array, uint* support, int length)
		{
			for (int i = 1; i < length; i *= 2)
			{
				int num = 0;
				while (num + i < length)
				{
					int num2 = num + i;
					int num3 = num2 + i;
					if (num3 > length)
					{
						num3 = length;
					}
					int j = num;
					int k = num;
					int l = num2;
					while (k < num2)
					{
						if (l >= num3)
						{
							break;
						}
						if (array[k] <= array[l])
						{
							support[j] = array[k++];
						}
						else
						{
							support[j] = array[l++];
						}
						j++;
					}
					while (k < num2)
					{
						support[j] = array[k++];
						j++;
					}
					while (l < num3)
					{
						support[j] = array[l++];
						j++;
					}
					for (j = num; j < num3; j++)
					{
						array[j] = support[j];
					}
					num += i * 2;
				}
			}
		}

		public unsafe static void MergeSort(uint[] arr, int sortSize, ref uint[] supportArray)
		{
			sortSize = Math.Min(sortSize, arr.Length);
			if (arr == null || sortSize == 0)
			{
				return;
			}
			if (supportArray == null || supportArray.Length < sortSize)
			{
				supportArray = new uint[sortSize];
			}
			fixed (uint[] array = arr)
			{
				uint* array2;
				if (arr == null || array.Length == 0)
				{
					array2 = null;
				}
				else
				{
					array2 = &array[0];
				}
				uint[] array3;
				uint* support;
				if ((array3 = supportArray) == null || array3.Length == 0)
				{
					support = null;
				}
				else
				{
					support = &array3[0];
				}
				CoreUnsafeUtils.MergeSort(array2, support, sortSize);
				array3 = null;
			}
		}

		public unsafe static void MergeSort(NativeArray<uint> arr, int sortSize, ref NativeArray<uint> supportArray)
		{
			sortSize = Math.Min(sortSize, arr.Length);
			if (!arr.IsCreated || sortSize == 0)
			{
				return;
			}
			if (!supportArray.IsCreated || supportArray.Length < sortSize)
			{
				ref supportArray.ResizeArray(arr.Length);
			}
			CoreUnsafeUtils.MergeSort((uint*)arr.GetUnsafePtr<uint>(), (uint*)supportArray.GetUnsafePtr<uint>(), sortSize);
		}

		private unsafe static void InsertionSort(uint* arr, int length)
		{
			for (int i = 0; i < length; i++)
			{
				int num = i;
				while (num >= 1 && arr[num] < arr[num - 1])
				{
					uint num2 = arr[num];
					arr[num] = arr[num - 1];
					arr[num - 1] = num2;
					num--;
				}
			}
		}

		public unsafe static void InsertionSort(uint[] arr, int sortSize)
		{
			sortSize = Math.Min(arr.Length, sortSize);
			if (arr == null || sortSize == 0)
			{
				return;
			}
			fixed (uint[] array = arr)
			{
				uint* arr2;
				if (arr == null || array.Length == 0)
				{
					arr2 = null;
				}
				else
				{
					arr2 = &array[0];
				}
				CoreUnsafeUtils.InsertionSort(arr2, sortSize);
			}
		}

		public unsafe static void InsertionSort(NativeArray<uint> arr, int sortSize)
		{
			sortSize = Math.Min(arr.Length, sortSize);
			if (!arr.IsCreated || sortSize == 0)
			{
				return;
			}
			CoreUnsafeUtils.InsertionSort((uint*)arr.GetUnsafePtr<uint>(), sortSize);
		}

		private unsafe static void RadixSort(uint* array, uint* support, int radixBits, int bitStates, int length)
		{
			uint num = (uint)(bitStates - 1);
			uint* ptr;
			uint* ptr2;
			uint* ptr3;
			uint* ptr4;
			CoreUnsafeUtils.CalculateRadixSortSupportArrays(bitStates, length, support, out ptr, out ptr2, out ptr3, out ptr4);
			int num2 = 32 / radixBits;
			uint* ptr5 = ptr4;
			uint* ptr6 = array;
			for (int i = 0; i < num2; i++)
			{
				int num3 = i * radixBits;
				for (int j = 0; j < 3 * bitStates; j++)
				{
					ptr[j] = 0U;
				}
				for (int k = 0; k < length; k++)
				{
					ptr2[(ulong)(ptr6[k] >> num3 & num) * 4UL / 4UL] += 1U;
				}
				for (int l = 1; l < bitStates; l++)
				{
					ptr3[l] = ptr3[l - 1] + ptr2[l - 1];
				}
				for (int m = 0; m < length; m++)
				{
					uint num4 = ptr6[m];
					uint num5 = num4 >> num3 & num;
					ref int ptr7 = ref *(int*)ptr5;
					uint num6 = ptr3[(ulong)num5 * 4UL / 4UL];
					uint* ptr8 = ptr + (ulong)num5 * 4UL / 4UL;
					uint num7 = *ptr8;
					*ptr8 = num7 + 1U;
					*(ref ptr7 + (IntPtr)((ulong)(num6 + num7) * 4UL)) = (int)num4;
				}
				uint* ptr9 = ptr6;
				ptr6 = ptr5;
				ptr5 = ptr9;
			}
		}

		public unsafe static void RadixSort(uint[] arr, int sortSize, ref uint[] supportArray, int radixBits = 8)
		{
			sortSize = Math.Min(sortSize, arr.Length);
			int bitStates;
			CoreUnsafeUtils.CalculateRadixParams(radixBits, out bitStates);
			if (arr == null || sortSize == 0)
			{
				return;
			}
			int num = CoreUnsafeUtils.CalculateRadixSupportSize(bitStates, sortSize);
			if (supportArray == null || supportArray.Length < num)
			{
				supportArray = new uint[num];
			}
			fixed (uint[] array = arr)
			{
				uint* array2;
				if (arr == null || array.Length == 0)
				{
					array2 = null;
				}
				else
				{
					array2 = &array[0];
				}
				uint[] array3;
				uint* support;
				if ((array3 = supportArray) == null || array3.Length == 0)
				{
					support = null;
				}
				else
				{
					support = &array3[0];
				}
				CoreUnsafeUtils.RadixSort(array2, support, radixBits, bitStates, sortSize);
				array3 = null;
			}
		}

		public unsafe static void RadixSort(NativeArray<uint> array, int sortSize, ref NativeArray<uint> supportArray, int radixBits = 8)
		{
			sortSize = Math.Min(sortSize, array.Length);
			int bitStates;
			CoreUnsafeUtils.CalculateRadixParams(radixBits, out bitStates);
			if (!array.IsCreated || sortSize == 0)
			{
				return;
			}
			int num = CoreUnsafeUtils.CalculateRadixSupportSize(bitStates, sortSize);
			if (!supportArray.IsCreated || supportArray.Length < num)
			{
				ref supportArray.ResizeArray(num);
			}
			CoreUnsafeUtils.RadixSort((uint*)array.GetUnsafePtr<uint>(), (uint*)supportArray.GetUnsafePtr<uint>(), radixBits, bitStates, sortSize);
		}

		public unsafe static void QuickSort(uint[] arr, int left, int right)
		{
			fixed (uint[] array = arr)
			{
				uint* data;
				if (arr == null || array.Length == 0)
				{
					data = null;
				}
				else
				{
					data = &array[0];
				}
				CoreUnsafeUtils.QuickSort<uint, uint, CoreUnsafeUtils.UintKeyGetter>((void*)data, left, right);
			}
		}

		public unsafe static void QuickSort(ulong[] arr, int left, int right)
		{
			fixed (ulong[] array = arr)
			{
				ulong* data;
				if (arr == null || array.Length == 0)
				{
					data = null;
				}
				else
				{
					data = &array[0];
				}
				CoreUnsafeUtils.QuickSort<ulong, ulong, CoreUnsafeUtils.UlongKeyGetter>((void*)data, left, right);
			}
		}

		public unsafe static void QuickSort<T>(int count, void* data) where T : struct, IComparable<T>
		{
			CoreUnsafeUtils.QuickSort<T, T, CoreUnsafeUtils.DefaultKeyGetter<T>>(data, 0, count - 1);
		}

		public unsafe static void QuickSort<TValue, TKey, TGetter>(int count, void* data) where TValue : struct where TKey : struct, IComparable<TKey> where TGetter : struct, CoreUnsafeUtils.IKeyGetter<TValue, TKey>
		{
			CoreUnsafeUtils.QuickSort<TValue, TKey, TGetter>(data, 0, count - 1);
		}

		public unsafe static void QuickSort<TValue, TKey, TGetter>(void* data, int left, int right) where TValue : struct where TKey : struct, IComparable<TKey> where TGetter : struct, CoreUnsafeUtils.IKeyGetter<TValue, TKey>
		{
			if (left < right)
			{
				int num = CoreUnsafeUtils.Partition<TValue, TKey, TGetter>(data, left, right);
				if (num >= 1)
				{
					CoreUnsafeUtils.QuickSort<TValue, TKey, TGetter>(data, left, num);
				}
				if (num + 1 < right)
				{
					CoreUnsafeUtils.QuickSort<TValue, TKey, TGetter>(data, num + 1, right);
				}
			}
		}

		public unsafe static int IndexOf<T>(void* data, int count, T v) where T : struct, IEquatable<T>
		{
			for (int i = 0; i < count; i++)
			{
				T t = UnsafeUtility.ReadArrayElement<T>(data, i);
				if (t.Equals(v))
				{
					return i;
				}
			}
			return -1;
		}

		public unsafe static int CompareHashes<TOldValue, TOldGetter, TNewValue, TNewGetter>(int oldHashCount, void* oldHashes, int newHashCount, void* newHashes, int* addIndices, int* removeIndices, out int addCount, out int remCount) where TOldValue : struct where TOldGetter : struct, CoreUnsafeUtils.IKeyGetter<TOldValue, Hash128> where TNewValue : struct where TNewGetter : struct, CoreUnsafeUtils.IKeyGetter<TNewValue, Hash128>
		{
			TOldGetter toldGetter = Activator.CreateInstance<TOldGetter>();
			TNewGetter tnewGetter = Activator.CreateInstance<TNewGetter>();
			addCount = 0;
			remCount = 0;
			if (oldHashCount == newHashCount)
			{
				Hash128 hash = default(Hash128);
				Hash128 hash2 = default(Hash128);
				CoreUnsafeUtils.CombineHashes<TOldValue, TOldGetter>(oldHashCount, oldHashes, &hash);
				CoreUnsafeUtils.CombineHashes<TNewValue, TNewGetter>(newHashCount, newHashes, &hash2);
				if (hash == hash2)
				{
					return 0;
				}
			}
			int num = 0;
			int i = 0;
			int j = 0;
			while (i < oldHashCount || j < newHashCount)
			{
				if (i == oldHashCount)
				{
					while (j < newHashCount)
					{
						int num2 = addCount;
						addCount = num2 + 1;
						addIndices[num2] = j;
						num++;
						j++;
					}
				}
				else if (j == newHashCount)
				{
					while (i < oldHashCount)
					{
						int num2 = remCount;
						remCount = num2 + 1;
						removeIndices[num2] = i;
						num++;
						i++;
					}
				}
				else
				{
					TNewValue tnewValue = UnsafeUtility.ReadArrayElement<TNewValue>(newHashes, j);
					TOldValue toldValue = UnsafeUtility.ReadArrayElement<TOldValue>(oldHashes, i);
					Hash128 hash3 = tnewGetter.Get(ref tnewValue);
					Hash128 hash4 = toldGetter.Get(ref toldValue);
					if (hash3 == hash4)
					{
						j++;
						i++;
					}
					else if (hash3 < hash4)
					{
						while (j < newHashCount)
						{
							if (!(hash3 < hash4))
							{
								break;
							}
							int num2 = addCount;
							addCount = num2 + 1;
							addIndices[num2] = j;
							j++;
							num++;
							tnewValue = UnsafeUtility.ReadArrayElement<TNewValue>(newHashes, j);
							hash3 = tnewGetter.Get(ref tnewValue);
						}
					}
					else
					{
						while (i < oldHashCount && hash4 < hash3)
						{
							int num2 = remCount;
							remCount = num2 + 1;
							removeIndices[num2] = i;
							num++;
							i++;
						}
					}
				}
			}
			return num;
		}

		public unsafe static int CompareHashes(int oldHashCount, Hash128* oldHashes, int newHashCount, Hash128* newHashes, int* addIndices, int* removeIndices, out int addCount, out int remCount)
		{
			return CoreUnsafeUtils.CompareHashes<Hash128, CoreUnsafeUtils.DefaultKeyGetter<Hash128>, Hash128, CoreUnsafeUtils.DefaultKeyGetter<Hash128>>(oldHashCount, (void*)oldHashes, newHashCount, (void*)newHashes, addIndices, removeIndices, out addCount, out remCount);
		}

		public unsafe static void CombineHashes<TValue, TGetter>(int count, void* hashes, Hash128* outHash) where TValue : struct where TGetter : struct, CoreUnsafeUtils.IKeyGetter<TValue, Hash128>
		{
			TGetter tgetter = Activator.CreateInstance<TGetter>();
			for (int i = 0; i < count; i++)
			{
				TValue tvalue = UnsafeUtility.ReadArrayElement<TValue>(hashes, i);
				Hash128 hash = tgetter.Get(ref tvalue);
				HashUtilities.AppendHash(ref hash, ref *outHash);
			}
		}

		public unsafe static void CombineHashes(int count, Hash128* hashes, Hash128* outHash)
		{
			CoreUnsafeUtils.CombineHashes<Hash128, CoreUnsafeUtils.DefaultKeyGetter<Hash128>>(count, (void*)hashes, outHash);
		}

		private unsafe static int Partition<TValue, TKey, TGetter>(void* data, int left, int right) where TValue : struct where TKey : struct, IComparable<TKey> where TGetter : struct, CoreUnsafeUtils.IKeyGetter<TValue, TKey>
		{
			TGetter tgetter = default(TGetter);
			TValue tvalue = UnsafeUtility.ReadArrayElement<TValue>(data, left);
			TKey other = tgetter.Get(ref tvalue);
			left--;
			right++;
			for (;;)
			{
				TValue value = default(TValue);
				TKey tkey = default(TKey);
				int num;
				do
				{
					left++;
					value = UnsafeUtility.ReadArrayElement<TValue>(data, left);
					tkey = tgetter.Get(ref value);
					num = tkey.CompareTo(other);
				}
				while (num < 0);
				TValue value2 = default(TValue);
				TKey tkey2 = default(TKey);
				do
				{
					right--;
					value2 = UnsafeUtility.ReadArrayElement<TValue>(data, right);
					tkey2 = tgetter.Get(ref value2);
					num = tkey2.CompareTo(other);
				}
				while (num > 0);
				if (left >= right)
				{
					break;
				}
				UnsafeUtility.WriteArrayElement<TValue>(data, right, value);
				UnsafeUtility.WriteArrayElement<TValue>(data, left, value2);
			}
			return right;
		}

		public unsafe static bool HaveDuplicates(int[] arr)
		{
			int* ptr = stackalloc int[checked(unchecked((UIntPtr)arr.Length) * 4)];
			arr.CopyTo((void*)ptr, arr.Length);
			CoreUnsafeUtils.QuickSort<int>(arr.Length, (void*)ptr);
			for (int i = arr.Length - 1; i > 0; i--)
			{
				if (UnsafeUtility.ReadArrayElement<int>((void*)ptr, i).CompareTo(UnsafeUtility.ReadArrayElement<int>((void*)ptr, i - 1)) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public struct FixedBufferStringQueue
		{
			public int Count { readonly get; private set; }

			public unsafe FixedBufferStringQueue(byte* ptr, int length)
			{
				this.m_BufferStart = ptr;
				this.m_BufferLength = length;
				this.m_BufferEnd = this.m_BufferStart + this.m_BufferLength;
				this.m_ReadCursor = this.m_BufferStart;
				this.m_WriteCursor = this.m_BufferStart;
				this.Count = 0;
				this.Clear();
			}

			public unsafe bool TryPush(string v)
			{
				int num = v.Length * 2 + 4;
				if (this.m_WriteCursor + num >= this.m_BufferEnd)
				{
					return false;
				}
				*(int*)this.m_WriteCursor = v.Length;
				this.m_WriteCursor += 4;
				char* ptr = (char*)this.m_WriteCursor;
				int i = 0;
				while (i < v.Length)
				{
					*ptr = v[i];
					i++;
					ptr++;
				}
				this.m_WriteCursor += 2 * v.Length;
				int count = this.Count + 1;
				this.Count = count;
				return true;
			}

			public unsafe bool TryPop(out string v)
			{
				if (this.m_ReadCursor + 4 >= this.m_BufferEnd)
				{
					v = null;
					return false;
				}
				int num = *(int*)this.m_ReadCursor;
				if (num != 0)
				{
					this.m_ReadCursor += 4;
					v = new string((char*)this.m_ReadCursor, 0, num);
					this.m_ReadCursor += num * 2;
					return true;
				}
				v = null;
				return false;
			}

			public unsafe void Clear()
			{
				this.m_WriteCursor = this.m_BufferStart;
				this.m_ReadCursor = this.m_BufferStart;
				this.Count = 0;
				UnsafeUtility.MemClear((void*)this.m_BufferStart, (long)this.m_BufferLength);
			}

			private unsafe byte* m_ReadCursor;

			private unsafe byte* m_WriteCursor;

			private unsafe readonly byte* m_BufferEnd;

			private unsafe readonly byte* m_BufferStart;

			private readonly int m_BufferLength;
		}

		public interface IKeyGetter<TValue, TKey>
		{
			TKey Get(ref TValue v);
		}

		internal struct DefaultKeyGetter<T> : CoreUnsafeUtils.IKeyGetter<T, T>
		{
			public T Get(ref T v)
			{
				return v;
			}
		}

		internal struct UintKeyGetter : CoreUnsafeUtils.IKeyGetter<uint, uint>
		{
			public uint Get(ref uint v)
			{
				return v;
			}
		}

		internal struct UlongKeyGetter : CoreUnsafeUtils.IKeyGetter<ulong, ulong>
		{
			public ulong Get(ref ulong v)
			{
				return v;
			}
		}
	}
}
