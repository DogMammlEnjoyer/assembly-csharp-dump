using System;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;

public class LckEntitlementsNetworked : MonoBehaviour
{
	public void Awake()
	{
		if (this.m_rigNetworkController.IsNull())
		{
			this.m_rigNetworkController = base.GetComponentInParent<VRRigSerializer>();
		}
		if (this.m_rigNetworkController.IsNull())
		{
			Debug.LogError("LCK: Unable to find VRRigSerializer for LckEntitlementsNetworked.");
			return;
		}
		InDelegateListProcessor<RigContainer, PhotonMessageInfoWrapped> succesfullSpawnEvent = this.m_rigNetworkController.SuccesfullSpawnEvent;
		if (succesfullSpawnEvent == null)
		{
			return;
		}
		InAction<RigContainer, PhotonMessageInfoWrapped> inAction = new InAction<RigContainer, PhotonMessageInfoWrapped>(this.OnSuccessfulSpawn);
		succesfullSpawnEvent.Add(inAction);
	}

	private void OnSuccessfulSpawn(in RigContainer rig, in PhotonMessageInfoWrapped info)
	{
		if (LckEntitlementsManager.Instance == null)
		{
			Debug.LogError("LCK: LckEntitlementsManager.Instance is not available in the scene!");
			return;
		}
		string userId = this.m_rigNetworkController.VRRig.OwningNetPlayer.UserId;
		if (userId.IsNullOrEmpty())
		{
			Debug.LogError("LCK: owningUserId is null on spawn. Cannot process entitlements.");
			return;
		}
		if (rig.Rig.isLocal)
		{
			LckEntitlementsManager.Instance.OnLocalPlayerSpawned(userId);
			return;
		}
		LckEntitlementsManager.Instance.OnRemotePlayerSpawned(userId);
	}

	private void OnDestroy()
	{
		if (this.m_rigNetworkController != null && this.m_rigNetworkController.SuccesfullSpawnEvent != null)
		{
			ListProcessor<InAction<RigContainer, PhotonMessageInfoWrapped>> succesfullSpawnEvent = this.m_rigNetworkController.SuccesfullSpawnEvent;
			InAction<RigContainer, PhotonMessageInfoWrapped> inAction = new InAction<RigContainer, PhotonMessageInfoWrapped>(this.OnSuccessfulSpawn);
			succesfullSpawnEvent.Remove(inAction);
		}
	}

	[SerializeField]
	private VRRigSerializer m_rigNetworkController;
}
