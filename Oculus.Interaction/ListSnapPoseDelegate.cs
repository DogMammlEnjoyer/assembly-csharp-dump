using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ListSnapPoseDelegate : MonoBehaviour, ISnapPoseDelegate
	{
		protected virtual void Start()
		{
			this._snappedIds = new HashSet<int>();
			this._layout = new ListLayout();
			this._layoutEase = new ListLayoutEase(this._layout, 0.3f, null);
			this._layoutEase.UpdateTime(Time.timeSinceLevelLoad);
		}

		protected virtual void Update()
		{
			this._layoutEase.UpdateTime(Time.timeSinceLevelLoad);
		}

		protected virtual float SizeForId(int id)
		{
			return this._defaultSize;
		}

		protected virtual float FloatForPose(Pose pose)
		{
			return base.transform.InverseTransformPoint(pose.position).x;
		}

		protected virtual Pose PoseForFloat(float position)
		{
			return new Pose(base.transform.TransformPoint(new Vector3(position, 0f, 0f)), base.transform.rotation);
		}

		public void TrackElement(int id, Pose p)
		{
			this._layout.AddElement(id, this.SizeForId(id), this.FloatForPose(p));
		}

		public void UntrackElement(int id)
		{
			this._layout.RemoveElement(id);
		}

		public void SnapElement(int id, Pose pose)
		{
			this._snappedIds.Add(id);
		}

		public void UnsnapElement(int id)
		{
			this._snappedIds.Remove(id);
		}

		public void MoveTrackedElement(int id, Pose p)
		{
			this._layout.MoveElement(id, this.FloatForPose(p));
		}

		public bool SnapPoseForElement(int id, Pose pose, out Pose result)
		{
			if (this._snappedIds.Contains(id))
			{
				result = this.PoseForFloat(this._layoutEase.GetPosition(id));
			}
			else
			{
				result = this.PoseForFloat(this._layout.GetTargetPosition(id, this.FloatForPose(pose), this.SizeForId(id)));
			}
			return true;
		}

		public float Size
		{
			get
			{
				return this._layout.Size;
			}
		}

		private HashSet<int> _snappedIds;

		private ListLayout _layout;

		private ListLayoutEase _layoutEase;

		[SerializeField]
		private float _defaultSize = 1f;
	}
}
