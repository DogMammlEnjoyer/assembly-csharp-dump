using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class GRAbilityAttackLaser : GRAbilityBase
{
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	protected override void OnStart()
	{
		if (this.animData.Count > 0)
		{
			this.lastAnimIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.animData.Count, this.lastAnimIndex);
			this.duration = this.animData[this.lastAnimIndex].duration;
			this.PlayAnim(this.animData[this.lastAnimIndex].animName, 0.1f, this.animData[this.lastAnimIndex].speed);
			this.animNameString = this.animData[this.lastAnimIndex].animName;
		}
		else
		{
			this.duration = 0.5f;
		}
		this.soundAttack.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundAttack.Play(null);
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
		this.state = GRAbilityAttackLaser.State.Tell;
	}

	protected override void OnStop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
		if (this.laserFx != null)
		{
			this.laserFx.DisableLazer();
		}
		if (this.tellLaserFx != null)
		{
			this.tellLaserFx.DisableLazer();
		}
	}

	public override bool IsDone()
	{
		return this.state == GRAbilityAttackLaser.State.Done;
	}

	protected override void OnUpdateShared(float dt)
	{
		float num = (float)(Time.timeAsDouble - this.startTime);
		switch (this.state)
		{
		case GRAbilityAttackLaser.State.Tell:
		{
			this.targetPos = this.root.position + this.root.transform.forward;
			if (this.target != null)
			{
				this.targetPos = this.target.position;
			}
			Vector3 position = this.head.position;
			Vector3 a = this.targetPos - position;
			float num2 = a.magnitude;
			if (num2 > 0f)
			{
				Vector3 a2 = a / num2;
				num2 = Mathf.Min(this.maxLaserRange, num2);
				this.targetPos = position + a2 * num2;
			}
			if (!this.doNotFaceTarget)
			{
				GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
			}
			if (num > this.tellDuration)
			{
				this.state = GRAbilityAttackLaser.State.Attack;
				if (this.damageCollider != null && this.laserOrigins.Length != 0)
				{
					this.damageCollider.transform.position = (position + this.targetPos) / 2f;
					this.damageCollider.height = num2;
					this.damageCollider.direction = 2;
					if (num2 > 0f)
					{
						this.damageCollider.transform.rotation = Quaternion.LookRotation(a / num2);
					}
				}
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(true);
				}
				if (this.tellLaserFx != null)
				{
					this.tellLaserFx.DisableLazer();
				}
				if (this.laserFx != null && this.target != null)
				{
					GamePlayer component = this.target.GetComponent<GamePlayer>();
					if (component != null && component.rig != null)
					{
						this.laserFx.EnableLazer(this.laserOrigins, this.targetPos);
					}
				}
				this.initialPos = this.root.position;
				this.initialVel = (this.targetPos - this.initialPos).normalized * this.attackMoveSpeed;
				return;
			}
			if (this.tellLaserFx != null)
			{
				this.tellLaserFx.EnableLazer(this.laserOrigins, this.targetPos);
				return;
			}
			break;
		}
		case GRAbilityAttackLaser.State.Attack:
		{
			float d = num - this.tellDuration;
			Vector3 vector = this.initialPos + this.initialVel * d;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(vector, out navMeshHit, 0.5f, this.walkableArea))
			{
				vector = navMeshHit.position;
				if (NavMesh.Raycast(this.initialPos, vector, out navMeshHit, this.walkableArea))
				{
					vector = navMeshHit.position;
				}
				this.root.position = vector;
			}
			if (num > this.tellDuration + this.attackDuration)
			{
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(false);
				}
				if (this.laserFx != null)
				{
					this.laserFx.DisableLazer();
				}
				this.state = GRAbilityAttackLaser.State.FollowThrough;
				return;
			}
			break;
		}
		case GRAbilityAttackLaser.State.FollowThrough:
			if (num >= this.duration)
			{
				this.state = GRAbilityAttackLaser.State.Done;
			}
			break;
		default:
			return;
		}
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
			}
		}
	}

	public string GetAnimName()
	{
		return this.animNameString;
	}

	public override bool IsCoolDownOver()
	{
		return base.IsCoolDownOver(this.coolDown);
	}

	public override float GetRange()
	{
		return this.range;
	}

	public float duration;

	public float tellDuration;

	public float attackDuration;

	public float coolDown;

	public float range;

	public float attackMoveSpeed;

	public bool doNotFaceTarget;

	public List<AnimationData> animData;

	public AbilitySound soundAttack;

	public float maxLaserRange;

	public Transform[] laserOrigins;

	public Monkeye_LazerFX tellLaserFx;

	public Monkeye_LazerFX laserFx;

	private GRAbilityAttackLaser.State state;

	public float maxTurnSpeed;

	public GameObject damageTrigger;

	public CapsuleCollider damageCollider;

	private Transform target;

	private string animNameString;

	private int lastAnimIndex = -1;

	public Vector3 targetPos;

	public Vector3 initialPos;

	public Vector3 initialVel;

	private enum State
	{
		Tell,
		Attack,
		FollowThrough,
		Done
	}
}
