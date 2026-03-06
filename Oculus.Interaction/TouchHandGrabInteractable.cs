using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class TouchHandGrabInteractable : PointerInteractable<TouchHandGrabInteractor, TouchHandGrabInteractable>
	{
		public ColliderGroup ColliderGroup
		{
			get
			{
				return this._colliderGroup;
			}
		}

		protected override void Start()
		{
			base.Start();
			this._colliderGroup = new ColliderGroup(this._colliders, this._boundsCollider);
		}

		public void InjectAllTouchHandGrabInteractable(Collider boundsCollider, List<Collider> colliders)
		{
			this.InjectBoundsCollider(boundsCollider);
			this.InjectColliders(colliders);
		}

		public void InjectBoundsCollider(Collider boundsCollider)
		{
			this._boundsCollider = boundsCollider;
		}

		public void InjectColliders(List<Collider> colliders)
		{
			this._colliders = colliders;
		}

		[SerializeField]
		private Collider _boundsCollider;

		[SerializeField]
		private List<Collider> _colliders;

		private ColliderGroup _colliderGroup;
	}
}
