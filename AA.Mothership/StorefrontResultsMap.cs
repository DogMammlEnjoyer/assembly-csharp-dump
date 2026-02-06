using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class StorefrontResultsMap : IDisposable, IDictionary<string, MothershipBoundOfferDisplay>, ICollection<KeyValuePair<string, MothershipBoundOfferDisplay>>, IEnumerable<KeyValuePair<string, MothershipBoundOfferDisplay>>, IEnumerable
{
	internal StorefrontResultsMap(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	internal static HandleRef getCPtr(StorefrontResultsMap obj)
	{
		if (obj != null)
		{
			return obj.swigCPtr;
		}
		return new HandleRef(null, IntPtr.Zero);
	}

	internal static HandleRef swigRelease(StorefrontResultsMap obj)
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

	~StorefrontResultsMap()
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
					MothershipApiPINVOKE.delete_StorefrontResultsMap(this.swigCPtr);
				}
				this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			}
		}
	}

	public MothershipBoundOfferDisplay this[string key]
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

	public bool TryGetValue(string key, out MothershipBoundOfferDisplay value)
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

	public ICollection<MothershipBoundOfferDisplay> Values
	{
		get
		{
			ICollection<MothershipBoundOfferDisplay> collection = new List<MothershipBoundOfferDisplay>();
			foreach (KeyValuePair<string, MothershipBoundOfferDisplay> keyValuePair in this)
			{
				collection.Add(keyValuePair.Value);
			}
			return collection;
		}
	}

	public void Add(KeyValuePair<string, MothershipBoundOfferDisplay> item)
	{
		this.Add(item.Key, item.Value);
	}

	public bool Remove(KeyValuePair<string, MothershipBoundOfferDisplay> item)
	{
		return this.Contains(item) && this.Remove(item.Key);
	}

	public bool Contains(KeyValuePair<string, MothershipBoundOfferDisplay> item)
	{
		return this[item.Key] == item.Value;
	}

	public void CopyTo(KeyValuePair<string, MothershipBoundOfferDisplay>[] array)
	{
		this.CopyTo(array, 0);
	}

	public void CopyTo(KeyValuePair<string, MothershipBoundOfferDisplay>[] array, int arrayIndex)
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
			array.SetValue(new KeyValuePair<string, MothershipBoundOfferDisplay>(key, this[key]), arrayIndex + i);
		}
	}

	IEnumerator<KeyValuePair<string, MothershipBoundOfferDisplay>> IEnumerable<KeyValuePair<string, MothershipBoundOfferDisplay>>.GetEnumerator()
	{
		return new StorefrontResultsMap.StorefrontResultsMapEnumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new StorefrontResultsMap.StorefrontResultsMapEnumerator(this);
	}

	public StorefrontResultsMap.StorefrontResultsMapEnumerator GetEnumerator()
	{
		return new StorefrontResultsMap.StorefrontResultsMapEnumerator(this);
	}

	public StorefrontResultsMap() : this(MothershipApiPINVOKE.new_StorefrontResultsMap__SWIG_0(), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public StorefrontResultsMap(StorefrontResultsMap other) : this(MothershipApiPINVOKE.new_StorefrontResultsMap__SWIG_1(StorefrontResultsMap.getCPtr(other)), true)
	{
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private uint size()
	{
		uint result = MothershipApiPINVOKE.StorefrontResultsMap_size(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public bool empty()
	{
		bool result = MothershipApiPINVOKE.StorefrontResultsMap_empty(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Clear()
	{
		MothershipApiPINVOKE.StorefrontResultsMap_Clear(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private MothershipBoundOfferDisplay getitem(string key)
	{
		MothershipBoundOfferDisplay result = new MothershipBoundOfferDisplay(MothershipApiPINVOKE.StorefrontResultsMap_getitem(this.swigCPtr, key), false);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void setitem(string key, MothershipBoundOfferDisplay x)
	{
		MothershipApiPINVOKE.StorefrontResultsMap_setitem(this.swigCPtr, key, MothershipBoundOfferDisplay.getCPtr(x));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool ContainsKey(string key)
	{
		bool result = MothershipApiPINVOKE.StorefrontResultsMap_ContainsKey(this.swigCPtr, key);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	public void Add(string key, MothershipBoundOfferDisplay val)
	{
		MothershipApiPINVOKE.StorefrontResultsMap_Add(this.swigCPtr, key, MothershipBoundOfferDisplay.getCPtr(val));
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	public bool Remove(string key)
	{
		bool result = MothershipApiPINVOKE.StorefrontResultsMap_Remove(this.swigCPtr, key);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private IntPtr create_iterator_begin()
	{
		IntPtr result = MothershipApiPINVOKE.StorefrontResultsMap_create_iterator_begin(this.swigCPtr);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private string get_next_key(IntPtr swigiterator)
	{
		string result = MothershipApiPINVOKE.StorefrontResultsMap_get_next_key(this.swigCPtr, swigiterator);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	private void destroy_iterator(IntPtr swigiterator)
	{
		MothershipApiPINVOKE.StorefrontResultsMap_destroy_iterator(this.swigCPtr, swigiterator);
		if (MothershipApiPINVOKE.SWIGPendingException.Pending)
		{
			throw MothershipApiPINVOKE.SWIGPendingException.Retrieve();
		}
	}

	private HandleRef swigCPtr;

	protected bool swigCMemOwn;

	public sealed class StorefrontResultsMapEnumerator : IEnumerator, IEnumerator<KeyValuePair<string, MothershipBoundOfferDisplay>>, IDisposable
	{
		public StorefrontResultsMapEnumerator(StorefrontResultsMap collection)
		{
			this.collectionRef = collection;
			this.keyCollection = new List<string>(collection.Keys);
			this.currentIndex = -1;
			this.currentObject = null;
			this.currentSize = this.collectionRef.Count;
		}

		public KeyValuePair<string, MothershipBoundOfferDisplay> Current
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
				return (KeyValuePair<string, MothershipBoundOfferDisplay>)this.currentObject;
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
				this.currentObject = new KeyValuePair<string, MothershipBoundOfferDisplay>(key, this.collectionRef[key]);
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

		private StorefrontResultsMap collectionRef;

		private IList<string> keyCollection;

		private int currentIndex;

		private object currentObject;

		private int currentSize;
	}
}
