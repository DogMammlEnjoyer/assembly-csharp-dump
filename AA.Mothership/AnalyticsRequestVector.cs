using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class AnalyticsRequestVector : IDisposable, IEnumerable, IEnumerable<MothershipAnalyticsEvent>
{
	internal AnalyticsRequestVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(AnalyticsRequestVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(AnalyticsRequestVector obj)
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

	~AnalyticsRequestVector()
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
					MothershipApiPINVOKE.delete_AnalyticsRequestVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public AnalyticsRequestVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			MothershipAnalyticsEvent x = (MothershipAnalyticsEvent)obj;
			this.Add(x);
		}
	}

	public AnalyticsRequestVector(IEnumerable<MothershipAnalyticsEvent> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (MothershipAnalyticsEvent x in c)
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

	public MothershipAnalyticsEvent this[int index]
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

	public void CopyTo(MothershipAnalyticsEvent[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(MothershipAnalyticsEvent[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, MothershipAnalyticsEvent[] array, int arrayIndex, int count)
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

	public MothershipAnalyticsEvent[] ToArray()
	{
		MothershipAnalyticsEvent[] array = new MothershipAnalyticsEvent[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<MothershipAnalyticsEvent> IEnumerable<MothershipAnalyticsEvent>.GetEnumerator()
	{
		return new AnalyticsRequestVector.AnalyticsRequestVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new AnalyticsRequestVector.AnalyticsRequestVectorEnumerator(this);
	}

	public AnalyticsRequestVector.AnalyticsRequestVectorEnumerator GetEnumerator()
	{
		return new AnalyticsRequestVector.AnalyticsRequestVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.AnalyticsRequestVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(MothershipAnalyticsEvent x)
	{
		MothershipApiPINVOKE.AnalyticsRequestVector_Add(this.swigCPtr, MothershipAnalyticsEvent.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.AnalyticsRequestVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.AnalyticsRequestVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.AnalyticsRequestVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public AnalyticsRequestVector() : this(MothershipApiPINVOKE.new_AnalyticsRequestVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public AnalyticsRequestVector(AnalyticsRequestVector other) : this(MothershipApiPINVOKE.new_AnalyticsRequestVector__SWIG_1(AnalyticsRequestVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public AnalyticsRequestVector(int capacity) : this(MothershipApiPINVOKE.new_AnalyticsRequestVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private MothershipAnalyticsEvent getitemcopy(int index)
	{
		MothershipAnalyticsEvent result = new MothershipAnalyticsEvent(MothershipApiPINVOKE.AnalyticsRequestVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private MothershipAnalyticsEvent getitem(int index)
	{
		MothershipAnalyticsEvent result = new MothershipAnalyticsEvent(MothershipApiPINVOKE.AnalyticsRequestVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, MothershipAnalyticsEvent val)
	{
		MothershipApiPINVOKE.AnalyticsRequestVector_setitem(this.swigCPtr, index, MothershipAnalyticsEvent.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(AnalyticsRequestVector values)
	{
		MothershipApiPINVOKE.AnalyticsRequestVector_AddRange(this.swigCPtr, AnalyticsRequestVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public AnalyticsRequestVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.AnalyticsRequestVector_GetRange(this.swigCPtr, index, count);
		AnalyticsRequestVector result = (intPtr == IntPtr.Zero) ? null : new AnalyticsRequestVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, MothershipAnalyticsEvent x)
	{
		MothershipApiPINVOKE.AnalyticsRequestVector_Insert(this.swigCPtr, index, MothershipAnalyticsEvent.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, AnalyticsRequestVector values)
	{
		MothershipApiPINVOKE.AnalyticsRequestVector_InsertRange(this.swigCPtr, index, AnalyticsRequestVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.AnalyticsRequestVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.AnalyticsRequestVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static AnalyticsRequestVector Repeat(MothershipAnalyticsEvent value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.AnalyticsRequestVector_Repeat(MothershipAnalyticsEvent.getCPtr(value), count);
		AnalyticsRequestVector result = (intPtr == IntPtr.Zero) ? null : new AnalyticsRequestVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.AnalyticsRequestVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.AnalyticsRequestVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, AnalyticsRequestVector values)
	{
		MothershipApiPINVOKE.AnalyticsRequestVector_SetRange(this.swigCPtr, index, AnalyticsRequestVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class AnalyticsRequestVectorEnumerator : IEnumerator, IEnumerator<MothershipAnalyticsEvent>, IDisposable
	{
		public AnalyticsRequestVectorEnumerator(AnalyticsRequestVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public MothershipAnalyticsEvent Current
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
				return (MothershipAnalyticsEvent)this.currentObject;
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

		private AnalyticsRequestVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
