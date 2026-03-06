using System;
using UnityEngine;

namespace Oculus.Interaction
{
	[Obsolete("Use GrabFreeTransformer instead")]
	public class OneGrabFreeTransformer : MonoBehaviour, ITransformer
	{
		public void Initialize(IGrabbable grabbable)
		{
			this._grabbable = grabbable;
			Vector3 localPosition = this._grabbable.Transform.localPosition;
			this._parentConstraints = TransformerUtils.GenerateParentConstraints(this._positionConstraints, localPosition);
		}

		public void BeginTransform()
		{
			Pose worldPose = this._grabbable.GrabPoints[0];
			Transform transform = this._grabbable.Transform;
			this._localToTarget = TransformerUtils.WorldToLocalPose(worldPose, transform.worldToLocalMatrix);
		}

		public void UpdateTransform()
		{
			Transform transform = this._grabbable.Transform;
			Pose world = this._grabbable.GrabPoints[0];
			Pose pose = TransformerUtils.AlignLocalToWorldPose(transform.localToWorldMatrix, this._localToTarget, world);
			transform.rotation = TransformerUtils.GetConstrainedTransformRotation(pose.rotation, this._rotationConstraints, null);
			transform.position = TransformerUtils.GetConstrainedTransformPosition(pose.position, this._parentConstraints, transform.parent);
		}

		public void EndTransform()
		{
		}

		[SerializeField]
		private TransformerUtils.PositionConstraints _positionConstraints = new TransformerUtils.PositionConstraints
		{
			XAxis = default(TransformerUtils.ConstrainedAxis),
			YAxis = default(TransformerUtils.ConstrainedAxis),
			ZAxis = default(TransformerUtils.ConstrainedAxis)
		};

		[SerializeField]
		private TransformerUtils.RotationConstraints _rotationConstraints = new TransformerUtils.RotationConstraints
		{
			XAxis = default(TransformerUtils.ConstrainedAxis),
			YAxis = default(TransformerUtils.ConstrainedAxis),
			ZAxis = default(TransformerUtils.ConstrainedAxis)
		};

		private IGrabbable _grabbable;

		private Pose _grabDeltaInLocalSpace;

		private TransformerUtils.PositionConstraints _parentConstraints;

		private Pose _localToTarget;
	}
}
