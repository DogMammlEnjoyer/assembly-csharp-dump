using System;
using System.Collections.Generic;

namespace UnityEngine.ResourceManagement.Profiling
{
	internal class ProfilerFrameData<T1, T2>
	{
		internal Dictionary<T1, T2> Data
		{
			get
			{
				return this.m_Data;
			}
		}

		public ProfilerFrameData()
		{
			this.m_Data = new Dictionary<T1, T2>(32);
		}

		public ProfilerFrameData(int count)
		{
			this.m_Data = new Dictionary<T1, T2>(count);
		}

		public bool Add(T1 key, T2 value)
		{
			int num = this.m_Data.ContainsKey(key) ? 1 : 0;
			this.m_Data[key] = value;
			this.m_Version += 1U;
			return num == 0;
		}

		internal bool Remove(T1 key)
		{
			bool flag = this.m_Data.Remove(key);
			if (flag)
			{
				this.m_Version += 1U;
			}
			return flag;
		}

		public T2[] Values
		{
			get
			{
				if (this.m_ArrayVersion == this.m_Version)
				{
					return this.m_Array ?? Array.Empty<T2>();
				}
				this.m_Array = new T2[this.m_Data.Count];
				this.m_Data.Values.CopyTo(this.m_Array, 0);
				this.m_ArrayVersion = this.m_Version;
				return this.m_Array;
			}
		}

		public T2 this[T1 key]
		{
			get
			{
				T2 result;
				if (!this.m_Data.TryGetValue(key, out result))
				{
					throw new ArgumentOutOfRangeException("Key " + key.ToString() + " not found for FrameData");
				}
				return result;
			}
			set
			{
				T2 t;
				if (this.m_Array != null && this.m_Data.TryGetValue(key, out t))
				{
					for (int i = 0; i < this.m_Array.Length; i++)
					{
						if (this.m_Array[i].Equals(t))
						{
							this.m_Array[i] = value;
							break;
						}
					}
				}
				this.m_Data[key] = value;
			}
		}

		public bool TryGetValue(T1 key, out T2 value)
		{
			return this.m_Data.TryGetValue(key, out value);
		}

		public bool ContainsKey(T1 key)
		{
			return this.m_Data.ContainsKey(key);
		}

		public IEnumerable<KeyValuePair<T1, T2>> Enumerate()
		{
			foreach (KeyValuePair<T1, T2> keyValuePair in this.m_Data)
			{
				yield return keyValuePair;
			}
			Dictionary<T1, T2>.Enumerator enumerator = default(Dictionary<T1, T2>.Enumerator);
			yield break;
			yield break;
		}

		private Dictionary<T1, T2> m_Data;

		private T2[] m_Array;

		private uint m_Version;

		private uint m_ArrayVersion;
	}
}
