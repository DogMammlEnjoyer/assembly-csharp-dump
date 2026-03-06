using System;
using System.Collections.Generic;
using GorillaNetworking;
using GTMathUtil;
using Unity.Profiling;
using UnityEngine;

public class GorillaFriendCollider : MonoBehaviour, IGorillaSliceableSimple
{
	public void Awake()
	{
		this.thisCapsule = base.GetComponent<CapsuleCollider>();
		this.thisBox = base.GetComponent<BoxCollider>();
		if (!GorillaFriendCollider.updateAdded)
		{
			GorillaFriendCollider.updateAdded = true;
			VRRigCache.OnActiveRigsChanged += GorillaFriendCollider.UpdateActiveRigs;
			GorillaFriendCollider.UpdateActiveRigs();
		}
	}

	private static void UpdateActiveRigs()
	{
		VRRigCache.Instance.GetActiveRigs(GorillaFriendCollider.playerRigs);
	}

	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	private void AddUserID(in string userID)
	{
		if (this.playerIDsCurrentlyTouching.Contains(userID))
		{
			return;
		}
		this.playerIDsCurrentlyTouching.Add(userID);
	}

	public void SliceUpdate()
	{
		using (GorillaFriendCollider.profiler_SliceUpdate.Auto())
		{
			if (NetworkSystem.Instance.InRoom || this.runCheckWhileNotInRoom)
			{
				this.RefreshPlayersWithinBounds();
			}
		}
	}

	public void RefreshPlayersWithinBounds()
	{
		this.playerIDsCurrentlyTouching.Clear();
		for (int i = 0; i < GorillaFriendCollider.playerRigs.Count; i++)
		{
			float y = GorillaFriendCollider.playerRigs[i].bodyTransform.transform.position.y;
			bool flag = !this.applyCapsuleYLimits || (y >= this.capsuleColliderYLimits.x && y <= this.capsuleColliderYLimits.y);
			bool flag2 = (this.thisBox != null && WithinBounds.PointWithinBoxColliderBounds(GorillaFriendCollider.playerRigs[i].rigContainer.SpeakerHead.position, this.thisBox)) || (this.thisBox == null && this.thisCapsule != null && WithinBounds.PointWithinCapsuleColliderBounds(GorillaFriendCollider.playerRigs[i].rigContainer.SpeakerHead.position, this.thisCapsule));
			if (flag && flag2)
			{
				this.playerIDsCurrentlyTouching.Add(GorillaFriendCollider.playerRigs[i].isLocal ? NetworkSystem.Instance.LocalPlayer.UserId : GorillaFriendCollider.playerRigs[i].creator.UserId);
			}
		}
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.LocalPlayer != null && this.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId) && GorillaComputer.instance.friendJoinCollider != this)
		{
			GorillaComputer.instance.allowedMapsToJoin = this.myAllowedMapsToJoin;
			GorillaComputer.instance.friendJoinCollider = this;
			GorillaComputer.instance.UpdateScreen();
		}
	}

	public List<string> playerIDsCurrentlyTouching = new List<string>();

	private CapsuleCollider thisCapsule;

	private BoxCollider thisBox;

	[Tooltip("If using a capsule collider, the player position can be checked against these minimum and maximum Y limits (world position) to make it behave more like a cylinder check")]
	public bool applyCapsuleYLimits;

	[Tooltip("If the player's Y world position is lower than Limits.x or higher than Limits.y, they will not be considered \"Inside\" the friend collider")]
	public Vector2 capsuleColliderYLimits = Vector2.zero;

	public bool runCheckWhileNotInRoom;

	public string[] myAllowedMapsToJoin;

	private readonly Collider[] overlapColliders = new Collider[20];

	public bool manualRefreshOnly;

	private float _nextUpdateTime = -1f;

	private static List<VRRig> playerRigs = new List<VRRig>();

	private static bool updateAdded = false;

	private static readonly ProfilerMarker profiler_SliceUpdate = new ProfilerMarker("GT/FriendCollider.SliceUpdate");
}
