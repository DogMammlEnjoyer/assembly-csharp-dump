using System;
using Modio.Mods;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyRating : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			if (this._text != null)
			{
				this._text.text = string.Format(this._format, mod.Stats.RatingsPercent);
			}
		}

		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		[Tooltip("Uses string.Format().\n{0} outputs the rating percentage value.")]
		private string _format = "{0}%";
	}
}
