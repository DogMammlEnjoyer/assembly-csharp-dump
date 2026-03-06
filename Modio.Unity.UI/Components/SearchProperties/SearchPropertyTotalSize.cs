using System;
using Modio.Mods;
using Modio.Unity.UI.Search;
using TMPro;
using UnityEngine;

namespace Modio.Unity.UI.Components.SearchProperties
{
	[Serializable]
	public class SearchPropertyTotalSize : ISearchProperty
	{
		private bool IsCustomFormat()
		{
			return this._sizeFormat == StringFormatBytes.Custom;
		}

		public void OnSearchUpdate(ModioUISearch search)
		{
			if (this._totalFileSize != null)
			{
				long num = 0L;
				if (this._alsoIncludeSizeOf != null && this._alsoIncludeSizeOf.Mod != null && (!this._ignoreInstalledMods || this._alsoIncludeSizeOf.Mod.File.State != ModFileState.Installed))
				{
					num += this._alsoIncludeSizeOf.Mod.File.FileSize;
				}
				foreach (Mod mod in search.LastSearchResultMods)
				{
					if (!this._ignoreInstalledMods || mod.File.State != ModFileState.Installed)
					{
						num += mod.File.FileSize;
					}
				}
				this._totalFileSize.text = StringFormat.Bytes(this._sizeFormat, num, this._customSizeFormat, false);
			}
		}

		[SerializeField]
		private TMP_Text _totalFileSize;

		[SerializeField]
		[Tooltip("Bytes: \"1048576\".\r\nBytesComma: \"1,048,576\".\r\nSuffix: \"1 MB\".")]
		private StringFormatBytes _sizeFormat = StringFormatBytes.Suffix;

		[SerializeField]
		[ShowIf("IsCustomFormat")]
		private string _customSizeFormat;

		[SerializeField]
		private ModioUIMod _alsoIncludeSizeOf;

		[SerializeField]
		private bool _ignoreInstalledMods;
	}
}
