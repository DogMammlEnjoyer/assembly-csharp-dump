using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class TakeMyHand_HandLink : HoldableObject, IGorillaSliceableSimple
{
	public bool IsTentacleGrab { get; private set; }

	public bool IsLocal { get; private set; }

	private void Start()
	{
		this.myOtherHandLink = (this.isLeftHand ? this.myRig.rightHandLink : this.myRig.leftHandLink);
		if (this.myRig.isOfflineVRRig)
		{
			base.gameObject.SetActive(false);
			this.IsLocal = true;
		}
		if (this.interactionPoint == null)
		{
			this.interactionPoint = base.GetComponent<InteractionPoint>();
		}
	}

	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	public void SliceUpdate()
	{
		this.interactionPoint.enabled = (this.isReadyForGrabbing && (this.myRig.transform.position - VRRig.LocalRig.transform.position).sqrMagnitude < 9f);
	}

	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.CanBeGrabbed())
		{
			return;
		}
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager != null && gorillaGuardianManager.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			(this.isLeftHand ? this.myRig.leftHolds : this.myRig.rightHolds).OnGrab(pointGrabbed, grabbingHand);
			return;
		}
		TakeMyHand_HandLink takeMyHand_HandLink = (grabbingHand == EquipmentInteractor.instance.leftHand) ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
		if (takeMyHand_HandLink.isReadyForGrabbing && Time.time - takeMyHand_HandLink.gripPressedAtTimestamp < 0.1f)
		{
			takeMyHand_HandLink.LocalCreateLink(this);
		}
	}

	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.myRig.isOfflineVRRig)
		{
			TakeMyHand_HandLink takeMyHand_HandLink = (releasingHand == EquipmentInteractor.instance.leftHand) ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
			bool flag = false;
			HandLinkAuthorityStatus handLinkAuthorityStatus = GTPlayer.Instance.TakeMyHand_GetSelfHandLinkAuthority();
			int num;
			HandLinkAuthorityStatus chainAuthority = takeMyHand_HandLink.GetChainAuthority(out num);
			if (handLinkAuthorityStatus.type >= HandLinkAuthorityType.ButtGrounded && chainAuthority.type < handLinkAuthorityStatus.type)
			{
				flag = true;
			}
			else if (takeMyHand_HandLink.myOtherHandLink.grabbedLink != null)
			{
				int num2;
				HandLinkAuthorityStatus chainAuthority2 = takeMyHand_HandLink.myOtherHandLink.GetChainAuthority(out num2);
				if (chainAuthority2.type >= HandLinkAuthorityType.ButtGrounded && chainAuthority.type < chainAuthority2.type)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Vector3 averageVelocity = GTPlayer.Instance.GetHandVelocityTracker(takeMyHand_HandLink.isLeftHand).GetAverageVelocity(true, 0.15f, false);
				this.myRig.netView.SendRPC("DroppedByPlayer", this.myRig.OwningNetPlayer, new object[]
				{
					averageVelocity
				});
				this.myRig.ApplyLocalTrajectoryOverride(averageVelocity);
			}
			takeMyHand_HandLink.BreakLink();
		}
		return true;
	}

	public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	public override void DropItemCleanup()
	{
		if (this.grabbedLink != null)
		{
			this.grabbedLink.BreakLink();
		}
	}

	public bool CanBeGrabbed()
	{
		return (!GorillaComputer.instance.IsPlayerInVirtualStump() || !CustomMapManager.WantsHoldingHandsDisabled()) && Time.time >= this.rejectGrabsUntilTimestamp && this.isReadyForGrabbing && this.grabbedPlayer == null;
	}

	public bool IsLinkActive()
	{
		return this.grabbedLink != null;
	}

	public bool TentacleTryCreateLink(TakeMyHand_HandLink remoteLink)
	{
		if (!this.myRig.isLocal || this.grabbedPlayer != null)
		{
			return false;
		}
		if (GorillaComputer.instance.IsPlayerInVirtualStump() && CustomMapManager.WantsHoldingHandsDisabled())
		{
			return false;
		}
		if (Time.time < this.rejectGrabsUntilTimestamp)
		{
			return false;
		}
		if (!remoteLink.CanBeGrabbed())
		{
			return false;
		}
		GRPlayer grplayer = GRPlayer.Get(remoteLink.myRig);
		GRPlayer grplayer2 = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer);
		if (grplayer2 != null && grplayer != null && grplayer2.State == GRPlayer.GRPlayerState.Ghost != (grplayer.State == GRPlayer.GRPlayerState.Ghost))
		{
			return false;
		}
		this.IsTentacleGrab = true;
		this.grabbedLink = remoteLink;
		this.grabbedLink.TentacleOffset = Vector3.zero;
		this.grabbedPlayer = remoteLink.myRig.OwningNetPlayer;
		this.grabbedHandIsLeft = remoteLink.isLeftHand;
		Action onHandLinkChanged = TakeMyHand_HandLink.OnHandLinkChanged;
		if (onHandLinkChanged != null)
		{
			onHandLinkChanged();
		}
		return true;
	}

	public Vector3 TentacleOffset { get; set; }

	public Vector3 LinkPosition
	{
		get
		{
			return base.transform.position + this.TentacleOffset;
		}
	}

	private void LocalCreateLink(TakeMyHand_HandLink remoteLink)
	{
		if (this.grabbedPlayer != null || !this.myRig.isLocal)
		{
			return;
		}
		GRPlayer grplayer = GRPlayer.Get(remoteLink.myRig);
		GRPlayer grplayer2 = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer);
		if (grplayer2 != null && grplayer != null && grplayer2.State == GRPlayer.GRPlayerState.Ghost != (grplayer.State == GRPlayer.GRPlayerState.Ghost))
		{
			return;
		}
		EquipmentInteractor.instance.UpdateHandEquipment(remoteLink, this.isLeftHand);
		this.grabbedLink = remoteLink;
		this.grabbedPlayer = remoteLink.myRig.OwningNetPlayer;
		this.grabbedHandIsLeft = remoteLink.isLeftHand;
		this.TentacleOffset = Vector3.zero;
		if (remoteLink.IsTentacleGrab)
		{
			remoteLink.TentacleOffset = base.transform.position - remoteLink.transform.position;
		}
		else
		{
			remoteLink.TentacleOffset = Vector3.zero;
		}
		GorillaTagger.Instance.StartVibration(this.isLeftHand, this.hapticStrengthOnGrab, this.hapticDurationOnGrab);
		(this.isLeftHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer).GTPlayOneShot(this.audioOnGrab, 1f);
		Action onHandLinkChanged = TakeMyHand_HandLink.OnHandLinkChanged;
		if (onHandLinkChanged == null)
		{
			return;
		}
		onHandLinkChanged();
	}

	public void BreakLinkTo(TakeMyHand_HandLink targetLink)
	{
		if (this.grabbedLink == targetLink)
		{
			this.BreakLink();
		}
	}

	public void BreakLink()
	{
		if (this.grabbedPlayer == null || this.grabbedLink == null)
		{
			return;
		}
		Vector3 velocity = this.myRig.LatestVelocity();
		GTPlayer.Instance.SetVelocity(velocity);
		this.IsTentacleGrab = false;
		this.TentacleOffset = Vector3.zero;
		this.grabbedLink = null;
		this.grabbedPlayer = null;
		this.grabbedHandIsLeft = false;
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isLeftHand);
		Action onHandLinkChanged = TakeMyHand_HandLink.OnHandLinkChanged;
		if (onHandLinkChanged == null)
		{
			return;
		}
		onHandLinkChanged();
	}

	public static bool IsHandInChainWithOtherPlayer(TakeMyHand_HandLink startingLink, int targetPlayer)
	{
		TakeMyHand_HandLink takeMyHand_HandLink = startingLink;
		int num = 0;
		int roomPlayerCount = NetworkSystem.Instance.RoomPlayerCount;
		while (takeMyHand_HandLink != null && num < roomPlayerCount)
		{
			if (takeMyHand_HandLink.myRig == null || takeMyHand_HandLink.myRig.creator == null)
			{
				return false;
			}
			if (takeMyHand_HandLink.myRig.creator.ActorNumber == targetPlayer)
			{
				return true;
			}
			TakeMyHand_HandLink takeMyHand_HandLink2 = null;
			RigContainer rigContainer;
			if (takeMyHand_HandLink.grabbedLink != null && takeMyHand_HandLink.grabbedLink.myOtherHandLink != null)
			{
				takeMyHand_HandLink2 = takeMyHand_HandLink.grabbedLink.myOtherHandLink;
			}
			else if (takeMyHand_HandLink.grabbedPlayer != null && VRRigCache.Instance.TryGetVrrig(takeMyHand_HandLink.grabbedPlayer, out rigContainer))
			{
				TakeMyHand_HandLink takeMyHand_HandLink3 = takeMyHand_HandLink.grabbedHandIsLeft ? rigContainer.Rig.leftHandLink : rigContainer.Rig.rightHandLink;
				if (takeMyHand_HandLink3 != null && takeMyHand_HandLink3.myOtherHandLink != null)
				{
					takeMyHand_HandLink2 = takeMyHand_HandLink3.myOtherHandLink;
				}
			}
			takeMyHand_HandLink = takeMyHand_HandLink2;
			num++;
		}
		return false;
	}

	public void LocalUpdate(bool isGroundedHand, bool isGroundedButt, bool isGripPressed, bool isReadyForGrabbing)
	{
		if (isGripPressed && !this.wasGripPressed)
		{
			this.gripPressedAtTimestamp = Time.time;
		}
		this.wasGripPressed = isGripPressed;
		this.isReadyForGrabbing = (isReadyForGrabbing && Time.time >= this.rejectGrabsUntilTimestamp);
		this.isGroundedHand = isGroundedHand;
		this.isGroundedButt = isGroundedButt;
		if (this.grabbedLink != null)
		{
			if (!this.grabbedLink.isReadyForGrabbing && this.grabbedLink.grabbedPlayer != NetworkSystem.Instance.LocalPlayer)
			{
				this.BreakLink();
				return;
			}
			if ((!this.IsTentacleGrab && !isGripPressed) || !this.grabbedLink.myRig.gameObject.activeSelf)
			{
				this.BreakLink();
				return;
			}
			GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
			if (gorillaGuardianManager != null && gorillaGuardianManager.IsPlayerGuardian(this.grabbedPlayer))
			{
				this.BreakLink();
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.grabbedLink.myRig);
			GRPlayer grplayer2 = GRPlayer.Get(NetworkSystem.Instance.LocalPlayer);
			if (grplayer2 != null && grplayer != null && grplayer2.State == GRPlayer.GRPlayerState.Ghost != (grplayer.State == GRPlayer.GRPlayerState.Ghost))
			{
				this.BreakLink();
				return;
			}
			if (GorillaComputer.instance.IsPlayerInVirtualStump() && CustomMapManager.WantsHoldingHandsDisabled())
			{
				this.BreakLink();
				return;
			}
		}
	}

	public void RejectGrabsFor(float duration)
	{
		this.rejectGrabsUntilTimestamp = Mathf.Max(this.rejectGrabsUntilTimestamp, Time.time + duration);
	}

	public void Write(out bool isGroundedHand, out bool isGroundedButt, out int grabbedPlayerActorNumber, out bool grabbedHandIsLeft)
	{
		isGroundedHand = this.isGroundedHand;
		isGroundedButt = this.isGroundedButt;
		if (this.grabbedPlayer != null)
		{
			grabbedPlayerActorNumber = this.grabbedPlayer.ActorNumber;
			grabbedHandIsLeft = this.grabbedHandIsLeft;
			return;
		}
		grabbedPlayerActorNumber = 0;
		grabbedHandIsLeft = false;
	}

	public void Read(Vector3 remoteHandLocalPos, Quaternion remoteBodyWorldRot, Vector3 remoteBodyWorldPos, bool isGroundedHand, bool isGroundedButt, bool isReadyForGrabbing, bool isTentacleGrab, int grabbedPlayerActorNumber, bool grabbedHandIsLeft)
	{
		this.isGroundedHand = isGroundedHand;
		this.isGroundedButt = isGroundedButt;
		this.isReadyForGrabbing = isReadyForGrabbing;
		if (grabbedPlayerActorNumber == 0)
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsLocal)
			{
				(grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).BreakLink();
			}
			bool flag = this.grabbedPlayer != null;
			this.grabbedPlayer = null;
			this.grabbedLink = null;
			if (flag)
			{
				Action onHandLinkChanged = TakeMyHand_HandLink.OnHandLinkChanged;
				if (onHandLinkChanged != null)
				{
					onHandLinkChanged();
				}
			}
		}
		else if (this.lastReadGrabbedPlayerActorNumber == grabbedPlayerActorNumber)
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsValid && this.grabbedPlayer.ActorNumber == grabbedPlayerActorNumber && this.grabbedPlayer.IsLocal && !this.IsLocalGrabInRange(grabbedHandIsLeft, remoteHandLocalPos, remoteBodyWorldRot, remoteBodyWorldPos, 7f))
			{
				if (this.grabbedHandIsLeft)
				{
					VRRig.LocalRig.leftHandLink.BreakLink();
				}
				else
				{
					VRRig.LocalRig.rightHandLink.BreakLink();
				}
			}
		}
		else
		{
			if (this.grabbedPlayer != null && this.grabbedPlayer.IsLocal)
			{
				VRRig.LocalRig.leftHandLink.BreakLinkTo(this);
				VRRig.LocalRig.rightHandLink.BreakLinkTo(this);
			}
			NetPlayer player = NetworkSystem.Instance.GetPlayer(grabbedPlayerActorNumber);
			if (player != null)
			{
				bool flag2 = true;
				if (player.IsLocal && !isTentacleGrab && !(grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).IsTentacleGrab)
				{
					flag2 = this.IsLocalGrabInRange(grabbedHandIsLeft, remoteHandLocalPos, remoteBodyWorldRot, remoteBodyWorldPos, 0.25f);
				}
				if (!flag2)
				{
					(grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).RejectGrabsFor(0.5f);
					bool flag3 = this.grabbedPlayer != null;
					this.grabbedPlayer = null;
					this.grabbedLink = null;
					if (flag3)
					{
						Action onHandLinkChanged2 = TakeMyHand_HandLink.OnHandLinkChanged;
						if (onHandLinkChanged2 != null)
						{
							onHandLinkChanged2();
						}
					}
				}
				else if (player == this.myRig.OwningNetPlayer)
				{
					bool flag4 = this.grabbedPlayer != null;
					this.grabbedPlayer = null;
					this.grabbedLink = null;
					if (flag4)
					{
						Action onHandLinkChanged3 = TakeMyHand_HandLink.OnHandLinkChanged;
						if (onHandLinkChanged3 != null)
						{
							onHandLinkChanged3();
						}
					}
				}
				else
				{
					this.grabbedPlayer = player;
					this.grabbedHandIsLeft = grabbedHandIsLeft;
					this.IsTentacleGrab = isTentacleGrab;
					this.CheckFormLinkWithRemoteGrab();
					Action onHandLinkChanged4 = TakeMyHand_HandLink.OnHandLinkChanged;
					if (onHandLinkChanged4 != null)
					{
						onHandLinkChanged4();
					}
				}
			}
			else
			{
				bool flag5 = this.grabbedPlayer != null;
				this.grabbedPlayer = null;
				this.grabbedLink = null;
				if (flag5)
				{
					Action onHandLinkChanged5 = TakeMyHand_HandLink.OnHandLinkChanged;
					if (onHandLinkChanged5 != null)
					{
						onHandLinkChanged5();
					}
				}
			}
		}
		this.lastReadGrabbedPlayerActorNumber = grabbedPlayerActorNumber;
	}

	private bool IsLocalGrabInRange(bool grabbedLeftHand, Vector3 handLocalPos, Quaternion bodyWorldRot, Vector3 bodyWorldPos, float tolerance)
	{
		return ((grabbedLeftHand ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).transform.position - (bodyWorldPos + bodyWorldRot * handLocalPos)).IsShorterThan(tolerance);
	}

	private void CheckFormLinkWithRemoteGrab()
	{
		if (this.grabbedPlayer != NetworkSystem.Instance.LocalPlayer)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.grabbedPlayer, out rigContainer))
			{
				TakeMyHand_HandLink takeMyHand_HandLink = this.grabbedHandIsLeft ? rigContainer.Rig.leftHandLink : rigContainer.Rig.rightHandLink;
				if (takeMyHand_HandLink.grabbedPlayer == this.myRig.creator)
				{
					this.grabbedLink = takeMyHand_HandLink;
					this.grabbedLink.grabbedLink = this;
				}
			}
			return;
		}
		TakeMyHand_HandLink takeMyHand_HandLink2 = this.grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
		if (takeMyHand_HandLink2.isReadyForGrabbing)
		{
			takeMyHand_HandLink2.LocalCreateLink(this);
			return;
		}
		takeMyHand_HandLink2.RejectGrabsFor(0.5f);
	}

	public HandLinkAuthorityStatus GetChainAuthority(out int stepsToAuth)
	{
		TakeMyHand_HandLink takeMyHand_HandLink = this.grabbedLink;
		int num = 1;
		HandLinkAuthorityStatus handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.None, -1f, -1);
		stepsToAuth = -1;
		while (takeMyHand_HandLink != null && num < 10 && !takeMyHand_HandLink.IsLocal)
		{
			if (takeMyHand_HandLink.isGroundedHand)
			{
				stepsToAuth = num;
				return new HandLinkAuthorityStatus(HandLinkAuthorityType.HandGrounded, -1f, -1);
			}
			if (handLinkAuthorityStatus.type < HandLinkAuthorityType.ResidualHandGrounded && (double)(takeMyHand_HandLink.myRig.LastHandTouchedGroundAtNetworkTime + 1f) > PhotonNetwork.Time)
			{
				stepsToAuth = num;
				handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.ResidualHandGrounded, takeMyHand_HandLink.myRig.LastHandTouchedGroundAtNetworkTime, takeMyHand_HandLink.myRig.OwningNetPlayer.ActorNumber);
			}
			else if (handLinkAuthorityStatus.type < HandLinkAuthorityType.ButtGrounded && takeMyHand_HandLink.isGroundedButt)
			{
				stepsToAuth = num;
				handLinkAuthorityStatus = new HandLinkAuthorityStatus(HandLinkAuthorityType.ButtGrounded, -1f, -1);
			}
			else if (handLinkAuthorityStatus.type == HandLinkAuthorityType.None)
			{
				HandLinkAuthorityStatus handLinkAuthorityStatus2 = new HandLinkAuthorityStatus(HandLinkAuthorityType.None, takeMyHand_HandLink.myRig.LastTouchedGroundAtNetworkTime, takeMyHand_HandLink.myRig.OwningNetPlayer.ActorNumber);
				if (handLinkAuthorityStatus2 > handLinkAuthorityStatus)
				{
					stepsToAuth = num;
					handLinkAuthorityStatus = handLinkAuthorityStatus2;
				}
			}
			num++;
			takeMyHand_HandLink = takeMyHand_HandLink.myOtherHandLink.grabbedLink;
		}
		return handLinkAuthorityStatus;
	}

	public void VisuallySnapHandsTogether()
	{
		if (this.grabbedLink == null)
		{
			return;
		}
		if (this.IsTentacleGrab || this.grabbedLink.IsTentacleGrab)
		{
			return;
		}
		if (this.grabbedLink.snapPositionCalculatedAtFrame == Time.frameCount)
		{
			this.snapPositionCalculatedAtFrame = Time.frameCount;
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 position2 = this.grabbedLink.transform.position;
		Vector3 a = (position + position2) / 2f;
		Vector3 b = (this.isLeftHand ? this.myRig.leftHand.rigTarget : this.myRig.rightHand.rigTarget).position - position;
		Vector3 b2 = (this.grabbedLink.isLeftHand ? this.grabbedLink.myRig.leftHand.rigTarget : this.grabbedLink.myRig.rightHand.rigTarget).position - position2;
		Vector3 targetWorldPos = a + b;
		Vector3 targetWorldPos2 = a + b2;
		this.myIK.OverrideTargetPos(this.isLeftHand, targetWorldPos);
		this.grabbedLink.myIK.OverrideTargetPos(this.grabbedLink.isLeftHand, targetWorldPos2);
	}

	public void PlayVicariousTapHaptic()
	{
		GorillaTagger.Instance.StartVibration(this.isLeftHand, this.hapticStrengthOnVicariousTap, this.hapticDurationOnVicariousTap);
	}

	[FormerlySerializedAs("myPlayer")]
	[SerializeField]
	public VRRig myRig;

	[FormerlySerializedAs("leftHand")]
	[SerializeField]
	private bool isLeftHand;

	[SerializeField]
	public GorillaIK myIK;

	private TakeMyHand_HandLink myOtherHandLink;

	private bool isReadyForGrabbing;

	public bool isGroundedHand;

	public bool isGroundedButt;

	private bool wasGripPressed;

	private float gripPressedAtTimestamp;

	private float rejectGrabsUntilTimestamp;

	public TakeMyHand_HandLink grabbedLink;

	public NetPlayer grabbedPlayer;

	public bool grabbedHandIsLeft;

	private const bool DEBUG_GRAB_ANYONE = false;

	[SerializeField]
	private float hapticStrengthOnGrab;

	[SerializeField]
	private float hapticDurationOnGrab;

	[SerializeField]
	private float hapticStrengthOnVicariousTap;

	[SerializeField]
	private float hapticDurationOnVicariousTap;

	[SerializeField]
	private AudioClip audioOnGrab;

	public InteractionPoint interactionPoint;

	public static Action OnHandLinkChanged;

	private int lastReadGrabbedPlayerActorNumber;

	private int snapPositionCalculatedAtFrame = -1;
}
