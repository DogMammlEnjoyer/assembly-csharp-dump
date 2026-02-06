using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GRAbilityAttackSimple : GRAbilityBase
{
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.EnableList(this.damageTrigger, false);
	}

	protected override void OnStart()
	{
		if ((double)(this.tellDuration * this.timeMult) > 0.0)
		{
			this.PlayState(GRAbilityAttackSimple.State.Tell, this.tellAnimData, this.soundTell, false);
		}
		else
		{
			this.PlayState(GRAbilityAttackSimple.State.Attack, this.attackAnimData, this.soundAttack, true);
		}
		if (!this.allowMovement)
		{
			this.agent.SetIsPathing(false, true);
			this.agent.SetDisableNetworkSync(true);
		}
		this.events.Reset();
		this.events.OnAbilityStart(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	protected override void OnStop()
	{
		if (!this.allowMovement)
		{
			this.agent.SetIsPathing(true, true);
			this.agent.SetDisableNetworkSync(false);
		}
		this.EnableList(this.damageTrigger, false);
		this.events.OnAbilityStop(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	private void PlayState(GRAbilityAttackSimple.State newState, AnimationData animData, AbilitySound sound, bool damageEnabled)
	{
		if (!string.IsNullOrEmpty(animData.animName))
		{
			this.PlayAnim(animData.animName, 0.1f, animData.speed);
			this.animNameString = animData.animName;
			this.timeMult = ((this.adjustByAnimationSpeed && !Mathf.Approximately(animData.speed, 0f)) ? (1f / animData.speed) : 1f);
		}
		sound.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		sound.Play(null);
		this.EnableList(this.damageTrigger, damageEnabled);
		this.state = newState;
	}

	public override bool IsDone()
	{
		return this.state == GRAbilityAttackSimple.State.Done;
	}

	protected override void OnUpdateShared(float dt)
	{
		float num = (float)(Time.timeAsDouble - this.startTime);
		switch (this.state)
		{
		case GRAbilityAttackSimple.State.Tell:
			if (num > this.tellDuration * this.timeMult)
			{
				this.PlayState(GRAbilityAttackSimple.State.Attack, this.attackAnimData, this.soundAttack, true);
			}
			break;
		case GRAbilityAttackSimple.State.Attack:
			if (num > (this.tellDuration + this.attackDuration) * this.timeMult)
			{
				this.PlayState(GRAbilityAttackSimple.State.FollowThrough, this.outroAnimData, this.soundOutro, false);
			}
			break;
		case GRAbilityAttackSimple.State.FollowThrough:
			if (num >= this.duration * this.timeMult)
			{
				this.state = GRAbilityAttackSimple.State.Done;
			}
			break;
		}
		this.events.TryPlay(num / this.timeMult, this.audioSource);
	}

	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
	}

	public string GetAnimName()
	{
		return this.animNameString;
	}

	public void EnableList(List<GameObject> objs, bool enable)
	{
		for (int i = 0; i < objs.Count; i++)
		{
			if (objs[i] != null)
			{
				objs[i].SetActive(enable);
			}
		}
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

	public bool allowMovement;

	public AnimationData tellAnimData;

	public AnimationData attackAnimData;

	public AnimationData outroAnimData;

	public AbilitySound soundTell;

	public AbilitySound soundAttack;

	public AbilitySound soundOutro;

	private float timeMult = 1f;

	private GRAbilityAttackSimple.State state;

	public float maxTurnSpeed;

	public List<GameObject> damageTrigger;

	private string animNameString;

	public GameAbilityEvents events;

	public bool adjustByAnimationSpeed;

	private enum State
	{
		Tell,
		Attack,
		FollowThrough,
		Done
	}
}
