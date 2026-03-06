using System;
using Unity.Burst;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct BlendConstraintJob : IWeightedAnimationJob, IAnimationJob
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
			if (this.blendPosition.Get(stream))
			{
				Vector3 b = Vector3.Lerp(this.sourceA.GetPosition(stream) + this.sourceAOffset.translation, this.sourceB.GetPosition(stream) + this.sourceBOffset.translation, this.positionWeight.Get(stream));
				this.driven.SetPosition(stream, Vector3.Lerp(this.driven.GetPosition(stream), b, num));
			}
			else
			{
				this.driven.SetLocalPosition(stream, this.driven.GetLocalPosition(stream));
			}
			if (this.blendRotation.Get(stream))
			{
				Quaternion b2 = Quaternion.Lerp(this.sourceA.GetRotation(stream) * this.sourceAOffset.rotation, this.sourceB.GetRotation(stream) * this.sourceBOffset.rotation, this.rotationWeight.Get(stream));
				this.driven.SetRotation(stream, Quaternion.Lerp(this.driven.GetRotation(stream), b2, num));
				return;
			}
			this.driven.SetLocalRotation(stream, this.driven.GetLocalRotation(stream));
		}

		private const int k_BlendTranslationMask = 1;

		private const int k_BlendRotationMask = 2;

		public ReadWriteTransformHandle driven;

		public ReadOnlyTransformHandle sourceA;

		public ReadOnlyTransformHandle sourceB;

		public AffineTransform sourceAOffset;

		public AffineTransform sourceBOffset;

		public BoolProperty blendPosition;

		public BoolProperty blendRotation;

		public FloatProperty positionWeight;

		public FloatProperty rotationWeight;
	}
}
