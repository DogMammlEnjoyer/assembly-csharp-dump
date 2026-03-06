using System;
using Modio.Mods;
using Modio.Unity.UI.Search;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components
{
	public class ModioUITag : MonoBehaviour
	{
		public virtual void Set(ModTag tag)
		{
			this._tag = tag;
			if (this._label != null)
			{
				this._label.text = tag.NameLocalized;
			}
		}

		public void TagSelectedForSearch()
		{
			ModioUISearch.Default.SetSearchForTag(this._tag);
		}

		[SerializeField]
		private TMP_Text _label;

		private ModTag _tag;
	}
}
