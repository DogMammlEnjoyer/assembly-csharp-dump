using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class PrerequisiteLevelVector : IDisposable, IEnumerable, IEnumerable<PrerequisiteLevel>
{
	internal PrerequisiteLevelVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(PrerequisiteLevelVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(PrerequisiteLevelVector obj)
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

	~PrerequisiteLevelVector()
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
					MothershipApiPINVOKE.delete_PrerequisiteLevelVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public PrerequisiteLevelVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			PrerequisiteLevel x = (PrerequisiteLevel)obj;
			this.Add(x);
		}
	}

	public PrerequisiteLevelVector(IEnumerable<PrerequisiteLevel> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (PrerequisiteLevel x in c)
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

	public PrerequisiteLevel this[int index]
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

	public void CopyTo(PrerequisiteLevel[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(PrerequisiteLevel[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, PrerequisiteLevel[] array, int arrayIndex, int count)
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

	public PrerequisiteLevel[] ToArray()
	{
		PrerequisiteLevel[] array = new PrerequisiteLevel[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<PrerequisiteLevel> IEnumerable<PrerequisiteLevel>.GetEnumerator()
	{
		return new PrerequisiteLevelVector.PrerequisiteLevelVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new PrerequisiteLevelVector.PrerequisiteLevelVectorEnumerator(this);
	}

	public PrerequisiteLevelVector.PrerequisiteLevelVectorEnumerator GetEnumerator()
	{
		return new PrerequisiteLevelVector.PrerequisiteLevelVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.PrerequisiteLevelVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(PrerequisiteLevel x)
	{
		MothershipApiPINVOKE.PrerequisiteLevelVector_Add(this.swigCPtr, PrerequisiteLevel.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.PrerequisiteLevelVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.PrerequisiteLevelVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.PrerequisiteLevelVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public PrerequisiteLevelVector() : this(MothershipApiPINVOKE.new_PrerequisiteLevelVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public PrerequisiteLevelVector(PrerequisiteLevelVector other) : this(MothershipApiPINVOKE.new_PrerequisiteLevelVector__SWIG_1(PrerequisiteLevelVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public PrerequisiteLevelVector(int capacity) : this(MothershipApiPINVOKE.new_PrerequisiteLevelVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private PrerequisiteLevel getitemcopy(int index)
	{
		PrerequisiteLevel result = new PrerequisiteLevel(MothershipApiPINVOKE.PrerequisiteLevelVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private PrerequisiteLevel getitem(int index)
	{
		PrerequisiteLevel result = new PrerequisiteLevel(MothershipApiPINVOKE.PrerequisiteLevelVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, PrerequisiteLevel val)
	{
		MothershipApiPINVOKE.PrerequisiteLevelVector_setitem(this.swigCPtr, index, PrerequisiteLevel.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(PrerequisiteLevelVector values)
	{
		MothershipApiPINVOKE.PrerequisiteLevelVector_AddRange(this.swigCPtr, PrerequisiteLevelVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public PrerequisiteLevelVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.PrerequisiteLevelVector_GetRange(this.swigCPtr, index, count);
		PrerequisiteLevelVector result = (intPtr == IntPtr.Zero) ? null : new PrerequisiteLevelVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, PrerequisiteLevel x)
	{
		MothershipApiPINVOKE.PrerequisiteLevelVector_Insert(this.swigCPtr, index, PrerequisiteLevel.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, PrerequisiteLevelVector values)
	{
		MothershipApiPINVOKE.PrerequisiteLevelVector_InsertRange(this.swigCPtr, index, PrerequisiteLevelVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.PrerequisiteLevelVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.PrerequisiteLevelVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static PrerequisiteLevelVector Repeat(PrerequisiteLevel value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.PrerequisiteLevelVector_Repeat(PrerequisiteLevel.getCPtr(value), count);
		PrerequisiteLevelVector result = (intPtr == IntPtr.Zero) ? null : new PrerequisiteLevelVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.PrerequisiteLevelVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.PrerequisiteLevelVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, PrerequisiteLevelVector values)
	{
		MothershipApiPINVOKE.PrerequisiteLevelVector_SetRange(this.swigCPtr, index, PrerequisiteLevelVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class PrerequisiteLevelVectorEnumerator : IEnumerator, IEnumerator<PrerequisiteLevel>, IDisposable
	{
		public PrerequisiteLevelVectorEnumerator(PrerequisiteLevelVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public PrerequisiteLevel Current
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
				return (PrerequisiteLevel)this.currentObject;
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

		private PrerequisiteLevelVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
