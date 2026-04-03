using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.Gravity
{
	public class MonkeGravityController : MonoBehaviour, ICallbackUnique, ICallBack
	{
		public Collider ActivatorCollider
		{
			get
			{
				return this.m_activatorCollider;
			}
		}

		public Rigidbody TargetRigidBody
		{
			get
			{
				return this.m_targetRigidBody;
			}
		}

		public Transform TargetTransform
		{
			get
			{
				return this.m_targetTransform;
			}
		}

		public virtual float Scale
		{
			get
			{
				return this.TargetTransform.localScale.x;
			}
		}

		protected bool InstantRotation
		{
			get
			{
				return this.m_instantRotation;
			}
		}

		protected bool OverrideForceMode
		{
			get
			{
				return this.m_overrideForceMode;
			}
		}

		public bool Register
		{
			get
			{
				return this.m_register;
			}
		}

		public Vector3 GravityUp { get; private set; } = Vector3.up;

		public Vector3 GravityDown { get; private set; } = Vector3.down;

		public float GravityMultiplier { get; set; } = 1f;

		public Vector3 PersonalGravityDirection { get; set; } = Vector3.up;

		public void SetPersonalGravityDirection(Vector3 direction)
		{
			this.PersonalGravityDirection = direction.normalized;
		}

		public void SetPersonalGravityDirection(Transform reference)
		{
			this.PersonalGravityDirection = reference.up;
		}

		public int GravityZonesCount
		{
			get
			{
				return this.m_gravityZones.Count;
			}
		}

		bool ICallbackUnique.Registered { get; set; }

		protected virtual void Awake()
		{
			if (this.m_targetRigidBody.IsNull())
			{
				this.m_targetRigidBody = base.GetComponent<Rigidbody>();
			}
			if (this.m_targetTransform.IsNull())
			{
				this.m_targetTransform = base.transform;
			}
			if (this.m_alwaysInZone != null)
			{
				this.m_alwaysInZone.AddTarget(this);
			}
			else if (this.m_activatorCollider.IsNull())
			{
				this.m_activatorCollider = base.GetComponent<Collider>();
				if (this.m_activatorCollider.IsNull())
				{
					return;
				}
			}
			if (this.m_targetRigidBody.IsNull())
			{
				return;
			}
			this.m_register = true;
			this.m_globalGravityIntent = this.m_targetRigidBody.useGravity;
		}

		protected virtual void OnEnable()
		{
			if (!this.m_register)
			{
				return;
			}
			MonkeGravityManager.AddMonkeGravityController(this);
		}

		protected virtual void OnDisable()
		{
			if (!this.m_register)
			{
				return;
			}
			this.m_targetRigidBody.useGravity = this.m_globalGravityIntent;
			MonkeGravityManager.RemoveMonkeGravityController(this);
			for (int i = this.m_gravityZones.Count - 1; i > -1; i--)
			{
				BasicGravityZone basicGravityZone = this.m_gravityZones[i];
				basicGravityZone.RemoveTarget(this);
				this.OnLeftGravityZone(basicGravityZone);
			}
		}

		public virtual void CallBack()
		{
			GravityInfo defaultGravityInfo;
			if (this.m_gravityZones.Count < 1 || !this.m_gravityZones[this.m_gravityZones.Count - 1].GetGravityInfo(this, out defaultGravityInfo))
			{
				defaultGravityInfo = MonkeGravityManager.DefaultGravityInfo;
			}
			this.GravityUp = defaultGravityInfo.gravityUpDirection;
			this.GravityDown = -this.GravityUp;
			if (this.m_gravityZones.Count < 1)
			{
				Vector3 gravity = Physics.gravity;
				this.ApplyGravityForce(gravity, ForceMode.Acceleration);
			}
			if ((defaultGravityInfo.rotate || this.m_needsRotationRecovery) && this.m_useRotation)
			{
				this.ApplyGravityUpRotation(defaultGravityInfo.rotationDirection, defaultGravityInfo.rotationSpeed * Time.fixedDeltaTime);
			}
		}

		public virtual Vector3 GetWorldPoint()
		{
			return this.m_targetTransform.position;
		}

		public virtual void OnEnteredGravityZone(BasicGravityZone zone)
		{
			if (!this.m_gravityZones.Contains(zone))
			{
				this.m_gravityZones.Add(zone);
			}
			if (this.m_targetRigidBody.useGravity)
			{
				this.m_targetRigidBody.useGravity = false;
			}
		}

		public virtual void OnLeftGravityZone(BasicGravityZone zone)
		{
			this.m_gravityZones.Remove(zone);
			if (this.m_gravityZones.Count < 1)
			{
				this.m_targetRigidBody.useGravity = this.m_globalGravityIntent;
				this.m_needsRotationRecovery = true;
			}
		}

		public virtual void ApplyGravityForce(in Vector3 force, ForceMode forceType = ForceMode.Acceleration)
		{
			if (this.m_targetRigidBody.isKinematic)
			{
				return;
			}
			this.m_targetRigidBody.AddForce(force * this.GravityMultiplier, this.m_overrideForceMode ? this.m_forceModeOverride : forceType);
		}

		public void ClearRotationRecovery()
		{
			this.m_needsRotationRecovery = false;
		}

		public virtual void ApplyGravityUpRotation(in Vector3 upDir, float speed)
		{
			Vector3 up = this.m_targetTransform.up;
			Vector3 toDirection = this.m_instantRotation ? upDir : Vector3.RotateTowards(up, upDir, speed, 0f);
			Quaternion lhs = Quaternion.FromToRotation(up, toDirection);
			this.m_targetRigidBody.MoveRotation(lhs * this.m_targetTransform.rotation);
			if (lhs == Quaternion.identity)
			{
				this.m_needsRotationRecovery = false;
			}
		}

		[SerializeField]
		private Collider m_activatorCollider;

		[SerializeField]
		protected Rigidbody m_targetRigidBody;

		[SerializeField]
		protected Transform m_targetTransform;

		[SerializeField]
		private bool m_instantRotation;

		[SerializeField]
		private bool m_useRotation;

		[SerializeField]
		private bool m_overrideForceMode;

		[SerializeField]
		private ForceMode m_forceModeOverride;

		[SerializeField]
		private BasicGravityZone m_alwaysInZone;

		private bool m_register;

		private readonly List<BasicGravityZone> m_gravityZones = new List<BasicGravityZone>(3);

		private bool m_needsRotationRecovery;

		protected bool m_globalGravityIntent;
	}
}
