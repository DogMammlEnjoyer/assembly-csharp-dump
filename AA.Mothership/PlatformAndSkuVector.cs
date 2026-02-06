using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class PlatformAndSkuVector : IDisposable, IEnumerable, IEnumerable<SubscriptionPlatformAndSku>
{
	internal PlatformAndSkuVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(PlatformAndSkuVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(PlatformAndSkuVector obj)
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

	~PlatformAndSkuVector()
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
					MothershipApiPINVOKE.delete_PlatformAndSkuVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public PlatformAndSkuVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			SubscriptionPlatformAndSku x = (SubscriptionPlatformAndSku)obj;
			this.Add(x);
		}
	}

	public PlatformAndSkuVector(IEnumerable<SubscriptionPlatformAndSku> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (SubscriptionPlatformAndSku x in c)
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

	public SubscriptionPlatformAndSku this[int index]
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

	public void CopyTo(SubscriptionPlatformAndSku[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(SubscriptionPlatformAndSku[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, SubscriptionPlatformAndSku[] array, int arrayIndex, int count)
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

	public SubscriptionPlatformAndSku[] ToArray()
	{
		SubscriptionPlatformAndSku[] array = new SubscriptionPlatformAndSku[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<SubscriptionPlatformAndSku> IEnumerable<SubscriptionPlatformAndSku>.GetEnumerator()
	{
		return new PlatformAndSkuVector.PlatformAndSkuVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new PlatformAndSkuVector.PlatformAndSkuVectorEnumerator(this);
	}

	public PlatformAndSkuVector.PlatformAndSkuVectorEnumerator GetEnumerator()
	{
		return new PlatformAndSkuVector.PlatformAndSkuVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.PlatformAndSkuVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(SubscriptionPlatformAndSku x)
	{
		MothershipApiPINVOKE.PlatformAndSkuVector_Add(this.swigCPtr, SubscriptionPlatformAndSku.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.PlatformAndSkuVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.PlatformAndSkuVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.PlatformAndSkuVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public PlatformAndSkuVector() : this(MothershipApiPINVOKE.new_PlatformAndSkuVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public PlatformAndSkuVector(PlatformAndSkuVector other) : this(MothershipApiPINVOKE.new_PlatformAndSkuVector__SWIG_1(PlatformAndSkuVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public PlatformAndSkuVector(int capacity) : this(MothershipApiPINVOKE.new_PlatformAndSkuVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private SubscriptionPlatformAndSku getitemcopy(int index)
	{
		SubscriptionPlatformAndSku result = new SubscriptionPlatformAndSku(MothershipApiPINVOKE.PlatformAndSkuVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private SubscriptionPlatformAndSku getitem(int index)
	{
		SubscriptionPlatformAndSku result = new SubscriptionPlatformAndSku(MothershipApiPINVOKE.PlatformAndSkuVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, SubscriptionPlatformAndSku val)
	{
		MothershipApiPINVOKE.PlatformAndSkuVector_setitem(this.swigCPtr, index, SubscriptionPlatformAndSku.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(PlatformAndSkuVector values)
	{
		MothershipApiPINVOKE.PlatformAndSkuVector_AddRange(this.swigCPtr, PlatformAndSkuVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public PlatformAndSkuVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.PlatformAndSkuVector_GetRange(this.swigCPtr, index, count);
		PlatformAndSkuVector result = (intPtr == IntPtr.Zero) ? null : new PlatformAndSkuVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, SubscriptionPlatformAndSku x)
	{
		MothershipApiPINVOKE.PlatformAndSkuVector_Insert(this.swigCPtr, index, SubscriptionPlatformAndSku.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, PlatformAndSkuVector values)
	{
		MothershipApiPINVOKE.PlatformAndSkuVector_InsertRange(this.swigCPtr, index, PlatformAndSkuVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.PlatformAndSkuVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.PlatformAndSkuVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static PlatformAndSkuVector Repeat(SubscriptionPlatformAndSku value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.PlatformAndSkuVector_Repeat(SubscriptionPlatformAndSku.getCPtr(value), count);
		PlatformAndSkuVector result = (intPtr == IntPtr.Zero) ? null : new PlatformAndSkuVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.PlatformAndSkuVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.PlatformAndSkuVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, PlatformAndSkuVector values)
	{
		MothershipApiPINVOKE.PlatformAndSkuVector_SetRange(this.swigCPtr, index, PlatformAndSkuVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class PlatformAndSkuVectorEnumerator : IEnumerator, IEnumerator<SubscriptionPlatformAndSku>, IDisposable
	{
		public PlatformAndSkuVectorEnumerator(PlatformAndSkuVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public SubscriptionPlatformAndSku Current
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
				return (SubscriptionPlatformAndSku)this.currentObject;
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

		private PlatformAndSkuVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
