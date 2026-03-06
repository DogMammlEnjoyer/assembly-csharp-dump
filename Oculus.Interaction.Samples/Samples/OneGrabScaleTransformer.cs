using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class OneGrabScaleTransformer : MonoBehaviour, ITransformer
	{
		public OneGrabScaleTransformer.OneGrabScaleConstraints Constraints
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

		public void BeginTransform()
		{
			Pose pose = this._grabbable.GrabPoints[0];
			Transform transform = this._grabbable.Transform;
			this._initialLocalPosition = transform.InverseTransformPointUnscaled(pose.position);
			this._initialLocalScale = transform.localScale;
		}

		public void UpdateTransform()
		{
			Pose pose = this._grabbable.GrabPoints[0];
			Transform transform = this._grabbable.Transform;
			Vector3 vector = transform.InverseTransformPointUnscaled(pose.position);
			float num = this._initialLocalScale.x * vector.x / this._initialLocalPosition.x;
			float num2 = this._initialLocalScale.y * vector.y / this._initialLocalPosition.y;
			float num3 = this._initialLocalScale.z * vector.z / this._initialLocalPosition.z;
			if (this._constraints.MinX.Constrain)
			{
				num = Mathf.Max(this._constraints.MinX.Value, num);
			}
			if (this._constraints.MinY.Constrain)
			{
				num2 = Mathf.Max(this._constraints.MinY.Value, num2);
			}
			if (this._constraints.MinZ.Constrain)
			{
				num3 = Mathf.Max(this._constraints.MinZ.Value, num3);
			}
			if (this._constraints.MaxX.Constrain)
			{
				num = Mathf.Min(this._constraints.MaxX.Value, num);
			}
			if (this._constraints.MaxY.Constrain)
			{
				num2 = Mathf.Min(this._constraints.MaxY.Value, num2);
			}
			if (this._constraints.MaxZ.Constrain)
			{
				num3 = Mathf.Min(this._constraints.MaxZ.Value, num3);
			}
			if (this._constraints.IgnoreFixedAxes)
			{
				if (this._constraints.MinX.Constrain && this._constraints.MaxX.Constrain && this._constraints.MinX.Value == this._constraints.MaxX.Value)
				{
					num = transform.localScale.x;
				}
				if (this._constraints.MinY.Constrain && this._constraints.MaxY.Constrain && this._constraints.MinY.Value == this._constraints.MaxY.Value)
				{
					num2 = transform.localScale.y;
				}
				if (this._constraints.MinZ.Constrain && this._constraints.MaxZ.Constrain && this._constraints.MinZ.Value == this._constraints.MaxZ.Value)
				{
					num3 = transform.localScale.z;
				}
			}
			if (this._constraints.ConstrainXYAspectRatio)
			{
				if (num / num2 < this._initialLocalScale.x / this._initialLocalScale.y)
				{
					num2 = num * this._initialLocalScale.y / this._initialLocalScale.x;
				}
				else
				{
					num = num2 * this._initialLocalScale.x / this._initialLocalScale.y;
				}
			}
			transform.localScale = new Vector3(num, num2, num3);
		}

		public void EndTransform()
		{
		}

		[SerializeField]
		[Tooltip("Constraints for allowable values on different axes")]
		private OneGrabScaleTransformer.OneGrabScaleConstraints _constraints = new OneGrabScaleTransformer.OneGrabScaleConstraints
		{
			IgnoreFixedAxes = false,
			ConstrainXYAspectRatio = false,
			MinX = new FloatConstraint(),
			MaxX = new FloatConstraint(),
			MinY = new FloatConstraint(),
			MaxY = new FloatConstraint(),
			MinZ = new FloatConstraint(),
			MaxZ = new FloatConstraint()
		};

		private Vector3 _initialLocalScale;

		private Vector3 _initialLocalPosition;

		private IGrabbable _grabbable;

		[Serializable]
		public class OneGrabScaleConstraints
		{
			public bool IgnoreFixedAxes;

			public bool ConstrainXYAspectRatio;

			public FloatConstraint MinX;

			public FloatConstraint MaxX;

			public FloatConstraint MinY;

			public FloatConstraint MaxY;

			public FloatConstraint MinZ;

			public FloatConstraint MaxZ;
		}
	}
}
