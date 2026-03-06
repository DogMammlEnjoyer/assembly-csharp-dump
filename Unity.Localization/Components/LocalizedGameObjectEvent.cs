using System;
using UnityEngine.Localization.Events;

namespace UnityEngine.Localization.Components
{
	[AddComponentMenu("Localization/Asset/Localize Prefab Event")]
	public class LocalizedGameObjectEvent : LocalizedAssetEvent<GameObject, LocalizedGameObject, UnityEventGameObject>
	{
		protected override void UpdateAsset(GameObject localizedAsset)
		{
			if (this.m_Current != null)
			{
				Object.Destroy(this.m_Current);
				this.m_Current = null;
			}
			if (localizedAsset != null)
			{
				this.m_Current = Object.Instantiate<GameObject>(localizedAsset, base.transform);
				this.m_Current.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.NotEditable | HideFlags.DontSaveInBuild | HideFlags.DontUnloadUnusedAsset);
			}
			base.OnUpdateAsset.Invoke(this.m_Current);
		}

		private GameObject m_Current;
	}
}
