using System;
using Modio.Mods;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyDescription : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			this._text.text = mod.Description;
		}

		[SerializeField]
		private TMP_Text _text;
	}
}
