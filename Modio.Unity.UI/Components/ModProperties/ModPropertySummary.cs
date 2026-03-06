using System;
using Modio.Mods;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertySummary : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			this._text.text = mod.Summary;
			if (this._enableIfDescriptionDiffers != null)
			{
				bool active = !string.IsNullOrEmpty(mod.Description) && mod.Description != mod.Summary;
				this._enableIfDescriptionDiffers.SetActive(active);
			}
		}

		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		private GameObject _enableIfDescriptionDiffers;
	}
}
