using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

internal struct OVRNativeList<[IsUnmanaged] T> : IDisposable, IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> where T : struct, ValueType
{
	public int Count { readonly get; private set; }

	public int Capacity
	{
		get
		{
			return this._array.Length;
		}
	}

	public bool IsCreated
	{
		get
		{
			return this._array.IsCreated;
		}
	}

	public OVRNativeList(int? initialCapacity, Allocator allocator)
	{
		this._array = ((initialCapacity != null) ? new NativeArray<T>(initialCapacity.Value, allocator, NativeArrayOptions.ClearMemory) : default(NativeArray<T>));
		this._allocator = allocator;
		this.Count = 0;
	}

	public OVRNativeList(Allocator allocator)
	{
		this._array = default(NativeArray<T>);
		this._allocator = allocator;
		this.Count = 0;
	}

	public unsafe T* PtrToElementAt(int index)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", index, "index must be greater than or equal to zero.");
		}
		if (index >= this.Count)
		{
			throw new ArgumentOutOfRangeException("index", index, string.Format("{0} must be less than {1} ({2}).", "index", "Count", this.Count));
		}
		return this.Data + (IntPtr)index * (IntPtr)sizeof(T) / (IntPtr)sizeof(T);
	}

	public unsafe T* Data
	{
		get
		{
			if (!this.IsCreated)
			{
				return null;
			}
			return (T*)this._array.GetUnsafePtr<T>();
		}
	}

	public NativeArray<T> AsNativeArray()
	{
		return this._array.GetSubArray(0, this.Count);
	}

	public unsafe Span<T> AsSpan()
	{
		return new Span<T>((void*)this.Data, this.Count);
	}

	public unsafe ReadOnlySpan<T> AsReadOnlySpan()
	{
		return new ReadOnlySpan<T>((void*)this.Data, this.Count);
	}

	public NativeArray<T>.Enumerator GetEnumerator()
	{
		return this.AsNativeArray().GetEnumerator();
	}

	public void Add(T item)
	{
		this.EnsureCapacity(this.Count + 1);
		int count = this.Count;
		this.Count = count + 1;
		this._array[count] = item;
	}

	public unsafe void AddRange(IEnumerable<T> collection)
	{
		OVREnumerable<T> ovrenumerable = collection.ToNonAlloc<T>();
		int num;
		if (ovrenumerable.TryGetCount(out num))
		{
			this.EnsureCapacity(this.Count + num);
			T[] array = collection as T[];
			if (array != null)
			{
				T[] array2;
				T* source;
				if ((array2 = array) == null || array2.Length == 0)
				{
					source = null;
				}
				else
				{
					source = &array2[0];
				}
				UnsafeUtility.MemCpy((void*)(this.Data + (IntPtr)this.Count * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)), (void*)source, (long)(sizeof(T) * array.Length));
				array2 = null;
				this.Count += num;
				return;
			}
		}
		foreach (T item in ovrenumerable)
		{
			this.Add(item);
		}
	}

	public void Clear()
	{
		this.Count = 0;
	}

	public T this[int index]
	{
		get
		{
			if (index >= this.Count)
			{
				throw new IndexOutOfRangeException(string.Format("{0} ({1}) must be less than {2} ({3}).", new object[]
				{
					"index",
					index,
					"Count",
					this.Count
				}));
			}
			return this._array[index];
		}
		set
		{
			if (index >= this.Count)
			{
				throw new IndexOutOfRangeException(string.Format("{0} ({1}) must be less than {2} ({3}).", new object[]
				{
					"index",
					index,
					"Count",
					this.Count
				}));
			}
			this._array[index] = value;
		}
	}

	public void Dispose()
	{
		if (this._array.IsCreated)
		{
			this._array.Dispose();
			this._array = default(NativeArray<T>);
		}
		this.Count = 0;
	}

	public JobHandle Dispose(JobHandle dependency)
	{
		this.Count = 0;
		return this._array.Dispose(dependency);
	}

	public unsafe static implicit operator T*(OVRNativeList<T> list)
	{
		return list.Data;
	}

	public static implicit operator Span<T>(OVRNativeList<T> list)
	{
		return list.AsSpan();
	}

	public static implicit operator ReadOnlySpan<T>(OVRNativeList<T> list)
	{
		return list.AsReadOnlySpan();
	}

	IEnumerator<T> IEnumerable<!0>.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	private void EnsureCapacity(int capacity)
	{
		if (this.Capacity >= capacity)
		{
			return;
		}
		capacity = Math.Max(capacity, Math.Max(4, this.Capacity * 3 / 2));
		NativeArray<T> nativeArray = new NativeArray<T>(capacity, this._allocator, NativeArrayOptions.ClearMemory);
		if (this._array.IsCreated)
		{
			UnsafeUtility.MemCpy(nativeArray.GetUnsafePtr<T>(), this._array.GetUnsafeReadOnlyPtr<T>(), (long)(sizeof(T) * this.Count));
			this._array.Dispose();
		}
		this._array = nativeArray;
	}

	private NativeArray<T> _array;

	private readonly Allocator _allocator;
}
