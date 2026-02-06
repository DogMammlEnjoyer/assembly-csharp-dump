using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class HydratedTrackVector : IDisposable, IEnumerable, IEnumerable<HydratedProgressionTrackResponse>
{
	internal HydratedTrackVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(HydratedTrackVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(HydratedTrackVector obj)
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

	~HydratedTrackVector()
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
					MothershipApiPINVOKE.delete_HydratedTrackVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public HydratedTrackVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			HydratedProgressionTrackResponse x = (HydratedProgressionTrackResponse)obj;
			this.Add(x);
		}
	}

	public HydratedTrackVector(IEnumerable<HydratedProgressionTrackResponse> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (HydratedProgressionTrackResponse x in c)
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

	public HydratedProgressionTrackResponse this[int index]
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

	public void CopyTo(HydratedProgressionTrackResponse[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(HydratedProgressionTrackResponse[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, HydratedProgressionTrackResponse[] array, int arrayIndex, int count)
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

	public HydratedProgressionTrackResponse[] ToArray()
	{
		HydratedProgressionTrackResponse[] array = new HydratedProgressionTrackResponse[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<HydratedProgressionTrackResponse> IEnumerable<HydratedProgressionTrackResponse>.GetEnumerator()
	{
		return new HydratedTrackVector.HydratedTrackVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new HydratedTrackVector.HydratedTrackVectorEnumerator(this);
	}

	public HydratedTrackVector.HydratedTrackVectorEnumerator GetEnumerator()
	{
		return new HydratedTrackVector.HydratedTrackVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.HydratedTrackVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(HydratedProgressionTrackResponse x)
	{
		MothershipApiPINVOKE.HydratedTrackVector_Add(this.swigCPtr, HydratedProgressionTrackResponse.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.HydratedTrackVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.HydratedTrackVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.HydratedTrackVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public HydratedTrackVector() : this(MothershipApiPINVOKE.new_HydratedTrackVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public HydratedTrackVector(HydratedTrackVector other) : this(MothershipApiPINVOKE.new_HydratedTrackVector__SWIG_1(HydratedTrackVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public HydratedTrackVector(int capacity) : this(MothershipApiPINVOKE.new_HydratedTrackVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HydratedProgressionTrackResponse getitemcopy(int index)
	{
		HydratedProgressionTrackResponse result = new HydratedProgressionTrackResponse(MothershipApiPINVOKE.HydratedTrackVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private HydratedProgressionTrackResponse getitem(int index)
	{
		HydratedProgressionTrackResponse result = new HydratedProgressionTrackResponse(MothershipApiPINVOKE.HydratedTrackVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, HydratedProgressionTrackResponse val)
	{
		MothershipApiPINVOKE.HydratedTrackVector_setitem(this.swigCPtr, index, HydratedProgressionTrackResponse.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(HydratedTrackVector values)
	{
		MothershipApiPINVOKE.HydratedTrackVector_AddRange(this.swigCPtr, HydratedTrackVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public HydratedTrackVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.HydratedTrackVector_GetRange(this.swigCPtr, index, count);
		HydratedTrackVector result = (intPtr == IntPtr.Zero) ? null : new HydratedTrackVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, HydratedProgressionTrackResponse x)
	{
		MothershipApiPINVOKE.HydratedTrackVector_Insert(this.swigCPtr, index, HydratedProgressionTrackResponse.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, HydratedTrackVector values)
	{
		MothershipApiPINVOKE.HydratedTrackVector_InsertRange(this.swigCPtr, index, HydratedTrackVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.HydratedTrackVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.HydratedTrackVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static HydratedTrackVector Repeat(HydratedProgressionTrackResponse value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.HydratedTrackVector_Repeat(HydratedProgressionTrackResponse.getCPtr(value), count);
		HydratedTrackVector result = (intPtr == IntPtr.Zero) ? null : new HydratedTrackVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.HydratedTrackVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.HydratedTrackVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, HydratedTrackVector values)
	{
		MothershipApiPINVOKE.HydratedTrackVector_SetRange(this.swigCPtr, index, HydratedTrackVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class HydratedTrackVectorEnumerator : IEnumerator, IEnumerator<HydratedProgressionTrackResponse>, IDisposable
	{
		public HydratedTrackVectorEnumerator(HydratedTrackVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public HydratedProgressionTrackResponse Current
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
				return (HydratedProgressionTrackResponse)this.currentObject;
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

		private HydratedTrackVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
