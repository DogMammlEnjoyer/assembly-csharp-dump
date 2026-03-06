using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace UnityEngine.Rendering
{
	public class ContextContainer : IDisposable
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Get<T>() where T : ContextItem, new()
		{
			uint value = ContextContainer.TypeId<T>.value;
			if (!this.Contains(value))
			{
				throw new InvalidOperationException("Type " + typeof(T).FullName + " has not been created yet.");
			}
			return (T)((object)this.m_Items[(int)value].storage);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Create<T>() where T : ContextItem, new()
		{
			uint value = ContextContainer.TypeId<T>.value;
			if (this.Contains(value))
			{
				throw new InvalidOperationException("Type " + typeof(T).FullName + " has already been created.");
			}
			return this.CreateAndGetData<T>(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetOrCreate<T>() where T : ContextItem, new()
		{
			uint value = ContextContainer.TypeId<T>.value;
			if (this.Contains(value))
			{
				return (T)((object)this.m_Items[(int)value].storage);
			}
			return this.CreateAndGetData<T>(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains<T>() where T : ContextItem, new()
		{
			uint value = ContextContainer.TypeId<T>.value;
			return this.Contains(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool Contains(uint typeId)
		{
			return (ulong)typeId < (ulong)((long)this.m_Items.Length) && this.m_Items[(int)typeId].isSet;
		}

		private T CreateAndGetData<T>(uint typeId) where T : ContextItem, new()
		{
			if ((long)this.m_Items.Length <= (long)((ulong)typeId))
			{
				ContextContainer.Item[] array = new ContextContainer.Item[math.max((long)((ulong)math.ceilpow2(ContextContainer.s_TypeCount)), (long)(this.m_Items.Length * 2))];
				for (int i = 0; i < this.m_Items.Length; i++)
				{
					array[i] = this.m_Items[i];
				}
				this.m_Items = array;
			}
			this.m_ActiveItemIndices.Add(typeId);
			ContextContainer.Item[] items = this.m_Items;
			ref ContextItem ptr = ref items[(int)typeId].storage;
			if (ptr == null)
			{
				ptr = Activator.CreateInstance<T>();
			}
			items[(int)typeId].isSet = true;
			return (T)((object)items[(int)typeId].storage);
		}

		public void Dispose()
		{
			foreach (uint num in this.m_ActiveItemIndices)
			{
				ContextContainer.Item[] items = this.m_Items;
				uint num2 = num;
				items[(int)num2].storage.Reset();
				items[(int)num2].isSet = false;
			}
			this.m_ActiveItemIndices.Clear();
		}

		private ContextContainer.Item[] m_Items = new ContextContainer.Item[64];

		private List<uint> m_ActiveItemIndices = new List<uint>();

		private static uint s_TypeCount;

		private static class TypeId<T>
		{
			public static uint value = ContextContainer.s_TypeCount++;
		}

		private struct Item
		{
			public ContextItem storage;

			public bool isSet;
		}
	}
}
