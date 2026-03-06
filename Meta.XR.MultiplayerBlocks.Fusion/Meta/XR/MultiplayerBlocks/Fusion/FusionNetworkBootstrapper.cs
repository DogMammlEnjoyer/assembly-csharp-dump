using System;
using Fusion;
using Meta.XR.MultiplayerBlocks.Colocation;
using Meta.XR.MultiplayerBlocks.Colocation.Fusion;
using Meta.XR.MultiplayerBlocks.Shared;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Fusion
{
	[NetworkBehaviourWeaved(0)]
	public class FusionNetworkBootstrapper : NetworkBehaviour
	{
		private void Awake()
		{
			this._params.ovrCameraRig = UnityEngine.Object.FindObjectOfType<OVRCameraRig>();
			this._params.colocationController = UnityEngine.Object.FindObjectOfType<ColocationController>();
			this._params.setupColocationReadyEvents = delegate()
			{
				this._params.colocationLauncher.ColocationReady += this.OnColocationReady;
			};
		}

		public override void Spawned()
		{
			PlatformInit.GetEntitlementInformation(delegate(PlatformInfo info)
			{
				if (info.OculusUser != null)
				{
					NetworkBootstrapperUtils.SetEntitlementIds(info, ref this._params);
					NetworkBootstrapperUtils.SetUpAndStartAutomaticColocation(ref this._params, this.anchorPrefab, this.networkData, this.networkMessenger);
				}
			});
		}

		private void OnColocationReady()
		{
			if (this._params.colocationController != null)
			{
				this._params.colocationController.ColocationReadyCallbacks.Invoke();
			}
			Logger.Log("FusionNetworkBootstrapper: Colocation is successful and ready", LogLevel.Info);
		}

		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
		}

		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
		}

		[SerializeField]
		private GameObject anchorPrefab;

		[SerializeField]
		private FusionNetworkData networkData;

		[SerializeField]
		private FusionMessenger networkMessenger;

		private NetworkBootstrapperParams _params;
	}
}
