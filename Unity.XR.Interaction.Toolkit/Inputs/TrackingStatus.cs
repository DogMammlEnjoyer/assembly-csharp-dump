using System;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs
{
	public struct TrackingStatus
	{
		public bool isConnected { readonly get; set; }

		public bool isTracked { readonly get; set; }

		public InputTrackingState trackingState { readonly get; set; }
	}
}
