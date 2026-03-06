using System;
using Unity.Burst;
using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
	[BurstCompile]
	public struct MultiReferentialConstraintJob : IWeightedAnimationJob, IAnimationJob
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
				int num2 = this.driver.Get(stream);
				if (num2 != this.m_PrevDriverIdx)
				{
					this.UpdateOffsets(num2);
				}
				Vector3 t;
				Quaternion r;
				this.sources[num2].GetGlobalTR(stream, out t, out r);
				AffineTransform lhs = new AffineTransform(t, r);
				int num3 = 0;
				for (int i = 0; i < this.sources.Length; i++)
				{
					if (i != num2)
					{
						AffineTransform affineTransform = lhs * this.offsetTx[num3];
						ReadWriteTransformHandle value = this.sources[i];
						Vector3 a;
						Quaternion a2;
						value.GetGlobalTR(stream, out a, out a2);
						value.SetGlobalTR(stream, Vector3.Lerp(a, affineTransform.translation, num), Quaternion.Lerp(a2, affineTransform.rotation, num), false);
						num3++;
						this.sources[i] = value;
					}
				}
				AnimationRuntimeUtils.PassThrough(stream, this.sources[num2]);
				return;
			}
			for (int j = 0; j < this.sources.Length; j++)
			{
				AnimationRuntimeUtils.PassThrough(stream, this.sources[j]);
			}
		}

		internal void UpdateOffsets(int driver)
		{
			driver = Mathf.Clamp(driver, 0, this.sources.Length - 1);
			int num = 0;
			AffineTransform lhs = this.sourceBindTx[driver].Inverse();
			for (int i = 0; i < this.sourceBindTx.Length; i++)
			{
				if (i != driver)
				{
					this.offsetTx[num] = lhs * this.sourceBindTx[i];
					num++;
				}
			}
			this.m_PrevDriverIdx = driver;
		}

		public IntProperty driver;

		public NativeArray<ReadWriteTransformHandle> sources;

		public NativeArray<AffineTransform> sourceBindTx;

		public NativeArray<AffineTransform> offsetTx;

		private int m_PrevDriverIdx;
	}
}
