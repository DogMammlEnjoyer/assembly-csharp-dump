using System;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Input_ActionFile_LocalizationItem
	{
		public SteamVR_Input_ActionFile_LocalizationItem(string newLanguage)
		{
			this.language = newLanguage;
		}

		public SteamVR_Input_ActionFile_LocalizationItem(Dictionary<string, string> dictionary)
		{
			if (dictionary == null)
			{
				return;
			}
			if (dictionary.ContainsKey("language_tag"))
			{
				this.language = dictionary["language_tag"];
			}
			else
			{
				Debug.Log("<b>[SteamVR]</b> Input: Error in actions file, no language_tag in localization array item.");
			}
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				if (keyValuePair.Key != "language_tag")
				{
					this.items.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		public const string languageTagKeyName = "language_tag";

		public string language;

		public Dictionary<string, string> items = new Dictionary<string, string>();
	}
}
