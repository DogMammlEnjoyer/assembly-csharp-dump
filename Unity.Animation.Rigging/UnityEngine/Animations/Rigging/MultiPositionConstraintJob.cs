using System;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct MultiPositionConstraintJob : IWeightedAnimationJob, IAnimationJob
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
			Vector3 position = this.driven.GetPosition(stream);
			Vector3 vector = position;
			Vector3 vector2 = position;
			for (int i = 0; i < this.sourceTransforms.Length; i++)
			{
				float num4 = this.weightBuffer[i] * num3;
				if (num4 >= 1E-05f)
				{
					ReadOnlyTransformHandle value = this.sourceTransforms[i];
					vector2 += (value.GetPosition(stream) + this.sourceOffsets[i] - position) * num4;
					this.sourceTransforms[i] = value;
				}
			}
			AffineTransform identity = AffineTransform.identity;
			if (this.drivenParent.IsValid(stream))
			{
				Vector3 t;
				Quaternion r;
				this.drivenParent.GetGlobalTR(stream, out t, out r);
				identity = new AffineTransform(t, r);
				vector2 = identity.InverseTransform(vector2);
				vector = identity.InverseTransform(vector);
			}
			if (Vector3.Dot(this.axesMask, this.axesMask) < 3f)
			{
				vector2 = AnimationRuntimeUtils.Lerp(vector, vector2, this.axesMask);
			}
			vector2 = identity * (vector2 + this.drivenOffset.Get(stream));
			this.driven.SetPosition(stream, Vector3.Lerp(position, vector2, num));
		}

		private const float k_Epsilon = 1E-05f;

		public ReadWriteTransformHandle driven;

		public ReadOnlyTransformHandle drivenParent;

		public Vector3Property drivenOffset;

		public NativeArray<ReadOnlyTransformHandle> sourceTransforms;

		public NativeArray<PropertyStreamHandle> sourceWeights;

		public NativeArray<Vector3> sourceOffsets;

		public NativeArray<float> weightBuffer;

		public Vector3 axesMask;
	}
}
