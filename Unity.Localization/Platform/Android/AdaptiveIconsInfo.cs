using System;
using System.Collections.Generic;
using UnityEngine.Localization.Metadata;

namespace UnityEngine.Localization.Platform.Android
{
	[DisplayName("Android Adaptive Icon Info", null)]
	[Metadata(AllowedTypes = MetadataType.LocalizationSettings, AllowMultiple = false, MenuItem = "Android/Adaptive Icon")]
	[Serializable]
	public class AdaptiveIconsInfo : IMetadata
	{
		internal void RefreshAdaptiveIcons()
		{
			this.AdaptiveIcons.Clear();
			this.AdaptiveIcons.Add(this.m_Adaptive_idpi);
			this.AdaptiveIcons.Add(this.m_Adaptive_mdpi);
			this.AdaptiveIcons.Add(this.m_Adaptive_hdpi);
			this.AdaptiveIcons.Add(this.m_Adaptive_xhdpi);
			this.AdaptiveIcons.Add(this.m_Adaptive_xxhdpi);
			this.AdaptiveIcons.Add(this.m_Adaptive_xxxhdpi);
		}

		public AdaptiveIcon AdaptiveHdpi
		{
			get
			{
				return this.m_Adaptive_hdpi;
			}
			set
			{
				this.m_Adaptive_hdpi = value;
			}
		}

		public AdaptiveIcon AdaptiveIdpi
		{
			get
			{
				return this.m_Adaptive_idpi;
			}
			set
			{
				this.m_Adaptive_idpi = value;
			}
		}

		public AdaptiveIcon AdaptiveMdpi
		{
			get
			{
				return this.m_Adaptive_mdpi;
			}
			set
			{
				this.m_Adaptive_mdpi = value;
			}
		}

		public AdaptiveIcon AdaptiveXhdpi
		{
			get
			{
				return this.m_Adaptive_xhdpi;
			}
			set
			{
				this.m_Adaptive_xhdpi = value;
			}
		}

		public AdaptiveIcon AdaptiveXXHdpi
		{
			get
			{
				return this.m_Adaptive_xxhdpi;
			}
			set
			{
				this.m_Adaptive_xxhdpi = value;
			}
		}

		public AdaptiveIcon AdaptiveXXXHdpi
		{
			get
			{
				return this.m_Adaptive_xxxhdpi;
			}
			set
			{
				this.m_Adaptive_xxxhdpi = value;
			}
		}

		[SerializeField]
		private AdaptiveIcon m_Adaptive_idpi;

		[SerializeField]
		private AdaptiveIcon m_Adaptive_mdpi;

		[SerializeField]
		private AdaptiveIcon m_Adaptive_hdpi;

		[SerializeField]
		private AdaptiveIcon m_Adaptive_xhdpi;

		[SerializeField]
		private AdaptiveIcon m_Adaptive_xxhdpi;

		[SerializeField]
		private AdaptiveIcon m_Adaptive_xxxhdpi;

		internal List<AdaptiveIcon> AdaptiveIcons = new List<AdaptiveIcon>();
	}
}
