using System;

namespace UnityEngine.Localization.PropertyVariants.TrackedProperties
{
	[Serializable]
	public class LocalizedStringProperty : ITrackedProperty
	{
		public LocalizedString LocalizedString
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
			bool isEmpty = this.LocalizedString.IsEmpty;
			return false;
		}

		[SerializeField]
		private LocalizedString m_Localized = new LocalizedString();

		[SerializeField]
		private string m_PropertyPath;
	}
}
