using System;
using System.Collections;
using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class PingPongPaddle : MonoBehaviour, ITransformer
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
				this._leftHandGrabInteractable.WhenStateChanged += this.HandleLeftHandGrabInteractableStateChanged;
				this._rightHandGrabInteractable.WhenStateChanged += this.HandleRightHandGrabInteractableStateChanged;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._collisionEvents.WhenCollisionEnter -= this.HandleCollisionEnter;
				this._leftHandGrabInteractable.WhenStateChanged -= this.HandleLeftHandGrabInteractableStateChanged;
				this._rightHandGrabInteractable.WhenStateChanged -= this.HandleRightHandGrabInteractableStateChanged;
			}
		}

		private void HandleLeftHandGrabInteractableStateChanged(InteractableStateChangeArgs stateChange)
		{
			if (stateChange.NewState == InteractableState.Select)
			{
				this._activeController |= OVRInput.Controller.LTouch;
				return;
			}
			if (stateChange.PreviousState == InteractableState.Select)
			{
				this._activeController &= ~OVRInput.Controller.LTouch;
			}
		}

		private void HandleRightHandGrabInteractableStateChanged(InteractableStateChangeArgs stateChange)
		{
			if (stateChange.NewState == InteractableState.Select)
			{
				this._activeController |= OVRInput.Controller.RTouch;
				return;
			}
			if (stateChange.PreviousState == InteractableState.Select)
			{
				this._activeController &= ~OVRInput.Controller.RTouch;
			}
		}

		private void HandleCollisionEnter(Collision collision)
		{
			this.TryPlayCollisionAudio(collision);
		}

		private void TryPlayCollisionAudio(Collision collision)
		{
			float sqrMagnitude = collision.relativeVelocity.sqrMagnitude;
			if (collision.collider.gameObject == null)
			{
				return;
			}
			float num = Time.time - this._timeAtLastCollision;
			if (0.1f > num)
			{
				return;
			}
			this._timeAtLastCollision = Time.time;
			this.PlayCollisionHaptics(sqrMagnitude);
		}

		private void PlayCollisionHaptics(float strength)
		{
			float pitch = this._collisionStrength.Evaluate(strength);
			base.StartCoroutine(this.HapticsRoutine(pitch, this._activeController));
		}

		private IEnumerator HapticsRoutine(float pitch, OVRInput.Controller controller)
		{
			OVRInput.SetControllerVibration(pitch * 0.5f, pitch * 0.2f, controller);
			yield return this._hapticsWait;
			OVRInput.SetControllerVibration(0f, 0f, controller);
			yield break;
		}

		public void Initialize(IGrabbable grabbable)
		{
			this._grabbable = grabbable;
		}

		public void BeginTransform()
		{
			Pose pose = this._grabbable.GrabPoints[0];
			Transform transform = this._rigidbody.transform;
			this._grabDeltaInLocalSpace = new Pose(transform.InverseTransformVector(pose.position - transform.position), Quaternion.Inverse(pose.rotation) * transform.rotation);
		}

		public void UpdateTransform()
		{
			Pose pose = this._grabbable.GrabPoints[0];
			this._rigidbody.MoveRotation(pose.rotation * this._grabDeltaInLocalSpace.rotation);
			this._rigidbody.MovePosition(pose.position - this._rigidbody.transform.TransformVector(this._grabDeltaInLocalSpace.position));
		}

		public void EndTransform()
		{
		}

		[SerializeField]
		private HandGrabInteractable _leftHandGrabInteractable;

		[SerializeField]
		private HandGrabInteractable _rightHandGrabInteractable;

		[SerializeField]
		private Rigidbody _rigidbody;

		[SerializeField]
		private AnimationCurve _collisionStrength;

		private const float _timeBetweenCollisions = 0.1f;

		private WaitForSeconds _hapticsWait = new WaitForSeconds(0.1f);

		private AudioPhysics.CollisionEvents _collisionEvents;

		private float _timeAtLastCollision;

		protected bool _started;

		private OVRInput.Controller _activeController;

		private IGrabbable _grabbable;

		private Pose _grabDeltaInLocalSpace;
	}
}
