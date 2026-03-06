using System;
using System.Collections.Generic;
using System.Linq;
using Modio.Mods;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	public class ModPropertyTags : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			if (!this._tags.Any<ModioUITag>())
			{
				if (this._tagTemplate == null)
				{
					bool flag = mod.Tags.Any((ModTag tag) => tag.IsVisible);
					if (this._noTagsActive != null)
					{
						this._noTagsActive.SetActive(!flag);
					}
					if (this._tagsActive != null)
					{
						this._tagsActive.SetActive(flag);
					}
					return;
				}
				this._tags.Add(this._tagTemplate);
			}
			int num = 0;
			foreach (ModTag tag2 in mod.Tags)
			{
				if (num >= this._tags.Count)
				{
					this._tags.Add(Object.Instantiate<ModioUITag>(this._tags[0], this._tags[0].transform.parent));
				}
				ModioUITag modioUITag = this._tags[num];
				modioUITag.gameObject.SetActive(true);
				modioUITag.Set(tag2);
				num++;
			}
			for (int j = num; j < this._tags.Count; j++)
			{
				this._tags[j].gameObject.SetActive(false);
			}
			if (this._noTagsActive != null)
			{
				this._noTagsActive.SetActive(num == 0);
			}
			if (this._tagsActive != null)
			{
				this._tagsActive.SetActive(num != 0);
			}
		}

		[SerializeField]
		private ModioUITag _tagTemplate;

		[SerializeField]
		private GameObject _noTagsActive;

		[SerializeField]
		private GameObject _tagsActive;

		private readonly List<ModioUITag> _tags = new List<ModioUITag>();
	}
}
