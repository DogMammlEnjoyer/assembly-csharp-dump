using System;
using UnityEngine;

[Serializable]
public class GRAbilityAttackSimpleWander : GRAbilityBase
{
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.wander.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.attack.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	protected override void OnStart()
	{
		this.wander.Start();
		this.attack.Start();
	}

	protected override void OnStop()
	{
		this.wander.Stop();
		this.attack.Stop();
	}

	protected override void OnThink(float dt)
	{
		this.wander.Think(dt);
		this.attack.Think(dt);
	}

	protected override void OnUpdateAuthority(float dt)
	{
		this.wander.UpdateAuthority(dt);
		this.attack.UpdateAuthority(dt);
	}

	protected override void OnUpdateRemote(float dt)
	{
		this.wander.UpdateRemote(dt);
		this.attack.UpdateRemote(dt);
	}

	public override bool IsDone()
	{
		return this.attack.IsDone();
	}

	public override bool IsCoolDownOver()
	{
		return this.attack.IsCoolDownOver();
	}

	public override float GetRange()
	{
		return this.attack.GetRange();
	}

	public GRAbilityWander wander;

	public GRAbilityAttackSimple attack;
}
