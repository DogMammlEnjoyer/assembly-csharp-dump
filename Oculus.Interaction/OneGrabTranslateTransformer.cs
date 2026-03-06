using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class OneGrabTranslateTransformer : MonoBehaviour, ITransformer
	{
		public OneGrabTranslateTransformer.OneGrabTranslateConstraints Constraints
		{
			get
			{
				return this._constraints;
			}
			set
			{
				this._constraints = value;
				this.GenerateParentConstraints();
			}
		}

		public void Initialize(IGrabbable grabbable)
		{
			this._grabbable = grabbable;
			this._initialPosition = this._grabbable.Transform.localPosition;
			this.GenerateParentConstraints();
		}

		private void GenerateParentConstraints()
		{
			if (!this._constraints.ConstraintsAreRelative)
			{
				this._parentConstraints = this._constraints;
				return;
			}
			this._parentConstraints = new OneGrabTranslateTransformer.OneGrabTranslateConstraints();
			this._parentConstraints.MinX = new FloatConstraint();
			this._parentConstraints.MinY = new FloatConstraint();
			this._parentConstraints.MinZ = new FloatConstraint();
			this._parentConstraints.MaxX = new FloatConstraint();
			this._parentConstraints.MaxY = new FloatConstraint();
			this._parentConstraints.MaxZ = new FloatConstraint();
			if (this._constraints.MinX.Constrain)
			{
				this._parentConstraints.MinX.Constrain = true;
				this._parentConstraints.MinX.Value = this._constraints.MinX.Value + this._initialPosition.x;
			}
			if (this._constraints.MaxX.Constrain)
			{
				this._parentConstraints.MaxX.Constrain = true;
				this._parentConstraints.MaxX.Value = this._constraints.MaxX.Value + this._initialPosition.x;
			}
			if (this._constraints.MinY.Constrain)
			{
				this._parentConstraints.MinY.Constrain = true;
				this._parentConstraints.MinY.Value = this._constraints.MinY.Value + this._initialPosition.y;
			}
			if (this._constraints.MaxY.Constrain)
			{
				this._parentConstraints.MaxY.Constrain = true;
				this._parentConstraints.MaxY.Value = this._constraints.MaxY.Value + this._initialPosition.y;
			}
			if (this._constraints.MinZ.Constrain)
			{
				this._parentConstraints.MinZ.Constrain = true;
				this._parentConstraints.MinZ.Value = this._constraints.MinZ.Value + this._initialPosition.z;
			}
			if (this._constraints.MaxZ.Constrain)
			{
				this._parentConstraints.MaxZ.Constrain = true;
				this._parentConstraints.MaxZ.Value = this._constraints.MaxZ.Value + this._initialPosition.z;
			}
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
			Pose pose = this._grabbable.GrabPoints[0];
			Quaternion rotation = transform.rotation * this._localToTarget.rotation;
			Pose world = new Pose(pose.position, rotation);
			Pose pose2 = TransformerUtils.AlignLocalToWorldPose(transform.localToWorldMatrix, this._localToTarget, world);
			transform.position = pose2.position;
			transform.rotation = pose2.rotation;
			this.ConstrainTransform();
		}

		private void ConstrainTransform()
		{
			Transform transform = this._grabbable.Transform;
			Vector3 localPosition = transform.localPosition;
			if (this._parentConstraints.MinX.Constrain)
			{
				localPosition.x = Mathf.Max(localPosition.x, this._parentConstraints.MinX.Value);
			}
			if (this._parentConstraints.MaxX.Constrain)
			{
				localPosition.x = Mathf.Min(localPosition.x, this._parentConstraints.MaxX.Value);
			}
			if (this._parentConstraints.MinY.Constrain)
			{
				localPosition.y = Mathf.Max(localPosition.y, this._parentConstraints.MinY.Value);
			}
			if (this._parentConstraints.MaxY.Constrain)
			{
				localPosition.y = Mathf.Min(localPosition.y, this._parentConstraints.MaxY.Value);
			}
			if (this._parentConstraints.MinZ.Constrain)
			{
				localPosition.z = Mathf.Max(localPosition.z, this._parentConstraints.MinZ.Value);
			}
			if (this._parentConstraints.MaxZ.Constrain)
			{
				localPosition.z = Mathf.Min(localPosition.z, this._parentConstraints.MaxZ.Value);
			}
			transform.localPosition = localPosition;
		}

		public void EndTransform()
		{
		}

		public void InjectOptionalConstraints(OneGrabTranslateTransformer.OneGrabTranslateConstraints constraints)
		{
			this._constraints = constraints;
		}

		[SerializeField]
		private OneGrabTranslateTransformer.OneGrabTranslateConstraints _constraints = new OneGrabTranslateTransformer.OneGrabTranslateConstraints
		{
			MinX = new FloatConstraint(),
			MaxX = new FloatConstraint(),
			MinY = new FloatConstraint(),
			MaxY = new FloatConstraint(),
			MinZ = new FloatConstraint(),
			MaxZ = new FloatConstraint()
		};

		private OneGrabTranslateTransformer.OneGrabTranslateConstraints _parentConstraints;

		private Vector3 _initialPosition;

		private IGrabbable _grabbable;

		private Pose _localToTarget;

		[Serializable]
		public class OneGrabTranslateConstraints
		{
			public bool ConstraintsAreRelative;

			public FloatConstraint MinX;

			public FloatConstraint MaxX;

			public FloatConstraint MinY;

			public FloatConstraint MaxY;

			public FloatConstraint MinZ;

			public FloatConstraint MaxZ;
		}
	}
}
