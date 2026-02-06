using System;
using GorillaLocomotion;
using UnityEngine;

public class SIBlasterSprayProjectile : MonoBehaviour
{
	private void OnEnable()
	{
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
	}

	public void LocalProjectileHit(SIPlayer player = null)
	{
		if (player != null && this.projectile.hitEffectPlayer != null)
		{
			Object.Instantiate<GameObject>(this.projectile.hitEffectPlayer, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player == null && this.projectile.hitEffect != null)
		{
			Object.Instantiate<GameObject>(this.projectile.hitEffect, this.projectile.transform.position, this.projectile.transform.rotation);
		}
		if (player != null)
		{
			this.TriggerBlastDirectHitPlayer(player);
		}
		this.projectile.DespawnProjectile();
	}

	public void TriggerBlastDirectHitPlayer(SIPlayer playerHit)
	{
		if (playerHit == SIPlayer.LocalPlayer)
		{
			return;
		}
		float num = Vector3.Angle(base.transform.forward, Vector3.up);
		Vector3 vector = Vector3.RotateTowards(base.transform.forward.normalized, Vector3.up, Mathf.Clamp(num - this.upwardsAngle, 0f, this.upwardsAngle) * 0.017453292f, 0f);
		this.projectile.parentBlaster.SendClientToClientRPC(1, new object[]
		{
			this.projectile.projectileId,
			base.transform.position,
			vector,
			playerHit.ActorNr
		});
	}

	public void NetworkedProjectileHit(object[] data)
	{
		if (data == null || data.Length != 4)
		{
			return;
		}
		int num;
		if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
		{
			return;
		}
		Vector3 vector;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[1], out vector))
		{
			return;
		}
		Vector3 vector2;
		if (!GameEntityManager.ValidateDataType<Vector3>(data[2], out vector2))
		{
			return;
		}
		int actorNumber;
		if (!GameEntityManager.ValidateDataType<int>(data[3], out actorNumber))
		{
			return;
		}
		if ((base.transform.position - vector).magnitude > this.projectile.parentBlaster.maxLagDistance)
		{
			return;
		}
		this.projectile.DespawnProjectile();
		SIPlayer x = SIPlayer.Get(actorNumber);
		if (x == null)
		{
			return;
		}
		if (x != SIPlayer.LocalPlayer)
		{
			Object.Instantiate<GameObject>(this.projectile.hitEffect, vector, this.projectile.transform.rotation);
			return;
		}
		Object.Instantiate<GameObject>(this.projectile.hitEffectPlayer, vector, this.projectile.transform.rotation);
		GTPlayer.Instance.ApplyKnockback(vector2.normalized, this.knockbackSpeed, true);
	}

	private SIGadgetBlasterProjectile projectile;

	public float knockbackSpeed;

	public float verticalOffset = -0.133f;

	public float upwardsAngle = 30f;
}
