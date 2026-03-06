using System;
using Meta.XR.MultiplayerBlocks.Colocation;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.MultiplayerBlocks.Shared
{
	public class ColocationController : MonoBehaviour
	{
		public void Awake()
		{
			if (this.DebuggingOptions.enableVerboseLogging)
			{
				Meta.XR.MultiplayerBlocks.Colocation.Logger.SetAllLogsVisibility(true);
				return;
			}
			Meta.XR.MultiplayerBlocks.Colocation.Logger.SetAllLogsVisibility(false);
			Meta.XR.MultiplayerBlocks.Colocation.Logger.SetLogLevelVisibility(LogLevel.Error, true);
			Meta.XR.MultiplayerBlocks.Colocation.Logger.SetLogLevelVisibility(LogLevel.SharedSpatialAnchorsError, true);
		}

		[SerializeField]
		public UnityEvent ColocationReadyCallbacks;

		[SerializeField]
		internal ColocationDebuggingOptions DebuggingOptions;
	}
}
