using System;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class TransformFollow : MonoBehaviour
{
	private void Awake()
	{
		this.prevPos = base.transform.position;
	}

	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		Vector3 a;
		Quaternion rotation;
		this.transformToFollow.GetPositionAndRotation(out a, out rotation);
		base.transform.SetPositionAndRotation(a + rotation * this.offset, rotation);
	}

	public Transform transformToFollow;

	public Vector3 offset;

	public Vector3 prevPos;
}
