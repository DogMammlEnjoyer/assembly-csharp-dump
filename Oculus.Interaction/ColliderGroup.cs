using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ColliderGroup
	{
		public Collider Bounds
		{
			get
			{
				return this._boundsCollider;
			}
		}

		public List<Collider> Colliders
		{
			get
			{
				return this._colliders;
			}
		}

		public ColliderGroup(List<Collider> colliders, Collider boundsCollider)
		{
			this._colliders = colliders;
			this._boundsCollider = boundsCollider;
		}

		private Collider _boundsCollider;

		private List<Collider> _colliders;
	}
}
