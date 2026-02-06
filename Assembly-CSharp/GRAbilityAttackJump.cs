using System;
using UnityEngine;

[Serializable]
public class GRAbilityAttackJump : GRAbilityBase
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
		this.startTime = Time.timeAsDouble;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.state = GRAbilityAttackJump.State.Tell;
	}

	protected override void OnStop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	public override bool IsDone()
	{
		return Time.timeAsDouble - this.startTime >= (double)this.duration;
	}

	protected override void OnUpdateShared(float dt)
	{
		double num = (double)((float)Time.timeAsDouble) - this.startTime;
		switch (this.state)
		{
		case GRAbilityAttackJump.State.Tell:
			if (num > (double)this.jumpTime)
			{
				this.targetPos = this.agent.transform.position + this.agent.transform.forward * 0.5f;
				if (this.target != null)
				{
					Vector3 a = this.target.transform.position - this.agent.transform.position;
					this.targetPos = this.agent.transform.position + a * this.jumpLengthScale;
					this.targetPos.y = this.target.transform.position.y;
				}
				float num2 = this.attackLandTime - this.jumpTime;
				num2 = Mathf.Max(0.1f, num2);
				this.initialPos = this.agent.transform.position;
				Vector3 vector = this.targetPos - this.initialPos;
				float y = vector.y;
				vector.y = 0f;
				float num3 = num2;
				float y2 = 0f;
				if (num3 > 0f)
				{
					Vector3 gravity = Physics.gravity;
					y2 = (y - 0.5f * gravity.y * num3 * num3) / num3;
				}
				this.initialVel = vector / num2;
				this.initialVel.y = y2;
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(true);
				}
				this.PlayAnim(this.jumpAnimName, 0.1f, this.animSpeed);
				this.jumpSound.Play(null);
				this.state = GRAbilityAttackJump.State.Jump;
			}
			break;
		case GRAbilityAttackJump.State.Jump:
		{
			float d = (float)(num - (double)this.jumpTime);
			Vector3 position = this.initialPos + this.initialVel * d + 0.5f * Physics.gravity * d * d;
			this.root.position = position;
			if (num > (double)this.attackLandTime)
			{
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(false);
				}
				if (this.doReturnPhase)
				{
					float num4 = this.attackReturnTime - this.attackLandTime;
					num4 = Mathf.Max(0.1f, num4);
					Vector3 a2 = this.initialPos;
					this.initialPos = this.agent.transform.position;
					this.initialVel = (a2 - this.initialPos) / num4;
					this.state = GRAbilityAttackJump.State.Return;
				}
				else
				{
					this.state = GRAbilityAttackJump.State.Done;
				}
			}
			break;
		}
		case GRAbilityAttackJump.State.Return:
		{
			float d2 = (float)(num - (double)this.attackLandTime);
			Vector3 position2 = this.initialPos + this.initialVel * d2;
			this.root.position = position2;
			if (num > (double)this.attackReturnTime)
			{
				this.state = GRAbilityAttackJump.State.Done;
			}
			break;
		}
		}
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
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

	public float duration;

	public float jumpTime;

	public float attackLandTime;

	public float attackReturnTime;

	public bool doReturnPhase = true;

	public float jumpLengthScale = 1f;

	public string animName;

	public float animSpeed;

	public float maxTurnSpeed;

	public string jumpAnimName;

	public AbilitySound jumpSound;

	public GameObject damageTrigger;

	private Transform target;

	private GRAbilityAttackJump.State state;

	public Vector3 targetPos;

	public Vector3 initialPos;

	public Vector3 initialVel;

	private enum State
	{
		Tell,
		Jump,
		Return,
		Done
	}
}
