using System;
using GorillaTag.Cosmetics;
using UnityEngine;

public class ArcingProjectileLauncher : ElfLauncher
{
	protected override void ShootShared(Vector3 origin, Vector3 direction)
	{
		this.shootAudio.Play();
		Vector3 lossyScale = base.transform.lossyScale;
		float num = Vector3.Dot(direction, Vector3.up);
		Vector3 vector;
		if ((double)num > 0.99999 || (double)num < -0.99999)
		{
			if (this.parentHoldable.myRig != null)
			{
				vector = this.parentHoldable.myRig.transform.forward;
			}
			else
			{
				vector = Vector3.forward;
			}
		}
		else
		{
			vector = Vector3.ProjectOnPlane(direction, Vector3.up);
		}
		vector.Normalize();
		Vector3 axis = Vector3.Cross(vector, Vector3.up);
		float num2 = Vector3.SignedAngle(vector, direction, axis);
		float num3 = this.angleVelocityMultiplier.Evaluate(num2);
		float num4 = Mathf.Clamp(num2, this.fireAngleLimits.x, this.fireAngleLimits.y);
		float d = Mathf.Sin(num4 * 0.017453292f);
		float d2 = Mathf.Cos(num4 * 0.017453292f);
		Vector3 a = d * Vector3.up + d2 * vector;
		a.Normalize();
		Vector3 vector2 = a * (this.muzzleVelocity * lossyScale.x * num3);
		GameObject gameObject = ObjectPools.instance.Instantiate(this.elfProjectileHash, true);
		IProjectile component = gameObject.GetComponent<IProjectile>();
		if (component != null)
		{
			component.Launch(origin, Quaternion.LookRotation(vector, Vector3.up), vector2, 1f, this.parentHoldable.myRig, -1);
			return;
		}
		gameObject.transform.position = origin;
		gameObject.transform.rotation = Quaternion.LookRotation(direction);
		gameObject.transform.localScale = lossyScale;
		gameObject.GetComponent<Rigidbody>().linearVelocity = vector2;
	}

	[SerializeField]
	private Vector2 fireAngleLimits = new Vector3(-75f, 75f);

	[SerializeField]
	private AnimationCurve angleVelocityMultiplier;
}
