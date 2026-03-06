using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Jobs
{
	[BurstCompile]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public struct FloatTweenJob : ITweenJob<float>, IJob
	{
		public TweenJobData<float> jobData { readonly get; set; }

		public void Execute()
		{
			float t = this.jobData.nativeCurve.Evaluate(this.jobData.stateTransitionAmountFloat);
			float to = this.Lerp(this.jobData.stateOriginValue, this.jobData.stateTargetValue, t);
			NativeArray<float> outputData = this.jobData.outputData;
			outputData[0] = this.Lerp(this.jobData.tweenStartValue, to, this.jobData.tweenAmount);
		}

		public float Lerp(float from, float to, float t)
		{
			if (this.IsNearlyEqual(from, to))
			{
				return to;
			}
			return math.lerp(from, to, t);
		}

		public bool IsNearlyEqual(float from, float to)
		{
			return math.distancesq(from, to) < 2.5000003E-07f;
		}
	}
}
