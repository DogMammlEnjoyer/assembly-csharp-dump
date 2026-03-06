using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.Grab.GrabSurfaces
{
	public class ColliderGrabSurface : MonoBehaviour, IGrabSurface
	{
		protected virtual void Start()
		{
		}

		private Vector3 NearestPointInSurface(Vector3 targetPosition)
		{
			if (this._collider.bounds.Contains(targetPosition))
			{
				targetPosition = this._collider.ClosestPointOnBounds(targetPosition);
			}
			return this._collider.ClosestPoint(targetPosition);
		}

		public GrabPoseScore CalculateBestPoseAtSurface(in Pose targetPose, out Pose bestPose, in PoseMeasureParameters scoringModifier, Transform relativeTo)
		{
			Pose identity = Pose.identity;
			return this.CalculateBestPoseAtSurface(targetPose, identity, out bestPose, scoringModifier, relativeTo);
		}

		public GrabPoseScore CalculateBestPoseAtSurface(in Pose targetPose, in Pose offset, out Pose bestPose, in PoseMeasureParameters scoringModifier, Transform relativeTo)
		{
			Vector3 position = this.NearestPointInSurface(targetPose.position);
			bestPose = new Pose(position, targetPose.rotation);
			return new GrabPoseScore(ref targetPose, ref bestPose, ref offset, scoringModifier);
		}

		public bool CalculateBestPoseAtSurface(Ray targetRay, out Pose bestPose, Transform relativeTo)
		{
			RaycastHit raycastHit;
			if (this._collider.Raycast(targetRay, out raycastHit, float.PositiveInfinity))
			{
				bestPose.position = raycastHit.point;
				bestPose.rotation = relativeTo.rotation;
				return true;
			}
			bestPose = Pose.identity;
			return false;
		}

		public Pose MirrorPose(in Pose gripPose, Transform relativeTo)
		{
			return HandMirroring.Mirror(gripPose);
		}

		public IGrabSurface CreateMirroredSurface(GameObject gameObject)
		{
			return this.CreateDuplicatedSurface(gameObject);
		}

		public IGrabSurface CreateDuplicatedSurface(GameObject gameObject)
		{
			ColliderGrabSurface colliderGrabSurface = gameObject.AddComponent<ColliderGrabSurface>();
			colliderGrabSurface.InjectAllColliderGrabSurface(this._collider);
			return colliderGrabSurface;
		}

		public void InjectAllColliderGrabSurface(Collider collider)
		{
			this.InjectCollider(collider);
		}

		public void InjectCollider(Collider collider)
		{
			this._collider = collider;
		}

		GrabPoseScore IGrabSurface.CalculateBestPoseAtSurface(in Pose targetPose, out Pose bestPose, in PoseMeasureParameters scoringModifier, Transform relativeTo)
		{
			return this.CalculateBestPoseAtSurface(targetPose, out bestPose, scoringModifier, relativeTo);
		}

		GrabPoseScore IGrabSurface.CalculateBestPoseAtSurface(in Pose targetPose, in Pose offset, out Pose bestPose, in PoseMeasureParameters scoringModifier, Transform relativeTo)
		{
			return this.CalculateBestPoseAtSurface(targetPose, offset, out bestPose, scoringModifier, relativeTo);
		}

		Pose IGrabSurface.MirrorPose(in Pose gripPose, Transform relativeTo)
		{
			return this.MirrorPose(gripPose, relativeTo);
		}

		[SerializeField]
		private Collider _collider;
	}
}
