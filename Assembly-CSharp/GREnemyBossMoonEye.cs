using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

public class GREnemyBossMoonEye : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent
{
	private void Awake()
	{
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		if (this.navAgent != null)
		{
			this.navAgent.updateRotation = false;
		}
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.abilities = new GRAbilityBase[8];
	}

	public void OnEntityInit()
	{
		this.currBehavior = GREnemyBossMoonEye.Behavior.None;
		this.currAbility = null;
		this.SetupAbility(GREnemyBossMoonEye.Behavior.Idle, this.abilityIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.AttackLaser, this.abilityAttackLaser, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.Closed, this.abilityClosed, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.GravityStart, this.abilityGravityStart, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.GravityEnd, this.abilityGravityEnd, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.GravityIdle, this.abilityGravityIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.Dying, this.abilityDie, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.senseNearby.Setup(this.headTransform, this.entity);
		this.Setup(this.entity.createData);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry entry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		if (this.agent.navAgent != null)
		{
			this.agent.navAgent.autoTraverseOffMeshLink = false;
		}
		int maxHP = this.CalcMaxHP();
		if (this.enemy != null)
		{
			this.enemy.SetMaxHP(maxHP);
		}
		this.SetHP(maxHP);
		this.SetBehavior(GREnemyBossMoonEye.Behavior.Idle, true);
	}

	private void SetupAbility(GREnemyBossMoonEye.Behavior behavior, GRAbilityBase ability, GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
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
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	public void Setup(long entityCreateData)
	{
		this.SetBehavior(GREnemyBossMoonEye.Behavior.Idle, true);
	}

	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 8)
		{
			return;
		}
		this.SetBehavior((GREnemyBossMoonEye.Behavior)newState, false);
	}

	public void ResetEye()
	{
		if (this.entity.IsAuthority())
		{
			this.SetBehavior(GREnemyBossMoonEye.Behavior.Idle, false);
		}
	}

	public void SetHP(int hp)
	{
		this.hp = hp;
		if (this.enemy != null)
		{
			this.enemy.SetHP(hp);
		}
	}

	public bool TrySetBehavior(GREnemyBossMoonEye.Behavior newBehavior)
	{
		this.SetBehavior(newBehavior, false);
		return true;
	}

	private void SetBehavior(GREnemyBossMoonEye.Behavior newBehavior, bool force = false)
	{
		if (this.abilities == null)
		{
			Debug.LogError("Abilities have not been initialized", this);
			return;
		}
		if (newBehavior < GREnemyBossMoonEye.Behavior.Idle || newBehavior >= (GREnemyBossMoonEye.Behavior)this.abilities.Length)
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
		Debug.LogFormat("Boss Eye SetBehavior {0} -> {1}", new object[]
		{
			this.currBehavior,
			newBehavior
		});
		if (this.currAbility != null)
		{
			this.currAbility.Stop();
		}
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.Closed)
		{
			this.SetHP(this.CalcMaxHP());
		}
		this.currBehavior = newBehavior;
		this.currAbility = grabilityBase;
		if (this.currAbility != null)
		{
			this.currAbility.Start();
		}
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.AttackLaser)
		{
			this.abilityAttackLaser.SetTargetPlayer(this.agent.targetPlayer);
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

	private void RefreshBody()
	{
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
		GREnemyBossMoonEye.tempRigs.Clear();
		GREnemyBossMoonEye.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyBossMoonEye.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyBossMoonEye.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		if (this.currAbility != null)
		{
			this.currAbility.Think(dt);
		}
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.Idle)
		{
			this.ChooseNewBehavior();
		}
	}

	private bool TryChooseAttackBehavior()
	{
		if (Time.timeAsDouble > this.lastHitTime + (double)this.counterAttackWindow)
		{
			return false;
		}
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.Closed)
		{
			return false;
		}
		GREnemyBossMoonEye.tempPotentialAttacks.Clear();
		if (this.allowLaserAttack)
		{
			GREnemyBossMoonEye.tempPotentialAttacks.Add(GREnemyBossMoonEye.Behavior.AttackLaser);
		}
		for (int i = GREnemyBossMoonEye.tempPotentialAttacks.Count - 1; i >= 0; i--)
		{
			GRAbilityBase grabilityBase = this.abilities[(int)GREnemyBossMoonEye.tempPotentialAttacks[i]];
			if (grabilityBase == null || !this.senseNearby.IsAnyoneNearby(grabilityBase.GetRange(), false) || !grabilityBase.IsCoolDownOver())
			{
				GREnemyBossMoonEye.tempPotentialAttacks.RemoveAt(i);
			}
		}
		if (GREnemyBossMoonEye.tempPotentialAttacks.Count <= 0)
		{
			return false;
		}
		int index = Random.Range(0, GREnemyBossMoonEye.tempPotentialAttacks.Count);
		this.SetBehavior(GREnemyBossMoonEye.tempPotentialAttacks[index], false);
		return true;
	}

	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.TryChooseAttackBehavior())
		{
			return;
		}
		this.TrySetBehavior(GREnemyBossMoonEye.Behavior.Idle);
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
		if (this.currAbility != null)
		{
			this.currAbility.UpdateAuthority(dt);
			if (this.currAbility.IsDone())
			{
				this.SetBehavior(GREnemyBossMoonEye.Behavior.None, false);
				this.ChooseNewBehavior();
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

	public void InstantKill()
	{
		if (this.hp <= 0)
		{
			return;
		}
		this.SetHP(0);
		this.lastHitTime = Time.timeAsDouble;
		if (this.entity.IsAuthority())
		{
			this.SetBehavior(GREnemyBossMoonEye.Behavior.Closed, false);
		}
	}

	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.Dying)
		{
			return;
		}
		this.SetHP(this.hp - hit.hitAmount);
		this.lastHitTime = Time.timeAsDouble;
		if (this.hp <= 0)
		{
			this.hp = 0;
			if (this.entity.IsAuthority())
			{
				this.SetBehavior(GREnemyBossMoonEye.Behavior.Closed, false);
				return;
			}
		}
		else
		{
			this.lastSeenTargetPosition = tool.transform.position;
			this.lastSeenTargetTime = Time.timeAsDouble;
		}
	}

	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBehavior != GREnemyBossMoonEye.Behavior.AttackLaser)
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
		if (player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
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
	}

	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		int value2 = (this.targetPlayer == null) ? -1 : this.targetPlayer.ActorNumber;
		writer.Write(value);
		writer.Write(this.hp);
		writer.Write(value2);
	}

	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyBossMoonEye.Behavior newBehavior = (GREnemyBossMoonEye.Behavior)reader.ReadByte();
		int num = reader.ReadInt32();
		int playerID = reader.ReadInt32();
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
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
			if (hitTypeId == GameHitType.Club)
			{
				this.OnHitByClub(gameComponent, hit);
				return;
			}
			if (hitTypeId != GameHitType.Shield)
			{
				return;
			}
			this.OnHitByShield(gameComponent, hit);
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

	private GRAbilityBase[] abilities;

	private GRAbilityBase currAbility;

	public GRAbilityAgent abilityAgent;

	public GRAbilityIdle abilityIdle;

	public GRAbilityIdle abilityClosed;

	public GRAbilityAttackLaser abilityAttackLaser;

	public GRAbilityDie abilityDie;

	public GRAbilityIdle abilityGravityStart;

	public GRAbilityIdle abilityGravityEnd;

	public GRAbilityIdle abilityGravityIdle;

	public Transform headTransform;

	public NavMeshAgent navAgent;

	public AudioSource audioSource;

	public float counterAttackWindow = 3f;

	private Transform target;

	[ReadOnly]
	public int hp;

	[ReadOnly]
	public GREnemyBossMoonEye.Behavior currBehavior;

	[ReadOnly]
	public GREnemyBossMoonEye.BodyState currBodyState;

	[ReadOnly]
	public NetPlayer targetPlayer;

	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	[ReadOnly]
	public double lastSeenTargetTime;

	public bool allowLaserAttack;

	public bool canChaseJump = true;

	public float chaseJumpDistance = 5f;

	public float chaseJumpMinInterval = 1f;

	public float minChaseJumpDistance = 2f;

	private double lastHitTime;

	private List<Collider> colliders;

	private float lastHitPlayerTime;

	private float minTimeBetweenHits = 0.5f;

	public float hearingRadius = 5f;

	public int maxSimultaneousSummonedEntities = 6;

	private static List<VRRig> tempRigs = new List<VRRig>(16);

	private static List<GREnemyBossMoonEye.Behavior> tempPotentialAttacks = new List<GREnemyBossMoonEye.Behavior>(16);

	private Coroutine tryHitPlayerCoroutine;

	public enum Behavior
	{
		Idle,
		AttackLaser,
		Closed,
		GravityStart,
		GravityEnd,
		GravityIdle,
		Dying,
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
