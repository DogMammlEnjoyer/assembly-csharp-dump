using System;
using UnityEngine.Localization.Metadata;

namespace UnityEngine.Localization.Platform.Android
{
	[DisplayName("Android App Info", "Packages/com.unity.localization/Editor/Icons/Android/Android.png")]
	[Metadata(AllowedTypes = MetadataType.LocalizationSettings, AllowMultiple = false, MenuItem = "Android/App Info")]
	[Serializable]
	public class AppInfo : IMetadata
	{
		public LocalizedString DisplayName
		{
			get
			{
				return this.m_DisplayName;
			}
			set
			{
				this.m_DisplayName = value;
			}
		}

		[Tooltip("The user-visible name for the bundle, used by Google Assistant and visible on the Android Home screen.\n")]
		[SerializeField]
		private LocalizedString m_DisplayName = new LocalizedString();
	}
}
