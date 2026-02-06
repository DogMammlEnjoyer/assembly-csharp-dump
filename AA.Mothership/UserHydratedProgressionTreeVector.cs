using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class UserHydratedProgressionTreeVector : IDisposable, IEnumerable, IEnumerable<UserHydratedProgressionTreeResponse>
{
	internal UserHydratedProgressionTreeVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(UserHydratedProgressionTreeVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(UserHydratedProgressionTreeVector obj)
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

	~UserHydratedProgressionTreeVector()
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
					MothershipApiPINVOKE.delete_UserHydratedProgressionTreeVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public UserHydratedProgressionTreeVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			UserHydratedProgressionTreeResponse x = (UserHydratedProgressionTreeResponse)obj;
			this.Add(x);
		}
	}

	public UserHydratedProgressionTreeVector(IEnumerable<UserHydratedProgressionTreeResponse> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (UserHydratedProgressionTreeResponse x in c)
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

	public UserHydratedProgressionTreeResponse this[int index]
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

	public void CopyTo(UserHydratedProgressionTreeResponse[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(UserHydratedProgressionTreeResponse[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, UserHydratedProgressionTreeResponse[] array, int arrayIndex, int count)
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

	public UserHydratedProgressionTreeResponse[] ToArray()
	{
		UserHydratedProgressionTreeResponse[] array = new UserHydratedProgressionTreeResponse[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<UserHydratedProgressionTreeResponse> IEnumerable<UserHydratedProgressionTreeResponse>.GetEnumerator()
	{
		return new UserHydratedProgressionTreeVector.UserHydratedProgressionTreeVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new UserHydratedProgressionTreeVector.UserHydratedProgressionTreeVectorEnumerator(this);
	}

	public UserHydratedProgressionTreeVector.UserHydratedProgressionTreeVectorEnumerator GetEnumerator()
	{
		return new UserHydratedProgressionTreeVector.UserHydratedProgressionTreeVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.UserHydratedProgressionTreeVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(UserHydratedProgressionTreeResponse x)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTreeVector_Add(this.swigCPtr, UserHydratedProgressionTreeResponse.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.UserHydratedProgressionTreeVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.UserHydratedProgressionTreeVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTreeVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserHydratedProgressionTreeVector() : this(MothershipApiPINVOKE.new_UserHydratedProgressionTreeVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserHydratedProgressionTreeVector(UserHydratedProgressionTreeVector other) : this(MothershipApiPINVOKE.new_UserHydratedProgressionTreeVector__SWIG_1(UserHydratedProgressionTreeVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserHydratedProgressionTreeVector(int capacity) : this(MothershipApiPINVOKE.new_UserHydratedProgressionTreeVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private UserHydratedProgressionTreeResponse getitemcopy(int index)
	{
		UserHydratedProgressionTreeResponse result = new UserHydratedProgressionTreeResponse(MothershipApiPINVOKE.UserHydratedProgressionTreeVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private UserHydratedProgressionTreeResponse getitem(int index)
	{
		UserHydratedProgressionTreeResponse result = new UserHydratedProgressionTreeResponse(MothershipApiPINVOKE.UserHydratedProgressionTreeVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, UserHydratedProgressionTreeResponse val)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTreeVector_setitem(this.swigCPtr, index, UserHydratedProgressionTreeResponse.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(UserHydratedProgressionTreeVector values)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTreeVector_AddRange(this.swigCPtr, UserHydratedProgressionTreeVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserHydratedProgressionTreeVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UserHydratedProgressionTreeVector_GetRange(this.swigCPtr, index, count);
		UserHydratedProgressionTreeVector result = (intPtr == IntPtr.Zero) ? null : new UserHydratedProgressionTreeVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, UserHydratedProgressionTreeResponse x)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTreeVector_Insert(this.swigCPtr, index, UserHydratedProgressionTreeResponse.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, UserHydratedProgressionTreeVector values)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTreeVector_InsertRange(this.swigCPtr, index, UserHydratedProgressionTreeVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTreeVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTreeVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static UserHydratedProgressionTreeVector Repeat(UserHydratedProgressionTreeResponse value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UserHydratedProgressionTreeVector_Repeat(UserHydratedProgressionTreeResponse.getCPtr(value), count);
		UserHydratedProgressionTreeVector result = (intPtr == IntPtr.Zero) ? null : new UserHydratedProgressionTreeVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.UserHydratedProgressionTreeVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTreeVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, UserHydratedProgressionTreeVector values)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTreeVector_SetRange(this.swigCPtr, index, UserHydratedProgressionTreeVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class UserHydratedProgressionTreeVectorEnumerator : IEnumerator, IEnumerator<UserHydratedProgressionTreeResponse>, IDisposable
	{
		public UserHydratedProgressionTreeVectorEnumerator(UserHydratedProgressionTreeVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public UserHydratedProgressionTreeResponse Current
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
				return (UserHydratedProgressionTreeResponse)this.currentObject;
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

		private UserHydratedProgressionTreeVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
