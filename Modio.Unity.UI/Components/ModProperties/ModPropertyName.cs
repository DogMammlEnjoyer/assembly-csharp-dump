using System;
using Modio.Mods;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	public class ModPropertyName : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			this._text.text = mod.Name;
		}

		[SerializeField]
		private TMP_Text _text;
	}
}
