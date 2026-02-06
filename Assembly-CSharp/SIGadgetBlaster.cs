using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
[RequireComponent(typeof(SIGadgetBlasterType))]
public class SIGadgetBlaster : SIGadget, ITickSystemTick
{
	public bool LocalEquippedOrActivated
	{
		get
		{
			return this.IsEquippedLocal() || this.activatedLocally;
		}
	}

	public bool TickRunning { get; set; }

	protected override void OnEnable()
	{
		base.OnEnable();
		this.blasterType = base.GetComponent<SIGadgetBlasterType>();
		this.lastFired = 0f;
		this.environmentLayerMask = GTPlayer.Instance.locomotionEnabledLayers;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.StartGrabbing));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.StartGrabbing));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.StopGrabbing));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.StopGrabbing));
		TickSystem<object>.AddTickCallback(this);
	}

	private new void OnDisable()
	{
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
	}

	public void Tick()
	{
		if (this.projectilesToDespawn.Count <= 0)
		{
			return;
		}
		if (Time.time < this.projectilesToDespawnTimes.Peek() + 1f)
		{
			return;
		}
		SIGadgetBlasterProjectile sigadgetBlasterProjectile = this.projectilesToDespawn.Dequeue();
		this.activeProjectiles.RemoveIfContains(sigadgetBlasterProjectile);
		if (sigadgetBlasterProjectile == null || sigadgetBlasterProjectile.gameObject == null)
		{
			return;
		}
		SIGadgetBlaster.blasterProjectilePools[sigadgetBlasterProjectile.poolId].Add(sigadgetBlasterProjectile.gameObject);
	}

	protected override void OnUpdateAuthority(float dt)
	{
		base.OnUpdateAuthority(dt);
		this.blasterType.OnUpdateAuthority(dt);
	}

	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetBlasterState sigadgetBlasterState = (SIGadgetBlasterState)this.gameEntity.GetState();
		if (sigadgetBlasterState != this.currentState)
		{
			this.SetStateShared(sigadgetBlasterState);
		}
		this.blasterType.OnUpdateRemote(dt);
	}

	public void SetStateAuthority(SIGadgetBlasterState newState)
	{
		this.SetStateShared(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	private void SetStateShared(SIGadgetBlasterState newState)
	{
		if (newState == this.currentState || !SIGadgetBlaster.CanChangeState((long)newState))
		{
			return;
		}
		SIGadgetBlasterState sigadgetBlasterState = this.currentState;
		this.currentState = newState;
		this.blasterType.SetStateShared();
	}

	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.blasterType.ApplyUpgradeNodes(withUpgrades);
	}

	public static bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L;
	}

	public bool CheckInput()
	{
		float sensitivity = this.wasActivated ? this.inputActivateThreshold : this.inputDeactivateThreshold;
		this.wasActivated = this.buttonActivatable.CheckInput(true, true, sensitivity, true, true);
		return this.wasActivated;
	}

	public int NextFireId()
	{
		int num = this.projectileId;
		this.projectileId = num + 1;
		return num;
	}

	public override void ProcessClientToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID != 0)
		{
			if (rpcID != 1)
			{
				return;
			}
			if (data == null || data.Length < 2)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			SIGadgetBlasterProjectile sigadgetBlasterProjectile = null;
			for (int i = 0; i < this.activeProjectiles.Count; i++)
			{
				if (this.activeProjectiles[i].projectileId == num)
				{
					sigadgetBlasterProjectile = this.activeProjectiles[i];
					break;
				}
			}
			if (sigadgetBlasterProjectile == null)
			{
				return;
			}
			if (sigadgetBlasterProjectile.firedByPlayer != SIPlayer.Get(info.Sender.ActorNumber))
			{
				return;
			}
			sigadgetBlasterProjectile.GetComponent<SIGadgetProjectileType>().NetworkedProjectileHit(data);
			return;
		}
		else
		{
			if (data == null || data.Length == 0)
			{
				return;
			}
			if (!this.gameEntity.IsAttachedToPlayer(NetPlayer.Get(info.Sender)))
			{
				return;
			}
			this.blasterType.NetworkFireProjectile(data);
			return;
		}
	}

	public void StartGrabbing()
	{
		if (this.IsEquippedLocal() || this.activatedLocally)
		{
			this.SetStateAuthority(SIGadgetBlasterState.Idle);
		}
	}

	public void StopGrabbing()
	{
		this.SetStateShared(SIGadgetBlasterState.Idle);
	}

	public void DespawnProjectile(SIGadgetBlasterProjectile projectile)
	{
		projectile.gameObject.SetActive(false);
		if (!this.projectilesToDespawn.Contains(projectile))
		{
			this.projectilesToDespawn.Enqueue(projectile);
			this.projectilesToDespawnTimes.Enqueue(Time.time);
		}
	}

	public GameObject InstantiateProjectile(SIGadgetBlasterProjectile projectilePrefab, Vector3 position, Quaternion rotation, int thisFireId)
	{
		if (SIGadgetBlaster.blasterProjectilePools == null)
		{
			SIGadgetBlaster.blasterProjectilePools = new Dictionary<int, List<GameObject>>();
		}
		int instanceID = projectilePrefab.GetInstanceID();
		if (!SIGadgetBlaster.blasterProjectilePools.ContainsKey(instanceID))
		{
			SIGadgetBlaster.blasterProjectilePools.Add(instanceID, new List<GameObject>());
		}
		List<GameObject> list = SIGadgetBlaster.blasterProjectilePools[instanceID];
		GameObject gameObject;
		if (list.Count <= 0)
		{
			gameObject = Object.Instantiate<GameObject>(projectilePrefab.gameObject, position, rotation);
		}
		else
		{
			gameObject = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			gameObject.SetActive(true);
		}
		SIGadgetBlasterProjectile component = gameObject.GetComponent<SIGadgetBlasterProjectile>();
		component.transform.position = position;
		component.transform.rotation = rotation;
		component.parentBlaster = this;
		component.projectileId = thisFireId;
		component.firedByPlayer = (this.gameEntity.IsHeld() ? SIPlayer.Get(this.gameEntity.heldByActorNumber) : SIPlayer.Get(this.gameEntity.snappedByActorNumber));
		component.poolId = instanceID;
		this.activeProjectiles.Add(component);
		this.lastFired = Time.time;
		component.InitializeProjectile();
		return gameObject;
	}

	public void FireProjectileHaptics(float strength, float duration)
	{
		GorillaTagger.Instance.StartVibration(this.gameEntity.EquippedHandedness == EHandedness.Left, strength, duration);
	}

	public float CurrentFireRate()
	{
		int count = this.activeProjectiles.Count;
		if (count <= 1)
		{
			return 0f;
		}
		return (float)(count - 1) / (this.activeProjectiles[count - 1].timeSpawned - this.activeProjectiles[0].timeSpawned);
	}

	[OnEnterPlay_SetNull]
	public static Dictionary<int, List<GameObject>> blasterProjectilePools;

	[NonSerialized]
	public const float PROJECTILE_MAX_LATENCY = 1f;

	private SIGadgetBlasterType blasterType;

	[NonSerialized]
	public SIGadgetBlasterState currentState;

	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	[SerializeField]
	private float inputActivateThreshold = 0.35f;

	[SerializeField]
	private float inputDeactivateThreshold = 0.25f;

	public int maxProjectileCount = 10;

	public float maxLagDistance = 5f;

	private bool wasActivated;

	[NonSerialized]
	public float lastFired;

	[NonSerialized]
	public int projectileCount;

	private int projectileId;

	[NonSerialized]
	public List<SIGadgetBlasterProjectile> activeProjectiles = new List<SIGadgetBlasterProjectile>();

	[NonSerialized]
	public Queue<SIGadgetBlasterProjectile> projectilesToDespawn = new Queue<SIGadgetBlasterProjectile>();

	[NonSerialized]
	public Queue<float> projectilesToDespawnTimes = new Queue<float>();

	public Transform firingPosition;

	public AudioSource firingSource;

	public AudioSource blasterSource;

	[NonSerialized]
	public LayerMask environmentLayerMask;

	public enum RPCCalls
	{
		FireProjectile,
		ProjectileHitPlayer
	}
}
