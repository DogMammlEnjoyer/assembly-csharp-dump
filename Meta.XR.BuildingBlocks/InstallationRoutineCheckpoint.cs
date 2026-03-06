using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.XR.BuildingBlocks
{
	[Serializable]
	public class InstallationRoutineCheckpoint
	{
		public string InstallationRoutineId
		{
			get
			{
				return this._installationRoutineId;
			}
		}

		public List<VariantCheckpoint> InstallationVariants
		{
			get
			{
				return this._installationVariants;
			}
		}

		public InstallationRoutineCheckpoint(string installationRoutineId, List<VariantCheckpoint> installationVariants)
		{
			this._installationRoutineId = installationRoutineId;
			this._installationVariants = installationVariants;
		}

		[SerializeField]
		[HideInInspector]
		private string _installationRoutineId;

		[SerializeField]
		[HideInInspector]
		private List<VariantCheckpoint> _installationVariants;
	}
}
