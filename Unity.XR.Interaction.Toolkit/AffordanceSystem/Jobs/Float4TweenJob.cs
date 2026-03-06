using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Jobs
{
	[BurstCompile]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public struct Float4TweenJob : ITweenJob<float4>, IJob
	{
		public TweenJobData<float4> jobData { readonly get; set; }

		public void Execute()
		{
			float t = this.jobData.nativeCurve.Evaluate(this.jobData.stateTransitionAmountFloat);
			float4 to = this.Lerp(this.jobData.stateOriginValue, this.jobData.stateTargetValue, t);
			NativeArray<float4> outputData = this.jobData.outputData;
			outputData[0] = this.Lerp(this.jobData.tweenStartValue, to, this.jobData.tweenAmount);
		}

		public float4 Lerp(float4 from, float4 to, float t)
		{
			if (this.IsNearlyEqual(from, to))
			{
				return to;
			}
			return math.lerp(from, to, t);
		}

		public bool IsNearlyEqual(float4 from, float4 to)
		{
			return math.distancesq(from, to) < 2.5000003E-07f;
		}
	}
}
