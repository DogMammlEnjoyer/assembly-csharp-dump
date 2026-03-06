using System;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	internal struct IndirectBufferContext
	{
		public IndirectBufferContext(JobHandle cullingJobHandle)
		{
			this.cullingJobHandle = cullingJobHandle;
			this.bufferState = IndirectBufferContext.BufferState.Pending;
			this.occluderVersion = 0;
			this.subviewMask = 0;
		}

		public bool Matches(IndirectBufferContext.BufferState bufferState, int occluderVersion, int subviewMask)
		{
			return this.bufferState == bufferState && this.occluderVersion == occluderVersion && this.subviewMask == subviewMask;
		}

		public JobHandle cullingJobHandle;

		public IndirectBufferContext.BufferState bufferState;

		public int occluderVersion;

		public int subviewMask;

		public enum BufferState
		{
			Pending,
			Zeroed,
			NoOcclusionTest,
			AllInstancesOcclusionTested,
			OccludedInstancesReTested
		}
	}
}
