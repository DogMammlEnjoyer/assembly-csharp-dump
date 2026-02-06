using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class UserHydratedNodeVector : IDisposable, IEnumerable, IEnumerable<UserHydratedNodeDefinition>
{
	internal UserHydratedNodeVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(UserHydratedNodeVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(UserHydratedNodeVector obj)
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

	~UserHydratedNodeVector()
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
					MothershipApiPINVOKE.delete_UserHydratedNodeVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public UserHydratedNodeVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			UserHydratedNodeDefinition x = (UserHydratedNodeDefinition)obj;
			this.Add(x);
		}
	}

	public UserHydratedNodeVector(IEnumerable<UserHydratedNodeDefinition> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (UserHydratedNodeDefinition x in c)
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

	public UserHydratedNodeDefinition this[int index]
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

	public void CopyTo(UserHydratedNodeDefinition[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(UserHydratedNodeDefinition[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, UserHydratedNodeDefinition[] array, int arrayIndex, int count)
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

	public UserHydratedNodeDefinition[] ToArray()
	{
		UserHydratedNodeDefinition[] array = new UserHydratedNodeDefinition[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<UserHydratedNodeDefinition> IEnumerable<UserHydratedNodeDefinition>.GetEnumerator()
	{
		return new UserHydratedNodeVector.UserHydratedNodeVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new UserHydratedNodeVector.UserHydratedNodeVectorEnumerator(this);
	}

	public UserHydratedNodeVector.UserHydratedNodeVectorEnumerator GetEnumerator()
	{
		return new UserHydratedNodeVector.UserHydratedNodeVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.UserHydratedNodeVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(UserHydratedNodeDefinition x)
	{
		MothershipApiPINVOKE.UserHydratedNodeVector_Add(this.swigCPtr, UserHydratedNodeDefinition.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.UserHydratedNodeVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.UserHydratedNodeVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.UserHydratedNodeVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserHydratedNodeVector() : this(MothershipApiPINVOKE.new_UserHydratedNodeVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserHydratedNodeVector(UserHydratedNodeVector other) : this(MothershipApiPINVOKE.new_UserHydratedNodeVector__SWIG_1(UserHydratedNodeVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserHydratedNodeVector(int capacity) : this(MothershipApiPINVOKE.new_UserHydratedNodeVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private UserHydratedNodeDefinition getitemcopy(int index)
	{
		UserHydratedNodeDefinition result = new UserHydratedNodeDefinition(MothershipApiPINVOKE.UserHydratedNodeVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private UserHydratedNodeDefinition getitem(int index)
	{
		UserHydratedNodeDefinition result = new UserHydratedNodeDefinition(MothershipApiPINVOKE.UserHydratedNodeVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, UserHydratedNodeDefinition val)
	{
		MothershipApiPINVOKE.UserHydratedNodeVector_setitem(this.swigCPtr, index, UserHydratedNodeDefinition.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(UserHydratedNodeVector values)
	{
		MothershipApiPINVOKE.UserHydratedNodeVector_AddRange(this.swigCPtr, UserHydratedNodeVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserHydratedNodeVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UserHydratedNodeVector_GetRange(this.swigCPtr, index, count);
		UserHydratedNodeVector result = (intPtr == IntPtr.Zero) ? null : new UserHydratedNodeVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, UserHydratedNodeDefinition x)
	{
		MothershipApiPINVOKE.UserHydratedNodeVector_Insert(this.swigCPtr, index, UserHydratedNodeDefinition.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, UserHydratedNodeVector values)
	{
		MothershipApiPINVOKE.UserHydratedNodeVector_InsertRange(this.swigCPtr, index, UserHydratedNodeVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.UserHydratedNodeVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.UserHydratedNodeVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static UserHydratedNodeVector Repeat(UserHydratedNodeDefinition value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UserHydratedNodeVector_Repeat(UserHydratedNodeDefinition.getCPtr(value), count);
		UserHydratedNodeVector result = (intPtr == IntPtr.Zero) ? null : new UserHydratedNodeVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.UserHydratedNodeVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.UserHydratedNodeVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, UserHydratedNodeVector values)
	{
		MothershipApiPINVOKE.UserHydratedNodeVector_SetRange(this.swigCPtr, index, UserHydratedNodeVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class UserHydratedNodeVectorEnumerator : IEnumerator, IEnumerator<UserHydratedNodeDefinition>, IDisposable
	{
		public UserHydratedNodeVectorEnumerator(UserHydratedNodeVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public UserHydratedNodeDefinition Current
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
				return (UserHydratedNodeDefinition)this.currentObject;
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

		private UserHydratedNodeVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
