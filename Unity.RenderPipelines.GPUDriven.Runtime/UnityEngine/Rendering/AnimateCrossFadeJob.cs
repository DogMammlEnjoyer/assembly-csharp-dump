using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct AnimateCrossFadeJob : IJobParallelFor
	{
		public void Execute(int instanceIndex)
		{
			ref byte ptr = ref this.crossFadeArray.ElementAt(instanceIndex);
			if (ptr == 255)
			{
				return;
			}
			byte b = ptr & 128;
			ptr += (byte)(this.deltaTime / 0.333f * 127f);
			if (b != (ptr + 1 & 128))
			{
				ptr = byte.MaxValue;
			}
		}

		public const int k_BatchSize = 512;

		public const byte k_MeshLODTransitionToLowerLODBit = 128;

		private const byte k_LODFadeOff = 255;

		private const float k_CrossfadeAnimationTimeS = 0.333f;

		[ReadOnly]
		public float deltaTime;

		public UnsafeList<byte> crossFadeArray;
	}
}
