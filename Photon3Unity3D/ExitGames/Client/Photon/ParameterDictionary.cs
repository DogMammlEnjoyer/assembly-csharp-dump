using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

namespace ExitGames.Client.Photon
{
	public class ParameterDictionary : IEnumerable<KeyValuePair<byte, object>>, IEnumerable
	{
		public ParameterDictionary()
		{
			this.paramDict = new NonAllocDictionary<byte, object>(29U);
		}

		public ParameterDictionary(int capacity)
		{
			this.paramDict = new NonAllocDictionary<byte, object>((uint)capacity);
		}

		public static implicit operator NonAllocDictionary<byte, object>(ParameterDictionary value)
		{
			return value.paramDict;
		}

		IEnumerator<KeyValuePair<byte, object>> IEnumerable<KeyValuePair<byte, object>>.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<byte, object>>)this.paramDict).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<byte, object>>)this.paramDict).GetEnumerator();
		}

		public NonAllocDictionary<byte, object>.PairIterator GetEnumerator()
		{
			return this.paramDict.GetEnumerator();
		}

		public object this[byte key]
		{
			get
			{
				object obj = this.paramDict[key];
				StructWrapper<object> structWrapper = obj as StructWrapper<object>;
				bool flag = structWrapper == null;
				object result;
				if (flag)
				{
					result = obj;
				}
				else
				{
					result = structWrapper;
				}
				return result;
			}
			set
			{
				this.paramDict[key] = value;
			}
		}

		public int Count
		{
			get
			{
				return this.paramDict.Count;
			}
		}

		public void Clear()
		{
			this.wrapperPools.Clear();
			this.paramDict.Clear();
		}

		public void Add(byte code, string value)
		{
			bool flag = this.paramDict.ContainsKey(code);
			if (flag)
			{
				Debug.LogWarning(code.ToString() + " already exists as key in ParameterDictionary");
			}
			this.paramDict[code] = value;
		}

		public void Add(byte code, Hashtable value)
		{
			bool flag = this.paramDict.ContainsKey(code);
			if (flag)
			{
				Debug.LogWarning(code.ToString() + " already exists as key in ParameterDictionary");
			}
			this.paramDict[code] = value;
		}

		public void Add(byte code, byte value)
		{
			bool flag = this.paramDict.ContainsKey(code);
			if (flag)
			{
				Debug.LogError(code.ToString() + " already exists as key in ParameterDictionary");
			}
			StructWrapper<byte> value2 = StructWrapperPools.mappedByteWrappers[(int)value];
			this.paramDict[code] = value2;
		}

		public void Add(byte code, bool value)
		{
			bool flag = this.paramDict.ContainsKey(code);
			if (flag)
			{
				Debug.LogError(code.ToString() + " already exists as key in ParameterDictionary");
			}
			StructWrapper<bool> value2 = StructWrapperPools.mappedBoolWrappers[value ? 1 : 0];
			this.paramDict[code] = value2;
		}

		public void Add(byte code, short value)
		{
			bool flag = this.paramDict.ContainsKey(code);
			if (flag)
			{
				Debug.LogWarning(code.ToString() + " already exists as key in ParameterDictionary");
			}
			this.paramDict[code] = value;
		}

		public void Add(byte code, int value)
		{
			bool flag = this.paramDict.ContainsKey(code);
			if (flag)
			{
				Debug.LogWarning(code.ToString() + " already exists as key in ParameterDictionary");
			}
			this.paramDict[code] = value;
		}

		public void Add(byte code, long value)
		{
			bool flag = this.paramDict.ContainsKey(code);
			if (flag)
			{
				Debug.LogWarning(code.ToString() + " already exists as key in ParameterDictionary");
			}
			this.paramDict[code] = value;
		}

		public void Add(byte code, object value)
		{
			bool flag = this.paramDict.ContainsKey(code);
			if (flag)
			{
				Debug.LogWarning(code.ToString() + " already exists as key in ParameterDictionary");
			}
			this.paramDict[code] = value;
		}

		public T Unwrap<T>(byte key)
		{
			object obj = this.paramDict[key];
			return obj.Unwrap<T>();
		}

		public T Get<T>(byte key)
		{
			object obj = this.paramDict[key];
			return obj.Get<T>();
		}

		public bool ContainsKey(byte key)
		{
			return this.paramDict.ContainsKey(key);
		}

		public object TryGetObject(byte key)
		{
			object obj;
			bool flag = this.paramDict.TryGetValue(key, out obj);
			object result;
			if (flag)
			{
				result = obj;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public bool TryGetValue(byte key, out object value)
		{
			return this.paramDict.TryGetValue(key, out value);
		}

		public bool TryGetValue<T>(byte key, out T value) where T : struct
		{
			object obj;
			bool flag = this.paramDict.TryGetValue(key, out obj);
			bool flag2 = !flag;
			bool result;
			if (flag2)
			{
				value = default(T);
				result = false;
			}
			else
			{
				StructWrapper<T> structWrapper = obj as StructWrapper<T>;
				bool flag3 = structWrapper != null;
				if (flag3)
				{
					value = structWrapper.value;
				}
				else
				{
					StructWrapper<object> structWrapper2 = obj as StructWrapper<object>;
					bool flag4 = structWrapper2 != null;
					if (flag4)
					{
						value = (T)((object)structWrapper2.value);
					}
					else
					{
						value = (T)((object)obj);
					}
				}
				result = flag;
			}
			return result;
		}

		public string ToStringFull(bool includeTypes = true)
		{
			string result;
			if (includeTypes)
			{
				result = string.Format("(ParameterDictionary){0}", SupportClass.DictionaryToString(this.paramDict, includeTypes));
			}
			else
			{
				result = SupportClass.DictionaryToString(this.paramDict, includeTypes);
			}
			return result;
		}

		public readonly NonAllocDictionary<byte, object> paramDict;

		public readonly StructWrapperPools wrapperPools = new StructWrapperPools();
	}
}
