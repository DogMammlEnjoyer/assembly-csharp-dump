using System;
using UnityEngine;

public class GRShieldCollider : MonoBehaviour
{
	public float KnockbackVelocity
	{
		get
		{
			return this.knockbackVelocity;
		}
	}

	public GRToolDirectionalShield ShieldTool
	{
		get
		{
			return this.shieldTool;
		}
	}

	private void Awake()
	{
		this.lastBlockHittableEntityId = GameEntityId.Invalid;
		this.lastBlockHittableTime = 0.0;
	}

	public void OnEnemyBlocked(Vector3 enemyPosition)
	{
		if (this.shieldTool != null)
		{
			this.shieldTool.OnEnemyBlocked(enemyPosition);
		}
	}

	public void BlockHittable(Vector3 enemyPosition, Vector3 enemyAttackDirection, GameHittable hittable)
	{
		if (this.shieldTool != null)
		{
			double timeAsDouble = Time.timeAsDouble;
			if (timeAsDouble - this.lastBlockHittableTime >= 1.0 || !(hittable.gameEntity.id == this.lastBlockHittableEntityId))
			{
				this.lastBlockHittableEntityId = hittable.gameEntity.id;
				this.lastBlockHittableTime = timeAsDouble;
				this.shieldTool.BlockHittable(enemyPosition, enemyAttackDirection, hittable, this);
			}
		}
	}

	[SerializeField]
	private float knockbackVelocity = 3f;

	[SerializeField]
	private GRToolDirectionalShield shieldTool;

	private const float BLOCK_SAME_HITTABLE_COOLDOWN = 1f;

	private GameEntityId lastBlockHittableEntityId;

	private double lastBlockHittableTime;
}
