using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTagScripts.Builder
{
	public class RecyclerForceVolume : MonoBehaviour
	{
		private void Awake()
		{
			this.volume = base.GetComponent<Collider>();
			this.hasWindFX = (this.windEffectRenderer != null);
			if (this.hasWindFX)
			{
				this.windEffectRenderer.enabled = false;
			}
		}

		private bool TriggerFilter(Collider other, out Rigidbody rb, out Transform xf)
		{
			rb = null;
			xf = null;
			if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
			{
				rb = GorillaTagger.Instance.GetComponent<Rigidbody>();
				xf = GorillaTagger.Instance.headCollider.GetComponent<Transform>();
			}
			return rb != null && xf != null;
		}

		public void OnTriggerEnter(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			this.enterPos = transform.position;
			ObjectPools.instance.Instantiate(this.windSFX, this.enterPos, true);
			if (this.hasWindFX)
			{
				this.windEffectRenderer.transform.position = base.transform.position + Vector3.Dot(this.enterPos - base.transform.position, base.transform.right) * base.transform.right;
				this.windEffectRenderer.enabled = true;
			}
		}

		public void OnTriggerExit(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			if (this.hasWindFX)
			{
				this.windEffectRenderer.enabled = false;
			}
		}

		public void OnTriggerStay(Collider other)
		{
			Rigidbody rigidbody = null;
			Transform transform = null;
			if (!this.TriggerFilter(other, out rigidbody, out transform))
			{
				return;
			}
			if (this.disableGrip)
			{
				GTPlayer.Instance.SetMaximumSlipThisFrame();
			}
			SizeManager sizeManager = null;
			if (this.scaleWithSize)
			{
				sizeManager = rigidbody.GetComponent<SizeManager>();
			}
			Vector3 vector = rigidbody.linearVelocity;
			if (this.scaleWithSize && sizeManager)
			{
				vector /= sizeManager.currentScale;
			}
			Vector3 a = Vector3.Dot(base.transform.position - transform.position, base.transform.up) * base.transform.up;
			float num = a.magnitude + 0.0001f;
			Vector3 vector2 = a / num;
			float num2 = Vector3.Dot(vector, vector2);
			float d = this.accel;
			if (this.maxDepth > -1f)
			{
				float num3 = Vector3.Dot(transform.position - this.enterPos, vector2);
				float num4 = this.maxDepth - num3;
				float b = 0f;
				if (num4 > 0.0001f)
				{
					b = num2 * num2 / num4;
				}
				d = Mathf.Max(this.accel, b);
			}
			float deltaTime = Time.deltaTime;
			Vector3 b2 = base.transform.forward * d * deltaTime;
			vector += b2;
			Vector3 a2 = Vector3.Dot(vector, base.transform.up) * base.transform.up;
			Vector3 a3 = Vector3.Dot(vector, base.transform.right) * base.transform.right;
			Vector3 b3 = Mathf.Clamp(Vector3.Dot(vector, base.transform.forward), -1f * this.maxSpeed, this.maxSpeed) * base.transform.forward;
			float d2 = 1f;
			float d3 = 1f;
			if (this.dampenLateralVelocity)
			{
				d2 = 1f - this.dampenXVelPerc * 0.01f * deltaTime;
				d3 = 1f - this.dampenYVelPerc * 0.01f * deltaTime;
			}
			vector = d3 * a2 + d2 * a3 + b3;
			if (this.applyPullToCenterAcceleration && this.pullToCenterAccel > 0f && this.pullToCenterMaxSpeed > 0f)
			{
				vector -= num2 * vector2;
				if (num > this.pullTOCenterMinDistance)
				{
					num2 += this.pullToCenterAccel * deltaTime;
					float num5 = Mathf.Min(this.pullToCenterMaxSpeed, num / deltaTime);
					num2 = Mathf.Clamp(num2, -1f * num5, num5);
				}
				else
				{
					num2 = 0f;
				}
				vector += num2 * vector2;
			}
			if (this.scaleWithSize && sizeManager)
			{
				vector *= sizeManager.currentScale;
			}
			rigidbody.linearVelocity = vector;
		}

		[SerializeField]
		public bool scaleWithSize = true;

		[SerializeField]
		private float accel;

		[SerializeField]
		private float maxDepth = -1f;

		[SerializeField]
		private float maxSpeed;

		[SerializeField]
		private bool disableGrip;

		[SerializeField]
		private bool dampenLateralVelocity = true;

		[SerializeField]
		private float dampenXVelPerc;

		[FormerlySerializedAs("dampenZVelPerc")]
		[SerializeField]
		private float dampenYVelPerc;

		[SerializeField]
		private bool applyPullToCenterAcceleration = true;

		[SerializeField]
		private float pullToCenterAccel;

		[SerializeField]
		private float pullToCenterMaxSpeed;

		[SerializeField]
		private float pullTOCenterMinDistance = 0.1f;

		private Collider volume;

		public GameObject windSFX;

		[SerializeField]
		private MeshRenderer windEffectRenderer;

		private bool hasWindFX;

		private Vector3 enterPos;
	}
}
