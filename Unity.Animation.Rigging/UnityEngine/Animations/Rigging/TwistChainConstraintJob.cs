using System;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct TwistChainConstraintJob : IWeightedAnimationJob, IAnimationJob
	{
		public FloatProperty jobWeight { readonly get; set; }

		public void ProcessRootMotion(AnimationStream stream)
		{
		}

		public void ProcessAnimation(AnimationStream stream)
		{
			float num = this.jobWeight.Get(stream);
			if (num > 0f)
			{
				Quaternion rotation = this.rootTarget.GetRotation(stream);
				Quaternion rotation2 = this.tipTarget.GetRotation(stream);
				this.chain[0].SetRotation(stream, Quaternion.Lerp(this.chain[0].GetRotation(stream), rotation, num));
				for (int i = 1; i < this.chain.Length - 1; i++)
				{
					this.chain[i].SetRotation(stream, Quaternion.Lerp(this.chain[i].GetRotation(stream), this.rotations[i] * Quaternion.Lerp(rotation, rotation2, this.weights[i]), num));
				}
				this.chain[this.chain.Length - 1].SetRotation(stream, Quaternion.Lerp(this.chain[this.chain.Length - 1].GetRotation(stream), rotation2, num));
				return;
			}
			for (int j = 0; j < this.chain.Length; j++)
			{
				AnimationRuntimeUtils.PassThrough(stream, this.chain[j]);
			}
		}

		public ReadWriteTransformHandle rootTarget;

		public ReadWriteTransformHandle tipTarget;

		public NativeArray<ReadWriteTransformHandle> chain;

		public NativeArray<float> steps;

		public NativeArray<float> weights;

		public NativeArray<Quaternion> rotations;
	}
}
