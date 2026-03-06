using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oculus.Interaction
{
	public class TransformerUtils
	{
		public static TransformerUtils.PositionConstraints GenerateParentConstraints(TransformerUtils.PositionConstraints constraints, Vector3 initialPosition)
		{
			TransformerUtils.PositionConstraints positionConstraints;
			if (!constraints.ConstraintsAreRelative)
			{
				positionConstraints = constraints;
			}
			else
			{
				positionConstraints = new TransformerUtils.PositionConstraints();
				positionConstraints.XAxis = default(TransformerUtils.ConstrainedAxis);
				positionConstraints.YAxis = default(TransformerUtils.ConstrainedAxis);
				positionConstraints.ZAxis = default(TransformerUtils.ConstrainedAxis);
				if (constraints.XAxis.ConstrainAxis)
				{
					positionConstraints.XAxis.ConstrainAxis = true;
					positionConstraints.XAxis.AxisRange.Min = constraints.XAxis.AxisRange.Min + initialPosition.x;
					positionConstraints.XAxis.AxisRange.Max = constraints.XAxis.AxisRange.Max + initialPosition.x;
				}
				if (constraints.YAxis.ConstrainAxis)
				{
					positionConstraints.YAxis.ConstrainAxis = true;
					positionConstraints.YAxis.AxisRange.Min = constraints.YAxis.AxisRange.Min + initialPosition.y;
					positionConstraints.YAxis.AxisRange.Max = constraints.YAxis.AxisRange.Max + initialPosition.y;
				}
				if (constraints.ZAxis.ConstrainAxis)
				{
					positionConstraints.ZAxis.ConstrainAxis = true;
					positionConstraints.ZAxis.AxisRange.Min = constraints.ZAxis.AxisRange.Min + initialPosition.z;
					positionConstraints.ZAxis.AxisRange.Max = constraints.ZAxis.AxisRange.Max + initialPosition.z;
				}
			}
			return positionConstraints;
		}

		public static TransformerUtils.ScaleConstraints GenerateParentConstraints(TransformerUtils.ScaleConstraints constraints, Vector3 initialScale)
		{
			TransformerUtils.ScaleConstraints scaleConstraints;
			if (!constraints.ConstraintsAreRelative)
			{
				scaleConstraints = constraints;
			}
			else
			{
				scaleConstraints = new TransformerUtils.ScaleConstraints();
				scaleConstraints.XAxis = default(TransformerUtils.ConstrainedAxis);
				scaleConstraints.YAxis = default(TransformerUtils.ConstrainedAxis);
				scaleConstraints.ZAxis = default(TransformerUtils.ConstrainedAxis);
				if (constraints.XAxis.ConstrainAxis)
				{
					scaleConstraints.XAxis.ConstrainAxis = true;
					scaleConstraints.XAxis.AxisRange.Min = constraints.XAxis.AxisRange.Min * initialScale.x;
					scaleConstraints.XAxis.AxisRange.Max = constraints.XAxis.AxisRange.Max * initialScale.x;
				}
				if (constraints.YAxis.ConstrainAxis)
				{
					scaleConstraints.YAxis.ConstrainAxis = true;
					scaleConstraints.YAxis.AxisRange.Min = constraints.YAxis.AxisRange.Min * initialScale.y;
					scaleConstraints.YAxis.AxisRange.Max = constraints.YAxis.AxisRange.Max * initialScale.y;
				}
				if (constraints.ZAxis.ConstrainAxis)
				{
					scaleConstraints.ZAxis.ConstrainAxis = true;
					scaleConstraints.ZAxis.AxisRange.Min = constraints.ZAxis.AxisRange.Min * initialScale.z;
					scaleConstraints.ZAxis.AxisRange.Max = constraints.ZAxis.AxisRange.Max * initialScale.z;
				}
			}
			return scaleConstraints;
		}

		public static Vector3 GetConstrainedTransformPosition(Vector3 unconstrainedPosition, TransformerUtils.PositionConstraints positionConstraints, Transform relativeTransform = null)
		{
			Vector3 vector = unconstrainedPosition;
			if (relativeTransform != null)
			{
				vector = relativeTransform.InverseTransformPoint(vector);
			}
			if (positionConstraints.XAxis.ConstrainAxis)
			{
				vector.x = Mathf.Clamp(vector.x, positionConstraints.XAxis.AxisRange.Min, positionConstraints.XAxis.AxisRange.Max);
			}
			if (positionConstraints.YAxis.ConstrainAxis)
			{
				vector.y = Mathf.Clamp(vector.y, positionConstraints.YAxis.AxisRange.Min, positionConstraints.YAxis.AxisRange.Max);
			}
			if (positionConstraints.ZAxis.ConstrainAxis)
			{
				vector.z = Mathf.Clamp(vector.z, positionConstraints.ZAxis.AxisRange.Min, positionConstraints.ZAxis.AxisRange.Max);
			}
			if (relativeTransform != null)
			{
				vector = relativeTransform.TransformPoint(vector);
			}
			return vector;
		}

		public static Quaternion GetConstrainedTransformRotation(Quaternion unconstrainedRotation, TransformerUtils.RotationConstraints rotationConstraints, Transform relativeTransform = null)
		{
			if (relativeTransform != null)
			{
				unconstrainedRotation = Quaternion.Inverse(relativeTransform.rotation) * unconstrainedRotation;
			}
			Vector3 eulerAngles = unconstrainedRotation.eulerAngles;
			float num = eulerAngles.x;
			float num2 = eulerAngles.y;
			float num3 = eulerAngles.z;
			if (rotationConstraints.XAxis.ConstrainAxis)
			{
				num = TransformerUtils.<GetConstrainedTransformRotation>g__ClampAngle|8_0(num, rotationConstraints.XAxis.AxisRange.Min, rotationConstraints.XAxis.AxisRange.Max);
			}
			if (rotationConstraints.YAxis.ConstrainAxis)
			{
				num2 = TransformerUtils.<GetConstrainedTransformRotation>g__ClampAngle|8_0(num2, rotationConstraints.YAxis.AxisRange.Min, rotationConstraints.YAxis.AxisRange.Max);
			}
			if (rotationConstraints.ZAxis.ConstrainAxis)
			{
				num3 = TransformerUtils.<GetConstrainedTransformRotation>g__ClampAngle|8_0(num3, rotationConstraints.ZAxis.AxisRange.Min, rotationConstraints.ZAxis.AxisRange.Max);
			}
			Quaternion rhs = Quaternion.Euler(num, num2, num3);
			if (relativeTransform != null)
			{
				rhs = relativeTransform.rotation * rhs;
			}
			return rhs.normalized;
		}

		public static Vector3 GetConstrainedTransformScale(Vector3 unconstrainedScale, TransformerUtils.ScaleConstraints scaleConstraints)
		{
			Vector3 vector = unconstrainedScale;
			if (scaleConstraints.XAxis.ConstrainAxis)
			{
				vector.x = Mathf.Clamp(vector.x, scaleConstraints.XAxis.AxisRange.Min, scaleConstraints.XAxis.AxisRange.Max);
			}
			if (scaleConstraints.YAxis.ConstrainAxis)
			{
				vector.y = Mathf.Clamp(vector.y, scaleConstraints.YAxis.AxisRange.Min, scaleConstraints.YAxis.AxisRange.Max);
			}
			if (scaleConstraints.ZAxis.ConstrainAxis)
			{
				vector.z = Mathf.Clamp(vector.z, scaleConstraints.ZAxis.AxisRange.Min, scaleConstraints.ZAxis.AxisRange.Max);
			}
			return vector;
		}

		public static Pose WorldToLocalPose(Pose worldPose, Matrix4x4 worldToLocal)
		{
			return new Pose(worldToLocal.MultiplyPoint3x4(worldPose.position), worldToLocal.rotation * worldPose.rotation);
		}

		public static Pose AlignLocalToWorldPose(Matrix4x4 localToWorld, Pose local, Pose world)
		{
			Pose pose = new Pose(localToWorld.MultiplyPoint3x4(local.position), localToWorld.rotation * local.rotation);
			Pose pose2 = default(Pose);
			PoseUtils.Inverse(pose, ref pose2);
			Pose pose3 = new Pose(localToWorld.GetPosition(), localToWorld.rotation);
			Pose pose4 = PoseUtils.Multiply(pose2, pose3);
			return PoseUtils.Multiply(world, pose4);
		}

		public static float WorldToLocalMagnitude(float magnitude, Matrix4x4 worldToLocal)
		{
			return worldToLocal.MultiplyVector(magnitude * Vector3.forward).magnitude;
		}

		public static float LocalToWorldMagnitude(float magnitude, Matrix4x4 localToWorld)
		{
			return localToWorld.MultiplyVector(magnitude * Vector3.forward).magnitude;
		}

		public static Vector3 ConstrainAlongDirection(Vector3 position, Vector3 origin, Vector3 direction, FloatConstraint min, FloatConstraint max)
		{
			if (!min.Constrain && !max.Constrain)
			{
				return position;
			}
			float num = Vector3.Dot(position - origin, direction);
			float num2 = num;
			if (min.Constrain)
			{
				num2 = Mathf.Max(num2, min.Value);
			}
			if (max.Constrain)
			{
				num2 = Mathf.Min(num2, max.Value);
			}
			float d = num2 - num;
			return position + direction * d;
		}

		[CompilerGenerated]
		internal static float <GetConstrainedTransformRotation>g__ClampAngle|8_0(float angle, float min, float max)
		{
			if (min == max)
			{
				return min;
			}
			if (min <= max)
			{
				if (angle >= min && angle <= max)
				{
					return angle;
				}
			}
			else if (angle >= min || angle <= max)
			{
				return angle;
			}
			if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) <= Mathf.Abs(Mathf.DeltaAngle(max, angle)))
			{
				return min;
			}
			return max;
		}

		[Serializable]
		public struct FloatRange
		{
			public float Min;

			public float Max;
		}

		[Serializable]
		public struct ConstrainedAxis
		{
			public static TransformerUtils.ConstrainedAxis Unconstrained
			{
				get
				{
					return new TransformerUtils.ConstrainedAxis
					{
						ConstrainAxis = false,
						AxisRange = new TransformerUtils.FloatRange
						{
							Min = 1f,
							Max = 1f
						}
					};
				}
			}

			public bool ConstrainAxis;

			public TransformerUtils.FloatRange AxisRange;
		}

		[Serializable]
		public class PositionConstraints
		{
			public bool ConstraintsAreRelative;

			public TransformerUtils.ConstrainedAxis XAxis;

			public TransformerUtils.ConstrainedAxis YAxis;

			public TransformerUtils.ConstrainedAxis ZAxis;
		}

		[Serializable]
		public class RotationConstraints
		{
			public TransformerUtils.ConstrainedAxis XAxis;

			public TransformerUtils.ConstrainedAxis YAxis;

			public TransformerUtils.ConstrainedAxis ZAxis;
		}

		[Serializable]
		public class ScaleConstraints
		{
			public bool ConstraintsAreRelative;

			public TransformerUtils.ConstrainedAxis XAxis;

			public TransformerUtils.ConstrainedAxis YAxis;

			public TransformerUtils.ConstrainedAxis ZAxis;
		}
	}
}
