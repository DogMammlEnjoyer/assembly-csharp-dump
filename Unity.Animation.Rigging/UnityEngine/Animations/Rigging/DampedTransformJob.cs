using System;
using Unity.Burst;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct DampedTransformJob : IWeightedAnimationJob, IAnimationJob
	{
		public FloatProperty jobWeight { readonly get; set; }

		public void ProcessRootMotion(AnimationStream stream)
		{
		}

		public void ProcessAnimation(AnimationStream stream)
		{
			float num = this.jobWeight.Get(stream);
			float num2 = Mathf.Abs(stream.deltaTime);
			Vector3 vector;
			Quaternion quaternion;
			this.driven.GetGlobalTR(stream, out vector, out quaternion);
			if (num > 0f && num2 > 0f)
			{
				Vector3 t;
				Quaternion r;
				this.source.GetGlobalTR(stream, out t, out r);
				AffineTransform affineTransform = new AffineTransform(t, r);
				AffineTransform affineTransform2 = affineTransform * this.localBindTx;
				affineTransform2.translation = Vector3.Lerp(vector, affineTransform2.translation, num);
				affineTransform2.rotation = Quaternion.Lerp(quaternion, affineTransform2.rotation, num);
				float d = AnimationRuntimeUtils.Square(1f - this.dampPosition.Get(stream));
				float num3 = AnimationRuntimeUtils.Square(1f - this.dampRotation.Get(stream));
				bool flag = Vector3.Dot(this.aimBindAxis, this.aimBindAxis) > 0f;
				while (num2 > 0f)
				{
					float num4 = 40f * Mathf.Min(0.01667f, num2);
					this.prevDrivenTx.translation = this.prevDrivenTx.translation + (affineTransform2.translation - this.prevDrivenTx.translation) * d * num4;
					this.prevDrivenTx.rotation = this.prevDrivenTx.rotation * Quaternion.Lerp(Quaternion.identity, Quaternion.Inverse(this.prevDrivenTx.rotation) * affineTransform2.rotation, num3 * num4);
					if (flag)
					{
						Vector3 vector2 = this.prevDrivenTx.rotation * this.aimBindAxis;
						Vector3 vector3 = affineTransform.translation - this.prevDrivenTx.translation;
						this.prevDrivenTx.rotation = Quaternion.AngleAxis(Vector3.Angle(vector2, vector3), Vector3.Cross(vector2, vector3).normalized) * this.prevDrivenTx.rotation;
					}
					num2 -= 0.01667f;
				}
				this.driven.SetGlobalTR(stream, this.prevDrivenTx.translation, this.prevDrivenTx.rotation, false);
				return;
			}
			this.prevDrivenTx.Set(vector, quaternion);
			AnimationRuntimeUtils.PassThrough(stream, this.driven);
		}

		private const float k_FixedDt = 0.01667f;

		private const float k_DampFactor = 40f;

		public ReadWriteTransformHandle driven;

		public ReadOnlyTransformHandle source;

		public AffineTransform localBindTx;

		public Vector3 aimBindAxis;

		public AffineTransform prevDrivenTx;

		public FloatProperty dampPosition;

		public FloatProperty dampRotation;
	}
}
