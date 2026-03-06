using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class TwoGrabPlaneTransformer : MonoBehaviour, ITransformer
	{
		public TwoGrabPlaneTransformer.TwoGrabPlaneConstraints Constraints
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

		private Vector3 WorldPlaneNormal()
		{
			return ((this._planeTransform != null) ? this._planeTransform : this._grabbable.Transform).TransformDirection(this._localPlaneNormal).normalized;
		}

		public void BeginTransform()
		{
			Transform transform = this._grabbable.Transform;
			ref Pose ptr = this._grabbable.GrabPoints[0];
			Pose pose = this._grabbable.GrabPoints[1];
			Vector3 planeNormal = this.WorldPlaneNormal();
			TwoGrabPlaneTransformer.TwoGrabPlaneState twoGrabPlaneState = TwoGrabPlaneTransformer.TwoGrabPlane(ptr.position, pose.position, planeNormal);
			this._localToTarget = TransformerUtils.WorldToLocalPose(twoGrabPlaneState.Center, transform.worldToLocalMatrix);
			this._localMagnitudeToTarget = TransformerUtils.WorldToLocalMagnitude(twoGrabPlaneState.PlanarDistance, transform.worldToLocalMatrix);
		}

		public void UpdateTransform()
		{
			Transform transform = this._grabbable.Transform;
			ref Pose ptr = this._grabbable.GrabPoints[0];
			Pose pose = this._grabbable.GrabPoints[1];
			Vector3 vector = this.WorldPlaneNormal();
			TwoGrabPlaneTransformer.TwoGrabPlaneState twoGrabPlaneState = TwoGrabPlaneTransformer.TwoGrabPlane(ptr.position, pose.position, vector);
			float num = TransformerUtils.LocalToWorldMagnitude(this._localMagnitudeToTarget, transform.localToWorldMatrix);
			float num2 = ((num != 0f) ? (twoGrabPlaneState.PlanarDistance / num) : 1f) * transform.localScale.x;
			if (this._constraints.MinScale.Constrain)
			{
				num2 = Mathf.Max(this._constraints.MinScale.Value, num2);
			}
			if (this._constraints.MaxScale.Constrain)
			{
				num2 = Mathf.Min(this._constraints.MaxScale.Value, num2);
			}
			transform.localScale = num2 / transform.localScale.x * transform.localScale;
			Pose pose2 = TransformerUtils.AlignLocalToWorldPose(transform.localToWorldMatrix, this._localToTarget, twoGrabPlaneState.Center);
			transform.position = pose2.position;
			transform.rotation = pose2.rotation;
			transform.position = TransformerUtils.ConstrainAlongDirection(transform.position, (transform.parent != null) ? transform.parent.position : Vector3.zero, vector, this._constraints.MinY, this._constraints.MaxY);
		}

		public void EndTransform()
		{
		}

		public static TwoGrabPlaneTransformer.TwoGrabPlaneState TwoGrabPlane(Vector3 p0, Vector3 p1, Vector3 planeNormal)
		{
			Vector3 position = p0 * 0.5f + p1 * 0.5f;
			Vector3 b = Vector3.ProjectOnPlane(p0, planeNormal);
			Vector3 forward = Vector3.ProjectOnPlane(p1, planeNormal) - b;
			Quaternion rotation = Quaternion.LookRotation(forward, planeNormal);
			return new TwoGrabPlaneTransformer.TwoGrabPlaneState
			{
				Center = new Pose(position, rotation),
				PlanarDistance = forward.magnitude
			};
		}

		public void InjectOptionalPlaneTransform(Transform planeTransform)
		{
			this._planeTransform = planeTransform;
		}

		public void InjectOptionalConstraints(TwoGrabPlaneTransformer.TwoGrabPlaneConstraints constraints)
		{
			this._constraints = constraints;
		}

		[SerializeField]
		[Optional]
		private Transform _planeTransform;

		[SerializeField]
		[Optional]
		private Vector3 _localPlaneNormal = new Vector3(0f, 1f, 0f);

		[SerializeField]
		private TwoGrabPlaneTransformer.TwoGrabPlaneConstraints _constraints;

		private IGrabbable _grabbable;

		private Pose _localToTarget;

		private float _localMagnitudeToTarget;

		[Serializable]
		public class TwoGrabPlaneConstraints
		{
			public FloatConstraint MaxScale;

			public FloatConstraint MinScale;

			public FloatConstraint MaxY;

			public FloatConstraint MinY;
		}

		public struct TwoGrabPlaneState
		{
			public Pose Center;

			public float PlanarDistance;
		}
	}
}
