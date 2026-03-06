using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace UnityEngine.UIElements.Layout
{
	internal struct LayoutDataStore : IDisposable
	{
		public bool IsValid
		{
			get
			{
				return null != this.m_Data;
			}
		}

		public unsafe int Capacity
		{
			get
			{
				return this.m_Data->Capacity;
			}
		}

		public unsafe LayoutDataStore(ComponentType[] components, int initialCapacity, Allocator allocator)
		{
			Assert.IsTrue(components.Length != 0, "LayoutDataStore requires at least one component size.");
			Assert.IsTrue(components[0].Size >= 4, string.Format("{0} requires a minimum element size of {1} to alias", "LayoutDataStore", 4));
			this.m_Allocator = allocator;
			this.m_Data = (LayoutDataStore.Data*)UnsafeUtility.Malloc((long)UnsafeUtility.SizeOf<LayoutDataStore.Data>(), UnsafeUtility.AlignOf<LayoutDataStore.Data>(), this.m_Allocator);
			UnsafeUtility.MemClear((void*)this.m_Data, (long)UnsafeUtility.SizeOf<LayoutDataStore.Data>());
			this.m_Data->ComponentCount = components.Length;
			this.m_Data->Components = (LayoutDataStore.ComponentDataStore*)UnsafeUtility.Malloc((long)(UnsafeUtility.SizeOf<LayoutDataStore.ComponentDataStore>() * components.Length), UnsafeUtility.AlignOf<LayoutDataStore.ComponentDataStore>(), allocator);
			for (int i = 0; i < components.Length; i++)
			{
				this.m_Data->Components[i] = new LayoutDataStore.ComponentDataStore(components[i].Size, allocator);
			}
			this.ResizeCapacity(initialCapacity);
			this.m_Data->NextFreeIndex = 0;
		}

		public unsafe void Dispose()
		{
			for (int i = 0; i < this.m_Data->ComponentCount; i++)
			{
				this.m_Data->Components[i].Dispose();
			}
			UnsafeUtility.Free((void*)this.m_Data->Versions, this.m_Allocator);
			UnsafeUtility.Free((void*)this.m_Data->Components, this.m_Allocator);
			UnsafeUtility.Free((void*)this.m_Data, this.m_Allocator);
			this.m_Data = null;
		}

		public unsafe bool Exists(in LayoutHandle handle)
		{
			bool flag = (ulong)handle.Index >= (ulong)((long)this.m_Data->Capacity);
			return !flag && this.m_Data->Versions[handle.Index] == handle.Version;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe readonly void* GetComponentDataPtr(int index, int componentIndex)
		{
			return (void*)this.m_Data->Components[componentIndex].GetComponentDataPtr(index);
		}

		private unsafe LayoutHandle Allocate(byte** data, int count)
		{
			int nextFreeIndex = this.m_Data->NextFreeIndex;
			int nextFreeIndex2 = LayoutDataStore.GetNextFreeIndex(this.m_Data->Components, nextFreeIndex);
			bool flag = nextFreeIndex2 == -1;
			if (flag)
			{
				this.IncreaseCapacity();
				nextFreeIndex2 = LayoutDataStore.GetNextFreeIndex(this.m_Data->Components, nextFreeIndex);
			}
			int version = this.m_Data->Versions[nextFreeIndex];
			this.m_Data->NextFreeIndex = nextFreeIndex2;
			Debug.Assert(this.m_Data->ComponentCount == count, "All components must be initialized");
			Debug.Assert(data != null);
			for (int i = 0; i < count; i++)
			{
				Debug.Assert(*(IntPtr*)(data + (IntPtr)i * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) != (IntPtr)((UIntPtr)0));
				byte* componentDataPtr = this.m_Data->Components[i].GetComponentDataPtr(nextFreeIndex);
				UnsafeUtility.MemCpy((void*)componentDataPtr, *(IntPtr*)(data + (IntPtr)i * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)), (long)this.m_Data->Components[i].Size);
			}
			return new LayoutHandle(nextFreeIndex, version);
		}

		public unsafe void Free(in LayoutHandle handle)
		{
			bool flag = !this.Exists(handle);
			if (flag)
			{
				throw new InvalidOperationException(string.Format("Failed to Free handle with Index={0} Version={1}", handle.Index, handle.Version));
			}
			this.m_Data->Versions[handle.Index]++;
			LayoutDataStore.SetNextFreeIndex(this.m_Data->Components, handle.Index, this.m_Data->NextFreeIndex);
			this.m_Data->NextFreeIndex = handle.Index;
		}

		private unsafe static void SetNextFreeIndex(LayoutDataStore.ComponentDataStore* ptr, int index, int value)
		{
			*(int*)ptr->GetComponentDataPtr(index) = value;
		}

		private unsafe static int GetNextFreeIndex(LayoutDataStore.ComponentDataStore* ptr, int index)
		{
			return *(int*)ptr->GetComponentDataPtr(index);
		}

		private unsafe void IncreaseCapacity()
		{
			this.ResizeCapacity((int)((float)this.m_Data->Capacity * 1.5f));
		}

		private unsafe void ResizeCapacity(int capacity)
		{
			Assert.IsTrue(capacity > 0);
			this.m_Data->Versions = (int*)LayoutDataStore.ResizeArray((void*)this.m_Data->Versions, (long)this.m_Data->Capacity, (long)capacity, 4L, 4, this.m_Allocator);
			for (int i = 0; i < this.m_Data->ComponentCount; i++)
			{
				this.m_Data->Components[i].ResizeCapacity(capacity);
			}
			int num = (this.m_Data->Capacity > 0) ? (this.m_Data->Capacity - 1) : 0;
			for (int j = num; j < capacity; j++)
			{
				this.m_Data->Versions[j] = 1;
				LayoutDataStore.SetNextFreeIndex(this.m_Data->Components, j, j + 1);
			}
			LayoutDataStore.SetNextFreeIndex(this.m_Data->Components, capacity - 1, -1);
			this.m_Data->Capacity = capacity;
		}

		private unsafe static void* ResizeArray(void* fromPtr, long fromCount, long toCount, long size, int align, Allocator allocator)
		{
			Assert.IsTrue(toCount > 0L);
			void* ptr = UnsafeUtility.Malloc(size * toCount, align, allocator);
			Assert.IsTrue(ptr != null);
			bool flag = fromCount <= 0L;
			void* result;
			if (flag)
			{
				result = ptr;
			}
			else
			{
				long num = (toCount < fromCount) ? toCount : fromCount;
				long size2 = num * size;
				UnsafeUtility.MemCpy(ptr, fromPtr, size2);
				UnsafeUtility.Free(fromPtr, allocator);
				result = ptr;
			}
			return result;
		}

		public unsafe LayoutHandle Allocate<[IsUnmanaged] T0>(in T0 component0) where T0 : struct, ValueType
		{
			fixed (T0* ptr = &component0)
			{
				T0* ptr2 = ptr;
				byte** ptr3 = stackalloc byte*[checked(unchecked((UIntPtr)1) * (UIntPtr)sizeof(byte*))];
				*(IntPtr*)ptr3 = ptr2;
				return this.Allocate(ptr3, 1);
			}
		}

		public unsafe LayoutHandle Allocate<[IsUnmanaged] T0, [IsUnmanaged] T1, [IsUnmanaged] T2>(in T0 component0, in T1 component1, in T2 component2) where T0 : struct, ValueType where T1 : struct, ValueType where T2 : struct, ValueType
		{
			fixed (T0* ptr = &component0)
			{
				T0* ptr2 = ptr;
				fixed (T1* ptr3 = &component1)
				{
					T1* ptr4 = ptr3;
					fixed (T2* ptr5 = &component2)
					{
						T2* ptr6 = ptr5;
						byte** ptr7 = stackalloc byte*[checked(unchecked((UIntPtr)3) * (UIntPtr)sizeof(byte*))];
						*(IntPtr*)ptr7 = ptr2;
						*(IntPtr*)(ptr7 + sizeof(byte*) / sizeof(byte*)) = ptr4;
						*(IntPtr*)(ptr7 + (IntPtr)2 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr6;
						return this.Allocate(ptr7, 3);
					}
				}
			}
		}

		public unsafe LayoutHandle Allocate<[IsUnmanaged] T0, [IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3>(in T0 component0, in T1 component1, in T2 component2, in T3 component3) where T0 : struct, ValueType where T1 : struct, ValueType where T2 : struct, ValueType where T3 : struct, ValueType
		{
			fixed (T0* ptr = &component0)
			{
				T0* ptr2 = ptr;
				fixed (T1* ptr3 = &component1)
				{
					T1* ptr4 = ptr3;
					fixed (T2* ptr5 = &component2)
					{
						T2* ptr6 = ptr5;
						fixed (T3* ptr7 = &component3)
						{
							T3* ptr8 = ptr7;
							byte** ptr9 = stackalloc byte*[checked(unchecked((UIntPtr)4) * (UIntPtr)sizeof(byte*))];
							*(IntPtr*)ptr9 = ptr2;
							*(IntPtr*)(ptr9 + sizeof(byte*) / sizeof(byte*)) = ptr4;
							*(IntPtr*)(ptr9 + (IntPtr)2 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr6;
							*(IntPtr*)(ptr9 + (IntPtr)3 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr8;
							return this.Allocate(ptr9, 4);
						}
					}
				}
			}
		}

		public unsafe LayoutHandle Allocate<[IsUnmanaged] T0, [IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3, [IsUnmanaged] T4>(in T0 component0, in T1 component1, in T2 component2, in T3 component3, in T4 component4) where T0 : struct, ValueType where T1 : struct, ValueType where T2 : struct, ValueType where T3 : struct, ValueType where T4 : struct, ValueType
		{
			fixed (T0* ptr = &component0)
			{
				T0* ptr2 = ptr;
				fixed (T1* ptr3 = &component1)
				{
					T1* ptr4 = ptr3;
					fixed (T2* ptr5 = &component2)
					{
						T2* ptr6 = ptr5;
						fixed (T3* ptr7 = &component3)
						{
							T3* ptr8 = ptr7;
							fixed (T4* ptr9 = &component4)
							{
								T4* ptr10 = ptr9;
								byte** ptr11 = stackalloc byte*[checked(unchecked((UIntPtr)5) * (UIntPtr)sizeof(byte*))];
								*(IntPtr*)ptr11 = ptr2;
								*(IntPtr*)(ptr11 + sizeof(byte*) / sizeof(byte*)) = ptr4;
								*(IntPtr*)(ptr11 + (IntPtr)2 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr6;
								*(IntPtr*)(ptr11 + (IntPtr)3 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr8;
								*(IntPtr*)(ptr11 + (IntPtr)4 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr10;
								return this.Allocate(ptr11, 5);
							}
						}
					}
				}
			}
		}

		public unsafe LayoutHandle Allocate<[IsUnmanaged] T0, [IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3, [IsUnmanaged] T4, [IsUnmanaged] T5>(in T0 component0, in T1 component1, in T2 component2, in T3 component3, in T4 component4, in T5 component5) where T0 : struct, ValueType where T1 : struct, ValueType where T2 : struct, ValueType where T3 : struct, ValueType where T4 : struct, ValueType where T5 : struct, ValueType
		{
			fixed (T0* ptr = &component0)
			{
				T0* ptr2 = ptr;
				fixed (T1* ptr3 = &component1)
				{
					T1* ptr4 = ptr3;
					fixed (T2* ptr5 = &component2)
					{
						T2* ptr6 = ptr5;
						fixed (T3* ptr7 = &component3)
						{
							T3* ptr8 = ptr7;
							fixed (T4* ptr9 = &component4)
							{
								T4* ptr10 = ptr9;
								fixed (T5* ptr11 = &component5)
								{
									T5* ptr12 = ptr11;
									byte** ptr13 = stackalloc byte*[checked(unchecked((UIntPtr)6) * (UIntPtr)sizeof(byte*))];
									*(IntPtr*)ptr13 = ptr2;
									*(IntPtr*)(ptr13 + sizeof(byte*) / sizeof(byte*)) = ptr4;
									*(IntPtr*)(ptr13 + (IntPtr)2 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr6;
									*(IntPtr*)(ptr13 + (IntPtr)3 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr8;
									*(IntPtr*)(ptr13 + (IntPtr)4 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr10;
									*(IntPtr*)(ptr13 + (IntPtr)5 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr12;
									return this.Allocate(ptr13, 6);
								}
							}
						}
					}
				}
			}
		}

		public unsafe LayoutHandle Allocate<[IsUnmanaged] T0, [IsUnmanaged] T1, [IsUnmanaged] T2, [IsUnmanaged] T3, [IsUnmanaged] T4, [IsUnmanaged] T5, [IsUnmanaged] T6>(in T0 component0, in T1 component1, in T2 component2, in T3 component3, in T4 component4, in T5 component5, in T6 component6) where T0 : struct, ValueType where T1 : struct, ValueType where T2 : struct, ValueType where T3 : struct, ValueType where T4 : struct, ValueType where T5 : struct, ValueType where T6 : struct, ValueType
		{
			fixed (T0* ptr = &component0)
			{
				T0* ptr2 = ptr;
				fixed (T1* ptr3 = &component1)
				{
					T1* ptr4 = ptr3;
					fixed (T2* ptr5 = &component2)
					{
						T2* ptr6 = ptr5;
						fixed (T3* ptr7 = &component3)
						{
							T3* ptr8 = ptr7;
							fixed (T4* ptr9 = &component4)
							{
								T4* ptr10 = ptr9;
								fixed (T5* ptr11 = &component5)
								{
									T5* ptr12 = ptr11;
									fixed (T6* ptr13 = &component6)
									{
										T6* ptr14 = ptr13;
										byte** ptr15 = stackalloc byte*[checked(unchecked((UIntPtr)7) * (UIntPtr)sizeof(byte*))];
										*(IntPtr*)ptr15 = ptr2;
										*(IntPtr*)(ptr15 + sizeof(byte*) / sizeof(byte*)) = ptr4;
										*(IntPtr*)(ptr15 + (IntPtr)2 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr6;
										*(IntPtr*)(ptr15 + (IntPtr)3 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr8;
										*(IntPtr*)(ptr15 + (IntPtr)4 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr10;
										*(IntPtr*)(ptr15 + (IntPtr)5 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr12;
										*(IntPtr*)(ptr15 + (IntPtr)6 * (IntPtr)sizeof(byte*) / (IntPtr)sizeof(byte*)) = ptr14;
										return this.Allocate(ptr15, 7);
									}
								}
							}
						}
					}
				}
			}
		}

		private const int k_ChunkSize = 32768;

		private readonly Allocator m_Allocator;

		[NativeDisableUnsafePtrRestriction]
		private unsafe LayoutDataStore.Data* m_Data;

		private struct Chunk
		{
			[NativeDisableUnsafePtrRestriction]
			public unsafe byte* Buffer;
		}

		private struct ComponentDataStore : IDisposable
		{
			public ComponentDataStore(int size, Allocator allocator)
			{
				this.Allocator = allocator;
				this.Size = size;
				this.ComponentCountPerChunk = 32768 / size;
				this.ChunkCount = 0;
				this.m_Chunks = null;
			}

			public unsafe void Dispose()
			{
				bool flag = null == this.m_Chunks;
				if (!flag)
				{
					for (int i = 0; i < this.ChunkCount; i++)
					{
						UnsafeUtility.Free((void*)this.m_Chunks[i].Buffer, this.Allocator);
					}
					UnsafeUtility.Free((void*)this.m_Chunks, this.Allocator);
					this.ChunkCount = 0;
					this.m_Chunks = null;
				}
			}

			public unsafe byte* GetComponentDataPtr(int index)
			{
				int num = index / this.ComponentCountPerChunk;
				int num2 = index % this.ComponentCountPerChunk;
				return this.m_Chunks[num].Buffer + num2 * this.Size;
			}

			public unsafe void ResizeCapacity(int capacity)
			{
				int num = capacity / this.ComponentCountPerChunk + 1;
				bool flag = num > this.ChunkCount;
				if (flag)
				{
					this.m_Chunks = (LayoutDataStore.Chunk*)LayoutDataStore.ResizeArray((void*)this.m_Chunks, (long)this.ChunkCount, (long)num, (long)UnsafeUtility.SizeOf<LayoutDataStore.Chunk>(), UnsafeUtility.AlignOf<LayoutDataStore.Chunk>(), this.Allocator);
					for (int i = this.ChunkCount; i < num; i++)
					{
						this.m_Chunks[i] = new LayoutDataStore.Chunk
						{
							Buffer = (byte*)UnsafeUtility.Malloc(32768L, 4, this.Allocator)
						};
					}
				}
				else
				{
					bool flag2 = num < this.ChunkCount;
					if (flag2)
					{
						for (int j = this.ChunkCount - 1; j >= num; j--)
						{
							UnsafeUtility.Free((void*)this.m_Chunks[j].Buffer, this.Allocator);
						}
						this.m_Chunks = (LayoutDataStore.Chunk*)LayoutDataStore.ResizeArray((void*)this.m_Chunks, (long)this.ChunkCount, (long)num, (long)UnsafeUtility.SizeOf<LayoutDataStore.Chunk>(), UnsafeUtility.AlignOf<LayoutDataStore.Chunk>(), this.Allocator);
					}
				}
				this.ChunkCount = num;
			}

			public Allocator Allocator;

			public int Size;

			public int ComponentCountPerChunk;

			public int ChunkCount;

			[NativeDisableUnsafePtrRestriction]
			private unsafe LayoutDataStore.Chunk* m_Chunks;
		}

		private struct Data
		{
			public int Capacity;

			public int NextFreeIndex;

			public int ComponentCount;

			[NativeDisableUnsafePtrRestriction]
			public unsafe int* Versions;

			[NativeDisableUnsafePtrRestriction]
			public unsafe LayoutDataStore.ComponentDataStore* Components;
		}
	}
}
