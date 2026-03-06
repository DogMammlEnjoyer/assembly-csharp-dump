using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class TwoGrabRotateTransformer : MonoBehaviour, ITransformer
	{
		private Transform PivotTransform
		{
			get
			{
				if (!(this._pivotTransform != null))
				{
					return this._grabbable.Transform;
				}
				return this._pivotTransform;
			}
		}

		public void Initialize(IGrabbable grabbable)
		{
			this._grabbable = grabbable;
		}

		public void BeginTransform()
		{
			Vector3 planeNormal = this.CalculateRotationAxisInWorldSpace();
			this._previousHandsVectorOnPlane = this.CalculateHandsVectorOnPlane(planeNormal);
			this._relativeAngle = this._constrainedRelativeAngle;
		}

		public void UpdateTransform()
		{
			Vector3 vector = this.CalculateRotationAxisInWorldSpace();
			Vector3 vector2 = this.CalculateHandsVectorOnPlane(vector);
			float num = Vector3.SignedAngle(this._previousHandsVectorOnPlane, vector2, vector);
			float constrainedRelativeAngle = this._constrainedRelativeAngle;
			this._relativeAngle += num;
			this._constrainedRelativeAngle = this._relativeAngle;
			if (this._constraints.MinAngle.Constrain)
			{
				this._constrainedRelativeAngle = Mathf.Max(this._constrainedRelativeAngle, this._constraints.MinAngle.Value);
			}
			if (this._constraints.MaxAngle.Constrain)
			{
				this._constrainedRelativeAngle = Mathf.Min(this._constrainedRelativeAngle, this._constraints.MaxAngle.Value);
			}
			num = this._constrainedRelativeAngle - constrainedRelativeAngle;
			this._grabbable.Transform.RotateAround(this.PivotTransform.position, vector, num);
			this._previousHandsVectorOnPlane = vector2;
		}

		public void EndTransform()
		{
		}

		private Vector3 CalculateRotationAxisInWorldSpace()
		{
			Vector3 zero = Vector3.zero;
			zero[(int)this._rotationAxis] = 1f;
			return this.PivotTransform.TransformDirection(zero);
		}

		private Vector3 CalculateHandsVectorOnPlane(Vector3 planeNormal)
		{
			Vector3[] array = new Vector3[]
			{
				Vector3.ProjectOnPlane(this._grabbable.GrabPoints[0].position, planeNormal),
				Vector3.ProjectOnPlane(this._grabbable.GrabPoints[1].position, planeNormal)
			};
			return array[1] - array[0];
		}

		public void InjectOptionalPivotTransform(Transform pivotTransform)
		{
			this._pivotTransform = pivotTransform;
		}

		public void InjectOptionalRotationAxis(TwoGrabRotateTransformer.Axis rotationAxis)
		{
			this._rotationAxis = rotationAxis;
		}

		public void InjectOptionalConstraints(TwoGrabRotateTransformer.TwoGrabRotateConstraints constraints)
		{
			this._constraints = constraints;
		}

		[SerializeField]
		[Optional]
		private Transform _pivotTransform;

		[SerializeField]
		private TwoGrabRotateTransformer.Axis _rotationAxis = TwoGrabRotateTransformer.Axis.Up;

		[SerializeField]
		private TwoGrabRotateTransformer.TwoGrabRotateConstraints _constraints;

		private float _relativeAngle;

		private float _constrainedRelativeAngle;

		private IGrabbable _grabbable;

		private Vector3 _previousHandsVectorOnPlane;

		public enum Axis
		{
			Right,
			Up,
			Forward
		}

		[Serializable]
		public class TwoGrabRotateConstraints
		{
			public FloatConstraint MinAngle;

			public FloatConstraint MaxAngle;
		}
	}
}
