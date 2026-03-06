using System;
using System.Globalization;

namespace UnityEngine.Localization.Settings
{
	[Serializable]
	public class SystemLocaleSelector : IStartupLocaleSelector
	{
		public Locale GetStartupLocale(ILocalesProvider availableLocales)
		{
			Locale locale = SystemLocaleSelector.FindLocaleOrFallback(this.GetSystemCulture(), availableLocales);
			SystemLanguage applicationSystemLanguage = this.GetApplicationSystemLanguage();
			if (locale == null && applicationSystemLanguage != SystemLanguage.Unknown)
			{
				locale = SystemLocaleSelector.FindLocaleOrFallback(applicationSystemLanguage, availableLocales);
			}
			return locale;
		}

		private static Locale FindLocaleOrFallback(LocaleIdentifier localeIdentifier, ILocalesProvider availableLocales)
		{
			CultureInfo cultureInfo = localeIdentifier.CultureInfo;
			if (cultureInfo == null)
			{
				return null;
			}
			Locale locale = availableLocales.GetLocale(cultureInfo);
			if (locale == null)
			{
				cultureInfo = cultureInfo.Parent;
				while (cultureInfo != CultureInfo.InvariantCulture && locale == null)
				{
					locale = availableLocales.GetLocale(cultureInfo);
					cultureInfo = cultureInfo.Parent;
				}
				if (locale != null)
				{
					Debug.Log(string.Format("The Locale '{0}' is not available, however the parent locale '{1}' is available.", localeIdentifier, locale.Identifier));
				}
			}
			return locale;
		}

		protected virtual CultureInfo GetSystemCulture()
		{
			return CultureInfo.CurrentUICulture;
		}

		protected virtual SystemLanguage GetApplicationSystemLanguage()
		{
			return Application.systemLanguage;
		}
	}
}
