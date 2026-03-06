using System;
using UnityEngine;

namespace Meta.XR.BuildingBlocks
{
	[HelpURL("https://developer.oculus.com/documentation/unity/bb-overview/")]
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	public class BuildingBlock : MonoBehaviour
	{
		public string BlockId
		{
			get
			{
				return this.blockId;
			}
		}

		public string InstanceId
		{
			get
			{
				return this.instanceId;
			}
		}

		public int Version
		{
			get
			{
				return this.version;
			}
		}

		public InstallationRoutineCheckpoint InstallationRoutineCheckpoint
		{
			get
			{
				return this.installationRoutineCheckpoint;
			}
			set
			{
				this.installationRoutineCheckpoint = value;
			}
		}

		private void Awake()
		{
			if (Application.isPlaying)
			{
				return;
			}
			if (this.HasDuplicateInstanceId())
			{
				this.ResetInstanceId();
			}
		}

		private void ResetInstanceId()
		{
			this.instanceId = Guid.NewGuid().ToString();
		}

		private bool HasDuplicateInstanceId()
		{
			foreach (BuildingBlock buildingBlock in Object.FindObjectsByType<BuildingBlock>(FindObjectsSortMode.InstanceID))
			{
				if (buildingBlock != this && buildingBlock.InstanceId == this.InstanceId)
				{
					return true;
				}
			}
			return false;
		}

		private void Start()
		{
			OVRTelemetry.Start(163063912, 0, -1L).AddBlockInfo(this).SendIf(Application.isPlaying);
		}

		[SerializeField]
		[OVRReadOnly]
		internal string blockId;

		[SerializeField]
		[HideInInspector]
		internal string instanceId = Guid.NewGuid().ToString();

		[SerializeField]
		[OVRReadOnly]
		internal int version = 1;

		[SerializeField]
		[HideInInspector]
		private InstallationRoutineCheckpoint installationRoutineCheckpoint;
	}
}
