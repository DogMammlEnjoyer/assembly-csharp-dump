using System;
using UnityEngine;

namespace Oculus.Interaction
{
	[Obsolete("Use GrabFreeTransformer instead")]
	public class TwoGrabFreeTransformer : MonoBehaviour, ITransformer
	{
		public TwoGrabFreeTransformer.TwoGrabFreeConstraints Constraints
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
			this._baseScale = this._grabbable.Transform.localScale;
		}

		public void BeginTransform()
		{
			Transform transform = this._grabbable.Transform;
			Pose pose = this._grabbable.GrabPoints[0];
			Pose pose2 = this._grabbable.GrabPoints[1];
			this._prevGrabA = pose;
			this._prevGrabB = pose2;
			TwoGrabFreeTransformer.TwoGrabFreeState twoGrabFreeState = TwoGrabFreeTransformer.TwoGrabFreeInit(pose.position, pose2.position);
			this._prevGrabRotation = twoGrabFreeState.Center.rotation;
			this._localToTarget = TransformerUtils.WorldToLocalPose(twoGrabFreeState.Center, transform.worldToLocalMatrix);
			this._localMagnitudeToTarget = TransformerUtils.WorldToLocalMagnitude(twoGrabFreeState.Distance, transform.worldToLocalMatrix);
		}

		public void UpdateTransform()
		{
			Transform transform = this._grabbable.Transform;
			Pose pose = this._grabbable.GrabPoints[0];
			Pose pose2 = this._grabbable.GrabPoints[1];
			TwoGrabFreeTransformer.TwoGrabFreeState twoGrabFreeState = TwoGrabFreeTransformer.TwoGrabFree(this._prevGrabRotation, this._prevGrabA, this._prevGrabB, pose, pose2);
			float num = TransformerUtils.LocalToWorldMagnitude(this._localMagnitudeToTarget, transform.localToWorldMatrix);
			float targetScale = ((num != 0f) ? (twoGrabFreeState.Distance / num) : 1f) * transform.localScale.x;
			float num2 = this.ConstrainScale(targetScale);
			transform.localScale = num2 / transform.localScale.x * transform.localScale;
			Pose pose3 = TransformerUtils.AlignLocalToWorldPose(transform.localToWorldMatrix, this._localToTarget, twoGrabFreeState.Center);
			transform.position = pose3.position;
			transform.rotation = pose3.rotation;
			this._prevGrabRotation = twoGrabFreeState.Center.rotation;
			this._prevGrabA = pose;
			this._prevGrabB = pose2;
		}

		private float ConstrainScale(float targetScale)
		{
			float num = targetScale;
			if (this._constraints.MinScale.Constrain)
			{
				Vector3 vector = this._constraints.MinScale.Value * (this._constraints.ConstraintsAreRelative ? this._baseScale : Vector3.one);
				if (this._constraints.ConstrainXScale)
				{
					num = Mathf.Max(num, vector.x);
				}
				if (this._constraints.ConstrainYScale)
				{
					num = Mathf.Max(num, vector.y);
				}
				if (this._constraints.ConstrainZScale)
				{
					num = Mathf.Max(num, vector.z);
				}
			}
			if (this._constraints.MinScale.Constrain)
			{
				Vector3 vector2 = this._constraints.MaxScale.Value * (this._constraints.ConstraintsAreRelative ? this._baseScale : Vector3.one);
				if (this._constraints.ConstrainXScale)
				{
					num = Mathf.Min(num, vector2.x);
				}
				if (this._constraints.ConstrainYScale)
				{
					num = Mathf.Min(num, vector2.y);
				}
				if (this._constraints.ConstrainZScale)
				{
					num = Mathf.Min(num, vector2.z);
				}
			}
			return num;
		}

		public static TwoGrabFreeTransformer.TwoGrabFreeState TwoGrabFreeInit(Vector3 a, Vector3 b)
		{
			Vector3 position = Vector3.Lerp(a, b, 0.5f);
			Vector3 vector = b - a;
			Vector3 upwards = ((double)Mathf.Abs(Vector3.Dot(vector, Vector3.up)) < 0.999) ? Vector3.up : Vector3.right;
			Quaternion rotation = Quaternion.LookRotation(vector, upwards);
			return new TwoGrabFreeTransformer.TwoGrabFreeState
			{
				Center = new Pose(position, rotation),
				Distance = vector.magnitude
			};
		}

		public static TwoGrabFreeTransformer.TwoGrabFreeState TwoGrabFree(Quaternion initialRotation, Pose prevA, Pose prevB, Pose newA, Pose newB)
		{
			Vector3.Lerp(prevA.position, prevB.position, 0.5f);
			Vector3 position = Vector3.Lerp(newA.position, newB.position, 0.5f);
			Vector3 fromDirection = prevB.position - prevA.position;
			Vector3 vector = newB.position - newA.position;
			Quaternion lhs = Quaternion.FromToRotation(fromDirection, vector);
			Quaternion b = newA.rotation * Quaternion.Inverse(prevA.rotation);
			Quaternion normalized = Quaternion.Slerp(Quaternion.identity, b, 0.5f).normalized;
			Quaternion b2 = newB.rotation * Quaternion.Inverse(prevB.rotation);
			Quaternion normalized2 = Quaternion.Slerp(Quaternion.identity, b2, 0.5f).normalized;
			Vector3 upwards = lhs * normalized * normalized2 * initialRotation * Vector3.up;
			Quaternion normalized3 = Quaternion.LookRotation(vector, upwards).normalized;
			return new TwoGrabFreeTransformer.TwoGrabFreeState
			{
				Center = new Pose(position, normalized3),
				Distance = (newB.position - newA.position).magnitude
			};
		}

		public void MarkAsBaseScale()
		{
			this._baseScale = this._grabbable.Transform.localScale;
		}

		public void EndTransform()
		{
		}

		public void InjectOptionalConstraints(TwoGrabFreeTransformer.TwoGrabFreeConstraints constraints)
		{
			this._constraints = constraints;
		}

		[SerializeField]
		private TwoGrabFreeTransformer.TwoGrabFreeConstraints _constraints;

		private IGrabbable _grabbable;

		private Vector3 _baseScale;

		private Pose _localToTarget;

		private float _localMagnitudeToTarget;

		private Pose _prevGrabA;

		private Pose _prevGrabB;

		private Quaternion _prevGrabRotation;

		[Serializable]
		public class TwoGrabFreeConstraints
		{
			[Tooltip("If true then the constraints are relative to the initial/base scale of the object if false, constraints are absolute with respect to the object's selected axes.")]
			public bool ConstraintsAreRelative;

			public FloatConstraint MinScale;

			public FloatConstraint MaxScale;

			public bool ConstrainXScale = true;

			public bool ConstrainYScale;

			public bool ConstrainZScale;
		}

		public struct TwoGrabFreeState
		{
			public Pose Center;

			public float Distance;
		}
	}
}
