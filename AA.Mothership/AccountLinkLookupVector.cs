using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class AccountLinkLookupVector : IDisposable, IEnumerable, IEnumerable<AccountLinkLookupEntry>
{
	internal AccountLinkLookupVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(AccountLinkLookupVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(AccountLinkLookupVector obj)
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

	~AccountLinkLookupVector()
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
					MothershipApiPINVOKE.delete_AccountLinkLookupVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public AccountLinkLookupVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			AccountLinkLookupEntry x = (AccountLinkLookupEntry)obj;
			this.Add(x);
		}
	}

	public AccountLinkLookupVector(IEnumerable<AccountLinkLookupEntry> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (AccountLinkLookupEntry x in c)
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

	public AccountLinkLookupEntry this[int index]
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

	public void CopyTo(AccountLinkLookupEntry[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(AccountLinkLookupEntry[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, AccountLinkLookupEntry[] array, int arrayIndex, int count)
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

	public AccountLinkLookupEntry[] ToArray()
	{
		AccountLinkLookupEntry[] array = new AccountLinkLookupEntry[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<AccountLinkLookupEntry> IEnumerable<AccountLinkLookupEntry>.GetEnumerator()
	{
		return new AccountLinkLookupVector.AccountLinkLookupVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new AccountLinkLookupVector.AccountLinkLookupVectorEnumerator(this);
	}

	public AccountLinkLookupVector.AccountLinkLookupVectorEnumerator GetEnumerator()
	{
		return new AccountLinkLookupVector.AccountLinkLookupVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.AccountLinkLookupVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(AccountLinkLookupEntry x)
	{
		MothershipApiPINVOKE.AccountLinkLookupVector_Add(this.swigCPtr, AccountLinkLookupEntry.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.AccountLinkLookupVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.AccountLinkLookupVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.AccountLinkLookupVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public AccountLinkLookupVector() : this(MothershipApiPINVOKE.new_AccountLinkLookupVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public AccountLinkLookupVector(AccountLinkLookupVector other) : this(MothershipApiPINVOKE.new_AccountLinkLookupVector__SWIG_1(AccountLinkLookupVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public AccountLinkLookupVector(int capacity) : this(MothershipApiPINVOKE.new_AccountLinkLookupVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private AccountLinkLookupEntry getitemcopy(int index)
	{
		AccountLinkLookupEntry result = new AccountLinkLookupEntry(MothershipApiPINVOKE.AccountLinkLookupVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private AccountLinkLookupEntry getitem(int index)
	{
		AccountLinkLookupEntry result = new AccountLinkLookupEntry(MothershipApiPINVOKE.AccountLinkLookupVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, AccountLinkLookupEntry val)
	{
		MothershipApiPINVOKE.AccountLinkLookupVector_setitem(this.swigCPtr, index, AccountLinkLookupEntry.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(AccountLinkLookupVector values)
	{
		MothershipApiPINVOKE.AccountLinkLookupVector_AddRange(this.swigCPtr, AccountLinkLookupVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public AccountLinkLookupVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.AccountLinkLookupVector_GetRange(this.swigCPtr, index, count);
		AccountLinkLookupVector result = (intPtr == IntPtr.Zero) ? null : new AccountLinkLookupVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, AccountLinkLookupEntry x)
	{
		MothershipApiPINVOKE.AccountLinkLookupVector_Insert(this.swigCPtr, index, AccountLinkLookupEntry.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, AccountLinkLookupVector values)
	{
		MothershipApiPINVOKE.AccountLinkLookupVector_InsertRange(this.swigCPtr, index, AccountLinkLookupVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.AccountLinkLookupVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.AccountLinkLookupVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static AccountLinkLookupVector Repeat(AccountLinkLookupEntry value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.AccountLinkLookupVector_Repeat(AccountLinkLookupEntry.getCPtr(value), count);
		AccountLinkLookupVector result = (intPtr == IntPtr.Zero) ? null : new AccountLinkLookupVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.AccountLinkLookupVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.AccountLinkLookupVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, AccountLinkLookupVector values)
	{
		MothershipApiPINVOKE.AccountLinkLookupVector_SetRange(this.swigCPtr, index, AccountLinkLookupVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class AccountLinkLookupVectorEnumerator : IEnumerator, IEnumerator<AccountLinkLookupEntry>, IDisposable
	{
		public AccountLinkLookupVectorEnumerator(AccountLinkLookupVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public AccountLinkLookupEntry Current
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
				return (AccountLinkLookupEntry)this.currentObject;
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

		private AccountLinkLookupVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
