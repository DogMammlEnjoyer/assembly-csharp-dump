using System;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.BuildingBlocks
{
	public class PointAndLocate : SpaceLocator
	{
		protected override Transform RaycastOrigin
		{
			get
			{
				return this._raycastOrigin;
			}
		}

		public void Locate()
		{
			Pose pose;
			this.TryLocateSpace(out pose);
		}

		protected internal override Ray GetRaycastRay()
		{
			return new Ray(this.RaycastOrigin.position, this.RaycastOrigin.forward);
		}

		[Tooltip("Assign a Transform to use that as raycast origin")]
		[SerializeField]
		internal Transform _raycastOrigin;
	}
}
