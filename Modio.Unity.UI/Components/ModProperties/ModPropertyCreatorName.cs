using System;
using Modio.Mods;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	public class ModPropertyCreatorName : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			this._text.text = mod.Creator.Username;
		}

		[SerializeField]
		private TMP_Text _text;
	}
}
