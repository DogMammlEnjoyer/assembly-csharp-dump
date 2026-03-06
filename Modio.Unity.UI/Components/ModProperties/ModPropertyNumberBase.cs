using System;
using Modio.Mods;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public abstract class ModPropertyNumberBase : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			long value = this.GetValue(mod);
			if (this._text != null)
			{
				this._text.text = StringFormat.Kilo(this._format, value, this._customFormat);
			}
		}

		protected abstract long GetValue(Mod mod);

		private bool IsCustomFormat()
		{
			return this._format == StringFormatKilo.Custom;
		}

		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		[Tooltip("None: \"10500\".\r\nComma: \"10,500\".\r\nKilo: \"10.5k\".")]
		private StringFormatKilo _format = StringFormatKilo.Comma;

		[SerializeField]
		[ShowIf("IsCustomFormat")]
		private string _customFormat;
	}
}
