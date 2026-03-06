using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.XR.BuildingBlocks
{
	[RequireComponent(typeof(SpatialAnchorSpawnerBuildingBlock))]
	public class SpatialAnchorLoaderBuildingBlock : MonoBehaviour
	{
		private void Awake()
		{
			this._spatialAnchorSpawner = base.GetComponent<SpatialAnchorSpawnerBuildingBlock>();
			this._spatialAnchorCore = SpatialAnchorCoreBuildingBlock.GetFirstInstance();
		}

		public virtual void LoadAndInstantiateAnchors(List<Guid> uuids)
		{
			this._spatialAnchorCore.LoadAndInstantiateAnchors(this._spatialAnchorSpawner.AnchorPrefab, uuids);
		}

		public virtual void LoadAnchorsFromDefaultLocalStorage()
		{
			SpatialAnchorLocalStorageManagerBuildingBlock spatialAnchorLocalStorageManagerBuildingBlock = Object.FindAnyObjectByType<SpatialAnchorLocalStorageManagerBuildingBlock>();
			if (!spatialAnchorLocalStorageManagerBuildingBlock)
			{
				Debug.Log("[SpatialAnchorLocalStorageManagerBuildingBlock] component is missing.");
				return;
			}
			List<Guid> list;
			using (new OVRObjectPool.ListScope<Guid>(ref list))
			{
				spatialAnchorLocalStorageManagerBuildingBlock.GetAnchorAnchorUuidFromLocalStorage(list);
				if (list.Count > 0)
				{
					this._spatialAnchorCore.LoadAndInstantiateAnchors(this._spatialAnchorSpawner.AnchorPrefab, list);
				}
			}
		}

		private SpatialAnchorCoreBuildingBlock _spatialAnchorCore;

		private SpatialAnchorSpawnerBuildingBlock _spatialAnchorSpawner;
	}
}
