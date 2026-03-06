using System;

namespace UnityEngine.ResourceManagement.ResourceProviders
{
	[Serializable]
	public class ProviderLoadRequestOptions
	{
		public ProviderLoadRequestOptions Copy()
		{
			return (ProviderLoadRequestOptions)base.MemberwiseClone();
		}

		public bool IgnoreFailures
		{
			get
			{
				return this.m_IgnoreFailures;
			}
			set
			{
				this.m_IgnoreFailures = value;
			}
		}

		public int WebRequestTimeout
		{
			get
			{
				return this.m_WebRequestTimeout;
			}
			set
			{
				this.m_WebRequestTimeout = value;
			}
		}

		[SerializeField]
		private bool m_IgnoreFailures;

		private int m_WebRequestTimeout;
	}
}
