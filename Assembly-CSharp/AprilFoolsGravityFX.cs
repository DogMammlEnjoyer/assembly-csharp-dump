using System;
using System.Collections;
using GorillaExtensions;
using GorillaTag.Gravity;
using UnityEngine;

public class AprilFoolsGravityFX : MonoBehaviour
{
	private void Start()
	{
		BasicGravityZone basicGravityZone = base.gameObject.AddComponent<PersonalGravityZone>();
		MonkeGravityController component = base.GetComponent<MonkeGravityController>();
		basicGravityZone.AddTarget(component);
		component.SetPersonalGravityDirection(Random.insideUnitCircle.x0y().WithY(-0.5f).normalized);
		base.StartCoroutine(this.BackToNormal());
	}

	private IEnumerator BackToNormal()
	{
		yield return new WaitForSeconds(180f);
		PersonalGravityZone component = base.GetComponent<PersonalGravityZone>();
		MonkeGravityController component2 = base.GetComponent<MonkeGravityController>();
		component.RemoveTarget(component2);
		yield break;
	}
}
