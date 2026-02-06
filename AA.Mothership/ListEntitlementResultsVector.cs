using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ListEntitlementResultsVector : IDisposable, IEnumerable, IEnumerable<MothershipEntitlementCatalogItem>
{
	internal ListEntitlementResultsVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(ListEntitlementResultsVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(ListEntitlementResultsVector obj)
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

	~ListEntitlementResultsVector()
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
					MothershipApiPINVOKE.delete_ListEntitlementResultsVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public ListEntitlementResultsVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			MothershipEntitlementCatalogItem x = (MothershipEntitlementCatalogItem)obj;
			this.Add(x);
		}
	}

	public ListEntitlementResultsVector(IEnumerable<MothershipEntitlementCatalogItem> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (MothershipEntitlementCatalogItem x in c)
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

	public MothershipEntitlementCatalogItem this[int index]
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

	public void CopyTo(MothershipEntitlementCatalogItem[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(MothershipEntitlementCatalogItem[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, MothershipEntitlementCatalogItem[] array, int arrayIndex, int count)
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

	public MothershipEntitlementCatalogItem[] ToArray()
	{
		MothershipEntitlementCatalogItem[] array = new MothershipEntitlementCatalogItem[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<MothershipEntitlementCatalogItem> IEnumerable<MothershipEntitlementCatalogItem>.GetEnumerator()
	{
		return new ListEntitlementResultsVector.ListEntitlementResultsVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new ListEntitlementResultsVector.ListEntitlementResultsVectorEnumerator(this);
	}

	public ListEntitlementResultsVector.ListEntitlementResultsVectorEnumerator GetEnumerator()
	{
		return new ListEntitlementResultsVector.ListEntitlementResultsVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.ListEntitlementResultsVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(MothershipEntitlementCatalogItem x)
	{
		MothershipApiPINVOKE.ListEntitlementResultsVector_Add(this.swigCPtr, MothershipEntitlementCatalogItem.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.ListEntitlementResultsVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.ListEntitlementResultsVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.ListEntitlementResultsVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListEntitlementResultsVector() : this(MothershipApiPINVOKE.new_ListEntitlementResultsVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListEntitlementResultsVector(ListEntitlementResultsVector other) : this(MothershipApiPINVOKE.new_ListEntitlementResultsVector__SWIG_1(ListEntitlementResultsVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListEntitlementResultsVector(int capacity) : this(MothershipApiPINVOKE.new_ListEntitlementResultsVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private MothershipEntitlementCatalogItem getitemcopy(int index)
	{
		MothershipEntitlementCatalogItem result = new MothershipEntitlementCatalogItem(MothershipApiPINVOKE.ListEntitlementResultsVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private MothershipEntitlementCatalogItem getitem(int index)
	{
		MothershipEntitlementCatalogItem result = new MothershipEntitlementCatalogItem(MothershipApiPINVOKE.ListEntitlementResultsVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, MothershipEntitlementCatalogItem val)
	{
		MothershipApiPINVOKE.ListEntitlementResultsVector_setitem(this.swigCPtr, index, MothershipEntitlementCatalogItem.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(ListEntitlementResultsVector values)
	{
		MothershipApiPINVOKE.ListEntitlementResultsVector_AddRange(this.swigCPtr, ListEntitlementResultsVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListEntitlementResultsVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ListEntitlementResultsVector_GetRange(this.swigCPtr, index, count);
		ListEntitlementResultsVector result = (intPtr == IntPtr.Zero) ? null : new ListEntitlementResultsVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, MothershipEntitlementCatalogItem x)
	{
		MothershipApiPINVOKE.ListEntitlementResultsVector_Insert(this.swigCPtr, index, MothershipEntitlementCatalogItem.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, ListEntitlementResultsVector values)
	{
		MothershipApiPINVOKE.ListEntitlementResultsVector_InsertRange(this.swigCPtr, index, ListEntitlementResultsVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.ListEntitlementResultsVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.ListEntitlementResultsVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static ListEntitlementResultsVector Repeat(MothershipEntitlementCatalogItem value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ListEntitlementResultsVector_Repeat(MothershipEntitlementCatalogItem.getCPtr(value), count);
		ListEntitlementResultsVector result = (intPtr == IntPtr.Zero) ? null : new ListEntitlementResultsVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.ListEntitlementResultsVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.ListEntitlementResultsVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, ListEntitlementResultsVector values)
	{
		MothershipApiPINVOKE.ListEntitlementResultsVector_SetRange(this.swigCPtr, index, ListEntitlementResultsVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class ListEntitlementResultsVectorEnumerator : IEnumerator, IEnumerator<MothershipEntitlementCatalogItem>, IDisposable
	{
		public ListEntitlementResultsVectorEnumerator(ListEntitlementResultsVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public MothershipEntitlementCatalogItem Current
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
				return (MothershipEntitlementCatalogItem)this.currentObject;
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

		private ListEntitlementResultsVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
