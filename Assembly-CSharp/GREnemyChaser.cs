using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CjLib;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

public class GREnemyChaser : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent
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
		this.abilityAttackSwipe.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityPatrol.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityWander.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityFlashed.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
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

	private void Setup(long entityCreateData)
	{
		this.SetPatrolPath(entityCreateData);
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyChaser.Behavior.Patrol, true);
		}
		else
		{
			this.SetBehavior(GREnemyChaser.Behavior.Wander, true);
		}
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyChaser.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyChaser.BodyState.Bones, true);
	}

	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyChaser.Behavior.Jump, false);
	}

	private void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 11)
		{
			return;
		}
		this.SetBehavior((GREnemyChaser.Behavior)newState, false);
	}

	private void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyChaser.BodyState)newState, false);
	}

	private void SetPatrolPath(long entityCreateData)
	{
		GRPatrolPath grpatrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(entityCreateData);
		this.abilityPatrol.SetPatrolPath(grpatrolPath);
	}

	private void SetNextPatrolNode(int nextPatrolNode)
	{
		this.abilityPatrol.SetNextPatrolNode(nextPatrolNode);
	}

	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	private bool TrySetBehavior(GREnemyChaser.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemyChaser.Behavior.Jump && newBehavior == GREnemyChaser.Behavior.Stagger)
		{
			return false;
		}
		if (newBehavior == GREnemyChaser.Behavior.Stagger && Time.time < this.lastStaggerTime + this.staggerImmuneTime)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	private void SetBehavior(GREnemyChaser.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.Stop();
			break;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.Stop();
			break;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyChaser.Behavior.Dying:
			this.abilityDie.Stop();
			break;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.Stop();
			break;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.Stop();
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.Stop();
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.Stop();
			break;
		case GREnemyChaser.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyChaser.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilitySearch.Start();
			break;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.Start();
			break;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.Start();
			break;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Start();
			this.lastStaggerTime = Time.time;
			break;
		case GREnemyChaser.Behavior.Dying:
			if (this.entity.IsAuthority())
			{
				this.entity.manager.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			this.abilityDie.Start();
			break;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.Start();
			this.investigateLocation = null;
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.Start();
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.Start();
			this.investigateLocation = null;
			this.abilityAttackSwipe.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.Start();
			break;
		case GREnemyChaser.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyChaser.Behavior.Jump:
			this.abilityJump.Start();
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null)
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	private void SetBodyState(GREnemyChaser.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyChaser.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyChaser.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyChaser.BodyState.Shell:
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
		case GREnemyChaser.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyChaser.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyChaser.BodyState.Shell:
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
		GREnemyChaser.tempRigs.Clear();
		GREnemyChaser.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyChaser.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyChaser.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
		case GREnemyChaser.Behavior.Patrol:
		case GREnemyChaser.Behavior.Investigate:
			this.ChooseNewBehavior();
			return;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemyChaser.Behavior.Stagger:
		case GREnemyChaser.Behavior.Dying:
		case GREnemyChaser.Behavior.Attack:
		case GREnemyChaser.Behavior.Flashed:
			break;
		case GREnemyChaser.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			this.ChooseNewBehavior();
			break;
		case GREnemyChaser.Behavior.Search:
			this.ChooseNewBehavior();
			return;
		default:
			return;
		}
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
				if (magnitude < this.attackRange)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Attack, false);
				}
				else if (this.canChaseJump && this.abilityJump.IsCoolDownOver(this.chaseJumpMinInterval) && magnitude > this.attackRange + this.minChaseJumpDistance && GRSenseLineOfSight.HasNavmeshLineOfSight(base.transform.position, position, 10f))
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
			this.TrySetBehavior(GREnemyChaser.Behavior.Chase);
			return;
		}
		this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
		if (this.investigateLocation != null)
		{
			this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
			this.SetBehavior(GREnemyChaser.Behavior.Investigate, false);
			return;
		}
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyChaser.Behavior.Patrol, false);
			return;
		}
		this.SetBehavior(GREnemyChaser.Behavior.Wander, false);
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
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.UpdateAuthority(dt);
			return;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.UpdateAuthority(dt);
			return;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.UpdateAuthority(dt);
			return;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.UpdateAuthority(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.agent.targetPlayer == null)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Search, false);
					return;
				}
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Dying:
			this.abilityDie.UpdateAuthority(dt);
			return;
		case GREnemyChaser.Behavior.Chase:
		{
			this.abilityChase.UpdateAuthority(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Search, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float num = this.attackRange * this.attackRange;
				if ((grplayer.transform.position - base.transform.position).sqrMagnitude < num)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Attack, false);
					return;
				}
			}
			break;
		}
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.UpdateAuthority(dt);
			if (this.abilitySearch.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.UpdateAuthority(dt);
			if (this.abilityAttackSwipe.IsDone())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.UpdateAuthority(dt);
			if (this.abilityFlashed.IsDone())
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Search, false);
					return;
				}
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Investigate:
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
		case GREnemyChaser.Behavior.Jump:
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
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Wander:
			this.abilityWander.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Dying:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			return;
		default:
			return;
		}
	}

	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState != GREnemyChaser.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyChaser.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
		this.hp -= hit.hitAmount;
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
			this.SetBodyState(GREnemyChaser.BodyState.Destroyed, false);
			this.SetBehavior(GREnemyChaser.Behavior.Dying, false);
			return;
		}
		this.lastSeenTargetPosition = tool.transform.position;
		this.lastSeenTargetTime = Time.timeAsDouble;
		Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
		vector.y = 0f;
		this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
		this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
		this.TrySetBehavior(GREnemyChaser.Behavior.Stagger);
	}

	public void InstantDeath()
	{
		this.hp = 0;
		this.SetBodyState(GREnemyChaser.BodyState.Destroyed, false);
		this.SetBehavior(GREnemyChaser.Behavior.Dying, false);
	}

	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyChaser.BodyState.Shell)
		{
			this.hp -= hit.hitAmount;
			if (this.armor != null)
			{
				this.armor.SetHp(this.hp);
			}
			if (this.hp <= 0)
			{
				if (this.armor != null)
				{
					this.armor.PlayDestroyFx(this.armor.transform.position);
				}
				this.SetBodyState(GREnemyChaser.BodyState.Bones, false);
				if (grTool.gameEntity.IsHeldByLocalPlayer())
				{
					PlayerGameEvents.MiscEvent("GRArmorBreak_" + base.name, 1);
				}
				if (grTool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
				{
					this.armor.FragmentArmor();
				}
			}
			else if (grTool != null)
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.lastSeenTargetPosition = grTool.transform.position;
				this.lastSeenTargetTime = Time.timeAsDouble;
				Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
				vector.y = 0f;
				this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
				this.RefreshBody();
			}
			else
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.RefreshBody();
			}
		}
		GRToolFlash component = grTool.GetComponent<GRToolFlash>();
		if (component != null)
		{
			this.abilityFlashed.SetStunTime(component.stunDuration);
		}
		this.TrySetBehavior(GREnemyChaser.Behavior.Flashed);
	}

	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		Debug.Log(string.Format("Chaser On Hit By Shield dmg:{0} impulse:{1} size:{2}", hit.hitAmount, hit.hitImpulse, hit.hitImpulse.magnitude));
		this.OnHitByClub(tool, hit);
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyChaser.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyChaser.Behavior.Attack)
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
		if (this.currBehavior == GREnemyChaser.Behavior.Attack && player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position);
		}
		yield break;
	}

	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("speed: <color=\"yellow\">{0}<color=\"white\"> patrol node:<color=\"yellow\">{1}/{2}<color=\"white\">", this.navAgent.speed, this.abilityPatrol.nextPatrolNode, (this.abilityPatrol.GetPatrolPath() != null) ? this.abilityPatrol.GetPatrolPath().patrolNodes.Count : 0));
		strings.Add(string.Format("Dest: <color=\"yellow\">{0}<color=\"white\"> Pos: <color=\"yellow\">{1}<color=\"white\">", this.agent.navAgent.destination, this.agent.transform.position));
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
		GREnemyChaser.Behavior newBehavior = (GREnemyChaser.Behavior)reader.ReadByte();
		GREnemyChaser.BodyState newBodyState = (GREnemyChaser.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte nextPatrolNode = reader.ReadByte();
		int playerID = reader.ReadInt32();
		this.SetPatrolPath(this.entity.createData);
		this.SetNextPatrolNode((int)nextPatrolNode);
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
				return;
			case GameHitType.Flash:
				this.OnHitByFlash(gameComponent, hit);
				return;
			case GameHitType.Shield:
				this.OnHitByShield(gameComponent, hit);
				break;
			default:
				return;
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

	public GRAbilityAttackSwipe abilityAttackSwipe;

	public GRAbilityStagger abilityStagger;

	public GRAbilityDie abilityDie;

	public GRAbilityMoveToTarget abilityInvestigate;

	public GRAbilityPatrol abilityPatrol;

	public GRAbilityWander abilityWander;

	public GRAbilityFlashed abilityFlashed;

	public GRAbilityJump abilityJump;

	public List<Renderer> bones;

	public List<Renderer> always;

	public Transform coreMarker;

	public GRCollectible corePrefab;

	public Transform headTransform;

	public float turnSpeed = 540f;

	public SoundBankPlayer chaseSoundBank;

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
	public GREnemyChaser.Behavior currBehavior;

	[ReadOnly]
	public GREnemyChaser.BodyState currBodyState;

	[ReadOnly]
	public NetPlayer targetPlayer;

	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	[ReadOnly]
	public double lastSeenTargetTime;

	[ReadOnly]
	public Vector3 searchPosition;

	public bool canChaseJump = true;

	public float chaseJumpDistance = 5f;

	public float chaseJumpMinInterval = 1f;

	public float minChaseJumpDistance = 2f;

	public static RaycastHit[] visibilityHits = new RaycastHit[16];

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
		Wander,
		Stagger,
		Dying,
		Chase,
		Search,
		Attack,
		Flashed,
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
