using System;
using System.Collections.Generic;
using UnityEngine;

public class GRBossEyeStare : MonoBehaviour, IGorillaSliceableSimple
{
	private void Awake()
	{
		this.boss = base.GetComponentInParent<GREnemyBossMoon>();
	}

	private void OnEnable()
	{
		this.lastLocalRot = base.transform.localEulerAngles;
		GorillaSlicerSimpleManager.RegisterSliceable(this);
	}

	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this);
	}

	public void SliceUpdate()
	{
		if (this.boss.CurrAbility != this.lastAbility)
		{
			this.lastLocalRot = base.transform.localEulerAngles;
		}
		if (this.noUpdateAbilities.Contains(this.boss.CurrAbility))
		{
			this.lastLocalRot = base.transform.localEulerAngles;
			this.lastAbility = this.boss.CurrAbility;
			return;
		}
		if (base.transform.localEulerAngles != this.lastLocalRot)
		{
			this.lastLocalRot = base.transform.localEulerAngles;
			if (!this.noUpdateAbilities.Contains(this.boss.CurrAbility))
			{
				this.noUpdateAbilities.Add(this.boss.CurrAbility);
			}
			this.lastAbility = this.boss.CurrAbility;
			return;
		}
		if (this.closestPlayer == null || Time.time > this.lastCheck + this.checkForClosestPlayerCooldown)
		{
			VRRigCache.Instance.GetActiveRigs(this.rigs);
			float num = float.MaxValue;
			for (int i = 0; i < this.rigs.Count; i++)
			{
				float sqrMagnitude = (base.transform.position - this.rigs[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					this.closestPlayer = this.rigs[i].transform;
				}
			}
			this.lastCheck = Time.time;
		}
		this.lastAbility = this.boss.CurrAbility;
		if (this.closestPlayer == null)
		{
			return;
		}
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Vector3.up, (this.closestPlayer.position - base.transform.position).normalized) * Quaternion.Euler(this.rotOffset), this.lerpAmount);
		this.lastLocalRot = base.transform.localEulerAngles;
	}

	private Vector3 lastLocalRot;

	private List<GRAbilityBase> noUpdateAbilities = new List<GRAbilityBase>();

	private GREnemyBossMoon boss;

	private GRAbilityBase lastAbility;

	private float lastCheck;

	private float checkForClosestPlayerCooldown = 1f;

	private Transform closestPlayer;

	private List<VRRig> rigs = new List<VRRig>();

	public float lerpAmount = 0.3f;

	public Vector3 rotOffset;
}
