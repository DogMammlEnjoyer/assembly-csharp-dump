using System;
using UnityEngine;

public class OwlLook : MonoBehaviour
{
	private void Awake()
	{
		this.overlapRigs = new VRRig[20];
		if (this.myRig == null)
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
	}

	private void LateUpdate()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			if (this.rigs.Length != NetworkSystem.Instance.RoomPlayerCount)
			{
				this.rigs = VRRigCache.Instance.GetAllRigs();
			}
		}
		else if (this.rigs.Length != 1)
		{
			this.rigs = new VRRig[1];
			this.rigs[0] = VRRig.LocalRig;
		}
		float num = -1f;
		float num2 = Mathf.Cos(this.lookAtAngleDegrees / 180f * 3.1415927f);
		int num3 = 0;
		for (int i = 0; i < this.rigs.Length; i++)
		{
			if (!(this.rigs[i] == this.myRig))
			{
				Vector3 rhs = this.rigs[i].tagSound.transform.position - base.transform.position;
				if (rhs.magnitude <= this.lookRadius)
				{
					float num4 = Vector3.Dot(-base.transform.up, rhs.normalized);
					if (num4 > num2)
					{
						this.overlapRigs[num3++] = this.rigs[i];
					}
				}
			}
		}
		this.lookTarget = null;
		for (int j = 0; j < num3; j++)
		{
			Vector3 rhs = (this.overlapRigs[j].tagSound.transform.position - base.transform.position).normalized;
			float num4 = Vector3.Dot(base.transform.forward, rhs);
			if (num4 > num)
			{
				num = num4;
				this.lookTarget = this.overlapRigs[j].tagSound.transform;
			}
		}
		Vector3 vector = this.neck.forward;
		if (this.lookTarget != null)
		{
			vector = (this.lookTarget.position - this.head.position).normalized;
		}
		Vector3 vector2 = this.neck.InverseTransformDirection(vector);
		vector2.y = Mathf.Clamp(vector2.y, this.minNeckY, this.maxNeckY);
		vector = this.neck.TransformDirection(vector2.normalized);
		Vector3 forward = Vector3.RotateTowards(this.head.forward, vector, this.rotSpeed * 0.017453292f * Time.deltaTime, 0f);
		this.head.rotation = Quaternion.LookRotation(forward, this.neck.up);
	}

	public Transform head;

	public Transform lookTarget;

	public Transform neck;

	public float lookRadius = 0.5f;

	public Collider[] overlapColliders;

	public VRRig[] rigs = new VRRig[20];

	public VRRig[] overlapRigs;

	public float rotSpeed = 1f;

	public float lookAtAngleDegrees = 60f;

	public float maxNeckY;

	public float minNeckY;

	public VRRig myRig;
}
