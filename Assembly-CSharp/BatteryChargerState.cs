using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(XSceneRefTarget))]
[NetworkBehaviourWeaved(62)]
public class BatteryChargerState : NetworkComponent
{
	internal event Action onChargeChanged;

	internal event Action onFullyCharged;

	internal event Action<int> onEventPhaseChanged;

	internal float CurrentCharge
	{
		get
		{
			return this.currentCharge;
		}
	}

	internal float MaxCharge
	{
		get
		{
			return this.maxCharge;
		}
	}

	internal float ChargePercent
	{
		get
		{
			if (this.maxCharge <= 0f)
			{
				return 0f;
			}
			return this.currentCharge / this.maxCharge;
		}
	}

	internal float ChargePerCrankDegree
	{
		get
		{
			return this.chargePerCrankDegree;
		}
	}

	internal int EventPhase
	{
		get
		{
			return this.eventPhase;
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

	public void SetChargePerCrankDegree(float chargeRate)
	{
		this.chargePerCrankDegree = chargeRate;
	}

	protected override void Awake()
	{
		base.Awake();
		for (int i = 0; i < 20; i++)
		{
			this.crankSyncs[i].holderActorNr = -1;
		}
	}

	private void Update()
	{
		if (this.activeCrankerCount <= 0 && this.currentCharge > 0f)
		{
			this.currentCharge = Mathf.Max(0f, this.currentCharge - this.drainPerSecond * Time.deltaTime);
			Action action = this.onChargeChanged;
			if (action != null)
			{
				action();
			}
		}
		this.activeCrankerCount = 0;
		for (int i = 0; i < 20; i++)
		{
			if (this.crankSyncs[i].holderActorNr != -1)
			{
				this.activeCrankerCount++;
			}
		}
	}

	internal void UpdateLocalCrankState(int crankIndex, bool isLeftHand, float angle)
	{
		if (crankIndex < 0 || crankIndex >= 20)
		{
			return;
		}
		ref BatteryChargerState.CrankSyncState ptr = ref this.crankSyncs[crankIndex];
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

	internal bool NotifyCrankGrabbed(int crankIndex, bool isLeftHand)
	{
		if (crankIndex < 0 || crankIndex >= 20)
		{
			return false;
		}
		ref BatteryChargerState.CrankSyncState ptr = ref this.crankSyncs[crankIndex];
		if (ptr.holderActorNr != -1)
		{
			return false;
		}
		ptr.holderActorNr = this.LocalActorNr;
		this.pendingGrabTime[crankIndex] = Time.time;
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("RPC_BatteryMessage", RpcTarget.MasterClient, new object[]
			{
				isLeftHand ? 0 : 1,
				(byte)crankIndex,
				0f
			});
		}
		return true;
	}

	internal void NotifyCrankReleased(int crankIndex, float finalAngle)
	{
		if (crankIndex < 0 || crankIndex >= 20)
		{
			return;
		}
		BatteryChargerState.CrankSyncState[] array = this.crankSyncs;
		array[crankIndex].holderActorNr = -1;
		array[crankIndex].angle = finalAngle;
		this.pendingGrabTime[crankIndex] = 0f;
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("RPC_BatteryMessage", RpcTarget.All, new object[]
			{
				2,
				(byte)crankIndex,
				finalAngle
			});
		}
	}

	internal void NotifyCrankInput(int crankIndex, float degrees)
	{
		if (crankIndex < 0 || crankIndex >= 20)
		{
			return;
		}
		float num = Mathf.Abs(degrees) * this.chargePerCrankDegree;
		float num2 = this.currentCharge;
		this.currentCharge = Mathf.Clamp(this.currentCharge + num, 0f, this.maxCharge);
		Action action = this.onChargeChanged;
		if (action != null)
		{
			action();
		}
		if (num2 < this.maxCharge && this.currentCharge >= this.maxCharge)
		{
			Action action2 = this.onFullyCharged;
			if (action2 != null)
			{
				action2();
			}
		}
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("RPC_BatteryMessage", RpcTarget.MasterClient, new object[]
			{
				3,
				(byte)crankIndex,
				this.currentCharge
			});
		}
	}

	public void DisableNetworking()
	{
		this.m_disableNetworking = true;
	}

	public void EnableNetworking()
	{
		this.m_disableNetworking = false;
	}

	[PunRPC]
	public void RPC_BatteryMessage(byte msgType, byte crankIndex, float floatParam, PhotonMessageInfo info)
	{
		if (this.m_disableNetworking)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "RPC_BatteryMessage");
		if (crankIndex >= 20)
		{
			return;
		}
		switch (msgType)
		{
		case 0:
		case 1:
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				return;
			}
			ref BatteryChargerState.CrankSyncState ptr = ref this.crankSyncs[(int)crankIndex];
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
			ref BatteryChargerState.CrankSyncState ptr2 = ref this.crankSyncs[(int)crankIndex];
			if (ptr2.holderActorNr == info.Sender.ActorNumber)
			{
				ptr2.holderActorNr = -1;
				ptr2.angle = floatParam.ClampSafe(-360f, 360f);
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
			if (this.crankSyncs[(int)crankIndex].holderActorNr != info.Sender.ActorNumber)
			{
				return;
			}
			float num = this.currentCharge;
			this.currentCharge = floatParam.ClampSafe(0f, this.maxCharge);
			Action action = this.onChargeChanged;
			if (action != null)
			{
				action();
			}
			if (num < this.maxCharge && this.currentCharge >= this.maxCharge)
			{
				Action action2 = this.onFullyCharged;
				if (action2 == null)
				{
					return;
				}
				action2();
			}
			break;
		}
		default:
			return;
		}
	}

	public void SetEventPhase(int phase)
	{
		if (!PhotonNetwork.IsMasterClient || phase == this.eventPhase)
		{
			return;
		}
		this.eventPhase = phase;
		Action<int> action = this.onEventPhaseChanged;
		if (action == null)
		{
			return;
		}
		action(phase);
	}

	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.m_disableNetworking)
		{
			return;
		}
		stream.SendNext(this.currentCharge);
		stream.SendNext(this.eventPhase);
		for (int i = 0; i < 20; i++)
		{
			stream.SendNext(this.crankSyncs[i].holderActorNr);
			stream.SendNext(this.crankSyncs[i].isLeftHand);
			stream.SendNext(this.crankSyncs[i].angle);
		}
	}

	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient || this.m_disableNetworking)
		{
			return;
		}
		float num = (float)stream.ReceiveNext();
		int num2 = (int)stream.ReceiveNext();
		int localActorNr = this.LocalActorNr;
		int i = 0;
		while (i < 20)
		{
			int num3 = (int)stream.ReceiveNext();
			bool isLeftHand = (bool)stream.ReceiveNext();
			float angle = (float)stream.ReceiveNext();
			if (this.pendingGrabTime[i] <= 0f || this.crankSyncs[i].holderActorNr != localActorNr)
			{
				goto IL_D7;
			}
			if (num3 == localActorNr)
			{
				this.pendingGrabTime[i] = 0f;
				goto IL_D7;
			}
			if (num3 != -1)
			{
				this.pendingGrabTime[i] = 0f;
				goto IL_D7;
			}
			if (Time.time - this.pendingGrabTime[i] > 1f)
			{
				this.pendingGrabTime[i] = 0f;
				goto IL_D7;
			}
			IL_113:
			i++;
			continue;
			IL_D7:
			this.crankSyncs[i].holderActorNr = num3;
			this.crankSyncs[i].isLeftHand = isLeftHand;
			this.crankSyncs[i].angle = angle;
			goto IL_113;
		}
		bool flag = false;
		for (int j = 0; j < 20; j++)
		{
			if (this.crankSyncs[j].holderActorNr == localActorNr)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.currentCharge = num;
		}
		Action action = this.onChargeChanged;
		if (action != null)
		{
			action();
		}
		if (num2 != this.eventPhase)
		{
			this.eventPhase = num2;
			Action<int> action2 = this.onEventPhaseChanged;
			if (action2 == null)
			{
				return;
			}
			action2(this.eventPhase);
		}
	}

	[Networked]
	[NetworkedWeaved(0, 2)]
	private unsafe BatteryChargerState.FusionSyncState FusionData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BatteryChargerState.FusionData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(BatteryChargerState.FusionSyncState*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BatteryChargerState.FusionData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(BatteryChargerState.FusionSyncState*)(this.Ptr + 0) = value;
		}
	}

	[Networked]
	[Capacity(20)]
	[NetworkedWeaved(2, 60)]
	[NetworkedWeavedArray(20, 3, typeof(ReaderWriter@BatteryChargerState__FusionCrankData))]
	private unsafe NetworkArray<BatteryChargerState.FusionCrankData> FusionCranks
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BatteryChargerState.FusionCranks. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<BatteryChargerState.FusionCrankData>((byte*)(this.Ptr + 2), 20, ReaderWriter@BatteryChargerState__FusionCrankData.GetInstance());
		}
	}

	public override void WriteDataFusion()
	{
		this.FusionData = new BatteryChargerState.FusionSyncState
		{
			charge = this.currentCharge,
			eventPhase = this.eventPhase
		};
		for (int i = 0; i < 20; i++)
		{
			this.FusionCranks.Set(i, new BatteryChargerState.FusionCrankData
			{
				holderActorNr = this.crankSyncs[i].holderActorNr,
				isLeftHand = this.crankSyncs[i].isLeftHand,
				angle = this.crankSyncs[i].angle
			});
		}
	}

	public override void ReadDataFusion()
	{
		BatteryChargerState.FusionSyncState fusionData = this.FusionData;
		int localActorNr = this.LocalActorNr;
		int i = 0;
		while (i < 20)
		{
			BatteryChargerState.FusionCrankData fusionCrankData = this.FusionCranks[i];
			int holderActorNr = fusionCrankData.holderActorNr;
			if (this.pendingGrabTime[i] <= 0f || this.crankSyncs[i].holderActorNr != localActorNr)
			{
				goto IL_9D;
			}
			if (holderActorNr == localActorNr)
			{
				this.pendingGrabTime[i] = 0f;
				goto IL_9D;
			}
			if (holderActorNr != -1)
			{
				this.pendingGrabTime[i] = 0f;
				goto IL_9D;
			}
			if (Time.time - this.pendingGrabTime[i] > 1f)
			{
				this.pendingGrabTime[i] = 0f;
				goto IL_9D;
			}
			IL_E5:
			i++;
			continue;
			IL_9D:
			this.crankSyncs[i].holderActorNr = holderActorNr;
			this.crankSyncs[i].isLeftHand = fusionCrankData.isLeftHand;
			this.crankSyncs[i].angle = fusionCrankData.angle;
			goto IL_E5;
		}
		bool flag = false;
		for (int j = 0; j < 20; j++)
		{
			if (this.crankSyncs[j].holderActorNr == localActorNr)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.currentCharge = fusionData.charge;
		}
		Action action = this.onChargeChanged;
		if (action != null)
		{
			action();
		}
		if (fusionData.eventPhase != this.eventPhase)
		{
			this.eventPhase = fusionData.eventPhase;
			Action<int> action2 = this.onEventPhaseChanged;
			if (action2 == null)
			{
				return;
			}
			action2(this.eventPhase);
		}
	}

	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.FusionData = this._FusionData;
		NetworkBehaviourUtils.InitializeNetworkArray<BatteryChargerState.FusionCrankData>(this.FusionCranks, this._FusionCranks, "FusionCranks");
	}

	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._FusionData = this.FusionData;
		NetworkBehaviourUtils.CopyFromNetworkArray<BatteryChargerState.FusionCrankData>(this.FusionCranks, ref this._FusionCranks);
	}

	internal const int MAX_CRANKS = 20;

	[Header("Charging")]
	[Tooltip("Charge added per degree of crank rotation (across all cranks)")]
	[SerializeField]
	private float chargePerCrankDegree = 0.001f;

	[Tooltip("Charge drains at this rate per second when no one is cranking")]
	[SerializeField]
	private float drainPerSecond = 0.02f;

	[Tooltip("Maximum charge level (0 to 1)")]
	[SerializeField]
	private float maxCharge = 1f;

	private float currentCharge;

	private int activeCrankerCount;

	private int eventPhase = -1;

	internal BatteryChargerState.CrankSyncState[] crankSyncs = new BatteryChargerState.CrankSyncState[20];

	private const float GRAB_GRACE_PERIOD = 1f;

	private float[] pendingGrabTime = new float[20];

	private bool m_disableNetworking;

	[WeaverGenerated]
	[DefaultForProperty("FusionData", 0, 2)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private BatteryChargerState.FusionSyncState _FusionData;

	[WeaverGenerated]
	[DefaultForProperty("FusionCranks", 2, 60)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private BatteryChargerState.FusionCrankData[] _FusionCranks;

	internal struct CrankSyncState
	{
		public int holderActorNr;

		public bool isLeftHand;

		public float angle;
	}

	private enum BatteryMsg : byte
	{
		CrankGrabLeft,
		CrankGrabRight,
		CrankRelease,
		CrankInput
	}

	[NetworkStructWeaved(3)]
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	private struct FusionCrankData : INetworkStruct
	{
		[FieldOffset(0)]
		public int holderActorNr;

		[FieldOffset(4)]
		public NetworkBool isLeftHand;

		[FieldOffset(8)]
		public float angle;
	}

	[NetworkStructWeaved(2)]
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	private struct FusionSyncState : INetworkStruct
	{
		[FieldOffset(0)]
		public float charge;

		[FieldOffset(4)]
		public int eventPhase;
	}
}
