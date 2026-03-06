using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Jobs;

namespace UnityEngine.Rendering
{
	public static class ArrayExtensions
	{
		public static void ResizeArray<T>(this NativeArray<T> array, int capacity) where T : struct
		{
			NativeArray<T> nativeArray = new NativeArray<T>(capacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			if (array.IsCreated)
			{
				NativeArray<T>.Copy(array, nativeArray, array.Length);
				array.Dispose();
			}
			array = nativeArray;
		}

		public static void ResizeArray(this TransformAccessArray array, int capacity)
		{
			TransformAccessArray transformAccessArray = new TransformAccessArray(capacity, -1);
			if (array.isCreated)
			{
				for (int i = 0; i < array.length; i++)
				{
					transformAccessArray.Add(array[i]);
				}
				array.Dispose();
			}
			array = transformAccessArray;
		}

		public static void ResizeArray<T>(ref T[] array, int capacity)
		{
			if (array == null)
			{
				array = new T[capacity];
				return;
			}
			Array.Resize<T>(ref array, capacity);
		}

		public unsafe static void FillArray<[IsUnmanaged] T>(this NativeArray<T> array, in T value, int startIndex = 0, int length = -1) where T : struct, ValueType
		{
			T* unsafePtr = (T*)array.GetUnsafePtr<T>();
			int num = (length == -1) ? array.Length : (startIndex + length);
			for (int i = startIndex; i < num; i++)
			{
				unsafePtr[(IntPtr)i * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = value;
			}
		}
	}
}
