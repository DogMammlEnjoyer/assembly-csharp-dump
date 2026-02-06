using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class UserLedgerEntryVector : IDisposable, IEnumerable, IEnumerable<MothershipUserLedgerEntry>
{
	internal UserLedgerEntryVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(UserLedgerEntryVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(UserLedgerEntryVector obj)
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

	~UserLedgerEntryVector()
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
					MothershipApiPINVOKE.delete_UserLedgerEntryVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public UserLedgerEntryVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			MothershipUserLedgerEntry x = (MothershipUserLedgerEntry)obj;
			this.Add(x);
		}
	}

	public UserLedgerEntryVector(IEnumerable<MothershipUserLedgerEntry> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (MothershipUserLedgerEntry x in c)
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

	public MothershipUserLedgerEntry this[int index]
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

	public void CopyTo(MothershipUserLedgerEntry[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(MothershipUserLedgerEntry[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, MothershipUserLedgerEntry[] array, int arrayIndex, int count)
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

	public MothershipUserLedgerEntry[] ToArray()
	{
		MothershipUserLedgerEntry[] array = new MothershipUserLedgerEntry[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<MothershipUserLedgerEntry> IEnumerable<MothershipUserLedgerEntry>.GetEnumerator()
	{
		return new UserLedgerEntryVector.UserLedgerEntryVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new UserLedgerEntryVector.UserLedgerEntryVectorEnumerator(this);
	}

	public UserLedgerEntryVector.UserLedgerEntryVectorEnumerator GetEnumerator()
	{
		return new UserLedgerEntryVector.UserLedgerEntryVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.UserLedgerEntryVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(MothershipUserLedgerEntry x)
	{
		MothershipApiPINVOKE.UserLedgerEntryVector_Add(this.swigCPtr, MothershipUserLedgerEntry.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.UserLedgerEntryVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.UserLedgerEntryVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.UserLedgerEntryVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserLedgerEntryVector() : this(MothershipApiPINVOKE.new_UserLedgerEntryVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserLedgerEntryVector(UserLedgerEntryVector other) : this(MothershipApiPINVOKE.new_UserLedgerEntryVector__SWIG_1(UserLedgerEntryVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserLedgerEntryVector(int capacity) : this(MothershipApiPINVOKE.new_UserLedgerEntryVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private MothershipUserLedgerEntry getitemcopy(int index)
	{
		MothershipUserLedgerEntry result = new MothershipUserLedgerEntry(MothershipApiPINVOKE.UserLedgerEntryVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private MothershipUserLedgerEntry getitem(int index)
	{
		MothershipUserLedgerEntry result = new MothershipUserLedgerEntry(MothershipApiPINVOKE.UserLedgerEntryVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, MothershipUserLedgerEntry val)
	{
		MothershipApiPINVOKE.UserLedgerEntryVector_setitem(this.swigCPtr, index, MothershipUserLedgerEntry.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(UserLedgerEntryVector values)
	{
		MothershipApiPINVOKE.UserLedgerEntryVector_AddRange(this.swigCPtr, UserLedgerEntryVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public UserLedgerEntryVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UserLedgerEntryVector_GetRange(this.swigCPtr, index, count);
		UserLedgerEntryVector result = (intPtr == IntPtr.Zero) ? null : new UserLedgerEntryVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, MothershipUserLedgerEntry x)
	{
		MothershipApiPINVOKE.UserLedgerEntryVector_Insert(this.swigCPtr, index, MothershipUserLedgerEntry.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, UserLedgerEntryVector values)
	{
		MothershipApiPINVOKE.UserLedgerEntryVector_InsertRange(this.swigCPtr, index, UserLedgerEntryVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.UserLedgerEntryVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.UserLedgerEntryVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static UserLedgerEntryVector Repeat(MothershipUserLedgerEntry value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.UserLedgerEntryVector_Repeat(MothershipUserLedgerEntry.getCPtr(value), count);
		UserLedgerEntryVector result = (intPtr == IntPtr.Zero) ? null : new UserLedgerEntryVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.UserLedgerEntryVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.UserLedgerEntryVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, UserLedgerEntryVector values)
	{
		MothershipApiPINVOKE.UserLedgerEntryVector_SetRange(this.swigCPtr, index, UserLedgerEntryVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class UserLedgerEntryVectorEnumerator : IEnumerator, IEnumerator<MothershipUserLedgerEntry>, IDisposable
	{
		public UserLedgerEntryVectorEnumerator(UserLedgerEntryVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public MothershipUserLedgerEntry Current
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
				return (MothershipUserLedgerEntry)this.currentObject;
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

		private UserLedgerEntryVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
