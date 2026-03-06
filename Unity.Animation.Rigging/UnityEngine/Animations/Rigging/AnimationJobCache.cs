using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Animations.Rigging
{
	public struct AnimationJobCache : IDisposable
	{
		internal AnimationJobCache(float[] data)
		{
			this.m_Data = new NativeArray<float>(data, Allocator.Persistent);
		}

		public void Dispose()
		{
			this.m_Data.Dispose();
		}

		public float GetRaw(CacheIndex index, int offset = 0)
		{
			return this.m_Data[index.idx + offset];
		}

		public void SetRaw(float val, CacheIndex index, int offset = 0)
		{
			this.m_Data[index.idx + offset] = val;
		}

		public unsafe T Get<[IsUnmanaged] T>(CacheIndex index, int offset = 0) where T : struct, ValueType
		{
			int num = UnsafeUtility.SizeOf<T>();
			int num2 = num / UnsafeUtility.SizeOf<float>();
			T result = default(T);
			UnsafeUtility.MemCpy((void*)(&result), (void*)((byte*)((byte*)this.m_Data.GetUnsafeReadOnlyPtr<float>() + (IntPtr)index.idx * 4) + (IntPtr)(offset * num2) * 4), (long)num);
			return result;
		}

		public unsafe void Set<[IsUnmanaged] T>(T val, CacheIndex index, int offset = 0) where T : struct, ValueType
		{
			int num = UnsafeUtility.SizeOf<T>();
			int num2 = num / UnsafeUtility.SizeOf<float>();
			UnsafeUtility.MemCpy((void*)((byte*)((byte*)this.m_Data.GetUnsafePtr<float>() + (IntPtr)index.idx * 4) + (IntPtr)(offset * num2) * 4), (void*)(&val), (long)num);
		}

		public unsafe void SetArray<[IsUnmanaged] T>(T[] v, CacheIndex index, int offset = 0) where T : struct, ValueType
		{
			int num = UnsafeUtility.SizeOf<T>();
			int num2 = num / UnsafeUtility.SizeOf<float>();
			fixed (T[] array = v)
			{
				void* source;
				if (v == null || array.Length == 0)
				{
					source = null;
				}
				else
				{
					source = (void*)(&array[0]);
				}
				UnsafeUtility.MemCpy((void*)((byte*)((byte*)this.m_Data.GetUnsafePtr<float>() + (IntPtr)index.idx * 4) + (IntPtr)(offset * num2) * 4), source, (long)(num * v.Length));
			}
		}

		private NativeArray<float> m_Data;
	}
}
