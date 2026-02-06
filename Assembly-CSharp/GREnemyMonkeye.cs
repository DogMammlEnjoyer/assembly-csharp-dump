using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CjLib;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class GREnemyMonkeye : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent
{
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
	}

	public void OnEntityInit()
	{
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityChase.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilitySearch.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityAttackLaser.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityAttackDiscoWander.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityAttackSlamdown.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityPatrol.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.senseNearby.Setup(this.headTransform, this.entity);
		this.Setup(this.entity.createData);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry entry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		this.agent.navAgent.autoTraverseOffMeshLink = false;
		this.agent.onJumpRequested += this.OnAgentJumpRequested;
	}

	public void OnEntityDestroy()
	{
	}

	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	public void Setup(long entityCreateData)
	{
		this.SetPatrolPath(entityCreateData);
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.Patrol, true);
		}
		else
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.Idle, true);
		}
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyMonkeye.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyMonkeye.BodyState.Bones, true);
	}

	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyMonkeye.Behavior.Jump, false);
	}

	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 11)
		{
			return;
		}
		this.SetBehavior((GREnemyMonkeye.Behavior)newState, false);
	}

	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyMonkeye.BodyState)newState, false);
	}

	public void SetPatrolPath(long entityCreateData)
	{
		GRPatrolPath grpatrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(entityCreateData);
		this.abilityPatrol.SetPatrolPath(grpatrolPath);
	}

	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	public bool TrySetBehavior(GREnemyMonkeye.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemyMonkeye.Behavior.Jump && newBehavior == GREnemyMonkeye.Behavior.Stagger)
		{
			return false;
		}
		if (newBehavior == GREnemyMonkeye.Behavior.Stagger && Time.time < this.lastStaggerTime + this.staggerImmuneTime)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	public void SetBehavior(GREnemyMonkeye.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyMonkeye.Behavior.Patrol:
			this.abilityPatrol.Stop();
			break;
		case GREnemyMonkeye.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyMonkeye.Behavior.Dying:
			this.abilityDie.Stop();
			break;
		case GREnemyMonkeye.Behavior.Chase:
			this.abilityChase.Stop();
			break;
		case GREnemyMonkeye.Behavior.Search:
			this.abilitySearch.Stop();
			break;
		case GREnemyMonkeye.Behavior.Attack:
			this.abilityAttackLaser.Stop();
			break;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.Stop();
			break;
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			this.abilityAttackSlamdown.Stop();
			break;
		case GREnemyMonkeye.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyMonkeye.Behavior.Jump:
			this.abilityJump.Stop();
			this.lastJumpEndtime = Time.timeAsDouble;
			break;
		}
		this.currBehavior = newBehavior;
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
			this.abilitySearch.Start();
			break;
		case GREnemyMonkeye.Behavior.Patrol:
			this.abilityPatrol.Start();
			break;
		case GREnemyMonkeye.Behavior.Stagger:
			this.abilityStagger.Start();
			this.lastStaggerTime = Time.time;
			break;
		case GREnemyMonkeye.Behavior.Dying:
			this.abilityDie.Start();
			break;
		case GREnemyMonkeye.Behavior.Chase:
			this.abilityChase.Start();
			this.investigateLocation = null;
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyMonkeye.Behavior.Search:
			this.abilitySearch.Start();
			break;
		case GREnemyMonkeye.Behavior.Attack:
			this.abilityAttackLaser.Start();
			this.investigateLocation = null;
			this.abilityAttackLaser.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.Start();
			this.investigateLocation = null;
			break;
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			this.abilityAttackSlamdown.Start();
			this.investigateLocation = null;
			this.abilityAttackSlamdown.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyMonkeye.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyMonkeye.Behavior.Jump:
			this.abilityJump.Start();
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	private int CalcMaxHP()
	{
		float difficultyScalingForCurrentFloor = this.entity.manager.ghostReactorManager.reactor.difficultyScalingForCurrentFloor;
		return (int)((float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax) * difficultyScalingForCurrentFloor);
	}

	public void SetBodyState(GREnemyMonkeye.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyMonkeye.BodyState.Bones:
			this.hp = this.CalcMaxHP();
			this.enemy.SetMaxHP(this.hp);
			this.enemy.SetHP(this.hp);
			break;
		case GREnemyMonkeye.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyMonkeye.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyMonkeye.BodyState.Bones:
			this.hp = this.CalcMaxHP();
			this.enemy.SetMaxHP(this.hp);
			this.enemy.SetHP(this.hp);
			break;
		case GREnemyMonkeye.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyMonkeye.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyMonkeye.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyMonkeye.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			return;
		default:
			return;
		}
	}

	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyMonkeye.tempRigs.Clear();
		GREnemyMonkeye.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyMonkeye.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyMonkeye.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
		case GREnemyMonkeye.Behavior.Patrol:
		case GREnemyMonkeye.Behavior.Investigate:
			this.ChooseNewBehavior();
			return;
		case GREnemyMonkeye.Behavior.Stagger:
		case GREnemyMonkeye.Behavior.Dying:
		case GREnemyMonkeye.Behavior.Attack:
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			break;
		case GREnemyMonkeye.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemyMonkeye.Behavior.Search:
			this.ChooseNewBehavior();
			return;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.Think(dt);
			break;
		default:
			return;
		}
	}

	private bool TryChooseAttackBehavior(float toPlayerDistSq)
	{
		if (toPlayerDistSq < this.abilityAttackLaser.GetRange() * this.abilityAttackLaser.GetRange() && this.abilityAttackLaser.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.Attack, false);
			return true;
		}
		if (this.senseNearby.IsAnyoneNearby(this.abilityAttackDiscoWander.GetRange(), false) && this.abilityAttackDiscoWander.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.AttackDisco, false);
			return true;
		}
		if (this.senseNearby.IsAnyoneNearby(this.abilityAttackSlamdown.GetRange(), false) && this.abilityAttackSlamdown.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyMonkeye.Behavior.AttackSlamdown, false);
			return true;
		}
		return false;
	}

	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
		{
			if (this.agent.targetPlayer != null)
			{
				Vector3 position = GRPlayer.Get(this.agent.targetPlayer).transform.position;
				Vector3 a = position - base.transform.position;
				float magnitude = a.magnitude;
				if (this.TryChooseAttackBehavior(magnitude * magnitude))
				{
					return;
				}
				if (this.canChaseJump && this.abilityJump.IsCoolDownOver(this.chaseJumpMinInterval) && magnitude > this.attackRange + this.minChaseJumpDistance && GRSenseLineOfSight.HasNavmeshLineOfSight(base.transform.position, position, 10f))
				{
					Vector3 a2 = a / magnitude;
					float d = Mathf.Clamp(this.chaseJumpDistance, this.minChaseJumpDistance, magnitude - this.attackRange * 0.5f);
					NavMeshHit navMeshHit;
					if (NavMesh.SamplePosition(base.transform.position + a2 * d, out navMeshHit, 0.5f, AbilityHelperFunctions.GetNavMeshWalkableArea()))
					{
						this.agent.GetGameAgentManager().RequestJump(this.agent, base.transform.position, navMeshHit.position, 0.25f, 1.5f);
						return;
					}
				}
			}
			if (!this.abilityAttackLaser.IsCoolDownOver())
			{
				this.TrySetBehavior(GREnemyMonkeye.Behavior.Idle);
				return;
			}
			this.TrySetBehavior(GREnemyMonkeye.Behavior.Chase);
			return;
		}
		else
		{
			this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
			if (this.investigateLocation != null)
			{
				this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
				this.SetBehavior(GREnemyMonkeye.Behavior.Investigate, false);
				return;
			}
			if (this.abilityPatrol.HasValidPatrolPath())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Patrol, false);
				return;
			}
			this.SetBehavior(GREnemyMonkeye.Behavior.Idle, false);
			return;
		}
	}

	private void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	private void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
			this.abilityIdle.UpdateAuthority(dt);
			return;
		case GREnemyMonkeye.Behavior.Patrol:
			this.abilityPatrol.UpdateAuthority(dt);
			return;
		case GREnemyMonkeye.Behavior.Stagger:
			this.abilityStagger.UpdateAuthority(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.agent.targetPlayer == null)
				{
					this.SetBehavior(GREnemyMonkeye.Behavior.Search, false);
					return;
				}
				this.SetBehavior(GREnemyMonkeye.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.Dying:
			this.abilityDie.UpdateAuthority(dt);
			return;
		case GREnemyMonkeye.Behavior.Chase:
		{
			this.abilityChase.UpdateAuthority(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Search, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float sqrMagnitude = (grplayer.transform.position - base.transform.position).sqrMagnitude;
				this.TryChooseAttackBehavior(sqrMagnitude);
				return;
			}
			break;
		}
		case GREnemyMonkeye.Behavior.Search:
			this.abilitySearch.UpdateAuthority(dt);
			if (this.abilitySearch.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.Attack:
			this.abilityAttackLaser.UpdateAuthority(dt);
			if (this.abilityAttackLaser.IsDone())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.UpdateAuthority(dt);
			if (this.abilityAttackDiscoWander.IsDone())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			this.abilityAttackSlamdown.UpdateAuthority(dt);
			if (this.abilityAttackSlamdown.IsDone())
			{
				this.SetBehavior(GREnemyMonkeye.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.Investigate:
			this.abilityInvestigate.UpdateAuthority(dt);
			if (this.abilityInvestigate.IsDone())
			{
				this.investigateLocation = null;
			}
			if (GhostReactorManager.noiseDebugEnabled)
			{
				DebugUtil.DrawLine(base.transform.position, this.abilityInvestigate.GetTargetPos(), Color.green, true);
				return;
			}
			break;
		case GREnemyMonkeye.Behavior.Jump:
			this.abilityJump.UpdateAuthority(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
			}
			break;
		default:
			return;
		}
	}

	private void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyMonkeye.Behavior.Idle:
			this.abilityIdle.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Patrol:
			this.abilityPatrol.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Dying:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Chase:
			this.abilityChase.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Search:
			this.abilitySearch.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Attack:
			this.abilityAttackLaser.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.AttackDisco:
			this.abilityAttackDiscoWander.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.AttackSlamdown:
			this.abilityAttackSlamdown.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemyMonkeye.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			return;
		default:
			return;
		}
	}

	private void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyMonkeye.BodyState.Bones)
		{
			this.hp -= hit.hitAmount;
			this.enemy.SetHP(this.hp);
			if (this.damagedSounds.Count > 0)
			{
				this.damagedSoundIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.damagedSounds.Count, this.damagedSoundIndex);
				this.audioSource.PlayOneShot(this.damagedSounds[this.damagedSoundIndex], this.damagedSoundVolume);
			}
			if (this.fxDamaged != null)
			{
				this.fxDamaged.SetActive(false);
				this.fxDamaged.SetActive(true);
			}
			if (this.hp <= 0)
			{
				this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hit.hitByEntityId));
				this.SetBodyState(GREnemyMonkeye.BodyState.Destroyed, false);
				this.SetBehavior(GREnemyMonkeye.Behavior.Dying, false);
				return;
			}
			this.lastSeenTargetPosition = tool.transform.position;
			this.lastSeenTargetTime = Time.timeAsDouble;
			Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
			vector.y = 0f;
			this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
			if (this.allowStagger)
			{
				this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
				this.TrySetBehavior(GREnemyMonkeye.Behavior.Stagger);
				return;
			}
		}
		else if (this.currBodyState == GREnemyMonkeye.BodyState.Shell && this.armor != null)
		{
			this.armor.PlayBlockFx(hit.hitEntityPosition);
		}
	}

	public void InstantDeath()
	{
		this.hp = 0;
		this.SetBodyState(GREnemyMonkeye.BodyState.Destroyed, false);
		this.SetBehavior(GREnemyMonkeye.Behavior.Dying, false);
	}

	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
	}

	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyMonkeye.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyMonkeye.Behavior.Attack && this.currBehavior != GREnemyMonkeye.Behavior.AttackDisco && this.currBehavior != GREnemyMonkeye.Behavior.AttackSlamdown)
		{
			return;
		}
		GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
		if (component != null)
		{
			GameHittable component2 = base.GetComponent<GameHittable>();
			component.BlockHittable(this.headTransform.position, base.transform.forward, component2);
			return;
		}
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component3 = attachedRigidbody.GetComponent<GRPlayer>();
			if (component3 != null && component3.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
			{
				if (this.tryHitPlayerCoroutine != null)
				{
					base.StopCoroutine(this.tryHitPlayerCoroutine);
				}
				this.tryHitPlayerCoroutine = base.StartCoroutine(this.TryHitPlayer(component3));
			}
			GRBreakable component4 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component5 = attachedRigidbody.GetComponent<GameHittable>();
			if (component4 != null && component5 != null)
			{
				GameHitData hitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component5.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component4.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component4.transform.position,
					hittablePoint = component5.FindHittablePoint(collider)
				};
				component5.RequestHit(hitData);
			}
		}
	}

	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if ((this.currBehavior == GREnemyMonkeye.Behavior.Attack || this.currBehavior == GREnemyMonkeye.Behavior.AttackDisco || this.currBehavior == GREnemyMonkeye.Behavior.AttackSlamdown) && player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			Vector3 hitImpulse = player.transform.position - base.transform.position;
			hitImpulse.y = 0f;
			hitImpulse = hitImpulse.normalized * 6f;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position, hitImpulse);
		}
		yield break;
	}

	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("speed: <color=\"yellow\">{0}<color=\"white\"> patrol node:<color=\"yellow\">{1}/{2}<color=\"white\">", this.navAgent.speed, this.abilityPatrol.nextPatrolNode, (this.abilityPatrol.GetPatrolPath() != null) ? this.abilityPatrol.GetPatrolPath().patrolNodes.Count : 0));
	}

	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		byte value2 = (byte)this.currBodyState;
		byte value3 = (byte)this.abilityPatrol.nextPatrolNode;
		int value4 = (this.targetPlayer == null) ? -1 : this.targetPlayer.ActorNumber;
		writer.Write(value);
		writer.Write(value2);
		writer.Write(this.hp);
		writer.Write(value3);
		writer.Write(value4);
	}

	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyMonkeye.Behavior newBehavior = (GREnemyMonkeye.Behavior)reader.ReadByte();
		GREnemyMonkeye.BodyState newBodyState = (GREnemyMonkeye.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte nextPatrolNode = reader.ReadByte();
		int playerID = reader.ReadInt32();
		this.SetPatrolPath(this.entity.createData);
		this.abilityPatrol.SetNextPatrolNode((int)nextPatrolNode);
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
	}

	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		GRTool gameComponent = this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId);
		if (gameComponent != null)
		{
			switch (hitTypeId)
			{
			case GameHitType.Club:
				this.OnHitByClub(gameComponent, hit);
				break;
			case GameHitType.Flash:
				this.OnHitByFlash(gameComponent, hit);
				break;
			case GameHitType.Shield:
				this.OnHitByShield(gameComponent, hit);
				break;
			}
			if (gameComponent.gameEntity != null)
			{
				this.senseNearby.OnHitByPlayer(gameComponent.gameEntity.lastHeldByActorNumber);
			}
		}
	}

	public GameEntity entity;

	public GameAgent agent;

	public GREnemy enemy;

	public GRArmorEnemy armor;

	public GameHittable hittable;

	[SerializeField]
	private GRAttributes attributes;

	public GRSenseNearby senseNearby;

	public GRSenseLineOfSight senseLineOfSight;

	public Animation anim;

	public GRAbilityIdle abilityIdle;

	public GRAbilityChase abilityChase;

	public GRAbilityIdle abilitySearch;

	[FormerlySerializedAs("abilityAttackSwipe")]
	public GRAbilityAttackLaser abilityAttackLaser;

	public GRAbilityAttackSimpleWander abilityAttackDiscoWander;

	public GRAbilityAttackSimple abilityAttackSlamdown;

	public bool allowStagger;

	public GRAbilityStagger abilityStagger;

	public GRAbilityDie abilityDie;

	public GRAbilityMoveToTarget abilityInvestigate;

	public GRAbilityPatrol abilityPatrol;

	public GRAbilityJump abilityJump;

	public List<Renderer> bones;

	public List<Renderer> always;

	public Transform headTransform;

	public float turnSpeed = 540f;

	public float attackRange = 1.5f;

	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	public NavMeshAgent navAgent;

	public AudioSource audioSource;

	public AudioClip damagedSound;

	public float damagedSoundVolume;

	public List<AudioClip> damagedSounds;

	private int damagedSoundIndex;

	public GameObject fxDamaged;

	private Vector3? investigateLocation;

	private float lastStaggerTime;

	public float staggerImmuneTime = 10f;

	private Transform target;

	[ReadOnly]
	public int hp;

	[ReadOnly]
	public GREnemyMonkeye.Behavior currBehavior;

	[ReadOnly]
	public GREnemyMonkeye.BodyState currBodyState;

	[ReadOnly]
	public NetPlayer targetPlayer;

	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	[ReadOnly]
	public double lastSeenTargetTime;

	[ReadOnly]
	public Vector3 searchPosition;

	private double lastJumpEndtime;

	public bool canChaseJump = true;

	public float chaseJumpDistance = 5f;

	public float chaseJumpMinInterval = 1f;

	public float minChaseJumpDistance = 2f;

	private Rigidbody rigidBody;

	private List<Collider> colliders;

	private float lastHitPlayerTime;

	private float minTimeBetweenHits = 0.5f;

	public float hearingRadius = 5f;

	private static List<VRRig> tempRigs = new List<VRRig>(16);

	private Coroutine tryHitPlayerCoroutine;

	public enum Behavior
	{
		Idle,
		Patrol,
		Stagger,
		Dying,
		Chase,
		Search,
		Attack,
		AttackDisco,
		AttackSlamdown,
		Investigate,
		Jump,
		Count
	}

	public enum BodyState
	{
		Destroyed,
		Bones,
		Shell,
		Count
	}
}
