using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class OfferCatalogItemVector : IDisposable, IEnumerable, IEnumerable<OfferCatalogItem>
{
	internal OfferCatalogItemVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(OfferCatalogItemVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(OfferCatalogItemVector obj)
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

	~OfferCatalogItemVector()
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
					MothershipApiPINVOKE.delete_OfferCatalogItemVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public OfferCatalogItemVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			OfferCatalogItem x = (OfferCatalogItem)obj;
			this.Add(x);
		}
	}

	public OfferCatalogItemVector(IEnumerable<OfferCatalogItem> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (OfferCatalogItem x in c)
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

	public OfferCatalogItem this[int index]
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

	public void CopyTo(OfferCatalogItem[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(OfferCatalogItem[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, OfferCatalogItem[] array, int arrayIndex, int count)
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

	public OfferCatalogItem[] ToArray()
	{
		OfferCatalogItem[] array = new OfferCatalogItem[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<OfferCatalogItem> IEnumerable<OfferCatalogItem>.GetEnumerator()
	{
		return new OfferCatalogItemVector.OfferCatalogItemVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new OfferCatalogItemVector.OfferCatalogItemVectorEnumerator(this);
	}

	public OfferCatalogItemVector.OfferCatalogItemVectorEnumerator GetEnumerator()
	{
		return new OfferCatalogItemVector.OfferCatalogItemVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.OfferCatalogItemVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(OfferCatalogItem x)
	{
		MothershipApiPINVOKE.OfferCatalogItemVector_Add(this.swigCPtr, OfferCatalogItem.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.OfferCatalogItemVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.OfferCatalogItemVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.OfferCatalogItemVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public OfferCatalogItemVector() : this(MothershipApiPINVOKE.new_OfferCatalogItemVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public OfferCatalogItemVector(OfferCatalogItemVector other) : this(MothershipApiPINVOKE.new_OfferCatalogItemVector__SWIG_1(OfferCatalogItemVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public OfferCatalogItemVector(int capacity) : this(MothershipApiPINVOKE.new_OfferCatalogItemVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private OfferCatalogItem getitemcopy(int index)
	{
		OfferCatalogItem result = new OfferCatalogItem(MothershipApiPINVOKE.OfferCatalogItemVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private OfferCatalogItem getitem(int index)
	{
		OfferCatalogItem result = new OfferCatalogItem(MothershipApiPINVOKE.OfferCatalogItemVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, OfferCatalogItem val)
	{
		MothershipApiPINVOKE.OfferCatalogItemVector_setitem(this.swigCPtr, index, OfferCatalogItem.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(OfferCatalogItemVector values)
	{
		MothershipApiPINVOKE.OfferCatalogItemVector_AddRange(this.swigCPtr, OfferCatalogItemVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public OfferCatalogItemVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.OfferCatalogItemVector_GetRange(this.swigCPtr, index, count);
		OfferCatalogItemVector result = (intPtr == IntPtr.Zero) ? null : new OfferCatalogItemVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, OfferCatalogItem x)
	{
		MothershipApiPINVOKE.OfferCatalogItemVector_Insert(this.swigCPtr, index, OfferCatalogItem.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, OfferCatalogItemVector values)
	{
		MothershipApiPINVOKE.OfferCatalogItemVector_InsertRange(this.swigCPtr, index, OfferCatalogItemVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.OfferCatalogItemVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.OfferCatalogItemVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static OfferCatalogItemVector Repeat(OfferCatalogItem value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.OfferCatalogItemVector_Repeat(OfferCatalogItem.getCPtr(value), count);
		OfferCatalogItemVector result = (intPtr == IntPtr.Zero) ? null : new OfferCatalogItemVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.OfferCatalogItemVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.OfferCatalogItemVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, OfferCatalogItemVector values)
	{
		MothershipApiPINVOKE.OfferCatalogItemVector_SetRange(this.swigCPtr, index, OfferCatalogItemVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class OfferCatalogItemVectorEnumerator : IEnumerator, IEnumerator<OfferCatalogItem>, IDisposable
	{
		public OfferCatalogItemVectorEnumerator(OfferCatalogItemVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public OfferCatalogItem Current
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
				return (OfferCatalogItem)this.currentObject;
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

		private OfferCatalogItemVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
