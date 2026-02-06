using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ComplexNodeVector : IDisposable, IEnumerable, IEnumerable<SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t>
{
	internal ComplexNodeVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(ComplexNodeVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(ComplexNodeVector obj)
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

	~ComplexNodeVector()
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
					MothershipApiPINVOKE.delete_ComplexNodeVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public ComplexNodeVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t x = (SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t)obj;
			this.Add(x);
		}
	}

	public ComplexNodeVector(IEnumerable<SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t x in c)
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

	public SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t this[int index]
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

	public void CopyTo(SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t[] array, int arrayIndex, int count)
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

	public SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t[] ToArray()
	{
		SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t[] array = new SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t> IEnumerable<SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t>.GetEnumerator()
	{
		return new ComplexNodeVector.ComplexNodeVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new ComplexNodeVector.ComplexNodeVectorEnumerator(this);
	}

	public ComplexNodeVector.ComplexNodeVectorEnumerator GetEnumerator()
	{
		return new ComplexNodeVector.ComplexNodeVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.ComplexNodeVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t x)
	{
		MothershipApiPINVOKE.ComplexNodeVector_Add(this.swigCPtr, SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.ComplexNodeVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.ComplexNodeVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.ComplexNodeVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ComplexNodeVector() : this(MothershipApiPINVOKE.new_ComplexNodeVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ComplexNodeVector(ComplexNodeVector other) : this(MothershipApiPINVOKE.new_ComplexNodeVector__SWIG_1(ComplexNodeVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ComplexNodeVector(int capacity) : this(MothershipApiPINVOKE.new_ComplexNodeVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t getitemcopy(int index)
	{
		SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t result = new SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t(MothershipApiPINVOKE.ComplexNodeVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t getitem(int index)
	{
		SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t result = new SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t(MothershipApiPINVOKE.ComplexNodeVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t val)
	{
		MothershipApiPINVOKE.ComplexNodeVector_setitem(this.swigCPtr, index, SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(ComplexNodeVector values)
	{
		MothershipApiPINVOKE.ComplexNodeVector_AddRange(this.swigCPtr, ComplexNodeVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ComplexNodeVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ComplexNodeVector_GetRange(this.swigCPtr, index, count);
		ComplexNodeVector result = (intPtr == IntPtr.Zero) ? null : new ComplexNodeVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t x)
	{
		MothershipApiPINVOKE.ComplexNodeVector_Insert(this.swigCPtr, index, SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, ComplexNodeVector values)
	{
		MothershipApiPINVOKE.ComplexNodeVector_InsertRange(this.swigCPtr, index, ComplexNodeVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.ComplexNodeVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.ComplexNodeVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static ComplexNodeVector Repeat(SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ComplexNodeVector_Repeat(SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t.getCPtr(value), count);
		ComplexNodeVector result = (intPtr == IntPtr.Zero) ? null : new ComplexNodeVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.ComplexNodeVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.ComplexNodeVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, ComplexNodeVector values)
	{
		MothershipApiPINVOKE.ComplexNodeVector_SetRange(this.swigCPtr, index, ComplexNodeVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class ComplexNodeVectorEnumerator : IEnumerator, IEnumerator<SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t>, IDisposable
	{
		public ComplexNodeVectorEnumerator(ComplexNodeVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t Current
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
				return (SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t)this.currentObject;
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

		private ComplexNodeVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
