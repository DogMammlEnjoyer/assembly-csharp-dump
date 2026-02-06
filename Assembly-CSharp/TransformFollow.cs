using System;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class TransformFollow : MonoBehaviour
{
	private void Awake()
	{
		this.prevPos = base.transform.position;
		if (this.rotationOnly && base.transform.parent != null && base.transform.parent.GetComponent<TransformFollow>() != null)
		{
			this.forRigRecording = true;
		}
		if (this.forRigRecording)
		{
			this.parentFollow = base.transform.parent.GetComponent<TransformFollow>();
		}
	}

	private void LateUpdate()
	{
		this.prevPos = base.transform.position;
		if (!this.rotationOnly)
		{
			Vector3 a;
			Quaternion rotation;
			this.transformToFollow.GetPositionAndRotation(out a, out rotation);
			base.transform.SetPositionAndRotation(a + rotation * this.offset, rotation);
			return;
		}
		if (this.forRigRecording)
		{
			base.transform.localRotation = Quaternion.Inverse(this.parentFollow.transformToFollow.rotation) * this.transformToFollow.rotation;
			return;
		}
		base.transform.rotation = this.transformToFollow.rotation;
	}

	public Transform transformToFollow;

	public Vector3 offset;

	public Vector3 prevPos;

	public bool rotationOnly;

	private bool forRigRecording;

	private TransformFollow parentFollow;
}
