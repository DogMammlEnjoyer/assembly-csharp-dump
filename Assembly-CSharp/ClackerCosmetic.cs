using System;
using GorillaExtensions;
using Unity.Cinemachine;
using UnityEngine;

public class ClackerCosmetic : MonoBehaviour
{
	private void Start()
	{
		this.LocalRotationAxis = this.LocalRotationAxis.normalized;
		this.arm1.parent = this;
		this.arm2.parent = this;
		this.arm1.transform = this.clackerArm1;
		this.arm2.transform = this.clackerArm2;
		this.arm1.lastWorldPosition = this.clackerArm1.transform.TransformPoint(this.LocalCenterOfMass);
		this.arm2.lastWorldPosition = this.clackerArm2.transform.TransformPoint(this.LocalCenterOfMass);
		this.centerOfMassRadius = this.LocalCenterOfMass.magnitude;
		this.RotationCorrection = Quaternion.Euler(this.RotationCorrectionEuler);
	}

	private void Update()
	{
		Vector3 lastWorldPosition = this.arm1.lastWorldPosition;
		this.arm1.UpdateArm();
		this.arm2.UpdateArm();
		ref Vector3 eulerAngles = this.clackerArm1.transform.eulerAngles;
		Vector3 eulerAngles2 = this.clackerArm2.transform.eulerAngles;
		Mathf.DeltaAngle(eulerAngles.y, eulerAngles2.y);
		if ((this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).IsShorterThan(this.collisionDistance))
		{
			float sqrMagnitude = (this.arm1.velocity - this.arm2.velocity).sqrMagnitude;
			if (this.parentHoldable.InHand())
			{
				if (sqrMagnitude > this.heavyClackSpeed * this.heavyClackSpeed)
				{
					this.heavyClackAudio.Play();
				}
				else if (sqrMagnitude > this.mediumClackSpeed * this.mediumClackSpeed)
				{
					this.mediumClackAudio.Play();
				}
				else if (sqrMagnitude > this.minimumClackSpeed * this.minimumClackSpeed)
				{
					this.lightClackAudio.Play();
				}
			}
			Vector3 a = (this.arm1.lastWorldPosition + this.arm2.lastWorldPosition) / 2f;
			Vector3 vector = (this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).normalized * (this.collisionDistance + 0.001f) / 2f;
			Vector3 b = a + vector;
			Vector3 b2 = a - vector;
			if ((lastWorldPosition - b).IsLongerThan(lastWorldPosition - b2))
			{
				vector = -vector;
			}
			this.arm1.SetPosition(a + vector);
			this.arm2.SetPosition(a - vector);
			ref Vector3 ptr = ref this.arm1.velocity;
			Vector3 velocity = this.arm2.velocity;
			Vector3 velocity2 = this.arm1.velocity;
			ptr = velocity;
			this.arm2.velocity = velocity2;
			Vector3 b3 = (this.arm1.lastWorldPosition - this.arm2.lastWorldPosition).normalized * this.pushApartStrength * Mathf.Sqrt(sqrMagnitude);
			this.arm1.velocity = this.arm1.velocity + b3;
			this.arm2.velocity = this.arm2.velocity - b3;
		}
	}

	[SerializeField]
	private TransferrableObject parentHoldable;

	[SerializeField]
	private Transform clackerArm1;

	[SerializeField]
	private Transform clackerArm2;

	[SerializeField]
	private Vector3 LocalCenterOfMass;

	[SerializeField]
	private Vector3 LocalRotationAxis;

	[SerializeField]
	private Vector3 RotationCorrectionEuler;

	[SerializeField]
	private float drag;

	[SerializeField]
	private float gravity;

	[SerializeField]
	private float localFriction;

	[SerializeField]
	private float minimumClackSpeed;

	[SerializeField]
	private SoundBankPlayer lightClackAudio;

	[SerializeField]
	private float mediumClackSpeed;

	[SerializeField]
	private SoundBankPlayer mediumClackAudio;

	[SerializeField]
	private float heavyClackSpeed;

	[SerializeField]
	private SoundBankPlayer heavyClackAudio;

	[SerializeField]
	private float collisionDistance;

	private float centerOfMassRadius;

	[SerializeField]
	private float pushApartStrength;

	private ClackerCosmetic.PerArmData arm1;

	private ClackerCosmetic.PerArmData arm2;

	private Quaternion RotationCorrection;

	private struct PerArmData
	{
		public void UpdateArm()
		{
			Vector3 target = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
			Vector3 a = this.lastWorldPosition + this.velocity * Time.deltaTime * this.parent.drag;
			Vector3 vector = this.transform.parent.TransformDirection(this.parent.LocalRotationAxis);
			Vector3 vector2 = this.transform.position + (a - this.transform.position).ProjectOntoPlane(vector).normalized * this.parent.centerOfMassRadius;
			vector2 = Vector3.MoveTowards(vector2, target, this.parent.localFriction * Time.deltaTime);
			this.velocity = (vector2 - this.lastWorldPosition) / Time.deltaTime;
			this.velocity += Vector3.down * this.parent.gravity * Time.deltaTime;
			this.lastWorldPosition = vector2;
			this.transform.rotation = Quaternion.LookRotation(vector, vector2 - this.transform.position) * this.parent.RotationCorrection;
			this.lastWorldPosition = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
		}

		public void SetPosition(Vector3 newPosition)
		{
			Vector3 forward = this.transform.parent.TransformDirection(this.parent.LocalRotationAxis);
			this.transform.rotation = Quaternion.LookRotation(forward, newPosition - this.transform.position) * this.parent.RotationCorrection;
			this.lastWorldPosition = this.transform.TransformPoint(this.parent.LocalCenterOfMass);
		}

		public ClackerCosmetic parent;

		public Transform transform;

		public Vector3 velocity;

		public Vector3 lastWorldPosition;
	}
}
