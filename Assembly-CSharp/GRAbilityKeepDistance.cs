using System;
using CjLib;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class GRAbilityKeepDistance : GRAbilityBase
{
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.navMeshAgent = agent.GetComponent<NavMeshAgent>();
		this.moveAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
		if (this.attributes && this.moveAbility.moveSpeed == 0f)
		{
			this.moveAbility.moveSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.BackupSpeed);
		}
	}

	protected override void OnStart()
	{
		if (this.target != null)
		{
			Vector3 vector = this.agent.transform.position - this.target.position;
			if (this.maxDistanceFromTarget > 0f && vector.magnitude > this.maxDistanceFromTarget)
			{
				this.agent.SetStopped(true);
				this.PlayAnim(this.idleAnimName, 0.5f, 1f);
				this.idleSound.Play(null);
			}
			else
			{
				this.moveAbility.Start();
			}
		}
		else
		{
			this.moveAbility.Start();
		}
		this.agent.SetIsPathing(true, true);
		Vector3 targetPos = this.PickBackupDestination();
		this.moveAbility.SetTargetPos(targetPos);
		if (this.navMeshAgent != null)
		{
			this.defaultUpdateRotation = this.navMeshAgent.updateRotation;
			this.navMeshAgent.updateRotation = false;
		}
	}

	protected override void OnStop()
	{
		this.moveAbility.Stop();
		this.idleSound.Stop();
		if (this.navMeshAgent != null)
		{
			this.navMeshAgent.updateRotation = this.defaultUpdateRotation;
		}
		this.agent.SetStopped(false);
	}

	public override bool IsDone()
	{
		return false;
	}

	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.target = null;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				this.target = grplayer.transform;
				this.moveAbility.SetLookAtTarget(this.target);
			}
		}
	}

	protected override void OnThink(float dt)
	{
		Vector3 vector = this.agent.transform.position - this.target.position;
		if (this.moveAbility.IsDone())
		{
			if (this.maxDistanceFromTarget < 0f || vector.magnitude < this.maxDistanceFromTarget)
			{
				if (this.navMeshAgent != null && this.navMeshAgent.isOnNavMesh && this.navMeshAgent.isStopped)
				{
					this.idleSound.Stop();
					this.moveAbility.Start();
				}
				Vector3 targetPos = this.PickBackupDestination();
				this.moveAbility.SetTargetPos(targetPos);
				return;
			}
		}
		else if (this.maxDistanceFromTarget > 0f && vector.magnitude > this.maxDistanceFromTarget)
		{
			this.moveAbility.SetTargetPos(this.root.position);
			this.moveAbility.Stop();
			this.agent.SetStopped(true);
			this.PlayAnim(this.idleAnimName, 0.5f, 1f);
			this.idleSound.Play(null);
		}
	}

	private Vector3 PickBackupDestination()
	{
		Vector3 position = this.agent.transform.position;
		if (this.target == null)
		{
			return position;
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(position, out navMeshHit, 1f, this.walkableArea))
		{
			Vector3 position2 = navMeshHit.position;
			Vector3 vector = this.agent.transform.position - this.target.position;
			vector.y = 0f;
			Vector3 normalized = vector.normalized;
			int i = 0;
			while (i < GRAbilityKeepDistance.rotations.Length)
			{
				Vector3 a = GRAbilityKeepDistance.rotations[i] * normalized;
				float d = 2f;
				Vector3 vector2 = position2 + a * d;
				NavMeshHit navMeshHit2;
				if (!NavMesh.Raycast(position2, vector2, out navMeshHit2, this.walkableArea))
				{
					goto IL_D6;
				}
				if (navMeshHit2.distance >= this.minBackupSpaceRequired)
				{
					vector2 = navMeshHit2.position;
					goto IL_D6;
				}
				IL_128:
				i++;
				continue;
				IL_D6:
				NavMeshHit navMeshHit3;
				if (!NavMesh.SamplePosition(vector2, out navMeshHit3, 1f, this.walkableArea))
				{
					goto IL_128;
				}
				Vector3 position3 = navMeshHit3.position;
				Vector3 vector3 = position3 - this.target.position;
				vector3.y = 0f;
				if (vector3.sqrMagnitude > vector.sqrMagnitude)
				{
					return position3;
				}
				goto IL_128;
			}
		}
		return position;
	}

	protected override void OnUpdateShared(float dt)
	{
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.root.position, this.moveAbility.GetTargetPos(), Color.magenta, true);
		}
	}

	protected override void OnUpdateAuthority(float dt)
	{
		this.moveAbility.UpdateAuthority(dt);
	}

	protected override void OnUpdateRemote(float dt)
	{
		this.moveAbility.UpdateRemote(dt);
	}

	private NavMeshAgent navMeshAgent;

	private Transform target;

	public GRAbilityMoveToTarget moveAbility;

	public string idleAnimName;

	public AbilitySound idleSound;

	public float minBackupSpaceRequired = 0.5f;

	public float maxDistanceFromTarget = -1f;

	private bool defaultUpdateRotation;

	private static Quaternion[] rotations = new Quaternion[]
	{
		Quaternion.Euler(0f, 0f, 0f),
		Quaternion.Euler(0f, 30f, 0f),
		Quaternion.Euler(0f, -30f, 0f),
		Quaternion.Euler(0f, 60f, 0f),
		Quaternion.Euler(0f, -60f, 0f),
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, -90f, 0f),
		Quaternion.Euler(0f, 135f, 0f),
		Quaternion.Euler(0f, -135f, 0f),
		Quaternion.Euler(0f, 180f, 0f)
	};
}
