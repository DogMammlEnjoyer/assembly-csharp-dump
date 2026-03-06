using System;
using System.Globalization;

namespace UnityEngine.Localization
{
	[Serializable]
	public struct LocaleIdentifier : IEquatable<LocaleIdentifier>, IComparable<LocaleIdentifier>
	{
		public string Code
		{
			get
			{
				return this.m_Code;
			}
		}

		public CultureInfo CultureInfo
		{
			get
			{
				if (this.m_CultureInfo == null && !string.IsNullOrEmpty(this.m_Code))
				{
					try
					{
						this.m_CultureInfo = CultureInfo.GetCultureInfo(this.m_Code);
					}
					catch (CultureNotFoundException)
					{
					}
				}
				return this.m_CultureInfo;
			}
		}

		public LocaleIdentifier(string code)
		{
			this.m_Code = code;
			this.m_CultureInfo = null;
		}

		public LocaleIdentifier(CultureInfo culture)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			this.m_Code = culture.Name;
			this.m_CultureInfo = culture;
		}

		public LocaleIdentifier(SystemLanguage systemLanguage)
		{
			this = new LocaleIdentifier(SystemLanguageConverter.GetSystemLanguageCultureCode(systemLanguage));
		}

		public static implicit operator LocaleIdentifier(string code)
		{
			return new LocaleIdentifier(code);
		}

		public static implicit operator LocaleIdentifier(CultureInfo culture)
		{
			return new LocaleIdentifier(culture);
		}

		public static implicit operator LocaleIdentifier(SystemLanguage systemLanguage)
		{
			return new LocaleIdentifier(systemLanguage);
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.m_Code))
			{
				return "undefined";
			}
			return ((this.CultureInfo != null) ? this.CultureInfo.EnglishName : "Custom") + "(" + this.Code + ")";
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is LocaleIdentifier)
			{
				LocaleIdentifier other = (LocaleIdentifier)obj;
				return this.Equals(other);
			}
			return false;
		}

		public bool Equals(LocaleIdentifier other)
		{
			return (string.IsNullOrEmpty(other.Code) && string.IsNullOrEmpty(this.Code)) || string.Equals(this.Code, other.Code, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			if (string.IsNullOrEmpty(this.Code))
			{
				return base.GetHashCode();
			}
			return this.Code.GetHashCode();
		}

		public int CompareTo(LocaleIdentifier other)
		{
			if (this.CultureInfo == null || other.CultureInfo == null)
			{
				return 1;
			}
			return string.CompareOrdinal(this.CultureInfo.EnglishName, other.CultureInfo.EnglishName);
		}

		public static bool operator ==(LocaleIdentifier l1, LocaleIdentifier l2)
		{
			return l1.Equals(l2);
		}

		public static bool operator !=(LocaleIdentifier l1, LocaleIdentifier l2)
		{
			return !l1.Equals(l2);
		}

		[SerializeField]
		private string m_Code;

		private CultureInfo m_CultureInfo;
	}
}
