using System;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct MultiParentConstraintJob : IWeightedAnimationJob, IAnimationJob
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
			AffineTransform affineTransform = new AffineTransform(Vector3.zero, QuaternionExt.zero);
			for (int i = 0; i < this.sourceTransforms.Length; i++)
			{
				ReadOnlyTransformHandle value = this.sourceTransforms[i];
				float num5 = this.weightBuffer[i] * num3;
				if (num5 >= 1E-05f)
				{
					Vector3 t;
					Quaternion r;
					value.GetGlobalTR(stream, out t, out r);
					AffineTransform affineTransform2 = new AffineTransform(t, r);
					affineTransform2 *= this.sourceOffsets[i];
					affineTransform.translation += affineTransform2.translation * num5;
					affineTransform.rotation = QuaternionExt.Add(affineTransform.rotation, QuaternionExt.Scale(affineTransform2.rotation, num5));
					this.sourceTransforms[i] = value;
					num4 += num5;
				}
			}
			Vector3 vector;
			Quaternion quaternion;
			this.driven.GetGlobalTR(stream, out vector, out quaternion);
			AffineTransform affineTransform3 = new AffineTransform(vector, quaternion);
			affineTransform.rotation = QuaternionExt.NormalizeSafe(affineTransform.rotation);
			if (num4 < 1f)
			{
				affineTransform.translation += vector * (1f - num4);
				affineTransform.rotation = Quaternion.Lerp(quaternion, affineTransform.rotation, num4);
			}
			AffineTransform identity = AffineTransform.identity;
			if (this.drivenParent.IsValid(stream))
			{
				Vector3 t2;
				Quaternion r2;
				this.drivenParent.GetGlobalTR(stream, out t2, out r2);
				identity = new AffineTransform(t2, r2);
				affineTransform = identity.InverseMul(affineTransform);
				affineTransform3 = identity.InverseMul(affineTransform3);
			}
			if (Vector3.Dot(this.positionAxesMask, this.positionAxesMask) < 3f)
			{
				affineTransform.translation = AnimationRuntimeUtils.Lerp(affineTransform3.translation, affineTransform.translation, this.positionAxesMask);
			}
			if (Vector3.Dot(this.rotationAxesMask, this.rotationAxesMask) < 3f)
			{
				affineTransform.rotation = Quaternion.Euler(AnimationRuntimeUtils.Lerp(affineTransform3.rotation.eulerAngles, affineTransform.rotation.eulerAngles, this.rotationAxesMask));
			}
			affineTransform = identity * affineTransform;
			this.driven.SetGlobalTR(stream, Vector3.Lerp(vector, affineTransform.translation, num), Quaternion.Lerp(quaternion, affineTransform.rotation, num), false);
		}

		private const float k_Epsilon = 1E-05f;

		public ReadWriteTransformHandle driven;

		public ReadOnlyTransformHandle drivenParent;

		public NativeArray<ReadOnlyTransformHandle> sourceTransforms;

		public NativeArray<PropertyStreamHandle> sourceWeights;

		public NativeArray<AffineTransform> sourceOffsets;

		public NativeArray<float> weightBuffer;

		public Vector3 positionAxesMask;

		public Vector3 rotationAxesMask;
	}
}
