using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	internal class GaussianWindow1D_Quaternion : GaussianWindow1d<Quaternion>
	{
		public GaussianWindow1D_Quaternion(float sigma, int maxKernelRadius = 10) : base(sigma, maxKernelRadius)
		{
		}

		protected override Quaternion Compute(int windowPos)
		{
			Quaternion q = new Quaternion(0f, 0f, 0f, 0f);
			Quaternion quaternion = this.m_Data[this.m_CurrentPos];
			Quaternion lhs = Quaternion.Inverse(quaternion);
			for (int i = 0; i < base.KernelSize; i++)
			{
				float num = this.m_Kernel[i];
				Quaternion quaternion2 = lhs * this.m_Data[windowPos];
				if (Quaternion.Dot(Quaternion.identity, quaternion2) < 0f)
				{
					num = -num;
				}
				q.x += quaternion2.x * num;
				q.y += quaternion2.y * num;
				q.z += quaternion2.z * num;
				q.w += quaternion2.w * num;
				if (++windowPos == base.KernelSize)
				{
					windowPos = 0;
				}
			}
			return quaternion * Quaternion.Normalize(q);
		}
	}
}
