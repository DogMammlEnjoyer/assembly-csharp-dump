using System;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct MultiAimConstraintJob : IWeightedAnimationJob, IAnimationJob
	{
		public FloatProperty jobWeight { readonly get; set; }

		public void ProcessRootMotion(AnimationStream stream)
		{
		}

		public void ProcessAnimation(AnimationStream stream)
		{
			float num = this.jobWeight.Get(stream);
			if (num <= 0f)
			{
				AnimationRuntimeUtils.PassThrough(stream, this.driven);
				return;
			}
			AnimationStreamHandleUtility.ReadFloats(stream, this.sourceWeights, this.weightBuffer);
			float num2 = AnimationRuntimeUtils.Sum(this.weightBuffer);
			if (num2 < 1E-05f)
			{
				AnimationRuntimeUtils.PassThrough(stream, this.driven);
				return;
			}
			float num3 = (num2 > 1f) ? (1f / num2) : 1f;
			Quaternion rotation = Quaternion.Inverse(this.drivenParent.GetRotation(stream));
			Matrix4x4 matrix4x = Matrix4x4.Inverse(this.drivenParent.GetLocalToWorldMatrix(stream));
			float num4 = 0f;
			Quaternion quaternion = QuaternionExt.zero;
			Vector3 position = this.driven.GetPosition(stream);
			Quaternion localRotation = this.driven.GetLocalRotation(stream);
			Vector3 point = this.ComputeWorldUpVector(stream);
			Vector3 vector = AnimationRuntimeUtils.Select(Vector3.zero, this.upAxis, this.axesMask);
			bool flag = Vector3.Dot(this.axesMask, this.axesMask) < 3f;
			bool flag2 = this.worldUpType != MultiAimConstraintJob.WorldUpType.None && Vector3.Dot(vector, vector) > 1E-05f;
			Vector2 vector2 = new Vector2(this.minLimit.Get(stream), this.maxLimit.Get(stream));
			for (int i = 0; i < this.sourceTransforms.Length; i++)
			{
				float num5 = this.weightBuffer[i] * num3;
				if (num5 >= 1E-05f)
				{
					ReadOnlyTransformHandle value = this.sourceTransforms[i];
					Vector3 vector3 = localRotation * this.aimAxis;
					Vector3 vector4 = matrix4x.MultiplyVector(value.GetPosition(stream) - position);
					if (vector4.sqrMagnitude >= 1E-05f)
					{
						Vector3 normalized = Vector3.Cross(vector3, vector4).normalized;
						if (flag)
						{
							normalized = AnimationRuntimeUtils.Select(Vector3.zero, normalized, this.axesMask).normalized;
							if (Vector3.Dot(normalized, normalized) > 1E-05f)
							{
								vector3 = AnimationRuntimeUtils.ProjectOnPlane(vector3, normalized);
								vector4 = AnimationRuntimeUtils.ProjectOnPlane(vector4, normalized);
							}
							else
							{
								vector4 = vector3;
							}
						}
						Quaternion quaternion2 = Quaternion.AngleAxis(Mathf.Clamp(Vector3.Angle(vector3, vector4), vector2.x, vector2.y), normalized);
						if (flag2)
						{
							Vector3 normalized2 = Vector3.Cross(Vector3.Cross(rotation * point, vector4).normalized, vector4).normalized;
							quaternion2 = QuaternionExt.FromToRotation(Vector3.Cross(Vector3.Cross(quaternion2 * localRotation * vector, vector4).normalized, vector4).normalized, normalized2) * quaternion2;
						}
						quaternion = QuaternionExt.Add(quaternion, QuaternionExt.Scale(this.sourceOffsets[i] * quaternion2, num5));
						this.sourceTransforms[i] = value;
						num4 += num5;
					}
				}
			}
			quaternion = QuaternionExt.NormalizeSafe(quaternion);
			if (num4 < 1f)
			{
				quaternion = Quaternion.Lerp(Quaternion.identity, quaternion, num4);
			}
			Quaternion quaternion3 = quaternion * localRotation;
			if (flag)
			{
				quaternion3 = Quaternion.Euler(AnimationRuntimeUtils.Select(localRotation.eulerAngles, quaternion3.eulerAngles, this.axesMask));
			}
			Vector3 vector5 = this.drivenOffset.Get(stream);
			if (Vector3.Dot(vector5, vector5) > 0f)
			{
				quaternion3 *= Quaternion.Euler(vector5);
			}
			this.driven.SetLocalRotation(stream, Quaternion.Lerp(localRotation, quaternion3, num));
		}

		private Vector3 ComputeWorldUpVector(AnimationStream stream)
		{
			Vector3 result = Vector3.up;
			switch (this.worldUpType)
			{
			case MultiAimConstraintJob.WorldUpType.None:
				result = Vector3.zero;
				break;
			case MultiAimConstraintJob.WorldUpType.ObjectUp:
			{
				Vector3 a = Vector3.zero;
				if (this.worldUpObject.IsValid(stream))
				{
					a = this.worldUpObject.GetPosition(stream);
				}
				Vector3 position = this.driven.GetPosition(stream);
				result = (a - position).normalized;
				break;
			}
			case MultiAimConstraintJob.WorldUpType.ObjectRotationUp:
			{
				Quaternion rotation = Quaternion.identity;
				if (this.worldUpObject.IsValid(stream))
				{
					rotation = this.worldUpObject.GetRotation(stream);
				}
				result = rotation * this.worldUpAxis;
				break;
			}
			case MultiAimConstraintJob.WorldUpType.Vector:
				result = this.worldUpAxis;
				break;
			}
			return result;
		}

		private const float k_Epsilon = 1E-05f;

		public ReadWriteTransformHandle driven;

		public ReadOnlyTransformHandle drivenParent;

		public Vector3Property drivenOffset;

		public NativeArray<ReadOnlyTransformHandle> sourceTransforms;

		public NativeArray<PropertyStreamHandle> sourceWeights;

		public NativeArray<Quaternion> sourceOffsets;

		public NativeArray<float> weightBuffer;

		public Vector3 aimAxis;

		public Vector3 upAxis;

		public MultiAimConstraintJob.WorldUpType worldUpType;

		public Vector3 worldUpAxis;

		public ReadOnlyTransformHandle worldUpObject;

		public Vector3 axesMask;

		public FloatProperty minLimit;

		public FloatProperty maxLimit;

		public enum WorldUpType
		{
			None,
			SceneUp,
			ObjectUp,
			ObjectRotationUp,
			Vector
		}
	}
}
