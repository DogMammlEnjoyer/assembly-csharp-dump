using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.BuildingBlocks
{
	public class SpatialAnchorLocalStorageManagerBuildingBlock : MonoBehaviour
	{
		private void Start()
		{
			this._spatialAnchorCore = SpatialAnchorCoreBuildingBlock.GetFirstInstance();
			this._spatialAnchorCore.OnAnchorCreateCompleted.AddListener(new UnityAction<OVRSpatialAnchor, OVRSpatialAnchor.OperationResult>(this.SaveAnchorUuidToLocalStorage));
			this._spatialAnchorCore.OnAnchorEraseCompleted.AddListener(new UnityAction<OVRSpatialAnchor, OVRSpatialAnchor.OperationResult>(this.RemoveAnchorFromLocalStorage));
		}

		internal void SaveAnchorUuidToLocalStorage(OVRSpatialAnchor anchor, OVRSpatialAnchor.OperationResult result)
		{
			if (result != OVRSpatialAnchor.OperationResult.Success)
			{
				return;
			}
			if (!PlayerPrefs.HasKey("numUuids"))
			{
				PlayerPrefs.SetInt("numUuids", 0);
			}
			int @int = PlayerPrefs.GetInt("numUuids");
			PlayerPrefs.SetString("uuid" + @int.ToString(), anchor.Uuid.ToString());
			PlayerPrefs.SetInt("numUuids", @int + 1);
		}

		internal void RemoveAnchorFromLocalStorage(OVRSpatialAnchor anchor, OVRSpatialAnchor.OperationResult result)
		{
			Guid uuid = anchor.Uuid;
			if (result == OVRSpatialAnchor.OperationResult.Failure)
			{
				return;
			}
			int num = PlayerPrefs.GetInt("numUuids", 0);
			for (int i = 0; i < num; i++)
			{
				string key = "uuid" + i.ToString();
				if (PlayerPrefs.GetString(key, "").Equals(uuid.ToString()))
				{
					string key2 = "uuid" + (num - 1).ToString();
					string @string = PlayerPrefs.GetString(key2);
					PlayerPrefs.SetString(key, @string);
					PlayerPrefs.DeleteKey(key2);
					num--;
					if (num < 0)
					{
						num = 0;
					}
					PlayerPrefs.SetInt("numUuids", num);
					return;
				}
			}
		}

		internal void GetAnchorAnchorUuidFromLocalStorage(List<Guid> uuids)
		{
			if (!PlayerPrefs.HasKey("numUuids"))
			{
				this.Reset();
				Debug.Log("[SpatialAnchorLocalStorageManagerBuildingBlock] Anchor not found.");
				return;
			}
			uuids.Clear();
			int @int = PlayerPrefs.GetInt("numUuids");
			for (int i = 0; i < @int; i++)
			{
				string key = "uuid" + i.ToString();
				if (PlayerPrefs.HasKey(key))
				{
					string @string = PlayerPrefs.GetString(key);
					uuids.Add(new Guid(@string));
				}
			}
		}

		public void Reset()
		{
			PlayerPrefs.SetInt("numUuids", 0);
		}

		private void OnDestroy()
		{
			this._spatialAnchorCore.OnAnchorCreateCompleted.RemoveAllListeners();
		}

		private SpatialAnchorCoreBuildingBlock _spatialAnchorCore;

		private const string NumUuidsPlayerPref = "numUuids";
	}
}
