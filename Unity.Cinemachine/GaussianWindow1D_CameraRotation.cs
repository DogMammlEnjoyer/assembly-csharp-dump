using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	internal class GaussianWindow1D_CameraRotation : GaussianWindow1d<Vector2>
	{
		public GaussianWindow1D_CameraRotation(float sigma, int maxKernelRadius = 10) : base(sigma, maxKernelRadius)
		{
		}

		protected override Vector2 Compute(int windowPos)
		{
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = this.m_Data[this.m_CurrentPos];
			for (int i = 0; i < base.KernelSize; i++)
			{
				Vector2 vector3 = this.m_Data[windowPos] - vector2;
				if (vector3.y > 180f)
				{
					vector3.y -= 360f;
				}
				if (vector3.y < -180f)
				{
					vector3.y += 360f;
				}
				vector += vector3 * this.m_Kernel[i];
				if (++windowPos == base.KernelSize)
				{
					windowPos = 0;
				}
			}
			return vector2 + vector;
		}
	}
}
