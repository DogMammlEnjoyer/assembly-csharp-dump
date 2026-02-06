using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class GRAbilitySummon : GRAbilityBase
{
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	protected override void OnStart()
	{
		this.lastAnimIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.animData.Count, this.lastAnimIndex);
		this.duration = this.animData[this.lastAnimIndex].duration;
		this.chargeTime = this.animData[this.lastAnimIndex].eventTime;
		this.PlayAnim(this.animData[this.lastAnimIndex].animName, 0.1f, this.animSpeed);
		this.state = GRAbilitySummon.State.Charge;
		this.summonSound.Play(this.audioSource);
		this.spawnedCount = 0;
		this.agent.SetStopped(true);
		this.agent.SetSpeed(1f);
		if (this.fxStartSummon != null)
		{
			this.fxStartSummon.SetActive(false);
			this.fxStartSummon.SetActive(true);
		}
	}

	protected override void OnStop()
	{
		this.lookAtTarget = null;
		this.agent.SetStopped(false);
	}

	public void SetLookAtTarget(Transform transform)
	{
		this.lookAtTarget = transform;
	}

	protected override void OnThink(float dt)
	{
		this.UpdateState(dt);
	}

	protected override void OnUpdateShared(float dt)
	{
		if (this.lookAtTarget != null)
		{
			GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.lookAtTarget, 360f);
		}
	}

	private void UpdateState(float dt)
	{
		double num = Time.timeAsDouble - this.startTime;
		switch (this.state)
		{
		case GRAbilitySummon.State.Charge:
			if (num > (double)this.chargeTime)
			{
				this.SetState(GRAbilitySummon.State.Spawn);
				return;
			}
			break;
		case GRAbilitySummon.State.Spawn:
			if (!this.spawned)
			{
				this.spawned = this.DoSpawn();
			}
			if (this.spawned && num > (double)this.duration)
			{
				this.SetState(GRAbilitySummon.State.Done);
				this.spawned = false;
			}
			break;
		case GRAbilitySummon.State.Done:
			break;
		default:
			return;
		}
	}

	private void SetState(GRAbilitySummon.State newState)
	{
		GRAbilitySummon.State state = this.state;
		this.state = newState;
		switch (newState)
		{
		default:
			return;
		}
	}

	private Vector3? GetSpawnLocation()
	{
		if (this.summonMarkers != null && this.summonMarkers.Count > 0)
		{
			int index = Random.Range(0, this.summonMarkers.Count);
			if (this.summonMarkers[index] != null)
			{
				return new Vector3?(this.summonMarkers[index].transform.position);
			}
		}
		Vector3 position = this.root.position;
		float num = Random.Range(-this.summonConeAngle / 2f, this.summonConeAngle / 2f);
		int i = 0;
		while (i < 5)
		{
			Vector3 a = Quaternion.Euler(0f, num, 0f) * this.root.forward;
			Vector3 vector = position + a * this.desiredSpawnDistance;
			NavMeshHit navMeshHit;
			if (!NavMesh.Raycast(position, vector, out navMeshHit, this.walkableArea))
			{
				goto IL_126;
			}
			if (navMeshHit.distance >= this.minSpawnDistance)
			{
				vector = navMeshHit.position + Vector3.up * this.spawnHeight;
				goto IL_126;
			}
			num += 15f;
			if (num > this.summonConeAngle / 2f)
			{
				this.summonConeAngle = -this.summonConeAngle / 2f;
			}
			IL_151:
			i++;
			continue;
			IL_126:
			RaycastHit raycastHit;
			if (!Physics.Raycast(vector, Vector3.down, out raycastHit) || raycastHit.collider.gameObject.GetComponent<GRHazardousMaterial>() == null)
			{
				return new Vector3?(vector);
			}
			goto IL_151;
		}
		return null;
	}

	public bool ForceSpawn()
	{
		return this.DoSpawn();
	}

	private bool DoSpawn()
	{
		Vector3? spawnLocation = this.GetSpawnLocation();
		if (spawnLocation != null)
		{
			if (this.entity.IsAuthority())
			{
				Quaternion identity = Quaternion.identity;
				GhostReactorManager.Get(this.entity).gameEntityManager.RequestCreateItem(this.entityPrefabToSpawn.name.GetStaticHash(), spawnLocation.Value, identity, 0L, this.entity.id);
				this.spawnedCount++;
			}
			if (this.audioSource != null)
			{
				this.audioSource.PlayOneShot(this.summonSpawnAudioClip);
			}
			if (this.fxOnSpawn != null)
			{
				this.fxOnSpawn.SetActive(false);
				this.fxOnSpawn.SetActive(true);
			}
			return true;
		}
		return false;
	}

	public override bool IsDone()
	{
		return this.state == GRAbilitySummon.State.Done;
	}

	public override bool IsCoolDownOver()
	{
		return base.IsCoolDownOver(this.coolDown);
	}

	public override float GetRange()
	{
		return this.range;
	}

	private int lastAnimIndex = -1;

	public GameEntity entityPrefabToSpawn;

	public List<AnimationData> animData;

	private float animSpeed = 1f;

	public float coolDown;

	public float range;

	public float chargeTime = 3f;

	public float duration = 3f;

	public float desiredSpawnDistance = 3f;

	public float minSpawnDistance = 1f;

	public float spawnHeight = 1f;

	public float summonConeAngle = 120f;

	private bool spawned;

	public AudioClip summonSpawnAudioClip;

	public GameObject fxStartSummon;

	public GameObject fxOnSpawn;

	public AbilitySound summonSound;

	private int spawnedCount;

	public Transform lookAtTarget;

	public List<GRAbilitySummon.SummonMarker> summonMarkers;

	private GRAbilitySummon.State state;

	[Serializable]
	public class SummonMarker
	{
		public Transform transform;
	}

	private enum State
	{
		Charge,
		Spawn,
		Done
	}
}
