using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class SubscriptionsByPlayerMap : IDisposable, IDictionary<string, SubscriptionsVector>, ICollection<KeyValuePair<string, SubscriptionsVector>>, IEnumerable<KeyValuePair<string, SubscriptionsVector>>, IEnumerable
{
	internal SubscriptionsByPlayerMap(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(SubscriptionsByPlayerMap obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(SubscriptionsByPlayerMap obj)
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

	~SubscriptionsByPlayerMap()
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
					MothershipApiPINVOKE.delete_SubscriptionsByPlayerMap(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public SubscriptionsVector this[string key]
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

	public bool TryGetValue(string key, out SubscriptionsVector value)
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

	public ICollection<SubscriptionsVector> Values
	{
		get
		{
			ICollection<SubscriptionsVector> collection = new List<SubscriptionsVector>();
			foreach (KeyValuePair<string, SubscriptionsVector> keyValuePair in this)
			{
				collection.Add(keyValuePair.Value);
			}
			return collection;
		}
	}

	public void Add(KeyValuePair<string, SubscriptionsVector> item)
	{
		this.Add(item.Key, item.Value);
	}

	public bool Remove(KeyValuePair<string, SubscriptionsVector> item)
	{
		return this.Contains(item) && this.Remove(item.Key);
	}

	public bool Contains(KeyValuePair<string, SubscriptionsVector> item)
	{
		return this[item.Key] == item.Value;
	}

	public void CopyTo(KeyValuePair<string, SubscriptionsVector>[] array)
	{
		this.CopyTo(array, 0);
	}

	public void CopyTo(KeyValuePair<string, SubscriptionsVector>[] array, int arrayIndex)
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
			array.SetValue(new KeyValuePair<string, SubscriptionsVector>(key, this[key]), arrayIndex + i);
		}
	}

	IEnumerator<KeyValuePair<string, SubscriptionsVector>> IEnumerable<KeyValuePair<string, SubscriptionsVector>>.GetEnumerator()
	{
		return new SubscriptionsByPlayerMap.SubscriptionsByPlayerMapEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new SubscriptionsByPlayerMap.SubscriptionsByPlayerMapEnumerator(this);
	}

	public SubscriptionsByPlayerMap.SubscriptionsByPlayerMapEnumerator GetEnumerator()
	{
		return new SubscriptionsByPlayerMap.SubscriptionsByPlayerMapEnumerator(this);
	}

	public SubscriptionsByPlayerMap() : this(MothershipApiPINVOKE.new_SubscriptionsByPlayerMap__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public SubscriptionsByPlayerMap(SubscriptionsByPlayerMap other) : this(MothershipApiPINVOKE.new_SubscriptionsByPlayerMap__SWIG_1(SubscriptionsByPlayerMap.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.SubscriptionsByPlayerMap_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public bool empty()
	{
		bool result = MothershipApiPINVOKE.SubscriptionsByPlayerMap_empty(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Clear()
	{
		MothershipApiPINVOKE.SubscriptionsByPlayerMap_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private SubscriptionsVector getitem(string key)
	{
		SubscriptionsVector result = new SubscriptionsVector(MothershipApiPINVOKE.SubscriptionsByPlayerMap_getitem(this.swigCPtr, key), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(string key, SubscriptionsVector x)
	{
		MothershipApiPINVOKE.SubscriptionsByPlayerMap_setitem(this.swigCPtr, key, SubscriptionsVector.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ContainsKey(string key)
	{
		bool result = MothershipApiPINVOKE.SubscriptionsByPlayerMap_ContainsKey(this.swigCPtr, key);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Add(string key, SubscriptionsVector val)
	{
		MothershipApiPINVOKE.SubscriptionsByPlayerMap_Add(this.swigCPtr, key, SubscriptionsVector.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool Remove(string key)
	{
		bool result = MothershipApiPINVOKE.SubscriptionsByPlayerMap_Remove(this.swigCPtr, key);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private IntPtr create_iterator_begin()
	{
		IntPtr result = MothershipApiPINVOKE.SubscriptionsByPlayerMap_create_iterator_begin(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private string get_next_key(IntPtr swigiterator)
	{
		string result = MothershipApiPINVOKE.SubscriptionsByPlayerMap_get_next_key(this.swigCPtr, swigiterator);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void destroy_iterator(IntPtr swigiterator)
	{
		MothershipApiPINVOKE.SubscriptionsByPlayerMap_destroy_iterator(this.swigCPtr, swigiterator);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class SubscriptionsByPlayerMapEnumerator : IEnumerator, IEnumerator<KeyValuePair<string, SubscriptionsVector>>, IDisposable
	{
		public SubscriptionsByPlayerMapEnumerator(SubscriptionsByPlayerMap collection)
		{
			this.collectionRef = collection;
			this.keyCollection = new List<string>(collection.Keys);
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public KeyValuePair<string, SubscriptionsVector> Current
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
				return (KeyValuePair<string, SubscriptionsVector>)this.currentObject;
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
				this.currentObject = new KeyValuePair<string, SubscriptionsVector>(key, this.collectionRef[key]);
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

		private SubscriptionsByPlayerMap collectionRef;

		private IList<string> keyCollection;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
