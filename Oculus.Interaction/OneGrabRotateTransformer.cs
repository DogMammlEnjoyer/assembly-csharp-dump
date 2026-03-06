using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class OneGrabRotateTransformer : MonoBehaviour, ITransformer
	{
		public Transform Pivot
		{
			get
			{
				if (!(this._pivotTransform != null))
				{
					return base.transform;
				}
				return this._pivotTransform;
			}
		}

		public OneGrabRotateTransformer.Axis RotationAxis
		{
			get
			{
				return this._rotationAxis;
			}
		}

		public OneGrabRotateTransformer.OneGrabRotateConstraints Constraints
		{
			get
			{
				return this._constraints;
			}
			set
			{
				this._constraints = value;
			}
		}

		public void Initialize(IGrabbable grabbable)
		{
			this._grabbable = grabbable;
		}

		public Pose ComputeWorldPivotPose()
		{
			if (this._pivotTransform != null)
			{
				return this._pivotTransform.GetPose(Space.World);
			}
			Transform transform = this._grabbable.Transform;
			Vector3 position = transform.position;
			Quaternion rotation = (transform.parent != null) ? (transform.parent.rotation * this._localRotation) : this._localRotation;
			return new Pose(position, rotation);
		}

		public void BeginTransform()
		{
			ref Pose ptr = this._grabbable.GrabPoints[0];
			Transform transform = this._grabbable.Transform;
			if (this._pivotTransform == null)
			{
				this._localRotation = transform.localRotation;
			}
			Vector3 zero = Vector3.zero;
			zero[(int)this._rotationAxis] = 1f;
			this._worldPivotPose = this.ComputeWorldPivotPose();
			Vector3 planeNormal = this._worldPivotPose.rotation * zero;
			Quaternion quaternion = Quaternion.Inverse(this._worldPivotPose.rotation);
			Vector3 point = ptr.position - this._worldPivotPose.position;
			if (Mathf.Abs(point.magnitude) < 0.001f)
			{
				Vector3 zero2 = Vector3.zero;
				zero2[(int)((this._rotationAxis + 1) % (OneGrabRotateTransformer.Axis)3)] = 0.001f;
				point = this._worldPivotPose.rotation * zero2;
			}
			this._grabPositionInPivotSpace = quaternion * point;
			Vector3 position = quaternion * (transform.position - this._worldPivotPose.position);
			Quaternion rotation = quaternion * transform.rotation;
			this._transformPoseInPivotSpace = new Pose(position, rotation);
			Vector3 point2 = Vector3.ProjectOnPlane(this._worldPivotPose.rotation * this._grabPositionInPivotSpace, planeNormal);
			this._previousVectorInPivotSpace = Quaternion.Inverse(this._worldPivotPose.rotation) * point2;
			this._startAngle = this._constrainedRelativeAngle;
			this._relativeAngle = this._startAngle;
			float d = (transform.parent != null) ? transform.parent.lossyScale.x : 1f;
			this._transformPoseInPivotSpace.position = this._transformPoseInPivotSpace.position / d;
		}

		public void UpdateTransform()
		{
			ref Pose ptr = this._grabbable.GrabPoints[0];
			Transform transform = this._grabbable.Transform;
			Vector3 zero = Vector3.zero;
			zero[(int)this._rotationAxis] = 1f;
			this._worldPivotPose = this.ComputeWorldPivotPose();
			Vector3 vector = this._worldPivotPose.rotation * zero;
			Vector3 vector2 = Vector3.ProjectOnPlane(ptr.position - this._worldPivotPose.position, vector);
			Vector3 from = this._worldPivotPose.rotation * this._previousVectorInPivotSpace;
			this._previousVectorInPivotSpace = Quaternion.Inverse(this._worldPivotPose.rotation) * vector2;
			float num = Vector3.SignedAngle(from, vector2, vector);
			this._relativeAngle += num;
			this._constrainedRelativeAngle = this._relativeAngle;
			if (this.Constraints.MinAngle.Constrain)
			{
				this._constrainedRelativeAngle = Mathf.Max(this._constrainedRelativeAngle, this.Constraints.MinAngle.Value);
			}
			if (this.Constraints.MaxAngle.Constrain)
			{
				this._constrainedRelativeAngle = Mathf.Min(this._constrainedRelativeAngle, this.Constraints.MaxAngle.Value);
			}
			Quaternion quaternion = Quaternion.AngleAxis(this._constrainedRelativeAngle - this._startAngle, vector);
			float d = (transform.parent != null) ? transform.parent.lossyScale.x : 1f;
			Pose pose = new Pose(this._worldPivotPose.rotation * (d * this._transformPoseInPivotSpace.position), this._worldPivotPose.rotation * this._transformPoseInPivotSpace.rotation);
			Pose pose2 = new Pose(quaternion * pose.position, quaternion * pose.rotation);
			transform.position = this._worldPivotPose.position + pose2.position;
			transform.rotation = pose2.rotation;
		}

		public void EndTransform()
		{
		}

		public void InjectOptionalPivotTransform(Transform pivotTransform)
		{
			this._pivotTransform = pivotTransform;
		}

		public void InjectOptionalRotationAxis(OneGrabRotateTransformer.Axis rotationAxis)
		{
			this._rotationAxis = rotationAxis;
		}

		public void InjectOptionalConstraints(OneGrabRotateTransformer.OneGrabRotateConstraints constraints)
		{
			this._constraints = constraints;
		}

		[SerializeField]
		[Optional]
		private Transform _pivotTransform;

		[SerializeField]
		private OneGrabRotateTransformer.Axis _rotationAxis = OneGrabRotateTransformer.Axis.Up;

		[SerializeField]
		private OneGrabRotateTransformer.OneGrabRotateConstraints _constraints = new OneGrabRotateTransformer.OneGrabRotateConstraints
		{
			MinAngle = new FloatConstraint(),
			MaxAngle = new FloatConstraint()
		};

		private float _relativeAngle;

		private float _constrainedRelativeAngle;

		private IGrabbable _grabbable;

		private Vector3 _grabPositionInPivotSpace;

		private Pose _transformPoseInPivotSpace;

		private Pose _worldPivotPose;

		private Vector3 _previousVectorInPivotSpace;

		private Quaternion _localRotation;

		private float _startAngle;

		public enum Axis
		{
			Right,
			Up,
			Forward
		}

		[Serializable]
		public class OneGrabRotateConstraints
		{
			public FloatConstraint MinAngle;

			public FloatConstraint MaxAngle;
		}
	}
}
