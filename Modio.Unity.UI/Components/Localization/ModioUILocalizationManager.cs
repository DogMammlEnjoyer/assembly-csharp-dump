using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Modio.API;
using UnityEngine;

namespace Modio.Unity.UI.Components.Localization
{
	public class ModioUILocalizationManager : MonoBehaviour
	{
		public static bool LocalizationExists
		{
			get
			{
				return ModioUILocalizationManager.customLocalizationHandler != null || (ModioUILocalizationManager._languageTables != null && ModioUILocalizationManager._languageTables.Count > 0);
			}
		}

		public static bool LocalizationReady
		{
			get
			{
				return ModioUILocalizationManager.customLocalizationHandler != null || ModioUILocalizationManager._currentTable != null;
			}
		}

		public static event Action LanguageSet
		{
			add
			{
				ModioUILocalizationManager.LanguageSetInternal += value;
				if (ModioUILocalizationManager._currentTable != null || ModioUILocalizationManager.customLocalizationHandler != null)
				{
					value();
				}
			}
			remove
			{
				ModioUILocalizationManager.LanguageSetInternal -= value;
			}
		}

		private static event Action LanguageSetInternal;

		public static CultureInfo CultureInfo { get; private set; } = new CultureInfo("en");

		public static void SetCustomHandler(ModioUILocalizationManager.LocalizationHandler handler)
		{
			ModioUILocalizationManager.customLocalizationHandler = handler;
			if (ModioUILocalizationManager.customLocalizationHandler != null)
			{
				Action languageSetInternal = ModioUILocalizationManager.LanguageSetInternal;
				if (languageSetInternal == null)
				{
					return;
				}
				languageSetInternal();
			}
		}

		public void SetLanguageCode(string isoCode)
		{
			if (string.IsNullOrEmpty(isoCode))
			{
				isoCode = "en";
			}
			ModioUILocalizationManager._languageCode = isoCode;
			try
			{
				ModioUILocalizationManager.CultureInfo = new CultureInfo(isoCode);
			}
			catch (CultureNotFoundException)
			{
				ModioLog warning = ModioLog.Warning;
				if (warning != null)
				{
					warning.Log("Language code " + isoCode + " not found by CultureInfo. Using default culture.");
				}
				ModioUILocalizationManager.CultureInfo = new CultureInfo("en");
			}
			if (this._setCurrentSystemCulture)
			{
				CultureInfo.CurrentCulture = ModioUILocalizationManager.CultureInfo;
			}
			if (ModioUILocalizationManager._languageTables != null)
			{
				foreach (Dictionary<string, string> dictionary in ModioUILocalizationManager._languageTables)
				{
					string b;
					if (dictionary.TryGetValue("modio_languagecode", out b) && isoCode == b)
					{
						ModioUILocalizationManager._currentTable = dictionary;
						Action languageSetInternal = ModioUILocalizationManager.LanguageSetInternal;
						if (languageSetInternal == null)
						{
							break;
						}
						languageSetInternal();
						break;
					}
				}
			}
		}

		public static string GetLocalizedText(string key, bool errorIfMissing = true)
		{
			if (ModioUILocalizationManager.customLocalizationHandler != null)
			{
				return ModioUILocalizationManager.customLocalizationHandler(key, ModioUILocalizationManager._languageCode);
			}
			if (ModioUILocalizationManager._currentTable == null)
			{
				if (!errorIfMissing)
				{
					return null;
				}
				return key;
			}
			else
			{
				string result;
				if (ModioUILocalizationManager._currentTable.TryGetValue(key, out result))
				{
					return result;
				}
				if (!errorIfMissing)
				{
					return null;
				}
				Debug.LogError("Missing localized key " + key + " for language " + ModioUILocalizationManager._languageCode);
				return "MISSING KEY " + key;
			}
		}

		private void Awake()
		{
			string[] array = this._locTable.text.Split('\n', StringSplitOptions.None);
			ModioUILocalizationManager._languageTables = null;
			foreach (string text in array)
			{
				string input = text;
				if (text.EndsWith("\r"))
				{
					input = text.Substring(0, text.Length - 1);
				}
				string pattern = "(?:,\"|^\")(\"\"|[\\w\\W]*?)(?=\",|\"$)|(?:,(?!\")|^(?!\"))([^,]*?)(?=$|,)|(\\r\\n|\\n)";
				string[] array3 = Regex.Split(input, pattern);
				if (array3.Length >= 2)
				{
					string key = array3[1];
					if (ModioUILocalizationManager._languageTables == null)
					{
						ModioUILocalizationManager._languageTables = new List<Dictionary<string, string>>();
						for (int j = 1; j < array3.Length / 2; j++)
						{
							ModioUILocalizationManager._languageTables.Add(new Dictionary<string, string>
							{
								{
									key,
									array3[j * 2 + 1]
								}
							});
						}
					}
					else
					{
						int num = 1;
						while (num * 2 + 1 < array3.Length && num - 1 < ModioUILocalizationManager._languageTables.Count)
						{
							ModioUILocalizationManager._languageTables[num - 1].Add(key, array3[num * 2 + 1]);
							num++;
						}
					}
				}
			}
			ModioClient.OnInitialized += this.OnPluginInitialized;
		}

		private void OnDestroy()
		{
			ModioClient.OnInitialized -= this.OnPluginInitialized;
		}

		private void OnPluginInitialized()
		{
			this.SetLanguageCode(ModioAPI.LanguageCodeResponse);
		}

		private static ModioUILocalizationManager.LocalizationHandler customLocalizationHandler;

		private static string _languageCode;

		private static List<Dictionary<string, string>> _languageTables;

		private static Dictionary<string, string> _currentTable;

		[SerializeField]
		private TextAsset _locTable;

		[SerializeField]
		private bool _setCurrentSystemCulture = true;

		public delegate string LocalizationHandler(string key, string isoLanguageCode);
	}
}
