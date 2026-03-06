using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Splines
{
	[Serializable]
	internal class SplineDataDictionary<T> : IEnumerable<SplineDataKeyValuePair<T>>, IEnumerable
	{
		public IEnumerable<string> Keys
		{
			get
			{
				return from x in this.m_Data
				select x.Key;
			}
		}

		public IEnumerable<SplineData<T>> Values
		{
			get
			{
				return from x in this.m_Data
				select x.Value;
			}
		}

		private int FindIndex(string key)
		{
			int i = 0;
			int count = this.m_Data.Count;
			while (i < count)
			{
				if (this.m_Data[i].Key == key)
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		public bool TryGetValue(string key, out SplineData<T> value)
		{
			int num = this.FindIndex(key);
			value = ((num < 0) ? null : this.m_Data[num].Value);
			return num > -1;
		}

		public SplineData<T> GetOrCreate(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			SplineData<T> result;
			if (!this.TryGetValue(key, out result))
			{
				List<SplineDataKeyValuePair<T>> data = this.m_Data;
				SplineDataKeyValuePair<T> splineDataKeyValuePair = new SplineDataKeyValuePair<T>();
				splineDataKeyValuePair.Key = key;
				result = (splineDataKeyValuePair.Value = new SplineData<T>());
				data.Add(splineDataKeyValuePair);
			}
			return result;
		}

		public SplineData<T> this[string key]
		{
			get
			{
				SplineData<T> result;
				if (!this.TryGetValue(key, out result))
				{
					return null;
				}
				return result;
			}
			set
			{
				int num = this.FindIndex(key);
				SplineData<T> value2 = new SplineData<T>(value);
				if (num < 0)
				{
					this.m_Data.Add(new SplineDataKeyValuePair<T>
					{
						Key = key,
						Value = value2
					});
					return;
				}
				this.m_Data[num].Value = value2;
			}
		}

		public bool Contains(string key)
		{
			return this.FindIndex(key) > -1;
		}

		public IEnumerator<SplineDataKeyValuePair<T>> GetEnumerator()
		{
			return this.m_Data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.m_Data).GetEnumerator();
		}

		public bool Remove(string key)
		{
			int num = this.FindIndex(key);
			if (num < 0)
			{
				return false;
			}
			this.m_Data.RemoveAt(num);
			return true;
		}

		public void RemoveEmpty()
		{
			int i = this.m_Data.Count - 1;
			while (i > -1)
			{
				if (string.IsNullOrEmpty(this.m_Data[i].Key))
				{
					goto IL_4A;
				}
				SplineData<T> value = this.m_Data[i].Value;
				if (value != null && value.Count < 1)
				{
					goto IL_4A;
				}
				IL_80:
				i--;
				continue;
				IL_4A:
				Debug.Log(string.Format("{0} remove empty key \"{1}\"", typeof(T), this.m_Data[i].Key));
				this.m_Data.RemoveAt(i);
				goto IL_80;
			}
		}

		[SerializeField]
		private List<SplineDataKeyValuePair<T>> m_Data = new List<SplineDataKeyValuePair<T>>();
	}
}
