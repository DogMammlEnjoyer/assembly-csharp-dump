using System;
using System.Collections;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.Gravity;
using UnityEngine;

public class AprilFoolsGravityFX : MonoBehaviour
{
	private void Start()
	{
		GTPlayerTransform.EnableNetworkRotations();
		BasicGravityZone basicGravityZone = base.gameObject.AddComponent<PersonalGravityZone>();
		MonkeGravityController instance = GTPlayerTransform.Instance;
		basicGravityZone.AddTarget(instance);
		instance.SetPersonalGravityDirection(Random.insideUnitCircle.x0y().WithY(0.5f).normalized);
		base.StartCoroutine(this.BackToNormal());
	}

	private void OnDisable()
	{
		BasicGravityZone component = base.GetComponent<PersonalGravityZone>();
		MonkeGravityController instance = GTPlayerTransform.Instance;
		component.RemoveTarget(instance);
	}

	private IEnumerator BackToNormal()
	{
		yield return new WaitForSeconds(180f);
		BasicGravityZone component = base.GetComponent<PersonalGravityZone>();
		MonkeGravityController instance = GTPlayerTransform.Instance;
		component.RemoveTarget(instance);
		yield break;
	}
}
