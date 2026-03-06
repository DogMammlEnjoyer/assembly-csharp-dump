using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Jobs
{
	[BurstCompile]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public struct ColorTweenJob : ITweenJob<Color>, IJob
	{
		public TweenJobData<Color> jobData { readonly get; set; }

		public byte colorBlendMode { readonly get; set; }

		public float colorBlendAmount { readonly get; set; }

		public void Execute()
		{
			float t = this.jobData.nativeCurve.Evaluate(this.jobData.stateTransitionAmountFloat);
			Color newValue = this.Lerp(this.jobData.stateOriginValue, this.jobData.stateTargetValue, t);
			Color to = this.ProcessTargetAffordanceValue(this.jobData.initialValue, newValue);
			NativeArray<Color> outputData = this.jobData.outputData;
			outputData[0] = this.Lerp(this.jobData.tweenStartValue, to, this.jobData.tweenAmount);
		}

		private Color ProcessTargetAffordanceValue(Color initialValue, Color newValue)
		{
			Color result = newValue;
			switch (this.colorBlendMode)
			{
			case 1:
			{
				float colorBlendAmount = this.colorBlendAmount;
				result = new Color(initialValue.r + newValue.r * colorBlendAmount, initialValue.g + newValue.g * colorBlendAmount, initialValue.b + newValue.b * colorBlendAmount, initialValue.a + newValue.a * colorBlendAmount);
				break;
			}
			case 2:
				result = this.Lerp(initialValue, newValue, this.colorBlendAmount);
				break;
			}
			return result;
		}

		public Color Lerp(Color from, Color to, float t)
		{
			if (this.IsNearlyEqual(from, to))
			{
				return to;
			}
			return math.lerp(from, to, t);
		}

		public bool IsNearlyEqual(Color from, Color to)
		{
			return math.distancesq(from, to) < 2.5000003E-07f;
		}
	}
}
