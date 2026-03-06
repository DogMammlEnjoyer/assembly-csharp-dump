using System;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.Universal
{
	[MovedFrom(false, "UnityEngine.Experimental.Rendering.Universal", "com.unity.render-pipelines.universal", null)]
	public abstract class ShadowCasterGroup2D : MonoBehaviour
	{
		internal virtual void CacheValues()
		{
			if (this.m_ShadowCasters != null)
			{
				for (int i = 0; i < this.m_ShadowCasters.Count; i++)
				{
					this.m_ShadowCasters[i].CacheValues();
				}
			}
		}

		public List<ShadowCaster2D> GetShadowCasters()
		{
			return this.m_ShadowCasters;
		}

		public int GetShadowGroup()
		{
			return this.m_ShadowGroup;
		}

		public void RegisterShadowCaster2D(ShadowCaster2D shadowCaster2D)
		{
			if (this.m_ShadowCasters == null)
			{
				this.m_ShadowCasters = new List<ShadowCaster2D>();
			}
			int num = 0;
			while (num < this.m_ShadowCasters.Count && shadowCaster2D.m_Priority < this.m_ShadowCasters[num].m_Priority)
			{
				num++;
			}
			this.m_ShadowCasters.Insert(num, shadowCaster2D);
		}

		public void UnregisterShadowCaster2D(ShadowCaster2D shadowCaster2D)
		{
			if (this.m_ShadowCasters != null)
			{
				this.m_ShadowCasters.Remove(shadowCaster2D);
			}
		}

		[SerializeField]
		internal int m_ShadowGroup;

		[SerializeField]
		internal int m_Priority;

		private List<ShadowCaster2D> m_ShadowCasters;
	}
}
