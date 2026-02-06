using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class OfferEntitlementMap : IDisposable, IDictionary<string, OfferEntitlementChanges>, ICollection<KeyValuePair<string, OfferEntitlementChanges>>, IEnumerable<KeyValuePair<string, OfferEntitlementChanges>>, IEnumerable
{
	internal OfferEntitlementMap(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(OfferEntitlementMap obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(OfferEntitlementMap obj)
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

	~OfferEntitlementMap()
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
					MothershipApiPINVOKE.delete_OfferEntitlementMap(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public OfferEntitlementChanges this[string key]
	{
		get
		{
			return this.getitem(key);
		}
		set
		{
			this.setitem(key, value);
		}
	}

	public bool TryGetValue(string key, out OfferEntitlementChanges value)
	{
		if (this.ContainsKey(key))
		{
			value = this[key];
			return true;
		}
		value = null;
		return false;
	}

	public int Count
	{
		get
		{
			return (int)this.size();
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return false;
		}
	}

	public ICollection<string> Keys
	{
		get
		{
			ICollection<string> collection = new List<string>();
			int count = this.Count;
			if (count > 0)
			{
				IntPtr swigiterator = this.create_iterator_begin();
				for (int i = 0; i < count; i++)
				{
					collection.Add(this.get_next_key(swigiterator));
				}
				this.destroy_iterator(swigiterator);
			}
			return collection;
		}
	}

	public ICollection<OfferEntitlementChanges> Values
	{
		get
		{
			ICollection<OfferEntitlementChanges> collection = new List<OfferEntitlementChanges>();
			foreach (KeyValuePair<string, OfferEntitlementChanges> keyValuePair in this)
			{
				collection.Add(keyValuePair.Value);
			}
			return collection;
		}
	}

	public void Add(KeyValuePair<string, OfferEntitlementChanges> item)
	{
		this.Add(item.Key, item.Value);
	}

	public bool Remove(KeyValuePair<string, OfferEntitlementChanges> item)
	{
		return this.Contains(item) && this.Remove(item.Key);
	}

	public bool Contains(KeyValuePair<string, OfferEntitlementChanges> item)
	{
		return this[item.Key] == item.Value;
	}

	public void CopyTo(KeyValuePair<string, OfferEntitlementChanges>[] array)
	{
		this.CopyTo(array, 0);
	}

	public void CopyTo(KeyValuePair<string, OfferEntitlementChanges>[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0)
		{
			throw new ArgumentOutOfRangeException("arrayIndex", "Value is less than zero");
		}
		if (array.Rank > 1)
		{
			throw new ArgumentException("Multi dimensional array.", "array");
		}
		if (arrayIndex + this.Count > array.Length)
		{
			throw new ArgumentException("Number of elements to copy is too large.");
		}
		IList<string> list = new List<string>(this.Keys);
		for (int i = 0; i < list.Count; i++)
		{
			string key = list[i];
			array.SetValue(new KeyValuePair<string, OfferEntitlementChanges>(key, this[key]), arrayIndex + i);
		}
	}

	IEnumerator<KeyValuePair<string, OfferEntitlementChanges>> IEnumerable<KeyValuePair<string, OfferEntitlementChanges>>.GetEnumerator()
	{
		return new OfferEntitlementMap.OfferEntitlementMapEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new OfferEntitlementMap.OfferEntitlementMapEnumerator(this);
	}

	public OfferEntitlementMap.OfferEntitlementMapEnumerator GetEnumerator()
	{
		return new OfferEntitlementMap.OfferEntitlementMapEnumerator(this);
	}

	public OfferEntitlementMap() : this(MothershipApiPINVOKE.new_OfferEntitlementMap__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public OfferEntitlementMap(OfferEntitlementMap other) : this(MothershipApiPINVOKE.new_OfferEntitlementMap__SWIG_1(OfferEntitlementMap.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.OfferEntitlementMap_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public bool empty()
	{
		bool result = MothershipApiPINVOKE.OfferEntitlementMap_empty(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Clear()
	{
		MothershipApiPINVOKE.OfferEntitlementMap_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private OfferEntitlementChanges getitem(string key)
	{
		OfferEntitlementChanges result = new OfferEntitlementChanges(MothershipApiPINVOKE.OfferEntitlementMap_getitem(this.swigCPtr, key), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(string key, OfferEntitlementChanges x)
	{
		MothershipApiPINVOKE.OfferEntitlementMap_setitem(this.swigCPtr, key, OfferEntitlementChanges.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ContainsKey(string key)
	{
		bool result = MothershipApiPINVOKE.OfferEntitlementMap_ContainsKey(this.swigCPtr, key);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Add(string key, OfferEntitlementChanges val)
	{
		MothershipApiPINVOKE.OfferEntitlementMap_Add(this.swigCPtr, key, OfferEntitlementChanges.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool Remove(string key)
	{
		bool result = MothershipApiPINVOKE.OfferEntitlementMap_Remove(this.swigCPtr, key);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private IntPtr create_iterator_begin()
	{
		IntPtr result = MothershipApiPINVOKE.OfferEntitlementMap_create_iterator_begin(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private string get_next_key(IntPtr swigiterator)
	{
		string result = MothershipApiPINVOKE.OfferEntitlementMap_get_next_key(this.swigCPtr, swigiterator);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void destroy_iterator(IntPtr swigiterator)
	{
		MothershipApiPINVOKE.OfferEntitlementMap_destroy_iterator(this.swigCPtr, swigiterator);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class OfferEntitlementMapEnumerator : IEnumerator, IEnumerator<KeyValuePair<string, OfferEntitlementChanges>>, IDisposable
	{
		public OfferEntitlementMapEnumerator(OfferEntitlementMap collection)
		{
			this.collectionRef = collection;
			this.keyCollection = new List<string>(collection.Keys);
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public KeyValuePair<string, OfferEntitlementChanges> Current
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
				return (KeyValuePair<string, OfferEntitlementChanges>)this.currentObject;
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
				string key = this.keyCollection[this.currentIndex];
				this.currentObject = new KeyValuePair<string, OfferEntitlementChanges>(key, this.collectionRef[key]);
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

		private OfferEntitlementMap collectionRef;

		private IList<string> keyCollection;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
