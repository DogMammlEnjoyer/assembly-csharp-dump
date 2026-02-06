using System;
using UnityEngine;

public class SIBlasterPumpableProjectile : MonoBehaviour, SIGadgetProjectileModifier
{
	public void ModifyProjectile(SIGadgetBlasterProjectile projectile)
	{
		SIGadgetPumpBlaster component = projectile.parentBlaster.GetComponent<SIGadgetPumpBlaster>();
		if (component == null)
		{
			return;
		}
		this.pumpChargedAmount = Mathf.Min(this.maxPump, component.currentPumpChargeAmount);
		projectile.startingVelocity += this.pumpChargedAmount;
		if (this.strengthPerPumpCharge > 0f)
		{
			SIBlasterDirectHitProjectile component2 = projectile.GetComponent<SIBlasterDirectHitProjectile>();
			if (component2 != null)
			{
				component2.knockbackSpeed += this.strengthPerPumpCharge * this.pumpChargedAmount;
			}
			SIBlasterSplashProjectile component3 = projectile.GetComponent<SIBlasterSplashProjectile>();
			if (component3 != null)
			{
				component3.knockbackSpeed += this.strengthPerPumpCharge * this.pumpChargedAmount;
			}
			SIBlasterSprayProjectile component4 = projectile.GetComponent<SIBlasterSprayProjectile>();
			if (component4 != null)
			{
				component4.knockbackSpeed += this.strengthPerPumpCharge * this.pumpChargedAmount;
			}
		}
	}

	public float maxPump;

	public float pumpChargedAmount;

	public float velocityPerPumpCharge;

	public float strengthPerPumpCharge;
}
