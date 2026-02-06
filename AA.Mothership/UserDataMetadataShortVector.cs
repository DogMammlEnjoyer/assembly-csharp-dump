using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class UserDataMetadataShortVector : IDisposable, IEnumerable, IEnumerable<MothershipUserDataMetadataShort>
{
	internal UserDataMetadataShortVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(UserDataMetadataShortVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(UserDataMetadataShortVector obj)
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

	~UserDataMetadataShortVector()
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
					MothershipApiPINVOKE.delete_UserDataMetadataShortVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public UserDataMetadataShortVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			MothershipUserDataMetadataShort x = (MothershipUserDataMetadataShort)obj;
			this.Add(x);
		}
	}

	public UserDataMetadataShortVector(IEnumerable<MothershipUserDataMetadataShort> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (MothershipUserDataMetadataShort x in c)
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

	public MothershipUserDataMetadataShort this[int index]
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

	public void CopyTo(MothershipUserDataMetadataShort[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(MothershipUserDataMetadataShort[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, MothershipUserDataMetadataShort[] array, int arrayIndex, int count)
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

	public MothershipUserDataMetadataShort[] ToArray()
	{
		MothershipUserDataMetadataShort[] array = new MothershipUserDataMetadataShort[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<MothershipUserDataMetadataShort> IEnumerable<MothershipUserDataMetadataShort>.GetEnumerator()
	{
		return new UserDataMetadataShortVector.UserDataMetadataShortVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new UserDataMetadataShortVector.UserDataMetadataShortVectorEnumerator(this);
	}

	public UserDataMetadataShortVector.UserDataMetadataShortVectorEnumerator GetEnumerator()
	{
		return new UserDataMetadataShortVector.UserDataMetadataShortVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.UserDataMetadataShortVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(MothershipUserDataMetadataShort x)
	{
		MothershipApiPINVOKE.UserDataMetadataShortVector_Add(this.swigCPtr, MothershipUserDataMetadataShort.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.UserDataMetadataShortVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.UserDataMetadataShortVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.UserDataMetadataShortVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserDataMetadataShortVector() : this(MothershipApiPINVOKE.new_UserDataMetadataShortVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserDataMetadataShortVector(UserDataMetadataShortVector other) : this(MothershipApiPINVOKE.new_UserDataMetadataShortVector__SWIG_1(UserDataMetadataShortVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserDataMetadataShortVector(int capacity) : this(MothershipApiPINVOKE.new_UserDataMetadataShortVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private MothershipUserDataMetadataShort getitemcopy(int index)
	{
		MothershipUserDataMetadataShort result = new MothershipUserDataMetadataShort(MothershipApiPINVOKE.UserDataMetadataShortVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private MothershipUserDataMetadataShort getitem(int index)
	{
		MothershipUserDataMetadataShort result = new MothershipUserDataMetadataShort(MothershipApiPINVOKE.UserDataMetadataShortVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, MothershipUserDataMetadataShort val)
	{
		MothershipApiPINVOKE.UserDataMetadataShortVector_setitem(this.swigCPtr, index, MothershipUserDataMetadataShort.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(UserDataMetadataShortVector values)
	{
		MothershipApiPINVOKE.UserDataMetadataShortVector_AddRange(this.swigCPtr, UserDataMetadataShortVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserDataMetadataShortVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UserDataMetadataShortVector_GetRange(this.swigCPtr, index, count);
		UserDataMetadataShortVector result = (intPtr == IntPtr.Zero) ? null : new UserDataMetadataShortVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, MothershipUserDataMetadataShort x)
	{
		MothershipApiPINVOKE.UserDataMetadataShortVector_Insert(this.swigCPtr, index, MothershipUserDataMetadataShort.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, UserDataMetadataShortVector values)
	{
		MothershipApiPINVOKE.UserDataMetadataShortVector_InsertRange(this.swigCPtr, index, UserDataMetadataShortVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.UserDataMetadataShortVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.UserDataMetadataShortVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static UserDataMetadataShortVector Repeat(MothershipUserDataMetadataShort value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UserDataMetadataShortVector_Repeat(MothershipUserDataMetadataShort.getCPtr(value), count);
		UserDataMetadataShortVector result = (intPtr == IntPtr.Zero) ? null : new UserDataMetadataShortVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.UserDataMetadataShortVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.UserDataMetadataShortVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, UserDataMetadataShortVector values)
	{
		MothershipApiPINVOKE.UserDataMetadataShortVector_SetRange(this.swigCPtr, index, UserDataMetadataShortVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class UserDataMetadataShortVectorEnumerator : IEnumerator, IEnumerator<MothershipUserDataMetadataShort>, IDisposable
	{
		public UserDataMetadataShortVectorEnumerator(UserDataMetadataShortVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public MothershipUserDataMetadataShort Current
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
				return (MothershipUserDataMetadataShort)this.currentObject;
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

		private UserDataMetadataShortVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
