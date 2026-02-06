using System;
using Unity.XR.CoreUtils;
using UnityEngine;

[Serializable]
public class GRAbilityIdle : GRAbilityBase
{
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.animLoops = 0;
		this.cachedDuration = this.duration;
		this.cachedAnimSpeed = this.animSpeed;
	}

	protected override void OnStart()
	{
		this.agent.SetStopped(true);
		this.PlayAnim(this.animName, 0.3f, this.animSpeed);
		this.animLoops = 0;
		this.events.Reset();
		this.events.OnAbilityStart(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
	}

	protected override void OnStop()
	{
		this.events.OnAbilityStop(base.GetAbilityTime(Time.timeAsDouble), this.audioSource);
		this.agent.SetStopped(false);
	}

	protected override void OnUpdateShared(float dt)
	{
		float abilityTime = (float)(Time.timeAsDouble - this.startTime);
		if (this.anim != null && this.anim[this.animName] != null)
		{
			if ((int)this.anim[this.animName].normalizedTime > this.animLoops)
			{
				this.events.Reset();
				this.animLoops = (int)this.anim[this.animName].normalizedTime;
			}
			abilityTime = this.anim[this.animName].time - this.anim[this.animName].length * (float)this.animLoops;
		}
		this.events.TryPlay(abilityTime, this.audioSource);
	}

	public override bool IsDone()
	{
		return (double)this.duration > 0.0 && Time.timeAsDouble >= this.startTime + (double)this.duration;
	}

	public override bool IsCoolDownOver()
	{
		return base.IsCoolDownOver(this.coolDown);
	}

	public override float GetRange()
	{
		return this.range;
	}

	public void SpeedUp(float mult)
	{
		this.duration = this.cachedDuration / mult;
		this.animSpeed = this.cachedAnimSpeed * mult;
	}

	public float duration;

	public string animName;

	public float animSpeed;

	public float coolDown;

	public float range;

	private float cachedDuration;

	private float cachedAnimSpeed;

	public GameAbilityEvents events;

	[ReadOnly]
	public int animLoops;
}
