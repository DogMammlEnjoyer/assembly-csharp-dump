using System;
using System.Collections;
using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace Oculus.Interaction
{
	public class PressureBreakable : MonoBehaviour, IHandGrabUseDelegate
	{
		protected virtual void Awake()
		{
			this._unbrokenObject.SetActive(!this._isBroken);
			this._brokenObject.SetActive(this._isBroken);
		}

		protected virtual void Start()
		{
			this._brokenBodiesInitialPoses = new Pose[this._brokenBodies.Length];
			for (int i = 0; i < this._brokenBodies.Length; i++)
			{
				Rigidbody rigidbody = this._brokenBodies[i];
				this._brokenBodiesInitialPoses[i] = new Pose(rigidbody.transform.localPosition, rigidbody.transform.localRotation);
			}
		}

		protected virtual void Update()
		{
			if (this._useStrength >= this._breakThreshold)
			{
				this.Break();
			}
		}

		public void BeginUse()
		{
		}

		public void EndUse()
		{
			this._useStrength = 0f;
		}

		public float ComputeUseStrength(float strength)
		{
			this._useStrength = strength;
			return this._useStrength;
		}

		private void Break()
		{
			if (this._isBroken)
			{
				return;
			}
			this._isBroken = true;
			this._unbrokenObject.SetActive(!this._isBroken);
			HandGrabInteractable[] grabInteractables = this._grabInteractables;
			for (int i = 0; i < grabInteractables.Length; i++)
			{
				grabInteractables[i].Disable();
			}
			this._brokenObject.SetActive(this._isBroken);
			foreach (Rigidbody rigidbody in this._brokenBodies)
			{
				rigidbody.mass = 1f / (float)this._brokenBodies.Length;
				rigidbody.AddExplosionForce(this._explosionForce, base.transform.position, this._explosionRadius);
			}
			base.StartCoroutine(this.Unbreak());
		}

		private IEnumerator Unbreak()
		{
			if (!this._isBroken)
			{
				yield break;
			}
			yield return new WaitForSeconds(this._unbreakDelay);
			this._isBroken = false;
			this._brokenObject.SetActive(this._isBroken);
			for (int i = 0; i < this._brokenBodies.Length; i++)
			{
				Rigidbody rigidbody = this._brokenBodies[i];
				Pose pose = this._brokenBodiesInitialPoses[i];
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
				rigidbody.transform.localPosition = pose.position;
				rigidbody.transform.localRotation = pose.rotation;
			}
			HandGrabInteractable[] grabInteractables = this._grabInteractables;
			for (int j = 0; j < grabInteractables.Length; j++)
			{
				grabInteractables[j].Enable();
			}
			this._unbrokenObject.SetActive(!this._isBroken);
			yield break;
		}

		[SerializeField]
		[Range(0f, 1f)]
		private float _breakThreshold = 0.9f;

		[SerializeField]
		private GameObject _unbrokenObject;

		[SerializeField]
		private GameObject _brokenObject;

		[SerializeField]
		private Rigidbody[] _brokenBodies;

		[SerializeField]
		private HandGrabInteractable[] _grabInteractables;

		[Header("Break Effects")]
		[SerializeField]
		private float _explosionForce = 3f;

		[SerializeField]
		private float _explosionRadius = 0.5f;

		[SerializeField]
		private float _unbreakDelay = 3f;

		private float _useStrength;

		private bool _isBroken;

		private Pose[] _brokenBodiesInitialPoses;
	}
}
