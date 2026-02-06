using System;
using System.Collections.Generic;
using GorillaTagScripts.GhostReactor;
using UnityEngine;

[Serializable]
public class GRAbilityDie : GRAbilityBase
{
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		if (this.disableAllCollidersWhenDead)
		{
			agent.GetComponentsInChildren<Collider>(this.disableCollidersWhenDead);
		}
		if (this.disableAllRenderersWhenDead)
		{
			agent.GetComponentsInChildren<Renderer>(this.hideWhenDead);
		}
		GRAbilityDie.Disable(this.disableCollidersWhenDead, false);
		this.staggerMovement.Setup(root);
	}

	protected override void OnStart()
	{
		this.totalDeathDelay = this.delayDeath;
		if (this.animData.Count > 0)
		{
			int index = Random.Range(0, this.animData.Count);
			this.totalDeathDelay += this.animData[index].duration;
			this.staggerMovement.InitFromVelocityAndDuration(this.staggerMovement.velocity, this.totalDeathDelay);
			this.PlayAnim(this.animData[index].animName, 0.1f, this.animData[index].speed);
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.isDead = false;
		if (this.doKnockback)
		{
			this.staggerMovement.Start();
		}
		this.soundDeath.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundOnHide.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundDeath.Play(null);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, true);
		if (this.fxDeath != null)
		{
			this.fxDeath.SetActive(false);
		}
		this.events.Reset();
		this.events.OnAbilityStart(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	protected override void OnStop()
	{
		this.staggerMovement.Stop();
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		GRAbilityDie.Hide(this.hideWhenDead, false);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, false);
		this.events.OnAbilityStop(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	public void SetStaggerVelocity(Vector3 vel)
	{
		float magnitude = vel.magnitude;
		if (magnitude > 0f)
		{
			Vector3 a = vel / magnitude;
			a.y = 0f;
			vel = a * magnitude;
		}
		this.staggerMovement.InitFromVelocityAndDuration(vel, this.totalDeathDelay);
	}

	public void SetInstigatingPlayerIndex(int actorNumber)
	{
		Debug.Log(string.Format("SetInstigatingPlayerIndex {0}", actorNumber));
		this.instigatingActorNumber = actorNumber;
	}

	private void Die()
	{
		this.soundOnHide.Play(null);
		if (this.fxDeath != null)
		{
			this.fxDeath.SetActive(false);
			this.fxDeath.SetActive(true);
		}
		GRAbilityDie.Hide(this.hideWhenDead, true);
		GRAbilityDie.Disable(this.disableCollidersWhenDead, true);
		GameEntity entity = this.agent.entity;
		GameEntity gameEntity;
		if (this.lootTable != null && entity.IsAuthority() && this.lootTable.TryForRandomItem(entity, out gameEntity, 0))
		{
			Transform transform = this.lootSpawnMarker;
			if (transform == null)
			{
				transform = this.agent.transform;
			}
			Vector3 vector = transform.position;
			if (transform == null)
			{
				vector.y += 0.33f;
			}
			RaycastHit raycastHit;
			if (this.spawnOnGround && Physics.Raycast(new Ray(vector + Vector3.up * 0.5f, -Vector3.up), out raycastHit, 5f, this.groundLayerMask.value, QueryTriggerInteraction.Ignore))
			{
				vector = raycastHit.point;
			}
			entity.manager.RequestCreateItem(gameEntity.gameObject.name.GetStaticHash(), vector, transform.rotation, 0L);
		}
		GREnemy component = entity.GetComponent<GREnemy>();
		if (component != null && component.damageFlash != null)
		{
			component.damageFlash.Play();
		}
	}

	public void DestroySelf()
	{
		Debug.Log("DESTROY SELF");
		this.ReportDeathStat();
		if (this.agent.entity.IsAuthority())
		{
			this.agent.entity.manager.RequestDestroyItem(this.agent.entity.id);
		}
	}

	public void ReportDeathStat()
	{
		if (this.reported)
		{
			return;
		}
		this.reported = true;
		GameEntity entity = this.agent.entity;
		GRPlayer grplayer = GRPlayer.Get(this.instigatingActorNumber);
		if (grplayer != null)
		{
			grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.Kills, 1f);
		}
		GhostReactor.instance.shiftManager.shiftStats.IncrementEnemyKills(entity.GetEnemyType());
	}

	public override bool IsDone()
	{
		return false;
	}

	protected override void OnUpdateShared(float dt)
	{
		if (this.startTime >= 0.0)
		{
			if (this.doKnockback)
			{
				this.staggerMovement.Update(dt);
			}
			double num = Time.timeAsDouble - this.startTime;
			if (!this.isDead && num > (double)this.totalDeathDelay)
			{
				this.isDead = true;
				this.Die();
			}
			else if (this.isDead && num > (double)(this.totalDeathDelay + this.destroyDelay))
			{
				GhostReactorManager.Get(this.entity).OnAbilityDie(this.entity, this.delayRespawn);
				this.DestroySelf();
				this.startTime = -1.0;
			}
			this.events.TryPlay((float)num, this.audioSource);
		}
	}

	public static void Hide(List<Renderer> renderers, bool hide)
	{
		if (renderers == null)
		{
			return;
		}
		for (int i = 0; i < renderers.Count; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = !hide;
			}
		}
	}

	public static void Disable(List<Collider> colliders, bool disable)
	{
		if (colliders == null)
		{
			return;
		}
		for (int i = 0; i < colliders.Count; i++)
		{
			if (colliders[i] != null)
			{
				colliders[i].enabled = !disable;
			}
		}
	}

	public float delayDeath;

	public float delayRespawn = -1f;

	public List<Renderer> hideWhenDead;

	public List<Collider> disableCollidersWhenDead;

	public bool disableAllCollidersWhenDead;

	public bool disableAllRenderersWhenDead;

	public GameObject fxDeath;

	public AbilitySound soundDeath;

	public AbilitySound soundOnHide;

	public float destroyDelay = 3f;

	public bool doKnockback = true;

	public GRBreakableItemSpawnConfig lootTable;

	public bool spawnOnGround;

	public LayerMask groundLayerMask;

	public Transform lootSpawnMarker;

	public List<AnimationData> animData;

	private int instigatingActorNumber;

	private bool isDead;

	private float totalDeathDelay;

	public GRAbilityInterpolatedMovement staggerMovement;

	public GameAbilityEvents events;

	private bool reported;
}
