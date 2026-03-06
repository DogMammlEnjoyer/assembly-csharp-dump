using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Colocation
{
	internal class AutomaticColocationLauncher
	{
		public event Action ColocationReady;

		public event Action<ColocationFailedReason> ColocationFailed;

		public void Init(INetworkData networkData, INetworkMessenger networkMessenger, SharedAnchorManager sharedAnchorManager, GameObject cameraRig, ulong myPlayerId, ulong myOculusId)
		{
			Logger.Log("AutomaticColocationLauncher: Init function called", LogLevel.Verbose);
			this._networkData = networkData;
			this._networkMessenger = networkMessenger;
			this._networkMessenger.AnchorShareRequestReceived += this.OnAnchorShareRequestReceived;
			this._networkMessenger.AnchorShareRequestCompleted += this.OnAnchorShareRequestCompleted;
			this._sharedAnchorManager = sharedAnchorManager;
			this._cameraRig = cameraRig;
			this._myPlayerId = myPlayerId;
			this._myOculusId = myOculusId;
		}

		public void ColocateAutomatically()
		{
			this.ColocateAutomaticallyInternal();
		}

		public void ColocateByPlayerWithOculusId(ulong oculusId)
		{
			this.ColocateByPlayerWithOculusIdInternal(oculusId);
		}

		public void CreateColocatedSpace()
		{
			this.CreateColocatedSpaceInternal();
		}

		private void ColocateAutomaticallyInternal()
		{
			AutomaticColocationLauncher.<ColocateAutomaticallyInternal>d__19 <ColocateAutomaticallyInternal>d__;
			<ColocateAutomaticallyInternal>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ColocateAutomaticallyInternal>d__.<>4__this = this;
			<ColocateAutomaticallyInternal>d__.<>1__state = -1;
			<ColocateAutomaticallyInternal>d__.<>t__builder.Start<AutomaticColocationLauncher.<ColocateAutomaticallyInternal>d__19>(ref <ColocateAutomaticallyInternal>d__);
		}

		private void ColocateByPlayerWithOculusIdInternal(ulong oculusId)
		{
			AutomaticColocationLauncher.<ColocateByPlayerWithOculusIdInternal>d__20 <ColocateByPlayerWithOculusIdInternal>d__;
			<ColocateByPlayerWithOculusIdInternal>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ColocateByPlayerWithOculusIdInternal>d__.<>4__this = this;
			<ColocateByPlayerWithOculusIdInternal>d__.oculusId = oculusId;
			<ColocateByPlayerWithOculusIdInternal>d__.<>1__state = -1;
			<ColocateByPlayerWithOculusIdInternal>d__.<>t__builder.Start<AutomaticColocationLauncher.<ColocateByPlayerWithOculusIdInternal>d__20>(ref <ColocateByPlayerWithOculusIdInternal>d__);
		}

		private Anchor? FindAlignmentAnchorUsedByOculusId(ulong oculusId)
		{
			List<Player> allPlayers = this._networkData.GetAllPlayers();
			uint? num = null;
			foreach (Player player in allPlayers)
			{
				if (oculusId == player.oculusId)
				{
					num = new uint?(player.colocationGroupId);
				}
			}
			if (num == null)
			{
				Logger.Log(string.Format("{0}: Could not find the colocated group belonging to oculusId: {1}", "AutomaticColocationLauncher", oculusId), LogLevel.Error);
				return null;
			}
			foreach (Anchor anchor in this._networkData.GetAllAnchors())
			{
				if (num.Value == anchor.colocationGroupId)
				{
					return new Anchor?(anchor);
				}
			}
			Logger.Log(string.Format("{0}: Could not find the anchor belonging on colocationGroupId: {1}", "AutomaticColocationLauncher", num), LogLevel.Error);
			return null;
		}

		private void CreateColocatedSpaceInternal()
		{
			this.CreateNewColocatedSpace();
		}

		private void CreateNewColocatedSpace()
		{
			AutomaticColocationLauncher.<CreateNewColocatedSpace>d__23 <CreateNewColocatedSpace>d__;
			<CreateNewColocatedSpace>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<CreateNewColocatedSpace>d__.<>4__this = this;
			<CreateNewColocatedSpace>d__.<>1__state = -1;
			<CreateNewColocatedSpace>d__.<>t__builder.Start<AutomaticColocationLauncher.<CreateNewColocatedSpace>d__23>(ref <CreateNewColocatedSpace>d__);
		}

		private void AlignPlayerToAnchor()
		{
			Logger.Log("AutomaticColocationLauncher AlignPlayerToAnchor was called", LogLevel.Verbose);
			AlignCameraToAnchor alignCameraToAnchor = this._cameraRig.AddComponent<AlignCameraToAnchor>();
			alignCameraToAnchor.CameraAlignmentAnchor = this._myAlignmentAnchor;
			alignCameraToAnchor.RealignToAnchor();
		}

		private List<Anchor> GetAllAlignmentAnchors()
		{
			List<Anchor> list = new List<Anchor>();
			foreach (Anchor anchor in this._networkData.GetAllAnchors())
			{
				if (anchor.isAlignmentAnchor)
				{
					list.Add(anchor);
				}
			}
			return list;
		}

		private Task<bool> ShareAndLocalizeAnchor(Anchor anchor)
		{
			this._alignToAnchorTask = new TaskCompletionSource<bool>();
			this.SendAnchorShareRequest(anchor);
			return this._alignToAnchorTask.Task;
		}

		private void SendAnchorShareRequest(Anchor anchor)
		{
			Logger.Log(string.Format("{0}: Called {1} with anchor id: {2}, playerId: {3}, oculusId: {4}", new object[]
			{
				"AutomaticColocationLauncher",
				"SendAnchorShareRequest",
				anchor.automaticAnchorUuid,
				this._myPlayerId,
				this._myOculusId
			}), LogLevel.Verbose);
			Player? playerWithOculusId = this._networkData.GetPlayerWithOculusId(anchor.ownerOculusId);
			if (playerWithOculusId == null)
			{
				Logger.Log(string.Format("{0}: Anchor owner {1} isn't connected.", "AutomaticColocationLauncher", anchor.ownerOculusId), LogLevel.Error);
				this._alignToAnchorTask.TrySetResult(false);
			}
			ulong playerId = playerWithOculusId.Value.playerId;
			Logger.Log(string.Format("{0}: Request anchor sharing from playerId: {1}, oculusId: {2}", "AutomaticColocationLauncher", playerId, anchor.ownerOculusId), LogLevel.Info);
			this._networkMessenger.SendAnchorShareRequest(playerId, new ShareAndLocalizeParams(this._myPlayerId, this._myOculusId, anchor.automaticAnchorUuid));
		}

		private void OnAnchorShareRequestReceived(ShareAndLocalizeParams shareAndLocalizeParams)
		{
			AutomaticColocationLauncher.<OnAnchorShareRequestReceived>d__28 <OnAnchorShareRequestReceived>d__;
			<OnAnchorShareRequestReceived>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnAnchorShareRequestReceived>d__.<>4__this = this;
			<OnAnchorShareRequestReceived>d__.shareAndLocalizeParams = shareAndLocalizeParams;
			<OnAnchorShareRequestReceived>d__.<>1__state = -1;
			<OnAnchorShareRequestReceived>d__.<>t__builder.Start<AutomaticColocationLauncher.<OnAnchorShareRequestReceived>d__28>(ref <OnAnchorShareRequestReceived>d__);
		}

		private void OnAnchorShareRequestCompleted(ShareAndLocalizeParams shareAndLocalizeParams)
		{
			Logger.Log(string.Format("{0}: Called {1} with playerId: {2}, oculusId: {3}", new object[]
			{
				"AutomaticColocationLauncher",
				"OnAnchorShareRequestCompleted",
				this._myPlayerId,
				this._myOculusId
			}), LogLevel.Info);
			if (!shareAndLocalizeParams.anchorFlowSucceeded)
			{
				Logger.Log("AutomaticColocationLauncher: Anchor flow failed.", LogLevel.Error);
				Action<ColocationFailedReason> colocationFailed = this.ColocationFailed;
				if (colocationFailed != null)
				{
					colocationFailed(ColocationFailedReason.AutomaticFailedToShareAnchor);
				}
				this._alignToAnchorTask.TrySetResult(false);
				return;
			}
			Guid anchorToLocalize = new Guid(shareAndLocalizeParams.anchorUUID.ToString());
			this.LocalizeAnchor(anchorToLocalize);
		}

		private void LocalizeAnchor(Guid anchorToLocalize)
		{
			AutomaticColocationLauncher.<LocalizeAnchor>d__30 <LocalizeAnchor>d__;
			<LocalizeAnchor>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<LocalizeAnchor>d__.<>4__this = this;
			<LocalizeAnchor>d__.anchorToLocalize = anchorToLocalize;
			<LocalizeAnchor>d__.<>1__state = -1;
			<LocalizeAnchor>d__.<>t__builder.Start<AutomaticColocationLauncher.<LocalizeAnchor>d__30>(ref <LocalizeAnchor>d__);
		}

		private GameObject _cameraRig;

		private TaskCompletionSource<bool> _alignToAnchorTask;

		private OVRSpatialAnchor _myAlignmentAnchor;

		private ulong _myPlayerId;

		private ulong _myOculusId;

		private INetworkData _networkData;

		private INetworkMessenger _networkMessenger;

		private ulong _oculusIdToColocateTo;

		private SharedAnchorManager _sharedAnchorManager;
	}
}
