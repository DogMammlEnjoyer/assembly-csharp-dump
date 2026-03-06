using System;

namespace UnityEngine.Localization.PropertyVariants.TrackedProperties
{
	[Serializable]
	public class LocalizedAssetProperty : ITrackedProperty
	{
		public LocalizedAssetBase LocalizedObject
		{
			get
			{
				return this.m_Localized;
			}
			set
			{
				this.m_Localized = value;
			}
		}

		public string PropertyPath
		{
			get
			{
				return this.m_PropertyPath;
			}
			set
			{
				this.m_PropertyPath = value;
			}
		}

		public bool HasVariant(LocaleIdentifier localeIdentifier)
		{
			bool isEmpty = this.LocalizedObject.IsEmpty;
			return false;
		}

		[SerializeReference]
		private LocalizedAssetBase m_Localized;

		[SerializeField]
		private string m_PropertyPath;
	}
}
