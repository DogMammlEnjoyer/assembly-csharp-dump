using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Pseudo;
using UnityEngine.Localization.Settings;
using UnityEngine.Pool;

namespace UnityEngine.Localization
{
	public class Locale : ScriptableObject, IEquatable<Locale>, IComparable<Locale>, ISerializationCallbackReceiver, IFormatProvider
	{
		public LocaleIdentifier Identifier
		{
			get
			{
				return this.m_Identifier;
			}
			set
			{
				this.m_Identifier = value;
			}
		}

		public MetadataCollection Metadata
		{
			get
			{
				return this.m_Metadata;
			}
			set
			{
				this.m_Metadata = value;
			}
		}

		public ushort SortOrder
		{
			get
			{
				return this.m_SortOrder;
			}
			set
			{
				this.m_SortOrder = value;
			}
		}

		public string LocaleName
		{
			get
			{
				if (!string.IsNullOrEmpty(this.m_LocaleName))
				{
					return this.m_LocaleName;
				}
				if (this.Identifier.CultureInfo != null)
				{
					return this.Identifier.CultureInfo.EnglishName;
				}
				return base.name;
			}
			set
			{
				this.m_LocaleName = value;
			}
		}

		[Obsolete("GetFallback is obsolete, please use GetFallbacks.")]
		public virtual Locale GetFallback()
		{
			return this.GetFallbacks().GetEnumerator().Current;
		}

		public IEnumerable<Locale> GetFallbacks()
		{
			if (this.Metadata == null)
			{
				yield break;
			}
			HashSet<Locale> processedLocales;
			using (CollectionPool<HashSet<Locale>, Locale>.Get(out processedLocales))
			{
				IList<IMetadata> entries = this.Metadata.MetadataEntries;
				int num;
				for (int i = 0; i < entries.Count; i = num)
				{
					FallbackLocale fallbackLocale = entries[i] as FallbackLocale;
					if (fallbackLocale != null && fallbackLocale.Locale != null && !processedLocales.Contains(fallbackLocale.Locale))
					{
						processedLocales.Add(fallbackLocale.Locale);
						yield return fallbackLocale.Locale;
					}
					num = i + 1;
				}
				if (processedLocales.Count == 0)
				{
					Locale locale = null;
					CultureInfo cultureInfo = this.Identifier.CultureInfo;
					if (cultureInfo != null)
					{
						while (cultureInfo != CultureInfo.InvariantCulture && locale == null)
						{
							Locale locale2 = LocalizationSettings.AvailableLocales.GetLocale(cultureInfo);
							if (locale2 != this)
							{
								locale = locale2;
							}
							cultureInfo = cultureInfo.Parent;
						}
					}
					if (locale != null)
					{
						yield return locale;
					}
				}
				entries = null;
			}
			processedLocales = null;
			PooledObject<HashSet<Locale>> pooledObject = default(PooledObject<HashSet<Locale>>);
			yield break;
			yield break;
		}

		public bool UseCustomFormatter
		{
			get
			{
				return this.m_UseCustomFormatter;
			}
			set
			{
				this.m_UseCustomFormatter = value;
				this.m_Formatter = null;
			}
		}

		public string CustomFormatterCode
		{
			get
			{
				return this.m_CustomFormatCultureCode;
			}
			set
			{
				this.m_CustomFormatCultureCode = value;
				this.m_Formatter = null;
			}
		}

		public virtual IFormatProvider Formatter
		{
			get
			{
				if (this.m_Formatter == null)
				{
					this.m_Formatter = Locale.GetFormatter(this.UseCustomFormatter, this.Identifier, this.CustomFormatterCode);
				}
				return this.m_Formatter;
			}
			set
			{
				this.m_Formatter = value;
			}
		}

		internal static CultureInfo GetFormatter(bool useCustom, LocaleIdentifier localeIdentifier, string customCode)
		{
			CultureInfo cultureInfo = null;
			if (useCustom)
			{
				cultureInfo = (string.IsNullOrEmpty(customCode) ? CultureInfo.InvariantCulture : new LocaleIdentifier(customCode).CultureInfo);
			}
			if (cultureInfo == null)
			{
				cultureInfo = localeIdentifier.CultureInfo;
			}
			return cultureInfo;
		}

		public static Locale CreateLocale(string code)
		{
			Locale locale = ScriptableObject.CreateInstance<Locale>();
			locale.m_Identifier = new LocaleIdentifier(code);
			if (locale.m_Identifier.CultureInfo != null)
			{
				locale.name = locale.m_Identifier.CultureInfo.EnglishName;
			}
			return locale;
		}

		public static Locale CreateLocale(LocaleIdentifier identifier)
		{
			Locale locale = ScriptableObject.CreateInstance<Locale>();
			locale.m_Identifier = identifier;
			if (locale.m_Identifier.CultureInfo != null)
			{
				locale.LocaleName = locale.m_Identifier.CultureInfo.EnglishName;
			}
			return locale;
		}

		public static Locale CreateLocale(SystemLanguage language)
		{
			return Locale.CreateLocale(new LocaleIdentifier(SystemLanguageConverter.GetSystemLanguageCultureCode(language)));
		}

		public static Locale CreateLocale(CultureInfo cultureInfo)
		{
			return Locale.CreateLocale(new LocaleIdentifier(cultureInfo));
		}

		public int CompareTo(Locale other)
		{
			if (other == null)
			{
				return -1;
			}
			if (this.SortOrder != other.SortOrder)
			{
				return this.SortOrder.CompareTo(other.SortOrder);
			}
			if (base.GetType() == other.GetType())
			{
				int num = string.CompareOrdinal(this.LocaleName, other.LocaleName);
				if (num == 0)
				{
					return base.GetInstanceID().CompareTo(other.GetInstanceID());
				}
				return num;
			}
			else
			{
				if (other is PseudoLocale)
				{
					return -1;
				}
				return 1;
			}
		}

		public void OnAfterDeserialize()
		{
			this.m_Formatter = null;
		}

		public void OnBeforeSerialize()
		{
			if (string.IsNullOrEmpty(this.m_LocaleName))
			{
				this.m_LocaleName = base.name;
			}
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.LocaleName))
			{
				return this.LocaleName;
			}
			return base.name;
		}

		public bool Equals(Locale other)
		{
			return !(other == null) && this.LocaleName == other.LocaleName && this.Identifier.Equals(other.Identifier);
		}

		object IFormatProvider.GetFormat(Type formatType)
		{
			IFormatProvider formatter = this.Formatter;
			if (formatter == null)
			{
				return null;
			}
			return formatter.GetFormat(formatType);
		}

		[SerializeField]
		private LocaleIdentifier m_Identifier;

		[SerializeField]
		[MetadataType(MetadataType.Locale)]
		private MetadataCollection m_Metadata = new MetadataCollection();

		[SerializeField]
		private string m_LocaleName;

		[SerializeField]
		private string m_CustomFormatCultureCode;

		[SerializeField]
		private bool m_UseCustomFormatter;

		[SerializeField]
		private ushort m_SortOrder = 10000;

		private IFormatProvider m_Formatter;
	}
}
