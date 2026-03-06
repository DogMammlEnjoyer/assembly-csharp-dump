using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class HandCollider : MonoBehaviour
	{
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.rigidbody.maxAngularVelocity = 50f;
		}

		private void Start()
		{
			this.colliders = base.GetComponentsInChildren<Collider>();
			if (HandCollider.physicMaterial_lowfriction == null)
			{
				HandCollider.physicMaterial_lowfriction = new PhysicsMaterial("hand_lowFriction");
				HandCollider.physicMaterial_lowfriction.dynamicFriction = 0f;
				HandCollider.physicMaterial_lowfriction.staticFriction = 0f;
				HandCollider.physicMaterial_lowfriction.bounciness = 0f;
				HandCollider.physicMaterial_lowfriction.bounceCombine = PhysicsMaterialCombine.Minimum;
				HandCollider.physicMaterial_lowfriction.frictionCombine = PhysicsMaterialCombine.Minimum;
			}
			if (HandCollider.physicMaterial_highfriction == null)
			{
				HandCollider.physicMaterial_highfriction = new PhysicsMaterial("hand_highFriction");
				HandCollider.physicMaterial_highfriction.dynamicFriction = 1f;
				HandCollider.physicMaterial_highfriction.staticFriction = 1f;
				HandCollider.physicMaterial_highfriction.bounciness = 0f;
				HandCollider.physicMaterial_highfriction.bounceCombine = PhysicsMaterialCombine.Minimum;
				HandCollider.physicMaterial_highfriction.frictionCombine = PhysicsMaterialCombine.Average;
			}
			this.SetPhysicMaterial(HandCollider.physicMaterial_lowfriction);
			this.scale = SteamVR_Utils.GetLossyScale(this.hand.transform);
		}

		private void SetPhysicMaterial(PhysicsMaterial mat)
		{
			if (this.colliders == null)
			{
				this.colliders = base.GetComponentsInChildren<Collider>();
			}
			for (int i = 0; i < this.colliders.Length; i++)
			{
				this.colliders[i].sharedMaterial = mat;
			}
		}

		public void SetCollisionDetectionEnabled(bool value)
		{
			this.rigidbody.detectCollisions = value;
		}

		public void MoveTo(Vector3 position, Quaternion rotation)
		{
			this.targetPosition = position;
			this.targetRotation = rotation;
			this.ExecuteFixedUpdate();
		}

		public void TeleportTo(Vector3 position, Quaternion rotation)
		{
			this.targetPosition = position;
			this.targetRotation = rotation;
			this.MoveTo(position, rotation);
			this.rigidbody.position = position;
			if (rotation.x != 0f || rotation.y != 0f || rotation.z != 0f || rotation.w != 0f)
			{
				this.rigidbody.rotation = rotation;
			}
			base.transform.position = position;
			base.transform.rotation = rotation;
		}

		public void Reset()
		{
			this.TeleportTo(this.targetPosition, this.targetRotation);
		}

		public void SetCenterPoint(Vector3 newCenter)
		{
			this.center = newCenter;
		}

		protected void ExecuteFixedUpdate()
		{
			this.collidersInRadius = Physics.CheckSphere(this.center, 0.2f, this.collisionMask);
			if (!this.collidersInRadius)
			{
				this.rigidbody.linearVelocity = Vector3.zero;
				this.rigidbody.angularVelocity = Vector3.zero;
				this.rigidbody.MovePosition(this.targetPosition);
				this.rigidbody.MoveRotation(this.targetRotation);
				return;
			}
			Vector3 target;
			Vector3 target2;
			if (this.GetTargetVelocities(out target, out target2))
			{
				float maxDistanceDelta = 20f * this.scale;
				float maxDistanceDelta2 = 10f * this.scale;
				this.rigidbody.linearVelocity = Vector3.MoveTowards(this.rigidbody.linearVelocity, target, maxDistanceDelta2);
				this.rigidbody.angularVelocity = Vector3.MoveTowards(this.rigidbody.angularVelocity, target2, maxDistanceDelta);
			}
		}

		protected bool GetTargetVelocities(out Vector3 velocityTarget, out Vector3 angularTarget)
		{
			bool flag = false;
			float d = 6000f;
			float d2 = 50f;
			Vector3 a = this.targetPosition - this.rigidbody.position;
			velocityTarget = a * d * Time.deltaTime;
			if (!float.IsNaN(velocityTarget.x) && !float.IsInfinity(velocityTarget.x))
			{
				flag = true;
			}
			else
			{
				velocityTarget = Vector3.zero;
			}
			float num;
			Vector3 vector;
			(this.targetRotation * Quaternion.Inverse(this.rigidbody.rotation)).ToAngleAxis(out num, out vector);
			if (num > 180f)
			{
				num -= 360f;
			}
			if (num != 0f && !float.IsNaN(vector.x) && !float.IsInfinity(vector.x))
			{
				angularTarget = num * vector * d2 * Time.deltaTime;
				flag = flag;
			}
			else
			{
				angularTarget = Vector3.zero;
			}
			return flag;
		}

		private void OnCollisionEnter(Collision collision)
		{
			bool flag = false;
			if (collision.rigidbody != null && !collision.rigidbody.isKinematic)
			{
				flag = true;
			}
			this.SetPhysicMaterial(flag ? HandCollider.physicMaterial_highfriction : HandCollider.physicMaterial_lowfriction);
			float magnitude = collision.relativeVelocity.magnitude;
			if (magnitude > 0.1f && Time.time - this.lastCollisionHapticsTime > 0.2f)
			{
				this.lastCollisionHapticsTime = Time.time;
				float amplitude = Util.RemapNumber(magnitude, 0.1f, 1f, 0.3f, 1f);
				float duration = Util.RemapNumber(magnitude, 0.1f, 1f, 0f, 0.06f);
				this.hand.hand.TriggerHapticPulse(duration, 100f, amplitude);
			}
		}

		private Rigidbody rigidbody;

		[HideInInspector]
		public HandPhysics hand;

		public LayerMask collisionMask;

		private Collider[] colliders;

		public HandCollider.FingerColliders fingerColliders;

		private static PhysicsMaterial physicMaterial_lowfriction;

		private static PhysicsMaterial physicMaterial_highfriction;

		private float scale;

		private Vector3 center;

		private Vector3 targetPosition = Vector3.zero;

		private Quaternion targetRotation = Quaternion.identity;

		protected const float MaxVelocityChange = 10f;

		protected const float VelocityMagic = 6000f;

		protected const float AngularVelocityMagic = 50f;

		protected const float MaxAngularVelocityChange = 20f;

		public bool collidersInRadius;

		private const float minCollisionEnergy = 0.1f;

		private const float maxCollisionEnergy = 1f;

		private const float minCollisionHapticsTime = 0.2f;

		private float lastCollisionHapticsTime;

		[Serializable]
		public class FingerColliders
		{
			public Transform[] this[int finger]
			{
				get
				{
					switch (finger)
					{
					case 0:
						return this.thumbColliders;
					case 1:
						return this.indexColliders;
					case 2:
						return this.middleColliders;
					case 3:
						return this.ringColliders;
					case 4:
						return this.pinkyColliders;
					default:
						return null;
					}
				}
				set
				{
					switch (finger)
					{
					case 0:
						this.thumbColliders = value;
						return;
					case 1:
						this.indexColliders = value;
						return;
					case 2:
						this.middleColliders = value;
						return;
					case 3:
						this.ringColliders = value;
						return;
					case 4:
						this.pinkyColliders = value;
						return;
					default:
						return;
					}
				}
			}

			[Tooltip("Starting at tip and going down. Max 2.")]
			public Transform[] thumbColliders = new Transform[1];

			[Tooltip("Starting at tip and going down. Max 3.")]
			public Transform[] indexColliders = new Transform[2];

			[Tooltip("Starting at tip and going down. Max 3.")]
			public Transform[] middleColliders = new Transform[2];

			[Tooltip("Starting at tip and going down. Max 3.")]
			public Transform[] ringColliders = new Transform[2];

			[Tooltip("Starting at tip and going down. Max 3.")]
			public Transform[] pinkyColliders = new Transform[2];
		}
	}
}
