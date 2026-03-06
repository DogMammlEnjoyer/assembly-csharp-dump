using System;
using System.Collections.Generic;
using UnityEngine.Localization.Metadata;

namespace UnityEngine.Localization.Platform.Android
{
	[DisplayName("Android Round Icon Info", null)]
	[Metadata(AllowedTypes = MetadataType.LocalizationSettings, AllowMultiple = false, MenuItem = "Android/Round Icon")]
	[Serializable]
	public class RoundIconsInfo : IMetadata
	{
		internal void RefreshRoundIcons()
		{
			this.RoundIcons.Clear();
			this.RoundIcons.Add(this.m_Round_idpi);
			this.RoundIcons.Add(this.m_Round_mdpi);
			this.RoundIcons.Add(this.m_Round_hdpi);
			this.RoundIcons.Add(this.m_Round_xhdpi);
			this.RoundIcons.Add(this.m_Round_xxhdpi);
			this.RoundIcons.Add(this.m_Round_xxxhdpi);
		}

		public LocalizedTexture RoundHdpi
		{
			get
			{
				return this.m_Round_hdpi;
			}
			set
			{
				this.m_Round_hdpi = value;
			}
		}

		public LocalizedTexture RoundIdpi
		{
			get
			{
				return this.m_Round_idpi;
			}
			set
			{
				this.m_Round_idpi = value;
			}
		}

		public LocalizedTexture RoundMdpi
		{
			get
			{
				return this.m_Round_mdpi;
			}
			set
			{
				this.m_Round_mdpi = value;
			}
		}

		public LocalizedTexture RoundXhdpi
		{
			get
			{
				return this.m_Round_xhdpi;
			}
			set
			{
				this.m_Round_xhdpi = value;
			}
		}

		public LocalizedTexture RoundXXHdpi
		{
			get
			{
				return this.m_Round_xxhdpi;
			}
			set
			{
				this.m_Round_xxhdpi = value;
			}
		}

		public LocalizedTexture RoundXXXHdpi
		{
			get
			{
				return this.m_Round_xxxhdpi;
			}
			set
			{
				this.m_Round_xxxhdpi = value;
			}
		}

		[SerializeField]
		private LocalizedTexture m_Round_idpi;

		[SerializeField]
		private LocalizedTexture m_Round_mdpi;

		[SerializeField]
		private LocalizedTexture m_Round_hdpi;

		[SerializeField]
		private LocalizedTexture m_Round_xhdpi;

		[SerializeField]
		private LocalizedTexture m_Round_xxhdpi;

		[SerializeField]
		private LocalizedTexture m_Round_xxxhdpi;

		internal List<LocalizedTexture> RoundIcons = new List<LocalizedTexture>();
	}
}
