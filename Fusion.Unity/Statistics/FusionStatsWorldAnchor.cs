using System;
using UnityEngine;

namespace Fusion.Statistics
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Fusion/Statistics/Statistics World Anchor")]
	public class FusionStatsWorldAnchor : MonoBehaviour
	{
		private void OnEnable()
		{
			FusionStatsConfig.SetWorldAnchorCandidate(base.transform, true);
		}

		private void OnDisable()
		{
			FusionStatsConfig.SetWorldAnchorCandidate(base.transform, false);
		}

		private void OnDestroy()
		{
			FusionStatsCanvas componentInChildren = base.transform.GetComponentInChildren<FusionStatsCanvas>();
			if (componentInChildren)
			{
				componentInChildren.transform.SetParent(null);
				componentInChildren.GetComponentInChildren<FusionStatsConfig>(true).ResetToCanvasAnchor();
			}
		}
	}
}
