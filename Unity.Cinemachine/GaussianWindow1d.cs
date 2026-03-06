using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	internal abstract class GaussianWindow1d<T>
	{
		public float Sigma { get; private set; }

		public int KernelSize
		{
			get
			{
				return this.m_Kernel.Length;
			}
		}

		private void GenerateKernel(float sigma, int maxKernelRadius)
		{
			int num = Math.Min(maxKernelRadius, Mathf.FloorToInt(Mathf.Abs(sigma) * 2.5f));
			this.m_Kernel = new float[2 * num + 1];
			if (num == 0)
			{
				this.m_Kernel[0] = 1f;
			}
			else
			{
				float num2 = 0f;
				for (int i = -num; i <= num; i++)
				{
					this.m_Kernel[i + num] = (float)(Math.Exp((double)((float)(-(float)(i * i)) / (2f * sigma * sigma))) / (6.283185307179586 * (double)sigma * (double)sigma));
					num2 += this.m_Kernel[i + num];
				}
				for (int j = -num; j <= num; j++)
				{
					this.m_Kernel[j + num] /= num2;
				}
			}
			this.Sigma = sigma;
		}

		protected abstract T Compute(int windowPos);

		public GaussianWindow1d(float sigma, int maxKernelRadius = 10)
		{
			this.GenerateKernel(sigma, maxKernelRadius);
			this.m_Data = new T[this.KernelSize];
			this.m_CurrentPos = -1;
		}

		public void Reset()
		{
			this.m_CurrentPos = -1;
		}

		public bool IsEmpty()
		{
			return this.m_CurrentPos < 0;
		}

		public void AddValue(T v)
		{
			if (this.m_CurrentPos < 0)
			{
				for (int i = 0; i < this.KernelSize; i++)
				{
					this.m_Data[i] = v;
				}
				this.m_CurrentPos = Mathf.Min(1, this.KernelSize - 1);
			}
			this.m_Data[this.m_CurrentPos] = v;
			int num = this.m_CurrentPos + 1;
			this.m_CurrentPos = num;
			if (num == this.KernelSize)
			{
				this.m_CurrentPos = 0;
			}
		}

		public T Filter(T v)
		{
			if (this.KernelSize < 3)
			{
				return v;
			}
			this.AddValue(v);
			return this.Value();
		}

		public T Value()
		{
			return this.Compute(this.m_CurrentPos);
		}

		public int BufferLength
		{
			get
			{
				return this.m_Data.Length;
			}
		}

		public void SetBufferValue(int index, T value)
		{
			this.m_Data[index] = value;
		}

		public T GetBufferValue(int index)
		{
			return this.m_Data[index];
		}

		protected T[] m_Data;

		protected float[] m_Kernel;

		protected int m_CurrentPos = -1;
	}
}
