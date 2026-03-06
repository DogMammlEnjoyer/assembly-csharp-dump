using System;
using System.Collections.Generic;

namespace UnityEngine.Recorder
{
	[Serializable]
	internal class SerializedDictionary<TKey, TValue> : ISerializationCallbackReceiver
	{
		public Dictionary<TKey, TValue> dictionary
		{
			get
			{
				return this.m_Dictionary;
			}
		}

		public void OnBeforeSerialize()
		{
			this.m_Keys.Clear();
			this.m_Values.Clear();
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this.m_Dictionary)
			{
				this.m_Keys.Add(keyValuePair.Key);
				this.m_Values.Add(keyValuePair.Value);
			}
		}

		public void OnAfterDeserialize()
		{
			this.m_Dictionary.Clear();
			for (int i = 0; i < this.m_Keys.Count; i++)
			{
				this.m_Dictionary.Add(this.m_Keys[i], this.m_Values[i]);
			}
		}

		[SerializeField]
		private List<TKey> m_Keys = new List<TKey>();

		[SerializeField]
		private List<TValue> m_Values = new List<TValue>();

		private readonly Dictionary<TKey, TValue> m_Dictionary = new Dictionary<TKey, TValue>();
	}
}
