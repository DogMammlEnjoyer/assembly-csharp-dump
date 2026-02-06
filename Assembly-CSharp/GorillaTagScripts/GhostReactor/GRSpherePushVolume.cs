using System;
using System.Collections;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	[RequireComponent(typeof(SphereCollider))]
	public class GRSpherePushVolume : MonoBehaviour
	{
		private void Awake()
		{
			this._collider = base.GetComponent<SphereCollider>();
			this._collider.enabled = false;
		}

		public void Trigger()
		{
			this._collider.enabled = true;
			base.StartCoroutine(this.DisableCoroutine());
		}

		private void OnTriggerStay(Collider other)
		{
			if (this._localFlung)
			{
				return;
			}
			if (this._coroutine != null)
			{
				return;
			}
			GRPlayer y;
			if (!other.gameObject.TryGetComponentInParent(out y))
			{
				return;
			}
			if (GRPlayer.GetLocal() != y)
			{
				return;
			}
			this._coroutine = base.StartCoroutine(this.ActionCoroutine(other));
			this._collider.enabled = false;
		}

		private IEnumerator ActionCoroutine(Collider other)
		{
			yield return new WaitForSeconds(this._pushDelay);
			Vector3 velocity = this.CalculatePushVector(other);
			GTPlayer.Instance.DoLaunch(velocity);
			this._localFlung = true;
			yield return new WaitForSeconds(this._pushCooldown);
			this._localFlung = false;
			this._coroutine = null;
			yield break;
		}

		private IEnumerator DisableCoroutine()
		{
			yield return new WaitForSeconds(this._disableAfter);
			this._collider.enabled = false;
			yield break;
		}

		private Vector3 CalculatePushVector(Collider other)
		{
			GRSpherePushVolume.PushKind pushKind = this._pushKind;
			Vector3 result;
			if (pushKind != GRSpherePushVolume.PushKind.Radial)
			{
				if (pushKind != GRSpherePushVolume.PushKind.UpAndOut)
				{
					throw new NotImplementedException();
				}
				result = this.CalculateUpAndOutPushVector(other);
			}
			else
			{
				result = this.CalculateRadialPushVector(other);
			}
			return result;
		}

		private Vector3 CalculateRadialPushVector(Collider other)
		{
			Vector3 vector = other.gameObject.transform.position - base.transform.position;
			float time = vector.magnitude / this._collider.radius;
			return this._pushScaling.Evaluate(time) * this._pushForce * vector.normalized;
		}

		private Vector3 CalculateUpAndOutPushVector(Collider other)
		{
			Vector3 vector = new Vector3(other.gameObject.transform.position.x - base.transform.position.x, 0f, other.gameObject.transform.position.z - base.transform.position.z);
			float time = vector.magnitude / this._collider.radius;
			vector.Normalize();
			Vector3.RotateTowards(vector, Vector3.up, 0.7853982f, 0f);
			vector *= this._pushForce * this._pushScaling.Evaluate(time);
			return vector;
		}

		[SerializeField]
		private GRSpherePushVolume.PushKind _pushKind;

		[SerializeField]
		private float _pushDelay;

		[SerializeField]
		private float _pushCooldown = 1f;

		[SerializeField]
		private AnimationCurve _pushScaling = AnimationCurve.Constant(0f, 1f, 1f);

		[SerializeField]
		private float _pushForce = 1f;

		[SerializeField]
		private float _disableAfter = 3f;

		private SphereCollider _collider;

		private bool _localFlung;

		private Coroutine _coroutine;

		public enum PushKind
		{
			Radial,
			UpAndOut
		}
	}
}
