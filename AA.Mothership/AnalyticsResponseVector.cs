using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class AnalyticsResponseVector : IDisposable, IEnumerable, IEnumerable<MothershipAnalyticsResultEntry>
{
	internal AnalyticsResponseVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(AnalyticsResponseVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(AnalyticsResponseVector obj)
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

	~AnalyticsResponseVector()
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
					MothershipApiPINVOKE.delete_AnalyticsResponseVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public AnalyticsResponseVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			MothershipAnalyticsResultEntry x = (MothershipAnalyticsResultEntry)obj;
			this.Add(x);
		}
	}

	public AnalyticsResponseVector(IEnumerable<MothershipAnalyticsResultEntry> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (MothershipAnalyticsResultEntry x in c)
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

	public MothershipAnalyticsResultEntry this[int index]
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

	public void CopyTo(MothershipAnalyticsResultEntry[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(MothershipAnalyticsResultEntry[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, MothershipAnalyticsResultEntry[] array, int arrayIndex, int count)
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

	public MothershipAnalyticsResultEntry[] ToArray()
	{
		MothershipAnalyticsResultEntry[] array = new MothershipAnalyticsResultEntry[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<MothershipAnalyticsResultEntry> IEnumerable<MothershipAnalyticsResultEntry>.GetEnumerator()
	{
		return new AnalyticsResponseVector.AnalyticsResponseVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new AnalyticsResponseVector.AnalyticsResponseVectorEnumerator(this);
	}

	public AnalyticsResponseVector.AnalyticsResponseVectorEnumerator GetEnumerator()
	{
		return new AnalyticsResponseVector.AnalyticsResponseVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.AnalyticsResponseVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(MothershipAnalyticsResultEntry x)
	{
		MothershipApiPINVOKE.AnalyticsResponseVector_Add(this.swigCPtr, MothershipAnalyticsResultEntry.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.AnalyticsResponseVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.AnalyticsResponseVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.AnalyticsResponseVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public AnalyticsResponseVector() : this(MothershipApiPINVOKE.new_AnalyticsResponseVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public AnalyticsResponseVector(AnalyticsResponseVector other) : this(MothershipApiPINVOKE.new_AnalyticsResponseVector__SWIG_1(AnalyticsResponseVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public AnalyticsResponseVector(int capacity) : this(MothershipApiPINVOKE.new_AnalyticsResponseVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private MothershipAnalyticsResultEntry getitemcopy(int index)
	{
		MothershipAnalyticsResultEntry result = new MothershipAnalyticsResultEntry(MothershipApiPINVOKE.AnalyticsResponseVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private MothershipAnalyticsResultEntry getitem(int index)
	{
		MothershipAnalyticsResultEntry result = new MothershipAnalyticsResultEntry(MothershipApiPINVOKE.AnalyticsResponseVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, MothershipAnalyticsResultEntry val)
	{
		MothershipApiPINVOKE.AnalyticsResponseVector_setitem(this.swigCPtr, index, MothershipAnalyticsResultEntry.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(AnalyticsResponseVector values)
	{
		MothershipApiPINVOKE.AnalyticsResponseVector_AddRange(this.swigCPtr, AnalyticsResponseVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public AnalyticsResponseVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.AnalyticsResponseVector_GetRange(this.swigCPtr, index, count);
		AnalyticsResponseVector result = (intPtr == IntPtr.Zero) ? null : new AnalyticsResponseVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, MothershipAnalyticsResultEntry x)
	{
		MothershipApiPINVOKE.AnalyticsResponseVector_Insert(this.swigCPtr, index, MothershipAnalyticsResultEntry.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, AnalyticsResponseVector values)
	{
		MothershipApiPINVOKE.AnalyticsResponseVector_InsertRange(this.swigCPtr, index, AnalyticsResponseVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.AnalyticsResponseVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.AnalyticsResponseVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static AnalyticsResponseVector Repeat(MothershipAnalyticsResultEntry value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.AnalyticsResponseVector_Repeat(MothershipAnalyticsResultEntry.getCPtr(value), count);
		AnalyticsResponseVector result = (intPtr == IntPtr.Zero) ? null : new AnalyticsResponseVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.AnalyticsResponseVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.AnalyticsResponseVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, AnalyticsResponseVector values)
	{
		MothershipApiPINVOKE.AnalyticsResponseVector_SetRange(this.swigCPtr, index, AnalyticsResponseVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class AnalyticsResponseVectorEnumerator : IEnumerator, IEnumerator<MothershipAnalyticsResultEntry>, IDisposable
	{
		public AnalyticsResponseVectorEnumerator(AnalyticsResponseVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public MothershipAnalyticsResultEntry Current
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
				return (MothershipAnalyticsResultEntry)this.currentObject;
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

		private AnalyticsResponseVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
