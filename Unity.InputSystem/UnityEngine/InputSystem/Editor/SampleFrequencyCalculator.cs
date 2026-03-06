using System;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.Editor
{
	internal struct SampleFrequencyCalculator
	{
		public SampleFrequencyCalculator(float targetFrequency, double realtimeSinceStartup)
		{
			this.targetFrequency = targetFrequency;
			this.m_SampleCount = 0;
			this.frequency = 0f;
			this.m_LastUpdateTime = realtimeSinceStartup;
		}

		public float targetFrequency { readonly get; private set; }

		public float frequency { readonly get; private set; }

		public void ProcessSample(InputEventPtr eventPtr)
		{
			if (eventPtr != null)
			{
				this.m_SampleCount++;
			}
		}

		public bool Update()
		{
			return this.Update(Time.realtimeSinceStartupAsDouble);
		}

		public bool Update(double realtimeSinceStartup)
		{
			double num = realtimeSinceStartup - this.m_LastUpdateTime;
			if (num < 1.0)
			{
				return false;
			}
			this.m_LastUpdateTime = realtimeSinceStartup;
			this.frequency = (float)((double)this.m_SampleCount / num);
			this.m_SampleCount = 0;
			return true;
		}

		private double m_LastUpdateTime;

		private int m_SampleCount;
	}
}
