using System;
using System.Collections.Generic;
using UnityEngine.Localization.Metadata;

namespace UnityEngine.Localization.Platform.Android
{
	[DisplayName("Android Legacy Icon Info", null)]
	[Metadata(AllowedTypes = MetadataType.LocalizationSettings, AllowMultiple = false, MenuItem = "Android/Legacy Icon")]
	[Serializable]
	public class LegacyIconsInfo : IMetadata
	{
		internal void RefreshLegacyIcons()
		{
			this.LegacyIcons.Clear();
			this.LegacyIcons.Add(this.m_Legacy_idpi);
			this.LegacyIcons.Add(this.m_Legacy_mdpi);
			this.LegacyIcons.Add(this.m_Legacy_hdpi);
			this.LegacyIcons.Add(this.m_Legacy_xhdpi);
			this.LegacyIcons.Add(this.m_Legacy_xxhdpi);
			this.LegacyIcons.Add(this.m_Legacy_xxxhdpi);
		}

		public LocalizedTexture LegacyHdpi
		{
			get
			{
				return this.m_Legacy_hdpi;
			}
			set
			{
				this.m_Legacy_hdpi = value;
			}
		}

		public LocalizedTexture LegacyIdpi
		{
			get
			{
				return this.m_Legacy_idpi;
			}
			set
			{
				this.m_Legacy_idpi = value;
			}
		}

		public LocalizedTexture LegacyMdpi
		{
			get
			{
				return this.m_Legacy_mdpi;
			}
			set
			{
				this.m_Legacy_mdpi = value;
			}
		}

		public LocalizedTexture LegacyXhdpi
		{
			get
			{
				return this.m_Legacy_xhdpi;
			}
			set
			{
				this.m_Legacy_xhdpi = value;
			}
		}

		public LocalizedTexture LegacyXXHdpi
		{
			get
			{
				return this.m_Legacy_xxhdpi;
			}
			set
			{
				this.m_Legacy_xxhdpi = value;
			}
		}

		public LocalizedTexture LegacyXXXHdpi
		{
			get
			{
				return this.m_Legacy_xxxhdpi;
			}
			set
			{
				this.m_Legacy_xxxhdpi = value;
			}
		}

		[SerializeField]
		private LocalizedTexture m_Legacy_idpi;

		[SerializeField]
		private LocalizedTexture m_Legacy_mdpi;

		[SerializeField]
		private LocalizedTexture m_Legacy_hdpi;

		[SerializeField]
		private LocalizedTexture m_Legacy_xhdpi;

		[SerializeField]
		private LocalizedTexture m_Legacy_xxhdpi;

		[SerializeField]
		private LocalizedTexture m_Legacy_xxxhdpi;

		internal List<LocalizedTexture> LegacyIcons = new List<LocalizedTexture>();
	}
}
