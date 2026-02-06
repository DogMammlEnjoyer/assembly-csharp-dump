using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class UserHydratedProgressionTrackVector : IDisposable, IEnumerable, IEnumerable<UserHydratedProgressionTrackResponse>
{
	internal UserHydratedProgressionTrackVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(UserHydratedProgressionTrackVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(UserHydratedProgressionTrackVector obj)
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

	~UserHydratedProgressionTrackVector()
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
					MothershipApiPINVOKE.delete_UserHydratedProgressionTrackVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public UserHydratedProgressionTrackVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			UserHydratedProgressionTrackResponse x = (UserHydratedProgressionTrackResponse)obj;
			this.Add(x);
		}
	}

	public UserHydratedProgressionTrackVector(IEnumerable<UserHydratedProgressionTrackResponse> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (UserHydratedProgressionTrackResponse x in c)
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

	public UserHydratedProgressionTrackResponse this[int index]
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

	public void CopyTo(UserHydratedProgressionTrackResponse[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(UserHydratedProgressionTrackResponse[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, UserHydratedProgressionTrackResponse[] array, int arrayIndex, int count)
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

	public UserHydratedProgressionTrackResponse[] ToArray()
	{
		UserHydratedProgressionTrackResponse[] array = new UserHydratedProgressionTrackResponse[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<UserHydratedProgressionTrackResponse> IEnumerable<UserHydratedProgressionTrackResponse>.GetEnumerator()
	{
		return new UserHydratedProgressionTrackVector.UserHydratedProgressionTrackVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new UserHydratedProgressionTrackVector.UserHydratedProgressionTrackVectorEnumerator(this);
	}

	public UserHydratedProgressionTrackVector.UserHydratedProgressionTrackVectorEnumerator GetEnumerator()
	{
		return new UserHydratedProgressionTrackVector.UserHydratedProgressionTrackVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.UserHydratedProgressionTrackVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(UserHydratedProgressionTrackResponse x)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTrackVector_Add(this.swigCPtr, UserHydratedProgressionTrackResponse.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.UserHydratedProgressionTrackVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.UserHydratedProgressionTrackVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTrackVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserHydratedProgressionTrackVector() : this(MothershipApiPINVOKE.new_UserHydratedProgressionTrackVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserHydratedProgressionTrackVector(UserHydratedProgressionTrackVector other) : this(MothershipApiPINVOKE.new_UserHydratedProgressionTrackVector__SWIG_1(UserHydratedProgressionTrackVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserHydratedProgressionTrackVector(int capacity) : this(MothershipApiPINVOKE.new_UserHydratedProgressionTrackVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private UserHydratedProgressionTrackResponse getitemcopy(int index)
	{
		UserHydratedProgressionTrackResponse result = new UserHydratedProgressionTrackResponse(MothershipApiPINVOKE.UserHydratedProgressionTrackVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private UserHydratedProgressionTrackResponse getitem(int index)
	{
		UserHydratedProgressionTrackResponse result = new UserHydratedProgressionTrackResponse(MothershipApiPINVOKE.UserHydratedProgressionTrackVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, UserHydratedProgressionTrackResponse val)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTrackVector_setitem(this.swigCPtr, index, UserHydratedProgressionTrackResponse.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(UserHydratedProgressionTrackVector values)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTrackVector_AddRange(this.swigCPtr, UserHydratedProgressionTrackVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserHydratedProgressionTrackVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UserHydratedProgressionTrackVector_GetRange(this.swigCPtr, index, count);
		UserHydratedProgressionTrackVector result = (intPtr == IntPtr.Zero) ? null : new UserHydratedProgressionTrackVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, UserHydratedProgressionTrackResponse x)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTrackVector_Insert(this.swigCPtr, index, UserHydratedProgressionTrackResponse.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, UserHydratedProgressionTrackVector values)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTrackVector_InsertRange(this.swigCPtr, index, UserHydratedProgressionTrackVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTrackVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTrackVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static UserHydratedProgressionTrackVector Repeat(UserHydratedProgressionTrackResponse value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UserHydratedProgressionTrackVector_Repeat(UserHydratedProgressionTrackResponse.getCPtr(value), count);
		UserHydratedProgressionTrackVector result = (intPtr == IntPtr.Zero) ? null : new UserHydratedProgressionTrackVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.UserHydratedProgressionTrackVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTrackVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, UserHydratedProgressionTrackVector values)
	{
		MothershipApiPINVOKE.UserHydratedProgressionTrackVector_SetRange(this.swigCPtr, index, UserHydratedProgressionTrackVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class UserHydratedProgressionTrackVectorEnumerator : IEnumerator, IEnumerator<UserHydratedProgressionTrackResponse>, IDisposable
	{
		public UserHydratedProgressionTrackVectorEnumerator(UserHydratedProgressionTrackVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public UserHydratedProgressionTrackResponse Current
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
				return (UserHydratedProgressionTrackResponse)this.currentObject;
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

		private UserHydratedProgressionTrackVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
