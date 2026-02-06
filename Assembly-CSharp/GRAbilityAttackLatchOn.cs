using System;
using UnityEngine;

[Serializable]
public class GRAbilityAttackLatchOn : GRAbilityBase
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
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.agent.SetSpeed(this.tellMoveSpeed);
		this.startTime = Time.timeAsDouble;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	protected override void OnStop()
	{
		this.agent.transform.SetParent(null);
		this.agent.SetIsPathing(true, true);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	public override bool IsDone()
	{
		return Time.timeAsDouble - this.startTime >= (double)this.duration;
	}

	protected override void OnUpdateAuthority(float dt)
	{
		this.UpdateNavSpeed();
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
	}

	protected override void OnUpdateRemote(float dt)
	{
		this.UpdateNavSpeed();
	}

	private void UpdateNavSpeed()
	{
		if (Time.timeAsDouble - this.startTime > (double)this.tellDuration)
		{
			this.agent.SetSpeed(this.attackMoveSpeed);
			this.agent.SetVelocity(this.agent.navAgent.velocity.normalized * this.attackMoveSpeed);
			if (this.damageTrigger != null)
			{
				this.damageTrigger.SetActive(true);
			}
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
				this.agent.transform.SetParent(grplayer.attachEnemy);
				this.agent.transform.localPosition = Vector3.zero;
				this.agent.transform.localRotation = Quaternion.identity;
				this.agent.SetIsPathing(false, true);
			}
		}
	}

	public float duration;

	public float attackMoveSpeed;

	public float tellDuration;

	public float tellMoveSpeed;

	public string animName;

	public float animSpeed;

	public float maxTurnSpeed;

	public Transform target;

	public GameObject damageTrigger;
}
