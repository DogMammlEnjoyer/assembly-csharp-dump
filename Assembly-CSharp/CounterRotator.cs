using System;
using GorillaTag.Gravity;
using UnityEngine;

public class CounterRotator : MonoBehaviour
{
	private void Start()
	{
		this.startingPosition = this.stabilizedObject.transform.position;
		this.startingRotation = this.stabilizedObject.transform.rotation;
	}

	private void LateUpdate()
	{
		Quaternion lhs = this.startingRotation * Quaternion.Inverse(this.stabilizedObject.transform.rotation);
		base.transform.rotation = lhs * base.transform.rotation;
		Vector3 b = this.startingPosition - this.stabilizedObject.transform.position;
		base.transform.position += b;
		if (this.gravityCompensator != null)
		{
			this.gravityCompensator.SetGravityDirection(-base.transform.up);
		}
	}

	[SerializeField]
	private GameObject stabilizedObject;

	[SerializeField]
	private ChangingBasicGravityZone gravityCompensator;

	private Vector3 startingPosition;

	private Quaternion startingRotation;
}
