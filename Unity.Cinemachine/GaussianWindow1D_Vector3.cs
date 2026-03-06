using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	internal class GaussianWindow1D_Vector3 : GaussianWindow1d<Vector3>
	{
		public GaussianWindow1D_Vector3(float sigma, int maxKernelRadius = 10) : base(sigma, maxKernelRadius)
		{
		}

		protected override Vector3 Compute(int windowPos)
		{
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < base.KernelSize; i++)
			{
				vector += this.m_Data[windowPos] * this.m_Kernel[i];
				if (++windowPos == base.KernelSize)
				{
					windowPos = 0;
				}
			}
			return vector;
		}
	}
}
