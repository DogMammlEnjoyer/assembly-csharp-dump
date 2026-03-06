using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	[ExcludeFromPreset]
	[Serializable]
	public class TMP_StyleSheet : ScriptableObject
	{
		internal List<TMP_Style> styles
		{
			get
			{
				return this.m_StyleList;
			}
		}

		private void Reset()
		{
			this.LoadStyleDictionaryInternal();
		}

		public TMP_Style GetStyle(int hashCode)
		{
			if (this.m_StyleLookupDictionary == null)
			{
				this.LoadStyleDictionaryInternal();
			}
			TMP_Style result;
			if (this.m_StyleLookupDictionary.TryGetValue(hashCode, out result))
			{
				return result;
			}
			return null;
		}

		public TMP_Style GetStyle(string name)
		{
			if (this.m_StyleLookupDictionary == null)
			{
				this.LoadStyleDictionaryInternal();
			}
			int hashCode = TMP_TextParsingUtilities.GetHashCode(name);
			TMP_Style result;
			if (this.m_StyleLookupDictionary.TryGetValue(hashCode, out result))
			{
				return result;
			}
			return null;
		}

		public void RefreshStyles()
		{
			this.LoadStyleDictionaryInternal();
		}

		private void LoadStyleDictionaryInternal()
		{
			if (this.m_StyleLookupDictionary == null)
			{
				this.m_StyleLookupDictionary = new Dictionary<int, TMP_Style>();
			}
			else
			{
				this.m_StyleLookupDictionary.Clear();
			}
			for (int i = 0; i < this.m_StyleList.Count; i++)
			{
				this.m_StyleList[i].RefreshStyle();
				if (!this.m_StyleLookupDictionary.ContainsKey(this.m_StyleList[i].hashCode))
				{
					this.m_StyleLookupDictionary.Add(this.m_StyleList[i].hashCode, this.m_StyleList[i]);
				}
			}
			int hashCode = TMP_TextParsingUtilities.GetHashCode("Normal");
			if (!this.m_StyleLookupDictionary.ContainsKey(hashCode))
			{
				TMP_Style tmp_Style = new TMP_Style("Normal", string.Empty, string.Empty);
				this.m_StyleList.Add(tmp_Style);
				this.m_StyleLookupDictionary.Add(hashCode, tmp_Style);
			}
		}

		[SerializeField]
		private List<TMP_Style> m_StyleList = new List<TMP_Style>(1);

		private Dictionary<int, TMP_Style> m_StyleLookupDictionary;
	}
}
