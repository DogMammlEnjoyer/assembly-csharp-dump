using System;
using UnityEngine;

namespace Fusion.Statistics
{
	[RequireComponent(typeof(NetworkObject))]
	[DisallowMultipleComponent]
	[AddComponentMenu("Fusion/Statistics/Network Object Statistics")]
	public class FusionNetworkObjectStatistics : MonoBehaviour
	{
		private void ToggleMonitoring(bool value)
		{
			this.NetworkObject = base.GetComponent<NetworkObject>();
			FusionStatistics fusionStatistics;
			if (this.NetworkObject.Runner && this.NetworkObject.Runner.IsRunning && this.NetworkObject.Runner.TryGetComponent<FusionStatistics>(out fusionStatistics) && fusionStatistics.MonitorNetworkObject(this.NetworkObject, this, value))
			{
				return;
			}
			Object.Destroy(this);
		}

		private void OnEnable()
		{
			this.ToggleMonitoring(true);
		}

		private void OnDisable()
		{
			this.ToggleMonitoring(false);
		}

		[HideInInspector]
		public NetworkObject NetworkObject;
	}
}
