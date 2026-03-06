using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public abstract class SkeletonDebugGizmos : MonoBehaviour
	{
		public float Radius
		{
			get
			{
				return this._radius;
			}
			set
			{
				this._radius = value;
			}
		}

		public SkeletonDebugGizmos.VisibilityFlags Visibility
		{
			get
			{
				return this._visibility;
			}
			set
			{
				this._visibility = value;
			}
		}

		public Color JointColor
		{
			get
			{
				return this._jointColor;
			}
			set
			{
				this._jointColor = value;
			}
		}

		public Color BoneColor
		{
			get
			{
				return this._boneColor;
			}
			set
			{
				this._boneColor = value;
			}
		}

		private float LineWidth
		{
			get
			{
				return this._radius / 2f;
			}
		}

		protected abstract bool TryGetJointPose(int jointId, out Pose pose);

		protected abstract bool TryGetParentJointId(int jointId, out int parent);

		protected bool HasNegativeScale
		{
			get
			{
				return base.transform.lossyScale.x < 0f || base.transform.lossyScale.y < 0f || base.transform.lossyScale.z < 0f;
			}
		}

		protected void Draw(int joint, SkeletonDebugGizmos.VisibilityFlags visibility)
		{
			Pose pose;
			if (this.TryGetJointPose(joint, out pose))
			{
				if (visibility.HasFlag(SkeletonDebugGizmos.VisibilityFlags.Axes))
				{
					DebugGizmos.LineWidth = this.LineWidth;
					DebugGizmos.DrawAxis(pose, this._radius);
				}
				if (visibility.HasFlag(SkeletonDebugGizmos.VisibilityFlags.Joints))
				{
					DebugGizmos.Color = this._jointColor;
					DebugGizmos.LineWidth = this._radius;
					DebugGizmos.DrawPoint(pose.position, null);
				}
				int jointId;
				Pose pose2;
				if (visibility.HasFlag(SkeletonDebugGizmos.VisibilityFlags.Bones) && this.TryGetParentJointId(joint, out jointId) && this.TryGetJointPose(jointId, out pose2))
				{
					DebugGizmos.Color = this._boneColor;
					DebugGizmos.LineWidth = this.LineWidth;
					DebugGizmos.DrawLine(pose.position, pose2.position, null);
				}
			}
		}

		[Tooltip("Which components of the skeleton will be visualized.")]
		[SerializeField]
		private SkeletonDebugGizmos.VisibilityFlags _visibility = SkeletonDebugGizmos.VisibilityFlags.Joints | SkeletonDebugGizmos.VisibilityFlags.Axes;

		[Tooltip("The joint debug spheres will be drawn with this color.")]
		[SerializeField]
		private Color _jointColor = Color.white;

		[Tooltip("The bone connecting lines will be drawn with this color.")]
		[SerializeField]
		private Color _boneColor = Color.gray;

		[Tooltip("The radius of the joint spheres and the thickness of the bone and axis lines.")]
		[SerializeField]
		private float _radius = 0.02f;

		[Flags]
		public enum VisibilityFlags
		{
			Joints = 1,
			Axes = 2,
			Bones = 4
		}
	}
}
