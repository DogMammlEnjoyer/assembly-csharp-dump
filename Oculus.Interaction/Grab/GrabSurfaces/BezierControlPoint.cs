using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction.Grab.GrabSurfaces
{
	[Serializable]
	public struct BezierControlPoint
	{
		public bool Disconnected
		{
			get
			{
				return this._disconnected;
			}
			set
			{
				this._disconnected = value;
			}
		}

		public Pose GetPose(Transform relativeTo)
		{
			return PoseUtils.GlobalPoseScaled(relativeTo, this._pose);
		}

		public void SetPose(in Pose worldSpacePose, Transform relativeTo)
		{
			this._pose = PoseUtils.DeltaScaled(relativeTo, worldSpacePose);
		}

		public Vector3 GetTangent(Transform relativeTo)
		{
			return relativeTo.TransformPoint(this._tangentPoint);
		}

		public void SetTangent(in Vector3 tangent, Transform relativeTo)
		{
			this._tangentPoint = relativeTo.InverseTransformPoint(tangent);
		}

		[SerializeField]
		[FormerlySerializedAs("pose")]
		private Pose _pose;

		[SerializeField]
		[FormerlySerializedAs("tangentPoint")]
		private Vector3 _tangentPoint;

		[SerializeField]
		[FormerlySerializedAs("disconnected")]
		private bool _disconnected;
	}
}
