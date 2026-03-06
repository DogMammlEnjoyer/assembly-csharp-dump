using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[SaveDuringPlay]
	[AddComponentMenu("Cinemachine/Helpers/Cinemachine Collision Impulse Source")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineCollisionImpulseSource.html")]
	public class CinemachineCollisionImpulseSource : CinemachineImpulseSource
	{
		private void Reset()
		{
			this.LayerMask = 1;
			this.IgnoreTag = string.Empty;
			this.UseImpactDirection = false;
			this.ScaleImpactWithMass = false;
			this.ScaleImpactWithSpeed = false;
		}

		private void Start()
		{
			base.TryGetComponent<Rigidbody>(out this.m_RigidBody);
			base.TryGetComponent<Rigidbody2D>(out this.m_RigidBody2D);
		}

		private void OnEnable()
		{
		}

		private void OnCollisionEnter(Collision c)
		{
			this.GenerateImpactEvent(c.collider, c.relativeVelocity);
		}

		private void OnTriggerEnter(Collider c)
		{
			this.GenerateImpactEvent(c, Vector3.zero);
		}

		private float GetMassAndVelocity(Collider other, ref Vector3 vel)
		{
			bool flag = vel == Vector3.zero;
			float num = 1f;
			if (this.ScaleImpactWithMass || this.ScaleImpactWithSpeed || this.UseImpactDirection)
			{
				if (this.m_RigidBody != null)
				{
					if (this.ScaleImpactWithMass)
					{
						num *= this.m_RigidBody.mass;
					}
					if (flag)
					{
						vel = -this.m_RigidBody.linearVelocity;
					}
				}
				Rigidbody rigidbody = (other != null) ? other.attachedRigidbody : null;
				if (rigidbody != null)
				{
					if (this.ScaleImpactWithMass)
					{
						num *= rigidbody.mass;
					}
					if (flag)
					{
						vel += rigidbody.linearVelocity;
					}
				}
			}
			return num;
		}

		private void GenerateImpactEvent(Collider other, Vector3 vel)
		{
			if (!base.enabled)
			{
				return;
			}
			if (other != null)
			{
				int layer = other.gameObject.layer;
				if ((1 << layer & this.LayerMask) == 0)
				{
					return;
				}
				if (this.IgnoreTag.Length != 0 && other.CompareTag(this.IgnoreTag))
				{
					return;
				}
			}
			float num = this.GetMassAndVelocity(other, ref vel);
			if (this.ScaleImpactWithSpeed)
			{
				num *= Mathf.Sqrt(vel.magnitude);
			}
			Vector3 a = this.DefaultVelocity;
			if (this.UseImpactDirection && !vel.AlmostZero())
			{
				a = -vel.normalized * a.magnitude;
			}
			base.GenerateImpulseWithVelocity(a * num);
		}

		private void OnCollisionEnter2D(Collision2D c)
		{
			this.GenerateImpactEvent2D(c.collider, c.relativeVelocity);
		}

		private void OnTriggerEnter2D(Collider2D c)
		{
			this.GenerateImpactEvent2D(c, Vector3.zero);
		}

		private float GetMassAndVelocity2D(Collider2D other2d, ref Vector3 vel)
		{
			bool flag = vel == Vector3.zero;
			float num = 1f;
			if (this.ScaleImpactWithMass || this.ScaleImpactWithSpeed || this.UseImpactDirection)
			{
				if (this.m_RigidBody2D != null)
				{
					if (this.ScaleImpactWithMass)
					{
						num *= this.m_RigidBody2D.mass;
					}
					if (flag)
					{
						vel = -this.m_RigidBody2D.linearVelocity;
					}
				}
				Rigidbody2D rigidbody2D = (other2d != null) ? other2d.attachedRigidbody : null;
				if (rigidbody2D != null)
				{
					if (this.ScaleImpactWithMass)
					{
						num *= rigidbody2D.mass;
					}
					if (flag)
					{
						Vector3 b = rigidbody2D.linearVelocity;
						vel += b;
					}
				}
			}
			return num;
		}

		private void GenerateImpactEvent2D(Collider2D other2d, Vector3 vel)
		{
			if (!base.enabled)
			{
				return;
			}
			if (other2d != null)
			{
				int layer = other2d.gameObject.layer;
				if ((1 << layer & this.LayerMask) == 0)
				{
					return;
				}
				if (this.IgnoreTag.Length != 0 && other2d.CompareTag(this.IgnoreTag))
				{
					return;
				}
			}
			float num = this.GetMassAndVelocity2D(other2d, ref vel);
			if (this.ScaleImpactWithSpeed)
			{
				num *= Mathf.Sqrt(vel.magnitude);
			}
			Vector3 a = this.DefaultVelocity;
			if (this.UseImpactDirection && !vel.AlmostZero())
			{
				a = -vel.normalized * a.magnitude;
			}
			base.GenerateImpulseWithVelocity(a * num);
		}

		[Header("Trigger Object Filter")]
		[Tooltip("Only collisions with objects on these layers will generate Impulse events")]
		[FormerlySerializedAs("m_LayerMask")]
		public LayerMask LayerMask = 1;

		[TagField]
		[Tooltip("No Impulse events will be generated for collisions with objects having these tags")]
		[FormerlySerializedAs("m_IgnoreTag")]
		public string IgnoreTag = string.Empty;

		[Header("How To Generate The Impulse")]
		[Tooltip("If checked, signal direction will be affected by the direction of impact")]
		[FormerlySerializedAs("m_UseImpactDirection")]
		public bool UseImpactDirection;

		[Tooltip("If checked, signal amplitude will be multiplied by the mass of the impacting object")]
		[FormerlySerializedAs("m_ScaleImpactWithMass")]
		public bool ScaleImpactWithMass;

		[Tooltip("If checked, signal amplitude will be multiplied by the speed of the impacting object")]
		[FormerlySerializedAs("m_ScaleImpactWithSpeed")]
		public bool ScaleImpactWithSpeed;

		private Rigidbody m_RigidBody;

		private Rigidbody2D m_RigidBody2D;
	}
}
