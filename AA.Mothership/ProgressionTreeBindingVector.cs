using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ProgressionTreeBindingVector : IDisposable, IEnumerable, IEnumerable<ProgressionTreeBindingResponse>
{
	internal ProgressionTreeBindingVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(ProgressionTreeBindingVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(ProgressionTreeBindingVector obj)
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

	~ProgressionTreeBindingVector()
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
					MothershipApiPINVOKE.delete_ProgressionTreeBindingVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public ProgressionTreeBindingVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			ProgressionTreeBindingResponse x = (ProgressionTreeBindingResponse)obj;
			this.Add(x);
		}
	}

	public ProgressionTreeBindingVector(IEnumerable<ProgressionTreeBindingResponse> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (ProgressionTreeBindingResponse x in c)
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

	public ProgressionTreeBindingResponse this[int index]
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

	public void CopyTo(ProgressionTreeBindingResponse[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(ProgressionTreeBindingResponse[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, ProgressionTreeBindingResponse[] array, int arrayIndex, int count)
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

	public ProgressionTreeBindingResponse[] ToArray()
	{
		ProgressionTreeBindingResponse[] array = new ProgressionTreeBindingResponse[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<ProgressionTreeBindingResponse> IEnumerable<ProgressionTreeBindingResponse>.GetEnumerator()
	{
		return new ProgressionTreeBindingVector.ProgressionTreeBindingVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new ProgressionTreeBindingVector.ProgressionTreeBindingVectorEnumerator(this);
	}

	public ProgressionTreeBindingVector.ProgressionTreeBindingVectorEnumerator GetEnumerator()
	{
		return new ProgressionTreeBindingVector.ProgressionTreeBindingVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.ProgressionTreeBindingVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(ProgressionTreeBindingResponse x)
	{
		MothershipApiPINVOKE.ProgressionTreeBindingVector_Add(this.swigCPtr, ProgressionTreeBindingResponse.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.ProgressionTreeBindingVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.ProgressionTreeBindingVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.ProgressionTreeBindingVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ProgressionTreeBindingVector() : this(MothershipApiPINVOKE.new_ProgressionTreeBindingVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ProgressionTreeBindingVector(ProgressionTreeBindingVector other) : this(MothershipApiPINVOKE.new_ProgressionTreeBindingVector__SWIG_1(ProgressionTreeBindingVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ProgressionTreeBindingVector(int capacity) : this(MothershipApiPINVOKE.new_ProgressionTreeBindingVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private ProgressionTreeBindingResponse getitemcopy(int index)
	{
		ProgressionTreeBindingResponse result = new ProgressionTreeBindingResponse(MothershipApiPINVOKE.ProgressionTreeBindingVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private ProgressionTreeBindingResponse getitem(int index)
	{
		ProgressionTreeBindingResponse result = new ProgressionTreeBindingResponse(MothershipApiPINVOKE.ProgressionTreeBindingVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, ProgressionTreeBindingResponse val)
	{
		MothershipApiPINVOKE.ProgressionTreeBindingVector_setitem(this.swigCPtr, index, ProgressionTreeBindingResponse.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(ProgressionTreeBindingVector values)
	{
		MothershipApiPINVOKE.ProgressionTreeBindingVector_AddRange(this.swigCPtr, ProgressionTreeBindingVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ProgressionTreeBindingVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ProgressionTreeBindingVector_GetRange(this.swigCPtr, index, count);
		ProgressionTreeBindingVector result = (intPtr == IntPtr.Zero) ? null : new ProgressionTreeBindingVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, ProgressionTreeBindingResponse x)
	{
		MothershipApiPINVOKE.ProgressionTreeBindingVector_Insert(this.swigCPtr, index, ProgressionTreeBindingResponse.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, ProgressionTreeBindingVector values)
	{
		MothershipApiPINVOKE.ProgressionTreeBindingVector_InsertRange(this.swigCPtr, index, ProgressionTreeBindingVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.ProgressionTreeBindingVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.ProgressionTreeBindingVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static ProgressionTreeBindingVector Repeat(ProgressionTreeBindingResponse value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ProgressionTreeBindingVector_Repeat(ProgressionTreeBindingResponse.getCPtr(value), count);
		ProgressionTreeBindingVector result = (intPtr == IntPtr.Zero) ? null : new ProgressionTreeBindingVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.ProgressionTreeBindingVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.ProgressionTreeBindingVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, ProgressionTreeBindingVector values)
	{
		MothershipApiPINVOKE.ProgressionTreeBindingVector_SetRange(this.swigCPtr, index, ProgressionTreeBindingVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class ProgressionTreeBindingVectorEnumerator : IEnumerator, IEnumerator<ProgressionTreeBindingResponse>, IDisposable
	{
		public ProgressionTreeBindingVectorEnumerator(ProgressionTreeBindingVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public ProgressionTreeBindingResponse Current
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
				return (ProgressionTreeBindingResponse)this.currentObject;
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

		private ProgressionTreeBindingVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
