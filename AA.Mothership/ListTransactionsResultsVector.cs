using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ListTransactionsResultsVector : IDisposable, IEnumerable, IEnumerable<MothershipTransactionCatalogItem>
{
	internal ListTransactionsResultsVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(ListTransactionsResultsVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(ListTransactionsResultsVector obj)
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

	~ListTransactionsResultsVector()
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
					MothershipApiPINVOKE.delete_ListTransactionsResultsVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public ListTransactionsResultsVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			MothershipTransactionCatalogItem x = (MothershipTransactionCatalogItem)obj;
			this.Add(x);
		}
	}

	public ListTransactionsResultsVector(IEnumerable<MothershipTransactionCatalogItem> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (MothershipTransactionCatalogItem x in c)
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

	public MothershipTransactionCatalogItem this[int index]
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

	public void CopyTo(MothershipTransactionCatalogItem[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(MothershipTransactionCatalogItem[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, MothershipTransactionCatalogItem[] array, int arrayIndex, int count)
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

	public MothershipTransactionCatalogItem[] ToArray()
	{
		MothershipTransactionCatalogItem[] array = new MothershipTransactionCatalogItem[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<MothershipTransactionCatalogItem> IEnumerable<MothershipTransactionCatalogItem>.GetEnumerator()
	{
		return new ListTransactionsResultsVector.ListTransactionsResultsVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new ListTransactionsResultsVector.ListTransactionsResultsVectorEnumerator(this);
	}

	public ListTransactionsResultsVector.ListTransactionsResultsVectorEnumerator GetEnumerator()
	{
		return new ListTransactionsResultsVector.ListTransactionsResultsVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.ListTransactionsResultsVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(MothershipTransactionCatalogItem x)
	{
		MothershipApiPINVOKE.ListTransactionsResultsVector_Add(this.swigCPtr, MothershipTransactionCatalogItem.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.ListTransactionsResultsVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.ListTransactionsResultsVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.ListTransactionsResultsVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListTransactionsResultsVector() : this(MothershipApiPINVOKE.new_ListTransactionsResultsVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListTransactionsResultsVector(ListTransactionsResultsVector other) : this(MothershipApiPINVOKE.new_ListTransactionsResultsVector__SWIG_1(ListTransactionsResultsVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListTransactionsResultsVector(int capacity) : this(MothershipApiPINVOKE.new_ListTransactionsResultsVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private MothershipTransactionCatalogItem getitemcopy(int index)
	{
		MothershipTransactionCatalogItem result = new MothershipTransactionCatalogItem(MothershipApiPINVOKE.ListTransactionsResultsVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private MothershipTransactionCatalogItem getitem(int index)
	{
		MothershipTransactionCatalogItem result = new MothershipTransactionCatalogItem(MothershipApiPINVOKE.ListTransactionsResultsVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, MothershipTransactionCatalogItem val)
	{
		MothershipApiPINVOKE.ListTransactionsResultsVector_setitem(this.swigCPtr, index, MothershipTransactionCatalogItem.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(ListTransactionsResultsVector values)
	{
		MothershipApiPINVOKE.ListTransactionsResultsVector_AddRange(this.swigCPtr, ListTransactionsResultsVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListTransactionsResultsVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ListTransactionsResultsVector_GetRange(this.swigCPtr, index, count);
		ListTransactionsResultsVector result = (intPtr == IntPtr.Zero) ? null : new ListTransactionsResultsVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, MothershipTransactionCatalogItem x)
	{
		MothershipApiPINVOKE.ListTransactionsResultsVector_Insert(this.swigCPtr, index, MothershipTransactionCatalogItem.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, ListTransactionsResultsVector values)
	{
		MothershipApiPINVOKE.ListTransactionsResultsVector_InsertRange(this.swigCPtr, index, ListTransactionsResultsVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.ListTransactionsResultsVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.ListTransactionsResultsVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static ListTransactionsResultsVector Repeat(MothershipTransactionCatalogItem value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ListTransactionsResultsVector_Repeat(MothershipTransactionCatalogItem.getCPtr(value), count);
		ListTransactionsResultsVector result = (intPtr == IntPtr.Zero) ? null : new ListTransactionsResultsVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.ListTransactionsResultsVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.ListTransactionsResultsVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, ListTransactionsResultsVector values)
	{
		MothershipApiPINVOKE.ListTransactionsResultsVector_SetRange(this.swigCPtr, index, ListTransactionsResultsVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class ListTransactionsResultsVectorEnumerator : IEnumerator, IEnumerator<MothershipTransactionCatalogItem>, IDisposable
	{
		public ListTransactionsResultsVectorEnumerator(ListTransactionsResultsVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public MothershipTransactionCatalogItem Current
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
				return (MothershipTransactionCatalogItem)this.currentObject;
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

		private ListTransactionsResultsVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
