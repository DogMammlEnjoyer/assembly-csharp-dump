using System;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct MultiRotationConstraintJob : IWeightedAnimationJob, IAnimationJob
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
			float num4 = 0f;
			Quaternion quaternion = QuaternionExt.zero;
			for (int i = 0; i < this.sourceTransforms.Length; i++)
			{
				float num5 = this.weightBuffer[i] * num3;
				if (num5 >= 1E-05f)
				{
					ReadOnlyTransformHandle value = this.sourceTransforms[i];
					quaternion = QuaternionExt.Add(quaternion, QuaternionExt.Scale(value.GetRotation(stream) * this.sourceOffsets[i], num5));
					this.sourceTransforms[i] = value;
					num4 += num5;
				}
			}
			quaternion = QuaternionExt.NormalizeSafe(quaternion);
			if (num4 < 1f)
			{
				quaternion = Quaternion.Lerp(this.driven.GetRotation(stream), quaternion, num4);
			}
			if (this.drivenParent.IsValid(stream))
			{
				quaternion = Quaternion.Inverse(this.drivenParent.GetRotation(stream)) * quaternion;
			}
			Quaternion localRotation = this.driven.GetLocalRotation(stream);
			if (Vector3.Dot(this.axesMask, this.axesMask) < 3f)
			{
				quaternion = Quaternion.Euler(AnimationRuntimeUtils.Lerp(localRotation.eulerAngles, quaternion.eulerAngles, this.axesMask));
			}
			Vector3 vector = this.drivenOffset.Get(stream);
			if (Vector3.Dot(vector, vector) > 0f)
			{
				quaternion *= Quaternion.Euler(vector);
			}
			this.driven.SetLocalRotation(stream, Quaternion.Lerp(localRotation, quaternion, num));
		}

		private const float k_Epsilon = 1E-05f;

		public ReadWriteTransformHandle driven;

		public ReadOnlyTransformHandle drivenParent;

		public Vector3Property drivenOffset;

		public NativeArray<ReadOnlyTransformHandle> sourceTransforms;

		public NativeArray<PropertyStreamHandle> sourceWeights;

		public NativeArray<Quaternion> sourceOffsets;

		public NativeArray<float> weightBuffer;

		public Vector3 axesMask;
	}
}
