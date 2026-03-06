using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Localization.Pseudo;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Settings
{
	[Serializable]
	public class LocalesProvider : ILocalesProvider, IPreloadRequired, IReset, IDisposable
	{
		public List<Locale> Locales
		{
			get
			{
				if (LocalizationSettings.Instance.IsPlayingOrWillChangePlaymode && !this.PreloadOperation.IsDone)
				{
					this.PreloadOperation.WaitForCompletion();
				}
				return this.m_Locales;
			}
		}

		public AsyncOperationHandle PreloadOperation
		{
			get
			{
				if (!this.m_LoadOperation.IsValid())
				{
					this.m_Locales.Clear();
					this.m_LoadOperation = AddressablesInterface.LoadAssetsWithLabel<Locale>("Locale", new Action<Locale>(this.AddLocale));
				}
				return this.m_LoadOperation;
			}
		}

		public Locale GetLocale(LocaleIdentifier id)
		{
			foreach (Locale locale in this.Locales)
			{
				if (!(locale == null) && !(locale is PseudoLocale) && locale.Identifier.Equals(id))
				{
					return locale;
				}
			}
			return this.FindFallbackLocale(id);
		}

		public Locale GetLocale(string code)
		{
			return this.GetLocale(new LocaleIdentifier(code));
		}

		public Locale GetLocale(SystemLanguage systemLanguage)
		{
			return this.GetLocale(new LocaleIdentifier(systemLanguage));
		}

		public void AddLocale(Locale locale)
		{
			if (locale == null)
			{
				return;
			}
			if (!(locale is PseudoLocale))
			{
				foreach (Locale locale2 in this.m_Locales)
				{
					if (!(locale2 is PseudoLocale) && locale2.Identifier == locale.Identifier)
					{
						Debug.LogWarning(string.Format("Ignoring locale {0}. The locale {1} has the same Id `{2}`", locale, locale2, locale.Identifier));
						return;
					}
				}
			}
			int num = this.m_Locales.BinarySearch(locale);
			if (num < 0)
			{
				this.m_Locales.Insert(~num, locale);
			}
		}

		public bool RemoveLocale(Locale locale)
		{
			if (locale == null)
			{
				return false;
			}
			bool result = this.Locales.Remove(locale);
			LocalizationSettings instanceDontCreateDefault = LocalizationSettings.GetInstanceDontCreateDefault();
			if (instanceDontCreateDefault == null)
			{
				return result;
			}
			instanceDontCreateDefault.OnLocaleRemoved(locale);
			return result;
		}

		public Locale FindFallbackLocale(LocaleIdentifier localeIdentifier)
		{
			CultureInfo cultureInfo = localeIdentifier.CultureInfo;
			if (cultureInfo == null)
			{
				return null;
			}
			Locale locale = null;
			cultureInfo = cultureInfo.Parent;
			while (cultureInfo != CultureInfo.InvariantCulture && locale == null)
			{
				locale = this.GetLocale(cultureInfo);
				cultureInfo = cultureInfo.Parent;
			}
			return locale;
		}

		public void ResetState()
		{
			this.m_Locales.Clear();
			this.m_LoadOperation = default(AsyncOperationHandle);
		}

		~LocalesProvider()
		{
			AddressablesInterface.SafeRelease(this.m_LoadOperation);
		}

		void IDisposable.Dispose()
		{
			this.m_Locales.Clear();
			AddressablesInterface.SafeRelease(this.m_LoadOperation);
			GC.SuppressFinalize(this);
		}

		private readonly List<Locale> m_Locales = new List<Locale>();

		private AsyncOperationHandle m_LoadOperation;
	}
}
