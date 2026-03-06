using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering.Universal
{
	internal struct LayerBatch
	{
		public unsafe void InitRTIds(int index)
		{
			for (int i = 0; i < 4; i++)
			{
				*(ref this.renderTargetUsed.FixedElementField + i) = false;
				*(ref this.renderTargetIds.FixedElementField + (IntPtr)i * 4) = Shader.PropertyToID(string.Format("_LightTexture_{0}_{1}", index, i));
			}
			this.lights = new List<Light2D>();
			this.shadowIndices = new List<int>();
			this.shadowCasters = new List<ShadowCasterGroup2D>();
		}

		public unsafe RenderTargetIdentifier GetRTId(CommandBuffer cmd, RenderTextureDescriptor desc, int index)
		{
			if (!(*(ref this.renderTargetUsed.FixedElementField + index)))
			{
				cmd.GetTemporaryRT(*(ref this.renderTargetIds.FixedElementField + (IntPtr)index * 4), desc, FilterMode.Bilinear);
				*(ref this.renderTargetUsed.FixedElementField + index) = true;
			}
			return new RenderTargetIdentifier(*(ref this.renderTargetIds.FixedElementField + (IntPtr)index * 4));
		}

		public unsafe void ReleaseRT(CommandBuffer cmd)
		{
			for (int i = 0; i < 4; i++)
			{
				if (*(ref this.renderTargetUsed.FixedElementField + i))
				{
					cmd.ReleaseTemporaryRT(*(ref this.renderTargetIds.FixedElementField + (IntPtr)i * 4));
					*(ref this.renderTargetUsed.FixedElementField + i) = false;
				}
			}
		}

		public int startLayerID;

		public int endLayerValue;

		public SortingLayerRange layerRange;

		public LightStats lightStats;

		public bool useNormals;

		[FixedBuffer(typeof(int), 4)]
		private LayerBatch.<renderTargetIds>e__FixedBuffer renderTargetIds;

		[FixedBuffer(typeof(bool), 4)]
		private LayerBatch.<renderTargetUsed>e__FixedBuffer renderTargetUsed;

		public List<Light2D> lights;

		public List<int> shadowIndices;

		public List<ShadowCasterGroup2D> shadowCasters;

		internal int[] activeBlendStylesIndices;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <renderTargetIds>e__FixedBuffer
		{
			public int FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 4)]
		public struct <renderTargetUsed>e__FixedBuffer
		{
			public bool FixedElementField;
		}
	}
}
