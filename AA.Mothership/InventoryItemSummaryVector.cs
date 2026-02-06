using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class InventoryItemSummaryVector : IDisposable, IEnumerable, IEnumerable<MothershipInventoryItemSummary>
{
	internal InventoryItemSummaryVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(InventoryItemSummaryVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(InventoryItemSummaryVector obj)
	{
		if (obj == null)
		{
			return new HandleRef(null, IntPtr.Zero);
		}
		if (!obj.swigCMemOwn)
		{
			throw new ApplicationException("Cannot release ownership as memory is not owned");
		}
		HandleRef result = obj.swigCPtr;
		obj.swigCMemOwn = false;
		obj.Dispose();
		return result;
	}

	~InventoryItemSummaryVector()
	{
		this.Dispose(false);
	}

	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero)
			{
				if (this.swigCMemOwn)
				{
					this.swigCMemOwn = false;
					MothershipApiPINVOKE.delete_InventoryItemSummaryVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public InventoryItemSummaryVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			MothershipInventoryItemSummary x = (MothershipInventoryItemSummary)obj;
			this.Add(x);
		}
	}

	public InventoryItemSummaryVector(IEnumerable<MothershipInventoryItemSummary> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (MothershipInventoryItemSummary x in c)
		{
			this.Add(x);
		}
	}

	public bool IsFixedSize
	{
		get
		{
			return false;
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return false;
		}
	}

	public MothershipInventoryItemSummary this[int index]
	{
		get
		{
			return this.getitem(index);
		}
		set
		{
			this.setitem(index, value);
		}
	}

	public int Capacity
	{
		get
		{
			return (int)this.capacity();
		}
		set
		{
			if (value < 0 || value < (int)this.size())
			{
				throw new ArgumentOutOfRangeException("Capacity");
			}
			this.reserve((uint)value);
		}
	}

	public int Count
	{
		get
		{
			return (int)this.size();
		}
	}

	public bool IsSynchronized
	{
		get
		{
			return false;
		}
	}

	public void CopyTo(MothershipInventoryItemSummary[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(MothershipInventoryItemSummary[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, MothershipInventoryItemSummary[] array, int arrayIndex, int count)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException("index", "Value is less than zero");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Value is less than zero");
		}
		if (count < 0)
		{
			throw new ArgumentOutOfRangeException("count", "Value is less than zero");
		}
		if (array.Rank > 1)
		{
			throw new ArgumentException("Multi dimensional array.", "array");
		}
		if (index + count > this.Count || arrayIndex + count > array.Length)
		{
			throw new ArgumentException("Number of elements to copy is too large.");
		}
		for (int i = 0; i < count; i++)
		{
			array.SetValue(this.getitemcopy(index + i), arrayIndex + i);
		}
	}

	public MothershipInventoryItemSummary[] ToArray()
	{
		MothershipInventoryItemSummary[] array = new MothershipInventoryItemSummary[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<MothershipInventoryItemSummary> IEnumerable<MothershipInventoryItemSummary>.GetEnumerator()
	{
		return new InventoryItemSummaryVector.InventoryItemSummaryVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new InventoryItemSummaryVector.InventoryItemSummaryVectorEnumerator(this);
	}

	public InventoryItemSummaryVector.InventoryItemSummaryVectorEnumerator GetEnumerator()
	{
		return new InventoryItemSummaryVector.InventoryItemSummaryVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.InventoryItemSummaryVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(MothershipInventoryItemSummary x)
	{
		MothershipApiPINVOKE.InventoryItemSummaryVector_Add(this.swigCPtr, MothershipInventoryItemSummary.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.InventoryItemSummaryVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.InventoryItemSummaryVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.InventoryItemSummaryVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public InventoryItemSummaryVector() : this(MothershipApiPINVOKE.new_InventoryItemSummaryVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public InventoryItemSummaryVector(InventoryItemSummaryVector other) : this(MothershipApiPINVOKE.new_InventoryItemSummaryVector__SWIG_1(InventoryItemSummaryVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public InventoryItemSummaryVector(int capacity) : this(MothershipApiPINVOKE.new_InventoryItemSummaryVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private MothershipInventoryItemSummary getitemcopy(int index)
	{
		MothershipInventoryItemSummary result = new MothershipInventoryItemSummary(MothershipApiPINVOKE.InventoryItemSummaryVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private MothershipInventoryItemSummary getitem(int index)
	{
		MothershipInventoryItemSummary result = new MothershipInventoryItemSummary(MothershipApiPINVOKE.InventoryItemSummaryVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, MothershipInventoryItemSummary val)
	{
		MothershipApiPINVOKE.InventoryItemSummaryVector_setitem(this.swigCPtr, index, MothershipInventoryItemSummary.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(InventoryItemSummaryVector values)
	{
		MothershipApiPINVOKE.InventoryItemSummaryVector_AddRange(this.swigCPtr, InventoryItemSummaryVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public InventoryItemSummaryVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.InventoryItemSummaryVector_GetRange(this.swigCPtr, index, count);
		InventoryItemSummaryVector result = (intPtr == IntPtr.Zero) ? null : new InventoryItemSummaryVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, MothershipInventoryItemSummary x)
	{
		MothershipApiPINVOKE.InventoryItemSummaryVector_Insert(this.swigCPtr, index, MothershipInventoryItemSummary.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, InventoryItemSummaryVector values)
	{
		MothershipApiPINVOKE.InventoryItemSummaryVector_InsertRange(this.swigCPtr, index, InventoryItemSummaryVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.InventoryItemSummaryVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.InventoryItemSummaryVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static InventoryItemSummaryVector Repeat(MothershipInventoryItemSummary value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.InventoryItemSummaryVector_Repeat(MothershipInventoryItemSummary.getCPtr(value), count);
		InventoryItemSummaryVector result = (intPtr == IntPtr.Zero) ? null : new InventoryItemSummaryVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.InventoryItemSummaryVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.InventoryItemSummaryVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, InventoryItemSummaryVector values)
	{
		MothershipApiPINVOKE.InventoryItemSummaryVector_SetRange(this.swigCPtr, index, InventoryItemSummaryVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class InventoryItemSummaryVectorEnumerator : IEnumerator, IEnumerator<MothershipInventoryItemSummary>, IDisposable
	{
		public InventoryItemSummaryVectorEnumerator(InventoryItemSummaryVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public MothershipInventoryItemSummary Current
		{
			get
			{
				if (this.currentIndex == -1)
				{
					throw new InvalidOperationException("Enumeration not started.");
				}
				if (this.currentIndex > this.currentSize - 1)
				{
					throw new InvalidOperationException("Enumeration finished.");
				}
				if (this.currentObject == null)
				{
					throw new InvalidOperationException("Collection modified.");
				}
				return (MothershipInventoryItemSummary)this.currentObject;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this.Current;
			}
		}

		public bool MoveNext()
		{
			int count = this.collectionRef.Count;
			bool flag = this.currentIndex + 1 < count && count == this.currentSize;
			if (flag)
			{
				this.currentIndex++;
				this.currentObject = this.collectionRef[this.currentIndex];
				return flag;
			}
			this.currentObject = null;
			return flag;
		}

		public void Reset()
		{
			this.currentIndex = -1;
			this.currentObject = null;
			if (this.collectionRef.Count != this.currentSize)
			{
				throw new InvalidOperationException("Collection modified.");
			}
		}

		public void Dispose()
		{
			this.currentIndex = -1;
			this.currentObject = null;
		}

		private InventoryItemSummaryVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
