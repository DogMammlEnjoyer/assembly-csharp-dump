using System;

namespace UnityEngine.Localization.Settings
{
	[Serializable]
	public class PlayerPrefLocaleSelector : IStartupLocaleSelector, IInitialize
	{
		public string PlayerPreferenceKey
		{
			get
			{
				return this.m_PlayerPreferenceKey;
			}
			set
			{
				this.m_PlayerPreferenceKey = value;
			}
		}

		public void PostInitialization(LocalizationSettings settings)
		{
			if (LocalizationSettings.Instance.IsPlayingOrWillChangePlaymode)
			{
				Locale selectedLocale = settings.GetSelectedLocale();
				if (selectedLocale != null)
				{
					PlayerPrefs.SetString(this.PlayerPreferenceKey, selectedLocale.Identifier.Code);
				}
			}
		}

		public Locale GetStartupLocale(ILocalesProvider availableLocales)
		{
			if (PlayerPrefs.HasKey(this.PlayerPreferenceKey))
			{
				string @string = PlayerPrefs.GetString(this.PlayerPreferenceKey);
				if (!string.IsNullOrEmpty(@string))
				{
					return availableLocales.GetLocale(@string);
				}
			}
			return null;
		}

		[SerializeField]
		private string m_PlayerPreferenceKey = "selected-locale";
	}
}
