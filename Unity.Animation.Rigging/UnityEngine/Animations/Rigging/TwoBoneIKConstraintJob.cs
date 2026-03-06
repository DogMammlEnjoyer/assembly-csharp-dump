using System;
using Unity.Burst;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct TwoBoneIKConstraintJob : IWeightedAnimationJob, IAnimationJob
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
				AnimationRuntimeUtils.SolveTwoBoneIK(stream, this.root, this.mid, this.tip, this.target, this.hint, this.targetPositionWeight.Get(stream) * num, this.targetRotationWeight.Get(stream) * num, this.hintWeight.Get(stream) * num, this.targetOffset);
				return;
			}
			AnimationRuntimeUtils.PassThrough(stream, this.root);
			AnimationRuntimeUtils.PassThrough(stream, this.mid);
			AnimationRuntimeUtils.PassThrough(stream, this.tip);
		}

		public ReadWriteTransformHandle root;

		public ReadWriteTransformHandle mid;

		public ReadWriteTransformHandle tip;

		public ReadOnlyTransformHandle hint;

		public ReadOnlyTransformHandle target;

		public AffineTransform targetOffset;

		public FloatProperty targetPositionWeight;

		public FloatProperty targetRotationWeight;

		public FloatProperty hintWeight;
	}
}
