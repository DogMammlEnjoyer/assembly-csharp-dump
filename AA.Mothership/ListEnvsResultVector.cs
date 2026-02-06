using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ListEnvsResultVector : IDisposable, IEnumerable, IEnumerable<MothershipTitleEnvironment>
{
	internal ListEnvsResultVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(ListEnvsResultVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(ListEnvsResultVector obj)
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

	~ListEnvsResultVector()
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
					MothershipApiPINVOKE.delete_ListEnvsResultVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public ListEnvsResultVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			MothershipTitleEnvironment x = (MothershipTitleEnvironment)obj;
			this.Add(x);
		}
	}

	public ListEnvsResultVector(IEnumerable<MothershipTitleEnvironment> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (MothershipTitleEnvironment x in c)
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

	public MothershipTitleEnvironment this[int index]
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

	public void CopyTo(MothershipTitleEnvironment[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(MothershipTitleEnvironment[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, MothershipTitleEnvironment[] array, int arrayIndex, int count)
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

	public MothershipTitleEnvironment[] ToArray()
	{
		MothershipTitleEnvironment[] array = new MothershipTitleEnvironment[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<MothershipTitleEnvironment> IEnumerable<MothershipTitleEnvironment>.GetEnumerator()
	{
		return new ListEnvsResultVector.ListEnvsResultVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new ListEnvsResultVector.ListEnvsResultVectorEnumerator(this);
	}

	public ListEnvsResultVector.ListEnvsResultVectorEnumerator GetEnumerator()
	{
		return new ListEnvsResultVector.ListEnvsResultVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.ListEnvsResultVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(MothershipTitleEnvironment x)
	{
		MothershipApiPINVOKE.ListEnvsResultVector_Add(this.swigCPtr, MothershipTitleEnvironment.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.ListEnvsResultVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.ListEnvsResultVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.ListEnvsResultVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListEnvsResultVector() : this(MothershipApiPINVOKE.new_ListEnvsResultVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListEnvsResultVector(ListEnvsResultVector other) : this(MothershipApiPINVOKE.new_ListEnvsResultVector__SWIG_1(ListEnvsResultVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListEnvsResultVector(int capacity) : this(MothershipApiPINVOKE.new_ListEnvsResultVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private MothershipTitleEnvironment getitemcopy(int index)
	{
		MothershipTitleEnvironment result = new MothershipTitleEnvironment(MothershipApiPINVOKE.ListEnvsResultVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private MothershipTitleEnvironment getitem(int index)
	{
		MothershipTitleEnvironment result = new MothershipTitleEnvironment(MothershipApiPINVOKE.ListEnvsResultVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, MothershipTitleEnvironment val)
	{
		MothershipApiPINVOKE.ListEnvsResultVector_setitem(this.swigCPtr, index, MothershipTitleEnvironment.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(ListEnvsResultVector values)
	{
		MothershipApiPINVOKE.ListEnvsResultVector_AddRange(this.swigCPtr, ListEnvsResultVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListEnvsResultVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ListEnvsResultVector_GetRange(this.swigCPtr, index, count);
		ListEnvsResultVector result = (intPtr == IntPtr.Zero) ? null : new ListEnvsResultVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, MothershipTitleEnvironment x)
	{
		MothershipApiPINVOKE.ListEnvsResultVector_Insert(this.swigCPtr, index, MothershipTitleEnvironment.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, ListEnvsResultVector values)
	{
		MothershipApiPINVOKE.ListEnvsResultVector_InsertRange(this.swigCPtr, index, ListEnvsResultVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.ListEnvsResultVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.ListEnvsResultVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static ListEnvsResultVector Repeat(MothershipTitleEnvironment value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ListEnvsResultVector_Repeat(MothershipTitleEnvironment.getCPtr(value), count);
		ListEnvsResultVector result = (intPtr == IntPtr.Zero) ? null : new ListEnvsResultVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.ListEnvsResultVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.ListEnvsResultVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, ListEnvsResultVector values)
	{
		MothershipApiPINVOKE.ListEnvsResultVector_SetRange(this.swigCPtr, index, ListEnvsResultVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class ListEnvsResultVectorEnumerator : IEnumerator, IEnumerator<MothershipTitleEnvironment>, IDisposable
	{
		public ListEnvsResultVectorEnumerator(ListEnvsResultVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public MothershipTitleEnvironment Current
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
				return (MothershipTitleEnvironment)this.currentObject;
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

		private ListEnvsResultVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
