using System;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct ChainIKConstraintJob : IWeightedAnimationJob, IAnimationJob
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
				for (int i = 0; i < this.chain.Length; i++)
				{
					ReadWriteTransformHandle value = this.chain[i];
					this.linkPositions[i] = value.GetPosition(stream);
					this.chain[i] = value;
				}
				int num2 = this.chain.Length - 1;
				if (AnimationRuntimeUtils.SolveFABRIK(ref this.linkPositions, ref this.linkLengths, this.target.GetPosition(stream) + this.targetOffset.translation, this.cache.GetRaw(this.toleranceIdx, 0), this.maxReach, (int)this.cache.GetRaw(this.maxIterationsIdx, 0)))
				{
					float t = this.chainRotationWeight.Get(stream) * num;
					for (int j = 0; j < num2; j++)
					{
						Vector3 from = this.chain[j + 1].GetPosition(stream) - this.chain[j].GetPosition(stream);
						Vector3 to = this.linkPositions[j + 1] - this.linkPositions[j];
						Quaternion rotation = this.chain[j].GetRotation(stream);
						this.chain[j].SetRotation(stream, Quaternion.Lerp(rotation, QuaternionExt.FromToRotation(from, to) * rotation, t));
					}
				}
				this.chain[num2].SetRotation(stream, Quaternion.Lerp(this.chain[num2].GetRotation(stream), this.target.GetRotation(stream) * this.targetOffset.rotation, this.tipRotationWeight.Get(stream) * num));
				return;
			}
			for (int k = 0; k < this.chain.Length; k++)
			{
				AnimationRuntimeUtils.PassThrough(stream, this.chain[k]);
			}
		}

		public NativeArray<ReadWriteTransformHandle> chain;

		public ReadOnlyTransformHandle target;

		public AffineTransform targetOffset;

		public NativeArray<float> linkLengths;

		public NativeArray<Vector3> linkPositions;

		public FloatProperty chainRotationWeight;

		public FloatProperty tipRotationWeight;

		public CacheIndex toleranceIdx;

		public CacheIndex maxIterationsIdx;

		public AnimationJobCache cache;

		public float maxReach;
	}
}
