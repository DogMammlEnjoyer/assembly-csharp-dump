using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using GorillaTagScripts.GhostReactor;
using JetBrains.Annotations;
using Unity.XR.CoreUtils;
using UnityEngine;

public class GREnemyBossMoon : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent, IGRSummoningEntity
{
	public bool BossHasRevealed { get; private set; }

	public GRAbilityBase CurrAbility
	{
		get
		{
			return this.currAbility;
		}
	}

	private void Awake()
	{
		this.trackedEntities = new List<int>(16);
		this.trackedGameEntities = new List<GameEntity>(16);
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.abilities = new GRAbilityBase[32];
		this.adaptiveMusicController = Object.FindObjectOfType<GRAdaptiveMusicController>();
	}

	public void OnEntityInit()
	{
		this.currBehavior = GREnemyBossMoon.Behavior.None;
		this.currAbility = null;
		this.SetupAbility(GREnemyBossMoon.Behavior.HiddenIdle, this.abilityHiddenIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Reveal, this.abilityReveal, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Idle, this.abilityIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Exposed, this.abilityExposed, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.ExposedIdle, this.abilityExposedIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTongue, this.abilityAttackTongue01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTongueSwipe, this.abilityAttackTongueSwipe01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle00, this.abilityAttackTentacle00, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle01, this.abilityAttackTentacle01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle02, this.abilityAttackTentacle02, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle03, this.abilityAttackTentacle03, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle04, this.abilityAttackTentacle04, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle05, this.abilityAttackTentacle05, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackQuickTentacle00, this.abilityAttackQuickTentacle00, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackQuickTentacle01, this.abilityAttackQuickTentacle01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackQuickTentacle02, this.abilityAttackQuickTentacle02, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackQuickTentacle03, this.abilityAttackQuickTentacle03, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.SummonStart, this.abilitySummonStart, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.SummonEnd, this.abilitySummonEnd, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Summon01, this.abilitySummon01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Summon02, this.abilitySummon02, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Summon03, this.abilitySummon03, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Summon04, this.abilitySummon04, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.RetreatStart, this.abilityRetreatStart, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.RetreatEnd, this.abilityRetreatEnd, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.RetreatIdle, this.abilityRetreatIdle, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Dying, this.abilityDie, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.DyingIdle, this.abilityDieIdle, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Runaway, this.abilityRunaway, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.NextPhase, this.abilityIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.senseNearby.Setup(this.headTransform, this.entity);
		this.Setup(this.entity.createData);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			GhostReactorLevelGenConfig currLevelGenConfig = this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig();
			foreach (GRBonusEntry entry in currLevelGenConfig.enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
			if (currLevelGenConfig.minEnemyKills.Count > 0)
			{
				GREnemyCount grenemyCount = currLevelGenConfig.minEnemyKills[0];
				GREnemyType enemyType = grenemyCount.EnemyType;
				if (enemyType != GREnemyType.MoonBoss_Phase1)
				{
					if (enemyType == GREnemyType.MoonBoss_Phase2)
					{
						this.phases[1].runawayAfterPhase = true;
					}
				}
				else
				{
					this.phases[0].runawayAfterPhase = true;
				}
				GRBreakableItemSpawnConfig lootTableForType = this.GetLootTableForType(grenemyCount.EnemyType);
				this.abilityDie.lootTable = lootTableForType;
				this.abilityRunaway.lootTable = lootTableForType;
			}
		}
		if (this.agent.navAgent != null)
		{
			this.agent.navAgent.autoTraverseOffMeshLink = false;
		}
		this.SetBehavior(GREnemyBossMoon.Behavior.HiddenIdle, true);
		int maxHP = this.CalcMaxHP();
		if (this.enemy != null)
		{
			this.enemy.SetMaxHP(maxHP);
		}
		this.SetHP(maxHP);
	}

	private GRBreakableItemSpawnConfig GetLootTableForType(GREnemyType enemyType)
	{
		for (int i = 0; i < this.lootPhases.Count; i++)
		{
			if (this.lootPhases[i].enemyType == enemyType)
			{
				return this.lootPhases[i].lootTable;
			}
		}
		return null;
	}

	private void SetupAbility(GREnemyBossMoon.Behavior behavior, GRAbilityBase ability, GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		this.abilities[(int)behavior] = ability;
		ability.Setup(agent, anim, audioSource, root, head, lineOfSight);
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
		this.SetBehavior(GREnemyBossMoon.Behavior.HiddenIdle, true);
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyBossMoon.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyBossMoon.BodyState.Bones, true);
	}

	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 32)
		{
			return;
		}
		this.SetBehavior((GREnemyBossMoon.Behavior)newState, false);
	}

	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyBossMoon.BodyState)newState, false);
	}

	public void SetHP(int hp)
	{
		this.hp = hp;
		if (this.enemy != null)
		{
			this.enemy.SetHP(hp);
		}
	}

	public bool TrySetBehavior(GREnemyBossMoon.Behavior newBehavior)
	{
		if (newBehavior == GREnemyBossMoon.Behavior.Stagger)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	public void SetBehavior(GREnemyBossMoon.Behavior newBehavior, bool force = false)
	{
		if (newBehavior < GREnemyBossMoon.Behavior.HiddenIdle || newBehavior >= (GREnemyBossMoon.Behavior)this.abilities.Length)
		{
			Debug.LogErrorFormat("New Behavior Index is invalid {0} {1} {2}", new object[]
			{
				(int)newBehavior,
				newBehavior,
				base.gameObject.name
			});
			return;
		}
		GRAbilityBase grabilityBase = this.abilities[(int)newBehavior];
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		GREnemyBossMoon.Behavior behavior = this.currBehavior;
		if (behavior != GREnemyBossMoon.Behavior.AttackTongue)
		{
			if (behavior == GREnemyBossMoon.Behavior.NextPhase)
			{
				this.IncrementBossPhase();
			}
		}
		else
		{
			for (int i = 0; i < this.eyes.Count; i++)
			{
				this.eyes[i].ResetEye();
			}
			this.consecutiveCombos = 0;
			this.attacksAfterSummon = 0;
			this.currSummon = null;
			this.KillAllSummoned(true, true);
			if (this.triggerNextMusicTransition)
			{
				this.triggerNextMusicTransition = false;
				if (this.adaptiveMusicController != null)
				{
					this.adaptiveMusicController.TransitionToNextTrack();
				}
			}
		}
		Debug.LogFormat("Boss SetBehavior {0} -> {1}", new object[]
		{
			this.currBehavior,
			newBehavior
		});
		if (this.currAbility != null)
		{
			this.currAbility.Stop();
		}
		this.lastBehavior = this.currBehavior;
		this.currBehavior = newBehavior;
		this.currAbility = grabilityBase;
		if (this.currAbility != null)
		{
			this.currAbility.Start();
		}
		behavior = this.currBehavior;
		switch (behavior)
		{
		case GREnemyBossMoon.Behavior.Reveal:
			if (this.firstTimeReveal)
			{
				if (this.adaptiveMusicController != null)
				{
					this.adaptiveMusicController.Restart();
				}
				this.internalPhaseIndex = 0;
			}
			this.firstTimeReveal = false;
			this.BossHasRevealed = true;
			break;
		case GREnemyBossMoon.Behavior.Exposed:
			this.ToggleShockColliders(false);
			break;
		case GREnemyBossMoon.Behavior.ExposedIdle:
			break;
		case GREnemyBossMoon.Behavior.Stagger:
			this.lastStaggerTime = Time.time;
			break;
		case GREnemyBossMoon.Behavior.Dying:
			this.KillAllSummoned();
			this.TurnOffGrav();
			for (int j = 0; j < this.eyes.Count; j++)
			{
				this.eyes[j].TrySetBehavior(GREnemyBossMoonEye.Behavior.Dying);
			}
			if (this.adaptiveMusicController != null)
			{
				this.adaptiveMusicController.TransitionToLastTrack();
			}
			this.ToggleShockColliders(false);
			break;
		default:
			switch (behavior)
			{
			case GREnemyBossMoon.Behavior.AttackTongue:
				this.ToggleShockColliders(true);
				break;
			case GREnemyBossMoon.Behavior.Summon01:
			case GREnemyBossMoon.Behavior.Summon02:
			case GREnemyBossMoon.Behavior.Summon03:
			case GREnemyBossMoon.Behavior.Summon04:
				this.currSummon = (GRAbilitySummon)this.currAbility;
				break;
			case GREnemyBossMoon.Behavior.RetreatStart:
				this.TurnOnGrav();
				break;
			case GREnemyBossMoon.Behavior.RetreatEnd:
				this.TurnOffGrav();
				break;
			case GREnemyBossMoon.Behavior.Runaway:
				if (this.entity.manager.ghostReactorManager != null)
				{
					this.entity.manager.ghostReactorManager.InstantDeathForCurrentEnemies();
				}
				if (this.adaptiveMusicController != null)
				{
					this.adaptiveMusicController.TransitionToLastTrack();
				}
				break;
			}
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	public void SetSquishVolumeState(bool squishEnabled)
	{
		for (int i = 0; i < this.squishVolumes.Count; i++)
		{
			this.squishVolumes[i].overrideDisabled = !squishEnabled;
			this.squishVolumes[i].SliceUpdate();
		}
	}

	private int CalcMaxHP()
	{
		float difficultyScalingForCurrentFloor = this.entity.manager.ghostReactorManager.reactor.difficultyScalingForCurrentFloor;
		int result = (int)((float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax) * difficultyScalingForCurrentFloor);
		for (int i = 0; i < this.phases.Count; i++)
		{
			this.phases[i].minHP = Mathf.RoundToInt((float)this.phases[i].minHP * difficultyScalingForCurrentFloor);
		}
		return result;
	}

	public int GetCurrPhaseIndex()
	{
		if (this.phases == null)
		{
			return -1;
		}
		for (int i = 0; i < this.phases.Count; i++)
		{
			if (this.hp > this.phases[i].minHP)
			{
				return i;
			}
		}
		return this.phases.Count - 1;
	}

	public GREnemyBossMoon.PhaseDef GetCurrPhase()
	{
		int currPhaseIndex = this.GetCurrPhaseIndex();
		if (currPhaseIndex < 0 || currPhaseIndex >= this.phases.Count)
		{
			return null;
		}
		return this.phases[currPhaseIndex];
	}

	public void RestoreFullHealth()
	{
		this.SetHP(this.CalcMaxHP());
	}

	public void HurtBossHP()
	{
		this.HurtBoss(100, this.entity.id, Vector3.zero);
	}

	public void KillAllEyes()
	{
		for (int i = 0; i < this.eyes.Count; i++)
		{
			this.eyes[i].InstantKill();
		}
	}

	public void KillAllSummoned()
	{
		this.KillAllSummoned(true, true);
	}

	public void KillAllSummoned(bool ignoreMonkeye = false, bool killAllEnemies = true)
	{
		int num = 0;
		for (int i = 0; i < this.trackedGameEntities.Count; i++)
		{
			if (!(this.trackedGameEntities[i] == null))
			{
				GREnemyChaser component = this.trackedGameEntities[i].GetComponent<GREnemyChaser>();
				if (component != null)
				{
					component.InstantDeath();
					num++;
				}
				else
				{
					GREnemyRanged component2 = this.trackedGameEntities[i].GetComponent<GREnemyRanged>();
					if (component2 != null)
					{
						component2.InstantDeath();
						num++;
					}
					else
					{
						GREnemyPest component3 = this.trackedGameEntities[i].GetComponent<GREnemyPest>();
						if (component3 != null)
						{
							component3.InstantDeath();
							num++;
						}
						else
						{
							GREnemySummoner component4 = this.trackedGameEntities[i].GetComponent<GREnemySummoner>();
							if (component4 != null)
							{
								component4.InstantDeath();
								num++;
							}
							else if (!ignoreMonkeye)
							{
								GREnemyMonkeye component5 = this.trackedGameEntities[i].GetComponent<GREnemyMonkeye>();
								if (component5 != null)
								{
									component5.InstantDeath();
									num++;
								}
							}
						}
					}
				}
			}
		}
		if (killAllEnemies && this.entity.manager.ghostReactorManager != null)
		{
			this.entity.manager.ghostReactorManager.InstantDeathForCurrentEnemies();
		}
		Debug.Log(string.Format("Report killed all summon {0}", num));
	}

	public void GoBackPhase()
	{
		int currPhaseIndex = this.GetCurrPhaseIndex();
		if (currPhaseIndex <= 0)
		{
			Debug.LogWarning("GREnemyBossMoon - GoBackPhase - At first phase");
			return;
		}
		this.SetHP(this.phases[currPhaseIndex - 1].minHP);
	}

	public void GoToNextPhase()
	{
		int currPhaseIndex = this.GetCurrPhaseIndex();
		if (currPhaseIndex < 0 || currPhaseIndex >= this.phases.Count)
		{
			return;
		}
		this.SetHP(this.phases[currPhaseIndex].minHP);
	}

	private bool IsSummon(GREnemyBossMoon.Behavior behavior)
	{
		for (int i = 0; i < this.phases.Count; i++)
		{
			if (this.phases[i] != null && this.phases[i].summons != null && this.phases[i].summons.Contains(behavior))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsAnySummonBehavior(GREnemyBossMoon.Behavior behavior)
	{
		return this.currBehavior == GREnemyBossMoon.Behavior.SummonStart || this.currBehavior == GREnemyBossMoon.Behavior.SummonEnd || this.currBehavior == GREnemyBossMoon.Behavior.Summon01 || this.currBehavior == GREnemyBossMoon.Behavior.Summon02 || this.currBehavior == GREnemyBossMoon.Behavior.Summon03 || this.currBehavior == GREnemyBossMoon.Behavior.Summon04;
	}

	public GREnemyBossMoon.Behavior ChooseSummonForPhase()
	{
		GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
		if (currPhase == null)
		{
			return GREnemyBossMoon.Behavior.None;
		}
		return this.ChooseRandomBehavior(currPhase.summons);
	}

	public GREnemyBossMoon.Behavior ChooseAttackForPhase()
	{
		GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
		if (currPhase == null)
		{
			return GREnemyBossMoon.Behavior.None;
		}
		return this.ChooseRandomBehavior(currPhase.attacks);
	}

	public GREnemyBossMoon.Behavior ChooseRandomBehavior(List<GREnemyBossMoon.Behavior> behaviors)
	{
		if (behaviors == null || behaviors.Count <= 0)
		{
			return GREnemyBossMoon.Behavior.None;
		}
		int index = Random.Range(0, behaviors.Count);
		return behaviors[index];
	}

	public void SetBodyState(GREnemyBossMoon.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		this.currBodyState = newBodyState;
		if (this.currBodyState == GREnemyBossMoon.BodyState.Destroyed)
		{
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
		}
		Debug.LogFormat("State Change {0} {1}", new object[]
		{
			this.entity.id.index,
			this.currBodyState
		});
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
		case GREnemyBossMoon.BodyState.Destroyed:
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyBossMoon.BodyState.Bones:
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyBossMoon.BodyState.Shell:
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
		GREnemyBossMoon.tempRigs.Clear();
		GREnemyBossMoon.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyBossMoon.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyBossMoon.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		if (this.currAbility != null)
		{
			this.currAbility.Think(dt);
		}
		GREnemyBossMoon.Behavior behavior = this.currBehavior;
		if (behavior != GREnemyBossMoon.Behavior.HiddenIdle)
		{
			if (behavior != GREnemyBossMoon.Behavior.Idle)
			{
				if (behavior != GREnemyBossMoon.Behavior.RetreatIdle)
				{
					return;
				}
				this.waitInRetreat += dt * 12f;
				if (this.trackedEntities.Count <= 0 || this.waitInRetreat > 20f)
				{
					this.TrySetBehavior(GREnemyBossMoon.Behavior.RetreatEnd);
				}
			}
			else if (this.currAbility.IsDone())
			{
				this.ChooseNewBehavior(false);
				return;
			}
			return;
		}
		this.ChooseNewBehavior(true);
	}

	private GREnemyBossMoon.Behavior TryChooseAttackBehavior()
	{
		GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
		if (this.currBehavior == GREnemyBossMoon.Behavior.HiddenIdle)
		{
			if (currPhase != null && this.trackedEntities.Count <= currPhase.maxEnemiesForReveal && this.senseNearby.IsAnyoneNearby(this.abilityReveal.GetRange(), this.firstTimeReveal))
			{
				return GREnemyBossMoon.Behavior.Reveal;
			}
			return GREnemyBossMoon.Behavior.None;
		}
		else
		{
			if (GhostReactorManager.AggroDisabled)
			{
				return GREnemyBossMoon.Behavior.None;
			}
			if (currPhase == null)
			{
				return GREnemyBossMoon.Behavior.None;
			}
			if (currPhase.summons != null && currPhase.summons.Count > 0 && this.attacksAfterSummon <= 0 && this.trackedEntities.Count < currPhase.maxSimultaneousEnemies)
			{
				this.attacksAfterSummon = currPhase.attacksBetweenSummons;
				if (currPhase.summons.Count > 0)
				{
					this.currSummon = (GRAbilitySummon)this.abilities[(int)currPhase.summons[0]];
					if (this.currSummon != null)
					{
						for (int i = this.trackedEntities.Count; i < currPhase.maxSimultaneousEnemies; i++)
						{
							this.currSummon.ForceSpawn();
						}
					}
				}
			}
			List<GREnemyBossMoon.Behavior> list = currPhase.attacks;
			if (currPhase.comboAttacks != null && currPhase.comboAttacks.Count > 0 && ((currPhase.allowConsecutiveCombos && this.consecutiveCombos < 3) || this.consecutiveCombos <= 0) && Random.value < currPhase.comboAttackChance)
			{
				list = currPhase.comboAttacks;
				this.consecutiveCombos++;
			}
			else
			{
				this.consecutiveCombos = 0;
			}
			if (list != null && list.Count > 0)
			{
				GREnemyBossMoon.tempPotentialAttacks.Clear();
				for (int j = 0; j < list.Count; j++)
				{
					GREnemyBossMoon.tempPotentialAttacks.Add(list[j]);
				}
				for (int k = GREnemyBossMoon.tempPotentialAttacks.Count - 1; k >= 0; k--)
				{
					GRAbilityBase grabilityBase = this.abilities[(int)GREnemyBossMoon.tempPotentialAttacks[k]];
					if (grabilityBase == null || !this.senseNearby.IsAnyoneNearby(grabilityBase.GetRange(), false) || !grabilityBase.IsCoolDownOver())
					{
						GREnemyBossMoon.tempPotentialAttacks.RemoveAt(k);
					}
				}
				if (GREnemyBossMoon.tempPotentialAttacks.Count > 0)
				{
					this.attacksAfterSummon--;
					int index = Random.Range(0, GREnemyBossMoon.tempPotentialAttacks.Count);
					return GREnemyBossMoon.tempPotentialAttacks[index];
				}
			}
			return GREnemyBossMoon.Behavior.None;
		}
	}

	private bool AreAllEyesClosed()
	{
		for (int i = 0; i < this.eyes.Count; i++)
		{
			if (this.eyes[i].hp > 0)
			{
				return false;
			}
		}
		return true;
	}

	public void GotoDyingIdle()
	{
		this.SetBehavior(GREnemyBossMoon.Behavior.DyingIdle, true);
	}

	private void ChooseNewBehavior(bool forceAttack = false)
	{
		if (this.hp <= 0)
		{
			this.TrySetBehavior(GREnemyBossMoon.Behavior.Dying);
			return;
		}
		if (this.AreAllEyesClosed())
		{
			if (this.eyesPushVolume != null)
			{
				this.eyesPushVolume.Trigger();
			}
			this.TrySetBehavior(GREnemyBossMoon.Behavior.Exposed);
			return;
		}
		if (forceAttack || !this.restAfterAttack)
		{
			this.restAfterAttack = false;
			GREnemyBossMoon.Behavior behavior = this.TryChooseAttackBehavior();
			if (behavior != GREnemyBossMoon.Behavior.None)
			{
				if (this.TrySetBehavior(behavior) && this.currBehavior != GREnemyBossMoon.Behavior.AttackTongue)
				{
					GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
					this.restAfterAttack = currPhase.restAfterAttack;
				}
				if (this.currSummon != null)
				{
					GREnemyBossMoon.PhaseDef currPhase2 = this.GetCurrPhase();
					if (this.trackedEntities.Count < currPhase2.maxSimultaneousEnemies && Random.value < currPhase2.randomSummonChance)
					{
						this.currSummon.ForceSpawn();
					}
				}
				return;
			}
		}
		if (this.currBehavior == GREnemyBossMoon.Behavior.None)
		{
			this.restAfterAttack = false;
			this.TrySetBehavior(GREnemyBossMoon.Behavior.Idle);
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
		if (this.currBehavior == GREnemyBossMoon.Behavior.Runaway)
		{
			this.currAbility.UpdateAuthority(dt);
			return;
		}
		if (this.currBehavior == GREnemyBossMoon.Behavior.ExposedIdle)
		{
			GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
			if (this.hp <= 0)
			{
				this.SetBehavior(GREnemyBossMoon.Behavior.Dying, false);
			}
			else if (this.hp <= currPhase.minHP)
			{
				this.SetBehavior(GREnemyBossMoon.Behavior.AttackTongue, false);
			}
		}
		if (this.currAbility != null)
		{
			this.currAbility.UpdateAuthority(dt);
			GREnemyBossMoon.PhaseDef currPhase2 = this.GetCurrPhase();
			if (this.currAbility.IsDone())
			{
				if (this.currBehavior == GREnemyBossMoon.Behavior.NextPhase)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.AttackTongue, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.Exposed)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.ExposedIdle, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.SummonStart)
				{
					GREnemyBossMoon.Behavior newBehavior = this.ChooseSummonForPhase();
					this.SetBehavior(newBehavior, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.SummonEnd && currPhase2.retreatAfterSummon)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.RetreatStart, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.RetreatStart)
				{
					this.waitInRetreat = 0f;
					this.SetBehavior(GREnemyBossMoon.Behavior.RetreatIdle, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.RetreatIdle)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.RetreatEnd, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.ExposedIdle)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.AttackTongue, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.AttackTongue)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.HiddenIdle, false);
					return;
				}
				if (!this.IsSummon(this.currBehavior))
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.None, false);
					this.ChooseNewBehavior(false);
					return;
				}
				if (currPhase2 == null || this.trackedEntities.Count >= currPhase2.maxSimultaneousEnemies)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.SummonEnd, false);
					return;
				}
				this.SetBehavior(GREnemyBossMoon.Behavior.None, false);
				GREnemyBossMoon.Behavior newBehavior2 = this.ChooseSummonForPhase();
				this.SetBehavior(newBehavior2, false);
				return;
			}
			else if (this.AreAllEyesClosed() && this.currBehavior != GREnemyBossMoon.Behavior.Exposed && this.currBehavior != GREnemyBossMoon.Behavior.ExposedIdle && this.lastBehavior != GREnemyBossMoon.Behavior.Exposed && this.lastBehavior != GREnemyBossMoon.Behavior.ExposedIdle)
			{
				this.TrySetBehavior(GREnemyBossMoon.Behavior.Exposed);
			}
		}
	}

	private void OnUpdateRemote(float dt)
	{
		if (this.currAbility != null)
		{
			this.currAbility.UpdateRemote(dt);
		}
	}

	private void CatchUpPhase(int phase)
	{
		this.BossHasRevealed = true;
		this.internalPhaseIndex = phase;
		this.AdjustByPhaseIndex(phase);
		if (this.adaptiveMusicController != null)
		{
			this.adaptiveMusicController.RestartAt(phase);
		}
	}

	private void IncrementBossPhase()
	{
		this.internalPhaseIndex++;
		this.triggerNextMusicTransition = true;
		this.AdjustByPhaseIndex(this.internalPhaseIndex);
		Debug.Log(string.Format("Incrementing phase to phase {0}!", this.internalPhaseIndex));
	}

	private void SyncPhase(int phase)
	{
		this.internalPhaseIndex = phase;
		if (this.adaptiveMusicController != null)
		{
			this.adaptiveMusicController.GoToTrack(this.internalPhaseIndex, false);
		}
		this.AdjustByPhaseIndex(this.internalPhaseIndex);
		Debug.Log(string.Format("Syncing phase to phase {0}!", this.internalPhaseIndex));
	}

	private void AdjustByPhaseIndex(int phase)
	{
		switch (this.internalPhaseIndex)
		{
		case 1:
			this.abilityIdle.SpeedUp(3f);
			this.AdjustAttackAnimSpeed(1.2f);
			return;
		case 2:
			this.abilityIdle.SpeedUp(4f);
			this.AdjustAttackAnimSpeed(1.4f);
			return;
		case 3:
			this.abilityIdle.SpeedUp(4f);
			this.AdjustAttackAnimSpeed(1.6f);
			return;
		default:
			return;
		}
	}

	private void AdjustAttackAnimSpeed(float speed)
	{
		this.abilityAttackTentacle00.attackAnimData.speed = speed;
		this.abilityAttackTentacle01.attackAnimData.speed = speed;
		this.abilityAttackTentacle02.attackAnimData.speed = speed;
		this.abilityAttackTentacle03.attackAnimData.speed = speed;
		this.abilityAttackTentacle04.attackAnimData.speed = speed;
		this.abilityAttackTentacle05.attackAnimData.speed = speed;
	}

	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		this.HurtBoss(hit.hitAmount, hit.hitEntityId, tool.transform.position);
	}

	private void HurtBoss(int hitAmount, GameEntityId hitByEntityId, Vector3 toolPosition)
	{
		if (this.currBehavior == GREnemyBossMoon.Behavior.Dying || this.currBehavior == GREnemyBossMoon.Behavior.DyingIdle || this.currBehavior == GREnemyBossMoon.Behavior.Runaway || this.IsAnySummonBehavior(this.currBehavior))
		{
			return;
		}
		if (this.currBodyState == GREnemyBossMoon.BodyState.Bones)
		{
			int num = this.hp;
			GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
			this.SetHP(this.hp - hitAmount);
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
				if (hitByEntityId != GameEntityId.Invalid)
				{
					this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hitByEntityId));
				}
				this.SetBodyState(GREnemyBossMoon.BodyState.Destroyed, false);
				this.SetBehavior(GREnemyBossMoon.Behavior.Dying, false);
				return;
			}
			if (num > currPhase.minHP && this.hp <= currPhase.minHP)
			{
				if (currPhase.runawayAfterPhase)
				{
					Debug.Log("Force runaway!");
					if (hitByEntityId != GameEntityId.Invalid)
					{
						this.abilityRunaway.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hitByEntityId));
					}
					this.SetBehavior(GREnemyBossMoon.Behavior.Runaway, false);
				}
				else
				{
					Debug.Log("Force next phase transition!");
					this.SetBehavior(GREnemyBossMoon.Behavior.NextPhase, false);
				}
			}
			this.lastSeenTargetPosition = toolPosition;
			this.lastSeenTargetTime = Time.timeAsDouble;
			Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
			vector.y = 0f;
			this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
		}
	}

	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
	}

	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	public void ReportDeathStat()
	{
		if (this.currAbility != null)
		{
			GRAbilityDie grabilityDie = this.currAbility as GRAbilityDie;
			if (grabilityDie != null)
			{
				grabilityDie.ReportDeathStat();
			}
		}
	}

	private bool IsAttackBehavior(GREnemyBossMoon.Behavior behavior)
	{
		return behavior == GREnemyBossMoon.Behavior.AttackTentacle00 || behavior == GREnemyBossMoon.Behavior.AttackTentacle01 || behavior == GREnemyBossMoon.Behavior.AttackTentacle02 || behavior == GREnemyBossMoon.Behavior.AttackTentacle03 || behavior == GREnemyBossMoon.Behavior.AttackTentacle04 || behavior == GREnemyBossMoon.Behavior.AttackTentacle05 || behavior == GREnemyBossMoon.Behavior.AttackQuickTentacle00 || behavior == GREnemyBossMoon.Behavior.AttackQuickTentacle01 || behavior == GREnemyBossMoon.Behavior.AttackQuickTentacle02 || behavior == GREnemyBossMoon.Behavior.AttackQuickTentacle03 || behavior == GREnemyBossMoon.Behavior.AttackTongue || behavior == GREnemyBossMoon.Behavior.AttackTongueSwipe;
	}

	[CanBeNull]
	private GRAbilityBase GetAssociatedAbilityForBehavior(GREnemyBossMoon.Behavior behavior)
	{
		switch (behavior)
		{
		case GREnemyBossMoon.Behavior.AttackTentacle00:
			return this.abilityAttackTentacle00;
		case GREnemyBossMoon.Behavior.AttackTentacle01:
			return this.abilityAttackTentacle01;
		case GREnemyBossMoon.Behavior.AttackTentacle02:
			return this.abilityAttackTentacle02;
		case GREnemyBossMoon.Behavior.AttackTentacle03:
			return this.abilityAttackTentacle03;
		case GREnemyBossMoon.Behavior.AttackTentacle04:
			return this.abilityAttackTentacle04;
		case GREnemyBossMoon.Behavior.AttackTentacle05:
			return this.abilityAttackTentacle05;
		case GREnemyBossMoon.Behavior.AttackQuickTentacle00:
			return this.abilityAttackQuickTentacle00;
		case GREnemyBossMoon.Behavior.AttackQuickTentacle01:
			return this.abilityAttackQuickTentacle01;
		case GREnemyBossMoon.Behavior.AttackQuickTentacle02:
			return this.abilityAttackQuickTentacle02;
		case GREnemyBossMoon.Behavior.AttackQuickTentacle03:
			return this.abilityAttackQuickTentacle03;
		case GREnemyBossMoon.Behavior.AttackTongue:
			return this.abilityAttackTongue01;
		case GREnemyBossMoon.Behavior.AttackTongueSwipe:
			return this.abilityAttackTongueSwipe01;
		}
		return null;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyBossMoon.BodyState.Destroyed)
		{
			return;
		}
		if (!this.IsAttackBehavior(this.currBehavior))
		{
			return;
		}
		if (collider.isTrigger)
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
			if (component3 == null)
			{
				GorillaTagger component4 = attachedRigidbody.GetComponent<GorillaTagger>();
				if (component4 != null && component4.offlineVRRig != null)
				{
					component3 = component4.offlineVRRig.GetComponent<GRPlayer>();
				}
			}
			if (component3 != null && component3.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
			{
				this.HitPlayer(component3, false);
			}
			GRBreakable component5 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component6 = attachedRigidbody.GetComponent<GameHittable>();
			if (component5 != null && component6 != null)
			{
				GameHitData hitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component6.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component5.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component5.transform.position,
					hittablePoint = component6.FindHittablePoint(collider)
				};
				component6.RequestHit(hitData);
			}
		}
	}

	private void TurnOnGrav()
	{
		if (this.currentGravActivator != null)
		{
			return;
		}
		this.currentGravActivator = this.gravActivators[Random.Range(0, this.gravActivators.Length)];
		this.currentGravActivator.SetActive(true);
	}

	private void TurnOffGrav()
	{
		if (this.currentGravActivator == null)
		{
			return;
		}
		this.currentGravActivator.SetActive(false);
		this.currentGravActivator = null;
	}

	[ContextMenu("Debug Hit Player")]
	private void DebugHitPlayer()
	{
		this.HitPlayer(VRRig.LocalRig.GetComponent<GRPlayer>(), true);
	}

	public void HitPlayer(GRPlayer player, bool useImpulse = false)
	{
		if (this.currBodyState == GREnemyBossMoon.BodyState.Destroyed || this.tryHitPlayerCoroutine != null)
		{
			base.StopCoroutine(this.tryHitPlayerCoroutine);
		}
		this.tryHitPlayerCoroutine = base.StartCoroutine(this.TryHitPlayer(player, useImpulse));
	}

	private IEnumerator TryHitPlayer(GRPlayer player, bool useImpulse = false)
	{
		yield return new WaitForUpdate();
		if (player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			ICustomKnockbackAbility customKnockbackAbility = this.GetAssociatedAbilityForBehavior(this.currBehavior) as ICustomKnockbackAbility;
			Vector3 vector2;
			if (customKnockbackAbility != null)
			{
				Vector3? vector = customKnockbackAbility.CalculateImpulse(player.transform);
				if (vector != null)
				{
					Vector3 valueOrDefault = vector.GetValueOrDefault();
					vector2 = valueOrDefault;
					goto IL_F4;
				}
			}
			vector2 = (player.transform.position - this.knockbackTransform.position).normalized * this.knockbackImpulse;
			IL_F4:
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position, vector2);
			this.cameraShaker.Shake();
			float magnitude = vector2.magnitude;
			GorillaTagger.Instance.StartVibration(true, magnitude, 0.333f);
			GorillaTagger.Instance.StartVibration(false, magnitude, 0.333f);
			if (useImpulse)
			{
				GTPlayer.Instance.ApplyKnockback(vector2 / magnitude, magnitude, true);
			}
		}
		yield break;
	}

	public void ShockPlayer()
	{
		if (this.currBodyState == GREnemyBossMoon.BodyState.Destroyed || this.tryShockPlayerCoroutine != null)
		{
			return;
		}
		this.tryShockPlayerCoroutine = base.StartCoroutine(this.TryShockPlayer());
	}

	private IEnumerator TryShockPlayer()
	{
		this.bodyRenderer.sharedMaterials = this.shockedBodyMaterials;
		yield return new WaitForSecondsRealtime(1f);
		this.bodyRenderer.sharedMaterials = this.defaultBodyMaterials;
		this.tryShockPlayerCoroutine = null;
		yield break;
	}

	private void ToggleShockColliders(bool toggle)
	{
		for (int i = 0; i < this.shockColliders.Count; i++)
		{
			this.shockColliders[i].enabled = toggle;
		}
	}

	public void GroundSlamWeak(Transform slamCenter)
	{
		this._GroundSlam(slamCenter, 0.1f, 6f, 5f);
	}

	public void GroundSlam(Transform slamCenter)
	{
		this._GroundSlam(slamCenter, 1f, 11f, 8f);
	}

	public void _GroundSlam(Transform slamCenter, float duration, float distance, float hitVelocity)
	{
		GREnemyBossMoon.<_GroundSlam>d__173 <_GroundSlam>d__;
		<_GroundSlam>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<_GroundSlam>d__.<>4__this = this;
		<_GroundSlam>d__.slamCenter = slamCenter;
		<_GroundSlam>d__.duration = duration;
		<_GroundSlam>d__.distance = distance;
		<_GroundSlam>d__.hitVelocity = hitVelocity;
		<_GroundSlam>d__.<>1__state = -1;
		<_GroundSlam>d__.<>t__builder.Start<GREnemyBossMoon.<_GroundSlam>d__173>(ref <_GroundSlam>d__);
	}

	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Concat(new string[]
		{
			"<color=\"white\">State:</color> <color=\"yellow\">",
			this.currBehavior.ToString(),
			"</color>\n",
			string.Format("<color=\"white\">Phase:</color> <color=\"yellow\">{0}</color>\n", this.GetCurrPhaseIndex()),
			string.Format("<color=\"white\">HP:</color> <color=\"yellow\">{0}</color>", this.hp)
		}));
	}

	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		byte value2 = (byte)this.currBodyState;
		int value3 = (this.targetPlayer == null) ? -1 : this.targetPlayer.ActorNumber;
		writer.Write(value);
		writer.Write(value2);
		writer.Write(this.hp);
		writer.Write(value3);
		writer.Write(this.internalPhaseIndex);
	}

	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyBossMoon.Behavior newBehavior = (GREnemyBossMoon.Behavior)reader.ReadByte();
		GREnemyBossMoon.BodyState newBodyState = (GREnemyBossMoon.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		int playerID = reader.ReadInt32();
		int num2 = reader.ReadInt32();
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
		if (num2 != -1)
		{
			if (this.internalPhaseIndex == -1)
			{
				Debug.Log(string.Format("Catching up to boss phase {0}.", num2));
				this.CatchUpPhase(num2);
				return;
			}
			if (num2 != this.internalPhaseIndex)
			{
				Debug.Log(string.Format("Syncing up to boss phase {0}.", this.internalPhaseIndex));
				this.SyncPhase(num2);
			}
		}
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

	private void AddTrackedEntity(GameEntity entityToTrack)
	{
		int netId = entityToTrack.GetNetId();
		this.trackedEntities.AddIfNew(netId);
		if (!this.trackedGameEntities.Contains(entityToTrack))
		{
			this.trackedGameEntities.Add(entityToTrack);
		}
	}

	private void RemoveTrackedEntity(GameEntity entityToRemove)
	{
		int netId = entityToRemove.GetNetId();
		if (this.trackedEntities.Contains(netId))
		{
			this.trackedEntities.Remove(netId);
		}
		if (this.trackedGameEntities.Contains(entityToRemove))
		{
			this.trackedGameEntities.Remove(entityToRemove);
		}
	}

	public void OnSummonedEntityInit(GameEntity entity)
	{
		this.AddTrackedEntity(entity);
	}

	public void OnSummonedEntityDestroy(GameEntity entity)
	{
		this.RemoveTrackedEntity(entity);
	}

	public GameEntity entity;

	public GameAgent agent;

	public GREnemy enemy;

	public GameHittable hittable;

	[SerializeField]
	private GRAttributes attributes;

	public List<GREnemyBossMoon.PhaseDef> phases;

	private int internalPhaseIndex = -1;

	public List<GREnemyBossMoon.LootPhase> lootPhases;

	public GRSenseNearby senseNearby;

	public GRSenseLineOfSight senseLineOfSight;

	public List<GREnemyBossMoonEye> eyes;

	public GRSpherePushVolume eyesPushVolume;

	public Animation anim;

	public GRAbilityIdle abilityReveal;

	private bool firstTimeReveal = true;

	public GRAbilityIdle abilityIdle;

	public GRAbilityIdle abilityHiddenIdle;

	public GRBossMoonTentacleAttack abilityAttackTentacle00;

	public GRBossMoonTentacleAttack abilityAttackTentacle01;

	public GRBossMoonTentacleAttack abilityAttackTentacle02;

	public GRBossMoonTentacleAttack abilityAttackTentacle03;

	public GRBossMoonTentacleAttack abilityAttackTentacle04;

	public GRBossMoonTentacleAttack abilityAttackTentacle05;

	public GRBossMoonTentacleAttack abilityAttackQuickTentacle00;

	public GRBossMoonTentacleAttack abilityAttackQuickTentacle01;

	public GRBossMoonTentacleAttack abilityAttackQuickTentacle02;

	public GRBossMoonTentacleAttack abilityAttackQuickTentacle03;

	public GRBossMoonTentacleAttack abilityAttackTongue01;

	public GRBossMoonTentacleAttack abilityAttackTongueSwipe01;

	public GRAbilityIdle abilitySummonStart;

	public GRAbilityIdle abilitySummonEnd;

	public GRAbilitySummon abilitySummon01;

	public GRAbilitySummon abilitySummon02;

	public GRAbilitySummon abilitySummon03;

	public GRAbilitySummon abilitySummon04;

	public GRAbilityIdle abilityRetreatStart;

	public GRAbilityIdle abilityRetreatEnd;

	public GRAbilityIdle abilityRetreatIdle;

	public GRAbilityIdle abilityExposed;

	public GRAbilityIdle abilityExposedIdle;

	public GRAbilityDie abilityDie;

	public GRAbilityDie abilityDieIdle;

	public GRAbilityDie abilityRunaway;

	public GRAbilityIdle abilityNextPhase;

	private GRAbilityBase[] abilities;

	private GRAbilityBase currAbility;

	private GRAbilitySummon currSummon;

	public GRAbilityAgent abilityAgent;

	public List<Renderer> bones;

	public List<Renderer> always;

	public Transform headTransform;

	public AudioSource audioSource;

	public AudioClip damagedSound;

	public float damagedSoundVolume;

	public List<AudioClip> damagedSounds;

	private int damagedSoundIndex;

	public GameObject fxDamaged;

	public GameObject[] gravActivators;

	private GameObject currentGravActivator;

	public Renderer bodyRenderer;

	public Material[] defaultBodyMaterials;

	public Material[] shockedBodyMaterials;

	private float lastStaggerTime;

	public float staggerImmuneTime = 10f;

	private Transform target;

	[ReadOnly]
	public int hp;

	[ReadOnly]
	public GREnemyBossMoon.Behavior currBehavior;

	[ReadOnly]
	public GREnemyBossMoon.BodyState currBodyState;

	[ReadOnly]
	public NetPlayer targetPlayer;

	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	[ReadOnly]
	public double lastSeenTargetTime;

	[ReadOnly]
	public Vector3 searchPosition;

	private GREnemyBossMoon.Behavior lastBehavior;

	private bool restAfterAttack;

	private int consecutiveCombos;

	private int attacksAfterSummon = 3;

	private float waitInRetreat;

	private double lastJumpEndtime;

	public bool canChaseJump = true;

	public float chaseJumpDistance = 5f;

	public float chaseJumpMinInterval = 1f;

	public float minChaseJumpDistance = 2f;

	public float knockbackImpulse = 11f;

	public Transform knockbackTransform;

	private Rigidbody rigidBody;

	private List<Collider> colliders;

	private float lastHitPlayerTime;

	private float minTimeBetweenHits = 2f;

	public float hearingRadius = 5f;

	public List<GREnemyBossMoonColliderHelper> shockColliders;

	public List<GRSquishVolume> squishVolumes;

	public CameraShakeDispatcher cameraShaker;

	private List<int> trackedEntities;

	private List<GameEntity> trackedGameEntities;

	private GRAdaptiveMusicController adaptiveMusicController;

	private bool triggerNextMusicTransition;

	private static List<VRRig> tempRigs = new List<VRRig>(16);

	private static List<GREnemyBossMoon.Behavior> tempPotentialAttacks = new List<GREnemyBossMoon.Behavior>(16);

	private Coroutine tryHitPlayerCoroutine;

	private Coroutine tryShockPlayerCoroutine;

	[Serializable]
	public class PhaseDef
	{
		public int minHP;

		public List<GREnemyBossMoon.Behavior> attacks;

		public List<GREnemyBossMoon.Behavior> comboAttacks;

		public bool restAfterAttack = true;

		public float comboAttackChance = 0.25f;

		public bool allowConsecutiveCombos;

		public List<GREnemyBossMoon.Behavior> summons;

		public int maxSimultaneousEnemies = 6;

		public int maxEnemiesForReveal = 4;

		public int attacksBetweenSummons = 4;

		public bool retreatAfterSummon = true;

		public float randomSummonChance = 0.1f;

		public bool runawayAfterPhase;
	}

	[Serializable]
	public class LootPhase
	{
		public GREnemyType enemyType;

		public GRBreakableItemSpawnConfig lootTable;
	}

	public enum Behavior
	{
		HiddenIdle,
		Idle,
		Reveal,
		Exposed,
		ExposedIdle,
		Stagger,
		Dying,
		AttackTentacle00,
		AttackTentacle01,
		AttackTentacle02,
		AttackTentacle03,
		AttackTentacle04,
		AttackTentacle05,
		AttackQuickTentacle00,
		AttackQuickTentacle01,
		AttackQuickTentacle02,
		AttackQuickTentacle03,
		AttackTongue,
		SummonStart,
		SummonEnd,
		Summon01,
		Summon02,
		Summon03,
		Summon04,
		RetreatStart,
		RetreatEnd,
		RetreatIdle,
		DyingIdle,
		Runaway,
		AttackTongueSwipe,
		NextPhase,
		None,
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
