using System;
using Modio.Mods;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyVersion : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			if (this._disableIfNoVersionInfo != null)
			{
				this._disableIfNoVersionInfo.SetActive(!string.IsNullOrEmpty(mod.File.Version));
			}
			if (this._text != null)
			{
				this._text.text = mod.File.Version;
			}
		}

		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		private GameObject _disableIfNoVersionInfo;
	}
}
