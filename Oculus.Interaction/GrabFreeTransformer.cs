using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class GrabFreeTransformer : MonoBehaviour, ITransformer
	{
		public void Initialize(IGrabbable grabbable)
		{
			this._grabbable = grabbable;
			this._relativePositionConstraints = TransformerUtils.GenerateParentConstraints(this._positionConstraints, this._grabbable.Transform.localPosition);
			this._relativeScaleConstraints = TransformerUtils.GenerateParentConstraints(this._scaleConstraints, this._grabbable.Transform.localScale);
		}

		public void BeginTransform()
		{
			int count = this._grabbable.GrabPoints.Count;
			this._deltas = ArrayPool<GrabFreeTransformer.GrabPointDelta>.Shared.Rent(count);
			GrabFreeTransformer.InitializeDeltas(count, this._grabbable.GrabPoints, ref this._deltas);
			Vector3 centroid = GrabFreeTransformer.GetCentroid(this._grabbable.GrabPoints);
			Transform transform = this._grabbable.Transform;
			this._grabDeltaInLocalSpace = new Pose(transform.InverseTransformVector(centroid - transform.position), transform.rotation);
			this._lastRotation = Quaternion.identity;
			this._lastScale = transform.localScale;
		}

		public void UpdateTransform()
		{
			int count = this._grabbable.GrabPoints.Count;
			Transform transform = this._grabbable.Transform;
			Vector3 a = GrabFreeTransformer.UpdateTransformerPointData(this._grabbable.GrabPoints, ref this._deltas);
			this._lastScale = ((count <= 1) ? transform.localScale : (GrabFreeTransformer.UpdateScale(count, this._deltas) * this._lastScale));
			transform.localScale = TransformerUtils.GetConstrainedTransformScale(this._lastScale, this._relativeScaleConstraints);
			if (this._resetScaleResponsivenessOnConstraintOvershoot)
			{
				this._lastScale = transform.localScale;
			}
			this._lastRotation = GrabFreeTransformer.UpdateRotation(count, this._deltas) * this._lastRotation;
			Quaternion unconstrainedRotation = this._lastRotation * this._grabDeltaInLocalSpace.rotation;
			transform.rotation = TransformerUtils.GetConstrainedTransformRotation(unconstrainedRotation, this._rotationConstraints, transform.parent);
			Vector3 unconstrainedPosition = a - transform.TransformVector(this._grabDeltaInLocalSpace.position);
			transform.position = TransformerUtils.GetConstrainedTransformPosition(unconstrainedPosition, this._relativePositionConstraints, transform.parent);
		}

		public void EndTransform()
		{
			ArrayPool<GrabFreeTransformer.GrabPointDelta>.Shared.Return(this._deltas, false);
			this._deltas = null;
		}

		internal static void InitializeDeltas(int count, List<Pose> poses, ref GrabFreeTransformer.GrabPointDelta[] deltas)
		{
			Vector3 centroid = GrabFreeTransformer.GetCentroid(poses);
			for (int i = 0; i < count; i++)
			{
				Vector3 centroidOffset = GrabFreeTransformer.GetCentroidOffset(poses[i], centroid);
				deltas[i] = new GrabFreeTransformer.GrabPointDelta(centroidOffset, poses[i].rotation);
			}
		}

		internal static Vector3 UpdateTransformerPointData(List<Pose> poses, ref GrabFreeTransformer.GrabPointDelta[] deltas)
		{
			Vector3 centroid = GrabFreeTransformer.GetCentroid(poses);
			for (int i = 0; i < poses.Count; i++)
			{
				Vector3 centroidOffset = GrabFreeTransformer.GetCentroidOffset(poses[i], centroid);
				deltas[i].UpdateData(centroidOffset, poses[i].rotation);
			}
			return centroid;
		}

		internal static Vector3 GetCentroid(List<Pose> poses)
		{
			int count = poses.Count;
			Vector3 a = Vector3.zero;
			for (int i = 0; i < count; i++)
			{
				Pose pose = poses[i];
				a += pose.position;
			}
			return a / (float)count;
		}

		internal static Vector3 GetCentroidOffset(Pose pose, Vector3 centre)
		{
			return centre - pose.position;
		}

		internal static Quaternion UpdateRotation(int count, GrabFreeTransformer.GrabPointDelta[] deltas)
		{
			Quaternion quaternion = Quaternion.identity;
			float t = 1f / (float)count;
			for (int i = 0; i < count; i++)
			{
				GrabFreeTransformer.GrabPointDelta grabPointDelta = deltas[i];
				Quaternion b = grabPointDelta.Rotation * Quaternion.Inverse(grabPointDelta.PrevRotation);
				if (grabPointDelta.IsValidAxis())
				{
					Vector3 normalized = grabPointDelta.CentroidOffset.normalized;
					Quaternion b2 = Quaternion.FromToRotation(grabPointDelta.PrevCentroidOffset.normalized, normalized);
					quaternion = Quaternion.Slerp(Quaternion.identity, b2, t) * quaternion;
					float num;
					Vector3 lhs;
					b.ToAngleAxis(out num, out lhs);
					float num2 = Vector3.Dot(lhs, normalized);
					b = Quaternion.AngleAxis(num * num2, normalized);
				}
				quaternion = Quaternion.Slerp(Quaternion.identity, b, t) * quaternion;
			}
			return quaternion;
		}

		internal static float UpdateScale(int count, GrabFreeTransformer.GrabPointDelta[] deltas)
		{
			float num = 0f;
			for (int i = 0; i < count; i++)
			{
				GrabFreeTransformer.GrabPointDelta grabPointDelta = deltas[i];
				if (grabPointDelta.IsValidAxis())
				{
					float num2 = Mathf.Sqrt(grabPointDelta.CentroidOffset.sqrMagnitude / grabPointDelta.PrevCentroidOffset.sqrMagnitude);
					num += num2 / (float)count;
				}
				else
				{
					num += 1f / (float)count;
				}
			}
			return num;
		}

		public void InjectOptionalPositionConstraints(TransformerUtils.PositionConstraints constraints)
		{
			this._positionConstraints = constraints;
		}

		public void InjectOptionalRotationConstraints(TransformerUtils.RotationConstraints constraints)
		{
			this._rotationConstraints = constraints;
		}

		public void InjectOptionalScaleConstraints(TransformerUtils.ScaleConstraints constraints)
		{
			this._scaleConstraints = constraints;
		}

		[SerializeField]
		[Tooltip("Constrains the position of the object along different axes. Units are meters.")]
		private TransformerUtils.PositionConstraints _positionConstraints = new TransformerUtils.PositionConstraints
		{
			XAxis = default(TransformerUtils.ConstrainedAxis),
			YAxis = default(TransformerUtils.ConstrainedAxis),
			ZAxis = default(TransformerUtils.ConstrainedAxis)
		};

		[SerializeField]
		[Tooltip("Constrains the rotation of the object along different axes. Units are degrees.")]
		private TransformerUtils.RotationConstraints _rotationConstraints = new TransformerUtils.RotationConstraints
		{
			XAxis = default(TransformerUtils.ConstrainedAxis),
			YAxis = default(TransformerUtils.ConstrainedAxis),
			ZAxis = default(TransformerUtils.ConstrainedAxis)
		};

		[SerializeField]
		[Tooltip("Constrains the local scale of the object along different axes. Expressed as a scale factor.")]
		private TransformerUtils.ScaleConstraints _scaleConstraints = new TransformerUtils.ScaleConstraints
		{
			ConstraintsAreRelative = true,
			XAxis = new TransformerUtils.ConstrainedAxis
			{
				ConstrainAxis = true,
				AxisRange = new TransformerUtils.FloatRange
				{
					Min = 1f,
					Max = 1f
				}
			},
			YAxis = new TransformerUtils.ConstrainedAxis
			{
				ConstrainAxis = true,
				AxisRange = new TransformerUtils.FloatRange
				{
					Min = 1f,
					Max = 1f
				}
			},
			ZAxis = new TransformerUtils.ConstrainedAxis
			{
				ConstrainAxis = true,
				AxisRange = new TransformerUtils.FloatRange
				{
					Min = 1f,
					Max = 1f
				}
			}
		};

		[SerializeField]
		[Tooltip("If enabled, breaks the \"grab point\" when scale is constrained so that reversing the scale motion immediately scales rather than waiting for the grabs to \"catch up\" to the original grab point.")]
		private bool _resetScaleResponsivenessOnConstraintOvershoot;

		private IGrabbable _grabbable;

		private Pose _grabDeltaInLocalSpace;

		private TransformerUtils.PositionConstraints _relativePositionConstraints;

		private TransformerUtils.ScaleConstraints _relativeScaleConstraints;

		private Quaternion _lastRotation = Quaternion.identity;

		private Vector3 _lastScale = Vector3.one;

		private GrabFreeTransformer.GrabPointDelta[] _deltas;

		internal struct GrabPointDelta
		{
			public Vector3 PrevCentroidOffset { readonly get; private set; }

			public Vector3 CentroidOffset { readonly get; private set; }

			public Quaternion PrevRotation { readonly get; private set; }

			public Quaternion Rotation { readonly get; private set; }

			public GrabPointDelta(Vector3 centroidOffset, Quaternion rotation)
			{
				this.CentroidOffset = centroidOffset;
				this.PrevCentroidOffset = centroidOffset;
				this.Rotation = rotation;
				this.PrevRotation = rotation;
			}

			public void UpdateData(Vector3 centroidOffset, Quaternion rotation)
			{
				this.PrevCentroidOffset = this.CentroidOffset;
				this.CentroidOffset = centroidOffset;
				this.PrevRotation = this.Rotation;
				if (Quaternion.Dot(rotation, this.Rotation) < 0f)
				{
					rotation.x = -rotation.x;
					rotation.y = -rotation.y;
					rotation.z = -rotation.z;
					rotation.w = -rotation.w;
				}
				this.Rotation = rotation;
			}

			public bool IsValidAxis()
			{
				return this.CentroidOffset.sqrMagnitude > 1E-06f;
			}

			private const float _epsilon = 1E-06f;
		}
	}
}
