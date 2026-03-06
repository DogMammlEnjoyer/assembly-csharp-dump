using System;

namespace UnityEngine.Rendering
{
	internal class ProbeVolumeDebug : IDebugData
	{
		public ProbeVolumeDebug()
		{
			this.Init();
		}

		private void Init()
		{
			this.drawProbes = false;
			this.drawBricks = false;
			this.drawCells = false;
			this.realtimeSubdivision = false;
			this.subdivisionCellUpdatePerFrame = 4;
			this.subdivisionDelayInSeconds = 1f;
			this.probeShading = DebugProbeShadingMode.SH;
			this.probeSize = 0.3f;
			this.subdivisionViewCullingDistance = 500f;
			this.probeCullingDistance = 200f;
			this.maxSubdivToVisualize = 7;
			this.minSubdivToVisualize = 0;
			this.exposureCompensation = 0f;
			this.drawProbeSamplingDebug = false;
			this.probeSamplingDebugSize = 0.3f;
			this.drawVirtualOffsetPush = false;
			this.offsetSize = 0.025f;
			this.freezeStreaming = false;
			this.displayCellStreamingScore = false;
			this.displayIndexFragmentation = false;
			this.otherStateIndex = 0;
			this.autoDrawProbes = true;
			this.isolationProbeDebug = true;
			this.visibleLayers = byte.MaxValue;
		}

		public Action GetReset()
		{
			return delegate()
			{
				this.Init();
			};
		}

		public bool drawProbes;

		public bool drawBricks;

		public bool drawCells;

		public bool realtimeSubdivision;

		public int subdivisionCellUpdatePerFrame = 4;

		public float subdivisionDelayInSeconds = 1f;

		public DebugProbeShadingMode probeShading;

		public float probeSize = 0.3f;

		public float subdivisionViewCullingDistance = 500f;

		public float probeCullingDistance = 200f;

		public int maxSubdivToVisualize = 7;

		public int minSubdivToVisualize;

		public float exposureCompensation;

		public bool drawProbeSamplingDebug;

		public float probeSamplingDebugSize = 0.3f;

		public bool debugWithSamplingNoise;

		public uint samplingRenderingLayer;

		public bool drawVirtualOffsetPush;

		public float offsetSize = 0.025f;

		public bool freezeStreaming;

		public bool displayCellStreamingScore;

		public bool displayIndexFragmentation;

		public int otherStateIndex;

		public bool verboseStreamingLog;

		public bool debugStreaming;

		public bool autoDrawProbes = true;

		public bool isolationProbeDebug = true;

		public byte visibleLayers;

		public static Vector3 currentOffset;

		internal static int s_ActiveAdjustmentVolumes;
	}
}
