using System;

namespace UnityEngine.Localization.Metadata
{
	[Metadata(AllowedTypes = MetadataType.Locale)]
	[Serializable]
	public class FallbackLocale : IMetadata
	{
		public FallbackLocale()
		{
		}

		public FallbackLocale(Locale fallback)
		{
			this.Locale = fallback;
		}

		public Locale Locale
		{
			get
			{
				return this.m_Locale;
			}
			set
			{
				this.m_Locale = value;
				if (this.IsCyclic(value))
				{
					this.m_Locale = null;
				}
			}
		}

		internal bool IsCyclic(Locale locale)
		{
			if (locale == null)
			{
				return false;
			}
			MetadataCollection metadata = locale.Metadata;
			FallbackLocale fallbackLocale = (metadata != null) ? metadata.GetMetadata<FallbackLocale>() : null;
			while (fallbackLocale != null && fallbackLocale.Locale != null)
			{
				if (fallbackLocale.Locale == locale)
				{
					Debug.LogWarning(string.Format("Cyclic fallback linking detected. Can not set fallback locale '{0}' as it would create an infinite loop.", locale));
					return true;
				}
				MetadataCollection metadata2 = fallbackLocale.Locale.Metadata;
				fallbackLocale = ((metadata2 != null) ? metadata2.GetMetadata<FallbackLocale>() : null);
			}
			return false;
		}

		[SerializeField]
		private Locale m_Locale;
	}
}
