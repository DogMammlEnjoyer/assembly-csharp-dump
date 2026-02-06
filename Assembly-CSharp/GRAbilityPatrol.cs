using System;
using CjLib;
using Photon.Pun;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class GRAbilityPatrol : GRAbilityBase
{
	public bool HasValidPatrolPath()
	{
		return this.patrolPath != null && this.patrolPath.patrolNodes.Count > 1;
	}

	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.moveAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
		if (this.attributes && this.moveAbility.moveSpeed == 0f)
		{
			this.moveAbility.moveSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
		}
		this.navMeshAgent = agent.GetComponent<NavMeshAgent>();
		this.InitializeRandoms();
		this.nextPatrolNode = 0;
	}

	private void InitializeRandoms()
	{
		this.patrolGroanSoundDelayRandom = new Unity.Mathematics.Random((uint)this.entity.GetNetId());
		this.patrolGroanSoundRandom = new Unity.Mathematics.Random((uint)this.entity.GetNetId());
	}

	protected override void OnStart()
	{
		this.moveAbility.Start();
		this.agent.SetIsPathing(true, true);
		if (this.patrolPath != null)
		{
			this.moveAbility.SetTarget(this.patrolPath.patrolNodes[this.nextPatrolNode]);
		}
		else
		{
			Debug.LogError("Starting patrol ability with no patrol path");
		}
		this.CalculateNextPatrolGroan();
	}

	protected override void OnStop()
	{
		this.moveAbility.Stop();
	}

	public override bool IsDone()
	{
		return false;
	}

	public void SetPatrolPath(GRPatrolPath patrolPath)
	{
		this.patrolPath = patrolPath;
	}

	public GRPatrolPath GetPatrolPath()
	{
		return this.patrolPath;
	}

	public void SetNextPatrolNode(int nextPatrolNode)
	{
		this.nextPatrolNode = nextPatrolNode;
	}

	public void CalculateNextPatrolGroan()
	{
		this.nextPatrolGroanTime = this.patrolGroanSoundDelayRandom.NextDouble(this.ambientSoundDelayMin, this.ambientSoundDelayMax) + PhotonNetwork.Time;
	}

	private void PlayPatrolGroan()
	{
		this.audioSource.clip = this.ambientPatrolSounds[this.patrolGroanSoundRandom.NextInt(this.ambientPatrolSounds.Length - 1)];
		this.audioSource.volume = this.ambientSoundVolume;
		this.audioSource.Play();
		this.CalculateNextPatrolGroan();
	}

	protected override void OnUpdateAuthority(float dt)
	{
		this.moveAbility.UpdateAuthority(dt);
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.root.position, this.moveAbility.GetTargetPos(), Color.green, true);
		}
		if (this.moveAbility.IsDone())
		{
			this.nextPatrolNode = (this.nextPatrolNode + 1) % this.patrolPath.patrolNodes.Count;
			this.moveAbility.SetTarget(this.patrolPath.patrolNodes[this.nextPatrolNode]);
		}
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	protected override void OnUpdateRemote(float dt)
	{
		this.moveAbility.SetTarget(null);
		this.moveAbility.SetTargetPos(this.agent.navAgent.destination);
		this.moveAbility.UpdateRemote(dt);
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.root.position, this.moveAbility.GetTargetPos(), Color.green, true);
		}
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	private NavMeshAgent navMeshAgent;

	public GRAbilityMoveToTarget moveAbility;

	private GRPatrolPath patrolPath;

	public double lastStateChange;

	public float ambientSoundVolume = 0.5f;

	public double ambientSoundDelayMin = 5.0;

	public double ambientSoundDelayMax = 10.0;

	public AudioClip[] ambientPatrolSounds;

	private double lastPartrolAmbientSoundTime;

	private double nextPatrolGroanTime;

	private Unity.Mathematics.Random patrolGroanSoundDelayRandom;

	private Unity.Mathematics.Random patrolGroanSoundRandom;

	[ReadOnly]
	public int nextPatrolNode;
}
