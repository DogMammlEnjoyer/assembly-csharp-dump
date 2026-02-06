using System;
using UnityEngine;

[Serializable]
public class GRAbilityGrabbed : GRAbilityBase
{
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.idleAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	protected override void OnStart()
	{
		this.agent.SetIsPathing(false, true);
		this.idleAbility.Start();
	}

	protected override void OnStop()
	{
		this.idleAbility.Stop();
		this.agent.SetIsPathing(true, true);
	}

	public override bool IsDone()
	{
		return this.idleAbility.IsDone();
	}

	protected override void OnUpdateAuthority(float dt)
	{
		this.idleAbility.UpdateAuthority(dt);
	}

	protected override void OnUpdateRemote(float dt)
	{
		this.idleAbility.UpdateRemote(dt);
	}

	public GRAbilityIdle idleAbility;
}
