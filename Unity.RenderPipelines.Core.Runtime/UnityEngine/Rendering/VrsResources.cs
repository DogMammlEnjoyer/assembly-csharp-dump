using System;

namespace UnityEngine.Rendering
{
	internal class VrsResources : IDisposable
	{
		internal Material visualizationMaterial
		{
			get
			{
				if (this.m_VisualizationMaterial == null)
				{
					this.m_VisualizationMaterial = new Material(this.m_VisualizationShader);
				}
				return this.m_VisualizationMaterial;
			}
		}

		internal VrsResources(VrsRenderPipelineRuntimeResources resources)
		{
			this.InitializeResources(resources);
		}

		~VrsResources()
		{
			this.Dispose();
			GC.SuppressFinalize(this);
		}

		public void Dispose()
		{
			this.DisposeResources();
		}

		private void InitializeResources(VrsRenderPipelineRuntimeResources resources)
		{
			if (!this.InitComputeShader(resources))
			{
				this.DisposeResources();
				return;
			}
			this.m_VisualizationShader = resources.visualizationShader;
			this.conversionLutBuffer = resources.conversionLookupTable.CreateBuffer(false);
			this.visualizationLutBuffer = resources.visualizationLookupTable.CreateBuffer(true);
			this.AllocFragmentSizeBuffer();
		}

		private void DisposeResources()
		{
			GraphicsBuffer graphicsBuffer = this.conversionLutBuffer;
			if (graphicsBuffer != null)
			{
				graphicsBuffer.Dispose();
			}
			this.conversionLutBuffer = null;
			GraphicsBuffer graphicsBuffer2 = this.visualizationLutBuffer;
			if (graphicsBuffer2 != null)
			{
				graphicsBuffer2.Dispose();
			}
			this.visualizationLutBuffer = null;
			GraphicsBuffer graphicsBuffer3 = this.validatedShadingRateFragmentSizeBuffer;
			if (graphicsBuffer3 != null)
			{
				graphicsBuffer3.Dispose();
			}
			this.validatedShadingRateFragmentSizeBuffer = null;
			this.m_VisualizationShader = null;
			this.m_VisualizationMaterial = null;
		}

		private void AllocFragmentSizeBuffer()
		{
			uint[] array = new uint[Vrs.shadingRateFragmentSizeCount];
			ShadingRateFragmentSize shadingRateFragmentSize = ShadingRateFragmentSize.FragmentSize1x1;
			uint value = (uint)ShadingRateInfo.QueryNativeValue(shadingRateFragmentSize);
			foreach (ShadingRateFragmentSize shadingRateFragmentSize2 in ShadingRateInfo.availableFragmentSizes)
			{
				Array.Fill<uint>(array, value, (int)shadingRateFragmentSize, shadingRateFragmentSize2 - shadingRateFragmentSize + 1);
				shadingRateFragmentSize = shadingRateFragmentSize2;
				value = (uint)ShadingRateInfo.QueryNativeValue(shadingRateFragmentSize);
			}
			Array.Fill<uint>(array, value, (int)shadingRateFragmentSize, (int)(ShadingRateFragmentSize.FragmentSize4x4 - shadingRateFragmentSize + 1));
			this.validatedShadingRateFragmentSizeBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, array.Length, 4);
			this.validatedShadingRateFragmentSizeBuffer.SetData(array);
		}

		private bool InitComputeShader(VrsRenderPipelineRuntimeResources resources)
		{
			if (!ShadingRateInfo.supportsPerImageTile)
			{
				return false;
			}
			if (!SystemInfo.supportsComputeShaders)
			{
				return false;
			}
			this.tileSize = ShadingRateInfo.imageTileSize;
			if (this.tileSize.x != this.tileSize.y || (this.tileSize.x != 8 && this.tileSize.x != 16 && this.tileSize.x != 32))
			{
				Debug.LogError(string.Format("VRS unsupported tile size: {0}x{1}.", this.tileSize.x, this.tileSize.y));
				return false;
			}
			ComputeShader computeShader = resources.textureComputeShader;
			if (computeShader != null && computeShader.keywordSpace.keywordCount <= 0U)
			{
				this.textureReduceKernel = -1;
				this.textureCopyKernel = -1;
				return false;
			}
			this.textureComputeShader = resources.textureComputeShader;
			this.textureComputeShader.EnableKeyword(string.Format("{0}{1}", "VRS_TILE_SIZE_", this.tileSize.x));
			this.textureReduceKernel = VrsResources.TryFindKernel(this.textureComputeShader, "TextureReduce");
			this.textureCopyKernel = VrsResources.TryFindKernel(this.textureComputeShader, "TextureReduce");
			return this.textureReduceKernel != -1 && this.textureCopyKernel != -1;
		}

		private static int TryFindKernel(ComputeShader computeShader, string name)
		{
			if (!computeShader.HasKernel(name))
			{
				return -1;
			}
			return computeShader.FindKernel(name);
		}

		internal ProfilingSampler conversionProfilingSampler = new ProfilingSampler("VrsConversion");

		internal ProfilingSampler visualizationProfilingSampler = new ProfilingSampler("VrsVisualization");

		internal GraphicsBuffer conversionLutBuffer;

		internal GraphicsBuffer visualizationLutBuffer;

		internal ComputeShader textureComputeShader;

		internal int textureReduceKernel = -1;

		internal int textureCopyKernel = -1;

		internal Vector2Int tileSize;

		internal GraphicsBuffer validatedShadingRateFragmentSizeBuffer;

		private Shader m_VisualizationShader;

		private Material m_VisualizationMaterial;
	}
}
