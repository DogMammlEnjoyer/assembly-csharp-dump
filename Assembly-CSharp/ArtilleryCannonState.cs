using System;
using System.Runtime.InteropServices;
using Fusion;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(XSceneRefTarget))]
[NetworkBehaviourWeaved(8)]
public class ArtilleryCannonState : NetworkComponent
{
	internal event Action onRotationChanged;

	internal event Action onFired;

	internal float CurrentPitch
	{
		get
		{
			return this.currentPitch;
		}
	}

	internal float CurrentYaw
	{
		get
		{
			return this.currentYaw;
		}
	}

	internal float PitchMin
	{
		get
		{
			return this.pitchMin;
		}
	}

	internal float PitchMax
	{
		get
		{
			return this.pitchMax;
		}
	}

	internal float DegreesPerCrankDegree
	{
		get
		{
			return this.degreesPerCrankDegree;
		}
	}

	private int LocalActorNr
	{
		get
		{
			if (PhotonNetwork.LocalPlayer == null)
			{
				return -1;
			}
			return PhotonNetwork.LocalPlayer.ActorNumber;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this.pitchCrankSync.holderActorNr = -1;
		this.yawCrankSync.holderActorNr = -1;
	}

	internal void UpdateLocalCrankState(int crankIndex, bool isLeftHand, float angle)
	{
		ref ArtilleryCannonState.CrankSyncState ptr = ref (crankIndex == 0) ? ref this.pitchCrankSync : ref this.yawCrankSync;
		int localActorNr = this.LocalActorNr;
		if (ptr.holderActorNr == localActorNr)
		{
			ptr.isLeftHand = isLeftHand;
			ptr.angle = angle;
		}
	}

	internal static VRRig FindRigForActor(int actorNr)
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(actorNr, out rigContainer))
		{
			return rigContainer.Rig;
		}
		return null;
	}

	internal unsafe bool NotifyCrankGrabbed(int crankIndex, bool isLeftHand)
	{
		ref ArtilleryCannonState.CrankSyncState ptr = ref (crankIndex == 0) ? ref this.pitchCrankSync : ref this.yawCrankSync;
		if (ptr.holderActorNr != -1)
		{
			return false;
		}
		ptr.holderActorNr = this.LocalActorNr;
		*(ref (crankIndex == 0) ? ref this.pitchPendingGrabTime : ref this.yawPendingGrabTime) = Time.time;
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("RPC_ArtilleryMessage", RpcTarget.MasterClient, new object[]
			{
				isLeftHand ? 0 : 1,
				(byte)crankIndex,
				0f
			});
		}
		return true;
	}

	internal unsafe void NotifyCrankReleased(int crankIndex, float finalAngle)
	{
		ref ArtilleryCannonState.CrankSyncState ptr = ref (crankIndex == 0) ? ref this.pitchCrankSync : ref this.yawCrankSync;
		ptr.holderActorNr = -1;
		ptr.angle = finalAngle;
		*(ref (crankIndex == 0) ? ref this.pitchPendingGrabTime : ref this.yawPendingGrabTime) = 0f;
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("RPC_ArtilleryMessage", RpcTarget.All, new object[]
			{
				2,
				(byte)crankIndex,
				finalAngle
			});
		}
	}

	internal void NotifyCrankInput(int crankIndex, float degrees)
	{
		if (crankIndex == 0)
		{
			this.currentPitch = Mathf.Clamp(this.currentPitch + degrees * this.degreesPerCrankDegree, this.pitchMin, this.pitchMax);
		}
		else
		{
			this.currentYaw += degrees * this.degreesPerCrankDegree;
		}
		Action action = this.onRotationChanged;
		if (action != null)
		{
			action();
		}
		if (PhotonNetwork.InRoom)
		{
			float num = (crankIndex == 0) ? this.currentPitch : this.currentYaw;
			base.SendRPC("RPC_ArtilleryMessage", RpcTarget.MasterClient, new object[]
			{
				3,
				(byte)crankIndex,
				num
			});
		}
	}

	internal bool TryFire()
	{
		if (Time.time < this.lastFireTime + this.fireCooldown)
		{
			return false;
		}
		this.lastFireTime = Time.time;
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("RPC_ArtilleryMessage", RpcTarget.Others, new object[]
			{
				4,
				0,
				0f
			});
		}
		return true;
	}

	[PunRPC]
	public void RPC_ArtilleryMessage(byte msgType, byte crankIndex, float floatParam, PhotonMessageInfo info)
	{
		switch (msgType)
		{
		case 0:
		case 1:
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			ref ArtilleryCannonState.CrankSyncState ptr = ref (crankIndex == 0) ? ref this.pitchCrankSync : ref this.yawCrankSync;
			if (ptr.holderActorNr == -1)
			{
				ptr.holderActorNr = info.Sender.ActorNumber;
				ptr.isLeftHand = (msgType == 0);
				return;
			}
			break;
		}
		case 2:
		{
			ref ArtilleryCannonState.CrankSyncState ptr2 = ref (crankIndex == 0) ? ref this.pitchCrankSync : ref this.yawCrankSync;
			if (ptr2.holderActorNr == info.Sender.ActorNumber)
			{
				ptr2.holderActorNr = -1;
				ptr2.angle = floatParam;
				return;
			}
			break;
		}
		case 3:
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			if ((ref (crankIndex == 0) ? ref this.pitchCrankSync : ref this.yawCrankSync).holderActorNr != info.Sender.ActorNumber)
			{
				return;
			}
			if (crankIndex == 0)
			{
				this.currentPitch = Mathf.Clamp(floatParam, this.pitchMin, this.pitchMax);
			}
			else
			{
				this.currentYaw = floatParam;
			}
			Action action = this.onRotationChanged;
			if (action == null)
			{
				return;
			}
			action();
			return;
		}
		case 4:
		{
			int actorNumber = info.Sender.ActorNumber;
			if (this.pitchCrankSync.holderActorNr != actorNumber && this.yawCrankSync.holderActorNr != actorNumber)
			{
				return;
			}
			Action action2 = this.onFired;
			if (action2 == null)
			{
				return;
			}
			action2();
			break;
		}
		default:
			return;
		}
	}

	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.currentPitch);
		stream.SendNext(this.currentYaw);
		stream.SendNext(this.pitchCrankSync.holderActorNr);
		stream.SendNext(this.pitchCrankSync.isLeftHand);
		stream.SendNext(this.pitchCrankSync.angle);
		stream.SendNext(this.yawCrankSync.holderActorNr);
		stream.SendNext(this.yawCrankSync.isLeftHand);
		stream.SendNext(this.yawCrankSync.angle);
	}

	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		float num = (float)stream.ReceiveNext();
		float num2 = (float)stream.ReceiveNext();
		int localActorNr = this.LocalActorNr;
		this.ReadCrankSyncPUN(stream, ref this.pitchCrankSync, ref this.pitchPendingGrabTime, localActorNr);
		this.ReadCrankSyncPUN(stream, ref this.yawCrankSync, ref this.yawPendingGrabTime, localActorNr);
		if (this.pitchCrankSync.holderActorNr != localActorNr)
		{
			this.currentPitch = num;
		}
		if (this.yawCrankSync.holderActorNr != localActorNr)
		{
			this.currentYaw = num2;
		}
		Action action = this.onRotationChanged;
		if (action == null)
		{
			return;
		}
		action();
	}

	private void ReadCrankSyncPUN(PhotonStream stream, ref ArtilleryCannonState.CrankSyncState crank, ref float pendingTime, int localActor)
	{
		int num = (int)stream.ReceiveNext();
		bool isLeftHand = (bool)stream.ReceiveNext();
		float angle = (float)stream.ReceiveNext();
		if (pendingTime > 0f && crank.holderActorNr == localActor)
		{
			if (num == localActor)
			{
				pendingTime = 0f;
			}
			else if (num != -1)
			{
				pendingTime = 0f;
			}
			else
			{
				if (Time.time - pendingTime <= 1f)
				{
					return;
				}
				pendingTime = 0f;
			}
		}
		crank.holderActorNr = num;
		crank.isLeftHand = isLeftHand;
		crank.angle = angle;
	}

	[Networked]
	[NetworkedWeaved(0, 8)]
	private unsafe ArtilleryCannonState.FusionSyncState FusionData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ArtilleryCannonState.FusionData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(ArtilleryCannonState.FusionSyncState*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ArtilleryCannonState.FusionData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(ArtilleryCannonState.FusionSyncState*)(this.Ptr + 0) = value;
		}
	}

	public override void WriteDataFusion()
	{
		this.FusionData = new ArtilleryCannonState.FusionSyncState
		{
			pitch = this.currentPitch,
			yaw = this.currentYaw,
			pitchHolderActorNr = this.pitchCrankSync.holderActorNr,
			pitchIsLeftHand = this.pitchCrankSync.isLeftHand,
			pitchCrankAngle = this.pitchCrankSync.angle,
			yawHolderActorNr = this.yawCrankSync.holderActorNr,
			yawIsLeftHand = this.yawCrankSync.isLeftHand,
			yawCrankAngle = this.yawCrankSync.angle
		};
	}

	public override void ReadDataFusion()
	{
		ArtilleryCannonState.FusionSyncState fusionData = this.FusionData;
		int localActorNr = this.LocalActorNr;
		this.ReadCrankSyncFusion(ref this.pitchCrankSync, ref this.pitchPendingGrabTime, localActorNr, fusionData.pitchHolderActorNr, fusionData.pitchIsLeftHand, fusionData.pitchCrankAngle);
		this.ReadCrankSyncFusion(ref this.yawCrankSync, ref this.yawPendingGrabTime, localActorNr, fusionData.yawHolderActorNr, fusionData.yawIsLeftHand, fusionData.yawCrankAngle);
		if (this.pitchCrankSync.holderActorNr != localActorNr)
		{
			this.currentPitch = fusionData.pitch;
		}
		if (this.yawCrankSync.holderActorNr != localActorNr)
		{
			this.currentYaw = fusionData.yaw;
		}
		Action action = this.onRotationChanged;
		if (action == null)
		{
			return;
		}
		action();
	}

	private void ReadCrankSyncFusion(ref ArtilleryCannonState.CrankSyncState crank, ref float pendingTime, int localActor, int incomingHolder, bool incomingLeftHand, float incomingAngle)
	{
		if (pendingTime > 0f && crank.holderActorNr == localActor)
		{
			if (incomingHolder == localActor)
			{
				pendingTime = 0f;
			}
			else if (incomingHolder != -1)
			{
				pendingTime = 0f;
			}
			else
			{
				if (Time.time - pendingTime <= 1f)
				{
					return;
				}
				pendingTime = 0f;
			}
		}
		crank.holderActorNr = incomingHolder;
		crank.isLeftHand = incomingLeftHand;
		crank.angle = incomingAngle;
	}

	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.FusionData = this._FusionData;
	}

	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._FusionData = this.FusionData;
	}

	internal const int CRANK_PITCH = 0;

	internal const int CRANK_YAW = 1;

	[Header("Rotation Limits")]
	[SerializeField]
	private float pitchMin = -10f;

	[SerializeField]
	private float pitchMax = 60f;

	[Tooltip("How many degrees the cannon rotates per degree of crank rotation")]
	[SerializeField]
	private float degreesPerCrankDegree = 0.5f;

	[Header("Firing")]
	[SerializeField]
	private float fireCooldown = 2f;

	private float currentPitch;

	private float currentYaw;

	private float lastFireTime;

	internal ArtilleryCannonState.CrankSyncState pitchCrankSync;

	internal ArtilleryCannonState.CrankSyncState yawCrankSync;

	private const float GRAB_GRACE_PERIOD = 1f;

	private float pitchPendingGrabTime;

	private float yawPendingGrabTime;

	[WeaverGenerated]
	[DefaultForProperty("FusionData", 0, 8)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private ArtilleryCannonState.FusionSyncState _FusionData;

	internal struct CrankSyncState
	{
		public int holderActorNr;

		public bool isLeftHand;

		public float angle;
	}

	private enum ArtilleryMsg : byte
	{
		CrankGrabLeft,
		CrankGrabRight,
		CrankRelease,
		CrankInput,
		Fire
	}

	[NetworkStructWeaved(8)]
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	private struct FusionSyncState : INetworkStruct
	{
		[FieldOffset(0)]
		public float pitch;

		[FieldOffset(4)]
		public float yaw;

		[FieldOffset(8)]
		public int pitchHolderActorNr;

		[FieldOffset(12)]
		public NetworkBool pitchIsLeftHand;

		[FieldOffset(16)]
		public float pitchCrankAngle;

		[FieldOffset(20)]
		public int yawHolderActorNr;

		[FieldOffset(24)]
		public NetworkBool yawIsLeftHand;

		[FieldOffset(28)]
		public float yawCrankAngle;
	}
}
