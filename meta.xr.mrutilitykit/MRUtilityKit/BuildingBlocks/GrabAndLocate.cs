using System;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.BuildingBlocks
{
	public class GrabAndLocate : SpaceLocator
	{
		protected override Transform RaycastOrigin
		{
			get
			{
				return base.transform;
			}
		}

		protected override float MaxRaycastDistance
		{
			get
			{
				return 3f;
			}
		}

		public void Awake()
		{
			this._handGrabInteractable = base.GetComponentInChildren<HandGrabInteractable>();
			this._grabInteractable = base.GetComponentInChildren<GrabInteractable>();
			this._placeWithAnchor = base.GetComponent<PlaceWithAnchor>();
			this._cameraRig = Object.FindFirstObjectByType<OVRCameraRig>();
		}

		private void OnEnable()
		{
			this._handGrabInteractable.WhenStateChanged += this.OnInteractableStateChanged;
			this._grabInteractable.WhenStateChanged += this.OnInteractableStateChanged;
		}

		private void OnDisable()
		{
			this._handGrabInteractable.WhenStateChanged -= this.OnInteractableStateChanged;
			this._grabInteractable.WhenStateChanged -= this.OnInteractableStateChanged;
		}

		private void OnInteractableStateChanged(InteractableStateChangeArgs stateChange)
		{
			if (stateChange.PreviousState == InteractableState.Select)
			{
				Pose pose;
				this.TryLocateSpace(out pose);
			}
		}

		protected internal override Ray GetRaycastRay()
		{
			Vector3 origin = base.transform.position + base.transform.up * 0.5f;
			Vector3 direction = -base.transform.up;
			return new Ray(origin, direction);
		}

		private HandGrabInteractable _handGrabInteractable;

		private GrabInteractable _grabInteractable;

		private PlaceWithAnchor _placeWithAnchor;

		private OVRCameraRig _cameraRig;

		private bool _requestMove;
	}
}
