using System;
using System.Collections.Generic;
using Oculus.Interaction.Surfaces;
using UnityEngine;

namespace Oculus.Interaction
{
	public class SurfaceSnapPoseDelegate : MonoBehaviour, ISnapPoseDelegate
	{
		protected virtual void Awake()
		{
			this.Surface = (this._surface as ISurface);
			this._snappedPoses = new Dictionary<int, Pose>();
		}

		protected virtual void Start()
		{
		}

		public void TrackElement(int id, Pose p)
		{
		}

		public void UntrackElement(int id)
		{
		}

		private bool ComputeWorldSurfacePose(Pose pose, out Pose result)
		{
			SurfaceHit surfaceHit;
			if (this.Surface.ClosestSurfacePoint(pose.position, out surfaceHit, 0f))
			{
				result = new Pose(surfaceHit.Point, Quaternion.LookRotation(surfaceHit.Normal, pose.up));
				return true;
			}
			result = pose;
			return false;
		}

		private bool ComputeLocalSurfacePose(Pose pose, out Pose result)
		{
			Pose pose2;
			if (this.ComputeWorldSurfacePose(pose, out pose2))
			{
				result = new Pose(this.Surface.Transform.InverseTransformPoint(pose2.position), Quaternion.Inverse(pose2.rotation) * this.Surface.Transform.rotation);
				return true;
			}
			result = pose;
			return false;
		}

		public void SnapElement(int id, Pose pose)
		{
			Pose value;
			if (this.ComputeLocalSurfacePose(pose, out value))
			{
				this._snappedPoses.Add(id, value);
				return;
			}
			this._snappedPoses.Add(id, pose);
		}

		public void UnsnapElement(int id)
		{
			this._snappedPoses.Remove(id);
		}

		public void MoveTrackedElement(int id, Pose p)
		{
		}

		public bool SnapPoseForElement(int id, Pose pose, out Pose result)
		{
			Pose pose2;
			if (this._snappedPoses.TryGetValue(id, out pose2))
			{
				result = new Pose(this.Surface.Transform.TransformPoint(pose2.position), this.Surface.Transform.rotation * pose2.rotation);
				return true;
			}
			return this.ComputeWorldSurfacePose(pose, out result);
		}

		public void InjectAllSurfaceSnapPoseDelegate(ISurface surface)
		{
			this.InjectSurface(surface);
		}

		public void InjectSurface(ISurface surface)
		{
			this._surface = (surface as Object);
			this.Surface = surface;
		}

		[SerializeField]
		[Interface(typeof(ISurface), new Type[]
		{

		})]
		private Object _surface;

		protected ISurface Surface;

		private Dictionary<int, Pose> _snappedPoses;
	}
}
