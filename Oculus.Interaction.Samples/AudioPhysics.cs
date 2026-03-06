using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class AudioPhysics : MonoBehaviour
	{
		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._collisionEvents = this._rigidbody.gameObject.AddComponent<AudioPhysics.CollisionEvents>();
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._collisionEvents.WhenCollisionEnter += this.HandleCollisionEnter;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._collisionEvents.WhenCollisionEnter -= this.HandleCollisionEnter;
			}
		}

		protected virtual void OnDestroy()
		{
			if (this._collisionEvents != null)
			{
				Object.Destroy(this._collisionEvents);
			}
		}

		private void HandleCollisionEnter(Collision collision)
		{
			this.TryPlayCollisionAudio(collision, this._rigidbody);
		}

		private void TryPlayCollisionAudio(Collision collision, Rigidbody rigidbody)
		{
			float sqrMagnitude = collision.relativeVelocity.sqrMagnitude;
			if (collision.collider.gameObject == null)
			{
				return;
			}
			float num = Time.time - this._timeAtLastCollision;
			if (this._timeBetweenCollisions > num)
			{
				return;
			}
			AudioPhysics target;
			if (!this._allowMultipleCollisions && collision.collider.gameObject.TryGetComponent<AudioPhysics>(out target) && AudioPhysics.GetObjectVelocity(target) > AudioPhysics.GetObjectVelocity(this))
			{
				return;
			}
			this._timeAtLastCollision = Time.time;
			this.PlayCollisionAudio(this._impactAudioEvents, sqrMagnitude);
		}

		private void PlayCollisionAudio(ImpactAudio impactAudio, float magnitude)
		{
			if (impactAudio.HardCollisionSound == null || impactAudio.SoftCollisionSound == null)
			{
				return;
			}
			if (magnitude > this._minimumVelocity)
			{
				if (magnitude > this._velocitySplit && impactAudio.HardCollisionSound != null)
				{
					this.PlaySoundOnAudioTrigger(impactAudio.HardCollisionSound);
					return;
				}
				this.PlaySoundOnAudioTrigger(impactAudio.SoftCollisionSound);
			}
		}

		private static float GetObjectVelocity(AudioPhysics target)
		{
			return target._rigidbody.velocity.sqrMagnitude;
		}

		private void PlaySoundOnAudioTrigger(AudioTrigger audioTrigger)
		{
			if (audioTrigger != null)
			{
				audioTrigger.PlayAudio();
			}
		}

		[Tooltip("Add a reference to the rigidbody on this gameobject.")]
		[SerializeField]
		private Rigidbody _rigidbody;

		[Tooltip("Reference an audio trigger instance for soft and hard collisions.")]
		[SerializeField]
		private ImpactAudio _impactAudioEvents;

		[Tooltip("Collisions below this value will play a soft audio event, and collisions above will play a hard audio event.")]
		[Range(0f, 8f)]
		[SerializeField]
		private float _velocitySplit = 1f;

		[Tooltip("Collisions below this value will be ignored and will not play audio.")]
		[Range(0f, 2f)]
		[SerializeField]
		private float _minimumVelocity;

		[Tooltip("The shortest amount of time in seconds between collisions. Used to cull multiple fast collision events.")]
		[Range(0f, 2f)]
		[SerializeField]
		private float _timeBetweenCollisions = 0.2f;

		[Tooltip("By default (false), when two physics objects collide with physics audio components, we only play the one with the higher velocity.Setting this to true will allow both impacts to play.")]
		[SerializeField]
		private bool _allowMultipleCollisions;

		private float _timeAtLastCollision;

		protected bool _started;

		private AudioPhysics.CollisionEvents _collisionEvents;

		public class CollisionEvents : MonoBehaviour
		{
			public event Action<Collision> WhenCollisionEnter = delegate(Collision <p0>)
			{
			};

			private void OnCollisionEnter(Collision collision)
			{
				this.WhenCollisionEnter(collision);
			}
		}
	}
}
