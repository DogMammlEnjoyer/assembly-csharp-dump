using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class BuggyBuddy : MonoBehaviour
	{
		private void Start()
		{
			this.body = base.GetComponent<Rigidbody>();
			this.m_Wheels = base.GetComponentsInChildren<WheelCollider>();
			this.body.centerOfMass = this.body.transform.InverseTransformPoint(this.centerOfMass.position) * this.body.transform.lossyScale.x;
		}

		private void Update()
		{
			this.m_Wheels[0].ConfigureVehicleSubsteps(this.criticalSpeed, this.stepsBelow, this.stepsAbove);
			float num = this.maxTorque * this.throttle;
			if (this.steer.y < -0.5f)
			{
				num *= -1f;
			}
			float num2 = this.maxAngle * this.steer.x;
			this.speed = base.transform.InverseTransformVector(this.body.linearVelocity).z;
			float num3 = Mathf.Abs(this.speed);
			num2 /= 1f + num3 / 20f;
			float num4 = Mathf.Abs(num);
			this.mvol = Mathf.Lerp(this.mvol, Mathf.Pow(num4 / this.maxTorque, 0.8f) * Mathf.Lerp(0.4f, 1f, Mathf.Abs(this.m_Wheels[2].rpm) / 200f) * Mathf.Lerp(1f, 0.5f, this.handBrake), Time.deltaTime * 9f);
			this.au_motor.volume = Mathf.Clamp01(this.mvol);
			float value = Mathf.Lerp(0.8f, 1f, this.mvol);
			this.au_motor.pitch = Mathf.Clamp01(value);
			this.svol = Mathf.Lerp(this.svol, this.skidsample.amt / this.skidSpeed, Time.deltaTime * 9f);
			this.au_skid.volume = Mathf.Clamp01(this.svol);
			float value2 = Mathf.Lerp(0.9f, 1f, this.svol);
			this.au_skid.pitch = Mathf.Clamp01(value2);
			for (int i = 0; i < this.wheelRenders.Length; i++)
			{
				WheelCollider wheelCollider = this.m_Wheels[i];
				if (wheelCollider.transform.localPosition.z > 0f)
				{
					wheelCollider.steerAngle = num2;
					wheelCollider.motorTorque = num;
				}
				float z = wheelCollider.transform.localPosition.z;
				wheelCollider.motorTorque = num;
				float x = wheelCollider.transform.localPosition.x;
				float x2 = wheelCollider.transform.localPosition.x;
				if (this.wheelRenders[i] != null && this.m_Wheels[0].enabled)
				{
					Vector3 position;
					Quaternion rotation;
					wheelCollider.GetWorldPose(out position, out rotation);
					Transform transform = this.wheelRenders[i].transform;
					transform.position = position;
					transform.rotation = rotation;
				}
			}
			this.steer = Vector2.Lerp(this.steer, Vector2.zero, Time.deltaTime * 4f);
		}

		private void FixedUpdate()
		{
			this.body.AddForce(this.localGravity * this.body.mass, ForceMode.Force);
		}

		public static float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
		{
			if (toVector == Vector3.zero)
			{
				return 0f;
			}
			float num = Vector3.Angle(fromVector, toVector);
			Vector3 lhs = Vector3.Cross(fromVector, toVector);
			return num * Mathf.Sign(Vector3.Dot(lhs, upVector)) * 0.017453292f;
		}

		public Transform turret;

		private float turretRot;

		[Tooltip("Maximum steering angle of the wheels")]
		public float maxAngle = 30f;

		[Tooltip("Maximum Turning torque")]
		public float maxTurnTorque = 30f;

		[Tooltip("Maximum torque applied to the driving wheels")]
		public float maxTorque = 300f;

		[Tooltip("Maximum brake torque applied to the driving wheels")]
		public float brakeTorque = 30000f;

		[Tooltip("If you need the visual wheels to be attached automatically, drag the wheel shape here.")]
		public GameObject[] wheelRenders;

		[Tooltip("The vehicle's speed when the physics engine can use different amount of sub-steps (in m/s).")]
		public float criticalSpeed = 5f;

		[Tooltip("Simulation sub-steps when the speed is above critical.")]
		public int stepsBelow = 5;

		[Tooltip("Simulation sub-steps when the speed is below critical.")]
		public int stepsAbove = 1;

		private WheelCollider[] m_Wheels;

		public AudioSource au_motor;

		[HideInInspector]
		public float mvol;

		public AudioSource au_skid;

		private float svol;

		public WheelDust skidsample;

		private float skidSpeed = 3f;

		public Vector3 localGravity;

		[HideInInspector]
		public Rigidbody body;

		public float rapidfireTime;

		private float shootTimer;

		[HideInInspector]
		public Vector2 steer;

		[HideInInspector]
		public float throttle;

		[HideInInspector]
		public float handBrake;

		[HideInInspector]
		public Transform controllerReference;

		[HideInInspector]
		public float speed;

		public Transform centerOfMass;
	}
}
