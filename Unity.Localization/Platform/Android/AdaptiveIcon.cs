using System;

namespace UnityEngine.Localization.Platform.Android
{
	[Serializable]
	public class AdaptiveIcon
	{
		public LocalizedTexture Background
		{
			get
			{
				return this.m_Background;
			}
			set
			{
				this.m_Background = value;
			}
		}

		public LocalizedTexture Foreground
		{
			get
			{
				return this.m_Foreground;
			}
			set
			{
				this.m_Foreground = value;
			}
		}

		[SerializeField]
		private LocalizedTexture m_Background;

		[SerializeField]
		private LocalizedTexture m_Foreground;
	}
}
