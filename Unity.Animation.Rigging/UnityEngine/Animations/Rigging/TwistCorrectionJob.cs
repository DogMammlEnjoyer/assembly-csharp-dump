using System;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct TwistCorrectionJob : IWeightedAnimationJob, IAnimationJob
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
				for (int i = 0; i < this.twistTransforms.Length; i++)
				{
					AnimationRuntimeUtils.PassThrough(stream, this.twistTransforms[i]);
				}
				return;
			}
			if (this.twistTransforms.Length == 0)
			{
				return;
			}
			AnimationStreamHandleUtility.ReadFloats(stream, this.twistWeights, this.weightBuffer);
			Quaternion quaternion = TwistCorrectionJob.TwistRotation(this.axisMask, this.sourceInverseBindRotation * this.source.GetLocalRotation(stream));
			Quaternion quaternion2 = Quaternion.Inverse(quaternion);
			for (int j = 0; j < this.twistTransforms.Length; j++)
			{
				ReadWriteTransformHandle value = this.twistTransforms[j];
				float f = Mathf.Clamp(this.weightBuffer[j], -1f, 1f);
				Quaternion b = Quaternion.Lerp(Quaternion.identity, (Mathf.Sign(f) < 0f) ? quaternion2 : quaternion, Mathf.Abs(f));
				value.SetLocalRotation(stream, Quaternion.Lerp(this.twistBindRotations[j], b, num));
				this.twistTransforms[j] = value;
			}
		}

		private static Quaternion TwistRotation(Vector3 axis, Quaternion rot)
		{
			return new Quaternion(axis.x * rot.x, axis.y * rot.y, axis.z * rot.z, rot.w);
		}

		public ReadOnlyTransformHandle source;

		public Quaternion sourceInverseBindRotation;

		public Vector3 axisMask;

		public NativeArray<ReadWriteTransformHandle> twistTransforms;

		public NativeArray<PropertyStreamHandle> twistWeights;

		public NativeArray<Quaternion> twistBindRotations;

		public NativeArray<float> weightBuffer;
	}
}
