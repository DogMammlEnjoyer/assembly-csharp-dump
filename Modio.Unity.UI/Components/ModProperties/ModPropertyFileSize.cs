using System;
using Modio.Mods;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.ModProperties
{
	[Serializable]
	public class ModPropertyFileSize : IModProperty
	{
		public void OnModUpdate(Mod mod)
		{
			this._text.text = ((((mod != null) ? mod.File : null) == null) ? "NULL" : StringFormat.Bytes(this._format, mod.File.FileSize, this._customFormat, false));
		}

		private bool IsCustomFormat()
		{
			return this._format == StringFormatBytes.Custom;
		}

		[SerializeField]
		private TMP_Text _text;

		[SerializeField]
		[Tooltip("Bytes: \"1048576\".\r\nBytesComma: \"1,048,576\".\r\nSuffix: \"1 MB\".")]
		private StringFormatBytes _format = StringFormatBytes.Suffix;

		[SerializeField]
		[ShowIf("IsCustomFormat")]
		private string _customFormat;
	}
}
