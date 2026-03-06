using System;
using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction.Samples
{
	public class RespawnOnDrop : MonoBehaviour
	{
		public UnityEvent WhenRespawned
		{
			get
			{
				return this._whenRespawned;
			}
		}

		protected virtual void OnEnable()
		{
			this._initialPosition = base.transform.position;
			this._initialRotation = base.transform.rotation;
			this._initialScale = base.transform.localScale;
			this._freeTransformers = base.GetComponents<TwoGrabFreeTransformer>();
			this._rigidBody = base.GetComponent<Rigidbody>();
		}

		protected virtual void Update()
		{
			if (base.transform.position.y < this._yThresholdForRespawn)
			{
				this.Respawn();
			}
		}

		protected virtual void FixedUpdate()
		{
			if (this._sleepCountDown > 0)
			{
				int num = this._sleepCountDown - 1;
				this._sleepCountDown = num;
				if (num == 0)
				{
					this._rigidBody.isKinematic = false;
				}
			}
		}

		public void Respawn()
		{
			base.transform.position = this._initialPosition;
			base.transform.rotation = this._initialRotation;
			base.transform.localScale = this._initialScale;
			if (this._rigidBody)
			{
				this._rigidBody.velocity = Vector3.zero;
				this._rigidBody.angularVelocity = Vector3.zero;
				if (!this._rigidBody.isKinematic && this._sleepFrames > 0)
				{
					this._sleepCountDown = this._sleepFrames;
					this._rigidBody.isKinematic = true;
				}
			}
			TwoGrabFreeTransformer[] freeTransformers = this._freeTransformers;
			for (int i = 0; i < freeTransformers.Length; i++)
			{
				freeTransformers[i].MarkAsBaseScale();
			}
			this._whenRespawned.Invoke();
		}

		[SerializeField]
		[Tooltip("Respawn will happen when the transform moves below this World Y position.")]
		private float _yThresholdForRespawn;

		[SerializeField]
		[Tooltip("UnityEvent triggered when a respawn occurs.")]
		private UnityEvent _whenRespawned = new UnityEvent();

		[SerializeField]
		[Tooltip("If the transform has an associated rigidbody, make it kinematic during this number of frames after a respawn, in order to avoid ghost collisions.")]
		private int _sleepFrames;

		private Vector3 _initialPosition;

		private Quaternion _initialRotation;

		private Vector3 _initialScale;

		private TwoGrabFreeTransformer[] _freeTransformers;

		private Rigidbody _rigidBody;

		private int _sleepCountDown;
	}
}
