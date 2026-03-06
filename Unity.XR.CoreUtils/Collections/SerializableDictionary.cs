using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.CoreUtils.Collections
{
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		public List<SerializableDictionary<TKey, TValue>.Item> SerializedItems
		{
			get
			{
				return this.m_Items;
			}
		}

		public SerializableDictionary()
		{
		}

		public SerializableDictionary(IDictionary<TKey, TValue> input) : base(input)
		{
		}

		public virtual void OnBeforeSerialize()
		{
			this.m_Items.Clear();
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
			{
				this.m_Items.Add(new SerializableDictionary<TKey, TValue>.Item
				{
					Key = keyValuePair.Key,
					Value = keyValuePair.Value
				});
			}
		}

		public virtual void OnAfterDeserialize()
		{
			base.Clear();
			foreach (SerializableDictionary<TKey, TValue>.Item item in this.m_Items)
			{
				if (base.ContainsKey(item.Key))
				{
					Debug.LogWarning(string.Format("The key \"{0}\" is duplicated in the {1}.{2} and will be ignored.", item.Key, base.GetType().Name, "SerializedItems"));
				}
				else
				{
					base.Add(item.Key, item.Value);
				}
			}
		}

		[SerializeField]
		private List<SerializableDictionary<TKey, TValue>.Item> m_Items = new List<SerializableDictionary<TKey, TValue>.Item>();

		[Serializable]
		public struct Item
		{
			public TKey Key;

			public TValue Value;
		}
	}
}
