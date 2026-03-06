using System;
using System.Linq;

namespace UnityEngine.Recorder
{
	[ExecuteInEditMode]
	public class RecorderBindings : MonoBehaviour
	{
		public void SetBindingValue(string id, Object value)
		{
			this.m_References.dictionary[id] = value;
		}

		public Object GetBindingValue(string id)
		{
			Object result;
			if (!this.m_References.dictionary.TryGetValue(id, out result))
			{
				return null;
			}
			return result;
		}

		public bool HasBindingValue(string id)
		{
			return this.m_References.dictionary.ContainsKey(id);
		}

		public void RemoveBinding(string id)
		{
			if (this.m_References.dictionary.ContainsKey(id))
			{
				this.m_References.dictionary.Remove(id);
				this.MarkSceneDirty();
			}
		}

		public bool IsEmpty()
		{
			return this.m_References == null || !this.m_References.dictionary.Keys.Any<string>();
		}

		public void DuplicateBinding(string src, string dst)
		{
			if (this.m_References.dictionary.ContainsKey(src))
			{
				this.m_References.dictionary[dst] = this.m_References.dictionary[src];
				this.MarkSceneDirty();
			}
		}

		private void MarkSceneDirty()
		{
		}

		[SerializeField]
		private RecorderBindings.PropertyObjects m_References = new RecorderBindings.PropertyObjects();

		[Serializable]
		private class PropertyObjects : SerializedDictionary<string, Object>
		{
		}
	}
}
