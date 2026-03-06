using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Meta.XR.BuildingBlocks;
using Meta.XR.MultiplayerBlocks.Colocation;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.MultiplayerBlocks.Shared
{
	public class ColocationSessionEventHandler : MonoBehaviour
	{
		private void Awake()
		{
			this._colocationController = Object.FindObjectOfType<ColocationController>();
			this._cameraRig = Object.FindObjectOfType<OVRCameraRig>();
			if (this.basis == ColocationSessionEventHandler.Basis.RoomAnchors)
			{
				LocalMatchmaking.BeforeStartHost = new Func<Task<bool>>(this.SpaceSharingBeforeHostStart);
			}
		}

		private void Start()
		{
			ColocationSessionEventHandler.Basis basis = this.basis;
			if (basis != ColocationSessionEventHandler.Basis.SharedSpatialAnchor)
			{
				if (basis != ColocationSessionEventHandler.Basis.RoomAnchors)
				{
					throw new ArgumentOutOfRangeException();
				}
				LocalMatchmaking.OnSessionCreateSucceeded.AddListener(new UnityAction<Guid>(this.OnSessionCreatedWithSpaceSharing));
				LocalMatchmaking.OnSessionDiscoverSucceeded.AddListener(new UnityAction<Guid>(this.OnSessionDiscoveredWithSpaceSharing));
			}
			else
			{
				SharedSpatialAnchorCore sharedSpatialAnchorCore = Object.FindObjectOfType<SharedSpatialAnchorCore>();
				if (sharedSpatialAnchorCore == null)
				{
					throw new InvalidOperationException("SharedSpatialAnchorCore component is missing from the scene, add this component to allow anchor sharing.");
				}
				this._sharedAnchorManager = new SharedAnchorManager(sharedSpatialAnchorCore);
				if (this._colocationController.DebuggingOptions.visualizeAlignmentAnchor)
				{
					this._sharedAnchorManager.AnchorPrefab = this.AnchorPrefab;
				}
				LocalMatchmaking.OnSessionCreateSucceeded.AddListener(new UnityAction<Guid>(this.OnSessionCreatedWithSpatialAnchor));
				LocalMatchmaking.OnSessionDiscoverSucceeded.AddListener(new UnityAction<Guid>(this.OnSessionDiscoveredWithSpatialAnchor));
			}
			LocalMatchmaking.OnSessionCreateFailed.AddListener(new UnityAction<string>(Debug.LogError));
		}

		private void OnSessionCreatedWithSpatialAnchor(Guid groupUuid)
		{
			ColocationSessionEventHandler.<OnSessionCreatedWithSpatialAnchor>d__10 <OnSessionCreatedWithSpatialAnchor>d__;
			<OnSessionCreatedWithSpatialAnchor>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnSessionCreatedWithSpatialAnchor>d__.<>4__this = this;
			<OnSessionCreatedWithSpatialAnchor>d__.groupUuid = groupUuid;
			<OnSessionCreatedWithSpatialAnchor>d__.<>1__state = -1;
			<OnSessionCreatedWithSpatialAnchor>d__.<>t__builder.Start<ColocationSessionEventHandler.<OnSessionCreatedWithSpatialAnchor>d__10>(ref <OnSessionCreatedWithSpatialAnchor>d__);
		}

		private void OnSessionDiscoveredWithSpatialAnchor(Guid groupUuid)
		{
			ColocationSessionEventHandler.<OnSessionDiscoveredWithSpatialAnchor>d__11 <OnSessionDiscoveredWithSpatialAnchor>d__;
			<OnSessionDiscoveredWithSpatialAnchor>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnSessionDiscoveredWithSpatialAnchor>d__.<>4__this = this;
			<OnSessionDiscoveredWithSpatialAnchor>d__.groupUuid = groupUuid;
			<OnSessionDiscoveredWithSpatialAnchor>d__.<>1__state = -1;
			<OnSessionDiscoveredWithSpatialAnchor>d__.<>t__builder.Start<ColocationSessionEventHandler.<OnSessionDiscoveredWithSpatialAnchor>d__11>(ref <OnSessionDiscoveredWithSpatialAnchor>d__);
		}

		private Task<bool> SpaceSharingBeforeHostStart()
		{
			ColocationSessionEventHandler.<SpaceSharingBeforeHostStart>d__12 <SpaceSharingBeforeHostStart>d__;
			<SpaceSharingBeforeHostStart>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SpaceSharingBeforeHostStart>d__.<>4__this = this;
			<SpaceSharingBeforeHostStart>d__.<>1__state = -1;
			<SpaceSharingBeforeHostStart>d__.<>t__builder.Start<ColocationSessionEventHandler.<SpaceSharingBeforeHostStart>d__12>(ref <SpaceSharingBeforeHostStart>d__);
			return <SpaceSharingBeforeHostStart>d__.<>t__builder.Task;
		}

		private Task<bool> RequestScenePermissionIfNeeded()
		{
			ColocationSessionEventHandler.<RequestScenePermissionIfNeeded>d__13 <RequestScenePermissionIfNeeded>d__;
			<RequestScenePermissionIfNeeded>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<RequestScenePermissionIfNeeded>d__.<>1__state = -1;
			<RequestScenePermissionIfNeeded>d__.<>t__builder.Start<ColocationSessionEventHandler.<RequestScenePermissionIfNeeded>d__13>(ref <RequestScenePermissionIfNeeded>d__);
			return <RequestScenePermissionIfNeeded>d__.<>t__builder.Task;
		}

		private Task<bool> LoadScene()
		{
			ColocationSessionEventHandler.<LoadScene>d__14 <LoadScene>d__;
			<LoadScene>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<LoadScene>d__.<>1__state = -1;
			<LoadScene>d__.<>t__builder.Start<ColocationSessionEventHandler.<LoadScene>d__14>(ref <LoadScene>d__);
			return <LoadScene>d__.<>t__builder.Task;
		}

		private void OnSessionCreatedWithSpaceSharing(Guid groupUuid)
		{
			ColocationSessionEventHandler.<OnSessionCreatedWithSpaceSharing>d__15 <OnSessionCreatedWithSpaceSharing>d__;
			<OnSessionCreatedWithSpaceSharing>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnSessionCreatedWithSpaceSharing>d__.<>4__this = this;
			<OnSessionCreatedWithSpaceSharing>d__.groupUuid = groupUuid;
			<OnSessionCreatedWithSpaceSharing>d__.<>1__state = -1;
			<OnSessionCreatedWithSpaceSharing>d__.<>t__builder.Start<ColocationSessionEventHandler.<OnSessionCreatedWithSpaceSharing>d__15>(ref <OnSessionCreatedWithSpaceSharing>d__);
		}

		private void OnSessionDiscoveredWithSpaceSharing(Guid groupUuid)
		{
			ColocationSessionEventHandler.<OnSessionDiscoveredWithSpaceSharing>d__16 <OnSessionDiscoveredWithSpaceSharing>d__;
			<OnSessionDiscoveredWithSpaceSharing>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<OnSessionDiscoveredWithSpaceSharing>d__.<>4__this = this;
			<OnSessionDiscoveredWithSpaceSharing>d__.groupUuid = groupUuid;
			<OnSessionDiscoveredWithSpaceSharing>d__.<>1__state = -1;
			<OnSessionDiscoveredWithSpaceSharing>d__.<>t__builder.Start<ColocationSessionEventHandler.<OnSessionDiscoveredWithSpaceSharing>d__16>(ref <OnSessionDiscoveredWithSpaceSharing>d__);
		}

		private void OnDestroy()
		{
			ColocationSessionEventHandler.Basis basis = this.basis;
			if (basis == ColocationSessionEventHandler.Basis.SharedSpatialAnchor)
			{
				LocalMatchmaking.OnSessionCreateSucceeded.RemoveListener(new UnityAction<Guid>(this.OnSessionCreatedWithSpatialAnchor));
				LocalMatchmaking.OnSessionDiscoverSucceeded.RemoveListener(new UnityAction<Guid>(this.OnSessionDiscoveredWithSpatialAnchor));
				return;
			}
			if (basis != ColocationSessionEventHandler.Basis.RoomAnchors)
			{
				throw new ArgumentOutOfRangeException();
			}
			LocalMatchmaking.OnSessionCreateSucceeded.RemoveListener(new UnityAction<Guid>(this.OnSessionCreatedWithSpaceSharing));
			LocalMatchmaking.OnSessionDiscoverSucceeded.RemoveListener(new UnityAction<Guid>(this.OnSessionDiscoveredWithSpaceSharing));
			LocalMatchmaking.BeforeStartHost = null;
		}

		[Tooltip("The basis alignment/common reference approach for colocation")]
		[SerializeField]
		internal ColocationSessionEventHandler.Basis basis;

		[SerializeField]
		private GameObject AnchorPrefab;

		private ColocationController _colocationController;

		private SharedAnchorManager _sharedAnchorManager;

		private AlignCameraToAnchor _alignCameraToAnchor;

		private OVRCameraRig _cameraRig;

		internal enum Basis
		{
			SharedSpatialAnchor,
			RoomAnchors
		}

		[Serializable]
		private struct SpaceSharingInfo
		{
			internal Guid RoomId;

			internal Pose FloorAnchor;
		}
	}
}
