using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering
{
	[ExecuteAlways]
	[AddComponentMenu("")]
	public class ProbeVolumePerSceneData : MonoBehaviour
	{
		public ProbeVolumeBakingSet bakingSet
		{
			get
			{
				return this.serializedBakingSet;
			}
		}

		internal void Clear()
		{
			this.QueueSceneRemoval();
			this.serializedBakingSet = null;
		}

		internal void QueueSceneLoading()
		{
			if (this.serializedBakingSet == null)
			{
				return;
			}
			ProbeReferenceVolume.instance.AddPendingSceneLoading(this.sceneGUID, this.serializedBakingSet);
		}

		internal void QueueSceneRemoval()
		{
			if (this.serializedBakingSet != null)
			{
				ProbeReferenceVolume.instance.AddPendingSceneRemoval(this.sceneGUID);
			}
		}

		private void OnEnable()
		{
			ProbeReferenceVolume.instance.RegisterPerSceneData(this);
		}

		private void OnDisable()
		{
			this.QueueSceneRemoval();
			ProbeReferenceVolume.instance.UnregisterPerSceneData(this);
		}

		private void OnValidate()
		{
		}

		internal void Initialize()
		{
			ProbeReferenceVolume.instance.RegisterBakingSet(this);
			this.QueueSceneRemoval();
			this.QueueSceneLoading();
		}

		internal bool ResolveCellData()
		{
			return this.serializedBakingSet != null && this.serializedBakingSet.ResolveCellData(this.serializedBakingSet.GetSceneCellIndexList(this.sceneGUID));
		}

		[SerializeField]
		[FormerlySerializedAs("bakingSet")]
		internal ProbeVolumeBakingSet serializedBakingSet;

		[SerializeField]
		internal string sceneGUID = "";

		[FormerlySerializedAs("asset")]
		[SerializeField]
		internal ObsoleteProbeVolumeAsset obsoleteAsset;

		[FormerlySerializedAs("cellSharedDataAsset")]
		[SerializeField]
		internal TextAsset obsoleteCellSharedDataAsset;

		[FormerlySerializedAs("cellSupportDataAsset")]
		[SerializeField]
		internal TextAsset obsoleteCellSupportDataAsset;

		[FormerlySerializedAs("serializedScenarios")]
		[SerializeField]
		private List<ProbeVolumePerSceneData.ObsoleteSerializablePerScenarioDataItem> obsoleteSerializedScenarios = new List<ProbeVolumePerSceneData.ObsoleteSerializablePerScenarioDataItem>();

		[Serializable]
		internal struct ObsoletePerScenarioData
		{
			public int sceneHash;

			public TextAsset cellDataAsset;

			public TextAsset cellOptionalDataAsset;
		}

		[Serializable]
		private struct ObsoleteSerializablePerScenarioDataItem
		{
			public string scenario;

			public ProbeVolumePerSceneData.ObsoletePerScenarioData data;
		}
	}
}
