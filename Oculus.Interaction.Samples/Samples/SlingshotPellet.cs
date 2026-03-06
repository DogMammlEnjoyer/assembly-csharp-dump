using System;
using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class SlingshotPellet : MonoBehaviour
	{
		public HandGrabInteractor HandGrabber
		{
			get
			{
				return this._lastHandGrabInteractor;
			}
		}

		private void Awake()
		{
			this.Identifier = UniqueIdentifier.Generate(Context.Global.GetInstance(), this);
		}

		private void OnEnable()
		{
			HandGrabInteractable[] handGrabInteractables = this._handGrabInteractables;
			for (int i = 0; i < handGrabInteractables.Length; i++)
			{
				handGrabInteractables[i].WhenSelectingInteractorAdded.Action += this.HandleSelectingHandGrabInteractorAdded;
			}
		}

		private void OnDisable()
		{
			HandGrabInteractable[] handGrabInteractables = this._handGrabInteractables;
			for (int i = 0; i < handGrabInteractables.Length; i++)
			{
				handGrabInteractables[i].WhenSelectingInteractorAdded.Action -= this.HandleSelectingHandGrabInteractorAdded;
			}
		}

		private void HandleSelectingHandGrabInteractorAdded(HandGrabInteractor interactor)
		{
			this._lastHandGrabInteractor = interactor;
		}

		public void Attach()
		{
			Pose pose = base.transform.GetPose(Space.World);
			this.grabbable.ProcessPointerEvent(new PointerEvent(this.Identifier.ID, PointerEventType.Hover, pose, null));
			this.grabbable.ProcessPointerEvent(new PointerEvent(this.Identifier.ID, PointerEventType.Select, pose, null));
			this.grabbable.ProcessPointerEvent(new PointerEvent(this.Identifier.ID, PointerEventType.Move, pose, null));
		}

		public void Move(Transform transform)
		{
			this.grabbable.ProcessPointerEvent(new PointerEvent(this.Identifier.ID, PointerEventType.Move, transform.GetPose(Space.World), null));
		}

		public void Eject(Vector3 force)
		{
			this.grabbable.ProcessPointerEvent(new PointerEvent(this.Identifier.ID, PointerEventType.Cancel, base.transform.GetPose(Space.World), null));
			this._linearVelocity = force;
			this._hasPendingForce = true;
		}

		private void FixedUpdate()
		{
			if (this._hasPendingForce)
			{
				this._hasPendingForce = false;
				this._rigidbody.AddForce(this._linearVelocity, ForceMode.VelocityChange);
				this._rigidbody.AddTorque(Vector3.zero, ForceMode.VelocityChange);
			}
		}

		[SerializeField]
		private Rigidbody _rigidbody;

		[SerializeField]
		private Grabbable grabbable;

		[SerializeField]
		private HandGrabInteractable[] _handGrabInteractables;

		private HandGrabInteractor _lastHandGrabInteractor;

		private UniqueIdentifier Identifier;

		private bool _hasPendingForce;

		private Vector3 _linearVelocity;
	}
}
