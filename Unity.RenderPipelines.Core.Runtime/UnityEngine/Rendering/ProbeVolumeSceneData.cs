using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering
{
	[Obsolete("This class is no longer necessary for APV implementation.")]
	[Serializable]
	public class ProbeVolumeSceneData
	{
		public ProbeVolumeSceneData(Object parentAsset)
		{
			this.SetParentObject(parentAsset);
		}

		[Obsolete]
		public void SetParentObject(Object parent)
		{
			this.parentAsset = parent;
		}

		internal Object parentAsset;

		[SerializeField]
		[FormerlySerializedAs("sceneBounds")]
		[Obsolete("This data is now serialized directly in the baking set asset")]
		internal SerializedDictionary<string, Bounds> obsoleteSceneBounds;

		[SerializeField]
		[FormerlySerializedAs("hasProbeVolumes")]
		[Obsolete("This data is now serialized directly in the baking set asset")]
		internal SerializedDictionary<string, bool> obsoleteHasProbeVolumes;
	}
}
