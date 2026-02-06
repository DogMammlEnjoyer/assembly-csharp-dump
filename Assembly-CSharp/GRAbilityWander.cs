using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class GRAbilityWander : GRAbilityBase
{
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.moveAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	protected override void OnStart()
	{
		this.moveAbility.Start();
		Vector3 targetPos = this.PickRandomDestination();
		this.moveAbility.SetTargetPos(targetPos);
	}

	protected override void OnStop()
	{
		this.moveAbility.Stop();
	}

	public override bool IsDone()
	{
		return false;
	}

	protected override void OnThink(float dt)
	{
		if (this.moveAbility.IsDone())
		{
			Vector3 targetPos = this.PickRandomDestination();
			this.moveAbility.SetTargetPos(targetPos);
		}
	}

	private Vector3 PickRandomDestination()
	{
		Vector3 position = this.agent.transform.position;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(position, out navMeshHit, 1f, this.walkableArea))
		{
			Vector3 position2 = navMeshHit.position;
			Vector3 forward = this.agent.transform.forward;
			float num = 0f;
			for (int i = 0; i < GRAbilityWander.rotations.Length; i++)
			{
				Vector3 a = GRAbilityWander.rotations[i] * forward;
				float num2 = 8f;
				if (NavMesh.Raycast(position2, position2 + a * num2, out navMeshHit, this.walkableArea))
				{
					num2 = navMeshHit.distance * 0.95f;
				}
				float num3 = num2 * GRAbilityWander.rotationWeight[i];
				if (num3 > num && NavMesh.SamplePosition(position2 + a * num2, out navMeshHit, 1f, this.walkableArea))
				{
					num = num3;
					position = navMeshHit.position;
				}
			}
		}
		return position;
	}

	protected override void OnUpdateAuthority(float dt)
	{
		this.moveAbility.UpdateAuthority(dt);
	}

	protected override void OnUpdateRemote(float dt)
	{
		this.moveAbility.UpdateRemote(dt);
	}

	public GRAbilityMoveToTarget moveAbility;

	private static Quaternion[] rotations = new Quaternion[]
	{
		Quaternion.Euler(0f, 0f, 0f),
		Quaternion.Euler(0f, 45f, 0f),
		Quaternion.Euler(0f, -45f, 0f),
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, -90f, 0f),
		Quaternion.Euler(0f, 135f, 0f),
		Quaternion.Euler(0f, -135f, 0f),
		Quaternion.Euler(0f, 180f, 0f)
	};

	private static float[] rotationWeight = new float[]
	{
		1f,
		0.75f,
		0.75f,
		0.5f,
		0.5f,
		0.2f,
		0.2f,
		0.2f
	};
}
