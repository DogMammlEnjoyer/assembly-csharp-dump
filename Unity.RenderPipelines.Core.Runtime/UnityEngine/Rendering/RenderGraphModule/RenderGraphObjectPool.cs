using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace UnityEngine.Rendering.RenderGraphModule
{
	public sealed class RenderGraphObjectPool
	{
		internal RenderGraphObjectPool()
		{
		}

		public T[] GetTempArray<T>(int size)
		{
			Stack<object> stack;
			if (!this.m_ArrayPool.TryGetValue(new ValueTuple<Type, int>(typeof(T), size), out stack))
			{
				stack = new Stack<object>();
				this.m_ArrayPool.Add(new ValueTuple<Type, int>(typeof(T), size), stack);
			}
			T[] array = (stack.Count > 0) ? ((T[])stack.Pop()) : new T[size];
			this.m_AllocatedArrays.Add(new ValueTuple<object, ValueTuple<Type, int>>(array, new ValueTuple<Type, int>(typeof(T), size)));
			return array;
		}

		public MaterialPropertyBlock GetTempMaterialPropertyBlock()
		{
			MaterialPropertyBlock materialPropertyBlock = RenderGraphObjectPool.SharedObjectPool<MaterialPropertyBlock>.Get();
			materialPropertyBlock.Clear();
			this.m_AllocatedMaterialPropertyBlocks.Add(materialPropertyBlock);
			return materialPropertyBlock;
		}

		internal void ReleaseAllTempAlloc()
		{
			foreach (ValueTuple<object, ValueTuple<Type, int>> valueTuple in this.m_AllocatedArrays)
			{
				Stack<object> stack;
				this.m_ArrayPool.TryGetValue(valueTuple.Item2, out stack);
				stack.Push(valueTuple.Item1);
			}
			this.m_AllocatedArrays.Clear();
			foreach (MaterialPropertyBlock toRelease in this.m_AllocatedMaterialPropertyBlocks)
			{
				RenderGraphObjectPool.SharedObjectPool<MaterialPropertyBlock>.Release(toRelease);
			}
			this.m_AllocatedMaterialPropertyBlocks.Clear();
		}

		internal bool IsEmpty()
		{
			return this.m_AllocatedArrays.Count == 0 && this.m_AllocatedMaterialPropertyBlocks.Count == 0;
		}

		internal T Get<T>() where T : class, new()
		{
			return RenderGraphObjectPool.SharedObjectPool<T>.Get();
		}

		internal void Release<T>(T value) where T : class, new()
		{
			RenderGraphObjectPool.SharedObjectPool<T>.Release(value);
		}

		internal void Cleanup()
		{
			this.m_AllocatedArrays.Clear();
			this.m_AllocatedMaterialPropertyBlocks.Clear();
			this.m_ArrayPool.Clear();
			foreach (RenderGraphObjectPool.SharedObjectPoolBase ptr in RenderGraphObjectPool.s_AllocatedPools)
			{
				ptr.Clear();
			}
		}

		private static DynamicArray<RenderGraphObjectPool.SharedObjectPoolBase> s_AllocatedPools = new DynamicArray<RenderGraphObjectPool.SharedObjectPoolBase>();

		private Dictionary<ValueTuple<Type, int>, Stack<object>> m_ArrayPool = new Dictionary<ValueTuple<Type, int>, Stack<object>>();

		private List<ValueTuple<object, ValueTuple<Type, int>>> m_AllocatedArrays = new List<ValueTuple<object, ValueTuple<Type, int>>>();

		private List<MaterialPropertyBlock> m_AllocatedMaterialPropertyBlocks = new List<MaterialPropertyBlock>();

		private class SharedObjectPoolBase
		{
			public virtual void Clear()
			{
			}
		}

		private class SharedObjectPool<T> : RenderGraphObjectPool.SharedObjectPoolBase where T : class, new()
		{
			private static ObjectPool<T> AllocatePool()
			{
				ObjectPool<T> result = new ObjectPool<T>(() => Activator.CreateInstance<T>(), null, null, null, true, 10, 10000);
				DynamicArray<RenderGraphObjectPool.SharedObjectPoolBase> s_AllocatedPools = RenderGraphObjectPool.s_AllocatedPools;
				RenderGraphObjectPool.SharedObjectPoolBase sharedObjectPoolBase = new RenderGraphObjectPool.SharedObjectPool<T>();
				s_AllocatedPools.Add(sharedObjectPoolBase);
				return result;
			}

			public override void Clear()
			{
				RenderGraphObjectPool.SharedObjectPool<T>.s_Pool.Clear();
			}

			public static T Get()
			{
				return RenderGraphObjectPool.SharedObjectPool<T>.s_Pool.Get();
			}

			public static void Release(T toRelease)
			{
				RenderGraphObjectPool.SharedObjectPool<T>.s_Pool.Release(toRelease);
			}

			private static readonly ObjectPool<T> s_Pool = RenderGraphObjectPool.SharedObjectPool<T>.AllocatePool();
		}
	}
}
