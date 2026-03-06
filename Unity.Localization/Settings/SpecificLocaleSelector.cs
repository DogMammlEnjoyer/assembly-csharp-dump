using System;

namespace UnityEngine.Localization.Settings
{
	[Serializable]
	public class SpecificLocaleSelector : IStartupLocaleSelector
	{
		public LocaleIdentifier LocaleId
		{
			get
			{
				return this.m_LocaleId;
			}
			set
			{
				this.m_LocaleId = value;
			}
		}

		public Locale GetStartupLocale(ILocalesProvider availableLocales)
		{
			return availableLocales.GetLocale(this.LocaleId);
		}

		[SerializeField]
		private LocaleIdentifier m_LocaleId = new LocaleIdentifier(SystemLanguage.English);
	}
}
