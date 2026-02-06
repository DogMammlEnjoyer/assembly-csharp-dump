using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ListDeploymentsResultVector : IDisposable, IEnumerable, IEnumerable<MothershipTitleEnvDeployment>
{
	internal ListDeploymentsResultVector(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(ListDeploymentsResultVector obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(ListDeploymentsResultVector obj)
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

	~ListDeploymentsResultVector()
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
					MothershipApiPINVOKE.delete_ListDeploymentsResultVector(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public ListDeploymentsResultVector(IEnumerable c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (object obj in c)
		{
			MothershipTitleEnvDeployment x = (MothershipTitleEnvDeployment)obj;
			this.Add(x);
		}
	}

	public ListDeploymentsResultVector(IEnumerable<MothershipTitleEnvDeployment> c) : this()
	{
		if (c == null)
		{
			throw new ArgumentNullException("c");
		}
		foreach (MothershipTitleEnvDeployment x in c)
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

	public MothershipTitleEnvDeployment this[int index]
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

	public void CopyTo(MothershipTitleEnvDeployment[] array)
	{
		this.CopyTo(0, array, 0, this.Count);
	}

	public void CopyTo(MothershipTitleEnvDeployment[] array, int arrayIndex)
	{
		this.CopyTo(0, array, arrayIndex, this.Count);
	}

	public void CopyTo(int index, MothershipTitleEnvDeployment[] array, int arrayIndex, int count)
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

	public MothershipTitleEnvDeployment[] ToArray()
	{
		MothershipTitleEnvDeployment[] array = new MothershipTitleEnvDeployment[this.Count];
		this.CopyTo(array);
		return array;
	}

	IEnumerator<MothershipTitleEnvDeployment> IEnumerable<MothershipTitleEnvDeployment>.GetEnumerator()
	{
		return new ListDeploymentsResultVector.ListDeploymentsResultVectorEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new ListDeploymentsResultVector.ListDeploymentsResultVectorEnumerator(this);
	}

	public ListDeploymentsResultVector.ListDeploymentsResultVectorEnumerator GetEnumerator()
	{
		return new ListDeploymentsResultVector.ListDeploymentsResultVectorEnumerator(this);
	}

	public void Clear()
	{
		MothershipApiPINVOKE.ListDeploymentsResultVector_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Add(MothershipTitleEnvDeployment x)
	{
		MothershipApiPINVOKE.ListDeploymentsResultVector_Add(this.swigCPtr, MothershipTitleEnvDeployment.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.ListDeploymentsResultVector_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private uint capacity()
	{
		uint result = MothershipApiPINVOKE.ListDeploymentsResultVector_capacity(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void reserve(uint n)
	{
		MothershipApiPINVOKE.ListDeploymentsResultVector_reserve(this.swigCPtr, n);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListDeploymentsResultVector() : this(MothershipApiPINVOKE.new_ListDeploymentsResultVector__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListDeploymentsResultVector(ListDeploymentsResultVector other) : this(MothershipApiPINVOKE.new_ListDeploymentsResultVector__SWIG_1(ListDeploymentsResultVector.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListDeploymentsResultVector(int capacity) : this(MothershipApiPINVOKE.new_ListDeploymentsResultVector__SWIG_2(capacity), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private MothershipTitleEnvDeployment getitemcopy(int index)
	{
		MothershipTitleEnvDeployment result = new MothershipTitleEnvDeployment(MothershipApiPINVOKE.ListDeploymentsResultVector_getitemcopy(this.swigCPtr, index), true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private MothershipTitleEnvDeployment getitem(int index)
	{
		MothershipTitleEnvDeployment result = new MothershipTitleEnvDeployment(MothershipApiPINVOKE.ListDeploymentsResultVector_getitem(this.swigCPtr, index), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(int index, MothershipTitleEnvDeployment val)
	{
		MothershipApiPINVOKE.ListDeploymentsResultVector_setitem(this.swigCPtr, index, MothershipTitleEnvDeployment.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void AddRange(ListDeploymentsResultVector values)
	{
		MothershipApiPINVOKE.ListDeploymentsResultVector_AddRange(this.swigCPtr, ListDeploymentsResultVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public ListDeploymentsResultVector GetRange(int index, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ListDeploymentsResultVector_GetRange(this.swigCPtr, index, count);
		ListDeploymentsResultVector result = (intPtr == IntPtr.Zero) ? null : new ListDeploymentsResultVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Insert(int index, MothershipTitleEnvDeployment x)
	{
		MothershipApiPINVOKE.ListDeploymentsResultVector_Insert(this.swigCPtr, index, MothershipTitleEnvDeployment.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void InsertRange(int index, ListDeploymentsResultVector values)
	{
		MothershipApiPINVOKE.ListDeploymentsResultVector_InsertRange(this.swigCPtr, index, ListDeploymentsResultVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveAt(int index)
	{
		MothershipApiPINVOKE.ListDeploymentsResultVector_RemoveAt(this.swigCPtr, index);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void RemoveRange(int index, int count)
	{
		MothershipApiPINVOKE.ListDeploymentsResultVector_RemoveRange(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public static ListDeploymentsResultVector Repeat(MothershipTitleEnvDeployment value, int count)
	{
		IntPtr intPtr = MothershipApiPINVOKE.ListDeploymentsResultVector_Repeat(MothershipTitleEnvDeployment.getCPtr(value), count);
		ListDeploymentsResultVector result = (intPtr == IntPtr.Zero) ? null : new ListDeploymentsResultVector(intPtr, true);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Reverse()
	{
		MothershipApiPINVOKE.ListDeploymentsResultVector_Reverse__SWIG_0(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void Reverse(int index, int count)
	{
		MothershipApiPINVOKE.ListDeploymentsResultVector_Reverse__SWIG_1(this.swigCPtr, index, count);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public void SetRange(int index, ListDeploymentsResultVector values)
	{
		MothershipApiPINVOKE.ListDeploymentsResultVector_SetRange(this.swigCPtr, index, ListDeploymentsResultVector.getCPtr(values));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class ListDeploymentsResultVectorEnumerator : IEnumerator, IEnumerator<MothershipTitleEnvDeployment>, IDisposable
	{
		public ListDeploymentsResultVectorEnumerator(ListDeploymentsResultVector collection)
		{
			this.collectionRef = collection;
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public MothershipTitleEnvDeployment Current
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
				return (MothershipTitleEnvDeployment)this.currentObject;
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

		private ListDeploymentsResultVector collectionRef;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
