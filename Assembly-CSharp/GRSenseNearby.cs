using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

[Serializable]
public class GRSenseNearby
{
	private bool BossEntityPresent
	{
		get
		{
			return GhostReactorManager.Get(this._entity).GetBossEntity() != null;
		}
	}

	public void Setup(Transform headTransform, GameEntity entity)
	{
		this.rigsNearby = new List<VRRig>();
		this.headTransform = headTransform;
		this._entity = entity;
	}

	public void OnHitByPlayer(int hitByActorId)
	{
		GRPlayer grplayer = GRPlayer.Get(hitByActorId);
		if (grplayer != null)
		{
			VRRig rig = grplayer.gamePlayer.rig;
			if (!this.rigsNearby.Contains(rig))
			{
				this.rigsNearby.Add(rig);
			}
		}
	}

	public void UpdateNearby(List<VRRig> allRigs, GRSenseLineOfSight senseLineOfSight)
	{
		Vector3 position = this.headTransform.position;
		Vector3 forward = this.headTransform.rotation * Vector3.forward;
		this.RemoveNotNearby(position);
		this.AddNearby(position, forward, allRigs);
		this.RemoveNoLineOfSight(position, senseLineOfSight);
	}

	public bool IsAnyoneNearby()
	{
		return !GhostReactorManager.AggroDisabled && this.rigsNearby != null && this.rigsNearby.Count > 0;
	}

	public bool IsAnyoneNearby(float range, bool ignoreBossEntity = false)
	{
		if (!ignoreBossEntity && this.BossEntityPresent && this.rigsNearby.Count > 0)
		{
			return true;
		}
		if (!this.IsAnyoneNearby())
		{
			return false;
		}
		Vector3 position = this.headTransform.position;
		float num = range * range;
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			if (!(this.rigsNearby[i] == null) && (GRSenseNearby.GetRigTestLocation(this.rigsNearby[i]) - position).sqrMagnitude <= num)
			{
				return true;
			}
		}
		return false;
	}

	public static Vector3 GetRigTestLocation(VRRig rig)
	{
		return rig.transform.position;
	}

	public void AddNearby(Vector3 position, Vector3 forward, List<VRRig> allRigs)
	{
		if (this.BossEntityPresent)
		{
			foreach (VRRig item in allRigs)
			{
				if (!this.rigsNearby.Contains(item))
				{
					this.rigsNearby.Add(item);
				}
			}
			return;
		}
		float num = this.range * this.range;
		float num2 = Mathf.Cos(this.fov * 0.017453292f);
		for (int i = 0; i < allRigs.Count; i++)
		{
			VRRig vrrig = allRigs[i];
			GRPlayer component = vrrig.GetComponent<GRPlayer>();
			if (component.State != GRPlayer.GRPlayerState.Ghost && !component.InStealthMode && !this.rigsNearby.Contains(vrrig))
			{
				Vector3 a = GRSenseNearby.GetRigTestLocation(vrrig) - position;
				float sqrMagnitude = a.sqrMagnitude;
				float num3 = this.hearingRange * this.hearingRange;
				if (sqrMagnitude >= num3)
				{
					if (sqrMagnitude >= num)
					{
						goto IL_116;
					}
					if (sqrMagnitude > 0f)
					{
						float d = Mathf.Sqrt(sqrMagnitude);
						if (Vector3.Dot(a / d, forward) < num2)
						{
							goto IL_116;
						}
					}
				}
				this.rigsNearby.Add(vrrig);
			}
			IL_116:;
		}
	}

	public void RemoveNotNearby(Vector3 position)
	{
		if (this.BossEntityPresent)
		{
			return;
		}
		float num = this.exitRange * this.exitRange;
		int i = 0;
		while (i < this.rigsNearby.Count)
		{
			VRRig vrrig = this.rigsNearby[i];
			if (!(vrrig != null))
			{
				goto IL_61;
			}
			GRPlayer component = vrrig.GetComponent<GRPlayer>();
			if ((GRSenseNearby.GetRigTestLocation(vrrig) - position).sqrMagnitude > num || component.State == GRPlayer.GRPlayerState.Ghost || component.InStealthMode)
			{
				goto IL_61;
			}
			IL_71:
			i++;
			continue;
			IL_61:
			this.rigsNearby.RemoveAt(i);
			i--;
			goto IL_71;
		}
	}

	public void RemoveNoLineOfSight(Vector3 headPos, GRSenseLineOfSight senseLineOfSight)
	{
		if (this.BossEntityPresent)
		{
			return;
		}
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			Vector3 rigTestLocation = GRSenseNearby.GetRigTestLocation(this.rigsNearby[i]);
			if (!senseLineOfSight.HasLineOfSight(headPos, rigTestLocation))
			{
				this.rigsNearby.RemoveAt(i);
				i--;
			}
		}
	}

	public VRRig PickClosest(out float outDistanceSq)
	{
		Vector3 position = this.headTransform.position;
		float num = float.MaxValue;
		VRRig result = null;
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			float sqrMagnitude = (GRSenseNearby.GetRigTestLocation(this.rigsNearby[i]) - position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = this.rigsNearby[i];
			}
		}
		outDistanceSq = num;
		return result;
	}

	public float range;

	public float hearingRange;

	public float exitRange;

	public float fov;

	[ReadOnly]
	public List<VRRig> rigsNearby;

	private Transform headTransform;

	private GameEntity _entity;
}
