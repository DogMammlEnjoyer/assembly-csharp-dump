using System;
using UnityEngine;

public class SIBlasterRandomAngularVelocity : MonoBehaviour, SIGadgetProjectileModifier
{
	public void ModifyProjectile(SIGadgetBlasterProjectile projectile)
	{
		projectile.rb.angularVelocity = new Vector3(Random.Range(-this.maxVel, this.maxVel), Random.Range(-this.maxVel, this.maxVel), Random.Range(-this.maxVel, this.maxVel));
	}

	public float maxVel;
}
