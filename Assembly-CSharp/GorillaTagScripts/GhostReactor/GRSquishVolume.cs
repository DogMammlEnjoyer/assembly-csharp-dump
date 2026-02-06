using System;
using System.Collections;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor
{
	public class GRSquishVolume : MonoBehaviour, IGorillaSliceableSimple
	{
		private void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this);
		}

		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
		}

		private void Start()
		{
			this.SetCollider(false);
			this.SetTentacleColliders(true);
			this.moonBoss = base.GetComponentInParent<GREnemyBossMoon>();
			if (this.moonBoss != null && !this.moonBoss.squishVolumes.Contains(this))
			{
				this.moonBoss.squishVolumes.Add(this);
			}
		}

		public void SliceUpdate()
		{
			this.SetCollider(!this.overrideDisabled && base.transform.position.y < this.squishHeight && Vector3.Angle(-base.transform.forward, Quaternion.Euler(this.rotationOffset) * Vector3.down) < this.facingDownDegrees);
		}

		public void SetCollider(bool colliderEnabled)
		{
			this._collider.enabled = colliderEnabled;
		}

		private void OnTriggerEnter(Collider other)
		{
			GRPlayer y;
			if (!other.gameObject.TryGetComponentInParent(out y))
			{
				return;
			}
			if (GRPlayer.GetLocal() != y)
			{
				return;
			}
			if (this._reenableCoroutine != null)
			{
				return;
			}
			this.SetTentacleColliders(false);
			GTPlayer.Instance.DoLaunch(this.GetLaunchVector());
			this.moonBoss.HitPlayer(GRPlayer.GetLocal(), false);
			this._reenableCoroutine = base.StartCoroutine(this.ReenableCoroutine());
			this.moonBoss.SetSquishVolumeState(false);
		}

		private void SetTentacleColliders(bool enabled)
		{
			Collider[] collidersToDisable = this._collidersToDisable;
			for (int i = 0; i < collidersToDisable.Length; i++)
			{
				collidersToDisable[i].enabled = enabled;
			}
		}

		private IEnumerator ReenableCoroutine()
		{
			yield return new WaitForSeconds(this._reenableDelay);
			this.SetTentacleColliders(true);
			this._reenableCoroutine = null;
			this.moonBoss.SetSquishVolumeState(true);
			yield break;
		}

		private Vector3 GetLaunchVector()
		{
			Vector3 position = GRPlayer.GetLocal().transform.position;
			Vector3 lhs = position - base.transform.position;
			Vector3 b = base.transform.position + base.transform.right * Vector3.Dot(lhs, base.transform.right);
			Vector3 normalized = (position - b).normalized;
			Vector3 normalized2 = new Vector3(normalized.x, 0f, normalized.y).normalized;
			float maxRadiansDelta = Random.Range(this._launchDeflectionDegrees / 2f, this._launchDeflectionDegrees) * 0.017453292f;
			Vector3 vector = Vector3.RotateTowards(normalized2, Vector3.up, maxRadiansDelta, 0f);
			return this._launchStrength * vector.normalized;
		}

		[SerializeField]
		private Collider _collider;

		[SerializeField]
		private Collider[] _collidersToDisable;

		[SerializeField]
		private float _reenableDelay = 1f;

		[SerializeField]
		private float _launchStrength = 8f;

		[SerializeField]
		private float _launchDeflectionDegrees = 10f;

		private Coroutine _reenableCoroutine;

		private GREnemyBossMoon moonBoss;

		public float squishHeight;

		public Vector3 rotationOffset;

		public float facingDownDegrees = 20f;

		public bool overrideDisabled;
	}
}
