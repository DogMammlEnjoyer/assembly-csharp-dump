using System;
using Modio.Mods;
using Modio.Unity.UI.Components.Localization;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public abstract class ModPropertyDateBase : IModProperty
	{
		public virtual void OnModUpdate(Mod mod)
		{
			this._text.text = this.GetValue(mod).ToString(this._format, ModioUILocalizationManager.CultureInfo);
		}

		protected abstract DateTime GetValue(Mod mod);

		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		private string _format = "dd MMM, yy";
	}
}
