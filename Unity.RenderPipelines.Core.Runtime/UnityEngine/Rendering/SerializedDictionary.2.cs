using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	[Serializable]
	public abstract class SerializedDictionary<K, V, SK, SV> : Dictionary<K, V>, ISerializationCallbackReceiver
	{
		public abstract SK SerializeKey(K key);

		public abstract SV SerializeValue(V value);

		public abstract K DeserializeKey(SK serializedKey);

		public abstract V DeserializeValue(SV serializedValue);

		public void OnBeforeSerialize()
		{
			this.m_Keys.Clear();
			this.m_Values.Clear();
			foreach (KeyValuePair<K, V> keyValuePair in this)
			{
				this.m_Keys.Add(this.SerializeKey(keyValuePair.Key));
				this.m_Values.Add(this.SerializeValue(keyValuePair.Value));
			}
		}

		public void OnAfterDeserialize()
		{
			base.Clear();
			for (int i = 0; i < this.m_Keys.Count; i++)
			{
				base.Add(this.DeserializeKey(this.m_Keys[i]), this.DeserializeValue(this.m_Values[i]));
			}
		}

		[SerializeField]
		private List<SK> m_Keys = new List<SK>();

		[SerializeField]
		private List<SV> m_Values = new List<SV>();
	}
}
